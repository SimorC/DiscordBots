using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBots.SteamWorkshopVotingSystem.Service
{
    public interface IBotService
    {
        Task CreateLog(LogMessage logMsg);
        Task MessageReceived(IUserMessage message);
    }
}
