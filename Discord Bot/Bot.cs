using System.IO;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.EventArgs;
using DSharpPlus.CommandsNext;
using Newtonsoft.Json;
using DiscordBot.Services;

namespace DiscordBot
{
    /// <summary>
    /// Wrapper around actual bot
    /// </summary>
    public class Bot
    {
        public DiscordClient Client { get; protected set; }
        public CommandsNextExtension Commands { get; protected set; }

        protected JsonConfig jsonConfig;
        protected DiscordConfiguration discordConfig;
        protected CommandsNextConfiguration commandsConfig;
        protected ServicesProvider servicesProvider;

        public void SetupServices(ServicesProvider provider)
        {
            servicesProvider = provider;
        }

        public async Task RunAsync()
        {
            //Reading JSON config file (config.json) for our token
            await ReadJsonConfig();

            discordConfig = SetupDiscordConfig();

            Client = new DiscordClient(discordConfig);
            Client.Ready += OnBotWokeUp;
            Client.SocketErrored += OnSocketError;
            Client.ClientErrored += OnClientError;

            commandsConfig = SetupCommandsConfig();

            //Regestering commands, NOTIFY!: If u do another class with commands, dont forget to register
            Commands = Client.UseCommandsNext(commandsConfig);
            Commands.RegisterCommands<MainCommands>();
            Commands.RegisterCommands<UtilityCommands>();

            await Client.ConnectAsync();

            //If bot quits early, we basically waiting forever so bot doesnt get down after a second
            await Task.Delay(-1);
        }

        private async Task ReadJsonConfig()
        {
            var json = string.Empty;

            using (var fs = File.OpenRead("config.json"))
            {
                using (var sr = new StreamReader(fs, new UTF8Encoding(false)))
                {
                    json = await sr.ReadToEndAsync().ConfigureAwait(false);
                }
            }

            jsonConfig = JsonConvert.DeserializeObject<JsonConfig>(json);
        }

        private DiscordConfiguration SetupDiscordConfig()
        {
            var config = new DiscordConfiguration();
            config.Token = jsonConfig.Token;
            config.TokenType = TokenType.Bot;
            config.AutoReconnect = true;
            config.MinimumLogLevel = Microsoft.Extensions.Logging.LogLevel.Debug;

            return config;
        }

        protected CommandsNextConfiguration SetupCommandsConfig()
        {
            var commands = new CommandsNextConfiguration();
            commands.StringPrefixes = new string[] { jsonConfig.Prefix };
            commands.EnableMentionPrefix = true;
            commands.EnableDms = true;
            commands.DmHelp = true;
            commands.EnableMentionPrefix = true;

            return commands;
        }

        private Task OnClientError(DiscordClient sender, ClientErrorEventArgs e)
        {
            if (e.Handled) return Task.CompletedTask;

            Program.Init();
            return Task.CompletedTask;
        }

        private Task OnSocketError(DiscordClient sender, SocketErrorEventArgs e)
        {
            if (e.Handled) return Task.CompletedTask;

            Program.Init();
            return Task.CompletedTask;
        }

        private Task OnBotWokeUp(DiscordClient client, ReadyEventArgs args)
        {
            return Task.CompletedTask;
        }
    }
}
