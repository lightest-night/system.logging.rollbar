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
        private readonly IApiClient _apiClient;
        private readonly ConfigurationManager _configManager;

        public RollbarClient(IApiClient apiClient, ConfigurationManager configManager)
        {
            _apiClient = apiClient;
            _configManager = configManager;
        }
        
        public async Task Log(LogData logData)
        {
            try
            {
                var request = new ApiRequest("item/")
                {
                    UseMachineToken = false,
                    Body = GeneratePayload(logData)
                };

                await _apiClient.Post(request);
            }
            catch (Exception ex)
            {
                Console.WriteLine(JsonConvert.SerializeObject(ex));
                throw;
            }
        }
        
        private RollbarPayload GeneratePayload(LogData log)
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

            return new RollbarPayload
            {
                AccessToken = _configManager.Bind<RollbarConfig>().AccessToken,
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
                        Message = new RollbarMessage
                        {
                            Body = log.Message,
                            Metadata = log.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public).ToDictionary(prop => prop.Name, prop => prop.GetValue(log, null)?.ToString())
                        },
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