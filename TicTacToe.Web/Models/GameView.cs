using GameEngine.Enums;

namespace TicTacToe.Web.Models
{
    public class GameView
    {
        public Guid Revision { get; init; }
        public IReadOnlyDictionary<string, string> Cells { get; init; }
        public bool Started { get; init; }
        public GameStatus Status { get; init; }
        public string Player1 { get; init; }
        public string Player2 { get; init; }
        public string CurrentPlayer { get; init; }
        public string CurrentSymbol { get; init; }
        public string Winner { get; init; }
        public IReadOnlyList<string> WinningPositions { get; init; }
        public bool AiTurn { get; init; }
        public bool AgainstAi { get; init; }
        public DifficultyLevel Difficulty { get; init; }
    }
}
