using Microsoft.Extensions.DependencyInjection;
using System;
using System.Reflection;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using DiscordBots.Configuration;
using DiscordBots.SteamWorkshopVotingSystem.Service;

namespace DiscordBots.SteamWorkshopVotingSystem
{
    public class Main
    {
        private readonly DiscordSocketClient _client;
        private readonly CommandService _commands;
        private readonly IServiceProvider _services;
        private readonly IBotService _botService;

        public Main()
        {
            _client = new DiscordSocketClient();
            _commands = new CommandService(new CommandServiceConfig { DefaultRunMode = RunMode.Async });
            _botService = new BotService(_client); // TODO: Add DI

            _services = new ServiceCollection()
                .AddSingleton(_client)
                .AddSingleton(_commands)
                .BuildServiceProvider();
        }

        public async Task StartBot()
        {
            _client.Log += ClientLog;

            await RegisterCommandsAsync();

            await _client.LoginAsync(TokenType.Bot, GlobalVars.VotingSystemToken);
            await _client.StartAsync();

            await Task.Delay(-1);
        }

        private Task ClientLog(LogMessage arg)
            => _botService.CreateLog(arg);

        public async Task RegisterCommandsAsync()
        {
            _client.MessageReceived += MessageReceivedAsync;
            await _commands.AddModulesAsync(Assembly.Load("DiscordBots.SteamWorkshopVotingSystem"), _services);
        }

        private async Task MessageReceivedAsync(SocketMessage arg)
            => await _botService.MessageReceived(arg as SocketUserMessage);
    }
}
