using DiscordBot.GenshinData;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace DiscordBot.Services
{
    public class NamecardsHandler
    {
        public Dictionary<string, NamecardSettings> NamecardsDatabase { get; private set; } = new Dictionary<string, NamecardSettings>();
        public Dictionary<string, CharactersSettings> CharactersDatabase { get; private set; } = new Dictionary<string, CharactersSettings>();
        //caching, so we dont need to Deserialize everyime

        public const string IMG_CACHE_FOLDER = "img_cache";
        public const string NAMECARDS_JSON_FOLDER = "store";
        public const string NAMECARDS_JSON_DATA = "namecards.json";
        public const string CHARACTERS_JSON_DATA = "characters.json";
        public const string REQUEST_IMG_URL = "https://enka.network/ui/";

        public string fullPath;

        public NamecardsHandler()
        {
            string fullPathNamecards = Path.Combine(Directory.GetCurrentDirectory(), NAMECARDS_JSON_FOLDER, NAMECARDS_JSON_DATA);
            string fullPathCharacters = Path.Combine(Directory.GetCurrentDirectory(), NAMECARDS_JSON_FOLDER, CHARACTERS_JSON_DATA);

            Console.WriteLine(fullPath);
            if (NamecardsDatabase.Count > 0) return;

            string jsonTextNamecards = File.ReadAllText(fullPathNamecards);
            string jsonTextCharacters = File.ReadAllText(fullPathCharacters);

            NamecardsDatabase = JsonConvert.DeserializeObject<Dictionary<string, NamecardSettings>>(jsonTextNamecards);
            CharactersDatabase = JsonConvert.DeserializeObject<Dictionary<string, CharactersSettings>>(jsonTextCharacters);
        }

        public NamecardSettings GetCardByID(int id)
        {
            NamecardSettings settings = null;

            foreach (var namecard in NamecardsDatabase)
            {
                if (namecard.Key != id.ToString()) continue;
                settings = namecard.Value;
            }

            return settings;
        }

        public CharactersSettings GetCharacterByID(int id)
        {
            CharactersSettings settings = null;

            foreach (var character in CharactersDatabase)
            {
                if (character.Key != id.ToString()) continue;
                settings = character.Value;
            }

            return settings;
        }

        public static FileStream DownloadCardImage(NamecardSettings settings)
        {
            return DownloadImage(settings.picPath[0]);
        }

        public static FileStream DownloadAvatarImage(NamecardSettings settings)
        {
            return DownloadImage(settings.icon);
        }

        public static FileStream DownloadImage(string picturePath)
        {
            string fullPath = $"{REQUEST_IMG_URL}{picturePath}.png";
            string folderPath = Path.Combine(Directory.GetCurrentDirectory(), IMG_CACHE_FOLDER);

            if (!Directory.Exists(folderPath)) Directory.CreateDirectory(folderPath);
            if(!File.Exists(Path.Combine(folderPath, $"{picturePath}.png"))) //to not download 100500 times the same pic if its cached
            {
                WebClient webClient = new WebClient();

                webClient.DownloadFile(fullPath, Path.Combine(folderPath, $"{picturePath}.png"));
            }

            return File.OpenRead(Path.Combine(folderPath, $"{picturePath}.png"));
        }
    }
}
