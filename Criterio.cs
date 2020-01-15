using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Arquivarnet
{
    public class Criterio
    {
        public Criterio(string chave, object valor)
        {
            this.Chave = chave;
            this.Valor = valor;
        }
        public string Chave { get; set; }
        public object Valor { get; set; }

    }
}
