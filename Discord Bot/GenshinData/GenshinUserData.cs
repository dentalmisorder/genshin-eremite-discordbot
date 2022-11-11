using System;
using Newtonsoft.Json;
using System.Text;
using System.Collections.Generic;

namespace DiscordBot.GenshinData
{
    [Serializable]
    public class GenshinUserData
    {
        [JsonProperty("playerInfo")]
        public PlayerInfo playerInfo;
        [JsonProperty("avatarInfoList")]
        public List<AvatarInfoList> avatarInfoList = new List<AvatarInfoList>();
        [JsonProperty("ttl")]
        public long ttl;
    }
}
