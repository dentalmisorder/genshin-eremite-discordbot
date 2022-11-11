using Newtonsoft.Json;
using System.Collections.Generic;

namespace DiscordBot.GenshinData
{
    public class Weapon
    {
        [JsonProperty("level")]
        public int level;
        [JsonProperty("promoteLevel")]
        public int promoteLevel;
        [JsonProperty("affixMap")]
        public Dictionary<int, int> affixMap = new Dictionary<int, int>();
    }
}