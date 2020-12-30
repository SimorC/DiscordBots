using Discord;
using Discord.WebSocket;
using Moq;

namespace DiscordBots.SteamWorkshopVotingSystem.Test.Factory
{
    internal class VotingSystemFactory
    {
        public static LogMessage GetNormalLog()
            => new(LogSeverity.Info, "MockSource", "Normal message");

        public static LogMessage GetNullLog()
            => new(LogSeverity.Info, "MockSource", null);

        public static IMessageChannel GetMockTextChannel()
        {
            var mockSocketChannel = new Mock<IMessageChannel>();
            mockSocketChannel.Setup(m =>
                m.SendMessageAsync(
                    It.IsAny<string>(),
                    It.IsAny<bool>(),
                    It.IsAny<Embed>(),
                    It.IsAny<RequestOptions>()));

            return mockSocketChannel.Object;
        }

        public static Mock<IUserMessage> GetMockMessage(ulong channelId, string message, bool isBot)
        {
            var userMessage = new Mock<IUserMessage>();
            
            userMessage.SetupGet(um => um.Content).Returns(message);
            userMessage.SetupGet(um => um.Author.IsBot).Returns(isBot);
            userMessage.SetupGet(um => um.Channel.Id).Returns(channelId);

            userMessage.Setup(m => m.AddReactionAsync(It.IsAny<IEmote>(), It.IsAny<RequestOptions>()));
            userMessage.Setup(m => m.DeleteAsync(It.IsAny<RequestOptions>()));

            return userMessage;
        }
    }
}
