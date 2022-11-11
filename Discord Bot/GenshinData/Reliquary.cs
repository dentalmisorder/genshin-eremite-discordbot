using Newtonsoft.Json;
using System.Collections.Generic;

namespace DiscordBot.GenshinData
{
    public class Reliquary
    {
        [JsonProperty("level")]
        public int level;
        [JsonProperty("mainPropId")]
        public int mainPropId;
        [JsonProperty("appendPropIdList")]
        public List<int> appendPropIdList = new List<int>();
    }
}