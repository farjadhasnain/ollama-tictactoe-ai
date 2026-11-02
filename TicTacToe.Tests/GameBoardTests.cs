using System.Collections.Generic;
using GameEngine;
using GameEngine.Player;
using NUnit.Framework;

namespace TicTacToe.Tests
{
    [TestFixture]
    public class GameBoardTests
    {
        [Test]
        public void NewBoard_HasNineEmptyCells_AndNoResult()
        {
            var board = new GameBoard();
            Assert.That(board.Board.Count, Is.EqualTo(9));
            Assert.That(board.Board.Values, Is.All.Empty);
            Assert.That(board.CheckWin(), Is.False);
            Assert.That(board.CheckTie(), Is.False);
            Assert.That(board.IsBoardFull(), Is.False);
        }

        [TestCase("A1,B1,A2,B2,A3")]
        [TestCase("B1,A1,B2,A2,B3")]
        [TestCase("C1,A1,C2,A2,C3")]
        [TestCase("A1,A2,B1,B2,C1")]
        [TestCase("A2,A1,B2,B1,C2")]
        [TestCase("A3,A1,B3,B1,C3")]
        [TestCase("A1,A2,B2,B1,C3")]
        [TestCase("A3,A1,B2,B1,C1")]
        public void CheckWin_DetectsEveryWinningLine(string moves)
        {
            var session = Play(moves);
            Assert.That(session.Board.CheckWin(), Is.True);
            Assert.That(session.Board.GetWinningPositions(), Has.Count.EqualTo(3));
            Assert.That(session.Board.CheckTie(), Is.False);
            Assert.That(session.Board.IsBoardFull(), Is.False);
        }

        [Test]
        public void CheckWin_ReturnsFalse_ForMixedSymbols()
        {
            var session = Play("A1,A2,A3");
            Assert.That(session.Board.CheckWin(), Is.False);
            Assert.That(session.Board.CheckTie(), Is.False);
        }

        [Test]
        public void FullBoard_WithNoWinner_IsDraw()
        {
            var session = Play("A1,A2,A3,B2,B1,B3,C2,C1,C3");
            Assert.That(session.Board.IsBoardFull(), Is.True);
            Assert.That(session.Board.CheckTie(), Is.True);
            Assert.That(session.Board.CheckWin(), Is.False);
        }

        [Test]
        public void FullBoard_WithWinner_IsNotDraw()
        {
            var session = Play("A1,A2,A3,B1,B2,B3,C2,C1,C3");
            Assert.That(session.Board.IsBoardFull(), Is.True);
            Assert.That(session.Board.CheckWin(), Is.True);
            Assert.That(session.Board.GetWinningPositions(), Has.Count.EqualTo(3));
            Assert.That(session.Board.CheckTie(), Is.False);
        }

        [Test]
        public void Board_CannotBeModifiedByConsumer()
        {
            var board = new GameBoard();
            Assert.Throws<System.NotSupportedException>(() => ((IDictionary<string, string>)board.Board)["A1"] = "X");
            Assert.That(board.Board["A1"], Is.Empty);
        }

        private static GameSession Play(string moves)
        {
            var session = new GameSession(new PlayerA("Alice", "X"), new PlayerB("Bob", "O"));
            foreach (string move in moves.Split(',')) Assert.That(session.TryMakeMove(move), Is.True);
            return session;
        }
    }
}
