using System;

namespace Service_Battle_Board_Games.Exceptions
{
    public class BatalhaServiceExeception : Exception
    {
        public BatalhaServiceExeception(string mensagem) 
            : base(mensagem) { }
    }
}
