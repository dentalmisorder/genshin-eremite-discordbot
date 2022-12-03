using DiscordBot.DiscordData;
using DiscordBot.GenshinData;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DiscordBot.Services
{
    public class EremiteRecruitSystem
    {
        public const string JSON_MERCENARIES_FOLDER = "eremites_recruit_system";
        public const string IMAGE = "banner_eremite_recruit_system.png";
        public const string JSON_MERCENARIES_DATABASE = "eremites_recruits.json";
        public const string JSON_WINNERS_DATABASE = "eremites_won.json";
        public const string DOCUMENTATION_URL = "https://github.com/dentalmisorder/discordbot/wiki/Enroll---Eremite-Recruit-System#whats-that-enroll-into-a-new-recruiting-system-where-you-apply-for-a-contract-and-go-out-for-weekly-commissions-to-get-rare-rewards-the-smartest-and-luckiest-one-will-get-welkin-moon-from-the-contractor";

        public const DayOfWeek dayOfResults = DayOfWeek.Saturday;

        private List<EremiteRecruit> recruitsCached = new List<EremiteRecruit>();
        private List<EremiteRecruit> recruitsWithGuaranteedCached = new List<EremiteRecruit>();
        private List<EremiteRecruit> recruitsWithVipListCached = new List<EremiteRecruit>();

        private RecruitSystemResultsDatabase recruitSystemResultsDatabase = null;
        private DiscordDataHandler discordDataHandler = null;

        public RecruitSystemResultsDatabase GetResultsDb() => recruitSystemResultsDatabase;

        public EremiteRecruitSystem()
        {
            string folderPath = Path.Combine(Directory.GetCurrentDirectory(), JSON_MERCENARIES_FOLDER);

            if (!Directory.Exists(folderPath)) Directory.CreateDirectory(folderPath);
            CacheRecruitSystemDb();

            StartTimer();
        }

        private void Initialize() => discordDataHandler = ServicesProvider.Instance.DiscordDataHandler;

        private void CacheRecruitSystemDb()
        {
            var path = Path.Combine(Directory.GetCurrentDirectory(), JSON_MERCENARIES_FOLDER, JSON_WINNERS_DATABASE);

            recruitSystemResultsDatabase = new RecruitSystemResultsDatabase();
            if (!File.Exists(path)) return;
            string databaseResults = File.ReadAllText(path);

            recruitSystemResultsDatabase = JsonConvert.DeserializeObject<RecruitSystemResultsDatabase>(databaseResults);
        }

        private async Task StartTimer()
        {
            string folderPath = Path.Combine(Directory.GetCurrentDirectory(), JSON_MERCENARIES_FOLDER);
            string fullPath = Path.Combine(folderPath, JSON_MERCENARIES_DATABASE);

            if (File.Exists(fullPath)) await CacheRecruits(fullPath);

            Console.WriteLine("\n\nStarting Timer for Welkin Moon...");
            while(true)
            {
                await CheckDayOfResults();
                await Task.Delay(new TimeSpan(12, 0, 0));
            }
        }

        private async Task CheckDayOfResults()
        {
            if (dayOfResults != DateTime.Now.DayOfWeek) return;
            Console.WriteLine("Checking Results...");

            await SaveResults(GetRandomWinner());
            ResetAllDestroyable();
        }

        private void ResetAllDestroyable()
        {
            if (discordDataHandler == null) Initialize();
            var allUsers = discordDataHandler.GetAllUsers();
            foreach (var user in allUsers)
            {
                if (user.characters.Count > 0) user.characters.RemoveAll(character => character.shouldBeDestroyed);
            }
        }

        private async Task SaveResults(RecruitSystemResults results)
        {
            var path = Path.Combine(Directory.GetCurrentDirectory(), JSON_MERCENARIES_FOLDER, JSON_WINNERS_DATABASE);

            if (File.Exists(path))
            {
                if (DateTime.Compare(recruitSystemResultsDatabase.timestampLastResults.AddDays(2), DateTime.Now.ToUniversalTime()) > 0) return; //if last save + 2 days greater then this time, means week isnt passed

                await WriteToDatabase(recruitSystemResultsDatabase, results, path).ConfigureAwait(false);
            }
            else
            {
                await WriteToDatabase(recruitSystemResultsDatabase, results, path).ConfigureAwait(false);
            }
        }

        private async Task WriteToDatabase(RecruitSystemResultsDatabase db, RecruitSystemResults results, string pathToWrite)
        {
            db.timestampLastResults = DateTime.Now.ToUniversalTime();

            if (db.resultsHistory == null) db.resultsHistory = new List<RecruitSystemResults>();
            db.resultsHistory.Add(results);
            db.latestResult = results;

            var path = Path.Combine(Directory.GetCurrentDirectory(), JSON_MERCENARIES_FOLDER, JSON_MERCENARIES_DATABASE);
            File.Delete(path);

            await File.WriteAllTextAsync(pathToWrite, JsonConvert.SerializeObject(db, Formatting.Indented)).ConfigureAwait(false);
        }

        private async Task CacheRecruits(string fullPath)
        {
            string recruitsEnrolledJson = await File.ReadAllTextAsync(fullPath);

            recruitsCached = JsonConvert.DeserializeObject<List<EremiteRecruit>>(recruitsEnrolledJson);

            await Task.CompletedTask;
        }

        public async Task Enroll(CommandContext ctx, int uid)
        {
            string folderPath = Path.Combine(Directory.GetCurrentDirectory(), JSON_MERCENARIES_FOLDER);
            string fullPath = Path.Combine(folderPath, JSON_MERCENARIES_DATABASE);

            if (!Directory.Exists(folderPath)) Directory.CreateDirectory(folderPath);

            bool isAlreadyEnrolled = false;

            if (recruitsCached?.Count >= 1)
            {
                foreach (var recruit in recruitsCached)
                {
                    if (recruit.uid != uid) continue;

                    var builderError = new DiscordMessageBuilder();
                    var buttonLink = new DiscordLinkButtonComponent(DOCUMENTATION_URL, "More about Eremites Recruit System");

                    isAlreadyEnrolled = true;
                    builderError.WithContent($"You already enrolled for Eremites Recruit System this week with UID: {uid}\nThanks for helping with our commisions across Sumeru!");
                    builderError.WithFile(File.OpenRead(Path.Combine(folderPath, IMAGE)));
                    builderError.AddComponents(buttonLink);
                    await ctx.Member.SendMessageAsync(builderError).ConfigureAwait(false);
                }
            }

            if (isAlreadyEnrolled) return;
            var user = UpdateUserAkashaData(ctx);

            EremiteRecruit eremite = new EremiteRecruit(ctx.User.Username, ctx.User.Id, uid);
            recruitsCached.Add(eremite);
            
            await File.WriteAllTextAsync(fullPath, JsonConvert.SerializeObject(recruitsCached, Formatting.Indented));

            var builderSuccess = new DiscordMessageBuilder();
            var buttonAbout = new DiscordLinkButtonComponent(DOCUMENTATION_URL, "More about Eremites Recruit System");

            builderSuccess.WithContent($"You have successfully enrolled for Eremites Recruit System this week with UID: {uid}\nThanks for helping with our commisions! Travel across Sumeru to obtain rare rewards.");
            builderSuccess.WithFile(File.OpenRead(Path.Combine(folderPath, IMAGE)));
            builderSuccess.AddComponents(buttonAbout);
            await ctx.Member.SendMessageAsync(builderSuccess).ConfigureAwait(false);
        }

        public RecruitSystemResults GetRandomWinner()
        {
            var rnd = new Random();
            UseActiveCharactersPerks();

            var results = new RecruitSystemResults(
                recruitsCached.Count > 0 ? recruitsCached[rnd.Next(0, recruitsCached.Count)] : null,
                recruitsWithVipListCached.Count > 0 ? recruitsWithVipListCached[rnd.Next(0, recruitsWithVipListCached.Count)] : null, 
                recruitsWithGuaranteedCached.Count > 0 ? recruitsWithGuaranteedCached : null);

            UpdateWinCounter(results);

            return results;
        }

        private void UpdateWinCounter(RecruitSystemResults results)
        {
            if (results.randomEremiteWon != null)
            {
                var randomEremite = discordDataHandler.GetUser(results.randomEremiteWon.clientId);
                if (randomEremite != null) randomEremite.timesWelkinWon++;
            }
            if (results.randomVipEremiteWon != null)
            {
                var randomVipEremite = discordDataHandler.GetUser(results.randomVipEremiteWon.clientId);
                if (randomVipEremite != null) randomVipEremite.timesWelkinWon++;
            }

            if (results.guaranteedEremitesWon == null) return;
            if (results.guaranteedEremitesWon.Count <= 0) return;
            foreach (var guaranteed in results.guaranteedEremitesWon)
            {
                var user = discordDataHandler.GetUser(guaranteed.clientId);
                if (user == null) continue;
                user.timesWelkinWon++;
            }
        }

        private UserData UpdateUserAkashaData(CommandContext ctx)
        {
            if (discordDataHandler == null) Initialize();

            var user = discordDataHandler.GetUser(ctx.User.Id);
            discordDataHandler.RegisterNewUserIfNeeded(ctx, ref user);

            user.timesEremitesRecruitSystemEnrolled++;
            return user;
        }

        private void UseActiveCharactersPerks()
        {
            if (discordDataHandler == null) Initialize();

            var listToCheck = recruitsCached;
            foreach (var recruit in listToCheck)
            {
                var user = discordDataHandler.GetUser(recruit.clientId);
                if (user == null) continue;


                if (user.currentEquippedCharacter == null) continue;

                WriteAccordingToPerk(user, recruit);

                Character characterToDelete = null;
                if (user.characters.Count > 0) characterToDelete = user.characters.FirstOrDefault(character => character.shouldBeDestroyedOnEnroll);
                if (characterToDelete == null) return;

                user.characters.Remove(characterToDelete);

                if (user.currentEquippedCharacter == null) return;
                if (user.currentEquippedCharacter.characterName == characterToDelete.characterName) user.currentEquippedCharacter = null;
            }
        }

        private void WriteAccordingToPerk(UserData user, EremiteRecruit eremite)
        {
            var equippedChar = user.currentEquippedCharacter;

            if (equippedChar.perkStat == (int)Perk.GUARANTEED_NEXT_WELKIN)
            {
                recruitsCached.Remove(eremite);
                recruitsWithGuaranteedCached.Add(eremite);

                if (equippedChar.starsRarity >= 10)
                {
                    user.characters.Remove(user.currentEquippedCharacter);
                    user.currentEquippedCharacter = null;
                }
            }
            else if(equippedChar.perkStat == (int)Perk.VIP_LIST_NEXT_WELKIN)
            {
                recruitsCached.Remove(eremite);
                recruitsWithVipListCached.Add(eremite);

                if (equippedChar.starsRarity >= 10)
                {
                    user.characters.Remove(user.currentEquippedCharacter);
                    user.currentEquippedCharacter = null;
                }
            }
            else if (equippedChar.perkStat == (int)Perk.WELKIN_EREMITE_RECRUIT_ID_WRITE_TWICE)
            {
                recruitsCached.Add(eremite);
            }
            else if (equippedChar.perkStat == (int)Perk.WELKIN_EREMITE_RECRUIT_ID_WRITE_TRIPLE_NO_PRIMOS)
            {
                recruitsCached.Add(eremite);
                recruitsCached.Add(eremite);
            }
        }
    }
}
