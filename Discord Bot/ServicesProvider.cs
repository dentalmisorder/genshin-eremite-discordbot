namespace DiscordBot.Services
{
    public class ServicesProvider
    {
        public NamecardsHandler NamecardsHandler { get; private set; }
        public GenshinDataHandler GenshinDataHandler { get; private set; }

        public ServicesProvider()
        {
            GenshinDataHandler = new GenshinDataHandler();
            NamecardsHandler = new NamecardsHandler();

            NamecardsHandler.Initialize();
        }
    }
}
