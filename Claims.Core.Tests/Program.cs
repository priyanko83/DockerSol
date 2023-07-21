using System;
using System.Threading;

namespace Claims.Core.Tests
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Press enter to send declaration..............");
            ConsoleKeyInfo cki;
            do
            {
                cki = Console.ReadKey();
                if (cki.Key == ConsoleKey.Enter)
                {
                    Console.WriteLine("Creating declaration..............");
                    CommandsGenerator g = new CommandsGenerator();
                    g.CreateTestDeclarationsAsync().GetAwaiter().GetResult();
                    //g.UpdateTestDeclarationsAsync().GetAwaiter().GetResult();
                }
                Thread.Sleep(500);
            } while (cki.Key != ConsoleKey.Escape);
        }
    }
}
