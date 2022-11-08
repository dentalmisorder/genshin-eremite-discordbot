namespace DiscordBot
{
    public class Program
    {
        static void Main(string[] args)
        {
            var bot = new Bot();
            bot.RunAsync().GetAwaiter().GetResult(); 

            //we are using GetAwaiter() in case if u want to put code
            //beneath so it doesnt get stuck on this line
        }
    }
}
