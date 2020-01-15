using Arquivarnet;
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
using ArqOrm.Model;

namespace Arquivarnet
{
    public class Helper
    {

        internal static PropertyInfo[] DefinirTodasPropriedades<T>() where T : new()
        {
            Type tipo = typeof(T);
            return tipo.GetProperties();
        }

        internal static PropertyInfo[] DefinirPropriedadesBasicasInserir<T>() where T : new()
        {
            Type tipo = typeof(T);
            var propriedades = tipo.GetProperties();
            List<PropertyInfo> props = new List<PropertyInfo>();
            PropertyInfo chave = null;


            foreach (var i in propriedades)
            {
                if (i.PropertyType.FullName.Substring(0, 6) == "System")
                {
                    if (i.PropertyType.FullName.Length >= 18)
                    {
                        if (i.PropertyType.FullName.Substring(0, 18) != "System.Collections")
                        {
                            props.Add(i);
                        }
                    }
                    else
                    {
                        if (VerificaDatabaseIdentityAttribute<T>(i) == false)
                        {
                            if (VerificaChavePrimariaIdentity<T>(i) == false)
                            {
                                props.Add(i);
                            }

                        }

                    }
                }
                else
                {
                    if (i.PropertyType.BaseType.FullName == "System.Enum")
                    {
                        props.Add(i);
                    }
                }
            }

            PropertyInfo[] array = new PropertyInfo[props.Count];
            int index = 0;
            foreach (var i in props)
            {
                if (index == 0)
                {
                    array[0] = i;
                    index++;
                }
                else
                {
                    array[index] = i;
                    index++;
                }
            }

            return array;
        }
        internal static PropertyInfo[] DefinirPropriedadesBasicas<T>() where T : new()
        {
            Type tipo = typeof(T);
            var propriedades = tipo.GetProperties();
            List<PropertyInfo> props = new List<PropertyInfo>();
            PropertyInfo chave = null;
            

            foreach (var i in propriedades)
            {
                if (i.PropertyType.FullName.Substring(0, 6) == "System")
                {
                    if (i.PropertyType.FullName.Length >= 18)
                    {
                        if (i.PropertyType.FullName.Substring(0, 18) != "System.Collections")
                        {
                            props.Add(i);
                        }
                    }
                    else
                    {
                        if (VerificaDatabaseIdentityAttribute<T>(i) == false)
                        {
                            props.Add(i);
                        }

                    }
                }   
                else
                {
                    if (i.PropertyType.BaseType.FullName == "System.Enum")
                    {
                        props.Add(i);
                    }
                }
            }

            PropertyInfo[] array = new PropertyInfo[props.Count];
            int index = 0;
            foreach (var i in props)
            {
                if (index == 0)
                {
                    array[0] = i;
                    index++;
                }
                else
                {
                    array[index] = i;
                    index++;
                }
            }

            return array;
        }
        internal static PropertyInfo[] DefinirPropriedadesBasicas(Type tipo)
        {
            var propriedades = tipo.GetProperties();
            List<PropertyInfo> props = new List<PropertyInfo>();


            foreach (var i in propriedades)
            {
                if (i.PropertyType.FullName.Substring(0, 6) == "System")
                {
                    if (i.PropertyType.FullName.Length >= 18)
                    {
                        if (i.PropertyType.FullName.Substring(0, 18) != "System.Collections")
                        {
                            props.Add(i);
                        }
                    }
                    else
                    {
                        props.Add(i);
                    }
                }
            }


            PropertyInfo[] array = new PropertyInfo[props.Count];
            int index = 0;
            foreach (var i in props)
            {
                if (index == 0)
                {
                    array[0] = i;
                    index++;
                }
                else
                {
                    array[index] = i;
                    index++;
                }
            }

            return array;
        }

        internal static PropertyInfo[] DefinirPropriedadesColecoes<T>() where T : new()
        {
            Type tipo = typeof(T);
            var propriedades = tipo.GetProperties();
            List<PropertyInfo> props = new List<PropertyInfo>();


            foreach (var i in propriedades)
            {
                if (i.PropertyType.FullName.Substring(0, 6) == "System")
                {
                    if (i.PropertyType.FullName.Length >= 18)
                    {
                        if (i.PropertyType.FullName.Substring(0, 18) == "System.Collections")
                        {
                            props.Add(i);
                        }
                    }

                }
            }


            PropertyInfo[] array = new PropertyInfo[props.Count];
            int index = 0;
            foreach (var i in props)
            {
                if (index == 0)
                {
                    array[0] = i;
                    index++;
                }
                else
                {
                    array[index] = i;
                    index++;
                }
            }

            return array;
        }        

        internal static PropertyInfo[] DefinirPropriedadesCustomizadas<T>(Parametros campos)
        {
            if (campos.Propriedades != null)
            {
                Type tipo = typeof(T);
                var propriedades = tipo.GetProperties();
                List<PropertyInfo> props = new List<PropertyInfo>();

                props.Add(RecuperarChavePrimaria<T>());

                foreach (var i in propriedades)
                {
                    if (i.PropertyType.FullName.Substring(0, 6) == "System")
                    {
                        if (i.PropertyType.FullName.Length >= 18)
                        {
                            if (i.PropertyType.FullName.Substring(0, 18) != "System.Collections")
                            {
                                if (campos.Propriedades == null)
                                {
                                    props.Add(i);
                                }
                                else
                                {
                                    foreach (var c in campos.Propriedades)
                                    {
                                        if (c == i.Name)
                                        {
                                            props.Add(i);
                                        }
                                    }

                                }
                            }
                        }
                        else
                        {
                            if (campos.Propriedades == null)
                            {
                                props.Add(i);
                            }
                            else
                            {
                                foreach (var c in campos.Propriedades)
                                {
                                    if (c == i.Name)
                                    {
                                        props.Add(i);
                                    }
                                }
                            }
                        }
                    }
                }


                PropertyInfo[] array = new PropertyInfo[props.Count];
                int index = 0;
                foreach (var i in props)
                {
                    if (index == 0)
                    {
                        array[0] = i;
                        index++;
                    }
                    else
                    {
                        array[index] = i;
                        index++;
                    }
                }

                return array;

            }
            else
            {
                return null;
            }
        }

        internal static PropertyInfo[] DefinirClases<T>() where T : new()
        {
            Type tipo = typeof(T);
            var propriedades = tipo.GetProperties();
            List<PropertyInfo> props = new List<PropertyInfo>();


            foreach (var i in propriedades)
            {
                if (i.PropertyType.FullName.Substring(0, 6) != "System")
                {
                    props.Add(i);
                }
            }

            PropertyInfo[] array = new PropertyInfo[props.Count];
            int index = 0;
            foreach (var i in props)
            {
                if (index == 0)
                {
                    array[0] = i;
                    index++;
                }
                else
                {
                    array[index] = i;
                    index++;
                }
            }

            return array;
        }


        public static PropertyInfo RecuperarChavePrimaria<T>()
        {
            Type tipo = typeof(T);
            var propriedades = tipo.GetProperties();
            PropertyInfo chave = null;

            foreach (var c in propriedades)
            {
                // lista atributos do campo
                var keys = c.GetCustomAttributes(typeof(KeyAttribute), true);
                if (keys.Length > 0)
                {
                    chave = c;
                }
            }
            return chave;
        }

        public static PropertyInfo RecuperarChaveEstrangeira(Type tipo)
        {
            var propriedades = tipo.GetProperties();
            PropertyInfo chave = null;

            foreach (var c in propriedades)
            {
                // lista atributos do campo
                var keys = c.GetCustomAttributes(typeof(ForeignKeyAttribute), true);
                if (keys.Length > 0)
                {
                    chave = c;
                }
            }
            return chave;
        }
        public static string RecuperarChaveEstrangeira<T>(string Entidade) where T : new()
        {
            //Type tipo = RecuperarClassePorNome<T>(Entidade);
            Type tipoT = typeof(T);
            var propriedades = tipoT.GetProperties();
            PropertyInfo prop = null;
            foreach (var i in propriedades)
            {
                if (i.PropertyType.Name == Entidade)
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

        public static bool VerificaDatabaseIdentityAttribute<T>(PropertyInfo Entidade) where T : new()
        {
            //Type tipo = RecuperarClassePorNome<T>(Entidade);
            Type tipoT = typeof(T);
            var propriedades = tipoT.GetProperties();

            bool Identidade = false;
            var campos = Entidade.GetCustomAttributes(typeof(DatabaseIdentityAttribute), true);
            foreach (var i in campos)
            {
                DatabaseIdentityAttribute att = (DatabaseIdentityAttribute)i;
                Identidade = att.IsActive;
            }


            return Identidade;
        }
        public static bool VerificaChavePrimariaIdentity<T>(PropertyInfo Entidade) where T : new()
        {
            Type tipo = typeof(T);
            var propriedades = tipo.GetProperties();
            PropertyInfo chave = null;

            foreach (var c in propriedades)
            {
                // lista atributos do campo
                var keys = c.GetCustomAttributes(typeof(KeyAttribute), true);
                if (keys.Length > 0)
                {
                    chave = c;
                }
            }

            if (chave == Entidade)
            {
                if (chave.PropertyType.Name == "Int" || chave.PropertyType.Name == "long" || chave.PropertyType.Name == "Int32" || chave.PropertyType.Name == "Int64" || chave.PropertyType.Name == "short" || chave.PropertyType.Name == "Int16")
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        internal static Type RecuperarClassePorNome<T>(string obj) where T : new()
        {
            PropertyInfo nomeClasse = null;
            PropertyInfo[] classesDerivadas = DefinirPropriedadesColecoes<T>();

            if (classesDerivadas.Count() != 0)
            {
                foreach (var item in classesDerivadas)
                {
                    if (item.Name == obj)
                    {
                        nomeClasse = item;
                    }
                }
                if (nomeClasse != null)   
                {
                    return nomeClasse.PropertyType;
                }

            }
            else
            {
                return null;
            }

            return null;
        }
    }
}