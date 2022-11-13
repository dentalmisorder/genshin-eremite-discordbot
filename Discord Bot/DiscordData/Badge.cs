using Newtonsoft.Json;

namespace DiscordBot.DiscordData
{
    public class Badge
    {
        [JsonProperty("badgeName")]
        public string badgeName = string.Empty;

        [JsonProperty("badgeDescription")]
        public string badgeDescription = string.Empty;

        [JsonProperty("badgeDescription")]
        public string badgeEmoji = ":heart_on_fire:";
    }
}