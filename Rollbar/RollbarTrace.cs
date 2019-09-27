using System.Collections.Generic;

namespace LightestNight.System.Logging.Rollbar
{
    public class RollbarTrace
    {
        /// <summary>
        /// A list of stack frames, ordered such that the most recent call is last in the list.
        /// </summary>
        public IEnumerable<RollbarFrame> Frames { get; set; }
        
        /// <summary>
        /// An object describing the exception instance.
        /// </summary>
        public RollbarException Exception { get; set; }
    }
}