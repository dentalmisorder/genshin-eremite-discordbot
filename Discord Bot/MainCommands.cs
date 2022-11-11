using System.Threading.Tasks;
using DSharpPlus.Entities;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DiscordBot.Services;

namespace DiscordBot
{
    class MainCommands : BaseCommandModule
    {
        private NamecardsHandler namecardsHandler = null;
        private GenshinDataHandler genshinDataHandler = null;

        public const string PNG_PATH = ".png";

        public void Initialize(ServicesProvider servicesProvider)
        {
            namecardsHandler = servicesProvider.NamecardsHandler;
            genshinDataHandler = servicesProvider.GenshinDataHandler;
        }

        [Command("stats")]
        [Description("Get the stats about your account by UID")]
        public async Task GetStats(CommandContext ctx, int uid)
        {
            var userData = await GenshinDataHandler.LoadGenshinUserData(ctx, uid);

            await ctx.Channel.SendMessageAsync($"Name: {userData.playerInfo.nickname} [AR: {userData.playerInfo.level}]\nStatus: {userData.playerInfo.signature} \nAbyss Floor: {userData.playerInfo.towerFloorIndex}-{userData.playerInfo.towerLevelIndex} \nAchievements done: {userData.playerInfo.finishAchievementNum} \nWorld Level: {userData.playerInfo.worldLevel}");

            //webClient.DownloadFile($"{RequestImgPath}{userData.playerInfo.n}{PNG_PATH});
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
