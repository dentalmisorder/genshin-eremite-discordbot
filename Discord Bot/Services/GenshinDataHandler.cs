using DiscordBot.GenshinData;
using DSharpPlus.CommandsNext;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace DiscordBot.Services
{
    public class GenshinDataHandler
    {
        public const string GENSHIN_JSON_DATA_PATH = "genshinData.json";
        public const string GENSHIN_MATERIALS_DATA_PATH = "ascension_materials";

        public static async Task<GenshinUserData> LoadGenshinUserData(CommandContext ctx, int uid)
        {
            string fullPath = Path.Combine(Directory.GetCurrentDirectory(), GENSHIN_JSON_DATA_PATH);
            string requestUri = $"https://enka.network/u/{uid}/__data.json";

            var webClient = new WebClient();
            webClient.DownloadFile(requestUri, GENSHIN_JSON_DATA_PATH);

            GenshinUserData userData = null;

            try
            {
                userData = JsonConvert.DeserializeObject<GenshinUserData>(File.ReadAllText(fullPath));
            }
            catch (Exception ex)
            {
                await ctx.Channel.SendMessageAsync(ex.Message);
                await Task.CompletedTask;
            }

            return userData;
        }

        public static string GetMaterialsCardPath(string characterName, string characterSurname)
        {
            var fullPath = Path.Combine(Directory.GetCurrentDirectory(), GENSHIN_MATERIALS_DATA_PATH);

            var allFiles = Directory.GetFiles(fullPath);
            string targetName = string.Empty;

            string characterToLookup = $"{characterName}";
            if (characterSurname != null && characterSurname != string.Empty)
                characterToLookup = $"{characterToLookup} {characterSurname}";

            foreach (var item in allFiles)
            {
                if (!item.ToLower().Contains(characterToLookup.ToLower())) continue;
                targetName = item;
            }

            if (targetName == string.Empty) return string.Empty;

            return Path.Combine(fullPath, targetName);
        }

        public static FileStream GetMaterialsCard(string characterCardFor)
        {
            string path = GetMaterialsCardPath(characterCardFor, string.Empty);
            if (path == string.Empty) return null;

            return File.OpenRead(path);
        }

        public static FileStream GetMaterialsCard(string characterName, string characterSurname)
        {
            string path = GetMaterialsCardPath(characterName, characterSurname);
            if (path == string.Empty) return null;

            return File.OpenRead(path);
        }
    }
}
