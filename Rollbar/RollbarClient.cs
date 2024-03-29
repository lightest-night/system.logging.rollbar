using System;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Threading.Tasks;
using LightestNight.System.Api;
using LightestNight.System.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace LightestNight.System.Logging.Rollbar
{
    public class RollbarClient : IRollbarClient
    {
        private readonly IApiClientFactory _apiClientFactory;
        private readonly ConfigurationManager _configManager;

        public RollbarClient(IApiClientFactory apiClientFactory, ConfigurationManager configManager)
        {
            _apiClientFactory = apiClientFactory;
            _configManager = configManager;
        }
        
        public async Task Log(LogData logData)
        {
            try
            {
                var rollbarConfig = _configManager.Bind<RollbarConfig>();
                var request = new ApiRequest("item/")
                {
                    UseMachineToken = false,
                    Body = GeneratePayload(logData, rollbarConfig)
                };

                var apiClient = _apiClientFactory.Create(rollbarConfig.BaseUrl)
                    .SetSerializerSettings(settings => settings.NullValueHandling = NullValueHandling.Ignore);
                    
                await apiClient.Post(request);
            }
            catch (Exception ex)
            {
                Console.WriteLine(JsonConvert.SerializeObject(ex));
                throw;
            }
        }
        
        private static RollbarPayload GeneratePayload(LogData log, RollbarConfig config)
        {
            var os = RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
                ? OSPlatform.Windows.ToString()
                : RuntimeInformation.IsOSPlatform(OSPlatform.Linux)
                    ? OSPlatform.Linux.ToString()
                    : RuntimeInformation.IsOSPlatform(OSPlatform.OSX)
                        ? OSPlatform.OSX.ToString()
                        : string.Empty;

            var framework = Assembly.GetEntryAssembly()?.GetCustomAttribute<TargetFrameworkAttribute>()?.FrameworkDisplayName;

            var level = RollbarLevel.None;
            switch (log.Severity)
            {
                case LogLevel.Critical:
                    level = RollbarLevel.Critical;
                    break;
                case LogLevel.Debug:
                    level = RollbarLevel.Debug;
                    break;
                case LogLevel.Error:
                    level = RollbarLevel.Error;
                    break;
                case LogLevel.Information:
                case LogLevel.Trace:
                    level = RollbarLevel.Info;
                    break;
                case LogLevel.Warning:
                    level = RollbarLevel.Warning;
                    break;
            }

            RollbarMessage message = null;
            RollbarTrace trace = null;
            
            if (log.Exception != null)
            {
                var ex = log.Exception;

                var frames = ex.Frames?.Select(frame => new RollbarFrame
                {
                    Filename = frame.Filename,
                    LineNumber = frame.LineNumber,
                    ColumnNumber = frame.ColumnNumber,
                    Method = frame.Method,
                    ClassName = frame.ClassName,
                    Arguments = frame.Arguments
                });

                trace = new RollbarTrace
                {
                    Frames = frames,
                    Exception = new RollbarException
                    {
                        Class = ex.Exception.Source,
                        Message = ex.Exception.Message,
                        Description = log.Message
                    }
                };
            }
            else
            {
                message = new RollbarMessage
                {
                    Body = log.Message,
                    Metadata = log.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public).ToDictionary(prop => prop.Name, prop => prop.GetValue(log, null)?.ToString())
                };
            }

            return new RollbarPayload
            {
                AccessToken = config.AccessToken,
                Data = new RollbarData
                {
                    Environment = Environment.GetEnvironmentVariable("STAGE") ?? "test",
                    Body = new RollbarBody
                    {
                        Title = log.Title,
                        Uuid = Guid.NewGuid(),
                        Level = level,
                        Timestamp = ((DateTimeOffset)DateTime.UtcNow).ToUnixTimeSeconds(),
                        Platform = os,
                        Framework = framework,
                        Message = message,
                        Trace = trace,
                        Notifier = new RollbarNotifier
                        {
                            Name = "Lightest Night"
                        }
                    }
                }
            };
        }
    }
}