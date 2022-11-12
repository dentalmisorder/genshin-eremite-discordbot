using Newtonsoft.Json;

namespace DiscordBot.GenshinData
{
    public class EremiteRecruit
    {
        [JsonProperty("clientId")]
        public ulong clientId;
        [JsonProperty("uid")]
        public int uid;

        public EremiteRecruit(ulong client, int uid)
        {
            clientId = client;
            this.uid = uid;
        }
    }
}
