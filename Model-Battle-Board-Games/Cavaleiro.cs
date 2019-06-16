using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleBoardGame.Model
{
    public class Cavaleiro : ElementoDoExercito
    {
        public Cavaleiro()
        {
            this.Saude = 100;
        }
        public override int AlcanceMovimento { get; protected set; } = 3;
        public override int AlcanceAtaque { get; protected set; } = 1;
        public override int Ataque { get; protected set; } = 25;
        public override string Imagem { get; protected set; }
    }
}
