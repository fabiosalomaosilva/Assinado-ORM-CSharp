using ArqOrm.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;

namespace Arquivarnet
{
    public static class ArquivarnetLinq
    {
        public static string connectionString { get; set; }

        /**
        public static IEnumerable<T> Include<T, TKey>(this IEnumerable<T> source, Func<T, TKey> keySelector) where T : new() where TKey : new()
        {
            using (var db = new Session(connectionString))
            {
                var props = Helper.DefinirClases<T>();
                var chave1 = keySelector.Method.ReturnType.Name;
                var chave = Helper.RecuperarChavePrimaria<T>();
                var obj = new T();
                var obj1 = new TKey();
                
                foreach (var i in props)
                {                    
                    if (i.Name == chave1)   
                    {
                        foreach (var item in source)
                        {
                            if (item.ToString() == obj1.ToString())
                            {                                                           
                                dynamic dados = db.PesquisarID<TKey>(item);
                                var ddd = "";
                            }
                        }
                    }
                }
                return db.ListarTudo<T>();
            }
        }

        public static T Extend<T>(this T target, T source) where T : class
        {
            var properties =
                target.GetType().GetProperties().Where(pi => pi.CanRead && pi.CanWrite);

            foreach (var propertyInfo in properties)
            {
                var targetValue = propertyInfo.GetValue(target, null);
                //var defaultValue = propertyInfo.PropertyType.GetDefault();

                if (targetValue != null && !targetValue.Equals(defaultValue)) continue;

                var sourceValue = propertyInfo.GetValue(source, null);
                propertyInfo.SetValue(target, sourceValue, null);
            }

            return target;
        }
        **/

        public static T Incluir<T, TKey>(this T source, Func<T, TKey> Entidade) where T : new() where TKey : new()
        {
            var properties = Helper.DefinirTodasPropriedades<T>();
            T resultado = source;
            TKey obj1 = new TKey();
            TKey obj = new TKey();
            var chavePrimaria = Helper.RecuperarChavePrimaria<TKey>();
            var o = obj.GetType();

            var chaveEstrangeira = Helper.RecuperarChaveEstrangeira<T>(o.Name);
            object valorChavePrimaria = null;

            foreach (var i in properties)
            {
                var a = i.Name;
                if (a == chaveEstrangeira)
                {
                    valorChavePrimaria = i.GetValue(source, null);
                }
            }

            if (valorChavePrimaria != null)
            {
                foreach (var i in properties)
                {
                    var b = obj.ToString();
                    var a = i.PropertyType.FullName;

                    if (a == b)
                    {
                        using (var db = new Session(connectionString))
                        {
                            obj1 = db.PesquisarCriterio<TKey>(new Criterio(chavePrimaria.Name, valorChavePrimaria));
                            i.SetValue(resultado, obj1, null);
                        }
                    }
                }
            }
            return resultado;
        }

        public static DataSet ToDataSet<T>(this List<T> list) where T : new()
        {
            Type elementType = typeof(T);
            DataSet ds = new DataSet();
            DataTable t = new DataTable();

            ds.Tables.Add(t);

            //add a column to table for each public property on T
            foreach (var propInfo in Helper.DefinirPropriedadesBasicas<T>())
            {
                t.Columns.Add(propInfo.Name, propInfo.PropertyType);
            }

            //go through each property on T and add each value to the table
            foreach (T item in list)
            {
                DataRow row = t.NewRow();
                foreach (var propInfo in Helper.DefinirPropriedadesBasicas<T>())
                {
                    row[propInfo.Name] = propInfo.GetValue(item, null);
                }
            }

            return ds;
        }

        public static DataTable ToDataTable<T>(this List<T> list) where T : new()
        {
            //Type tipo = Helper.DefinirPropriedadesBasicas<T>();
            var elementType = Helper.DefinirPropriedadesBasicas<T>();
            DataTable t = new DataTable();

            //add a column to table for each public property on T
            foreach (var propInfo in elementType)
            {
                t.Columns.Add(propInfo.Name, propInfo.PropertyType);
            }

            //go through each property on T and add each value to the table
            foreach (T item in list)
            {
                DataRow row = t.NewRow();
                foreach (var propInfo in elementType)
                {
                    row[propInfo.Name] = propInfo.GetValue(item, null);
                }
            }

            return t;
        }


        private static List<T> ConvertToList<T>(DataTable dt)
        {
            var columnNames = dt.Columns.Cast<DataColumn>()
                .Select(c => c.ColumnName)
                .ToList();

            var properties = typeof(T).GetProperties();

            return dt.AsEnumerable().Select(row =>
            {
                var objT = Activator.CreateInstance<T>();

                foreach (var pro in properties)
                {
                    if (columnNames.Contains(pro.Name))
                    {
                        pro.SetValue(objT, row[pro.Name], null);
                    }
                }

                return objT;
            }).ToList();

        }


        public static int Count<T>(this T source) where T : class
        {
            int contador;
            try
            {
                using (Session db = new Session(connectionString))
                {
                    contador = db.MContador<T>();
                }
                return contador;
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("{0}\r\n{1}", ex.Message, ex.InnerException));
            }
        }

        public static int Count<T>(this T source, Parametros parametros) where T : class
        {
            int contador;
            try
            {
                using (Session db = new Session(connectionString))
                {
                    contador = db.MContador<T>(parametros);
                }
                return contador;
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("{0}\r\n{1}", ex.Message, ex.InnerException));
            }
        }

        public static int Max<T>(this List<T> source, string NomeColuna) where T : class
        {
            int max;
            try
            {
                using (Session db = new Session(connectionString))
                {
                    max = db.Max<T>(NomeColuna);
                }
                return max;
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("{0}\r\n{1}", ex.Message, ex.InnerException));
            }
        }
        public static int Min<T>(this List<T> source, string NomeColuna) where T : class
        {
            int min;
            try
            {
                using (Session db = new Session(connectionString))
                {
                    min = db.Min<T>(NomeColuna);
                }
                return min;
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("{0}\r\n{1}", ex.Message, ex.InnerException));
            }
        }

        public static string GetForeignKey<T, TKey>(this T fonte, Func<T, TKey> entidade) where T : class where TKey : class
        {
            //Type tipo = RecuperarClassePorNome<T>(Entidade);
            Type tipoT = typeof(T);
            var propriedades = tipoT.GetProperties();
            PropertyInfo prop = null;
            var ent = entidade.Method.ReturnParameter.ParameterType.Name;

            foreach (var i in propriedades)
            {
                if (i.PropertyType.Name == ent)
                {
                    prop = i;
                }
            }
            string chave = null;

            var campos = prop.GetCustomAttributes(typeof(ForeignKeyAttribute), true);
            foreach (var i in campos)
            {
                ForeignKeyAttribute att = (ForeignKeyAttribute)i;
                chave = att.Nome;
            }


            return chave;
        }

    }
}


