using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace GameEngine
{
    public class GameBoard
    {
        private readonly Dictionary<string, string> _board = new Dictionary<string, string>
        {
            {"A1", ""}, {"A2", ""}, {"A3", ""},
            {"B1", ""}, {"B2", ""}, {"B3", ""},
            {"C1", ""}, {"C2", ""}, {"C3", ""}
        };

        public IReadOnlyDictionary<string, string> Board { get; }

        public GameBoard()
        {
            Board = new ReadOnlyDictionary<string, string>(_board);
        }

        internal void PlaceMove(string position, string symbol)
        {
            _board[position] = symbol;
        }

        private static readonly string[][] _winningLines =
        {
            new[] { "A1", "A2", "A3" }, new[] { "B1", "B2", "B3" }, new[] { "C1", "C2", "C3" },
            new[] { "A1", "B1", "C1" }, new[] { "A2", "B2", "C2" }, new[] { "A3", "B3", "C3" },
            new[] { "A1", "B2", "C3" }, new[] { "A3", "B2", "C1" }
        };

        public IReadOnlyList<string> GetWinningPositions()
        {
            foreach (var line in _winningLines)
            {
                if (Match(line[0], line[1], line[2])) return Array.AsReadOnly(line);
            }
            return Array.Empty<string>();
        }

        public bool CheckWin()
        {
            return GetWinningPositions().Count > 0;
        }

        public bool CheckTie()
        {
            return IsBoardFull() && !CheckWin();
        }

        public bool IsBoardFull()
        {
            foreach (var cell in Board.Values)
            {
                if (cell == "")
                {
                    return false;
                }
            }
            return true;
        }

        private bool Match(string a, string b, string c)
        {
            return Board[a] != "" && Board[a] == Board[b] && Board[b] == Board[c];
        }
    }
}
