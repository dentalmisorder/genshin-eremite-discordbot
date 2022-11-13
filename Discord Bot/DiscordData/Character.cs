using Newtonsoft.Json;

namespace DiscordBot.DiscordData
{
    public class Character
    {
        [JsonProperty("characterName")]
        public string characterName = string.Empty;

        [JsonProperty("imageIconPath")]
        public string imageIconPath = string.Empty;

        [JsonProperty("characterDescriptionPerk")]
        public string descriptionPerk = string.Empty;

        [JsonProperty("perkStat")]
        public Perk perkStat;

        [JsonProperty("perkInfo")]
        public string perkInfo;
    }
}
