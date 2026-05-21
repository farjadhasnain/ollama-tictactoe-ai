using NUnit.Framework;
using GameEngine;
using TicTacToe.App.Player;
using GameEngine.Enums;

namespace TicTacToe.Tests
{
    [TestFixture]
    public class GameSessionTests
    {
        private PlayerA _player1;
        private PlayerB _player2;
        private GameSession _session;

        [SetUp]
        public void SetUp()
        {
            _player1 = new PlayerA("Alice", "X");
            _player2 = new PlayerB("Bob", "O");
            _session = new GameSession(_player1, _player2);
        }

        [Test]
        public void Status_IsInProgress_WhenGameStarts()
        {
            Assert.That(_session.Status, Is.EqualTo(GameStatus.InProgress));
        }

        [Test]
        public void CurrentPlayer_IsPlayer1_WhenGameStarts()
        {
            Assert.That(_session.CurrentPlayer, Is.EqualTo(_player1));
        }

        [Test]
        public void Winner_IsNull_WhenGameStarts()
        {
            Assert.That(_session.Winner, Is.Null);
        }

        [Test]
        public void TryMakeMove_ReturnsTrue_WhenMoveIsValid()
        {
            Assert.That(_session.TryMakeMove("A1"), Is.True);
        }

        [Test]
        public void TryMakeMove_ReturnsFalse_WhenPositionIsAlreadyTaken()
        {
            _session.TryMakeMove("A1");

            Assert.That(_session.TryMakeMove("A1"), Is.False);
        }

        [Test]
        public void TryMakeMove_ReturnsFalse_WhenPositionDoesNotExist()
        {
            Assert.That(_session.TryMakeMove("Z9"), Is.False);
        }

        [Test]
        public void TryMakeMove_PlacesCurrentPlayerSymbol_OnBoard()
        {
            _session.TryMakeMove("A1");

            Assert.That(_session.Board.Board["A1"], Is.EqualTo("X"));
        }

        [Test]
        public void TryMakeMove_DoesNotAlterBoard_WhenMoveIsInvalid()
        {
            _session.TryMakeMove("A1"); // player1 plays A1
            _session.TryMakeMove("A1"); // player2 tries same cell — invalid

            Assert.That(_session.Board.Board["A1"], Is.EqualTo("X"));
        }

        [Test]
        public void TryMakeMove_SwitchesToPlayer2_AfterPlayer1Moves()
        {
            _session.TryMakeMove("A1");

            Assert.That(_session.CurrentPlayer, Is.EqualTo(_player2));
        }

        [Test]
        public void TryMakeMove_SwitchesBackToPlayer1_AfterBothPlayersMoved()
        {
            _session.TryMakeMove("A1"); // player1
            _session.TryMakeMove("B1"); // player2

            Assert.That(_session.CurrentPlayer, Is.EqualTo(_player1));
        }

        [Test]
        public void TryMakeMove_DoesNotSwitchPlayer_WhenMoveIsInvalid()
        {
            _session.TryMakeMove("A1"); // player1 — valid, now player2's turn
            _session.TryMakeMove("Z9"); // player2 — invalid, should stay player2

            Assert.That(_session.CurrentPlayer, Is.EqualTo(_player2));
        }

        [Test]
        public void TryMakeMove_SetsStatusToWon_WhenWinningMoveIsPlayed()
        {
            // player1: A1, A2, A3 — top row win
            _session.TryMakeMove("A1"); // p1
            _session.TryMakeMove("B1"); // p2
            _session.TryMakeMove("A2"); // p1
            _session.TryMakeMove("B2"); // p2
            _session.TryMakeMove("A3"); // p1 wins

            Assert.That(_session.Status, Is.EqualTo(GameStatus.Won));
        }

        [Test]
        public void TryMakeMove_SetsWinner_WhenWinningMoveIsPlayed()
        {
            _session.TryMakeMove("A1"); // p1
            _session.TryMakeMove("B1"); // p2
            _session.TryMakeMove("A2"); // p1
            _session.TryMakeMove("B2"); // p2
            _session.TryMakeMove("A3"); // p1 wins

            Assert.That(_session.Winner, Is.EqualTo(_player1));
        }

        [Test]
        public void TryMakeMove_ReturnsFalse_WhenGameIsAlreadyOver()
        {
            _session.TryMakeMove("A1"); // p1
            _session.TryMakeMove("B1"); // p2
            _session.TryMakeMove("A2"); // p1
            _session.TryMakeMove("B2"); // p2
            _session.TryMakeMove("A3"); // p1 wins — game over

            Assert.That(_session.TryMakeMove("C1"), Is.False);
        }

        [Test]
        public void TryMakeMove_SetsStatusToDraw_WhenBoardIsFullWithNoWinner()
        {
            // X O X
            // X O O
            // O X X  — no winner, full board
            _session.TryMakeMove("A1"); // p1 X
            _session.TryMakeMove("A2"); // p2 O
            _session.TryMakeMove("A3"); // p1 X
            _session.TryMakeMove("B2"); // p2 O
            _session.TryMakeMove("B1"); // p1 X
            _session.TryMakeMove("B3"); // p2 O
            _session.TryMakeMove("C2"); // p1 X
            _session.TryMakeMove("C1"); // p2 O
            _session.TryMakeMove("C3"); // p1 X — draw

            Assert.That(_session.Status, Is.EqualTo(GameStatus.Draw));
        }

        [Test]
        public void TryMakeMove_WinnerRemainsNull_OnDraw()
        {
            _session.TryMakeMove("A1"); // p1 X
            _session.TryMakeMove("A2"); // p2 O
            _session.TryMakeMove("A3"); // p1 X
            _session.TryMakeMove("B2"); // p2 O
            _session.TryMakeMove("B1"); // p1 X
            _session.TryMakeMove("B3"); // p2 O
            _session.TryMakeMove("C2"); // p1 X
            _session.TryMakeMove("C1"); // p2 O
            _session.TryMakeMove("C3"); // p1 X — draw

            Assert.That(_session.Winner, Is.Null);
        }
    }
}
