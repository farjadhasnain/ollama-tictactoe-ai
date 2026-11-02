using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using GameEngine;
using GameEngine.Enums;
using NUnit.Framework;
using TicTacToe.Ai;

namespace TicTacToe.Tests
{
    [TestFixture]
    public class PlayerAiTests
    {
        [TestCase(DifficultyLevel.Easy, 1.5f)]
        [TestCase(DifficultyLevel.Medium, 0.7f)]
        [TestCase(DifficultyLevel.Hard, 0.1f)]
        public async Task Ai_UsesBoardSymbolAndDifficulty_WithoutMutatingBoard(DifficultyLevel difficulty, float temperature)
        {
            using var client = new TestChatClient { Response = "I'll play b2." };
            var ai = new PlayerAi("AI", "O", difficulty, client);
            var board = new GameBoard();
            Assert.That(await ai.GetMoveAsync(board.Board), Is.EqualTo("B2"));
            Assert.That(board.Board.Values, Is.All.Empty);
            Assert.That(client.Prompt, Does.Contain("playing as 'O'").And.Contain("A1").And.Contain("C3"));
            Assert.That(client.Temperature, Is.EqualTo(temperature));
            Assert.That(client.Token.CanBeCanceled, Is.True);
        }

        [TestCase("A1")]
        [TestCase("nonsense")]
        [TestCase(null)]
        public async Task Ai_FallsBackToEmptyCell_ForInvalidResponse(string response)
        {
            using var client = new TestChatClient { Response = response };
            var ai = new PlayerAi("AI", "O", DifficultyLevel.Medium, client);
            var board = new Dictionary<string, string> { { "A1", "X" }, { "A2", "" } };
            Assert.That(await ai.GetMoveAsync(board), Is.EqualTo("A2"));
        }

        [Test]
        public async Task Ai_FallsBack_WhenOllamaIsUnavailable()
        {
            using var client = new TestChatClient { Failure = new HttpRequestException("Offline") };
            var ai = new PlayerAi("AI", "O", DifficultyLevel.Medium, client);
            Assert.That(await ai.GetMoveAsync(new GameBoard().Board), Is.EqualTo("A1"));
        }

        [Test]
        public async Task Ai_DoesNotCallProvider_WhenBoardIsFull()
        {
            using var client = new TestChatClient();
            var ai = new PlayerAi("AI", "O", DifficultyLevel.Medium, client);
            Assert.That(await ai.GetMoveAsync(new Dictionary<string, string> { { "A1", "X" } }), Is.Null);
            Assert.That(client.Calls, Is.Zero);
        }

        [Test]
        public void Ai_PropagatesCallerCancellation()
        {
            using var client = new TestChatClient { WaitForCancellation = true };
            using var cancellation = new CancellationTokenSource();
            var ai = new PlayerAi("AI", "O", DifficultyLevel.Medium, client);
            var pending = ai.GetMoveAsync(new GameBoard().Board, cancellation.Token);
            cancellation.Cancel();
            Assert.That(async () => await pending, Throws.InstanceOf<System.OperationCanceledException>());
        }
    }
}
