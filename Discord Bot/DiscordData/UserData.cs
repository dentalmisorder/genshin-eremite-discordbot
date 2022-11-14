
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace DiscordBot.DiscordData
{
    public class UserData
    {
        [JsonProperty("userId")]
        public ulong userId; //snowflake id  prob

        [JsonProperty("username")]
        public string username = string.Empty;

        [JsonProperty("currentEquippedCharacter")]
        public Character currentEquippedCharacter = null;

        [JsonProperty("wallet")]
        public DiscordWallet wallet = new DiscordWallet(); //users money

        [JsonProperty("characters")]
        public List<Character> characters = new List<Character>(); //users inventory

        [JsonProperty("badges")]
        public List<Badge> badges = new List<Badge>(); //player badges for future mini-games/etc.

        [JsonProperty("timeLastTravel")]
        public DateTime timeLastTravel = DateTime.Now.AddDays(-5);

        [JsonProperty("timeLastTeapotVisit")]
        public DateTime timeLastTeapotVisit = DateTime.Now.AddDays(-5);

        [JsonProperty("timesWelkinWon")]
        public int timesWelkinWon = 0;

        [JsonProperty("timesPulled")]
        public int timesPulled = 0;

        [JsonProperty("timesTraveled")]
        public int timesTraveled = 0;

        [JsonProperty("timesTeapotVisited")]
        public int timesTeapotVisited = 0;

        [JsonProperty("recruitSystemEnrolled")]
        public int timesEremitesRecruitSystemEnrolled = 0;


        public void AddPulledCharacter(Character character)
        {
            Character duplicate = null;

            if(characters.Count <= 0)
            {
                characters.Add(character);
                return;
            }

            duplicate = characters.Find(characterSaved => characterSaved.characterName == character.characterName);

            if (duplicate != null && duplicate.starsRarity >= 10) characters.Add(duplicate);
            if (duplicate == null) characters.Add(character);
        }
    }
}
