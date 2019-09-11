namespace LightestNight.System.Logging.Rollbar
{
    public class RollbarConfig
    {
        /// <summary>
        /// The Base Url to connect to Rollbar using
        /// </summary>
        public string BaseUrl { get; set; } = "https://api.rollbar.com/api/1";

        /// <summary>
        /// The Rollbar AccessToken for the project to log to
        /// </summary>
        public string AccessToken { get; set; }
    }
}