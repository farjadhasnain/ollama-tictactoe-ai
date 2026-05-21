using NUnit.Framework;
using GameEngine;

namespace TicTacToe.Tests
{
    [TestFixture]
    public class GameBoardTests
    {
        private GameBoard _gameBoard;

        [SetUp]
        public void SetUp()
        {
            _gameBoard = new GameBoard();
        }

        [Test]
        public void CheckWin_ReturnsFalse_WhenBoardIsEmpty()
        {
            Assert.That(_gameBoard.CheckWin(), Is.False);
        }

        [Test]
        public void CheckWin_ReturnsTrue_WhenTopRowComplete()
        {
            _gameBoard.Board["A1"] = "X";
            _gameBoard.Board["A2"] = "X";
            _gameBoard.Board["A3"] = "X";

            Assert.That(_gameBoard.CheckWin(), Is.True);
        }

        [Test]
        public void CheckWin_ReturnsTrue_WhenMiddleRowComplete()
        {
            _gameBoard.Board["B1"] = "O";
            _gameBoard.Board["B2"] = "O";
            _gameBoard.Board["B3"] = "O";

            Assert.That(_gameBoard.CheckWin(), Is.True);
        }

        [Test]
        public void CheckWin_ReturnsTrue_WhenBottomRowComplete()
        {
            _gameBoard.Board["C1"] = "X";
            _gameBoard.Board["C2"] = "X";
            _gameBoard.Board["C3"] = "X";

            Assert.That(_gameBoard.CheckWin(), Is.True);
        }

        [Test]
        public void CheckWin_ReturnsTrue_WhenFirstColumnComplete()
        {
            _gameBoard.Board["A1"] = "X";
            _gameBoard.Board["B1"] = "X";
            _gameBoard.Board["C1"] = "X";

            Assert.That(_gameBoard.CheckWin(), Is.True);
        }

        [Test]
        public void CheckWin_ReturnsTrue_WhenSecondColumnComplete()
        {
            _gameBoard.Board["A2"] = "O";
            _gameBoard.Board["B2"] = "O";
            _gameBoard.Board["C2"] = "O";

            Assert.That(_gameBoard.CheckWin(), Is.True);
        }

        [Test]
        public void CheckWin_ReturnsTrue_WhenThirdColumnComplete()
        {
            _gameBoard.Board["A3"] = "X";
            _gameBoard.Board["B3"] = "X";
            _gameBoard.Board["C3"] = "X";

            Assert.That(_gameBoard.CheckWin(), Is.True);
        }

        [Test]
        public void CheckWin_ReturnsTrue_WhenTopLeftToBottomRightDiagonalComplete()
        {
            _gameBoard.Board["A1"] = "O";
            _gameBoard.Board["B2"] = "O";
            _gameBoard.Board["C3"] = "O";

            Assert.That(_gameBoard.CheckWin(), Is.True);
        }

        [Test]
        public void CheckWin_ReturnsTrue_WhenTopRightToBottomLeftDiagonalComplete()
        {
            _gameBoard.Board["A3"] = "X";
            _gameBoard.Board["B2"] = "X";
            _gameBoard.Board["C1"] = "X";

            Assert.That(_gameBoard.CheckWin(), Is.True);
        }

        [Test]
        public void CheckWin_ReturnsFalse_WhenRowHasMixedSymbols()
        {
            _gameBoard.Board["A1"] = "X";
            _gameBoard.Board["A2"] = "O";
            _gameBoard.Board["A3"] = "X";

            Assert.That(_gameBoard.CheckWin(), Is.False);
        }

        [Test]
        public void IsBoardFull_ReturnsFalse_WhenBoardIsEmpty()
        {
            Assert.That(_gameBoard.IsBoardFull(), Is.False);
        }

        [Test]
        public void IsBoardFull_ReturnsFalse_WhenBoardIsPartiallyFilled()
        {
            _gameBoard.Board["A1"] = "X";
            _gameBoard.Board["B2"] = "O";

            Assert.That(_gameBoard.IsBoardFull(), Is.False);
        }

        [Test]
        public void IsBoardFull_ReturnsTrue_WhenAllCellsAreFilled()
        {
            var keys = new System.Collections.Generic.List<string>(_gameBoard.Board.Keys);
            foreach (var key in keys)
            {
                _gameBoard.Board[key] = "X";
            }

            Assert.That(_gameBoard.IsBoardFull(), Is.True);
        }

        [Test]
        public void CheckTie_ReturnsFalse_WhenBoardIsEmpty()
        {
            Assert.That(_gameBoard.CheckTie(), Is.False);
        }

        [Test]
        public void CheckTie_ReturnsTrue_WhenBoardIsFullWithNoWinner()
        {
            // No row, column, or diagonal shares the same symbol
            _gameBoard.Board["A1"] = "X"; _gameBoard.Board["A2"] = "O"; _gameBoard.Board["A3"] = "X";
            _gameBoard.Board["B1"] = "X"; _gameBoard.Board["B2"] = "O"; _gameBoard.Board["B3"] = "O";
            _gameBoard.Board["C1"] = "O"; _gameBoard.Board["C2"] = "X"; _gameBoard.Board["C3"] = "X";

            Assert.That(_gameBoard.CheckTie(), Is.True);
        }

        [Test]
        public void CheckTie_ReturnsFalse_WhenBoardIsFullButThereIsAWinner()
        {
            // Top row is all X - there is a winner, so not a tie
            _gameBoard.Board["A1"] = "X"; _gameBoard.Board["A2"] = "X"; _gameBoard.Board["A3"] = "X";
            _gameBoard.Board["B1"] = "O"; _gameBoard.Board["B2"] = "O"; _gameBoard.Board["B3"] = "X";
            _gameBoard.Board["C1"] = "X"; _gameBoard.Board["C2"] = "O"; _gameBoard.Board["C3"] = "O";

            Assert.That(_gameBoard.CheckTie(), Is.False);
        }
    }
}
