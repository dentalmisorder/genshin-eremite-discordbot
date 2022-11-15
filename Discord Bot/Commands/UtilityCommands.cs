using System;
using System.IO;
using System.Threading.Tasks;
using DSharpPlus.Entities;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DiscordBot.Services;
using DiscordBot.DiscordData;
using System.Collections.Generic;

namespace DiscordBot.Commands
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
        public async Task Pulls(CommandContext ctx, [Description("How many pulls to do?")] int times)
        {
            if (discordDataHandler == null) Initialize();
            if (times <= 0) return;

            var user = discordDataHandler.GetUser(ctx.User.Id);
            discordDataHandler.RegisterNewUserIfNeeded(ctx, ref user);

            if (user == null || user.wallet.primogems < times * DiscordDataHandler.PULL_COST)
            {
                await ctx.Channel.SendMessageAsync("Sorry, you dont have primogems to make that many wishes. Try look for commissions or !travel across Teyvat or lower the amount of wishes").ConfigureAwait(false);
                return;
            }

            user.wallet.primogems -= times * DiscordDataHandler.PULL_COST;
            user.timesPulled += times;

            Character bestCharacter = null;
            List<string> charactersPulled = new List<string>();
            Random rnd = new Random();

            for (int i = 0; i < times; i++)
            {
                var characterPulled = discordDataHandler.PullSilent(rnd);
                user.AddPulledCharacter(characterPulled);

                if (bestCharacter == null) bestCharacter = characterPulled;
                if (bestCharacter.starsRarity < characterPulled.starsRarity) bestCharacter = characterPulled;
                if (!charactersPulled.Contains(characterPulled.characterName)) charactersPulled.Add(characterPulled.characterName);
            }

            string messageCharacters = string.Join('|', charactersPulled);

            string content = $"```{ctx.User.Username} pulled these characters: {messageCharacters}\nBest one: {bestCharacter.characterName}```";
            var builder = new DiscordMessageBuilder();

            builder.WithFile(File.OpenRead(Path.Combine(Directory.GetCurrentDirectory(), DiscordDataHandler.CHARACTER_FOLDER, bestCharacter.imagePullBannerPath)));
            builder.WithContent(content);

            await ctx.Channel.SendMessageAsync(builder).ConfigureAwait(false);
        }

        [Command("pull")]
        [Description("Pull for characters to get them in your inventory, !  character to set character as equipped, this way you will get buffs (like x2 mora, or EVEN DOUBLE CHANCES ON WELKIN MOON!)")]
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
                user.timesPulled++;
                await discordDataHandler.Pull(ctx, user).ConfigureAwait(false);
            }
        }

        [Command("setcharacter")]
        public async Task SetMainCharacter(CommandContext ctx, string characterName)
        {
            await SetMainCharacter(ctx, characterName, string.Empty).ConfigureAwait(false);
        }

        [Command("setcharacter")]
        public async Task SetMainCharacter(CommandContext ctx, string characterName, string characterSurname)
        {
            if (discordDataHandler == null) Initialize();

            UserData user = discordDataHandler.GetUser(ctx.User.Id);
            string twoPieceName = $"{characterName} {characterSurname}";
            if (user == null)
            {
                await ctx.Channel.SendMessageAsync("We had no data about you in our Eremites Database, we gonna register you right now, but you probably dont have characters to set right now, try !pull for characters :)").ConfigureAwait(false);
                discordDataHandler.RegisterNewUserIfNeeded(ctx, ref user);
                return;
            }

            Character characterToSet = null;

            foreach (var character in user.characters)
            {
                string charName = character.characterName.ToLower();
                if (!(characterName.ToLower().Contains(charName) || twoPieceName.ToLower().Contains(charName))) continue;

                characterToSet = character;
            }

            user.currentEquippedCharacter = characterToSet;

            if (user.currentEquippedCharacter == null)
            {
                await ctx.Channel.SendMessageAsync($"Your !akasha profile shows you dont own this character (use !pull to get one).").ConfigureAwait(false);
                return;
            }

            await ctx.Channel.SendMessageAsync($"{ctx.User.Username} sets his main character as {user.currentEquippedCharacter.characterName}").ConfigureAwait(false);
        }

        //You can use roles to define who can use commands, as example:
        //[RequireRoles(RoleCheckMode.Any, NSFW_ROLE, "Administrator")]
    }
}
