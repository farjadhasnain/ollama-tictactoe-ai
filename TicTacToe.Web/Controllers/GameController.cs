using GameEngine.Enums;
using Microsoft.AspNetCore.Mvc;
using TicTacToe.Web.Models;

namespace TicTacToe.Web.Controllers
{
    [AutoValidateAntiforgeryToken]
    public class GameController(GameStore games) : Controller
    {
        [HttpGet]
        public async Task<IActionResult> Index(CancellationToken cancellationToken)
        {
            return View(new GamePageViewModel
            {
                Game = await games.ReadAsync(BrowserId(), cancellationToken),
                Error = TempData["Error"] as string
            });
        }

        [HttpPost]
        public async Task<IActionResult> Start(string mode, string name1, string name2,
            DifficultyLevel difficulty, Guid revision, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid || (mode != "human" && mode != "ai"))
                TempData["Error"] = "Choose a valid game mode and difficulty.";
            else
                TempData["Error"] = await games.StartAsync(BrowserId(), mode == "ai", name1, name2, difficulty, revision, cancellationToken);

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Move(string position, Guid revision, CancellationToken cancellationToken)
        {
            TempData["Error"] = await games.MoveAsync(BrowserId(), position, revision, cancellationToken);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> RetryAi(Guid revision, CancellationToken cancellationToken)
        {
            TempData["Error"] = await games.RetryAiAsync(BrowserId(), revision, cancellationToken);
            return RedirectToAction(nameof(Index));
        }

        [Route("/Error")]
        [IgnoreAntiforgeryToken]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View();
        }

        private string BrowserId()
        {
            string id = HttpContext.Session.GetString("GameId");
            if (id == null)
            {
                id = Guid.NewGuid().ToString();
                HttpContext.Session.SetString("GameId", id);
            }
            return id;
        }
    }
}
