using Newtonsoft.Json;

namespace DiscordBot
{
    public class JsonConfig
    {
        [JsonProperty("token")]
        public string Token { get; protected set; }

        [JsonProperty("prefix")]
        public string Prefix { get; protected set; }
    }
}
