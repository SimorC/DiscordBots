using Discord;
using Discord.Commands;
using Discord.WebSocket;
using DiscordBots.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBots.SteamWorkshopVotingSystem
{
    internal class BotService
    {
        private readonly ulong _channelId = GlobalVars.TestTextChannelId;

        internal static Task CreateLog(LogMessage logMsg)
        {
            // TODO: Create proper log file
            Console.WriteLine(logMsg);
            return Task.CompletedTask;
        }

        internal async Task MessageReceived(SocketUserMessage message, DiscordSocketClient client)
        {
            int argPos = 0;

            var correctChannel = message.Channel.Id == _channelId;
            var correctPrefix = message.HasStringPrefix(GlobalVars.SteamWorkshopPrefix, ref argPos);
            var isBot = message.Author.IsBot;

            var action = (correctChannel, correctPrefix, isBot) switch
            {
                (true, true, _) => AddVotingEmojis(message),
                (true, false, false) => DeleteMessage(message, client),
                _ => Task.CompletedTask
            };

            await action;
        }

        private static async Task AddVotingEmojis(SocketUserMessage message)
        {
            await message.AddReactionAsync(new Emoji("🥵"));
            await message.AddReactionAsync(new Emoji("🥶"));
        }

        private async Task DeleteMessage(SocketUserMessage message, DiscordSocketClient client)
        {
            var observedChannel = client.GetChannel(_channelId) as IMessageChannel;

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
