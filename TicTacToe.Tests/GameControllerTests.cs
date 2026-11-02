using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using GameEngine.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Caching.Memory;
using NUnit.Framework;
using TicTacToe.Web;
using TicTacToe.Web.Controllers;
using TicTacToe.Web.Models;

namespace TicTacToe.Tests
{
    [TestFixture]
    public class GameControllerTests
    {
        private MemoryCache _cache;
        private TestChatClient _client;
        private GameController _controller;
        private DefaultHttpContext _http;

        [SetUp]
        public void SetUp()
        {
            _cache = new MemoryCache(new MemoryCacheOptions());
            _client = new TestChatClient();
            _http = new DefaultHttpContext { Session = new TestSession() };
            _controller = new GameController(new GameStore(_cache, _client))
            {
                ControllerContext = new ControllerContext { HttpContext = _http },
                TempData = new TempDataDictionary(_http, new TestTempDataProvider())
            };
        }

        [TearDown]
        public void TearDown()
        {
            _cache.Dispose();
            _client.Dispose();
            _controller.Dispose();
        }

        [Test]
        public async Task Index_ReturnsTypedView_AndReusesBrowserGame()
        {
            _controller.TempData["Error"] = "Choose an empty cell.";
            var first = await Read();
            string id = _http.Session.GetString("GameId");
            var second = await Read();
            Assert.That(first.Game.Started, Is.False);
            Assert.That(first.Error, Is.EqualTo("Choose an empty cell."));
            Assert.That(id, Is.Not.Null.And.Not.Empty);
            Assert.That(_http.Session.GetString("GameId"), Is.EqualTo(id));
            Assert.That(second.Game.Revision, Is.EqualTo(first.Game.Revision));
        }

        [TestCase("human", "Bob", false)]
        [TestCase("ai", "AI", true)]
        public async Task Start_CreatesSelectedMode_AndRedirects(string mode, string opponent, bool ai)
        {
            var initial = await Read();
            var result = await _controller.Start(mode, "Alice", "Bob", DifficultyLevel.Hard, initial.Game.Revision, default);
            AssertRedirect(result);
            var started = await Read();
            Assert.That(started.Game.Player1, Is.EqualTo("Alice"));
            Assert.That(started.Game.Player2, Is.EqualTo(opponent));
            Assert.That(started.Game.AgainstAi, Is.EqualTo(ai));
            Assert.That(started.Error, Is.Null);
        }

        [Test]
        public async Task Start_RejectsInvalidModeAndModelBindingErrors()
        {
            var initial = await Read();
            AssertRedirect(await _controller.Start("unknown", "Alice", "Bob", DifficultyLevel.Easy, initial.Game.Revision, default));
            Assert.That((await Read()).Error, Is.EqualTo("Choose a valid game mode and difficulty."));
            _controller.ModelState.AddModelError("difficulty", "Invalid value");
            AssertRedirect(await _controller.Start("human", "Alice", "Bob", DifficultyLevel.Easy, initial.Game.Revision, default));
            var rejected = await Read();
            Assert.That(rejected.Game.Started, Is.False);
            Assert.That(rejected.Error, Is.EqualTo("Choose a valid game mode and difficulty."));
        }

        [TestCase("human", "Bob", "")]
        [TestCase("ai", "Alice", "O")]
        public async Task Move_AppliesTurn_AndDisplaysInvalidMoveError(string mode, string nextPlayer, string aiSymbol)
        {
            var initial = await Read();
            await _controller.Start(mode, "Alice", "Bob", DifficultyLevel.Medium, initial.Game.Revision, default);
            var started = await Read();
            AssertRedirect(await _controller.Move("A1", started.Game.Revision, default));
            var moved = await Read();
            Assert.That(moved.Game.Cells["A1"], Is.EqualTo("X"));
            Assert.That(moved.Game.Cells["B2"], Is.EqualTo(aiSymbol));
            Assert.That(moved.Game.CurrentPlayer, Is.EqualTo(nextPlayer));
            AssertRedirect(await _controller.Move("A1", moved.Game.Revision, default));
            Assert.That((await Read()).Error, Is.EqualTo("Choose an empty cell on the board."));
        }

        [Test]
        public async Task RetryAi_ContinuesCancelledTurn()
        {
            var initial = await Read();
            await _controller.Start("ai", "Alice", "Bob", DifficultyLevel.Medium, initial.Game.Revision, default);
            var started = await Read();
            _client.WaitForCancellation = true;
            using var cancellation = new CancellationTokenSource();
            var pending = _controller.Move("A1", started.Game.Revision, cancellation.Token);
            cancellation.Cancel();
            Assert.That(async () => await pending, Throws.InstanceOf<OperationCanceledException>());
            var interrupted = await Read();
            Assert.That(interrupted.Game.AiTurn, Is.True);
            _client.WaitForCancellation = false;
            AssertRedirect(await _controller.RetryAi(interrupted.Game.Revision, default));
            var resumed = await Read();
            Assert.That(resumed.Game.Cells["B2"], Is.EqualTo("O"));
            Assert.That(resumed.Game.CurrentPlayer, Is.EqualTo("Alice"));
        }

        [Test]
        public void Error_ReturnsView()
        {
            Assert.That(_controller.Error(), Is.TypeOf<ViewResult>());
        }

        private async Task<GamePageViewModel> Read()
        {
            var result = (ViewResult)await _controller.Index(default);
            Assert.That(result.Model, Is.TypeOf<GamePageViewModel>());
            return (GamePageViewModel)result.Model;
        }

        private static void AssertRedirect(IActionResult result)
        {
            Assert.That(result, Is.TypeOf<RedirectToActionResult>());
            Assert.That(((RedirectToActionResult)result).ActionName, Is.EqualTo("Index"));
        }

        private class TestTempDataProvider : ITempDataProvider
        {
            public IDictionary<string, object> LoadTempData(HttpContext context) => new Dictionary<string, object>();
            public void SaveTempData(HttpContext context, IDictionary<string, object> values) { }
        }

        private class TestSession : ISession
        {
            private readonly Dictionary<string, byte[]> _values = new Dictionary<string, byte[]>();
            public bool IsAvailable => true;
            public string Id { get; } = Guid.NewGuid().ToString();
            public IEnumerable<string> Keys => _values.Keys;
            public void Clear() => _values.Clear();
            public Task LoadAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;
            public Task CommitAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;
            public void Remove(string key) => _values.Remove(key);
            public void Set(string key, byte[] value) => _values[key] = value;
            public bool TryGetValue(string key, out byte[] value) => _values.TryGetValue(key, out value);
        }
    }
}
