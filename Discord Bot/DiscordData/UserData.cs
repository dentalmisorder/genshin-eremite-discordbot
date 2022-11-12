
using Newtonsoft.Json;
using System;

namespace DiscordBot.DiscordData
{
    public class UserData
    {
        [JsonProperty("userId")]
        public ulong userId; //snowflake id  prob

        [JsonProperty("wallet")]
        public DiscordWallet wallet = new DiscordWallet();

        [JsonProperty("recruitSystemEnrolled")]
        public int timesEremitesRecruitSystemEnrolled = 0;

        [JsonProperty("timesWelkinWon")]
        public int timesWelkinWon = 0;

        [JsonProperty("timeLastTravel")]
        public DateTime timeLastTravel = DateTime.MinValue;
    }
}
