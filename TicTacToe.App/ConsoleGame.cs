using GameEngine;
using GameEngine.Enums;
using GameEngine.Player;
using OllamaSharp;
using System;
using System.Threading;
using System.Threading.Tasks;
using TicTacToe.Ai;

namespace TicTacToe.App
{
    public static class ConsoleGame
    {
        public static async Task RunAsync(CancellationToken cancellationToken = default)
        {
            Console.WriteLine("Tic Tac Toe. Copyright (c) 2025 Farjad. All rights reserved.");
            Console.WriteLine("1 - vs Human");
            Console.WriteLine("2 - vs AI");
            bool againstAi = Console.ReadLine() == "2";
            DifficultyLevel difficulty = DifficultyLevel.Easy;

            if (againstAi)
            {
                Console.WriteLine("Choose difficulty: 1 - Easy, 2 - Medium, 3 - Hard");
                string choice = Console.ReadLine();
                difficulty = choice == "3" ? DifficultyLevel.Hard : choice == "2" ? DifficultyLevel.Medium : DifficultyLevel.Easy;
            }

            string name1 = ReadRequired("Enter name for Player 1:");
            string symbol1 = ReadRequired($"{name1}, choose your symbol (e.g. X):").ToUpperInvariant();
            var player1 = new PlayerA(name1, symbol1);

            using var chatClient = new OllamaApiClient(
                new Uri(Environment.GetEnvironmentVariable("Ollama__Endpoint") ?? "http://localhost:11434"),
                Environment.GetEnvironmentVariable("Ollama__Model") ?? "gemma4");

            PlayerBase player2;
            if (againstAi)
            {
                player2 = new PlayerAi("AI", symbol1 == "O" ? "X" : "O", difficulty, chatClient);
            }
            else
            {
                string name2 = ReadRequired("Enter name for Player 2:");
                string symbol2;
                do
                {
                    symbol2 = ReadRequired($"{name2}, choose a different symbol from {symbol1}:").ToUpperInvariant();
                } while (symbol1 == symbol2);

                player2 = new PlayerB(name2, symbol2);
            }

            await GameStartAsync(new GameSession(player1, player2), cancellationToken);
        }

        public static async Task GameStartAsync(GameSession session, CancellationToken cancellationToken = default)
        {
            while (session.Status == GameStatus.InProgress)
            {
                cancellationToken.ThrowIfCancellationRequested();
                Draw(session);

                string move;
                if (session.CurrentPlayer is PlayerAi aiPlayer)
                {
                    Console.WriteLine($"AI [{aiPlayer.Difficulty}] is thinking...");
                    move = await aiPlayer.GetMoveAsync(session.Board.Board, cancellationToken);
                    Console.WriteLine($"AI plays: {move}");
                    if (!Console.IsOutputRedirected) await Task.Delay(800, cancellationToken);
                }
                else
                {
                    Console.WriteLine($"{session.CurrentPlayer.Name}'s turn. Enter move (e.g. A1, B2):");
                    move = Console.ReadLine();
                    if (move == null) return;
                }

                if (!session.TryMakeMove(move, out string error)) Console.WriteLine(error);
            }

            Draw(session);
            Console.WriteLine(session.Status == GameStatus.Won ? $"{session.Winner.Name} wins!" : "It's a draw!");
            if (!Console.IsInputRedirected)
            {
                Console.WriteLine("Game Over! Press any key to exit...");
                Console.ReadKey();
            }
        }

        private static string ReadRequired(string prompt)
        {
            while (true)
            {
                Console.WriteLine(prompt);
                string value = Console.ReadLine();
                if (value == null) throw new OperationCanceledException("Input closed.");
                if (!string.IsNullOrWhiteSpace(value)) return value.Trim();
            }
        }

        private static void Draw(GameSession session)
        {
            if (!Console.IsOutputRedirected) Console.Clear();
            ConsoleRenderer.DrawBoard(session.Board.Board, session.Player1, session.Player2);
        }
    }
}
