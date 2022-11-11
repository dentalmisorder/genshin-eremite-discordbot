using Newtonsoft.Json;

namespace DiscordBot.GenshinData
{
    public class PropMap
    {
        [JsonProperty("type")]
        public int type;
        [JsonProperty("ival")]
        public int ival;
        [JsonProperty("val")]
        public int val;
    }
}