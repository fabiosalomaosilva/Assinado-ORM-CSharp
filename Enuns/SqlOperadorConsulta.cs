using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Arquivarnet
{
    public enum SqlOperadorConsulta
    {
        [SqlOperadorTexto(Operador = "=")]
        IGUAL,
        [SqlOperadorTexto(Operador = "LIKE")]
        LIKE,
        [SqlOperadorTexto(Operador = "NOT LIKE")]
        NOT_LIKE
    }
}