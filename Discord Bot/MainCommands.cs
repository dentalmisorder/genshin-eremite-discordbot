using System.Threading.Tasks;
using DSharpPlus.Entities;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DiscordBot.Services;
using System;

namespace DiscordBot
{
    class MainCommands : BaseCommandModule
    {
        private NamecardsHandler namecardsHandler = null;
        private GenshinDataHandler genshinDataHandler = null;

        public const string PNG_PATH = ".png";

        public void Initialize()
        {
            var provider = new ServicesProvider();

            namecardsHandler = provider.NamecardsHandler;
            genshinDataHandler = provider.GenshinDataHandler;
        }

        [Command("stats")]
        [Description("Get the stats about your account by UID")]
        public async Task GetStats(CommandContext ctx, int uid)
        {
            if (namecardsHandler == null) Initialize();

            var userData = await GenshinDataHandler.LoadGenshinUserData(ctx, uid);

            if (userData == null) await Task.CompletedTask;
            var namecard = namecardsHandler.GetCardByID(userData.playerInfo.nameCardId);
            var character = namecardsHandler.GetCharacterByID(userData.playerInfo.profilePicture.avatarId);

            string picPath = namecard.picPath[0];
            if (picPath == string.Empty || picPath == null) picPath = namecard.picPath[1];

            var streamNamecard = namecardsHandler.DownloadImage(picPath);
            var builderNamecard = new DiscordMessageBuilder();
            builderNamecard.WithFile(streamNamecard);

            string characterIconName = character.sideIconName.Remove(13, 5);
            Console.WriteLine(characterIconName);

            var streamAvatar = namecardsHandler.DownloadImage(characterIconName);
            var builderAvatar = new DiscordMessageBuilder();
            builderAvatar.WithFile(streamAvatar);

            await ctx.Channel.SendMessageAsync(builderAvatar);
            await ctx.Channel.SendMessageAsync($"```arm\nName: {userData.playerInfo.nickname} [AR: {userData.playerInfo.level}] \nSignature: {userData.playerInfo.signature} \nStuck at Abyss: {userData.playerInfo.towerFloorIndex}-{userData.playerInfo.towerLevelIndex} | Achievements done: {userData.playerInfo.finishAchievementNum} | World Lvl: {userData.playerInfo.worldLevel}\n```");
            await ctx.Channel.SendMessageAsync(builderNamecard);

            //webClient.DownloadFile($"{RequestImgPath}{userData.playerInfo.n}{PNG_PATH});
        }

        [Command("enroll")]
        [Description("Enrolling in weekly FREE Welkin, all you need is to put UID, 1 UID per user and 1 Welkin per week. It is officially supported by Razer Gold. More: https://github.com/dentalmisorder/discordbot")]
        public async Task Enroll(CommandContext ctx, int uid)
        {
            //TODO: set UID in .json with all UIDs
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
            if (stream == null) await Task.CompletedTask;

            var builder = new DiscordMessageBuilder();

            builder.WithFile(stream);

            await ctx.Channel.SendMessageAsync(builder).ConfigureAwait(false);
        }
    }
}
