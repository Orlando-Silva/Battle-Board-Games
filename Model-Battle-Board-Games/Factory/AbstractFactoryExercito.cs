﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleBoardGame.Model.Factory
{
    public abstract class AbstractFactoryExercito
    {
        public abstract Arqueiro CriarArqueiro();

        public abstract Cavaleiro CriarCavalaria();

        public abstract Guerreiro CriarGuerreiro();

        public enum Nacao : int
        {
            India = 1,
            Persia = 2,
            Egito = 3,
            Grego = 4
        };

        /// <summary>
        /// Este método é uma factory para a Abstract Factory.
        /// Deste modo, não existirá dependência do sistema com as 
        /// Factories concretas.
        /// </summary>
        /// <param name="nacionalidade"></param>
        /// <returns></returns>
        public static AbstractFactoryExercito CriarFactoryExercito(Nacao nacionalidade)
        {
            AbstractFactoryExercito factory = null;
            if (nacionalidade == Nacao.Persia)
            {
                factory = new FactoryExercitoPersa();
            }
            else if (Nacao.Egito == nacionalidade)
            {
                factory = new FactoryExercitoEgipicio();
            }
            else if (Nacao.India == nacionalidade)
            {
                factory = new FactoryExercitoIndiano();
            }
            else if (Nacao.Grego == nacionalidade)
            {
                factory = new FactoryExercitoGrego();
            }
            return factory;
        }
    }
}
