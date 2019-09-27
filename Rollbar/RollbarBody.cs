using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace LightestNight.System.Logging.Rollbar
{
    public class RollbarBody
    {
        /// <summary>
        /// The Log Message
        /// </summary>
        public RollbarMessage Message { get; set; }
        
        /// <summary>
        /// The Trace
        /// </summary>
        public RollbarTrace Trace { get; set; }

        /// <summary>
        /// The severity level. One of: "critical", "error", "warning", "info", "debug"
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public RollbarLevel Level { get; set; } = RollbarLevel.None;

        /// <summary>
        /// When this occurred, as a unix timestamp.
        /// </summary>
        public long Timestamp { get; set; }

        /// <summary>
        /// A string, up to 40 characters, describing the version of the application code
        /// Rollbar understands these formats:
        /// - semantic version (i.e. "2.1.12")
        /// - integer (i.e. "45")
        /// - git SHA (i.e. "3da541559918a808c2402bba5012f6c60b27661c")
        /// </summary>
        [JsonProperty(PropertyName = "code_version")]
        public string CodeVersion { get; set; } = string.Empty;

        /// <summary>
        /// The platform on which this occurred. Meaningful platform names:
        /// "browser", "android", "ios", "flash", "client", "heroku", "google-app-engine"
        /// If this is a client-side event, be sure to specify the platform and use a post_client_item access token.
        /// </summary>
        public string Platform { get; set; } = string.Empty;

        /// <summary>
        /// The name of the language your code is written in.
        /// This can affect the order of the frames in the stack trace. The following languages set the most
        /// recent call first - 'ruby', 'javascript', 'php', 'java', 'objective-c', 'lua'
        /// It will also change the way the individual frames are displayed, with what is most consistent with
        /// users of the language.
        /// </summary>
        public string Language { get; set; } = "C#";

        /// <summary>
        /// The name of the framework your code uses
        /// </summary>
        public string Framework { get; set; } = string.Empty;

        /// <summary>
        /// A string, up to 36 characters, that uniquely identifies this occurrence.
        /// While it can now be any latin1 string, this may change to be a 16 byte field in the future.
        /// We recommend using a UUID4 (16 random bytes).
        /// The UUID space is unique to each project, and can be used to look up an occurrence later.
        /// It is also used to detect duplicate requests. If you send the same UUID in two payloads, the second
        /// one will be discarded.
        /// While optional, it is recommended that all clients generate and provide this field
        /// </summary>
        public Guid Uuid { get; set; } = Guid.NewGuid();

        /// <summary>
        /// A string that will be used as the title of the Item occurrences will be grouped into.
        /// Max length 255 characters.
        /// If omitted, we'll determine this on the backend.
        /// </summary>
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// Describes the library used to send this event.
        /// </summary>
        public RollbarNotifier Notifier { get; set; } = new RollbarNotifier();
    }
}