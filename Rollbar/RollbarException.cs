namespace LightestNight.System.Logging.Rollbar
{
    public class RollbarException
    {
        /// <summary>
        /// The exception class name
        /// </summary>
        public string Class { get; set; }
        
        /// <summary>
        /// The exception message, as a string
        /// </summary>
        public string Message { get; set; }
        
        /// <summary>
        /// An alternate human-readable string describing the exception
        /// </summary>
        /// <remarks>
        /// Usually the original exception message will have been machine-generated; you can use this to send something custom
        /// </remarks>
        public string Description { get; set; }
    }
}