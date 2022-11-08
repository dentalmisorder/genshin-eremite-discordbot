using System.IO;
using System.Threading.Tasks;
using DSharpPlus.Entities;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;

namespace DiscordBot
{
    class MainCommands : BaseCommandModule
    {
        public string GenshinMaterialsFolder { get; protected set; } = "ascension_materials";

        [Command("materials")]
        [Description("After a materials type a name of a character to get ascension stats card")]
        public async Task GetAscensionMaterials(CommandContext ctx, string characterName)
        {
            var fullPath = Path.Combine(Directory.GetCurrentDirectory(), GenshinMaterialsFolder);

            var allFiles = Directory.GetFiles(fullPath);
            string targetName = string.Empty;

            foreach (var item in allFiles)
            {
                if (!item.Contains(characterName.ToLower())) continue;
                targetName = item;
            }

            if (targetName == string.Empty) return;

            var stream = File.OpenRead(Path.Combine(fullPath, targetName));

            var builder = new DiscordMessageBuilder();

            builder.WithFile(stream);

            await ctx.Channel.SendMessageAsync(builder).ConfigureAwait(false);
        }

        [Command("materials")]
        [Description("After a materials type a name of a character to get ascension stats card")]
        public async Task GetAscensionMaterials(CommandContext ctx, string charFirstName, string charSecondName)
        {
            var fullPath = Path.Combine(Directory.GetCurrentDirectory(), GenshinMaterialsFolder);

            var allFiles = Directory.GetFiles(fullPath);
            string characterName = $"{charFirstName} {charSecondName}";
            string targetName = string.Empty;

            foreach (var item in allFiles)
            {
                if (!item.Contains(characterName.ToLower())) continue;
                targetName = item;
            }

            if (targetName == string.Empty) return;

            var stream = File.OpenRead(Path.Combine(fullPath, targetName));

            var builder = new DiscordMessageBuilder();

            builder.WithFile(stream);

            await ctx.Channel.SendMessageAsync(builder).ConfigureAwait(false);
        }
    }
}
