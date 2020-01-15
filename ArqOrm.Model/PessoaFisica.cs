using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Resources;

using System.Linq;
using System.Text;

namespace ArqOrm.Model
{
    public abstract class Base
    {       
        [Key]
        public int ID { get; set; }
    }

    public class PessoaFisica : Base
    {

        public string Nome { get; set; }
        public int? Idade { get; set; }
        public DateTime? DataCriacao { get; set; }
        public NaturezaJuridica NaturezaJuridica { get; set; }
        public IList<Telefone> Telefones { get; set; }
        public IList<Endereco> Enderecos { get; set; }
    }

    public class Telefone : Base
    {
        public int Numero { get; set; }
        public int DDD { get; set; }
        public int Ramal { get; set; }
        public Guid PessoaFisicaId { get; set; }
        [ForeignKey("PessoaFisicaId")]
        public PessoaFisica PesFisica { get; set; }
    }

    public class Endereco : Base
    {
        public string Rua { get; set; }
        public Guid PessoaFisicaId { get; set; }
        [ForeignKey("PessoaFisicaId")]
        public PessoaFisica PessoaFisica { get; set; }
    }

}
