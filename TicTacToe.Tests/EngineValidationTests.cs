using System;
using GameEngine;
using GameEngine.Player;
using NUnit.Framework;

namespace TicTacToe.Tests
{
    [TestFixture]
    public class EngineValidationTests
    {
        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        [TestCase("Z9")]
        public void InvalidMove_ReturnsError_WithoutChangingTurn(string position)
        {
            var session = CreateSession();
            Assert.That(session.TryMakeMove(position, out string error), Is.False);
            Assert.That(error, Is.EqualTo("Choose an empty cell on the board."));
            Assert.That(session.CurrentPlayer, Is.SameAs(session.Player1));
            Assert.That(session.Board.Board.Values, Is.All.Empty);
        }

        [Test]
        public void Move_NormalizesWhitespaceAndCase()
        {
            var session = CreateSession();
            Assert.That(session.TryMakeMove(" a1 ", out string error), Is.True);
            Assert.That(error, Is.Null);
            Assert.That(session.Board.Board["A1"], Is.EqualTo("X"));
        }

        [TestCase(null, "X")]
        [TestCase(" ", "X")]
        [TestCase("Alice", null)]
        [TestCase("Alice", " ")]
        public void Player_RejectsEmptyNameOrSymbol(string name, string symbol)
        {
            Assert.Throws<ArgumentException>(() => new PlayerBase(name, symbol));
        }

        [Test]
        public void Session_RejectsSameSymbol_AfterNormalization()
        {
            Assert.Throws<ArgumentException>(() => new GameSession(new PlayerA("Alice", " x "), new PlayerB("Bob", "X")));
        }

        [Test]
        public void Session_RejectsNullPlayers()
        {
            Assert.Throws<ArgumentNullException>(() => new GameSession(null, new PlayerB("Bob", "O")));
            Assert.Throws<ArgumentNullException>(() => new GameSession(new PlayerA("Alice", "X"), null));
        }

        private static GameSession CreateSession()
        {
            return new GameSession(new PlayerA("Alice", "X"), new PlayerB("Bob", "O"));
        }
    }
}
