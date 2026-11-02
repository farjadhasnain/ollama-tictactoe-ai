using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.AI;

namespace TicTacToe.Tests
{
    internal class TestChatClient : IChatClient
    {
        public string Response { get; set; } = "B2";
        public Exception Failure { get; set; }
        public bool WaitForCancellation { get; set; }
        public int Calls { get; private set; }
        public string Prompt { get; private set; }
        public float? Temperature { get; private set; }
        public CancellationToken Token { get; private set; }

        public async Task<ChatResponse> GetResponseAsync(IEnumerable<ChatMessage> messages, ChatOptions options = null,
            CancellationToken cancellationToken = default)
        {
            Calls++;
            Prompt = string.Join("\n", messages.Select(message => message.Text));
            Temperature = options?.Temperature;
            Token = cancellationToken;
            if (WaitForCancellation) await Task.Delay(Timeout.Infinite, cancellationToken);
            if (Failure != null) throw Failure;
            return new ChatResponse(new ChatMessage(ChatRole.Assistant, Response));
        }

        public IAsyncEnumerable<ChatResponseUpdate> GetStreamingResponseAsync(IEnumerable<ChatMessage> messages,
            ChatOptions options = null, CancellationToken cancellationToken = default)
        {
            throw new NotSupportedException();
        }

        public object GetService(Type serviceType, object serviceKey = null) => null;
        public void Dispose() { }
    }
}
