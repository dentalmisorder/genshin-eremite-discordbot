using System;
using Newtonsoft.Json;
using System.Text;

namespace DiscordBot.GenshinData
{
    [Serializable]
    public class GenshinUserData
    {
        [JsonProperty("playerInfo")]
        public PlayerInfo playerInfo;
        [JsonProperty("avatarInfoList")]
        public AvatarInfoList avatarInfoList;
        [JsonProperty("ttl")]
        public int ttl;
    }
}
