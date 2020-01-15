# Assinado ORM
Projeto de Mapeador de Objeto Relacional para aplicativos simples.
Possui suporte a POCO e DataAnnotations para definição da chave primária através da KeyAttribute.
Suporte apenas para Bancos de Dados Sql Server.
Novo suporte para Sql Server Full Text search incluso.
Nova atualização adicionou suporte para consultas com Expression Lambda "Incluir" para adicionar objetos associados.

# Observações:
- Ainda não há suporte para geração do Banco de Dados. 
- As tabelas e campos do Banco de Dados devem possuir os mesmos nomes das classes POCO e de suas propriedades.
- Não há necessidade de criar arquivos de mapeamento no aplicativo.
- Classes POCO devem ser decoradas com Atributos "Key" para indicação da Primary Key e de atributo "InverseProperty" na classe associada para indicar no relacionamento, a sua Foreign key.
- O objeto "Parametros" possui as propriedades necessárias para se realizarem consultas com parametros(Filtros) e retorno de campos(Propriedades).

# Características
1. ORM
2. Suporte a POCO
3. Suporte a SQL Full Text Search
4. Não necessita de arquivos de mapeamento
5. Suporte a DataAnnotations

# Intalação da biblioteca via Nuget Package

    Install-Package Arquivarnet-ORM -Version 1.54.1

# Exemplo

Classes POCO
   
    public class Estado
    {
        [Key]
        public int ID { get; set; }
        
        [MaxLength(50)]
        public string Estado { get; set; }
        
        public string UF { get; set; }
        
        //InverseProperty é o nome da classe que fará o relacionamento
        [InverseProperty("Municipio")]
        public virtual ICollection<Parcela> Municipios { get; set; }
    }
        
    public class Municipio
    {
        [Key]
        public int ID { get; set; }    
        
        public int EstadoId { get; set; }
        [ForeignKey("EstadoId")]
        public Estado Estado { get; set; }

        [MaxLength(60)]
        public string Municipio { get; set; }       
    }
    
Funções de CRUD

    public List<Perfil> ListarTodos()
        {
            try
            {
                using (var db = new Session(Utils.ConString))
                {
                    var param = new Parametros();
                    param.AddCriterio("Ativo", true);
                    param.AddCriterio("IdEmpresa", LoginRep.IdEmpresa);
                    param.AddPropriedades("NomePerfil");

                    return db.ListarPorCriterios<Perfil>(param, SqlOperadorComparacao.AND);
                }

            }
            catch (Exception ex)
            {
                throw new Exception(String.Format("Ocorreu um erro: {0}", ex.Message));
            }
        }
    
        
