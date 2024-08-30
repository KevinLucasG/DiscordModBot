using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.AsyncEvents;
using DiscordBot.commands;


namespace DiscordBot
{
    class DiscordBot
    {
        static async Task Main(string[] args)
        {
            var discord = new DiscordClient(new DiscordConfiguration
            { 
                Token = "",
                TokenType = TokenType.Bot,
                Intents = DiscordIntents.AllUnprivileged
            });

            var commands = discord.UseCommandsNext(new CommandsNextConfiguration

            {
                StringPrefixes = new[] { "!" } // Prefixo dos Comandos
            });

            commands.RegisterCommands<DiscordCommands>();

            await discord.ConnectAsync();
            await Task.Delay(-1);
        }
    }

}