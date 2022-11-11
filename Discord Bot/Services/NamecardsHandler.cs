using DiscordBot.GenshinData;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace DiscordBot.Services
{
    public class NamecardsHandler
    {
        public Dictionary<string, NamecardSettings> NamecardsDatabase { get; private set; } = new Dictionary<string, NamecardSettings>();
        //caching, so we dont need to Deserialize everyime

        public const string NAMECARDS_JSON_FOLDER = "store";
        public const string NAMECARDS_JSON_DATA = "namecards.json";

        public string fullPath;

        public void Initialize()
        {
            fullPath = Path.Combine(Directory.GetCurrentDirectory(), NAMECARDS_JSON_FOLDER, NAMECARDS_JSON_DATA);

            Console.WriteLine(fullPath);
            if (NamecardsDatabase.Count > 0) return;
            try
            {
                NamecardsDatabase = JsonConvert.DeserializeObject<Dictionary<string, NamecardSettings>>(fullPath);
            }
            catch(Exception ex)
            {
                Console.WriteLine($"\n[Namecards Handler] Error: {ex.Message}\n");
            }
        }
    }
}
