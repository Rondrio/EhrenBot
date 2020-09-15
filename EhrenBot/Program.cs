using System;

namespace EhrenBot
{
    class Program
    {
        static void Main(string[] args)
        {
            Bot bot = new Bot();
            bot.startup().GetAwaiter().GetResult();
        }
    }
}
