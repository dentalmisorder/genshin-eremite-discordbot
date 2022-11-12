using DiscordBot.GenshinData;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace DiscordBot.Services
{
    public class EremiteRecruitSystem
    {
        public const string JSON_MERCENARIES_FOLDER = "eremites_recruit_system";
        public const string IMAGE = "banner_eremite_recruit_system.png";
        public const string JSON_MERCENARIES_DATABASE = "eremites_recruits.json";
        public const string DOCUMENTATION_URL = "https://docs.google.com/document/d/1kO8hHnboGeMsSsdFOT-LwKPUa2rxmlWIGya-gj65amg/edit#heading=h.b9o9wrlffcje";

        public List<EremiteRecruit> recruitsCached = new List<EremiteRecruit>();

        public EremiteRecruitSystem()
        {
            string folderPath = Path.Combine(Directory.GetCurrentDirectory(), JSON_MERCENARIES_FOLDER);
            string fullPath = Path.Combine(folderPath, JSON_MERCENARIES_DATABASE);

            if (!Directory.Exists(folderPath)) Directory.CreateDirectory(folderPath);
            if (!File.Exists(fullPath)) return;

            CacheRecruits(fullPath);
        }

        private async Task CacheRecruits(string fullPath)
        {
            string recruitsEnrolledJson = await File.ReadAllTextAsync(fullPath);

            recruitsCached = JsonConvert.DeserializeObject<List<EremiteRecruit>>(recruitsEnrolledJson);

            await Task.CompletedTask;
        }

        public async Task Enroll(CommandContext ctx, int uid)
        {
            string folderPath = Path.Combine(Directory.GetCurrentDirectory(), JSON_MERCENARIES_FOLDER);
            string fullPath = Path.Combine(folderPath, JSON_MERCENARIES_DATABASE);

            if (!Directory.Exists(folderPath)) Directory.CreateDirectory(folderPath);

            bool isAlreadyEnrolled = false;

            if (recruitsCached?.Count >= 1)
            {
                foreach (var recruit in recruitsCached)
                {
                    if (recruit.uid != uid) continue;

                    var builderError = new DiscordMessageBuilder();
                    var buttonLink = new DiscordLinkButtonComponent(DOCUMENTATION_URL, "More about Eremites Recruit System");

                    isAlreadyEnrolled = true;
                    builderError.WithContent($"You already enrolled for Eremites Recruit System this week with UID: {uid}\nThanks for helping with our commisions across Sumeru!");
                    builderError.WithFile(File.OpenRead(Path.Combine(folderPath, IMAGE)));
                    builderError.AddComponents(buttonLink);
                    await ctx.Member.SendMessageAsync(builderError).ConfigureAwait(false);
                }
            }

            if (isAlreadyEnrolled) return;

            EremiteRecruit eremite = new EremiteRecruit(ctx.Client.CurrentUser.Id, uid);
            recruitsCached.Add(eremite);

            await File.WriteAllTextAsync(fullPath, JsonConvert.SerializeObject(recruitsCached));

            var builderSuccess = new DiscordMessageBuilder();
            var buttonAbout = new DiscordLinkButtonComponent(DOCUMENTATION_URL, "More about Eremites Recruit System");

            builderSuccess.WithContent($"You have successfully enrolled for Eremites Recruit System this week with UID: {uid}\nThanks for helping with our commisions! Travel across Sumeru to obtain rare rewards.");
            builderSuccess.WithFile(File.OpenRead(Path.Combine(folderPath, IMAGE)));
            builderSuccess.AddComponents(buttonAbout);
            await ctx.Member.SendMessageAsync(builderSuccess).ConfigureAwait(false);
        }

        public EremiteRecruit GetRandomWinner()
        {
            return recruitsCached[new Random().Next(0, recruitsCached.Count+1)];
        }
    }
}
