namespace LightestNight.System.Logging.Rollbar
{
    public class RollbarData
    {
        /// <summary>
        /// The name of the environment in which this occurrence was seen.
        /// </summary>
        /// <remarks>
        /// A string up to 255 characters. For best results, use "production" or "prod" for your production environment.
        /// You don't need to configure anything in the Rollbar UI for new environment names; we'll detect them automatically.
        /// </remarks>
        public string Environment { get; set; } = string.Empty;
        
        /// <summary>
        /// The main data being sent. It can either be a message, an exception, or a crash report.
        /// </summary>
        public RollbarBody Body { get; set; } = new RollbarBody();
    }
}