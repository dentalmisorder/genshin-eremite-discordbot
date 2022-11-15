using Newtonsoft.Json;

namespace DiscordBot.GenshinData
{
    public class EremiteRecruit
    {
        [JsonProperty("username")]
        public string username;
        [JsonProperty("clientId")]
        public ulong clientId;
        [JsonProperty("uid")]
        public int uid;

        public EremiteRecruit(string username, ulong client, int uid)
        {
            this.username = username;
            clientId = client;
            this.uid = uid;
        }
    }
}
