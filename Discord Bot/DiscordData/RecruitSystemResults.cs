using DiscordBot.GenshinData;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace DiscordBot.DiscordData
{
    public class RecruitSystemResults
    {
        [JsonProperty("randomEremiteWon")]
        public EremiteRecruit randomEremiteWon = null;
        [JsonProperty("randomVipEremiteWon")]
        public EremiteRecruit randomVipEremiteWon = null;
        [JsonProperty("guaranteedEremitesWon")]
        public List<EremiteRecruit> guaranteedEremitesWon = null;

        [JsonProperty("timestampResults")]
        public DateTime timestampResults = DateTime.Now.ToUniversalTime();

        public string GetResultsShortDate() => timestampResults.ToShortDateString();

        public RecruitSystemResults(EremiteRecruit randomEremite, EremiteRecruit randomVipEremite, List<EremiteRecruit> guaranteedEremites)
        {
            randomEremiteWon = randomEremite;
            randomVipEremiteWon = randomVipEremite;
            guaranteedEremitesWon = guaranteedEremites;
        }
    }
}
