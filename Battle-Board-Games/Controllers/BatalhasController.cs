using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service_Battle_Board_Games.Interfaces;

namespace Battle_Board_Games.Controllers
{
    [Authorize]
    [Route("[controller]")]
    public class BatalhasController : Controller
    {

        private readonly IBatalhaService _batalhaService;

        public BatalhasController(IBatalhaService batalhaService)
        {
            _batalhaService = batalhaService;
        }

        [Route("Lobby/{batalhaId}")]
        [HttpGet()]
        public ActionResult Lobby(int batalhaId)
        {
            var batalha = _batalhaService.BuscarLobby(batalhaId).Result;
            ViewBag.Id = batalha.Id;
            return View(batalha);
        }

        [Route("Tabuleiro/{batalhaId}")]
        [HttpGet()]
        public ActionResult Tabuleiro(int batalhaId)
        {
            var batalha = _batalhaService.BuscarTabuleiroAsync(batalhaId).Result;
            ViewBag.Id = batalha.Id;
            return View(batalha);
        }

    }
}