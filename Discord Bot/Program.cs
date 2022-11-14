using DiscordBot.Services;

namespace DiscordBot
{
    public class Program
    {
        static void Main(string[] args)
        {
            new ServicesProvider();

            Init();
        }

        public static void Init()
        {
            var bot = new Bot();

            var task = bot.RunAsync();

            task.GetAwaiter().GetResult();

            //Just in case, cause sometimes i look in her eyes and thats where i find the glimpse of us
            //.. i mean.. sometimes bot just finished the task even tho its Delay(-1), so we Re-Init

            bot.Client.DisconnectAsync();
            Init();
        }
    }
}
