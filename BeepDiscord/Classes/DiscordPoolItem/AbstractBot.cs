using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using BeepDiscord.Attribs;
using BeepDiscord.Utilities;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using Microsoft.Extensions.Logging;

namespace BeepDiscord.Classes.DiscordPoolItem
{
  public abstract class AbstractBot
    {
        protected bool Isconnected { get; set; } = false;
        private DiscordShardedClient BotInstance { get; set; }
        protected LogLevel Loglevel { get; set; }

        protected DiscordUser _BOT_USER { get; set; }
        protected string _ID { get; set; } = RandomGenerator.RandomString(Const.Consts._RANDOM_ID_LEN);
        protected string _TOKEN { get; set; }


        public AbstractBot(string Token, string ID, LogLevel Loglevel = LogLevel.Information)
        {
            try
            {
                this._TOKEN = Token;
                this._ID = ID == "" ? this._ID : ID;
                this.Loglevel = Loglevel;
                this.BotInstance = new DiscordShardedClient(new DiscordConfiguration()
                {
                    TokenType = TokenType.Bot,
                    Token = this._TOKEN,
                    MinimumLogLevel = this.Loglevel,
                    LogTimestampFormat = "MMM dd yyyy - hh:mm:ss tt"
                });
      }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public virtual  Task Start()
        {
            if (!Isconnected)
            {
                return Task.Run(async () =>
                {
                    await BotInstance.StartAsync();
                    this._BOT_USER = BotInstance.CurrentUser;
                    BotInstance.MessageCreated += BotInstanceOnMessageCreated;
                    BotInstance.MessageDeleted += BotInstanceOnMessageDeleted;
                    BotInstance.MessageUpdated += BotInstanceOnMessageUpdated;
                    BotInstance.MessageReactionAdded += BotInstanceOnMessageReactionAdded;
                    BotInstance.MessageReactionRemoved += BotInstanceOnMessageReactionRemoved;
                    BotInstance.SocketErrored += ErrorHandling;
                    Isconnected = true;
                });
            }
            return Task.CompletedTask;
        }

         public abstract Task ErrorHandling(DiscordClient sender, SocketErrorEventArgs e);
        private async Task BotInstanceOnMessageReactionRemoved(DiscordClient sender, MessageReactionRemoveEventArgs e)
        {
            await Task.Run(() =>
            {
                var    constinfo = this.GetType().GetMethods()
                    .Where(m => m.GetCustomAttributes(typeof(BotMessageReactionRemoved), false).Length > 0);
                Parallel.ForEach(constinfo, methodinfo =>
                {
                    var arrtibarray = methodinfo.GetCustomAttributes<BotMessageReactionRemoved>();

                    foreach (var VARIABLE in arrtibarray)
                    {
                        methodinfo.Invoke(this, new object[] {sender, e});
                    }
                });
            });
        }

        private async Task BotInstanceOnMessageReactionAdded(DiscordClient sender, MessageReactionAddEventArgs e)
        {
            await Task.Run(() =>
            {
                var    constinfo = this.GetType().GetMethods()
                    .Where(m => m.GetCustomAttributes(typeof(BotMessageReactionAdded), false).Length > 0);
                Parallel.ForEach(constinfo, methodinfo =>
                {
                    var arrtibarray = methodinfo.GetCustomAttributes<BotMessageReactionAdded>();

                    foreach (var VARIABLE in arrtibarray)
                    {
                        methodinfo.Invoke(this, new object[] {sender, e  });
                    }
                });
            });
        }

        private async Task BotInstanceOnMessageUpdated(DiscordClient sender, MessageUpdateEventArgs e)
        {
            await Task.Run(() =>
            {
                var    constinfo = this.GetType().GetMethods()
                    .Where(m => m.GetCustomAttributes(typeof(BotMessageUpdate), false).Length > 0);
                var msgwithargs = e.Message.Content.Split(" ".ToCharArray()[0]);
                Parallel.ForEach(constinfo, methodinfo =>
                {
                    var arrtibarray = methodinfo.GetCustomAttributes<BotMessageUpdate>();

                    foreach (var VARIABLE in arrtibarray)
                    {
                        methodinfo.Invoke(this, new object[] {sender, e ,  msgwithargs.Count() > 1 ? msgwithargs.ToList().GetRange(1,msgwithargs.Count()-1).ToArray() : new string[]{}  });
                    }
                });
            });
        }

        private async Task BotInstanceOnMessageDeleted(DiscordClient sender, MessageDeleteEventArgs e)
        {
            await Task.Run(() =>
            {
                var    constinfo = this.GetType().GetMethods()
                    .Where(m => m.GetCustomAttributes(typeof(BotMessageRemoved), false).Length > 0);
                var msgwithargs = e.Message.Content.Split(" ".ToCharArray()[0]);
                Parallel.ForEach(constinfo, methodinfo =>
                {
                    var arrtibarray = methodinfo.GetCustomAttributes<BotMessageRemoved>();

                    foreach (var VARIABLE in arrtibarray)
                    {
                        methodinfo.Invoke(this, new object[] {sender, e ,  msgwithargs.Count() > 1 ? msgwithargs.ToList().GetRange(1,msgwithargs.Count()-1).ToArray() : new string[]{}  });
                    }
                });
            });
        }

        private async Task BotInstanceOnMessageCreated(DiscordClient sender, MessageCreateEventArgs e)
        {
            await Task.Run(() =>
            {
            var    constinfo = this.GetType().GetMethods()
                    .Where(m => m.GetCustomAttributes(typeof(BotMessageResponse),true ).Length > 0);
            var msgwithargs = e.Message.Content.Split(" ".ToCharArray()[0]);
                Parallel.ForEach(constinfo, methodinfo =>
                {
                    var arrtibarray = methodinfo.GetCustomAttributes<BotMessageResponse>();

                    foreach (var VARIABLE in arrtibarray)
                    {
                        if (VARIABLE.IncommingMessageBody == msgwithargs[0])
                        {
                            methodinfo.Invoke(this, new object[] {sender, e ,  msgwithargs.Count() > 1 ? msgwithargs.ToList().GetRange(1,msgwithargs.Count()-1).ToArray() : new string[]{}  });
                        }
                    }
                });
            });
        }


        public  Task Stop()
        {
            return Task.Run(() =>
            {
                if (!Isconnected)
                {
                    BotInstance.StopAsync().Wait();
                    Isconnected = false;
                } ;
              
            });
        }

        public async Task Reconnect()
        {
            await Stop();
            await Start();
        }
    }
}