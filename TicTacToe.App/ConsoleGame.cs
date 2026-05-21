using GameEngine;
using GameEngine.Enums;
using System;
using TicTacToe.App.Player;

namespace TicTacToe.App
{
    public static class ConsoleGame
    {
        public static void Run()
        {
            Console.WriteLine("Tic Tac Toe. Copyright (c) 2025 Farjad. All rights reserved.");
            Console.WriteLine();

            Console.WriteLine("1 - vs Human");
            Console.WriteLine("2 - vs AI");

            var choice = Console.ReadLine();
            GameSession session;
            if (choice == "2")
            {
                Console.WriteLine("Choose difficulty level:");
                Console.WriteLine("1- Easy");
                Console.WriteLine("2- Medium");
                Console.WriteLine("3- Hard");
                var difficultyChoice = Console.ReadLine();
                DifficultyLevel difficulty = DifficultyLevel.Easy;
                switch (difficultyChoice)
                {
                    case "1":
                        difficulty = DifficultyLevel.Easy;
                        break;
                    case "2":
                        difficulty = DifficultyLevel.Medium;
                        break;
                    case "3":
                        difficulty = DifficultyLevel.Hard;
                        break;
                }

                Console.WriteLine("Enter name for Player 1:");
                string name = Console.ReadLine().ToUpper();
                Console.WriteLine($"{name}, choose your symbol (e.g. X):");
                string symbol = Console.ReadLine().ToUpper();

                var player = new PlayerA(name, symbol);
                var playerAi = new PlayerAi("AI", "£", difficulty);
                session = new GameSession(player, playerAi);
            }
            else
            {
                Console.WriteLine("Enter name for Player 1:");
                string name1 = Console.ReadLine().ToUpper();
                Console.WriteLine($"{name1}, choose your symbol (e.g. X):");
                string symbol1 = Console.ReadLine().ToUpper();

                Console.WriteLine("Enter name for Player 2:");
                string name2 = Console.ReadLine().ToUpper();
                Console.WriteLine($"{name2}, choose your symbol (e.g. O):");
                string symbol2 = Console.ReadLine().ToUpper();

                var player1 = new PlayerA(name1, symbol1);
                var player2 = new PlayerB(name2, symbol2);
                session = new GameSession(player1, player2);
            }
            GameStart(session);
        }

        public static void GameStart(GameSession session)
        {
            while (session.Status == GameStatus.InProgress)
            {
                Console.Clear();
                ConsoleRenderer.DrawBoard(session.Board.Board, session.Player1, session.Player2);

                string move;
                PlayerAi aiPlayer = session.CurrentPlayer as PlayerAi;
                if (aiPlayer != null)
                {
                    Console.WriteLine($"AI [{aiPlayer.Difficulty}] is thinking...");
                    move = aiPlayer.GetMove(session.Board.Board);
                }
                else
                {
                    Console.WriteLine($"{session.CurrentPlayer.Name}'s turn. Enter move (e.g. A1, B2):");
                    move = Console.ReadLine().ToUpper();
                }

                if (!session.TryMakeMove(move))
                {
                    Console.WriteLine("Invalid move. Press any key to try again...");
                    Console.ReadKey();
                }
            }

            Console.Clear();
            ConsoleRenderer.DrawBoard(session.Board.Board, session.Player1, session.Player2);

            if (session.Status == GameStatus.Won)
                Console.WriteLine($"{session.Winner.Name} wins!");
            else
                Console.WriteLine("It's a draw!");

            Console.WriteLine();
            Console.WriteLine("Game Over! Press any key to exit...");
            Console.ReadKey();
        }
    }
}