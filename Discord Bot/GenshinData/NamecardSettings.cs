using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace DiscordBot.GenshinData
{
    [Serializable]
    public class NamecardSettings
    {
        [JsonProperty("nameTextMapHash")]
        public int nameTextMapHash;

        [JsonProperty("icon")]
        public string icon;

        [JsonProperty("picPath")]
        public List<string> picPath = new List<string>();

        [JsonProperty("rankLevel")]
        public int rankLevel;

        [JsonProperty("materialType")]
        public string materialType;
    }
}