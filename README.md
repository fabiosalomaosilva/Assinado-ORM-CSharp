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
   
    public class Categoria
    {
        [Key]
        public int ID { get; set; }
        
        public string Nome { get; set; }      
        
        //InverseProperty é o nome da classe que fará o relacionamento
        [InverseProperty("Municipio")]
        public virtual ICollection<Parcela> Municipios { get; set; }
    }
        
    public class Produto
    {
        [Key]
        public int ID { get; set; }    
        
        public int CategoriaId { get; set; }
        [ForeignKey("CategoriaId")]
        public Categoria Categoria { get; set; }

        public string Nome { get; set; }      
        
        public string Descricao { get; set; } 
        
        public decimal Preco { get; set; } 
        
        public bool EmEstoque { get; set; } 
    }
    
Funções de CRUD

        public Produto SearchById(int id)
        {
            try
            {
                using (var db = new Session("connectionstring"))
                {
                    return db.PesquisarID<Produto>(id);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(String.Format("Ocorreu um erro: {0}", ex.Message));
            }

        }
        
        public Produto SearchByNome(string nomeProduto)
        {
            try
            {
                using (var db = new Session("connectionstring"))
                {
                    //Primeira forma
                    var criterio = new Criterio("Nome", nomeProduto)
                    return produto;
                    
                    //Segunda forma
                    var produto = db.PesquisarCriterio<Produto>(new Criterio("Nome", nomeProduto));
                    return produto;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(String.Format("Ocorreu um erro: {0}", ex.Message));
            }

        }
        
        public List<Produto> ListAll()
        {
            try
            {
                using (var db = new Session("connectionstring"))
                {
                    var lista =  db.ListarTudo<Produto>();
                    return lista;
                }

            }
            catch (Exception ex)
            {
                throw new Exception(String.Format("Ocorreu um erro: {0}", ex.Message));
            }
        }
    
        public List<Produto> SearchByCriterias(int categoriaId, bool emEstoque)
        {
            try
            {
                using (var db = new Session("connectionstring"))
                {
                   var param = new Parametros();
                    param.AddCriterio("CategoriaId", categoriaId);
                    param.AddCriterio("EmEstoque", emEstoque);
                    
                    var lista =  db.ListarPorCriterios<Produto>(param, SqlOperadorComparacao.AND);
                    return lista;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(String.Format("Ocorreu um erro: {0}", ex.Message));
            }
        }
        
        public void Insert(Produto obj)
        {
            try
            {
                using (var db = new Session("connectionstring"))
                {                    
                    obj.Nome = "iPhone";
                    obj.Descricao = "Aparelho celular marca Apple";
                    obj.CategoriaId = 1;
                    obj.Preco = 3500;
                    obj.EmEstoque = true;
                    db.Inserir(obj);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(String.Format("Ocorreu um erro: {0}", ex.Message));
            }
        }
        
        public void Edit(Produto obj)
        {
            try
            {
                using (var db = new Session("connectionstring"))
                {                    
                    obj.Nome = "iPhone 8";
                    obj.Preco = 3200;
                    db.Editar(obj);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(String.Format("Ocorreu um erro: {0}", ex.Message));
            }
        }
        
        public void Remove(int id)
        {
            try
            {
                using (var db = new Session("connectionstring"))
                {                    
                    var produto = db.PesquisarID<Produto>(id));
                    db.Editar(produto);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(String.Format("Ocorreu um erro: {0}", ex.Message));
            }
        }
