using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;

namespace EhrenBot
{
    class Bot
    {

        private readonly DiscordSocketClient client;
        private Dictionary<ulong, int> scoreBoard;


        public Bot()
        {
            scoreBoard = new Dictionary<ulong, int>();

            client = new DiscordSocketClient();

            client.ReactionAdded += Client_ReactionAdded;
            client.ReactionRemoved += Client_ReactionRemoved;
            client.MessageReceived += Client_MessageReceived;
        }

        public async Task startup()
        {
            await client.LoginAsync(TokenType.Bot, "NzU1MzgwOTkyODU2MDMxMjY0.X2CdXg.1hlag9eLcYYdT_ErRslo9aiPh1c");
            Console.WriteLine("Ehrenbot starting up");
            await client.StartAsync();
            Console.WriteLine("Ehrenbot started");

            await Task.Delay(Timeout.Infinite);
        }

        private Task Client_MessageReceived(SocketMessage message)
        {

            if (message.Content.Equals("Wie stehts um die Ehre ?")) {
                message.Channel.SendMessageAsync(scoreBoardToString());  
            }

            return Task.CompletedTask;
        }

        private Task Client_ReactionRemoved(Cacheable<IUserMessage, ulong> before, ISocketMessageChannel channel, SocketReaction reaction)
        {

            Task<IUserMessage> message = before.GetOrDownloadAsync();


            if (!reaction.Emote.Name.Equals("ehre"))
            {
                return Task.CompletedTask;
            }

            if (!scoreBoard.ContainsKey(message.Result.Author.Id))
            {
                return Task.CompletedTask;
            }

            scoreBoard[message.Result.Author.Id] -= 1;

            return Task.CompletedTask;
        }

        private Task Client_ReactionAdded(Cacheable<IUserMessage, ulong> before, ISocketMessageChannel channel, SocketReaction reaction)
        {

            Task<IUserMessage> message  = before.GetOrDownloadAsync();


            Console.WriteLine(reaction.Emote.Name);
            

            if (!reaction.Emote.Name.Equals("ehre")) {
                return Task.CompletedTask;
            }

            if (!scoreBoard.ContainsKey(message.Result.Author.Id))
            {
                scoreBoard.Add(message.Result.Author.Id, 1);
                return Task.CompletedTask;
            }

            scoreBoard[message.Result.Author.Id] += 1;
            
            return Task.CompletedTask;
        }

        private string scoreBoardToString()
        {
            string s = "";
            foreach (ulong user in scoreBoard.Keys)
            {
                s += ($"{client.GetUser(user)} : {scoreBoard[user]}\n");
            }
            return s;
        }
    }


}
