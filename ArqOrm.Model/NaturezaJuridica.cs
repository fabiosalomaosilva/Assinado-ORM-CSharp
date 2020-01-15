using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace ArqOrm.Model
{
    public enum NaturezaJuridica
    {
        [Display(Name="Pessoa física")]
        PessoaFisica,
        [Display(Name = "Pessoa jurídica")]
        PessoaJuridica
    }
}
