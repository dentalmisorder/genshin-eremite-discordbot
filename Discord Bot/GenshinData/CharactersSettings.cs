using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace DiscordBot.GenshinData
{
    [Serializable]
    public class CharactersSettings
    {
        [JsonProperty("Element")]
        public string element;

        [JsonProperty("Consts")]
        public List<string> consts = new List<string>();

        [JsonProperty("SkillOrder")]
        public List<long> skillOrder = new List<long>();

        [JsonProperty("Skills")]
        public Dictionary<string, string> skills = new Dictionary<string, string>();

        [JsonProperty("ProudMap")]
        public Dictionary<string, long> proudMap = new Dictionary<string, long>();

        [JsonProperty("NameTextMapHash")]
        public long nameTextMapHash;

        [JsonProperty("SideIconName")]
        public string sideIconName;

        [JsonProperty("QualityType")]
        public string qualityType;
    }
}