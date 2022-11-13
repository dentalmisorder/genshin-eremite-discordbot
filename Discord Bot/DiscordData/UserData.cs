
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace DiscordBot.DiscordData
{
    public class UserData
    {
        [JsonProperty("userId")]
        public ulong userId; //snowflake id  prob

        [JsonProperty("currentEquippedCharacter")]
        public Character currentEquippedCharacter = null;

        [JsonProperty("wallet")]
        public DiscordWallet wallet = new DiscordWallet(); //users money

        [JsonProperty("characters")]
        public List<Character> characters = new List<Character>(); //users inventory

        [JsonProperty("badges")]
        public List<Badge> badges = new List<Badge>(); //player badges for future mini-games/etc.

        [JsonProperty("recruitSystemEnrolled")]
        public int timesEremitesRecruitSystemEnrolled = 0;

        [JsonProperty("timesWelkinWon")]
        public int timesWelkinWon = 0;

        [JsonProperty("timeLastTravel")]
        public DateTime timeLastTravel = DateTime.MinValue;
    }
}
