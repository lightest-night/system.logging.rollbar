using System.Collections.Generic;
using Newtonsoft.Json;

namespace LightestNight.System.Logging.Rollbar
{
    public class RollbarFrame
    {
        /// <summary>
        ///  The filename including its full path.
        /// </summary>
        /// <example>/Users/brian/www/mox/mox/views/project.py</example>
        public string Filename { get; set; }
        
        /// <summary>
        /// The line number as an integer
        /// </summary>
        [JsonProperty(PropertyName = "lineno")]
        public int LineNumber { get; set; }
        
        /// <summary>
        /// The column number as an integer
        /// </summary>
        [JsonProperty(PropertyName = "colno")]
        public int ColumnNumber { get; set; }
        
        /// <summary>
        /// The method or function name
        /// </summary>
        public string Method { get; set; }
        
        /// <summary>
        /// The line of code
        /// </summary>
        public string Code { get; set; }
        
        /// <summary>
        /// A string containing the class name.
        /// </summary>
        /// <remarks>Used in the UI when the payload's top-level "language" key has the value "java"</remarks>
        [JsonProperty(PropertyName = "class_name")]
        public string ClassName { get; set; }
        
        /// <summary>
        /// Additional code before and after the "code" line
        /// </summary>
        public RollbarContext Context { get; set; }
        
        /// <summary>
        /// List of the names of the arguments to the method/function call.
        /// </summary>
        [JsonProperty(PropertyName = "argspec")]
        public IEnumerable<string> Arguments { get; set; }
    }
}