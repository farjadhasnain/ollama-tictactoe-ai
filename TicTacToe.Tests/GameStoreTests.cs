using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GameEngine.Enums;
using Microsoft.Extensions.Caching.Memory;
using NUnit.Framework;
using TicTacToe.Web;

namespace TicTacToe.Tests
{
    [TestFixture]
    public class GameStoreTests
    {
        private MemoryCache _cache;
        private TestChatClient _client;
        private GameStore _games;

        [SetUp]
        public void SetUp()
        {
            _cache = new MemoryCache(new MemoryCacheOptions());
            _client = new TestChatClient();
            _games = new GameStore(_cache, _client);
        }

        [TearDown]
        public void TearDown()
        {
            _cache.Dispose();
            _client.Dispose();
        }

        [Test]
        public async Task Games_AreIsolatedPerBrowser()
        {
            await Start("first");
            await Start("second");
            var first = await _games.ReadAsync("first");
            Assert.That(await _games.MoveAsync("first", "A1", first.Revision), Is.Null);
            Assert.That((await _games.ReadAsync("second")).Cells.Values, Is.All.Empty);
            Assert.That(first.Cells["A1"], Is.Empty);
        }

        [Test]
        public async Task HumanGame_AlternatesPlayers_AndRejectsOccupiedCell()
        {
            await Start("game");
            var first = await _games.ReadAsync("game");
            await _games.MoveAsync("game", "A1", first.Revision);
            var second = await _games.ReadAsync("game");
            Assert.That(second.CurrentPlayer, Is.EqualTo("Bob"));
            Assert.That(await _games.MoveAsync("game", "A1", second.Revision), Is.Not.Null);
            Assert.That((await _games.ReadAsync("game")).Revision, Is.EqualTo(second.Revision));
            Assert.That(await _games.MoveAsync("game", "A2", second.Revision), Is.Null);
            Assert.That((await _games.ReadAsync("game")).Cells["A2"], Is.EqualTo("O"));
        }

        [Test]
        public async Task DuplicateConcurrentMoves_OnlyApplyOnce()
        {
            await Start("game", true);
            var first = await _games.ReadAsync("game");
            var results = await Task.WhenAll(_games.MoveAsync("game", "A1", first.Revision), _games.MoveAsync("game", "A2", first.Revision));
            Assert.That(results.Count(result => result == null), Is.EqualTo(1));
            var game = await _games.ReadAsync("game");
            Assert.That(game.Cells.Values.Count(value => value != ""), Is.EqualTo(2));
            Assert.That(_client.Calls, Is.EqualTo(1));
        }

        [Test]
        public async Task AiOpponent_PlaysItsTurn_ThenReturnsControlToHuman()
        {
            await Start("game", true);
            var first = await _games.ReadAsync("game");
            Assert.That(await _games.MoveAsync("game", "A1", first.Revision), Is.Null);
            var game = await _games.ReadAsync("game");
            Assert.That(game.Cells["A1"], Is.EqualTo("X"));
            Assert.That(game.Cells["B2"], Is.EqualTo("O"));
            Assert.That(game.CurrentPlayer, Is.EqualTo("Alice"));
            Assert.That(game.AiTurn, Is.False);
        }

        [Test]
        public async Task CancelledAiTurn_CanBeRetried_WithoutLosingHumanMove()
        {
            await Start("game", true);
            _client.WaitForCancellation = true;
            using var cancellation = new CancellationTokenSource();
            var first = await _games.ReadAsync("game");
            var pending = _games.MoveAsync("game", "A1", first.Revision, cancellation.Token);
            cancellation.Cancel();
            Assert.That(async () => await pending, Throws.InstanceOf<OperationCanceledException>());
            var interrupted = await _games.ReadAsync("game");
            Assert.That(interrupted.Cells["A1"], Is.EqualTo("X"));
            Assert.That(interrupted.AiTurn, Is.True);
            Assert.That(await _games.MoveAsync("game", "A2", interrupted.Revision), Is.Not.Null);
            _client.WaitForCancellation = false;
            Assert.That(await _games.RetryAiAsync("game", interrupted.Revision), Is.Null);
            Assert.That((await _games.ReadAsync("game")).Cells["B2"], Is.EqualTo("O"));
        }

        [Test]
        public async Task CompletedGame_RejectsFurtherMoves_AndCanRestart()
        {
            await Start("game");
            foreach (string move in new[] { "A1", "B1", "A2", "B2", "A3" })
            {
                var current = await _games.ReadAsync("game");
                Assert.That(await _games.MoveAsync("game", move, current.Revision), Is.Null);
            }
            var won = await _games.ReadAsync("game");
            Assert.That(won.Winner, Is.EqualTo("Alice"));
            Assert.That(won.Status, Is.EqualTo(GameStatus.Won));
            Assert.That(await _games.MoveAsync("game", "C1", won.Revision), Is.EqualTo("The game is already over."));
            await Start("game");
            Assert.That((await _games.ReadAsync("game")).Cells.Values, Is.All.Empty);
            Assert.That(await _games.MoveAsync("game", "C1", won.Revision), Does.Contain("changed"));
        }

        [Test]
        public async Task Start_RejectsInvalidInputs_AndStaleRevision()
        {
            var empty = await _games.ReadAsync("game");
            Assert.That(await _games.StartAsync("game", false, " ", "Bob", DifficultyLevel.Easy, empty.Revision), Is.Not.Null);
            Assert.That(await _games.StartAsync("game", false, "Alice", "Bob", (DifficultyLevel)99, empty.Revision), Is.Not.Null);
            Assert.That(await _games.StartAsync("game", false, "Alice", "Bob", DifficultyLevel.Easy, Guid.NewGuid()), Does.Contain("changed"));
            Assert.That((await _games.ReadAsync("game")).Started, Is.False);
        }

        [Test]
        public async Task ExpiredGame_RejectsOldMoves()
        {
            await Start("game");
            var previous = await _games.ReadAsync("game");
            _cache.Remove("game");
            Assert.That(await _games.MoveAsync("game", "A1", previous.Revision), Does.Contain("changed"));
        }

        [Test]
        public async Task HumanWin_DoesNotTriggerAnotherAiMove()
        {
            await Start("game", true);
            _client.Response = "A2";
            var current = await _games.ReadAsync("game");
            await _games.MoveAsync("game", "A1", current.Revision);
            _client.Response = "A3";
            current = await _games.ReadAsync("game");
            await _games.MoveAsync("game", "B1", current.Revision);
            current = await _games.ReadAsync("game");
            await _games.MoveAsync("game", "C1", current.Revision);
            var won = await _games.ReadAsync("game");
            Assert.That(won.Winner, Is.EqualTo("Alice"));
            Assert.That(won.WinningPositions, Is.EqualTo(new[] { "A1", "B1", "C1" }));
            Assert.That(_client.Calls, Is.EqualTo(2));
        }

        [Test]
        public async Task Draw_EndsWebGame()
        {
            await Start("game");
            foreach (string move in new[] { "A1", "A2", "A3", "B2", "B1", "B3", "C2", "C1", "C3" })
            {
                var current = await _games.ReadAsync("game");
                Assert.That(await _games.MoveAsync("game", move, current.Revision), Is.Null);
            }
            var draw = await _games.ReadAsync("game");
            Assert.That(draw.Status, Is.EqualTo(GameStatus.Draw));
            Assert.That(draw.Winner, Is.Null);
            Assert.That(draw.WinningPositions, Is.Empty);
            Assert.That(await _games.MoveAsync("game", "A1", draw.Revision), Is.Not.Null);
            Assert.That(_client.Calls, Is.Zero);
        }
        private async Task Start(string id, bool ai = false)
        {
            var current = await _games.ReadAsync(id);
            Assert.That(await _games.StartAsync(id, ai, "Alice", "Bob", DifficultyLevel.Medium, current.Revision), Is.Null);
        }
    }
}
