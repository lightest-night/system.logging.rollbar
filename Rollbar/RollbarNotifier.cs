namespace LightestNight.System.Logging.Rollbar
{
    public class RollbarNotifier
    {
        /// <summary>
        /// Name of the library
        /// </summary>
        public string Name { get; set; } = string.Empty;
        
        /// <summary>
        /// Library version string
        /// </summary>
        public string Version { get; set; } = string.Empty;
    }
}