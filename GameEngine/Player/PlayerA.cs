namespace GameEngine.Player
{
    public class PlayerA : PlayerBase
    {
        public PlayerA(string name, string symbol) : base(name, symbol)
        {
            
        }
        //public string Name { get; set; }
        //public int Id { get; set; }
        //public string Symbol { get; set; }

        //public void MakeMove(Dictionary<string, string> board, string position)
        //{
        //    if (board.ContainsKey(position) && board[position] == "")
        //    {
        //        board[position] = Symbol;
        //    }
        //    else
        //    {
        //        Console.WriteLine($"Invalid move for {Name}. Please try again.");
        //    }
        //}
    }
}
