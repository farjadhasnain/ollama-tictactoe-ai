using GameEngine.Enums;
using TicTacToe.App.Player;

namespace GameEngine
{

    public class GameSession
    {
        public GameBoard Board { get; }
        public PlayerBase CurrentPlayer { get; private set; }
        public PlayerBase Winner { get; private set; }
        public GameStatus Status { get; private set; }

        private readonly PlayerBase _player1;
        private readonly PlayerBase _player2;

        public GameSession(PlayerBase player1, PlayerBase player2)
        {
            _player1 = player1;
            _player2 = player2;
            Board = new GameBoard();
            CurrentPlayer = player1;
            Status = GameStatus.InProgress;
        }

        public bool TryMakeMove(string position)
        {
            if (Status != GameStatus.InProgress)
            {
                return false;
            }

            if (!CurrentPlayer.IsValidMove(Board.Board, position))
            {
                return false;
            }

            CurrentPlayer.MakeMove(Board.Board, position);

            if (Board.CheckWin())
            {
                Winner = CurrentPlayer;
                Status = GameStatus.Won;
            }
            else if (Board.CheckTie())
            {
                Status = GameStatus.Draw;
            }
            else
            {
                CurrentPlayer = CurrentPlayer == _player1 ? _player2 : _player1;
            }

            return true;
        }

        public PlayerBase Player1 => _player1;
        public PlayerBase Player2 => _player2;
    }
}