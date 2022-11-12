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
        public const string JSON_MERCENARIES_DATABASE = "eremites_recruits.json";

        public List<EremiteRecruit> recruitsCached = new List<EremiteRecruit>();

        public async void Initialize()
        {
            string folderPath = Path.Combine(Directory.GetCurrentDirectory(), JSON_MERCENARIES_FOLDER);
            string fullPath = Path.Combine(folderPath, JSON_MERCENARIES_DATABASE);

            if (!Directory.Exists(folderPath)) Directory.CreateDirectory(folderPath);
            if (!File.Exists(fullPath)) return;

            string recruitsEnrolledJson = await File.ReadAllTextAsync(fullPath);

            recruitsCached = JsonConvert.DeserializeObject<List<EremiteRecruit>>(recruitsEnrolledJson);
            
        }

        public async Task Enroll(CommandContext ctx, int uid)
        {
            string folderPath = Path.Combine(Directory.GetCurrentDirectory(), JSON_MERCENARIES_FOLDER);
            string fullPath = Path.Combine(folderPath, JSON_MERCENARIES_DATABASE);

            if (!Directory.Exists(folderPath)) Directory.CreateDirectory(folderPath);

            if (recruitsCached?.Count >= 1)
            {
                foreach (var recruit in recruitsCached)
                {
                    if (recruit.uid != uid) continue;

                    var builderError = new DiscordMessageBuilder();
                    //var buttonLink = new DiscordLinkButtonComponent();
                    //builderError.AddComponents()
                    builderError.WithContent($"You already enrolled for Eremites Recruit System this week with UID: {uid}\nThanks for helping with our commisions across Sumeru!");
                    await ctx.Member.SendMessageAsync(builderError);
                    return;
                }
            }

            EremiteRecruit eremite = new EremiteRecruit(ctx.Client.CurrentUser.Id, uid);
            recruitsCached.Add(eremite);

            await File.WriteAllTextAsync(fullPath, JsonConvert.SerializeObject(recruitsCached));

            var builderSuccess = new DiscordMessageBuilder();
            builderSuccess.WithContent($"You have successfully enrolled for Eremites Recruit System this week with UID: {uid}\nThanks for helping with our commisions! Travel across Sumeru to obtain rare rewards.");
            await ctx.Member.SendMessageAsync(builderSuccess);
        }
    }
}
