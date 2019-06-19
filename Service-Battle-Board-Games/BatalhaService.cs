using BattleBoardGame.Model;
using BattleBoardGame.Model.DAL;
using BattleBoardGame.Model.Factory;
using Microsoft.EntityFrameworkCore;
using Service_Battle_Board_Games.Exceptions;
using Service_Battle_Board_Games.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static BattleBoardGame.Model.Factory.AbstractFactoryExercito;

namespace Service_Battle_Board_Games
{
    public class BatalhaService : Service, IBatalhaService
    {
        private const int AlturaDoTabuleiro = 8;
        private const int LarguraDoTabuleiro = 8;

        public BatalhaService(ModelJogosDeGuerra context) : base(context)
        {

        }

        public async Task<Batalha> AtualizarAsync(int id, Batalha batalha)
        {

            if (IdentificadoresDiferentes(id, batalha))
            {
                throw new BatalhaServiceExeception($"Os identificadores da batalha não são iguais.");
            }

            await RegistrarAtualizacao(batalha);

            return batalha;

        }

        public async Task<Batalha> EnviarAsync(Batalha batalha)
        {
            batalha = (await _context.Batalhas.AddAsync(batalha)).Entity;
            await _context.SaveChangesAsync();
            return batalha;
        }

        public async Task<IEnumerable<Batalha>> BuscarAsync(bool Finalizada = false)
        {
            return Finalizada
                ? _context.Batalhas.Where(b => b.Estado == Batalha.EstadoBatalhaEnum.Finalizado).ToList()
                : _context.Batalhas.ToList();
        }

        public async Task<int> ContarPorJogador(string jogador)
        {
            return await _context.Batalhas
                .Where(b => (b.ExercitoBranco != null &&
                            b.ExercitoBranco.UsuarioId == jogador)
                            ||
                            (b.ExercitoPreto != null &&
                            b.ExercitoPreto.UsuarioId == jogador))
                .CountAsync();
        }

        public async Task<Batalha> BuscarPorJogador(int id, string jogador)
        {
            return await _context.Batalhas
                .Include(b => b.ExercitoPreto)
                .Include(b => b.ExercitoBranco)
                .Include(b => b.Tabuleiro)
                .Include(b => b.Turno)
                .Include(b => b.Turno.Usuario)
                .FirstOrDefaultAsync(b => (b.ExercitoBranco.Usuario.Id == jogador
                                        || b.ExercitoPreto.Usuario.Id == jogador)
                                        && (b.ExercitoBranco != null && b.ExercitoPreto != null)
                                        && b.Id == id);
        }

        public async Task<int> BuscarQuantidadeAsync()
        {
            return await _context.Batalhas.CountAsync();
        }

        public async Task<Batalha> CriarAsync(Usuario jogador)
        {
            var batalha = BuscarBatalhasDisponiveis(jogador.Id);

            if (batalha is null)
            {
                batalha = new Batalha();
                await _context.Batalhas.AddAsync(batalha);
            }

            AdicionarJogadorABatalha(jogador, batalha);
            await _context.SaveChangesAsync();
            return batalha;
        }

        public async Task<Batalha> DeletarAsync(int id)
        {
            var batalha = await _context.Batalhas.FindAsync(id)
                ?? throw new BatalhaServiceExeception("Não foi possível encontrar a batalha");

            _context.Batalhas.Remove(batalha);
            await _context.SaveChangesAsync();
            return batalha;
        }

        public async Task<Exercito> EscolherNacaoAsync(Nacao nacao, int exercitoId)
        {
            var exercito = await _context.Exercitos.FindAsync(exercitoId)
                ?? throw new BatalhaServiceExeception($"Não foi possível encontrar o exercíto { exercitoId }");
            return await AdicionarNacao(nacao, exercito);
        }

        public async Task<Batalha> IniciarAsync(IniciarBatalhaParams iniciarBatalhaParams)
        {
    
            if (NacoesIguais(iniciarBatalhaParams))
                throw new BatalhaServiceExeception("Os jogadores devem escolher nações diferentes!");
            
            var batalha = await BuscarPorJogador(iniciarBatalhaParams.Id, iniciarBatalhaParams.Jogador)
                ?? throw new BatalhaServiceExeception("Não foi possível encontrar a batalha");

            if (batalha.Tabuleiro is null)        
                IniciarTabuleiro(batalha);
            
            if (BatalhaNaoIniciada(batalha))
            {
                AtualizarNacoesDoTabuleiro(iniciarBatalhaParams, batalha);
                batalha.Tabuleiro.IniciarJogo(batalha.ExercitoBranco, batalha.ExercitoPreto);
                DefinirPrimeiroTurno(batalha);
                batalha.Estado = Batalha.EstadoBatalhaEnum.Iniciado;
            }

            await _context.SaveChangesAsync();
            return batalha;
        }

        public async Task<Batalha> JogarAsync(Movimento movimento, string jogador)
        {
            movimento.Elemento = _context.ElementosDoExercitos
                .Include(el => el.Exercito)
                .FirstOrDefault(el => el.Id == movimento.ElementoId)
                ?? throw new BatalhaServiceExeception("Elemento não encontrado");


            movimento.Batalha = BuscarBatalhaParaJogada(movimento);

            if (JogadorInvalido(movimento, jogador))
                throw new BatalhaServiceExeception("O jogador autenticado não é o autor da jogada");

            var batalha = movimento.Batalha;

            if (ExercitoNaoPertenceAoJogador(movimento))
                throw new BatalhaServiceExeception("O jogador não é dono do exercito");

            if (VezDoJogador(movimento, batalha))
            {
                var jogada = batalha.Jogar(movimento);

                if (JogadaInvalida(jogada))
                    throw new BatalhaServiceExeception("A jogada é invalida");

                PrepararProximoTurno(batalha);
                await _context.SaveChangesAsync();
                return batalha;
            }
            else
            {
                throw new BatalhaServiceExeception($"O movimento não foi efetuado. Não é a vez do jogador { jogador }");
            }
        }

        public async Task<Batalha> BuscarLobby(int batalhaId)
        {
            return await _context.Batalhas
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
                   .FirstOrDefaultAsync();
        }

        public async Task<Batalha> BuscarTabuleiroAsync(int batalhaId)
        {
            return await _context.Batalhas
                .Where(x => x.Id.Equals(batalhaId))
                .Include(b => b.ExercitoBranco)
                .Include(b => b.ExercitoBranco.Elementos)
                .Include(b => b.ExercitoBranco.ElementosVivos)
                .Include(b => b.ExercitoBranco.Usuario)
                .Include(b => b.ExercitoPreto)
                .Include(b => b.ExercitoPreto.Elementos)
                .Include(b => b.ExercitoPreto.ElementosVivos)
                .Include(b => b.ExercitoPreto.Usuario)
                .Include(b => b.Tabuleiro)
                .Include(b => b.Turno)
                .Include(b => b.Turno.Usuario)
                .Include(b => b.Vencedor)
                .Include(b => b.Vencedor.Usuario)
                .FirstOrDefaultAsync();
        }

        public async Task<Batalha> BuscarPorIdAsync(int id)
        {
            return await _context.Batalhas.FindAsync(id);
        }

        private void AdicionarJogadorABatalha(Usuario jogador, Batalha batalha)
        {
            var exercito = new Exercito
            {
                Usuario = jogador
            };

            if (ExercitoBrancoVazio(batalha))
            {
                batalha.ExercitoBranco = exercito;
            }
            else
            {
                batalha.ExercitoPreto = exercito;
            }
        }

        private bool ExercitoBrancoVazio(Batalha batalha)
            => batalha.ExercitoBrancoId == null;


        private Batalha BuscarBatalhasDisponiveis(string jogador)
        {
            return _context.Batalhas.Include(b => b.ExercitoBranco)
                            .Include(b => b.ExercitoPreto)
                            .FirstOrDefault(b =>
                        (b.ExercitoBrancoId == null
                        || b.ExercitoPretoId == null) &&
                        (b.ExercitoBranco.UsuarioId != jogador
                        && b.ExercitoPreto.UsuarioId != jogador));
        }

        private void PrepararProximoTurno(Batalha batalha)
        {
            batalha.Turno = null;
            batalha.TurnoId = batalha.TurnoId == batalha.ExercitoBrancoId ?
                batalha.ExercitoPretoId : batalha.ExercitoBrancoId;
        }

        private async Task RegistrarAtualizacao(Batalha batalha)
        {
            _context.Entry(batalha).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }
      
        private async Task<Exercito> AdicionarNacao(AbstractFactoryExercito.Nacao nacao, Exercito exercito)
        {
            exercito.Nacao = nacao;
            await _context.SaveChangesAsync();
            return exercito;
        }

        private static void IniciarTabuleiro(Batalha batalha)
        {
            batalha.Tabuleiro = new Tabuleiro
            {
                Altura = AlturaDoTabuleiro,
                Largura = LarguraDoTabuleiro
            };
        }

        private static void DefinirPrimeiroTurno(Batalha batalha)
        {
            batalha.Turno = new Random().Next(100) < 50
                    ? batalha.ExercitoPreto 
                    : batalha.ExercitoBranco;
        }
        
        private bool VezDoJogador(Movimento movimento, Batalha batalha)
            => movimento.AutorId == batalha.Turno.UsuarioId;

        private bool ExercitoNaoPertenceAoJogador(Movimento movimento)
            => movimento.AutorId != movimento.Elemento.Exercito.UsuarioId;


        private static bool JogadorInvalido(Movimento movimento, string jogador)
            => jogador != movimento.AutorId;


        private Batalha BuscarBatalhaParaJogada(Movimento movimento)
            => _context.Batalhas
                .Include(b => b.Tabuleiro)
                .Include(b => b.Tabuleiro.ElementosDoExercito)
                .Include(b => b.ExercitoBranco)
                .Include(b => b.ExercitoPreto)
                .Include(b => b.Turno)
                .Include(b => b.Vencedor)
                .Include(b => b.ExercitoBranco.Usuario)
                .Include(b => b.ExercitoPreto.Usuario)
                .FirstOrDefault(m => m.Id == movimento.BatalhaId);


        public async Task<Batalha> BuscarAsync(int id)
                => await _context.Batalhas.Include(b => b.ExercitoPreto)
                    .Include(b => b.ExercitoPreto.Usuario)
                    .Include(b => b.ExercitoBranco)
                    .Include(b => b.ExercitoBranco.Usuario)
                    .Include(b => b.Tabuleiro)
                    .Include(b => b.Tabuleiro.ElementosDoExercito)
                    .Include(b => b.Turno)
                    .Include(b => b.Turno.Usuario)
                    .FirstOrDefaultAsync(b => b.Id == id);

        private bool JogadaInvalida(bool jogada)
            => !jogada;

        private static void AtualizarNacoesDoTabuleiro(IniciarBatalhaParams iniciarBatalhaParams, Batalha batalha)
        {
            batalha.ExercitoBranco.Nacao = iniciarBatalhaParams.NacaoExercitoBranco;
            batalha.ExercitoPreto.Nacao = iniciarBatalhaParams.NacaoExercitoPreto;
        }

        private bool BatalhaNaoIniciada(Batalha batalha)
            => batalha.Estado == Batalha.EstadoBatalhaEnum.NaoIniciado;

        private bool NacoesIguais(IniciarBatalhaParams iniciarBatalhasParams)
            => iniciarBatalhasParams.NacaoExercitoBranco == iniciarBatalhasParams.NacaoExercitoPreto;

        private bool IdentificadoresDiferentes(int id, Batalha batalha)
            => id != batalha.Id;

    }
}
