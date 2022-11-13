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
        private bool isAutoSaveOn = true;

        private int maxMoraObtainedByTraveling = 500;
        private int maxPrimosObtainedByTraveling = 60;
        private int minutesAutoSave = 10;


        public const string USERS_DATABASE_JSON = "usersDatabase.json";
        public const string DEFAULT_ICON_EREMITE_ID = "img_characters/nochar.png";
        public const string TRAVEL_FOLDER = "travel_sumeru";
        public const string IMAGE_BASE = "banner_travel_";

        public DiscordDataHandler()
        {
            string fullPath = Path.Combine(Directory.GetCurrentDirectory(), TRAVEL_FOLDER, USERS_DATABASE_JSON);
            AutoSave().ConfigureAwait(false);

            if (!File.Exists(fullPath)) return;

            CacheUsers(fullPath).ConfigureAwait(false);
        }

        private async Task CacheUsers(string fullPath)
        {
            string usersDataJson = await File.ReadAllTextAsync(fullPath);

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
            Console.WriteLine($"\n[Discord Data Handler] Saving users data...\n");

            string json = JsonConvert.SerializeObject(usersData);
            await File.WriteAllTextAsync(Path.Combine(Directory.GetCurrentDirectory(), TRAVEL_FOLDER, USERS_DATABASE_JSON), json);
        }

        public void Travel(CommandContext ctx)
        {
            UserData user = usersData.Find(data => data.userId == ctx.User.Id);

            bool isNewUser = user == null;
            if (isNewUser)
            {
                user = new UserData();
                user.userId = ctx.User.Id;
                usersData.Add(user);
            }

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

            ctx.Channel.SendMessageAsync(builder);
        }

        public void ShowAkashaProfile(CommandContext ctx)
        {
            UserData user = usersData.Find(data => data.userId == ctx.User.Id);

            bool isNewUser = user == null;
            if (isNewUser)
            {
                user = new UserData();
                user.userId = ctx.User.Id;
                usersData.Add(user);
            }

            var equippedChar = user.currentEquippedCharacter;
            var iconPath = equippedChar == null ? DEFAULT_ICON_EREMITE_ID : Path.Combine(Directory.GetCurrentDirectory(), user.currentEquippedCharacter.imageIconPath);

            string currentChar = equippedChar == null ? "None, use !pull to get one :)" : equippedChar.characterName;
            string characterBuff = equippedChar == null ? "None, use !setcharacter [name] or !pull to get one :)" : equippedChar.perkInfo;
            string eremiteID = $"```elm\n[{ctx.Member.DisplayName}]\nMain Character: {currentChar}\nCharacter Buff: {characterBuff}\n\nEnrolled for Eremites Recruit System: {user.timesEremitesRecruitSystemEnrolled} | Welkin Moon won: {user.timesWelkinWon}\nMora: {user.wallet.mora} | Primos: {user.wallet.primogems}\n```";

            var builder = new DiscordMessageBuilder();

            builder.WithFile(File.OpenRead(iconPath));
            builder.WithContent(eremiteID);

            ctx.Channel.SendMessageAsync(builder);
        }

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
