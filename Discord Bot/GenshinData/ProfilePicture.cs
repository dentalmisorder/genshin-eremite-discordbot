using Newtonsoft.Json;

namespace DiscordBot.GenshinData
{
    public class ProfilePicture
    {
        [JsonProperty("avatarId")]
        public int avatarId;
    }
}