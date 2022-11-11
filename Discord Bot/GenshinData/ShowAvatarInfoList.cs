using Newtonsoft.Json;

namespace DiscordBot.GenshinData
{
    public class ShowAvatarInfoList
    {
        [JsonProperty("avatarId")]
        public int avatarId;
        [JsonProperty("level")]
        public int level;
        [JsonProperty("costumeId")]
        public int costumeId;
    }
}