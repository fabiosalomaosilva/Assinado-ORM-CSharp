using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace ArqOrm.Model
{
    /// <summary>
    /// Atributo para verificação da chave estrangeira
    /// </summary>
    public class ForeignKeyAttribute : Attribute
    {
        public ForeignKeyAttribute(string nome)
        {
            this.Nome = nome;
        }
        public string Nome { get; set; }
    }

}