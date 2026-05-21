using System.Collections.Generic;
using NUnit.Framework;
using TicTacToe.App.Player;

namespace TicTacToe.Tests
{
    [TestFixture]
    public class PlayerBaseTests
    {
        // --- IsValidMove ---

        [Test]
        public void IsValidMove_ReturnsTrue_WhenPositionExistsAndIsEmpty()
        {
            var player = new PlayerA("P1", "X");
            var board = new Dictionary<string, string> { { "A1", "" } };

            Assert.That(player.IsValidMove(board, "A1"), Is.True);
        }

        [Test]
        public void IsValidMove_ReturnsFalse_WhenPositionIsAlreadyTaken()
        {
            var player = new PlayerA("P1", "X");
            var board = new Dictionary<string, string> { { "A1", "O" } };

            Assert.That(player.IsValidMove(board, "A1"), Is.False);
        }

        [Test]
        public void IsValidMove_ReturnsFalse_WhenPositionDoesNotExistOnBoard()
        {
            var player = new PlayerA("P1", "X");
            var board = new Dictionary<string, string> { { "A1", "" } };

            Assert.That(player.IsValidMove(board, "Z9"), Is.False);
        }

        // --- MakeMove ---

        [Test]
        public void MakeMove_PlacesPlayerSymbol_WhenMoveIsValid()
        {
            var player = new PlayerA("P1", "X");
            var board = new Dictionary<string, string> { { "A1", "" } };

            player.MakeMove(board, "A1");

            Assert.That(board["A1"], Is.EqualTo("X"));
        }

        [Test]
        public void MakeMove_DoesNotValidate_CallerIsResponsible()
        {
            // MakeMove is intentionally simple — it just places the symbol.
            // Validation is the caller's responsibility (GameSession.TryMakeMove).
            var player = new PlayerA("P1", "X");
            var board = new Dictionary<string, string> { { "A1", "" }, { "A2", "" } };

            player.MakeMove(board, "A1");
            player.MakeMove(board, "A2");

            Assert.That(board["A1"], Is.EqualTo("X"));
            Assert.That(board["A2"], Is.EqualTo("X"));
        }

        // --- Player symbol and name ---

        [Test]
        public void PlayerA_StoresName_AndSymbol_Correctly()
        {
            var player = new PlayerA("Alice", "X");

            Assert.That(player.Name, Is.EqualTo("Alice"));
            Assert.That(player.Symbol, Is.EqualTo("X"));
        }

        [Test]
        public void PlayerB_StoresName_AndSymbol_Correctly()
        {
            var player = new PlayerB("Bob", "O");

            Assert.That(player.Name, Is.EqualTo("Bob"));
            Assert.That(player.Symbol, Is.EqualTo("O"));
        }
    }
}
