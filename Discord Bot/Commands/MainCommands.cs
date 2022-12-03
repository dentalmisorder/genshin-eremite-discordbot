using System.Threading.Tasks;
using DSharpPlus.Entities;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DiscordBot.Services;
using System;
using System.Linq;

namespace DiscordBot.Commands
{
    class MainCommands : BaseCommandModule
    {
        private NamecardsHandler namecardsHandler = null;
        private EremiteRecruitSystem eremiteRecruitSystem = null;

        public const string PNG_PATH = ".png";

        private void Initialize()
        {
            var provider = ServicesProvider.Instance;

            namecardsHandler = provider.NamecardsHandler;
            eremiteRecruitSystem = provider.EremiteRecruitSystem;
        }

        [Command("stats")]
        [Description("Get the stats about your account by UID")]
        public async Task GetStats(CommandContext ctx, int uid)
        {
            if (namecardsHandler == null) Initialize();
            if (uid.ToString().Length > 9)
            {
                await ctx.Channel.SendMessageAsync("Your UID is longer then 9 digits. Copy your UID from Genshin Account (which is in lower right corner) and paste it here.").ConfigureAwait(false);
                return;
            }

            var userData = await GenshinDataHandler.LoadGenshinUserData(ctx, uid);

            if (userData == null) await Task.CompletedTask;
            var namecard = namecardsHandler.GetCardByID(userData.playerInfo.nameCardId);
            var character = namecardsHandler.GetCharacterByID(userData.playerInfo.profilePicture.avatarId);

            string picPath = namecard.picPath[0];
            if (picPath == string.Empty || picPath == null) picPath = namecard.picPath[1];

            var streamNamecard = NamecardsHandler.DownloadImage(picPath);
            var builderNamecard = new DiscordMessageBuilder();

            string characterIconName = character.sideIconName.Remove(13, 5); //split to get not sided img but front view

            var streamAvatar = NamecardsHandler.DownloadImage(characterIconName);
            var builderAvatar = new DiscordMessageBuilder();
            var buttonDetails = new DiscordLinkButtonComponent($"https://enka.network/u/{uid}", "Detailed Info about party");

            string content = $"```arm\nName: {userData.playerInfo.nickname} [AR: {userData.playerInfo.level}] \nSignature: {userData.playerInfo.signature} \nAbyss: {userData.playerInfo.towerFloorIndex}-{userData.playerInfo.towerLevelIndex} | Achievements done: {userData.playerInfo.finishAchievementNum} | World Lvl: {userData.playerInfo.worldLevel}\n```";
            builderAvatar.WithFile(streamAvatar);
            builderAvatar.AddComponents(buttonDetails);

            builderNamecard.WithFile(streamNamecard);
            builderNamecard.WithContent(content);

            await ctx.Channel.SendMessageAsync(builderAvatar).ConfigureAwait(false);
            await ctx.Channel.SendMessageAsync(builderNamecard).ConfigureAwait(false);
        }

        [Command("enroll")]
        [Description("Enrolling in weekly FREE Welkin, all you need is to put UID, 1 UID per user and 1 Welkin per week. It is officially supported by Razer Gold. More: https://github.com/dentalmisorder/discordbot/wiki")]
        public async Task Enroll(CommandContext ctx, int uid)
        {
            if (eremiteRecruitSystem == null) Initialize();
            if(uid.ToString().Length > 9)
            {
                await ctx.Channel.SendMessageAsync("Your UID is longer then 9 digits. Copy your UID from Genshin Account (which is in lower right corner) and paste it here.").ConfigureAwait(false);
                return;
            }

            //TODO: set UID in .json with all UIDs
            await eremiteRecruitSystem.Enroll(ctx, uid).ConfigureAwait(false);
        }

        [Command("welkinwinners")]
        [Description("Showing all the winners of Free Welkin Moon from past Week. To enroll simply type !enroll [genshin UID], thats all, !pull some characters to improve your luck and positions!")]
        public async Task WelkinWinners(CommandContext ctx)
        {
            if (eremiteRecruitSystem == null) Initialize();

            var resultsDb = eremiteRecruitSystem.GetResultsDb();
            var results = resultsDb.latestResult;

            if (results == null)
            {
                await ctx.Channel.SendMessageAsync("There is no winners data, its probably the first weekly drop or gifts are still being provided from previous Welkin drop").ConfigureAwait(false);
                return;
            }

            string vip = string.Empty;
            string guaranteed = string.Empty;
            string randomEremite = string.Empty;
            if(results.guaranteedEremitesWon != null)
            {
                foreach (var garantWinner in results.guaranteedEremitesWon)
                {
                    guaranteed = $"{guaranteed}\n{garantWinner.username}";
                }
            }

            var builder = new DiscordMessageBuilder();

            string winners = $"```arm";
            if (results.randomEremiteWon != null) randomEremite = $"{results.randomEremiteWon.username} [{ results.randomEremiteWon.clientId}]";
            if (results.randomVipEremiteWon != null) vip = $"{results.randomVipEremiteWon.username} [{results.randomVipEremiteWon.clientId}]";
            winners = winners + $"\nRandom Eremite: {randomEremite}";
            winners = winners + $"\nRandom VIP user: {vip}";
            winners = winners + $"\nGuaranteed users: {guaranteed}";
            winners = winners + $"\n\n[TIMESTAMP: {results.timestampResults.ToShortDateString()} {results.timestampResults.ToShortTimeString()}]```";

            builder.WithContent(winners);
            await ctx.Channel.SendMessageAsync(builder).ConfigureAwait(false);
        }


        [Command("materials")]
        [Description("After a materials type a name of a character to get ascension stats card")]
        public async Task GetAscensionMaterials(CommandContext ctx, string characterName)
        {
            var stream = GenshinDataHandler.GetMaterialsCard(characterName);
            if (stream == null) await Task.CompletedTask;

            var builder = new DiscordMessageBuilder();
            builder.WithFile(stream);

            await ctx.Channel.SendMessageAsync(builder).ConfigureAwait(false);
        }

        [Command("materials")]
        [Description("After a materials type a name of a character to get ascension stats card")]
        public async Task GetAscensionMaterials(CommandContext ctx, string characterName, string characterSurname)
        {
            var stream = GenshinDataHandler.GetMaterialsCard(characterName, characterSurname);
            if (stream == null)
            {
                await Task.CompletedTask;
                return;
            }

            var builder = new DiscordMessageBuilder();

            builder.WithFile(stream);

            await ctx.Channel.SendMessageAsync(builder).ConfigureAwait(false);
        }
    }
}
