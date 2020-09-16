using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
            initDictionary();

            client = new DiscordSocketClient();
            client.ReactionAdded += Client_ReactionAdded;
            client.ReactionRemoved += Client_ReactionRemoved;
            client.MessageReceived += Client_MessageReceived;
        }

        public async Task startup()
        {
            await client.LoginAsync(TokenType.Bot, "irgendeine scheiße");
            Console.WriteLine("Ehrenbot starting up");
            await client.StartAsync();
            Console.WriteLine("Ehrenbot started");

            await Task.Delay(Timeout.Infinite);
        }

        private Task Client_MessageReceived(SocketMessage message)
        {

            if (message.Content.Equals("Wie stehts um die Ehre ?"))
            {
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
            if (scoreBoard[message.Result.Author.Id] == 0) { scoreBoard.Remove(message.Result.Author.Id);  }
            scoreBoardToCSV();

            return Task.CompletedTask;
        }

        private Task Client_ReactionAdded(Cacheable<IUserMessage, ulong> before, ISocketMessageChannel channel, SocketReaction reaction)
        {
            Task<IUserMessage> message = before.GetOrDownloadAsync();

            if (!reaction.Emote.Name.Equals("ehre"))
            {
                return Task.CompletedTask;
            }

            if (!scoreBoard.ContainsKey(message.Result.Author.Id))
            {
                scoreBoard.Add(message.Result.Author.Id, 1);
                scoreBoardToCSV();
                return Task.CompletedTask;
            }

            scoreBoard[message.Result.Author.Id] += 1;
            scoreBoardToCSV();

            return Task.CompletedTask;
        }

        private string scoreBoardToString()
        {
            string s = "";
            foreach (ulong user in scoreBoard.Keys)
            {
                s += "---------------------------------------------------------\n";
                s += ($"{client.GetUser(user)} : {scoreBoard[user]}\n");
            }
            s += "---------------------------------------------------------\n";

            return s;
        }

        private void initDictionary()
        {
            scoreBoard = new Dictionary<ulong, int>();
            if (!File.Exists("./scoreBoard.csv")) return;

            using (var reader = new StreamReader("./scoreBoard.csv"))
            {

                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    string[] values = line.Split(',');

                    scoreBoard.Add(ulong.Parse(values[0]), int.Parse(values[1]));
                }
            }
        }

        private void scoreBoardToCSV()
        {
            string csv = string.Join(Environment.NewLine,
                scoreBoard.Select(d => $"{d.Key},{d.Value},")
            );

            File.WriteAllText("./scoreBoard.csv", String.Empty);
            File.WriteAllText("./scoreBoard.csv", csv);
        }
    }
}
