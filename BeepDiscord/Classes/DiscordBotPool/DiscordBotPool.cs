using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using BeepDiscord.Classes.DiscordPoolItem;

namespace BeepDiscord.Classes.DiscordBotPool
{
    public class DiscordBotPool
    {
        public List<AbstractBot> ListofBots = new();

        public Task<bool> Start()
        {
            return Task.Run(() =>
            {
               foreach (var bot in ListofBots)
                {
                    bot.Start();
                }
                return true;
            });
        }
    }
}