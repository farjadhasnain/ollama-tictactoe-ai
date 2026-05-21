using System;
using System.Collections.Generic;
using TicTacToe.App.Player;

namespace TicTacToe.App
{
    public static class ConsoleRenderer
    {
        public static void DrawBoard(Dictionary<string, string> board, PlayerBase p1, PlayerBase p2)
        {
            Console.WriteLine($"Player 1: {p1.Name} [{p1.Symbol}]   Player 2: {p2.Name} [{p2.Symbol}]");
            Console.WriteLine();
            Console.WriteLine("     1   |   2   |   3");
            Console.WriteLine($"  A  {Cell(board,"A1")}   |   {Cell(board,"A2")}   |   {Cell(board,"A3")}");
            Console.WriteLine("    -----|-------|-----");
            Console.WriteLine($"  B  {Cell(board,"B1")}   |   {Cell(board,"B2")}   |   {Cell(board,"B3")}");
            Console.WriteLine("    -----|-------|-----");
            Console.WriteLine($"  C  {Cell(board,"C1")}   |   {Cell(board,"C2")}   |   {Cell(board,"C3")}");
            Console.WriteLine();
        }

        private static string Cell(Dictionary<string, string> board, string key)
        {
            return board[key] == "" ? "." : board[key];
        }
    }
}