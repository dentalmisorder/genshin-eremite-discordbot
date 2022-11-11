using Newtonsoft.Json;

namespace DiscordBot.GenshinData
{
    public class Weapon
    {
        [JsonProperty("level")]
        public int level;
        [JsonProperty("promoteLevel")]
        public int promoteLevel;
        [JsonProperty("affixMap")]
        public int affixMap;
    }
}