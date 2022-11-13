using System;
using System.IO;
using System.Threading.Tasks;
using DSharpPlus.Entities;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DiscordBot.Services;

namespace DiscordBot
{
    class UtilityCommands : BaseCommandModule
    {
        private DiscordDataHandler discordDataHandler = null;

        public const string NSFW_FOLDER = "NSFW";
        public const string NSFW_ROLE = "Cherry";

        private void Initialize()
        {
            discordDataHandler = ServicesProvider.Instance.DiscordDataHandler;
        }

        [Command("nsfw")]
        [Description("Channel must be switched to restricted (marked as NSFW) before this command. Randomly gives genshin-related nsfw media (18+)")]
        [RequireNsfw()]
        public async Task Nsfw(CommandContext ctx)
        {
            var fullPath = Path.Combine(Directory.GetCurrentDirectory(), NSFW_FOLDER);

            var allFiles = Directory.GetFiles(fullPath);
            var random = new Random();

            var stream = File.OpenRead(Path.Combine(fullPath, allFiles[random.Next(0, allFiles.Length)]));

            var builder = new DiscordMessageBuilder();
            builder.WithFile(stream);

            await ctx.Channel.SendMessageAsync(builder).ConfigureAwait(false);
        }

        [Command("travel")]
        [Description("Travel across regionns with Eremites and recruits, help them do commisions and get the chance to obtain rare rewards!")]
        public async Task Travel(CommandContext ctx)
        {
            if (discordDataHandler == null) Initialize();

            discordDataHandler.Travel(ctx);
        }

        [Command("akashaprofile")]
        public async Task AkashaProfile(CommandContext ctx)
        {
            if (discordDataHandler == null) Initialize();

            discordDataHandler.ShowAkashaProfile(ctx);
        }

        //You can use roles to define who can use commands, as example:
        //[RequireRoles(RoleCheckMode.Any, NSFW_ROLE, "Administrator")]
    }
}
