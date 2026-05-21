using System.Collections.Generic;

namespace GameEngine
{
    public class GameBoard
    {
        public Dictionary<string, string> Board { get; } = new Dictionary<string, string>
        {
            {"A1", ""}, {"A2", ""}, {"A3", ""},
            {"B1", ""}, {"B2", ""}, {"B3", ""},
            {"C1", ""}, {"C2", ""}, {"C3", ""}
        };

        public bool CheckWin()
        {
            return (Match("A1","A2","A3") || Match("B1","B2","B3") || Match("C1","C2","C3") ||
                    Match("A1","B1","C1") || Match("A2","B2","C2") || Match("A3","B3","C3") ||
                    Match("A1","B2","C3") || Match("A3","B2","C1"));
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
