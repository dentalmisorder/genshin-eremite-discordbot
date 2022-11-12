
using Newtonsoft.Json;

namespace DiscordBot.DiscordData
{
    public class DiscordWallet
    {
        [JsonProperty("mora")]
        public int mora = 0;
        [JsonProperty("primogems")]
        public int primogems = 0;
    }
}
