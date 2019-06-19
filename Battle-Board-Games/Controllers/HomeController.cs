using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using BattleBoardGame.Models;
using Microsoft.AspNetCore.Authorization;
using Service_Battle_Board_Games.Interfaces;

namespace BattleBoardGame.Controllers
{
    public class HomeController : Controller
    {

        private readonly IBatalhaService _batalhaService;

        public HomeController(IBatalhaService batalhaService)
        {
            _batalhaService = batalhaService;
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Aplicação desenvolvida para o curso de Análise" +
            	" e Desenvolvimento de Sistemas da Universidade Positivo.";

            return View();
        }

        [Authorize]
        public IActionResult Contact()
        {
            ViewData["Message"] = "Contatos do Professor.";

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }
      
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [AllowAnonymous]
        public ActionResult Index()
        {
            ViewBag.Title = "Home Page";
            bool usuarioAutenticado = true;
/*                Utils.Utils.ObterUsuarioLogado(
                    ctx
                    ) != null;
*/
            if (!usuarioAutenticado)
            {
                return RedirectToAction("Login");
            }

            return View();
        }

        public ActionResult Tabuleiro(int BatalhaId = -1)
        {
            ViewBag.Title = "Tabuleiro";
            var batalha = _batalhaService.BuscarPorIdAsync(BatalhaId);

            if (batalha != null)
                return View(batalha);

            return View();
        }

        public ActionResult Login(string usuario, string password, string rememberme, string returnurl)
        {
            return View();
        }
    }
}
