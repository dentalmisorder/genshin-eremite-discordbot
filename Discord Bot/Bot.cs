using System.IO;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.EventArgs;
using DSharpPlus.CommandsNext;
using Newtonsoft.Json;
using DiscordBot.Commands;
using System;

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

        public async Task RunAsync()
        {
            //Reading JSON config file (config.json) for our token
            ReadJsonConfig();

            discordConfig = SetupDiscordConfig();

            Client = new DiscordClient(discordConfig);
            Client.SocketErrored += OnSocketError;
            Client.ClientErrored += OnClientError;
            Client.Zombied += Zombied;

            commandsConfig = SetupCommandsConfig();

            //Regestering commands, NOTIFY!: If u do another class with commands, dont forget to register
            Commands = Client.UseCommandsNext(commandsConfig);
            Commands.RegisterCommands<MainCommands>();
            Commands.RegisterCommands<UtilityCommands>();
            Commands.RegisterCommands<MinigamesCommands>();

            await Client.ConnectAsync().ConfigureAwait(false);

            //If bot quits early, we basically waiting forever so bot doesnt get down after a second
            await Task.Delay(-1);

            Reinit(Client);
        }


        private void ReadJsonConfig()
        {
            string json = File.ReadAllText("config.json");

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
            commands.EnableDms = false;
            commands.DmHelp = true;
            commands.EnableMentionPrefix = true;

            return commands;
        }

        private Task OnClientError(DiscordClient sender, ClientErrorEventArgs e)
        {
            Console.WriteLine("OnClientError...");
            Reinit(sender);

            return Task.CompletedTask;
        }

        private Task Zombied(DiscordClient sender, ZombiedEventArgs e)
        {
            Console.WriteLine("Zombied....");
            Reinit(sender);

            return Task.CompletedTask;
        }

        private Task OnSocketError(DiscordClient sender, SocketErrorEventArgs e)
        {
            Console.WriteLine("OnSocketError...");
            Reinit(sender);

            return Task.CompletedTask;
        }

        private async Task Reinit(DiscordClient client)
        {
            await client.DisconnectAsync();

            Console.WriteLine("Reinit, disconnecting client, re-initing..");
            Program.Init();
        }
    }
}
