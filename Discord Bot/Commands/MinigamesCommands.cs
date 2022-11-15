using DiscordBot.DiscordData;
using DiscordBot.Services;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DiscordBot.Commands
{
    class MinigamesCommands : BaseCommandModule
    {
        private DiscordDataHandler discordDataHandler = null;

        private int maxMoraObtainedByTraveling = 200;
        private int maxPrimosObtainedByTraveling = 80;
        private int hoursTravelRestrict = 1;

        private int fishblastingMinutesRestriction = 5;

        private int maxMoraObtainedByTeapot = 500;
        private int maxPrimosObtainedByTeapot = 160;
        private int daysTeapotRestrict = 1;

        public const string MINGAMES_FOLDER = "minigames";

        public const string TRAVEL_FOLDER = "minigames/travel";
        public const string IMAGE_TRAVEL_BASE = "banner_travel_";
        public const string TEAPOT_IMAGE = "teapot.png";

        private void Initialize()
        {
            discordDataHandler = ServicesProvider.Instance.DiscordDataHandler;
        }

        [Command("travel")]
        [Description("Travel across regionns with Eremites and recruits, help them do commisions and get the chance to obtain rare rewards!")]
        public async Task Travel(CommandContext ctx)
        {
            if (discordDataHandler == null) Initialize();
            UserData user = discordDataHandler.GetUser(ctx.User.Id);
            discordDataHandler.RegisterNewUserIfNeeded(ctx, ref user);

            if (DateTime.Compare(user.timeLastTravel.AddHours(hoursTravelRestrict).ToUniversalTime(), DateTime.Now.ToUniversalTime()) == 1)
            {
                await ctx.Channel.SendMessageAsync($"You can send travel expedition only once an hour! Come check commissions board at {user.timeLastTravel.AddHours(hoursTravelRestrict).ToUniversalTime().ToShortTimeString()} UTC!").ConfigureAwait(false);
                return;
            }

            var award = SentTimeGatedEvent(user, maxMoraObtainedByTraveling, maxPrimosObtainedByTraveling, MinigameType.Travel);
            user.timeLastTravel = DateTime.Now;
            user.timesTraveled++;

            string imgToSet = SetupTravelImage(award.mora, award.primogems);

            string path = Path.Combine(Directory.GetCurrentDirectory(), TRAVEL_FOLDER, $"{IMAGE_TRAVEL_BASE}{imgToSet}.png");
            string content = $"```arm\n {ctx.User.Username} came back after traveling across desert with \n|{award.mora}| Mora\n|{award.primogems}| Primogems.\n```";

            var builder = CreateBuilderWithFileAndContent(path, content);
            await ctx.Channel.SendMessageAsync(builder).ConfigureAwait(false);
        }

        [Command("teapot")]
        [Description("Teapot is a place what you can visit once a day to relax in spa and chill with warm sunsets, also characters who work there will pay you for renting")]
        public async Task Teapot(CommandContext ctx)
        {
            if (discordDataHandler == null) Initialize();
            UserData user = discordDataHandler.GetUser(ctx.User.Id);

            discordDataHandler.RegisterNewUserIfNeeded(ctx, ref user);

            if (DateTime.Compare(user.timeLastTeapotVisit.AddDays(daysTeapotRestrict).ToUniversalTime(), DateTime.Now.ToUniversalTime()) == 1)
            {
                await ctx.Member.SendMessageAsync($"You can visit teapot only once a day! Come check after 24 hours!").ConfigureAwait(false);
                return;
            }

            var award = SentTimeGatedEvent(user, maxMoraObtainedByTeapot, maxPrimosObtainedByTeapot, MinigameType.Teapot);
            user.timeLastTeapotVisit = DateTime.Now;
            user.timesTeapotVisited++;

            string path = Path.Combine(Directory.GetCurrentDirectory(), TRAVEL_FOLDER, TEAPOT_IMAGE);
            string content = $"```arm\n {ctx.User.Username} came back after visiting Teapot with \n|{award.mora}| Mora\n|{award.primogems}| Primogems.\n```";

            var builder = CreateBuilderWithFileAndContent(path, content);
            await ctx.Channel.SendMessageAsync(builder).ConfigureAwait(false);
        }

        [Command("moratop")]
        [Description("Get top players sorted by Mora")]
        private async Task MoraTop(CommandContext ctx)
        {
            if (discordDataHandler == null) Initialize();
            var sortedList = discordDataHandler.GetTop(BestUserType.Mora);

            string message = string.Empty;
            foreach (var user in sortedList)
            {
                message = string.Join('\n', message, $"[{user.username}] | [MORA: {user.wallet.mora}] | [ID:{user.userId}]");
            }

            await ctx.Channel.SendMessageAsync($"```arm\n{message}\n```").ConfigureAwait(false);
        }

        [Command("primostop")]
        [Description("Get top players sorted by Primogems")]
        private async Task PrimosTop(CommandContext ctx)
        {
            if (discordDataHandler == null) Initialize();
            var sortedList = discordDataHandler.GetTop(BestUserType.Primogems);

            string message = string.Empty;
            foreach (var user in sortedList)
            {
                message = string.Join('\n', message, $"[{user.username}] | [PRIMOS: {user.wallet.primogems}] | [ID:{user.userId}]");
            }

            await ctx.Channel.SendMessageAsync($"```arm\n{message}\n```").ConfigureAwait(false);
        }

        [Command("pullstop")]
        [Description("Get top players sorted by how many times user used !pull to get character")]
        private async Task PullsTop(CommandContext ctx)
        {
            if (discordDataHandler == null) Initialize();
            var sortedList = discordDataHandler.GetTop(BestUserType.PullingTimes);

            string message = string.Empty;
            foreach (var user in sortedList)
            {
                message = string.Join('\n', message, $"[{user.username}] | [PULLS: {user.timesPulled}] | [ID:{user.userId}]");
            }

            await ctx.Channel.SendMessageAsync($"```arm\n{message}\n```").ConfigureAwait(false);
        }

        private Award SentTimeGatedEvent(UserData user, int maxMora, int maxPrimos, MinigameType minigame)
        {
            var rnd = new Random();
            int moraFound = rnd.Next(0, maxMora);
            int primogemsFound = rnd.Next(0, maxPrimos);
            var award = new Award(moraFound, primogemsFound);

            int perk = user.currentEquippedCharacter == null ? 0 : user.currentEquippedCharacter.perkStat;
            ApplyPerk(award, perk, minigame);

            user.wallet.mora += award.mora;
            user.wallet.primogems += award.primogems;

            return award;
        }

        private DiscordMessageBuilder CreateBuilderWithFileAndContent(string filePath, string content)
        {
            var stream = File.OpenRead(filePath);

            var builder = new DiscordMessageBuilder();

            builder.WithContent(content);
            builder.WithFile(stream);

            return builder;
        }

        /// <summary>
        /// Also cringe construction, TODO: refactor, maybe do Dictionary<Award, CorrespondingImage>
        /// </summary>
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

        private void ApplyPerk(Award award, int perk, MinigameType minigame)
        {

            switch (perk)
            {
                case (int)Perk.DOUBLE_MORA:
                    award.mora = award.mora * 2;
                    break;

                case (int)Perk.DOUBLE_MORA_LOWER_PRIMOS:
                    award.mora = award.mora * 2;
                    award.primogems = award.primogems > 0 ? award.primogems / 2 : 0;
                    break;

                case (int)Perk.DOUBLE_PRIMOS:
                    award.primogems = award.primogems * 2;
                    break;

                case (int)Perk.DOUBLE_PRIMOS_LOWER_MORA:
                    award.primogems = award.primogems * 2;
                    award.mora = award.mora > 0 ? award.mora / 2 : 0;
                    break;

                case (int)Perk.TWICE_TRAVEL_BOUNTY:
                    if (minigame != MinigameType.Travel) break;
                    award.mora = award.mora * 2;
                    award.primogems = award.primogems * 2;
                    break;

                default:
                    break;
            }
        }
    }
}
