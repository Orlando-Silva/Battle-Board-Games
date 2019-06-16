using static BattleBoardGame.Model.Factory.AbstractFactoryExercito;

namespace BattleBoardGames.DTO
{
    public class IniciarBatalhaRequest
    {
        public Nacao NacaoExercitoBranco { get; set; }
        public Nacao NacaoExercitoPreto { get; set; }
    }
}
