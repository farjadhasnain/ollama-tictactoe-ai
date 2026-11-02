# Tic Tac Toe

Console and ASP.NET Core MVC applications using the same C# game engine. Both support two people playing on the same device and a human playing against an Ollama AI opponent.

## Run

Install the .NET 10 SDK, then run either frontend from the repository root:

```powershell
# Console
dotnet run --project TicTacToe.App

# Web: open http://localhost:5080
dotnet run --project TicTacToe.Web -- --urls http://localhost:5080
```

For AI games, start Ollama and install the configured model:

```powershell
ollama pull gemma4
ollama serve
```

The default endpoint is `http://localhost:11434` and the default model is `gemma4`. Set `Ollama__Endpoint` and `Ollama__Model` environment variables to override them for either frontend. The web defaults are also in `TicTacToe.Web/appsettings.json`.

The AI plays O in the web app and chooses a different symbol from the human in the console. Difficulty adjusts the existing prompt and temperature; hard mode does not guarantee optimal play. If Ollama is unavailable, exceeds the 30-second request timeout, or returns an invalid move, the AI plays the first available cell, preserving the original fallback behavior.

## Projects

- `GameEngine`: board, players, move validation, turn order, winning positions, and results. Contains no UI or Ollama dependencies. Submit moves through `GameSession.TryMakeMove`; the board and player identities are read-only to consumers.
- `TicTacToe.Ai`: asynchronous Ollama-backed opponent using an injected `IChatClient`. It determines its move; each frontend applies that move through the engine.
- `TicTacToe.App`: console input, board rendering, and game loop.
- `TicTacToe.Web`: MVC controllers, typed view models, Razor views, game orchestration, and server-side storage. No JavaScript is required to play; JavaScript adds the thinking indicator and mode controls.
- `TicTacToe.Tests`: NUnit tests for engine behavior, AI responses/failures/cancellation, and web game isolation and turn handling.

Web games are isolated by a session cookie and retained in memory for one idle hour. Restarting the server clears them. Multiple tabs in one browser share a game; revision checks reject stale or duplicate moves. An interrupted AI turn can be retried from the board. Online multiplayer across devices and persistent game history are outside this version.

All projects now target .NET 10 instead of .NET Framework 4.7.2. A future C# frontend can reference `GameEngine` directly; a frontend in another language can use a host exposing the same engine over HTTP.

## Verify

```powershell
dotnet build TicTacToe.App.sln -c Release
dotnet test TicTacToe.Tests/TicTacToe.Tests.csproj -c Release --settings TicTacToe.Tests/coverage.runsettings --collect:"Code Coverage;Format=Cobertura"
```
