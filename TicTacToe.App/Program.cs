using System;
using System.Threading;
using System.Threading.Tasks;

namespace TicTacToe.App
{
    public class Program
    {
        static async Task Main(string[] args)
        {
            using var cancellation = new CancellationTokenSource();
            Console.CancelKeyPress += (_, e) => { e.Cancel = true; cancellation.Cancel(); };
            try
            {
                await ConsoleGame.RunAsync(cancellation.Token);
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("Game cancelled.");
            }
        }
    }
}
