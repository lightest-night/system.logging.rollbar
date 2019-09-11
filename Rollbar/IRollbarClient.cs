using System.Threading.Tasks;

namespace LightestNight.System.Logging.Rollbar
{
    public interface IRollbarClient
    {
        /// <summary>
        /// Logs the given <see cref="LogData" /> object to Rollbar
        /// </summary>
        /// <param name="logData">The <see cref="LogData" /> object to log</param>
        Task Log(LogData logData);
    }
}