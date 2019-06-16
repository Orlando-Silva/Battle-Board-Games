using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using BattleBoardGame.Model;
using BattleBoardGame.Model.DAL;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static BattleBoardGame.Model.Factory.AbstractFactoryExercito;

namespace Battle_Board_Games.Controllers
{
    [Authorize]
    [Route("[controller]")]
    public class BatalhasController : Controller
    {

        private readonly ModelJogosDeGuerra _context;
        public BatalhasController(ModelJogosDeGuerra context)
        {
            this._context = context;
        }

        [Route("Lobby/{batalhaId}")]
        [HttpGet()]
        public ActionResult Lobby(int batalhaId)
        {
            var batalha = _context.Batalhas
                .Where(x => x.Id.Equals(batalhaId))
                .Include(b => b.ExercitoBranco)
                .Include(b => b.ExercitoBranco.Usuario)
                .Include(b => b.ExercitoPreto)
                .Include(b => b.ExercitoPreto.Usuario)
                .Include(b => b.Tabuleiro)
                .Include(b => b.Turno)
                .Include(b => b.Turno.Usuario)
                .Include(b => b.Vencedor)
                .Include(b => b.Vencedor.Usuario)
                .FirstOrDefault();
            ViewBag.Id = batalha.Id;
            return View(batalha);
        }

        [Route("Tabuleiro/{batalhaId}")]
        [HttpGet]
        public ActionResult Tabuleiro(int batalhaId)
        {
            var batalha = _context.Batalhas
                .Where(x => x.Id.Equals(batalhaId))
                .Include(b => b.ExercitoBranco)
                .Include(b => b.ExercitoBranco.Usuario)
                .Include(b => b.ExercitoPreto)
                .Include(b => b.ExercitoPreto.Usuario)
                .Include(b => b.Tabuleiro)
                .Include(b => b.Turno)
                .Include(b => b.Turno.Usuario)
                .Include(b => b.Vencedor)
                .Include(b => b.Vencedor.Usuario)
                .FirstOrDefault();
            ViewBag.Id = batalha.Id;
            return View(batalha);
        }

    }
}