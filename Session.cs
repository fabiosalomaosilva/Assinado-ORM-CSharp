using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using Arquivarnet;
using System.Data.Common;

namespace Arquivarnet
{
    public partial class Session : IRepositorio, IDisposable
    {
        private string ConString = null;
        
        /// <summary>
        /// Inicializa a sessão do Arquivarnet ORM
        /// </summary>
        /// <param name="ConnectionString">Informar a ConnectionString de comunicação com o Banco de Dados'.</param>
        public Session(string ConnectionString)
	    {
            ConString = ConnectionString;
            ArquivarnetLinq.connectionString = ConString;
	    }

        /// <summary>
        /// Listar todos os parametros da tabela do Banco de Dados
        /// </summary>
        public List<T> ListarTudo<T>() where T : new() 
        {
            Type tipo = typeof(T);            
            List<T> lista = new List<T>();
            PropertyInfo[] propriedades = Helper.DefinirPropriedadesBasicas<T>();
            Hashtable hashtable = new Hashtable();         

            try
            {
                using (SqlConnection con = new SqlConnection(ConString))
                {
                    string SqlCmdString = GenSql.SelectAllSqlString<T>(); 
                    using (SqlCommand cmd = new SqlCommand(SqlCmdString, con))
                    {
                        cmd.CommandType = CommandType.Text;
                        con.Open();

                        using (SqlDataReader dr = cmd.ExecuteReader())
                        {
                            foreach (PropertyInfo info in propriedades)
                            {
                                hashtable[info.Name.ToUpper()] = info;
                            }
                            while (dr.Read())
                            {
                                T newObject = new T();
                                for (int index = 0; index < dr.FieldCount; index++)
                                {
                                    PropertyInfo info = (PropertyInfo)
                                                        hashtable[dr.GetName(index).ToUpper()];
                                    if ((info != null) && info.CanWrite)
                                    {
                                        if (info.PropertyType.BaseType.Name == "Enum")
                                        {
                                            var tEnum = Enum.Parse(info.PropertyType, dr.GetString(index));
                                            if (!dr.IsDBNull(index))
                                                info.SetValue(newObject, tEnum, null);
                                        }
                                        else
                                        {
                                            if (!dr.IsDBNull(index))
                                                info.SetValue(newObject, dr.GetValue(index), null);
                                        }
                                    }
                                }
                                lista.Add(newObject);                                
                            }
                            return lista;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("{0}\r\n{1}", ex.Message, ex.InnerException));
            }
        }

        /// <summary>
        /// Listar todos os parametros da tabela do Banco de Dados
        /// </summary>
        /// <param name="parametros">Informar os parametros ('Propriedades' das classes POCO) que irão receber os dados do Banco de Dados'.</param>
        public List<T> ListarPorCampo<T>(Parametros parametros) where T : new() 
        {
            if (parametros.Propriedades == null)
            {
                throw new Exception("Lista de Propriedades de 'parametros' está vazia.");
            }

            Type tipo = typeof(T);
            List<T> lista = new List<T>();
            PropertyInfo[] propriedades;
            if (parametros.Propriedades != null)
            {
                propriedades = Helper.DefinirPropriedadesCustomizadas<T>(parametros);
            }
            else
            {
                propriedades = Helper.DefinirPropriedadesBasicas<T>();
            }
            Hashtable hashtable = new Hashtable();         

            try
            {
                using (SqlConnection con = new SqlConnection(ConString))
                {
                    string SqlCmdString = GenSql.SelectCamposSqlString<T>(parametros);
                    using (SqlCommand cmd = new SqlCommand(SqlCmdString, con))
                    {
                        cmd.CommandType = CommandType.Text;
                        con.Open();

                        using (SqlDataReader dr = cmd.ExecuteReader())
                        {
                            foreach (PropertyInfo info in propriedades)
                            {
                                hashtable[info.Name.ToUpper()] = info;
                            }
                            while (dr.Read())
                            {
                                T newObject = new T();
                                for (int index = 0; index < dr.FieldCount; index++)
                                {
                                    PropertyInfo info = (PropertyInfo)
                                                        hashtable[dr.GetName(index).ToUpper()];
                                    if ((info != null) && info.CanWrite)
                                    {
                                        if (info.PropertyType.BaseType.Name == "Enum")
                                        {
                                            var tEnum = Enum.Parse(info.PropertyType, dr.GetString(index));
                                            if (!dr.IsDBNull(index))
                                                info.SetValue(newObject, tEnum, null);
                                        }
                                        else
                                        {
                                            if (!dr.IsDBNull(index))
                                                info.SetValue(newObject, dr.GetValue(index), null);
                                        }
                                    }
                                }
                                lista.Add(newObject);
                            }
                            return lista;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("{0}\r\n{1}", ex.Message, ex.InnerException));
            }
        }

        /// <summary>
        /// Listar todos os parametros ou determinados parametros ('Propriedades' das classes POCO) da tabela do Banco de Dados com Parâmetros de pesquisa ou filtro.
        /// </summary>
        /// <param name="parametros">Informar os parametros ('Propriedades' das classes POCO) que irão receber os dados do Banco de Dados, bem como a Lista de Critérios (Chave/Valor) para consulta.</param>
        /// <param name="operador">Parâmetro tipo Enum. Serve para informar ao ORM o critério de comparação da consulta.</param>
        public List<T> ListarPorCriterios<T>(Parametros parametros, SqlOperadorComparacao operador) where T : new()
        {
            if (parametros.Criterios == null && parametros.Propriedades == null)
            {
                throw new Exception("Campo 'parametros' está vazio.");
            }

            Type tipo = typeof(T);
            List<T> lista = new List<T>();
            PropertyInfo[] propriedades;
            if (parametros.Propriedades != null)
            {
                propriedades = Helper.DefinirPropriedadesCustomizadas<T>(parametros);
            }
            else
            {
                propriedades = Helper.DefinirPropriedadesBasicas<T>();
            }
            Hashtable hashtable = new Hashtable();

            try
            {
                using (SqlConnection con = new SqlConnection(ConString))
                {
                    string SqlCmdString = GenSql.SelectCriteriosSqlString<T>(parametros, operador);
                    using (SqlCommand cmd = new SqlCommand(SqlCmdString, con))
                    {
                        if (parametros.Criterios != null)
                        {
                            foreach (var i in parametros.Criterios)
                            {
                                var t = i.Valor.GetType();
                                cmd.Parameters.Add(new SqlParameter("@" + i.Chave, GenSql.DefinirTipoParametro(i.Valor.GetType()))).Value = i.Valor;

                            }
                        }
                        cmd.CommandType = CommandType.Text;
                        con.Open();

                        using (SqlDataReader dr = cmd.ExecuteReader())
                        {
                            foreach (PropertyInfo info in propriedades)
                            {
                                hashtable[info.Name.ToUpper()] = info;
                            }

                            while (dr.Read())
                            {
                                T newObject = new T();
                                for (int index = 0; index < dr.FieldCount; index++)
                                {
                                    PropertyInfo info = (PropertyInfo)hashtable[dr.GetName(index).ToUpper()];
                                    if ((info != null) && info.CanWrite)
                                    {
                                        if (info.PropertyType.BaseType.Name == "Enum")
                                        {
                                            var tEnum = Enum.Parse(info.PropertyType, dr.GetString(index));
                                            if (!dr.IsDBNull(index))
                                                info.SetValue(newObject, tEnum, null);
                                        }
                                        else
                                        {
                                            if (!dr.IsDBNull(index))
                                                info.SetValue(newObject, dr.GetValue(index), null);
                                        }
                                    }
                                }
                                lista.Add(newObject);
                            }
                            return lista;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("{0}\r\n{1}", ex.Message, ex.InnerException));
            }
        }


        /// <summary>
        /// Listar todos os parametros ou determinados parametros ('Propriedades' das classes POCO) da tabela do Banco de Dados com Parâmetros de pesquisa ou filtro.
        /// </summary>
        /// <param name="parametros">Informar os parametros ('Propriedades' das classes POCO) que irão receber os dados do Banco de Dados, bem como a Lista de Critérios (Chave/Valor) para consulta.</param>
        public List<T> PesquisarUmaData<T>(Parametros parametros) where T : new()
        {
            if (parametros.DataInicial == null)
            {
                throw new Exception("Campo de data está vazio.");
            }

            DateTime data1 = Convert.ToDateTime(String.Format("{0} 00:00:00", Convert.ToDateTime(parametros.DataInicial.Valor).ToShortDateString()));
            DateTime data2 = Convert.ToDateTime(String.Format("{0} 23:59:59", Convert.ToDateTime(parametros.DataInicial.Valor).ToShortDateString()));

            Type tipo = typeof(T);
            List<T> lista = new List<T>();
            PropertyInfo[] propriedades;
            if (parametros.Propriedades != null)
            {
                propriedades = Helper.DefinirPropriedadesCustomizadas<T>(parametros);
            }
            else
            {
                propriedades = Helper.DefinirPropriedadesBasicas<T>();
            }
            Hashtable hashtable = new Hashtable();

            try
            {
                using (SqlConnection con = new SqlConnection(ConString))
                {
                    string SqlCmdString = GenSql.SelectEntreDuasDatasSqlString<T>(parametros);
                    using (SqlCommand cmd = new SqlCommand(SqlCmdString, con))
                    {
                        cmd.Parameters.Add(new SqlParameter("@Data1", SqlDbType.DateTime)).Value = Convert.ToString(data1);
                        cmd.Parameters.Add(new SqlParameter("@Data2", SqlDbType.DateTime)).Value = Convert.ToString(data2);
                        if (parametros.Criterios != null)
                        {
                            foreach (var i in parametros.Criterios)
                            {
                                var t = i.Valor.GetType();
                                cmd.Parameters.Add(new SqlParameter("@" + i.Chave, GenSql.DefinirTipoParametro(i.Valor.GetType()))).Value = i.Valor;

                            }
                        }
                        cmd.CommandType = CommandType.Text;
                        con.Open();

                        using (SqlDataReader dr = cmd.ExecuteReader())
                        {
                            foreach (PropertyInfo info in propriedades)
                            {
                                hashtable[info.Name.ToUpper()] = info;
                            }

                            while (dr.Read())
                            {
                                T newObject = new T();
                                for (int index = 0; index < dr.FieldCount; index++)
                                {
                                    PropertyInfo info = (PropertyInfo)
                                                        hashtable[dr.GetName(index).ToUpper()];
                                    if ((info != null) && info.CanWrite)
                                    {
                                        if (info.PropertyType.BaseType.Name == "Enum")
                                        {
                                            var tEnum = Enum.Parse(info.PropertyType, dr.GetString(index));
                                            if (!dr.IsDBNull(index))
                                                info.SetValue(newObject, tEnum, null);
                                        }
                                        else
                                        {
                                            if (!dr.IsDBNull(index))
                                                info.SetValue(newObject, dr.GetValue(index), null);
                                        }
                                    }
                                }
                                lista.Add(newObject);
                            }
                            return lista;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("{0}\r\n{1}", ex.Message, ex.InnerException));
            }
        }

        /// <summary>
        /// Listar todos os parametros ou determinados parametros ('Propriedades' das classes POCO) da tabela do Banco de Dados com Parâmetros de pesquisa ou filtro.
        /// </summary>
        /// <param name="parametros">Informar os parametros ('Propriedades' das classes POCO) que irão receber os dados do Banco de Dados, bem como a Lista de Critérios (Chave/Valor) para consulta.</param>
        public List<T> PesquisarEntreDatas<T>(Parametros parametros) where T : new()
        {
            if (parametros.DataInicial == null && parametros.DataFinal == null)
            {
                throw new Exception("Campo de data está vazio.");
            }
            if (parametros.DataFinal == null)   
            {
                parametros.DataFinal = parametros.DataInicial;
            }

            DateTime data1 = Convert.ToDateTime(String.Format("{0} 00:00:00", Convert.ToDateTime(parametros.DataInicial.Valor).ToShortDateString()));
            DateTime data2 = Convert.ToDateTime(String.Format("{0} 23:59:59", Convert.ToDateTime(parametros.DataFinal.Valor).ToShortDateString()));

            Type tipo = typeof(T);
            List<T> lista = new List<T>();
            PropertyInfo[] propriedades;
            if (parametros.Propriedades != null)
            {
                propriedades = Helper.DefinirPropriedadesCustomizadas<T>(parametros);
            }
            else
            {
                propriedades = Helper.DefinirPropriedadesBasicas<T>();
            }
            Hashtable hashtable = new Hashtable();

            try
            {
                using (SqlConnection con = new SqlConnection(ConString))
                {
                    string SqlCmdString = GenSql.SelectEntreDuasDatasSqlString<T>(parametros);
                    using (SqlCommand cmd = new SqlCommand(SqlCmdString, con))
                    {
                        cmd.Parameters.Add(new SqlParameter("@Data1", SqlDbType.DateTime)).Value = Convert.ToString(data1);
                        cmd.Parameters.Add(new SqlParameter("@Data2", SqlDbType.DateTime)).Value = Convert.ToString(data2);
                        if (parametros.Criterios != null)
                        {
                            foreach (var i in parametros.Criterios)
                            {
                                var t = i.Valor.GetType();
                                cmd.Parameters.Add(new SqlParameter("@" + i.Chave, GenSql.DefinirTipoParametro(i.Valor.GetType()))).Value = i.Valor;

                            }
                        }
                        cmd.CommandType = CommandType.Text;
                        con.Open();

                        using (SqlDataReader dr = cmd.ExecuteReader())
                        {
                            foreach (PropertyInfo info in propriedades)
                            {
                                hashtable[info.Name.ToUpper()] = info;
                            }

                            while (dr.Read())
                            {
                                T newObject = new T();
                                for (int index = 0; index < dr.FieldCount; index++)
                                {
                                    PropertyInfo info = (PropertyInfo)
                                                        hashtable[dr.GetName(index).ToUpper()];
                                    if ((info != null) && info.CanWrite)
                                    {
                                        if (info.PropertyType.BaseType.Name == "Enum")
                                        {
                                            var tEnum = Enum.Parse(info.PropertyType, dr.GetString(index));
                                            if (!dr.IsDBNull(index))
                                                info.SetValue(newObject, tEnum, null);
                                        }
                                        else
                                        {
                                            if (!dr.IsDBNull(index))
                                                info.SetValue(newObject, dr.GetValue(index), null);
                                        }
                                    }
                                }
                                lista.Add(newObject);
                            }
                            return lista;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("{0}\r\n{1}", ex.Message, ex.InnerException));
            }
        }
                
        /**
        /// <summary>
        /// Listar todos os parametros ou determinados parametros ('Propriedades' das classes POCO) da tabela do Banco de Dados com Parâmetros de pesquisa ou filtro.
        /// </summary>
        /// <param name="parametros">Informar os parametros ('Propriedades' das classes POCO) que irão receber os dados do Banco de Dados, bem como a Lista de Critérios (Chave/Valor) para consulta.</param>
        /// <param name="operador">Parâmetro tipo Enum. Serve para informar ao ORM o critério de comparação da consulta.</param>
        public List<T> ListarPorCriteriosIncludes<T>(Parametros parametros) where T : new()
        {
            if (parametros.Criterios == null && parametros.Propriedades == null && parametros.Includes == null)
            {
                throw new Exception("Campo 'parametros' está vazio.");
            }

            Type tipo = typeof(T);
            List<T> lista = new List<T>();
            Hashtable hashtable = new Hashtable();
            Hashtable hashtable2 = new Hashtable();
            PropertyInfo[] propriedades;
            if (parametros.Propriedades == null)
            {
                propriedades = Helper.DefinirTodasPropriedades<T>();
            }
            else
            {
                propriedades = Helper.DefinirPropriedadesCustomizadas<T>(parametros);
            }

            try
            {
                using (SqlConnection con = new SqlConnection(ConString))
                {
                    string SqlCmdString = Helper.SelectCriteriosIncludesSqlString<T>(parametros);
                    using (SqlCommand cmd = new SqlCommand(SqlCmdString, con))
                    {
                        if (parametros.Criterios != null)
                        {
                            foreach (var i in parametros.Criterios)
                            {
                                var t = i.Valor.GetType();
                                cmd.Parameters.Add(new SqlParameter("@" + i.Chave, Helper.DefinirTipoParametro(i.Valor.GetType()))).Value = i.Valor;

                            }
                        }
                        cmd.CommandType = CommandType.Text;
                        con.Open();

                        using (SqlDataReader dr = cmd.ExecuteReader())
                        {
                            foreach (PropertyInfo info in propriedades)
                            {
                                hashtable[info.Name.ToUpper()] = info;
                            }

                            var propDerivadas = Helper.DefinirClases<T>();
                            foreach (PropertyInfo info in propDerivadas)
                            {
                                var prop1 = Helper.DefinirPropriedadesBasicas(info.PropertyType);
                                foreach (var item1 in prop1)
                                {
                                    hashtable2[item1.Name.ToUpper()] = item1;
                                }
                            }


                            while (dr.Read())
                            {
                                T newObject = new T();
                                for (int index = 0; index < dr.FieldCount; index++)
                                {
                                    PropertyInfo info = (PropertyInfo)
                                                        hashtable[dr.GetName(index).ToUpper()];
                                    if ((info != null) && info.CanWrite)
                                    {
                                        info.SetValue(newObject, dr.GetValue(index), null);
                                    }
                                }

                                for (int index = 0; index < dr.FieldCount; index++)
                                {
                                    PropertyInfo info = (PropertyInfo)
                                                        hashtable2[dr.GetName(index).ToUpper()];
                                    if ((info != null) && info.CanWrite)
                                    {
                                        info.SetValue(newObject, dr.GetValue(index), null);
                                    }
                                }


                                lista.Add(newObject);
                            }
                            return lista;

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("{0}\r\n{1}", ex.Message, ex.InnerException));
            }
        }
        **/

        /// <summary>
        /// Listar todos os parametros ou determinados parametros ('Propriedades' das classes POCO) da tabela do Banco de Dados com apenas um Parâmetro de pesquisa ou filtro.
        /// </summary>
        /// <param name="campo">Informar os parametros ('Propriedades' das classes POCO) que irão receber os dados do Banco de Dados, bem como o único Critério (Chave/Valor) para consulta.</param>
        public List<T> ListarPorUmCriterio<T>(Parametros campo) where T : new()
        {
            if (campo.Criterio == null)
            {
                throw new Exception("Campo 'Critério' está vazio.");
            }
            Type tipo = typeof(T);
            List<T> lista = new List<T>();
            PropertyInfo[] propriedades;
            if (campo.Propriedades != null)
            {
                propriedades = Helper.DefinirPropriedadesCustomizadas<T>(campo);
            }
            else
            {
                propriedades = Helper.DefinirPropriedadesBasicas<T>();
            }
            Hashtable hashtable = new Hashtable();

            try
            {
                using (SqlConnection con = new SqlConnection(ConString))
                {
                    string SqlCmdString = GenSql.SelectCriteriosSqlString<T>(campo, SqlOperadorComparacao.AND);
                    using (SqlCommand cmd = new SqlCommand(SqlCmdString, con))
                    {
                        cmd.Parameters.Add(new SqlParameter("@" + campo.Criterio.Chave, GenSql.DefinirTipoParametro(campo.Criterio.Valor.GetType()))).Value = campo.Criterio.Valor;

                        cmd.CommandType = CommandType.Text;
                        con.Open();

                        using (SqlDataReader dr = cmd.ExecuteReader())
                        {
                            foreach (PropertyInfo info in propriedades)
                            {
                                hashtable[info.Name.ToUpper()] = info;
                            }

                            while (dr.Read())
                            {
                                T newObject = new T();
                                for (int index = 0; index < dr.FieldCount; index++)
                                {
                                    PropertyInfo info = (PropertyInfo)
                                                        hashtable[dr.GetName(index).ToUpper()];
                                    if ((info != null) && info.CanWrite)
                                    {
                                        if (info.PropertyType.BaseType.Name == "Enum")
                                        {
                                            var tEnum = Enum.Parse(info.PropertyType, dr.GetString(index));
                                            if (!dr.IsDBNull(index))
                                                info.SetValue(newObject, tEnum, null);
                                        }
                                        else
                                        {
                                            if (!dr.IsDBNull(index))
                                                info.SetValue(newObject, dr.GetValue(index), null);
                                        }
                                    }
                                }
                                lista.Add(newObject);
                            }
                            return lista;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("{0}\r\n{1}", ex.Message, ex.InnerException));
            }
        }

        /// <summary>
        /// Pesquisar em tabela Sql Server com FullTextSearch habilitado.
        /// </summary>
        /// <param name="textoPesquisa">Texto a ser pesquisado.</param>
        public List<T> PesquisaFullText<T>(string textoPesquisa) where T : new()
        {
            if (string.IsNullOrEmpty(textoPesquisa))
            {
                throw new Exception("Campo 'textoPesquisa' está vazio.");
            }

            textoPesquisa = String.Format(@"""*{0}*""", textoPesquisa);
            textoPesquisa = textoPesquisa.Replace(" ", "*\" or \"*");

            Type tipo = typeof(T);
            List<T> lista = new List<T>();
            PropertyInfo[] propriedades = Helper.DefinirPropriedadesBasicas<T>();
            Hashtable hashtable = new Hashtable();

            try
            {
                using (SqlConnection con = new SqlConnection(ConString))
                {
                    string SqlCmdString = GenSql.SelectFullTextSqlString<T>();
                    using (SqlCommand cmd = new SqlCommand(SqlCmdString, con))
                    {
                        cmd.Parameters.Add(new SqlParameter("@Campo", SqlDbType.NVarChar)).Value = textoPesquisa;

                        cmd.CommandType = CommandType.Text;
                        con.Open();

                        using (SqlDataReader dr = cmd.ExecuteReader())
                        {
                            foreach (PropertyInfo info in propriedades)
                            {
                                hashtable[info.Name.ToUpper()] = info;
                            }

                            while (dr.Read())
                            {
                                T newObject = new T();
                                for (int index = 0; index < dr.FieldCount; index++)
                                {
                                    PropertyInfo info = (PropertyInfo)
                                                        hashtable[dr.GetName(index).ToUpper()];
                                    if ((info != null) && info.CanWrite)
                                    {
                                        if (info.PropertyType.BaseType.Name == "Enum")
                                        {
                                            var tEnum = Enum.Parse(info.PropertyType, dr.GetString(index));
                                            if (!dr.IsDBNull(index))
                                                info.SetValue(newObject, tEnum, null);
                                        }
                                        else
                                        {
                                            if (!dr.IsDBNull(index))
                                                info.SetValue(newObject, dr.GetValue(index), null);
                                        }
                                    }
                                }
                                lista.Add(newObject);
                            }
                            return lista;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("{0}\r\n{1}", ex.Message, ex.InnerException));
            }
        }

        /// <summary>
        /// Pesquisar em tabela Sql Server com FullTextSearch habilitado com utilização de critérios (filtros) de busca e opção de retorno de dados.
        /// </summary>
        /// <param name="textoPesquisa">Texto a ser pesquisado.</param>
        /// <example>ListarPorCriterios<T>(listaCriterios, Operador.LIKE)</example>
        public List<T> PesquisaFullTextCriterios<T>(string textoPesquisa, Parametros parametros) where T : new()
        {
            if (string.IsNullOrEmpty(textoPesquisa))
            {
                throw new Exception("Campo 'textoPesquisa' está vazio.");
            }

            textoPesquisa = String.Format(@"""*{0}*""", textoPesquisa);
            textoPesquisa = textoPesquisa.Replace(" ", "*\" or \"*");

            Type tipo = typeof(T);
            List<T> lista = new List<T>();
            PropertyInfo[] propriedades;
            if (parametros.Propriedades == null)
	        {
		        propriedades = Helper.DefinirPropriedadesBasicas<T>();
	        }
            else
            {
                propriedades = Helper.DefinirPropriedadesCustomizadas<T>(parametros);
            }
            Hashtable hashtable = new Hashtable();

            try
            {
                using (SqlConnection con = new SqlConnection(ConString))
                {
                    string SqlCmdString = GenSql.SelectFullTextCriteriosSqlString<T>(parametros);
                    using (SqlCommand cmd = new SqlCommand(SqlCmdString, con))
                    {
                        cmd.Parameters.Add(new SqlParameter("@Campo", SqlDbType.NVarChar)).Value = textoPesquisa;
                        if (parametros.Criterios != null)
                        {
                            foreach (var i in parametros.Criterios)
                            {
                                var t = i.Valor.GetType();
                                cmd.Parameters.Add(new SqlParameter("@" + i.Chave, GenSql.DefinirTipoParametro(i.Valor.GetType()))).Value = i.Valor;

                            }
                        }

                        cmd.CommandType = CommandType.Text;
                        con.Open();

                        using (SqlDataReader dr = cmd.ExecuteReader())
                        {
                            foreach (PropertyInfo info in propriedades)
                            {
                                hashtable[info.Name.ToUpper()] = info;
                            }

                            while (dr.Read())
                            {
                                T newObject = new T();
                                for (int index = 0; index < dr.FieldCount; index++)
                                {
                                    PropertyInfo info = (PropertyInfo)
                                                        hashtable[dr.GetName(index).ToUpper()];
                                    if ((info != null) && info.CanWrite)
                                    {
                                        if (info.PropertyType.BaseType.Name == "Enum")
                                        {
                                            var tEnum = Enum.Parse(info.PropertyType, dr.GetString(index));
                                            if (!dr.IsDBNull(index))
                                                info.SetValue(newObject, tEnum, null);
                                        }
                                        else
                                        {
                                            if (!dr.IsDBNull(index))
                                                info.SetValue(newObject, dr.GetValue(index), null);
                                        }
                                    }
                                }
                                lista.Add(newObject);
                            }
                            return lista;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("{0}\r\n{1}", ex.Message, ex.InnerException));
            }
        }

        /// <summary>
        /// Pesquisa o Identificador do objeto
        /// </summary>
        /// <param name="valor">ID do objeto a ser encontrado.</param>
        public T PesquisarID<T>(object valor)  where T : new() 
        {
            Type tipo = typeof(T);
            PropertyInfo[] propriedades = Helper.DefinirPropriedadesBasicas<T>();
            Hashtable hashtable = new Hashtable();
            T newObject = new T();
            PropertyInfo chave = Helper.RecuperarChavePrimaria<T>();
            chave.SetValue(newObject, valor, null);

            Parametros cam = new Parametros()
            {
                Criterio = new Criterio(chave.Name, chave.GetValue(newObject, null))
            };

            try
            {
                using (SqlConnection con = new SqlConnection(ConString))
                {
                    string SqlCmdString = GenSql.SelectAllSqlStringPorID<T>(cam);
                    using (SqlCommand cmd = new SqlCommand(SqlCmdString, con))
                    {
                        cmd.Parameters.Add(new SqlParameter("@" + chave.Name, GenSql.DefinirTipoParametro(chave.PropertyType))).Value = chave.GetValue(newObject, null);

                        cmd.CommandType = CommandType.Text;
                        con.Open();

                        using (SqlDataReader dr = cmd.ExecuteReader())
                        {
                            foreach (PropertyInfo info in propriedades)
                            {
                                hashtable[info.Name.ToUpper()] = info;
                            }
                            while (dr.Read())
                            {
                                for (int index = 0; index < dr.FieldCount; index++)
                                {
                                    PropertyInfo info = (PropertyInfo)
                                                        hashtable[dr.GetName(index).ToUpper()];
                                    if ((info != null) && info.CanWrite)
                                    {
                                        if (info.PropertyType.BaseType.Name == "Enum")
                                        {
                                            var tEnum = Enum.Parse(info.PropertyType, dr.GetString(index));
                                            if (!dr.IsDBNull(index))
                                                info.SetValue(newObject, tEnum, null);
                                        }
                                        else
                                        {
                                            if (!dr.IsDBNull(index))
                                                info.SetValue(newObject, dr.GetValue(index), null);
                                        }
                                    }
                                }
                            }
                            return newObject;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("{0}\r\n{1}", ex.Message, ex.InnerException));
            }
        }

        /// <summary>
        /// Retorna um objeto conforme critério informado
        /// </summary>
        /// <param name="criterio">Critério para pesquisa.</param>
        public T PesquisarCriterio<T>(Criterio criterio) where T : new()
        {
            Type tipo = typeof(T);
            PropertyInfo[] propriedades = Helper.DefinirPropriedadesBasicas<T>();
            Hashtable hashtable = new Hashtable();
            T newObject = new T();

            try
            {
                using (SqlConnection con = new SqlConnection(ConString))
                {
                    string SqlCmdString = GenSql.SelectAllSqlUmCriterioString<T>(criterio);
                    using (SqlCommand cmd = new SqlCommand(SqlCmdString, con))
                    {
                        cmd.Parameters.Add(new SqlParameter("@Criterio", GenSql.DefinirTipoParametro(criterio.Valor.GetType()))).Value = criterio.Valor;

                        cmd.CommandType = CommandType.Text;
                        con.Open();

                        using (SqlDataReader dr = cmd.ExecuteReader())
                        {
                            foreach (PropertyInfo info in propriedades)
                            {
                                hashtable[info.Name.ToUpper()] = info;
                            }
                            while (dr.Read())
                            {
                                for (int index = 0; index < dr.FieldCount; index++)
                                {
                                    PropertyInfo info = (PropertyInfo)
                                                        hashtable[dr.GetName(index).ToUpper()];
                                    if ((info != null) && info.CanWrite)
                                    {
                                        if (info.PropertyType.BaseType.Name == "Enum")
                                        {
                                            var tEnum = Enum.Parse(info.PropertyType, dr.GetString(index));
                                            if (!dr.IsDBNull(index))
                                                info.SetValue(newObject, tEnum, null);
                                        }
                                        else
                                        {
                                            if (!dr.IsDBNull(index))
                                                info.SetValue(newObject, dr.GetValue(index), null);
                                        }
                                    }
                                }  
                            }
                            return newObject;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("{0}\r\n{1}", ex.Message, ex.InnerException));
            }
        }
        
        /// <summary>
        /// Inserir objeto no Banco de Dados
        /// </summary>
        /// <param name="obj">Objeto a ser inserido.</param>
        public void Inserir<T>(T obj) where T : new()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(ConString))
                {
                    Type tipo = typeof(T);
                    var propriedades = Helper.DefinirPropriedadesBasicasInserir<T>();

                    string SqlCmdString = GenSql.InsertSqlString<T>();
                    using (SqlCommand cmd = new SqlCommand(SqlCmdString, con))
                    {
                        cmd.CommandType = CommandType.Text;
                        con.Open();

                        foreach (var i in propriedades)
                        {
                            if (i.PropertyType.FullName.Substring(0, 6) == "System")
                            {
                                var valor = i.GetValue(obj, null);
                                var dbType = GenSql.DefinirTipoParametro(i.PropertyType);

                                if (valor == null)
                                {
                                    cmd.Parameters.Add(new SqlParameter("@" + i.Name, dbType)).Value = DBNull.Value;
                                }
                                else if (valor.ToString() == string.Empty)
                                {
                                    cmd.Parameters.Add(new SqlParameter("@" + i.Name, dbType)).Value = DBNull.Value;
                                }
                                else if (valor.ToString() == "00000000-0000-0000-0000-000000000000")
                                {
                                    cmd.Parameters.Add(new SqlParameter("@" + i.Name, dbType)).Value = DBNull.Value;
                                }
                                else if (valor.ToString() == string.Empty)
                                {
                                    cmd.Parameters.Add(new SqlParameter("@" + i.Name, dbType)).Value = DBNull.Value;
                                }
                                else
                                {
                                    cmd.Parameters.Add(new SqlParameter("@" + i.Name, dbType)).Value = i.GetValue(obj, null);
                                }
                            }
                            else if (i.PropertyType.BaseType.FullName == "System.Enum")
                            {
                                if (i.GetValue(obj, null) != null)
                                {
                                    cmd.Parameters.Add(new SqlParameter("@" + i.Name, SqlDbType.NVarChar)).Value = i.GetValue(obj, null);
                                }
                                else
	                            {
                                    cmd.Parameters.Add(new SqlParameter("@" + i.Name, SqlDbType.NVarChar)).Value = DBNull.Value;
	                            }
                            }
                        }
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("{0}\r\n{1}", ex.Message, ex.InnerException));
            }
        }
        
        /// <summary>
        /// Inseri lista de objetos no Banco de Dados
        /// </summary>
        /// <param name="lista">Lista de objeto a ser inserida.</param>
        public void InserirLista<T>(List<T> lista) where T : new()
        {                
            SqlConnection con = new SqlConnection(ConString);
            con.Open();
            SqlTransaction transacao = con.BeginTransaction();
            T obj = new T();

            try
            {
                Type tipo = typeof(T);
                var propriedades = Helper.DefinirPropriedadesBasicasInserir<T>();

                
                foreach (var item in lista)
                {
                    string SqlCmdString = GenSql.InsertSqlString<T>();
                    using (SqlCommand cmd = new SqlCommand(SqlCmdString, con))
                    {
                        cmd.CommandType = CommandType.Text;
                        foreach (var i in propriedades)
                        {
                            if (i.PropertyType.FullName.Substring(0, 6) == "System")
                            {
                                var dbTipe = GenSql.DefinirTipoParametro(i.PropertyType);

                                if (i.GetValue(obj, null) == null)
                                {
                                    cmd.Parameters.Add(new SqlParameter("@" + i.Name, dbTipe)).Value = DBNull.Value;
                                }
                                else if (i.GetValue(obj, null).ToString() == string.Empty)
                                {
                                    cmd.Parameters.Add(new SqlParameter("@" + i.Name, dbTipe)).Value = DBNull.Value;
                                }
                                else if (i.GetValue(obj, null).ToString() == "00000000-0000-0000-0000-000000000000")
                                {
                                    cmd.Parameters.Add(new SqlParameter("@" + i.Name, dbTipe)).Value = DBNull.Value;
                                }
                                else if (i.GetValue(obj, null).ToString() == string.Empty)
                                {
                                    cmd.Parameters.Add(new SqlParameter("@" + i.Name, dbTipe)).Value = DBNull.Value;
                                }

                                else
                                {
                                    cmd.Parameters.Add(new SqlParameter("@" + i.Name, dbTipe)).Value = i.GetValue(obj, null);
                                }
                            }
                            else if (i.PropertyType.BaseType.FullName == "System.Enum")
                            {
                                if (i.GetValue(obj, null) != null)
                                {
                                    cmd.Parameters.Add(new SqlParameter("@" + i.Name, SqlDbType.NVarChar)).Value = i.GetValue(obj, null);
                                }
                                else
                                {
                                    cmd.Parameters.Add(new SqlParameter("@" + i.Name, SqlDbType.NVarChar)).Value = DBNull.Value;
                                }
                            }

                        }
                        cmd.Transaction = transacao;
                        cmd.ExecuteNonQuery();
                    }
                }
                transacao.Commit();
            }
            catch (Exception ex)
            {
                transacao.Rollback();
                throw new Exception(string.Format("{0}\r\n{1}", ex.Message, ex.InnerException));
            }
            finally
            {
                con.Close();
            }
        }	

        /// <summary>
        /// Editar lista de objetos no Banco de Dados
        /// </summary>
        /// <param name="lista">Lista de objeto a ser editada.</param>
        public void EditarLista<T>(List<T> lista) where T : new()
        {
            SqlConnection con = new SqlConnection(ConString);
            con.Open();
            SqlTransaction transacao = con.BeginTransaction();
            T obj = new T();

            try
            {
                Type tipo = typeof(T);
                var propriedades = Helper.DefinirPropriedadesBasicas<T>();
                var parametros = tipo.GetFields();
                PropertyInfo chave = Helper.RecuperarChavePrimaria<T>();
                foreach (var item in lista)
                {

                    Parametros cam = new Parametros()
                    {
                        Criterio = new Criterio(chave.Name, chave.GetValue(item, null))
                    };

                    if (chave == null)
                    {
                        throw new Exception("Objeto (Entity) não possui atributo \"Key\" definido.");
                    }
                    else
                    {
                        var val = chave.GetValue(item, null);
                        if (val == null)
                        {
                            throw new Exception("Chave primária (ID) não foi preenchido.");
                        }
                        else
                        {
                            string SqlCmdString = GenSql.UpdateSqlString<T>(cam);
                            using (SqlCommand cmd = new SqlCommand(SqlCmdString, con))
                            {

                                foreach (var i in propriedades)
                                {
                                    if (i.PropertyType.FullName.Substring(0, 6) == "System")
                                    {
                                        var dbTipe = GenSql.DefinirTipoParametro(i.PropertyType);

                                        if (i.GetValue(obj, null) == null)
                                        {
                                            cmd.Parameters.Add(new SqlParameter("@" + i.Name, dbTipe)).Value = DBNull.Value;
                                        }
                                        else if (i.GetValue(obj, null).ToString() == string.Empty)
                                        {
                                            cmd.Parameters.Add(new SqlParameter("@" + i.Name, dbTipe)).Value = DBNull.Value;
                                        }
                                        else if (i.GetValue(obj, null).ToString() == "00000000-0000-0000-0000-000000000000")
                                        {
                                            cmd.Parameters.Add(new SqlParameter("@" + i.Name, dbTipe)).Value = DBNull.Value;
                                        }
                                        else if (i.GetValue(obj, null).ToString() == string.Empty)
                                        {
                                            cmd.Parameters.Add(new SqlParameter("@" + i.Name, dbTipe)).Value = DBNull.Value;
                                        }

                                        else
                                        {
                                            cmd.Parameters.Add(new SqlParameter("@" + i.Name, dbTipe)).Value = i.GetValue(obj, null);
                                        }
                                    }
                                    else if (i.PropertyType.BaseType.FullName == "System.Enum")
                                    {
                                        if (i.GetValue(obj, null) != null)
                                        {
                                            cmd.Parameters.Add(new SqlParameter("@" + i.Name, SqlDbType.NVarChar)).Value = i.GetValue(obj, null);
                                        }
                                        else
	                                    {
                                            cmd.Parameters.Add(new SqlParameter("@" + i.Name, SqlDbType.NVarChar)).Value = DBNull.Value;
	                                    }
                                    }
                        
                                }

                                cmd.CommandType = CommandType.Text;
                                cmd.Transaction = transacao;
                                cmd.ExecuteNonQuery();
                            }
                        }
                    }
                }
                transacao.Commit();

            }
            catch (Exception ex)
            {
                transacao.Rollback();
                throw new Exception(string.Format("{0}\r\n{1}", ex.Message, ex.InnerException));
            }
            finally
            {
                con.Close();
            }
        }	        
        
        /// <summary>
        /// Editar objeto no Banco de Dados
        /// </summary>
        /// <param name="obj">Objeto a ser editado. Chave primária (KeyAttribute da classe POCO) deve estar atribuída no ID da Classe POCO.</param>
        public void Editar<T>(T obj) where T : new()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(ConString))
                {
                    Type tipo = typeof(T);
                    var propriedades = Helper.DefinirPropriedadesBasicas<T>();
                    var parametros = tipo.GetFields();
                    PropertyInfo chave = Helper.RecuperarChavePrimaria<T>();

                    Parametros cam = new Parametros()
                    {
                        Criterio = new Criterio(chave.Name, chave.GetValue(obj, null))
                    };

                    if (chave == null)
                    {
                        throw new Exception("Objeto (Entity) não possui atributo \"Key\" definido.");
                    }
                    else
                    {
                        var val = chave.GetValue(obj, null);
                        if (val == null)
                        {
                            throw new Exception("Chave primária (ID) não foi preenchido.");
                        }
                        else
                        {
                            string SqlCmdString = GenSql.UpdateSqlString<T>(cam);
                            using (SqlCommand cmd = new SqlCommand(SqlCmdString, con))
                            {
                                con.Open();
                                
                                foreach (var i in propriedades)
                                {
                                    if (i.PropertyType.FullName.Substring(0, 6) == "System")
                                    {
                                        var dbTipe = GenSql.DefinirTipoParametro(i.PropertyType);

                                        if (i.GetValue(obj, null) == null)
                                        {
                                            cmd.Parameters.Add(new SqlParameter("@" + i.Name, dbTipe)).Value = DBNull.Value;
                                        }
                                        else if (i.GetValue(obj, null).ToString() == string.Empty)
                                        {
                                            cmd.Parameters.Add(new SqlParameter("@" + i.Name, dbTipe)).Value = DBNull.Value;
                                        }
                                        else if (i.GetValue(obj, null).ToString() == "00000000-0000-0000-0000-000000000000")
                                        {
                                            cmd.Parameters.Add(new SqlParameter("@" + i.Name, dbTipe)).Value = DBNull.Value;
                                        }
                                        else if (i.GetValue(obj, null).ToString() == string.Empty)
                                        {
                                            cmd.Parameters.Add(new SqlParameter("@" + i.Name, dbTipe)).Value = DBNull.Value;
                                        }

                                        else
                                        {
                                            cmd.Parameters.Add(new SqlParameter("@" + i.Name, dbTipe)).Value = i.GetValue(obj, null);
                                        }
                                    }
                                    else if (i.PropertyType.BaseType.FullName == "System.Enum")
                                    {
                                        if (i.GetValue(obj, null) != null)
                                        {
                                            cmd.Parameters.Add(new SqlParameter("@" + i.Name, SqlDbType.NVarChar)).Value = i.GetValue(obj, null);
                                        }
                                        else
	                                    {
                                            cmd.Parameters.Add(new SqlParameter("@" + i.Name, SqlDbType.NVarChar)).Value = DBNull.Value;
	                                    }
                                    }
                                }                            

                                cmd.CommandType = CommandType.Text;
                                cmd.ExecuteNonQuery();
                            }
                        }
                    }           
                }
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("{0}\r\n{1}", ex.Message, ex.InnerException));
            }
        }

        /// <summary>
        /// Editar objeto no Banco de Dados por propriedades específicas
        /// </summary>
        /// <param name="obj">Objeto a ser editado. Chave primária (KeyAttribute da classe POCO) deve estar atribuída no ID da Classe POCO.</param>
        /// <param name="propriedades">Define a propriedade ou lista de propriedades a serem alteradas pelo UPDATE.</param>
        public void EditarPropriedadesUnitarias<T>(T obj, Parametros propriedades) where T : new()
        {
            try
            {
                if (propriedades == null)
                {
                    throw new Exception("Parâmetro CAMPO está vazio.");
                }
                
                using (SqlConnection con = new SqlConnection(ConString))
                {
                    Type tipo = typeof(T);
                    List<T> lista = new List<T>();
                    PropertyInfo[] listaPropriedades;
                    if (propriedades.Propriedades != null)
                    {
                        listaPropriedades = Helper.DefinirPropriedadesCustomizadas<T>(propriedades);
                    }
                    else
                    {
                        listaPropriedades = Helper.DefinirPropriedadesBasicas<T>();
                    }
                    Hashtable hashtable = new Hashtable();
                    PropertyInfo chave = Helper.RecuperarChavePrimaria<T>();


                    if (chave == null)
                    {
                        throw new Exception("Objeto (Entity) não possui atributo \"Key\" definido.");
                    }
                    else
                    {
                        var val = chave.GetValue(obj, null);
                        if (val == null)
                        {
                            throw new Exception("Chave primária (ID) não foi preenchido.");
                        }
                        else
                        {
                            string SqlCmdString = GenSql.UpdateComPropriedadesSqlString<T>(propriedades);
                            using (SqlCommand cmd = new SqlCommand(SqlCmdString, con))
                            {
                                con.Open();

                                foreach (var i in listaPropriedades)
                                {
                                    if (i.PropertyType.FullName.Substring(0, 6) == "System")
                                    {
                                        var dbTipe = GenSql.DefinirTipoParametro(i.PropertyType);

                                        if (i.GetValue(obj, null) == null)
                                        {
                                            cmd.Parameters.Add(new SqlParameter("@" + i.Name, dbTipe)).Value = DBNull.Value;
                                        }
                                        else if (i.GetValue(obj, null).ToString() == string.Empty)
                                        {
                                            cmd.Parameters.Add(new SqlParameter("@" + i.Name, dbTipe)).Value = DBNull.Value;
                                        }
                                        else if (i.GetValue(obj, null).ToString() == "00000000-0000-0000-0000-000000000000")
                                        {
                                            cmd.Parameters.Add(new SqlParameter("@" + i.Name, dbTipe)).Value = DBNull.Value;
                                        }
                                        else if (i.GetValue(obj, null).ToString() == string.Empty)
                                        {
                                            cmd.Parameters.Add(new SqlParameter("@" + i.Name, dbTipe)).Value = DBNull.Value;
                                        }

                                        else
                                        {
                                            cmd.Parameters.Add(new SqlParameter("@" + i.Name, dbTipe)).Value = i.GetValue(obj, null);
                                        }
                                    }
                                    else if (i.PropertyType.BaseType.FullName == "System.Enum")
                                    {
                                        if (i.GetValue(obj, null) != null)
                                        {
                                            cmd.Parameters.Add(new SqlParameter("@" + i.Name, SqlDbType.NVarChar)).Value = i.GetValue(obj, null);
                                        }
                                        else
                                        {
                                            cmd.Parameters.Add(new SqlParameter("@" + i.Name, SqlDbType.NVarChar)).Value = DBNull.Value;
                                        }
                                    }
                                }

                                cmd.CommandType = CommandType.Text;
                                cmd.ExecuteNonQuery();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("{0}\r\n{1}", ex.Message, ex.InnerException));
            }
        }

        /// <summary>
        /// Excluir objeto no Banco de Dados
        /// </summary>
        /// <param name="obj">Objeto a ser excluído. Chave primária (KeyAttribute da classe POCO) deve estar atribuída no ID da Classe POCO.</param>
        public void Excluir<T>(T obj) where T : new()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(ConString))
                {                    
                    Type tipo = typeof(T);
                    var propriedades = Helper.DefinirPropriedadesBasicas<T>();
                    var parametros = tipo.GetFields();
                    PropertyInfo chave = Helper.RecuperarChavePrimaria<T>();

                    Parametros cam = new Parametros() 
                    { 
                        Criterio = new Criterio(chave.Name, chave.GetValue(obj, null)) 
                    };


                    if (chave == null)
                    {
                        throw new Exception("Objeto (Entity) não possui atributo \"Key\" definido.");
                    }
                    else
                    {
                        var val = chave.GetValue(obj, null);
                        if (val == null)
                        {
                            throw new Exception("Chave primária (ID) não foi preenchido.");
                        }
                        else
                        {
                            string SqlCmdString = GenSql.DeleteSqlString<T>(cam);                   

                            using (SqlCommand cmd = new SqlCommand(SqlCmdString, con))
                            {
                                con.Open();
                                cmd.Parameters.Add(new SqlParameter("@" + chave.Name, GenSql.DefinirTipoParametro(chave.PropertyType))).Value = chave.GetValue(obj, null);
                                cmd.CommandType = CommandType.Text;
                                cmd.ExecuteNonQuery();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("{0}\r\n{1}", ex.Message, ex.InnerException));
            }
        }





        /// <summary>
        /// Retorna a contagem de registros
        /// </summary>
        public T Contador<T>() where T : new()
        {
            List<T> lista = new List<T>();
            try
            {
                return new T();
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("{0}\r\n{1}", ex.Message, ex.InnerException));
            }
        }
        internal int MContador<T>() where T : class
        {
            int contador;
            try
            {
                using (SqlConnection con = new SqlConnection(ConString))
                {
                    string SqlCmdString = GenSql.SelectCount<T>();
                    using (SqlCommand cmd = new SqlCommand(SqlCmdString, con))
                    {
                        cmd.CommandType = CommandType.Text;
                        con.Open();
                        contador = Convert.ToInt32(cmd.ExecuteScalar());
                        
                    }
                }
                return contador;
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("{0}\r\n{1}", ex.Message, ex.InnerException));
            }
        }
        public  int MContador<T>(Parametros parametros) where T : class
        {
            int contador;
            try
            {
                using (SqlConnection con = new SqlConnection(ConString))
                {
                    string SqlCmdString = GenSql.SelectCountPropriedadea<T>(parametros);
                    using (SqlCommand cmd = new SqlCommand(SqlCmdString, con))
                    {
                        foreach (var i in parametros.Criterios)
                        {
                            var t = i.Valor.GetType();
                            cmd.Parameters.Add(new SqlParameter("@" + i.Chave, GenSql.DefinirTipoParametro(i.Valor.GetType()))).Value = i.Valor;
                        }

                        cmd.CommandType = CommandType.Text;
                        con.Open();
                        contador = Convert.ToInt32(cmd.ExecuteScalar());

                    }
                }
                return contador;
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("{0}\r\n{1}", ex.Message, ex.InnerException));
            }
        }
        internal int Max<T>(string coluna)
        {
            int max;
            try
            {
                using (SqlConnection con = new SqlConnection(ConString))
                {
                    string SqlCmdString = GenSql.SelectMax<T>(coluna);
                    using (SqlCommand cmd = new SqlCommand(SqlCmdString, con))
                    {
                        cmd.CommandType = CommandType.Text;
                        con.Open();
                        max = Convert.ToInt32(cmd.ExecuteScalar());
                    }
                }
                return max;
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("{0}\r\n{1}", ex.Message, ex.InnerException));
            }
        }
        internal int Min<T>(string coluna)
        {
            int min;
            try
            {
                using (SqlConnection con = new SqlConnection(ConString))
                {
                    string SqlCmdString = GenSql.SelectMin<T>(coluna);
                    using (SqlCommand cmd = new SqlCommand(SqlCmdString, con))
                    {
                        cmd.CommandType = CommandType.Text;
                        con.Open();
                        min = Convert.ToInt32(cmd.ExecuteScalar());
                    }
                }
                return min;
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("{0}\r\n{1}", ex.Message, ex.InnerException));
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
            }
        }
        ~Session()
        {
            Dispose(false);
        }


        /**
         *        /// <summary>
        /// Listar todos os parametros ou determinados parametros ('Propriedades' das classes POCO) da tabela do Banco de Dados com Parâmetros de pesquisa ou filtro.
        /// </summary>
        /// <param name="parametros">Informar os parametros ('Propriedades' das classes POCO) que irão receber os dados do Banco de Dados, bem como a Lista de Critérios (Chave/Valor) para consulta.</param>
        /// <param name="operador">Parâmetro tipo Enum. Serve para informar ao ORM o critério de comparação da consulta.</param>
        public List<T> ListarPorCriteriosJoin<T>(Parametros parametros, SqlOperadorComparacao operador) where T : new()
        {
            if (parametros.Criterios == null && parametros.Propriedades == null && parametros.Joins == null)
            {
                throw new Exception("Campo 'parametros' está vazio.");
            }

            Type tipo = typeof(T);
            List<T> lista = new List<T>();
            PropertyInfo[] propriedades;
            if (parametros.Propriedades != null)
            {
                propriedades = Helper.DefinirPropriedadesCustomizadas<T>(parametros);
            }
            else
            {
                propriedades = Helper.DefinirPropriedadesBasicas<T>();
            }
            Hashtable hashtable = new Hashtable();

            try
            {
                using (SqlConnection con = new SqlConnection(ConString))
                {
                    string SqlCmdString = GenSql.SelectCriteriosComJoins_SqlString<T>(parametros);
                    using (SqlCommand cmd = new SqlCommand(SqlCmdString, con))
                    {
                        if (parametros.Criterios != null)
                        {
                            foreach (var i in parametros.Criterios)
                            {
                                var t = i.Valor.GetType();
                                cmd.Parameters.Add(new SqlParameter("@" + i.Chave, GenSql.DefinirTipoParametro(i.Valor.GetType()))).Value = i.Valor;

                            }
                        }
                        cmd.CommandType = CommandType.Text;
                        con.Open();

                        using (SqlDataReader dr = cmd.ExecuteReader())
                        {
                            foreach (PropertyInfo info in propriedades)
                            {
                                hashtable[info.Name.ToUpper()] = info;
                            }

                            while (dr.Read())
                            {
                                T newObject = new T();
                                for (int index = 0; index < dr.FieldCount; index++)
                                {
                                    PropertyInfo info = (PropertyInfo)
                                                        hashtable[dr.GetName(index).ToUpper()];
                                    if ((info != null) && info.CanWrite)
                                    {
                                        if (!dr.IsDBNull(index))
                                            info.SetValue(newObject, dr.GetValue(index), null);
                                    }
                                }
                                lista.Add(newObject);
                            }
                            return lista;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("{0}\r\n{1}", ex.Message, ex.InnerException));
            }
        }

         **/


        internal T Primeiro<T>(string propriedade, string valor) where T : new()
        {
            Type tipo = typeof(T);
            PropertyInfo[] propriedades = Helper.DefinirPropriedadesBasicas<T>();
            Hashtable hashtable = new Hashtable();
            T newObject = new T();
            PropertyInfo chave = Helper.RecuperarChavePrimaria<T>();
            chave.SetValue(newObject, valor, null);

            try
            {
                using (SqlConnection con = new SqlConnection(ConString))
                {
                    string SqlCmdString = GenSql.SelectFirst<T>(propriedade, valor);
                    using (SqlCommand cmd = new SqlCommand(SqlCmdString, con))
                    {
                        cmd.Parameters.Add(new SqlParameter("@" + chave.Name, GenSql.DefinirTipoParametro(chave.PropertyType))).Value = chave.GetValue(newObject, null);

                        cmd.CommandType = CommandType.Text;
                        con.Open();

                        using (SqlDataReader dr = cmd.ExecuteReader())
                        {
                            foreach (PropertyInfo info in propriedades)
                            {
                                hashtable[info.Name.ToUpper()] = info;
                            }
                            while (dr.Read())
                            {
                                for (int index = 0; index < dr.FieldCount; index++)
                                {
                                    PropertyInfo info = (PropertyInfo)
                                                        hashtable[dr.GetName(index).ToUpper()];
                                    if ((info != null) && info.CanWrite)
                                    {
                                        if (!dr.IsDBNull(index))
                                            info.SetValue(newObject, dr.GetValue(index), null);
                                    }
                                }
                            }
                            return newObject;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("{0}\r\n{1}", ex.Message, ex.InnerException));
            }
        }
    }
}