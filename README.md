Ollama Tic-Tac-Toe AI
## Important Note

This project intentionally uses a generative AI model for experimentation and research purposes.

For a traditional Tic-Tac-Toe AI, deterministic algorithms such as minimax would be faster and more optimal.

The goal of this project is to explore the use of local LLMs as a behavioural decision layer for NPC development and game AI experimentation.

!!WARNING, This is an experimental project, This project explores generative AI-driven NPC behaviour using local LLMs.
A C# console Tic-Tac-Toe game using Ollama and OllamaSharp to power an AI opponent through a local large language model.

Features
Human vs Human mode
Human vs AI mode
AI difficulty selection
Local LLM integration using Ollama
Prompt-driven NPC behaviour
JSON move validation
Technologies
C#
.NET
Ollama
OllamaSharp
Local LLMs
Why?

This project explores the use of generative AI as a behavioural layer for NPC development.

Instead of hardcoding every move, the game passes the board state and difficulty level into a prompt, and the AI generates its next move dynamically.

Example
X - O
- X -
- - O

AI response:

{ "row": 2, "col": 1 }
Future Ideas
Personality-based NPCs
Adaptive AI opponents
Memory-aware NPCs
Voice-controlled NPC interaction
AI-driven strategy games
