using Newtonsoft.Json;
using System.Collections.Generic;

namespace DiscordBot.GenshinData
{
    public class AvatarInfoList
    {
        [JsonProperty("avatarID")]
        public int avatarID;
        
        [JsonProperty("talentIdList")]
        public List<int> talentIdList = new List<int>();
        [JsonProperty("inherentProudSkillList")]
        public List<int> inherentProudSkillList = new List<int>();
        [JsonProperty("skillDepotId")]
        public int skillDepotId;
        [JsonProperty("propMap")]
        public PropMap propMap;
        [JsonProperty("equipList")]
        public EquipList equipList;
    }
}