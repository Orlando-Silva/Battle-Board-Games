using BattleBoardGame.Model.DAL;
using System;

namespace Service_Battle_Board_Games
{
    public abstract class Service : IDisposable
    {
        protected readonly ModelJogosDeGuerra _context;

        public Service(ModelJogosDeGuerra context)
        {
            _context = context;
        }

        public void Dispose()
            => _context.Dispose();
    }
}
