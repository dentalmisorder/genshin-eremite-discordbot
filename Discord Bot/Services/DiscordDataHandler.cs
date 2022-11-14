using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DiscordBot.DiscordData;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using Newtonsoft.Json;

namespace DiscordBot.Services
{
    public class DiscordDataHandler
    {
        private List<UserData> usersData = new List<UserData>();
        private List<Character> charactersData = new List<Character>();
        private bool isAutoSaveOn = true;

        private int minutesAutoSave = 5;
        private int maxTopUsers = 10;

        private float fourStarChance = 0.40f;
        private float fiveStarChance = 0.05f;
        private float tenStarChance = 0.005f;

        private string starSign = "☆";

        public const int PULL_COST = 160;
        public const string USERS_DATABASE_JSON = "usersDatabase.json";
        public const string CHARACTER_FOLDER = "characters";
        public const string AKASHA_BANNERS_FOLDER = "akasha_banners";
        public const string CHARACTERS_DATABASE_JSON = "charactersDatabase.json";
        public const string DEFAULT_ICON_EREMITE_ID = "nochar.png";

        public DiscordDataHandler()
        {
            string fullPathDatabase = Path.Combine(Directory.GetCurrentDirectory(), USERS_DATABASE_JSON);
            string fullPathCharacters = Path.Combine(Directory.GetCurrentDirectory(), CHARACTER_FOLDER, CHARACTERS_DATABASE_JSON);
            AutoSave().ConfigureAwait(false);

            CacheUsersAndCharacters(fullPathDatabase, fullPathCharacters);
        }

        private void CacheUsersAndCharacters(string pathDatabase, string pathCharacters)
        {
            string charactersDataJson = File.ReadAllText(pathCharacters);

            //pre-load all Characters avaliable so we dont need to parse Json each time, we just take it and work with it
            charactersData = JsonConvert.DeserializeObject<List<Character>>(charactersDataJson);

            if (!File.Exists(pathDatabase)) return;
            string usersDataJson = File.ReadAllText(pathDatabase);

            //pre-load all UserData about discord mora,primos,etc. so we dont need to parse Json each time, we just take it and work with it
            usersData = JsonConvert.DeserializeObject<List<UserData>>(usersDataJson);
        }

        private async Task AutoSave()
        {
            
            while(isAutoSaveOn)
            {
                await Task.Delay(new TimeSpan(0, minutesAutoSave, 0));
                await SaveUsersData();
            }
        }

        private async Task SaveUsersData()
        {
            if (usersData.Count <= 0) return;

            string json = JsonConvert.SerializeObject(usersData, Formatting.Indented);

            Console.WriteLine($"\n[Discord Data Handler] Saving users data...\n");
            await File.WriteAllTextAsync(Path.Combine(Directory.GetCurrentDirectory(), USERS_DATABASE_JSON), json);
        }

        public void RegisterNewUserIfNeeded(CommandContext ctx, ref UserData user)
        {
            bool isNewUser = user == null;
            if (isNewUser)
            {
                user = new UserData();
                user.username = ctx.User.Username;
                user.userId = ctx.User.Id;
                usersData.Add(user);
            }
        }

        /// <summary>
        /// Gets TOP [10 (can be changed by variable)] users by type (Mora, Primos, etc)
        /// </summary>
        /// <param name="type">Type to sort for</param>
        /// <returns></returns>
        public List<UserData> GetTop(BestUserType type)
        {
            if (usersData.Count <= 0) return null;

            int counter = usersData.Count >= maxTopUsers ? maxTopUsers : usersData.Count;

            List<UserData> sortedList = new List<UserData>();
            for (int i = 0; i < counter; i++)
            {
                sortedList.Add(usersData[i]);
            }

            switch (type)
            {
                case BestUserType.Mora:
                    sortedList.OrderBy(data => data.wallet.mora);
                    break;

                case BestUserType.Primogems:
                    sortedList.OrderBy(data => data.wallet.primogems);
                    break;

                case BestUserType.PullingTimes:
                    sortedList.OrderBy(data => data.timesPulled);
                    break;
            }

            return sortedList;
        }

        public void ShowAkashaProfile(CommandContext ctx)
        {
            UserData user = GetUser(ctx.User.Id);

            RegisterNewUserIfNeeded(ctx, ref user);

            string folder = Path.Combine(Directory.GetCurrentDirectory(), CHARACTER_FOLDER, AKASHA_BANNERS_FOLDER);
            var equippedChar = user.currentEquippedCharacter;
            var iconPath = equippedChar == null ? Path.Combine(folder, DEFAULT_ICON_EREMITE_ID) : Path.Combine(folder, $"{user.currentEquippedCharacter.imageAkashaBannerPath}");

            string currentChar = equippedChar == null ? "None, use !pull to get one :)" : equippedChar.characterName;
            string charactersInInventory = string.Empty;

            foreach (var character in user.characters)
            {
                charactersInInventory = $"{charactersInInventory} {character.characterName}<{character.starsRarity}{starSign}> ";
            }
            string characterBuff = equippedChar == null ? "None, use !setcharacter [name] or !pull to get one :)" : equippedChar.perkInfo;
            string eremiteID = $"```elm\n[{ctx.Member.DisplayName}] [ID:{ctx.User.Id}]\nMain Character: {currentChar}\nCharacter Buff: {characterBuff}\n\nEnrolled for Eremites Recruit System: {user.timesEremitesRecruitSystemEnrolled} | Welkin Moon won: {user.timesWelkinWon}\nTimes traveled: {user.timesTraveled} | Teapot visited: {user.timesTeapotVisited} times\nCharacters Obtained: {charactersInInventory}\nMora: {user.wallet.mora} | Primos: {user.wallet.primogems}\n```";

            var builder = new DiscordMessageBuilder();

            builder.WithFile(File.OpenRead(iconPath));
            builder.WithContent(eremiteID);

            ctx.Channel.SendMessageAsync(builder).ConfigureAwait(false);
        }

        public async Task Pull(CommandContext ctx, UserData user)
        {
            Random rnd = new Random();

            var characterPulled = PullSilent(rnd);
            string pathToBannerImg = Path.Combine(Directory.GetCurrentDirectory(), CHARACTER_FOLDER, characterPulled.imagePullBannerPath);

            var builder = new DiscordMessageBuilder();
            builder.WithFile(File.OpenRead(pathToBannerImg));
            builder.WithContent($"```\n{ctx.Member.DisplayName} pulled a {characterPulled.characterName} <{characterPulled.starsRarity}{starSign}> ! Congrats!\n```");

            user.AddPulledCharacter(characterPulled);
            await ctx.Channel.SendMessageAsync(builder).ConfigureAwait(false);
        }
        
        /// <summary>
        /// Pulls a character and adds it to the user inventory
        /// </summary>
        /// <param name="user">User whom will get character in his inventory</param>
        /// <returns></returns>
        public Character PullSilent(Random rnd)
        {
            var charactersPool = PullPool(rnd);

            return charactersPool[rnd.Next(0, charactersPool.Count)];
        }

        /// <summary>
        /// Get randomizes pool of characters
        /// </summary>
        /// <returns></returns>
        public List<Character> PullPool(Random rnd)
        {
            float starsChance = (float)rnd.NextDouble();
            List<Character> charactersFromWhoToRoll = null;

            //CRINGE CONSTRUCTION! TODO: refactor but not at 3 AM pls bro i beg u spend some time
            if (starsChance <= tenStarChance)
            {
                charactersFromWhoToRoll = charactersData.FindAll(character => character.starsRarity == 10);
            }
            else if (starsChance <= fiveStarChance)
            {
                charactersFromWhoToRoll = charactersData.FindAll(character => character.starsRarity == 5);
            }
            else if (starsChance <= fourStarChance)
            {
                charactersFromWhoToRoll = charactersData.FindAll(character => character.starsRarity == 4);
            }
            else
            {
                charactersFromWhoToRoll = charactersData.FindAll(character => character.starsRarity == 3);
            }

            return charactersFromWhoToRoll;
        }

        public UserData GetUser(ulong userSnowflakeId) => usersData.Find(data => data.userId == userSnowflakeId);
    }
}
