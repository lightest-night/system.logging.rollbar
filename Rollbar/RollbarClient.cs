using System;
using System.Diagnostics;
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

                var apiClient = _apiClientFactory.Create(rollbarConfig.BaseUrl);
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
            
            if (log.Exception is Exception || log.Exception?.GetType().BaseType == typeof(Exception))
            {
                var ex = (Exception)log.Exception;
                
                var stackTrace = new StackTrace(ex, true);
                var frames = stackTrace.GetFrames()?.Select(frame => new RollbarFrame
                {
                    Filename = frame.GetFileName(),
                    LineNumber = frame.GetFileLineNumber(),
                    ColumnNumber = frame.GetFileColumnNumber(),
                    Method = frame.GetMethod().Name,
                    ClassName = frame.GetMethod().DeclaringType?.AssemblyQualifiedName,
                    Arguments = frame.GetMethod().GetParameters().Select(arg => arg.Name)
                });

                trace = new RollbarTrace
                {
                    Frames = frames,
                    Exception = new RollbarException
                    {
                        Class = ex.Source,
                        Message = ex.Message,
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