using System;
using System.IO;
using System.Threading.Tasks;
using DSharpPlus.Entities;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;

namespace DiscordBot
{
    class UtilityCommands : BaseCommandModule
    {
        public string NSFWFolder { get; protected set; } = "NSFW";
        public string GenshinNSFWFolder { get; protected set; } = "NSFW_Genshin";

        public const string NSFW_ROLE = "Cherry";

        [Command("nsfw")]
        [Description("Channel must be switched to restricted (marked as NSFW) before this command. Randomly gives nsfw media (18+)")]
        [RequireNsfw()]
        public async Task Nsfw(CommandContext ctx)
        {
            var fullPath = Path.Combine(Directory.GetCurrentDirectory(), NSFWFolder);

            var allFiles = Directory.GetFiles(fullPath);
            var random = new Random();

            var stream = File.OpenRead(Path.Combine(fullPath, allFiles[random.Next(0, allFiles.Length)]));

            var builder = new DiscordMessageBuilder();
            builder.WithFile(stream);

            await ctx.Channel.SendMessageAsync(builder).ConfigureAwait(false);
        }

        //You can use roles to define who can use commands, as example:
        //[RequireRoles(RoleCheckMode.Any, NSFW_ROLE, "Administrator")]

        [Command("NsfwGenshin")]
        [Description("Channel must be switched to restricted (marked as NSFW) before this command. Randomly gives nsfw media (18+)")]
        [RequireNsfw()]
        public async Task NsfwGenshin(CommandContext ctx)
        {
            var fullPath = Path.Combine(Directory.GetCurrentDirectory(), GenshinNSFWFolder);

            var allFiles = Directory.GetFiles(fullPath);
            var random = new Random();

            var stream = File.OpenRead(Path.Combine(fullPath, allFiles[random.Next(0, allFiles.Length)]));

            var builder = new DiscordMessageBuilder();
            builder.WithFile(stream);

            await ctx.Channel.SendMessageAsync(builder).ConfigureAwait(false);
        }
    }
}
