using TicTacToe.Web.Models;
using GameEngine;
using GameEngine.Enums;
using GameEngine.Player;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Caching.Memory;
using TicTacToe.Ai;

namespace TicTacToe.Web
{
    public class GameStore(IMemoryCache cache, IChatClient chatClient)
    {
        private readonly object _creationLock = new object();

        private WebGame GetGame(string browserId)
        {
            lock (_creationLock)
            {
                var game = cache.Get<WebGame>(browserId);
                if (game != null) return game;

                game = new WebGame();
                cache.Set(browserId, game, new MemoryCacheEntryOptions { SlidingExpiration = TimeSpan.FromHours(1) });
                return game;
            }
        }

        public async Task<GameView> ReadAsync(string browserId, CancellationToken cancellationToken = default)
        {
            var game = GetGame(browserId);
            await game.Gate.WaitAsync(cancellationToken);
            try
            {
                return Snapshot(game);
            }
            finally
            {
                game.Gate.Release();
            }
        }

        public async Task<string> StartAsync(string browserId, bool againstAi, string name1, string name2,
            DifficultyLevel difficulty, Guid revision, CancellationToken cancellationToken = default)
        {
            var game = GetGame(browserId);
            await game.Gate.WaitAsync(cancellationToken);
            try
            {
                if (revision != game.Revision) return "This game changed. Please try again.";
                if (!Enum.IsDefined(difficulty)) return "Choose a valid difficulty.";
                if (string.IsNullOrWhiteSpace(name1) || name1.Length > 40 ||
                    (!againstAi && (string.IsNullOrWhiteSpace(name2) || name2.Length > 40)))
                    return "Enter player names of up to 40 characters.";

                PlayerBase opponent = againstAi
                    ? new PlayerAi("AI", "O", difficulty, chatClient)
                    : new PlayerB(name2, "O");
                game.Session = new GameSession(new PlayerA(name1, "X"), opponent);
                game.Revision = Guid.NewGuid();
                return null;
            }
            finally
            {
                game.Gate.Release();
            }
        }

        public async Task<string> MoveAsync(string browserId, string position, Guid revision, CancellationToken cancellationToken = default)
        {
            var game = GetGame(browserId);
            await game.Gate.WaitAsync(cancellationToken);
            try
            {
                if (revision != game.Revision) return "This game changed. Please try again.";
                if (game.Session == null) return "Start a game first.";
                if (game.Session.CurrentPlayer is PlayerAi) return "It is the AI's turn. Retry its move.";
                if (!game.Session.TryMakeMove(position, out string error)) return error;

                game.Revision = Guid.NewGuid();
                await PlayAiAsync(game, cancellationToken);
                return null;
            }
            finally
            {
                game.Gate.Release();
            }
        }

        public async Task<string> RetryAiAsync(string browserId, Guid revision, CancellationToken cancellationToken = default)
        {
            var game = GetGame(browserId);
            await game.Gate.WaitAsync(cancellationToken);
            try
            {
                if (revision != game.Revision) return "This game changed. Please try again.";
                if (game.Session?.CurrentPlayer is not PlayerAi || game.Session.Status != GameStatus.InProgress)
                    return "There is no AI move to retry.";

                await PlayAiAsync(game, cancellationToken);
                return null;
            }
            finally
            {
                game.Gate.Release();
            }
        }

        private static async Task PlayAiAsync(WebGame game, CancellationToken cancellationToken)
        {
            if (game.Session.Status == GameStatus.InProgress && game.Session.CurrentPlayer is PlayerAi ai)
            {
                string move = await ai.GetMoveAsync(game.Session.Board.Board, cancellationToken);
                game.Session.TryMakeMove(move);
                game.Revision = Guid.NewGuid();
            }
        }

        private static GameView Snapshot(WebGame game)
        {
            var session = game.Session;
            return new GameView
            {
                Revision = game.Revision,
                Cells = session == null ? new GameBoard().Board : new Dictionary<string, string>(session.Board.Board),
                Started = session != null,
                Status = session?.Status ?? GameStatus.InProgress,
                Player1 = session?.Player1.Name,
                Player2 = session?.Player2.Name,
                CurrentPlayer = session?.CurrentPlayer.Name,
                CurrentSymbol = session?.CurrentPlayer.Symbol,
                Winner = session?.Winner?.Name,
                WinningPositions = session?.Board.GetWinningPositions() ?? Array.Empty<string>(),
                AiTurn = session?.CurrentPlayer is PlayerAi && session.Status == GameStatus.InProgress,
                Difficulty = session?.Player2 is PlayerAi ai ? ai.Difficulty : DifficultyLevel.Medium,
                AgainstAi = session?.Player2 is PlayerAi
            };
        }
    }

    internal class WebGame
    {
        public SemaphoreSlim Gate { get; } = new SemaphoreSlim(1, 1);
        public GameSession Session { get; set; }
        public Guid Revision { get; set; } = Guid.NewGuid();
    }
}
