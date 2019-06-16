namespace BattleBoardGame.Model.Factory
{
    class FactoryExercitoGrego : AbstractFactoryExercito
    {
        public override Arqueiro CriarArqueiro()
        {
            return new ArqueiroGrego();
        }

        public override Cavaleiro CriarCavalaria()
        {
            return new CavaleiroGrego();
        }

        public override Guerreiro CriarGuerreiro()
        {
            return new GuerreiroGrego();
        }
    }
}
