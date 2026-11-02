using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameEngine.Enums;
using Microsoft.Extensions.AI;
using GameEngine.Player;
using System.Threading;

namespace TicTacToe.Ai
{
    public class PlayerAi : PlayerBase
    {
        public DifficultyLevel Difficulty { get; }

        private readonly IChatClient _chatClient;

        public PlayerAi(string name, string symbol, DifficultyLevel difficulty, IChatClient chatClient)
            : base(name, symbol)
        {
            Difficulty = difficulty;
            _chatClient = chatClient ?? throw new ArgumentNullException(nameof(chatClient));
        }

        public async Task<string> GetMoveAsync(IReadOnlyDictionary<string, string> board, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (!board.Any(cell => cell.Value == "")) return null;

            using var timeout = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            timeout.CancelAfter(TimeSpan.FromSeconds(30));

            try
            {
                string prompt = BuildPrompt(board);
                ChatOptions options = new ChatOptions { Temperature = GetTemperature() };
                var response = await _chatClient.GetResponseAsync(prompt, options, timeout.Token);
                cancellationToken.ThrowIfCancellationRequested();

                return ExtractPosition(response.Text?.Trim().ToUpperInvariant() ?? "", board);
            }
            catch (Exception) when (!cancellationToken.IsCancellationRequested)
            {
                return board.FirstOrDefault(cell => cell.Value == "").Key;
            }
        }

        private string BuildPrompt(IReadOnlyDictionary<string, string> board)
        {
            string persona;
            switch (Difficulty)
            {
                case DifficultyLevel.Easy:
                    persona = "You are a beginner Tic Tac Toe player. You sometimes miss obvious wins and forget to block the opponent.";
                    break;
                case DifficultyLevel.Hard:
                    persona = "You are an expert Tic Tac Toe player. Always play the optimal move — win immediately if possible, otherwise block the opponent.";
                    break;
                default:
                    persona = "You are an average Tic Tac Toe player. Take obvious wins and block obvious threats but you are not perfect.";
                    break;
            }

            List<string> available = board
                .Where(x => x.Value == "")
                .Select(x => x.Key)
                .ToList();

            return new StringBuilder()
                .AppendLine(persona)
                .AppendLine()
                .AppendLine($"You are playing as '{Symbol}'.")
                .AppendLine()
                .AppendLine("Current board (. = empty):")
                .AppendLine(BuildBoardDisplay(board))
                .AppendLine()
                .AppendLine($"Available positions: {string.Join(", ", available)}")
                .AppendLine()
                .AppendLine("Reply with ONLY the position (e.g. A1). No explanation, no punctuation.")
                .ToString();
        }

        private string BuildBoardDisplay(IReadOnlyDictionary<string, string> board)
        {
            return new StringBuilder()
                .AppendLine("     1   |   2   |   3")
                .AppendLine($"  A  {Cell(board,"A1")}   |   {Cell(board,"A2")}   |   {Cell(board,"A3")}")
                .AppendLine("    -----|-------|-----")
                .AppendLine($"  B  {Cell(board,"B1")}   |   {Cell(board,"B2")}   |   {Cell(board,"B3")}")
                .AppendLine("    -----|-------|-----")
                .AppendLine($"  C  {Cell(board,"C1")}   |   {Cell(board,"C2")}   |   {Cell(board,"C3")}")
                .ToString();
        }

        private string Cell(IReadOnlyDictionary<string, string> board, string key)
        {
            return board[key] == "" ? "." : board[key];
        }

        // Higher temperature = more random = easier
        private float GetTemperature()
        {
            switch (Difficulty)
            {
                case DifficultyLevel.Easy:   return 1.5f;
                case DifficultyLevel.Hard:   return 0.1f;
                default:                     return 0.7f;
            }
        }

        // Scans the LLM response for a valid board position
        // Handles cases where the LLM adds extra words like "I'll play A1"
        private string ExtractPosition(string raw, IReadOnlyDictionary<string, string> board)
        {
            foreach (string key in board.Keys)
            {
                if (raw.Contains(key) && board[key] == "")
                    return key;
            }

            // Fallback: first available cell if the response was unparseable
            foreach (var pair in board)
            {
                if (pair.Value == "") return pair.Key;
            }

            return null;
        }
    }
}
