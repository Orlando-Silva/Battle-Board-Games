using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleBoardGame.Model
{
    public class Guerreiro : ElementoDoExercito
    {
        public Guerreiro()
        {
            this.Saude = 150;
        }
        public override int AlcanceMovimento { get; protected set; } = 1;
        public override int AlcanceAtaque { get; protected set; } = 1;
        public override int Ataque { get; protected set; } = 45;
        public override string Imagem { get; protected set; } 
    }
}
