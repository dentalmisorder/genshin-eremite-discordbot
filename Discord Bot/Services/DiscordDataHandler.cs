using System;
using System.Collections.Generic;
using System.IO;
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

        private int maxMoraObtainedByTraveling = 100;
        private int maxPrimosObtainedByTraveling = 40;
        private int minutesAutoSave = 5;

        private float fourStarChance = 0.40f;
        private float fiveStarChance = 0.05f;
        private float tenStarChance = 0.005f;

        public const int PULL_COST = 160;
        public const string USERS_DATABASE_JSON = "usersDatabase.json";
        public const string CHARACTER_FOLDER = "characters";
        public const string AKASHA_BANNERS_FOLDER = "akasha_banners";
        public const string CHARACTERS_DATABASE_JSON = "charactersDatabase.json";
        public const string DEFAULT_ICON_EREMITE_ID = "nochar.png";
        public const string TRAVEL_FOLDER = "travel_sumeru";
        public const string IMAGE_BASE = "banner_travel_";

        public DiscordDataHandler()
        {
            string fullPathDatabase = Path.Combine(Directory.GetCurrentDirectory(), TRAVEL_FOLDER, USERS_DATABASE_JSON);
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
                await SaveUsersData().ConfigureAwait(false);
            }
        }

        private async Task SaveUsersData()
        {
            if (usersData.Count <= 0) return;

            string json = string.Empty;
            json = JsonConvert.SerializeObject(usersData, Formatting.Indented);
            await File.WriteAllTextAsync(Path.Combine(Directory.GetCurrentDirectory(), TRAVEL_FOLDER, USERS_DATABASE_JSON), json).ConfigureAwait(false);

            Console.WriteLine($"\n[Discord Data Handler] Saving users data...\n");
        }

        public void RegisterNewUserIfNeeded(CommandContext ctx, ref UserData user)
        {
            bool isNewUser = user == null;
            if (isNewUser)
            {
                user = new UserData();
                user.userId = ctx.User.Id;
                usersData.Add(user);
            }
        }

        public void Travel(CommandContext ctx)
        {
            UserData user = GetUser(ctx.User.Id);

            RegisterNewUserIfNeeded(ctx, ref user);


            if (DateTime.Compare(user.timeLastTravel.AddDays(1).ToUniversalTime(), DateTime.Now.ToUniversalTime()) == 1)
            {
                ctx.Member.SendMessageAsync($"You can send travel expedition only once a day! Come check commissions board tomorrow!");
                return;
            }

            var rnd = new Random();
            int moraFound = rnd.Next(0, maxMoraObtainedByTraveling);
            int primogemsFound = rnd.Next(0, maxPrimosObtainedByTraveling);

            user.wallet.mora += moraFound;
            user.wallet.primogems += primogemsFound;
            user.timeLastTravel = DateTime.Now;

            //save if it was just created user to our list of users
            //it will save it automatically if auto-save is On


            string imgToSet = SetupTravelImage(moraFound, primogemsFound);

            string path = Path.Combine(Directory.GetCurrentDirectory(), TRAVEL_FOLDER, $"{IMAGE_BASE}{imgToSet}.png");
            var stream = File.OpenRead(path);

            var builder = new DiscordMessageBuilder();

            string content = $"```elm\n {ctx.User.Username} came back after traveling across desert with \n|{moraFound}| Mora\n|{primogemsFound}| Primogems.\n```";
            builder.WithContent(content);
            builder.WithFile(stream);

            ctx.Channel.SendMessageAsync(builder).ConfigureAwait(false);
            SaveUsersData().ConfigureAwait(false);
        }

        public void ShowAkashaProfile(CommandContext ctx)
        {
            UserData user = GetUser(ctx.User.Id);

            RegisterNewUserIfNeeded(ctx, ref user);

            string folder = Path.Combine(Directory.GetCurrentDirectory(), CHARACTER_FOLDER, AKASHA_BANNERS_FOLDER);
            var equippedChar = user.currentEquippedCharacter;
            var iconPath = equippedChar == null ? Path.Combine(folder, DEFAULT_ICON_EREMITE_ID) : Path.Combine(folder, $"{user.currentEquippedCharacter.imageAkashaBannerPath}");

            string currentChar = equippedChar == null ? "None, use !pull to get one :)" : equippedChar.characterName;
            string characterBuff = equippedChar == null ? "None, use !setcharacter [name] or !pull to get one :)" : equippedChar.perkInfo;
            string eremiteID = $"```elm\n[{ctx.Member.DisplayName}]\nMain Character: {currentChar}\nCharacter Buff: {characterBuff}\n\nEnrolled for Eremites Recruit System: {user.timesEremitesRecruitSystemEnrolled} | Welkin Moon won: {user.timesWelkinWon}\nMora: {user.wallet.mora} | Primos: {user.wallet.primogems}\n```";

            var builder = new DiscordMessageBuilder();

            builder.WithFile(File.OpenRead(iconPath));
            builder.WithContent(eremiteID);

            ctx.Channel.SendMessageAsync(builder).ConfigureAwait(false);
        }

        public async Task Pull(CommandContext ctx, UserData user)
        {
            Random rnd = new Random();

            float starsChance = (float)rnd.NextDouble();
            List<Character> charactersFromWhoToRoll = new List<Character>();

            if(starsChance <= tenStarChance)
            {
                charactersFromWhoToRoll = charactersData.FindAll(character => character.starsRarity == 10);
            }
            else if(starsChance <= fiveStarChance)
            {
                charactersFromWhoToRoll = charactersData.FindAll(character => character.starsRarity == 5);
            }
            else if(starsChance <= fourStarChance)
            {
                charactersFromWhoToRoll = charactersData.FindAll(character => character.starsRarity == 4);
            }
            else
            {
                charactersFromWhoToRoll = charactersData.FindAll(character => character.starsRarity == 3);
            }

            var characterPulled = charactersFromWhoToRoll[rnd.Next(0, charactersFromWhoToRoll.Count)];
            string pathToBannerImg = Path.Combine(Directory.GetCurrentDirectory(), CHARACTER_FOLDER, characterPulled.imagePullBannerPath);

            var builder = new DiscordMessageBuilder();
            builder.WithFile(File.OpenRead(pathToBannerImg));
            builder.WithContent($"```\n{ctx.Member.DisplayName} pulled a {characterPulled.characterName} <{characterPulled.starsRarity}☆> ! Congrats!\n```");

            await ctx.Channel.SendMessageAsync(builder).ConfigureAwait(false);
            if(!user.characters.Contains(characterPulled) && characterPulled.starsRarity < 10) user.characters.Add(characterPulled);

            await SaveUsersData().ConfigureAwait(false);
        }

        public UserData GetUser(ulong userSnowflakeId) => usersData.Find(data => data.userId == userSnowflakeId);

        private string SetupTravelImage(int moraFound, int primogemsFound)
        {
            string imgToSet = string.Empty;

            if (moraFound > 0 && primogemsFound > 0)
            {
                imgToSet = "primos_mora";
            }
            else if (moraFound > 0)
            {
                imgToSet = "mora";
            }
            else if (primogemsFound > 0)
            {
                imgToSet = "primos";
            }
            else
            {
                imgToSet = "nothing";
            }

            return imgToSet;
        }
    }
}
