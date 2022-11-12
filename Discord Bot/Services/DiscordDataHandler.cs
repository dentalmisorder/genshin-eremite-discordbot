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
        public List<UserData> usersData = new List<UserData>();

        public bool isAutoSaveOn = true;

        public int maxMoraObtainedByTraveling = 500;
        public int maxPrimosObtainedByTraveling = 60;

        public const string USERS_DATABASE_JSON = "usersDatabase.json";
        public const string FOLDER = "travel_sumeru";
        public const string IMAGE_BASE = "banner_travel_";
        public const int minutesAutoSave = 10;

        public DiscordDataHandler()
        {
            string fullPath = Path.Combine(Directory.GetCurrentDirectory(), FOLDER, USERS_DATABASE_JSON);
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
            await File.WriteAllTextAsync(Path.Combine(Directory.GetCurrentDirectory(), FOLDER, USERS_DATABASE_JSON), json);
        }

        public void Travel(CommandContext ctx)
        {
            Console.WriteLine($"{usersData.Count}, {usersData.ToString()}");
            UserData user = usersData.Find(data => data.userId == ctx.User.Id);

            bool isNewUser = user == null;
            if (isNewUser)
            {
                user = new UserData();
                user.userId = ctx.User.Id;
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

            if (isNewUser) usersData.Add(user);
            //save if it was just created user to our list of users
            //it will save it automatically if auto-save is On


            string imgToSet = SetupTravelImage(moraFound, primogemsFound);

            string path = Path.Combine(Directory.GetCurrentDirectory(), FOLDER, $"{IMAGE_BASE}{imgToSet}.png");
            var stream = File.OpenRead(path);

            var builder = new DiscordMessageBuilder();

            string content = $"```elm\n {ctx.User.Username} came back after traveling across desert with \n|{moraFound}| Mora\n|{primogemsFound}| Primogems.\n```";
            builder.WithContent(content);
            builder.WithFile(stream);

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
