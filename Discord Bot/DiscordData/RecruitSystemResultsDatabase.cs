using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace DiscordBot.DiscordData
{
    public class RecruitSystemResultsDatabase
    {
        [JsonProperty("latestResult")]
        public RecruitSystemResults latestResult = null;

        [JsonProperty("resultsHistory")]
        public List<RecruitSystemResults> resultsHistory = null;

        [JsonProperty("timestampLastResults")]
        public DateTime timestampLastResults = DateTime.Now;
    }
}
