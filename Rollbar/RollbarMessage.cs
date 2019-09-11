using System.Collections.Generic;

namespace LightestNight.System.Logging.Rollbar
{
    public class RollbarMessage
    {
        /// <summary>
        /// The primary message text, as a string
        /// </summary>
        public string Body { get; set; } = string.Empty;
        
        /// <summary>
        /// Arbitrary keys of metadata. Their values can be any valid JSON.
        /// </summary>
        public IDictionary<string, string> Metadata { get; set; } = new Dictionary<string, string>();
    }
}