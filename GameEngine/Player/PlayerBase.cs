using System;
using System.Collections.Generic;

namespace GameEngine.Player
{
    public class PlayerBase
    {
        public string Name { get; }
        public string Symbol { get; }

        public PlayerBase(string name, string symbol)
        {
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Enter a player name.", nameof(name));
            if (string.IsNullOrWhiteSpace(symbol)) throw new ArgumentException("Enter a player symbol.", nameof(symbol));

            Name = name.Trim();
            Symbol = symbol.Trim().ToUpperInvariant();
        }

        public void MakeMove(Dictionary<string, string> board, string position)
        {
            board[position] = Symbol;
        }

        public bool IsValidMove(IReadOnlyDictionary<string, string> board, string position)
        {
            return position != null && board.ContainsKey(position) && board[position] == "";
        }
    }
}
