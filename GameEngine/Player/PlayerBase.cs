using System.Collections.Generic;

namespace TicTacToe.App.Player
{
    public class PlayerBase
    {
        public string Name { get; set; }
        public string Symbol { get; set; }

        public PlayerBase(string name, string symbol)
        {
            Name = name;
            Symbol = symbol;
        }

        public void MakeMove(Dictionary<string, string> board, string position)
        {
            board[position] = Symbol;
        }

        public bool IsValidMove(Dictionary<string, string> board, string position)
        {
            return board.ContainsKey(position) && board[position] == "";
        }
    }
}
