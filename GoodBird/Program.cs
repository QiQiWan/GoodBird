using System;

namespace GoodBird
{
    class Program
    {
        static GoodBirdServer server = new GoodBirdServer();
        static void Main(string[] args)
        {
            server.Start();
            Console.ReadLine();
        }
    }
}
