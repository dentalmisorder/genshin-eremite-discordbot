using System;

namespace DiscordBot.Services
{
    public class ServicesProvider
    {
        public static ServicesProvider Instance;

        public NamecardsHandler NamecardsHandler { get; private set; }
        public GenshinDataHandler GenshinDataHandler { get; private set; }
        public EremiteRecruitSystem EremiteRecruitSystem { get; private set; }
        public DiscordDataHandler DiscordDataHandler { get; private set; }
        public CashbackService CashbackService { get; private set; }

        public ServicesProvider()
        {
            if (Instance != null) return;
            Instance = this;

            GenshinDataHandler = new GenshinDataHandler();
            NamecardsHandler = new NamecardsHandler();
            EremiteRecruitSystem = new EremiteRecruitSystem();
            DiscordDataHandler = new DiscordDataHandler();
            CashbackService = new CashbackService();
        }
    }
}
