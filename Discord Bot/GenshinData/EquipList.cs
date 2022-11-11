using Newtonsoft.Json;

namespace DiscordBot.GenshinData
{
    public class EquipList
    {
        [JsonProperty("itemId")]
        public int itemId;
        [JsonProperty("weapon")]
        public Weapon weapon;
        [JsonProperty("reliquary")]
        public Reliquary reliquary;
        [JsonProperty("flat")]
        public Flat flat;
    }
}