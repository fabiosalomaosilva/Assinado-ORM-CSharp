using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace ArqOrm.Model
{
    public class TableAttribute : Attribute
    {
        public TableAttribute(string nome)
        {
            this.Nome = nome;
        }
        public string Nome { get; set; }
    }
}
