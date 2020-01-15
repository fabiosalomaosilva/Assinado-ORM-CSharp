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

namespace Arquivarnet
{
    internal static class GenSql
    {
        internal static SqlDbType DefinirTipoParametro(Type t)
        {
            SqlParameter param = new SqlParameter();
            TypeConverter tc = TypeDescriptor.GetConverter(param.DbType);

            if (tc.CanConvertFrom(t))
            {
                param.DbType = (DbType)tc.ConvertFrom(t.Name);
            }
            else
            {
                string nome = t.Name;
                // tentar forçar a conversão
                try
                {
                    if (t.Name.Contains("Guid"))   
                    {
                        param.DbType = (DbType)tc.ConvertFrom("Guid");
                    }
                    else if (t.Name.Contains("Boolean"))
                    {
                        param.DbType = (DbType)tc.ConvertFrom("Boolean");
                    }
                    else if (t.Name.Contains("Bool"))
                    {
                        param.DbType = (DbType)tc.ConvertFrom("Bool");
                    }
                    else if (t.Name.Contains("DateTime"))
                    {
                        param.SqlDbType = SqlDbType.DateTime2;
                    }
                    else if (t.Name.Contains("string"))
                    {
                        param.DbType = (DbType)tc.ConvertFrom("string");
                    }
                    else if (t.Name.Contains("int"))
                    {
                        param.DbType = (DbType)tc.ConvertFrom("int");
                    }
                    else if (t.Name.Contains("long"))
                    {
                        param.DbType = (DbType)tc.ConvertFrom("long");
                    }
                    else if (t.Name.Contains("int64"))
                    {
                        param.DbType = (DbType)tc.ConvertFrom("int64");
                    }
                    else if (nome == "Byte[]")
                    {
                        param.SqlDbType = SqlDbType.VarBinary;
                    }
                    else if (t.FullName == "System.Nullable`1[[System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]")
                    {
                        param.SqlDbType = SqlDbType.Int;
                    }
                    else if (t.FullName == "System.Nullable`1[[System.Int16, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]")
                    {
                        param.SqlDbType = SqlDbType.SmallInt;
                    }
                    else if (t.FullName == "System.Nullable`1[[System.Int64, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]")
                    {
                        param.SqlDbType = SqlDbType.BigInt;
                    }
                    else if (t.FullName == "System.Nullable`1[[System.DateTime, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]")
                    {
                        param.SqlDbType = SqlDbType.DateTime2;
                    }
                    else if (t.FullName == "System.Nullable`1[[System.Guid, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]")
                    {
                        param.SqlDbType = SqlDbType.UniqueIdentifier;
                    }
                    else if (t.FullName == "System.Nullable`1[[System.Boolean, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]")
                    {
                        param.SqlDbType = SqlDbType.Bit;
                    }
                    else if (t.FullName == "System.Nullable`1[[System.Bool, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]")
                    {
                        param.SqlDbType = SqlDbType.Bit;
                    }
                    else if (t.FullName == "System.Nullable`1[[System.Decimal, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]")
                    {
                        param.SqlDbType = SqlDbType.Decimal;
                    }
                    else
                    {
                        param.DbType = (DbType)tc.ConvertFrom(t.Name);
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            }
            return param.SqlDbType;
        }

        internal static string InsertSqlString<T>() where T : new()
        {
            Type tipo = typeof(T);
            var propriedades = Helper.DefinirPropriedadesBasicasInserir<T>();

            int tam = propriedades.Length;
            int index = 0;

            StringBuilder s = new StringBuilder();
            s.Append(" INSERT INTO  ");
            s.Append(tipo.Name);
            s.Append(" (");

            for (int i = 0; i < tam; i++)
            {
                if (propriedades[i].PropertyType.FullName.Substring(0, 6) == "System")
                {
                    if (i < (tam - 1))
                    {
                        s.Append(String.Format(" {0}, ", propriedades[i].Name));
                    }
                    else
                    {
                        s.Append(String.Format(" {0} ", propriedades[i].Name));
                    }
                }
                else if (propriedades[i].PropertyType.BaseType.Name == "Enum")
                {
                    if (i < (tam - 1))
                    {
                        s.Append(String.Format(" {0}, ", propriedades[i].Name));
                    }
                    else
                    {
                        s.Append(String.Format(" {0} ", propriedades[i].Name));
                    }
                }
            }
    
            s.Append(" ) ");
            s.Append(" VALUES (");
            index = 0;

            for (int i = 0; i < tam; i++)
            {
                if (propriedades[i].PropertyType.FullName.Substring(0, 6) == "System")
                {
                    if (i < (tam - 1))
                    {
                        s.Append(String.Format(" @{0}, ", propriedades[i].Name));
                    }
                    else
                    {
                        s.Append(String.Format(" @{0} ", propriedades[i].Name));
                    }
                }
                else if (propriedades[i].PropertyType.BaseType.Name == "Enum")
                {
                    if (i < (tam - 1))
                    {
                        s.Append(String.Format(" @{0}, ", propriedades[i].Name));
                    }
                    else
                    {
                        s.Append(String.Format(" @{0} ", propriedades[i].Name));
                    }
                }
            }

            s.Append(" ) ");

            return s.ToString();
        }

        internal static string UpdateSqlString<T>(Parametros campos) where T : new()
        {
            Type tipo = typeof(T);
            var listaPropriedades = Helper.DefinirPropriedadesBasicas<T>();

            int tam = listaPropriedades.Length - 1;
            int index = 0;

            StringBuilder s = new StringBuilder();
            s.Append("UPDATE ");
            s.Append(tipo.Name);
            s.Append(" SET ");

            for (int i = 0; i < tam; i++)
            {
                if (listaPropriedades[i].PropertyType.FullName.Substring(0, 6) == "System")
                {
                    if (i < (tam - 1))
                    {
                        if (listaPropriedades[i].Name == campos.Criterio.Chave)
                        {
                            s.Append("");
                        }
                        else
                        {
                            s.Append(String.Format(" {0} = @{0},", listaPropriedades[i].Name));
                            index = 1;
                        }
                    }
                    else
                    {
                        if (listaPropriedades[i].Name == campos.Criterio.Chave)
                        {
                            s.Append("");
                        }
                        else
                        {
                            s.Append(String.Format(" {0} = @{0}", listaPropriedades[i].Name));
                            index = 1;
                        }
                    }
                }
                else if (listaPropriedades[i].PropertyType.BaseType.Name == "Enum")
                {
                    if (i < (tam - 1))
                    {
                        s.Append(String.Format(" {0} = @{0},", listaPropriedades[i].Name));
                    }
                    else
                    {
                        s.Append(String.Format(" {0} = @{0}", listaPropriedades[i].Name));
                    }
                }
            }
            s.Append(" WHERE ");
            s.Append(" (");
            s.Append(campos.Criterio.Chave);
            s.Append(" = @");
            s.Append(campos.Criterio.Chave);
            s.Append(")");

            return s.ToString();
        }

        internal static string DeleteSqlString<T>(Parametros campos) where T : new()
        {
            Type tipo = typeof(T);

            StringBuilder s = new StringBuilder();
            s.Append(" DELETE FROM ");
            s.Append(tipo.Name);
            s.Append(" WHERE ");
            s.Append(campos.Criterio.Chave);
            s.Append(" = @");
            s.Append(campos.Criterio.Chave);

            return s.ToString();
        }

        internal static string SelectAllSqlString<T>()
        {
            Type tipo = typeof(T);

            StringBuilder s = new StringBuilder();
            s.Append(" SELECT * FROM ");
            s.Append(tipo.Name);

            return s.ToString();
        }

        internal static string SelectAllSqlUmCriterioString<T>(Criterio criterio)
        {
            Type tipo = typeof(T);

            StringBuilder s = new StringBuilder();
            s.Append(" SELECT * FROM ");
            s.Append(tipo.Name);
            s.Append(" WHERE ");
            s.Append(criterio.Chave);
            s.Append(" = @Criterio");
            return s.ToString();
        }

        /**
        internal static string SelectCriteriosComJoins_SqlString<T>(Parametros campos) where T : new()
        {
            Type tipo = typeof(T);
            int tamIncludes = campos.Joins.Count;
            int indexIncludes = 0;


            StringBuilder s = new StringBuilder();
            s.Append(" SELECT  ");
            var chave = Helper.RecuperarChavePrimaria<T>();

            if (campos.Propriedades != null)
            {
                int tam = campos.Propriedades.Count;
                int index = 0;

                s.Append(String.Format("{0}.{1} as {0}_{1}", tipo.Name, chave.Name));
                s.Append(", ");

                foreach (var i in campos.Propriedades)
                {
                    s.Append(String.Format(" {0}.{1} as {0}_{1}, ", tipo.Name, i));
                }
            }
            else
            {
                var tipo1 = Helper.DefinirPropriedadesBasicas<T>();

                foreach (var i in tipo1)
                {
                    s.Append(String.Format(" {0}.{1} as {0}_{1}, ", tipo.Name, i.Name));
                }

            }

            foreach (var i in campos.Joins)
            {
                Type tipo1 = Helper.RecuperarClassePorNome<T>(i);

                var ii = tipo1.DeclaringType;
                var tt = ii.GetType();
                var props = Helper.DefinirPropriedadesBasicas(tipo1);
                int tam = props.Length;
                int index = 0;

                foreach (var item in props)
                {
                    if (tam == 1)
                    {
                        s.Append(String.Format(" {0}.{1} as {0}_{1} ", tipo1.Name, item.Name));
                        if (indexIncludes == 0)
                        {
                            s.Append(", ");
                            indexIncludes++;
                        }
                        else
                        {
                            if (indexIncludes == tamIncludes - 1)
                            {
                                s.Append("");
                            }
                            else
                            {
                                s.Append(", ");
                                indexIncludes++;
                            }
                        }
                    }
                    else
                    {
                        if (index == 0)
                        {
                            s.Append(String.Format(" {0}.{1} as {0}_{1}, ", tipo1.Name, item.Name));
                            index = 1;
                        }
                        else
                        {
                            if (index == tam - 1)
                            {
                                s.Append(String.Format(" {0}.{1} as {0}_{1} ", tipo1.Name, item.Name));
                                if (indexIncludes == 0)
                                {
                                    s.Append(", ");
                                    indexIncludes++;
                                }
                                else
                                {
                                    if (indexIncludes == tamIncludes - 1)
                                    {
                                        s.Append("");
                                    }
                                    else
                                    {
                                        s.Append(", ");
                                        indexIncludes++;
                                    }
                                }
                            }
                            else
                            {
                                s.Append(String.Format(" {0}.{1} as {0}_{1}, ", tipo1.Name, item.Name));
                                index = index + 1;
                            }
                        }
                    }

                }

            }

            s.Append(" FROM ");
            s.Append(tipo.Name);

            foreach (var i in campos.Joins)
            {
                s.Append(" INNER JOIN ");
                s.Append(i);
                s.Append(" ON ");
                s.Append(tipo.Name);
                s.Append(".");
                s.Append(chave.Name);
                s.Append(" = ");
                s.Append(i);
                s.Append(".");
                Type tipo1 = Helper.RecuperarClassePorNome<T>(i);
                s.Append(Helper.RecuperarChaveEstrangeira(tipo1).Name);
            }



            if (campos.Criterios != null)
            {
                int tam = campos.Criterios.Count;
                int index = 0;

                foreach (var i in campos.Criterios)
                {
                    if (tam == 1)
                    {
                        s.Append(String.Format(" WHERE {0} = @{1}", i.Chave, i.Chave));
                    }
                    else
                    {
                        if (index == 0)
                        {
                            s.Append(String.Format(" WHERE {0} = @{1} {2}", i.Chave, i.Chave, "AND"));
                            index = 1;
                        }
                        else
                        {
                            if (index == tam - 1)
                            {
                                s.Append(String.Format(" {0} = @{1}", i.Chave, i.Chave));
                            }
                            else
                            {
                                s.Append(String.Format(" {0} = @{1} {2}", i.Chave, i.Chave, "AND"));
                                index = index + 1;
                            }
                        }
                    }
                }
            }
            return s.ToString();
        }
        **/


        internal static string SelectAllSqlStringPorID<T>(Parametros campos)
        {
            Type tipo = typeof(T);
            var chave = Helper.RecuperarChavePrimaria<T>();

            StringBuilder s = new StringBuilder();
            s.Append(" SELECT  ");

            if (campos.Propriedades != null)
            {
                s.Append(chave.Name);
                s.Append(", ");

                for (int i = 0; i < campos.Propriedades.Count - 1; i++)
                {
                    s.Append(campos.Propriedades[i].ToString());
                    s.Append(", ");
                }
                s.Append(campos.Propriedades[campos.Propriedades.Count - 1].ToString());

            }
            else
            {
                s.Append(" * ");
            }
            s.Append(" FROM ");
            s.Append(tipo.Name);
            s.Append(" WHERE ");
            s.Append(campos.Criterio.Chave);
            s.Append(" = @");
            s.Append(campos.Criterio.Chave);

            return s.ToString();
        }

        internal static string SelectCamposSqlString<T>(Parametros campos)
        {
            Type tipo = typeof(T);
            var chave = Helper.RecuperarChavePrimaria<T>();

            StringBuilder s = new StringBuilder();
            s.Append(" SELECT  ");
            if (campos.Propriedades != null)
            {
                s.Append(chave.Name);
                s.Append(", ");

                for (int i = 0; i < campos.Propriedades.Count - 1; i++)
                {
                    s.Append(campos.Propriedades[i].ToString());
                    s.Append(", ");
                }
                s.Append(campos.Propriedades[campos.Propriedades.Count - 1].ToString());

            }
            else
            {
                s.Append(" * ");
            }
            s.Append(" FROM ");
            s.Append(tipo.Name);

            return s.ToString();
        }

        internal static string SelectComplexSqlString<T>(Entidade entidades) where T : new()
        {
            Type tipo = typeof(T);
            var propriedades = Helper.DefinirPropriedadesBasicas<T>();
            int tam = propriedades.Length;
            int index = 0;
            foreach (var et in entidades.Entidades)
            {
                var ti = et;
            }

            StringBuilder s = new StringBuilder();
            s.Append(" SELECT ");

            foreach (var i in propriedades)
            {
                if (tam == 1)
                {
                    s.Append(String.Format(" {0}_{1} ", tipo, i.Name));
                }
                else
                {
                    if (index == 0)
                    {
                        s.Append(String.Format(" {0}_{1}, ", tipo, i.Name));
                        index = 1;
                    }
                    else
                    {
                        if (index == tam - 1)
                        {
                            s.Append(String.Format(" {0}_{1} ", tipo, i.Name));
                        }
                        else
                        {
                            s.Append(String.Format(" {0}_{1}, ", tipo, i.Name));
                            index = index + 1;
                        }
                    }
                }
            }
            s.Append(tipo);
            return s.ToString();
        }

        internal static string SelectCriteriosSqlString<T>(Parametros campos, SqlOperadorComparacao operador) where T : new()
        {
            Type tipo = typeof(T);
            var chave = Helper.RecuperarChavePrimaria<T>();

            StringBuilder s = new StringBuilder();
            s.Append(" SELECT  ");

            if (campos.Propriedades != null)
            {
                s.Append(chave.Name);
                s.Append(", ");

                for (int i = 0; i < campos.Propriedades.Count - 1; i++)
                {
                    s.Append(campos.Propriedades[i].ToString());
                    s.Append(", ");
                }
                s.Append(campos.Propriedades[campos.Propriedades.Count - 1].ToString());
            }
            else
            {
                s.Append(" * ");
            }

            s.Append(" FROM ");
            s.Append(tipo.Name);
            if (campos.Criterios != null)
            {
                s.Append(" WHERE ");
                for (int i = 0; i < campos.Criterios.Count - 1; i++)
                {
                    s.Append(campos.Criterios[i].Chave);
                    s.Append(" = @");
                    s.Append(campos.Criterios[i].Chave);
                    s.Append(" AND ");
                }
                s.Append(campos.Criterios[campos.Criterios.Count - 1].Chave);
                s.Append(" = @");
                s.Append(campos.Criterios[campos.Criterios.Count - 1].Chave);
            }
            return s.ToString();
        }

        internal static string SelectEntreDuasDatasSqlString<T>(Parametros campos) where T : new()
        {
            Type tipo = typeof(T);

            StringBuilder s = new StringBuilder();
            s.Append(" SELECT  ");
            var chave = Helper.RecuperarChavePrimaria<T>();

            if (campos.Propriedades != null)
            {
                s.Append(chave.Name);
                s.Append(", ");

                for (int i = 0; i < campos.Propriedades.Count - 1; i++)
                {
                    s.Append(campos.Propriedades[i].ToString());
                    s.Append(", ");
                }
                s.Append(campos.Propriedades[campos.Propriedades.Count - 1].ToString());

            }
            else
            {
                s.Append(" * ");
            }

            s.Append(" FROM ");
            s.Append(tipo.Name);
            s.Append(" WHERE (CONVERT (nvarchar(10), ");
            s.Append(campos.DataInicial.Chave);
            s.Append(", 103) between @Data1 AND @Data2) ");


            if (campos.Criterios != null)
            {
                s.Append(" AND ");
                for (int i = 0; i < campos.Criterios.Count - 1; i++)
                {
                    s.Append(campos.Criterios[i].Chave);
                    s.Append(" = @");
                    s.Append(campos.Criterios[i].Chave);
                    s.Append(" AND ");
                }
                s.Append(campos.Criterios[campos.Criterios.Count - 1].Chave);
                s.Append(" = @");
                s.Append(campos.Criterios[campos.Criterios.Count - 1].Chave);
            }
            return s.ToString();
        }


        internal static string SelectFullTextSqlString<T>() where T : new()
        {
            Type tipo = typeof(T);

            StringBuilder s = new StringBuilder();
            s.Append(" SELECT * FROM ");
            s.Append(tipo.Name);
            s.Append(" WHERE CONTAINS (*, @Campo)");

            return s.ToString();
        }

        internal static string SelectFullTextCriteriosSqlString<T>(Parametros campos) where T : new()
        {
            Type tipo = typeof(T);
            var chave = Helper.RecuperarChavePrimaria<T>();

            StringBuilder s = new StringBuilder();
            s.Append(" SELECT  ");

            if (campos.Propriedades != null)
            {
                s.Append(chave.Name);
                s.Append(", ");

                for (int i = 0; i < campos.Propriedades.Count - 1; i++)
                {
                    s.Append(campos.Propriedades[i].ToString());
                    s.Append(", ");
                }
                s.Append(campos.Propriedades[campos.Propriedades.Count - 1].ToString());

            }
            else
            {
                s.Append(" * ");
            }

            s.Append(" FROM ");
            s.Append(tipo.Name);

            if (campos.Criterios != null)
            {
                s.Append(" WHERE CONTAINS (*, @Campo) AND ");
                for (int i = 0; i < campos.Criterios.Count - 1; i++)
                {
                    s.Append(campos.Criterios[i].Chave);
                    s.Append(" = @");
                    s.Append(campos.Criterios[i].Chave);
                    s.Append(" AND ");
                }
                s.Append(campos.Criterios[campos.Criterios.Count - 1].Chave);
                s.Append(" = @");
                s.Append(campos.Criterios[campos.Criterios.Count - 1].Chave);
            }
            else
            {
                s.Append(" WHERE CONTAINS (*, @Campo)");
            }


            return s.ToString();
        }


        internal static string UpdateComPropriedadesSqlString<T>(Parametros campos)
        {
            Type tipo = typeof(T);
            var chave = Helper.RecuperarChavePrimaria<T>();

            StringBuilder s = new StringBuilder();
            s.Append("UPDATE ");
            s.Append(tipo.Name);
            s.Append(" SET ");

            if (campos.Propriedades != null)
            {
                for (int i = 0; i < campos.Propriedades.Count - 1; i++)
                {
                    s.Append(String.Format(" {0} = @{1},", campos.Propriedades[i].ToString(), campos.Propriedades[i].ToString()));
                }
                s.Append(String.Format(" {0} = @{1}", campos.Propriedades[campos.Propriedades.Count - 1].ToString(), campos.Propriedades[campos.Propriedades.Count - 1].ToString()));
            }

            s.Append(" WHERE ");
            s.Append(" (");
            s.Append(chave.Name);
            s.Append(" = @");
            s.Append(chave.Name);
            s.Append(")");

            return s.ToString();


        }

        internal static string SelectCount<T>()
        {
            Type tipo = typeof(T);

            StringBuilder s = new StringBuilder();
            s.Append(" SELECT COUNT(ID) FROM ");
            s.Append(tipo.Name);

            return s.ToString();
        }

        internal static string SelectCountPropriedadea<T>(Parametros campos)
        {
            Type tipo = typeof(T);
            var chave = Helper.RecuperarChavePrimaria<T>();

            StringBuilder s = new StringBuilder();
            s.Append(" SELECT COUNT(ID) FROM ");
            s.Append(tipo.Name);

            if (campos.Criterios != null)
            {
                s.Append(" WHERE ");
                for (int i = 0; i < campos.Criterios.Count - 1; i++)
                {
                    s.Append(campos.Criterios[i].Chave);
                    s.Append(" = @");
                    s.Append(campos.Criterios[i].Chave);
                    s.Append(" AND ");
                }
                s.Append(campos.Criterios[campos.Criterios.Count - 1].Chave);
                s.Append(" = @");
                s.Append(campos.Criterios[campos.Criterios.Count - 1].Chave);
            }

            return s.ToString();


        }

        internal static string SelectMax<T>(string coluna)
        {
            Type tipo = typeof(T);

            StringBuilder s = new StringBuilder();
            s.Append(" SELECT MAX(");
            s.Append(coluna); 
            s.Append(") FROM ");
            s.Append(tipo.Name);

            return s.ToString();
        }

        internal static string SelectMin<T>(string coluna)
        {
            Type tipo = typeof(T);

            StringBuilder s = new StringBuilder();
            s.Append(" SELECT MIN(");
            s.Append(coluna);
            s.Append(") FROM ");
            s.Append(tipo.Name);

            return s.ToString();
        }

        internal static string SelectFirst<T>(string propriedade, string valor)
        {
            Type tipo = typeof(T);

            StringBuilder s = new StringBuilder();
            s.Append(" SELECT * FROM ");
            s.Append(tipo.Name);
            s.Append(" WHERE ");
            s.Append(propriedade);
            s.Append(" = ");
            s.Append(valor);

            return s.ToString();
        }
    }
}
