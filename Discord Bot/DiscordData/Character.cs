using Newtonsoft.Json;

namespace DiscordBot.DiscordData
{
    public class Character
    {
        [JsonProperty("characterName")]
        public string characterName = string.Empty;

        [JsonProperty("starsRarity")]
        public int starsRarity = 3;

        [JsonProperty("imageAkashaBannerPath")]
        public string imageAkashaBannerPath = string.Empty;

        [JsonProperty("imagePullBannerPath")]
        public string imagePullBannerPath = string.Empty;

        [JsonProperty("perkStat")]
        public int perkStat;

        [JsonProperty("perkInfo")]
        public string perkInfo = string.Empty;

        [JsonProperty("shouldBeDestroyed")]
        public bool shouldBeDestroyed = false;

        [JsonProperty("shouldBeDestroyedOnEnroll")]
        public bool shouldBeDestroyedOnEnroll = false;
    }
}
