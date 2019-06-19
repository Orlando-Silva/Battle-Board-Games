using BattleBoardGame.Model;
using System.Collections.Generic;
using System.Threading.Tasks;
using static BattleBoardGame.Model.Factory.AbstractFactoryExercito;

namespace Service_Battle_Board_Games.Interfaces
{
    public interface IBatalhaService
    {
        Task<int> BuscarQuantidadeAsync();
        Task<Batalha> BuscarAsync(int id);
        Task<Batalha> BuscarPorIdAsync(int id);
        Task<IEnumerable<Batalha>> BuscarAsync(bool Finalizada = false);
        Task<int> ContarPorJogador(string jogador);
        Task<Batalha> BuscarPorJogador(int id, string jogador);
        Task<Exercito> EscolherNacaoAsync(Nacao nacao, int ExercitoId);
        Task<Batalha> IniciarAsync(IniciarBatalhaParams iniciarBatalhaParams);
        Task<Batalha> JogarAsync(Movimento movimento, string jogador);
        Task<Batalha> AtualizarAsync(int id, Batalha batalha);
        Task<Batalha> EnviarAsync(Batalha batalha);
        Task<Batalha> CriarAsync(Usuario jogador);
        Task<Batalha> DeletarAsync(int id);
        Task<Batalha> BuscarLobby(int batalhaId);
        Task<Batalha> BuscarTabuleiroAsync(int batalhaId);
    }

    public class IniciarBatalhaParams
    {
        public int Id { get; set; }
        public string Jogador { get; set; }
        public Nacao NacaoExercitoBranco { get; set; }
        public Nacao NacaoExercitoPreto { get; set; }
    }
}
