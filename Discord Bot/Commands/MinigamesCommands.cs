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

        private int maxMoraObtainedByTraveling = 350;
        private int maxPrimosObtainedByTraveling = 80;
        private int hoursTravelRestrict = 1;

        private int minutesCooldownDependingOnTeam = 3;
        private int chancesPerCharacterResetCDTravel = 5;

        private int lowAmountPrimogemsPerk = 10;

        private int chanceCAPReset = 75;
        private int minutesCAPReset = 40;

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
            user.timeLastTravel = DateTime.Now;
            user.timesTraveled++;

            var award = SentTimeGatedEvent(ref user, maxMoraObtainedByTraveling, maxPrimosObtainedByTraveling, ctx, MinigameType.Travel);

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
            user.timeLastTeapotVisit = DateTime.Now;
            user.timesTeapotVisited++;

            var award = SentTimeGatedEvent(ref user, maxMoraObtainedByTeapot, maxPrimosObtainedByTeapot, ctx, MinigameType.Teapot);

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
            for (int i = 0; i < sortedList.Count; i++)
            {
                message = string.Join('\n', message, $"[{sortedList[i].username}] | [MORA: {sortedList[i].wallet.mora}] | [ID:{sortedList[i].userId}]");
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
            for (int i = 0; i < sortedList.Count; i++)
            {
                message = string.Join('\n', message, $"[{sortedList[i].username}] | [PRIMOS: {sortedList[i].wallet.primogems}] | [ID:{sortedList[i].userId}]");
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
            for (int i = 0; i < sortedList.Count; i++)
            {
                message = string.Join('\n', message, $"[{sortedList[i].username}] | [PULLS: {sortedList[i].timesPulled}] | [ID:{sortedList[i].userId}]");
            }

            await ctx.Channel.SendMessageAsync($"```arm\n{message}\n```").ConfigureAwait(false);
        }

        [Command("traveltop")]
        [Description("Get top players sorted by how many times user used !pull to get character")]
        private async Task TravelTop(CommandContext ctx)
        {
            if (discordDataHandler == null) Initialize();
            var sortedList = discordDataHandler.GetTop(BestUserType.TraveledTimes);

            string message = string.Empty;
            for (int i = 0; i < sortedList.Count; i++)
            {
                message = string.Join('\n', message, $"[{sortedList[i].username}] | [TRAVELS: {sortedList[i].timesTraveled}] | [ID:{sortedList[i].userId}]");
            }

            await ctx.Channel.SendMessageAsync($"```arm\n{message}\n```").ConfigureAwait(false);
        }

        [Command("welkintop")]
        [Description("Get top players sorted by how many times user used !pull to get character")]
        private async Task WelkinTop(CommandContext ctx)
        {
            if (discordDataHandler == null) Initialize();
            var sortedList = discordDataHandler.GetTop(BestUserType.WelkinWonTimes);

            string message = string.Empty;
            for (int i = 0; i < sortedList.Count; i++)
            {
                message = string.Join('\n', message, $"[{sortedList[i].username}] | [WELKIN WON: {sortedList[i].timesWelkinWon}] | [ID:{sortedList[i].userId}]");
            }

            await ctx.Channel.SendMessageAsync($"```arm\n{message}\n```").ConfigureAwait(false);
        }

        [Command("teapottop")]
        [Description("Get top players sorted by how many times user used !pull to get character")]
        private async Task TeapotTop(CommandContext ctx)
        {
            if (discordDataHandler == null) Initialize();
            var sortedList = discordDataHandler.GetTop(BestUserType.TeapotVisitedTimes);

            string message = string.Empty;
            for (int i = 0; i < sortedList.Count; i++)
            {
                message = string.Join('\n', message, $"[{sortedList[i].username}] | [TEAPOT VISITED: {sortedList[i].timesTeapotVisited}] | [ID:{sortedList[i].userId}]");
            }

            await ctx.Channel.SendMessageAsync($"```arm\n{message}\n```").ConfigureAwait(false);
        }

        private Award SentTimeGatedEvent(ref UserData user, int maxMora, int maxPrimos, CommandContext ctx, MinigameType minigame)
        {
            var rnd = new Random();
            int moraFound = rnd.Next(0, maxMora);
            int primogemsFound = rnd.Next(0, maxPrimos);
            var award = new Award(moraFound, primogemsFound);

            int perk = user.currentEquippedCharacter == null ? 0 : user.currentEquippedCharacter.perkStat;
            user = ApplyPerk(award, perk, user, ctx, minigame);

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

        private UserData ApplyPerk(Award award, int perk, UserData user, CommandContext ctx, MinigameType minigame)
        {
            int chance = 0;

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
                case (int)Perk.LOWER_TRAVEL_COOLDOWN_DEPEND_TEAM:
                    if (minigame != MinigameType.Travel) break;
                    int minutesCooldownDecrease = -1 * (minutesCooldownDependingOnTeam * user.characters.Count);
                    minutesCooldownDecrease = minutesCooldownDecrease > minutesCAPReset ? minutesCAPReset : minutesCooldownDecrease;
                    user.timeLastTravel = user.timeLastTravel.AddMinutes(minutesCooldownDecrease);
                    ctx.Channel.SendMessageAsync($"```Your cooldown of !travel was decreased by {minutesCooldownDecrease} minutes```");
                    break;

                case (int)Perk.CHANCE_TO_RESET_COOLDOWN_TRAVEL_BASED_TEAM:
                    if (minigame != MinigameType.Travel) break;
                    if(GenerateChance(user.characters.Count))
                    {
                        user.timeLastTravel = user.timeLastTravel.AddDays(-hoursTravelRestrict);
                        ctx.Channel.SendMessageAsync("```You got buff perk proc and your !travel cd restored.```");
                    }
                    break;
                case (int)Perk.CONVERT_MORA_INTO_PRIMOGEMS_TRAVEL:
                    if (minigame != MinigameType.Travel) break;
                    award = ConvertMoraInPrimos(ctx, award);
                    break;
                case (int)Perk.CONVERT_MORA_INTO_PRIMOGEMS_ALL:
                    award = ConvertMoraInPrimos(ctx, award);
                    break;
                case (int)Perk.DOUBLE_CHANCE_TO_RESET_COOLDOWN_TEAPOT_BASED_TEAM:
                    if (minigame != MinigameType.Teapot) break;
                    if(GenerateChance(user.characters.Count * 2))
                    {
                        user.timeLastTeapotVisit = user.timeLastTeapotVisit.AddDays(-daysTeapotRestrict);
                        ctx.Channel.SendMessageAsync("```You got buff perk proc and your !teapot cd restored.```");
                    }
                    break;

                default:
                    break;
            }

            return user;
        }

        private bool GenerateChance(int chancesGenerateBasedOn)
        {
            int chance = chancesGenerateBasedOn * chancesPerCharacterResetCDTravel;
            chance = chance > chanceCAPReset ? chanceCAPReset : chance;
            Random rnd = new Random();
            return rnd.Next(0, 100) < chance;
        }

        private Award ConvertMoraInPrimos(CommandContext ctx, Award award)
        {
            if (award.mora > 2)
            {
                int converted = (int)(award.mora / 2);
                award.primogems += converted;
                award.mora = 0;

                ctx.Channel.SendMessageAsync($"```[SACRIFICE Proc] Your Mora was converted with 1/2 ratio into primogems. Additional Primogems amount: {converted}```");
            }

            return award;
        }
    }
}
