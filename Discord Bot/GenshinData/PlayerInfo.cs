using Newtonsoft.Json;
using System.Collections.Generic;

namespace DiscordBot.GenshinData
{
    public class PlayerInfo
    {
        [JsonProperty("nickname")]
        public string nickname;
        [JsonProperty("level")]
        public string level;
        [JsonProperty("signature")]
        public string signature;

        [JsonProperty("worldLevel")]
        public int worldLevel;
        [JsonProperty("nameCardId")]
        public int nameCardId;
        [JsonProperty("finishAchievementNum")]
        public int finishAchievementNum;

        [JsonProperty("towerFloorIndex")]
        public int towerFloorIndex;
        [JsonProperty("towerLevelIndex")]
        public int towerLevelIndex;

        [JsonProperty("showNameCardIdList")]
        public List<int> showNameCardIdList = new List<int>();
        [JsonProperty("showAvatarInfoList")]
        public List<ShowAvatarInfoList> showAvatarInfoList = new List<ShowAvatarInfoList>();
        [JsonProperty("profilePicture")]
        public ProfilePicture profilePicture;
    }
}