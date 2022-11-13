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

        [Command("akasha")]
        [Description("Shows your Akasha Terminal profile, which records all your data, you can see there your main equipped character and their buffs, as well as Mora and Primogems")]
        public async Task Akasha(CommandContext ctx)
        {
            if (discordDataHandler == null) Initialize();

            discordDataHandler.ShowAkashaProfile(ctx);
        }

        [Command("profile")]
        [Description("Shows your Akasha Terminal profile, which records all your data, you can see there your main equipped character and their buffs, as well as Mora and Primogems")]
        public async Task Profile(CommandContext ctx)
        {
            if (discordDataHandler == null) Initialize();

            discordDataHandler.ShowAkashaProfile(ctx);
        }

        [Command("pulls")]
        [Description("Pull for characters to get them in your inventory, !setcharacter to set character as equipped, this way you will get buffs (like x2 mora, or EVEN DOUBLE CHANCES ON WELKIN MOON!)")]
        public async Task Pull(CommandContext ctx, [Description("How many pulls to do?")] int times)
        {
            if (discordDataHandler == null) Initialize();
            if (times <= 0) return;

            var user = discordDataHandler.GetUser(ctx.User.Id);
            discordDataHandler.RegisterNewUserIfNeeded(ctx, ref user);

            if (user == null || user.wallet.primogems < DiscordDataHandler.PULL_COST)
            {
                await ctx.Channel.SendMessageAsync("Sorry, you dont have primogems to make a wish. Try look for commissions or !travel across Teyvat").ConfigureAwait(false);
                return;
            }

            for (int i = 0; i < times; i++)
            {
                if (user.wallet.primogems >= DiscordDataHandler.PULL_COST)
                {
                    user.wallet.primogems -= DiscordDataHandler.PULL_COST;
                    await discordDataHandler.Pull(ctx, user);
                }
            }
        }

        [Command("pull")]
        [Description("Pull for characters to get them in your inventory, !setcharacter to set character as equipped, this way you will get buffs (like x2 mora, or EVEN DOUBLE CHANCES ON WELKIN MOON!)")]
        public async Task Pull(CommandContext ctx)
        {
            if (discordDataHandler == null) Initialize();

            var user = discordDataHandler.GetUser(ctx.User.Id);

            discordDataHandler.RegisterNewUserIfNeeded(ctx, ref user);

            if (user == null || user.wallet.primogems < DiscordDataHandler.PULL_COST)
            {
                await ctx.Channel.SendMessageAsync("Sorry, you dont have primogems to make a wish. Try look for commissions or !travel across Teyvat").ConfigureAwait(false);
                return;
            }
            if (user.wallet.primogems >= DiscordDataHandler.PULL_COST)
            {
                user.wallet.primogems -= DiscordDataHandler.PULL_COST;
                await discordDataHandler.Pull(ctx, user);
            }
        }

        //You can use roles to define who can use commands, as example:
        //[RequireRoles(RoleCheckMode.Any, NSFW_ROLE, "Administrator")]
    }
}
