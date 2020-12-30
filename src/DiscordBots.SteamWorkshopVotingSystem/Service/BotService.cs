using Discord;
using Discord.Commands;
using Discord.WebSocket;
using DiscordBots.Configuration;
using System;
using System.Threading.Tasks;

namespace DiscordBots.SteamWorkshopVotingSystem.Service
{
    public class BotService : IBotService
    {
        private readonly ulong _channelId = GlobalVars.TableTopTextChannel;
        private readonly DiscordSocketClient _client;

        public BotService(DiscordSocketClient client)
        {
            _client = client;
        }

        public Task CreateLog(LogMessage logMsg)
        {
            // TODO: Create proper log file
            Console.WriteLine(logMsg);
            return Task.CompletedTask;
        }

        public async Task MessageReceived(IUserMessage message)
        {
            int argPos = 0;

            var correctChannel = message.Channel.Id == _channelId;
            var correctPrefix = message.HasStringPrefix(GlobalVars.SteamWorkshopPrefix, ref argPos);
            var isBot = message.Author.IsBot;

            var action = (correctChannel, correctPrefix, isBot) switch
            {
                (true, true, _) => AddVotingEmojis(message),
                (true, false, false) => DeleteMessage(message),
                _ => Task.CompletedTask
            };

            await action;
        }

        private static async Task AddVotingEmojis(IMessage message)
        {
            await message.AddReactionAsync(new Emoji("🥵"));
            await message.AddReactionAsync(new Emoji("🥶"));
        }

        private async Task DeleteMessage(IUserMessage message)
        {
            var observedChannel = _client.GetChannel(_channelId) as IMessageChannel;

            await DeleteMessage(message, 0);

            var messageAsync = await observedChannel.SendMessageAsync($"Only steam workshop links allowed, {message.Author.Username}!");

            await DeleteMessage(messageAsync, 4000);
        }

        private static async Task DeleteMessage(IUserMessage message, int delay)
        {
            await Task.Run(() =>
            {
                System.Threading.Thread.Sleep(delay);
                message.DeleteAsync();
            });
        }
    }
}
