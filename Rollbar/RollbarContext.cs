using System.Collections.Generic;

namespace LightestNight.System.Logging.Rollbar
{
    public class RollbarContext
    {
        /// <summary>
        /// List of lines of code before the "code" line
        /// </summary>
        public IEnumerable<string> Pre { get; set; }
        
        /// <summary>
        /// List of lines of code after the "code" line
        /// </summary>
        public IEnumerable<string> Post { get; set; }
    }
}