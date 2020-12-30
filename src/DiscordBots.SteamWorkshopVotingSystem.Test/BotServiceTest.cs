using Discord;
using Discord.WebSocket;
using DiscordBots.Configuration;
using DiscordBots.SteamWorkshopVotingSystem.Service;
using DiscordBots.SteamWorkshopVotingSystem.Test.Factory;
using Moq;
using NUnit.Framework;
using System;
using System.Threading.Tasks;

namespace DiscordBots.SteamWorkshopVotingSystem.Test
{
    public class BotServiceTest
    {
        private IBotService _botService;
        private Mock<DiscordSocketClient> _mockClient;

        [SetUp]
        public void Setup()
        {
            _mockClient = new Mock<DiscordSocketClient>();

            _botService = new BotService(_mockClient.Object);
        }

        [Test]
        public void CreateLog_LogOk_NoThrow()
        {
            Assert.DoesNotThrowAsync(() => _botService.CreateLog(VotingSystemFactory.GetNormalLog()));
        }

        [Test]
        public void CreateLog_NullLogMessage_NoThrow()
        {
            Assert.DoesNotThrowAsync(() => _botService.CreateLog(VotingSystemFactory.GetNullLog()));
        }

        [Test]
        public void MessageReceived_IncorrectChannelUser_NoChanges()
        {
            var userMessage = VotingSystemFactory.GetMockMessage(1, "Test", false);

            Assert.DoesNotThrowAsync(() => _botService.MessageReceived(userMessage.Object));

            // Check reactions = 0
            userMessage.Verify(m => m.AddReactionAsync(It.IsAny<IEmote>(), It.IsAny<RequestOptions>()), Times.Exactly(0));

            // Check deletes = 0
            userMessage.Verify(m => m.DeleteAsync(It.IsAny<RequestOptions>()), Times.Exactly(0));
        }

        [Test]
        public void MessageReceived_IncorrectChannelBot_NoChanges()
        {
            var botMessage = VotingSystemFactory.GetMockMessage(1, "Test", true);

            Assert.DoesNotThrowAsync(() => _botService.MessageReceived(botMessage.Object));

            // Check reactions = 0
            botMessage.Verify(m => m.AddReactionAsync(It.IsAny<IEmote>(), It.IsAny<RequestOptions>()), Times.Exactly(0));

            // Check deletes = 0
            botMessage.Verify(m => m.DeleteAsync(It.IsAny<RequestOptions>()), Times.Exactly(0));
        }

        [Test]
        public void MessageReceived_CorrectPrefixUser_AddEmojis()
        {
            var userMessage = VotingSystemFactory.GetMockMessage(GlobalVars.TableTopTextChannel, GlobalVars.SteamWorkshopPrefix, false);

            Assert.DoesNotThrowAsync(() => _botService.MessageReceived(userMessage.Object));

            // Check reactions = 2
            userMessage.Verify(m => m.AddReactionAsync(It.IsAny<IEmote>(), It.IsAny<RequestOptions>()), Times.Exactly(2));

            // Check deletes = 0
            userMessage.Verify(m => m.DeleteAsync(It.IsAny<RequestOptions>()), Times.Exactly(0));
        }


        [Test]
        public void MessageReceived_CorrectPrefixBot_AddEmojis()
        {
            var botMessage = VotingSystemFactory.GetMockMessage(GlobalVars.TableTopTextChannel, GlobalVars.SteamWorkshopPrefix, true);

            Assert.DoesNotThrowAsync(() => _botService.MessageReceived(botMessage.Object));

            // Check reactions = 2
            botMessage.Verify(m => m.AddReactionAsync(It.IsAny<IEmote>(), It.IsAny<RequestOptions>()), Times.Exactly(2));

            // Check deletes = 0
            botMessage.Verify(m => m.DeleteAsync(It.IsAny<RequestOptions>()), Times.Exactly(0));
        }

        [Test]
        public void MessageReceived_IncorrectPrefixUser_DeleteMessage()
        {
            var userMessage = VotingSystemFactory.GetMockMessage(GlobalVars.TableTopTextChannel, "Test", false);

            Assert.DoesNotThrow(() => _botService.MessageReceived(userMessage.Object));

            // Check reactions = 0
            userMessage.Verify(m => m.AddReactionAsync(It.IsAny<IEmote>(), It.IsAny<RequestOptions>()), Times.Exactly(0));

            // Check deletes = 1
            userMessage.Verify(m => m.DeleteAsync(It.IsAny<RequestOptions>()), Times.Exactly(1));
        }

        [Test]
        public void MessageReceived_IncorrectPrefixUser_NoChanges()
        {
            var botMessage = VotingSystemFactory.GetMockMessage(GlobalVars.TableTopTextChannel, "Test", true);

            Assert.DoesNotThrow(() => _botService.MessageReceived(botMessage.Object));

            // Check reactions = 0
            botMessage.Verify(m => m.AddReactionAsync(It.IsAny<IEmote>(), It.IsAny<RequestOptions>()), Times.Exactly(0));

            // Check deletes = 0
            botMessage.Verify(m => m.DeleteAsync(It.IsAny<RequestOptions>()), Times.Exactly(0));
        }
    }
}