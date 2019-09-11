using Newtonsoft.Json;

namespace LightestNight.System.Logging.Rollbar
{
    public class RollbarPayload
    {
        /// <summary>An access token with scope "post_server_item" or "post_client_item".</summary>
        /// <remarks>
        /// A post_client_item token must be used if the "platform" is "browser", "android", "ios", "flash", or "client"
        /// A post_server_item token should be used for other platforms.
        /// </remarks>
        [JsonProperty(PropertyName = "access_token")]
        public string AccessToken { get; set; } = string.Empty;
        
        /// <summary>
        /// The Rollbar Data object
        /// </summary>
        public RollbarData Data { get; set; } = new RollbarData();
    }
}