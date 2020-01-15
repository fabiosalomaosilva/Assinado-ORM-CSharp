using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Arquivarnet
{
    /// <summary>
    /// Atributo para operações de consulta SQL
    /// </summary>
    public class SqlOperadorTextoAttribute : Attribute
    {
        /// <summary>
        /// Operador de consulta SQL
        /// </summary>
        /// <param name="Operador">Representação do operador em linguagem SQL</param>
        public string Operador { get; set; }
    }
}
