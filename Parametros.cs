using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Arquivarnet
{
    public class Parametros
    {
        public Parametros()
        {

        }

        public Parametros(Criterio criterio)
        {
            this.Criterio = criterio;
        }

        public Parametros(IList<Criterio> criterios)
        {
            IList<Criterio> c = criterios;
            this.Criterios = c;
        }
        /// <summary>
        /// Adiciona um critério tipo chave/valor a ser utilizado em updates, deletes e finds
        /// </summary>
        public Criterio Criterio { get; set; }

        /// <summary>
        /// Adiciona uma propriedade a ser utilizado consultas
        /// </summary>
        public string Propriedade { get; set; }

        /// <summary>
        /// Adiciona uma campo para fusão de tabelas relacionais
        /// </summary>
        //public IList<string> Joins { get; set; }

        /// <summary>
        /// Data para de início da consulta
        /// </summary>
        public Criterio DataInicial { get; set; }
        /// <summary>
        /// Data fim da consulta
        /// </summary>
        public Criterio DataFinal { get; set; }


        /// <summary>
        /// Adiciona uma propriedade a ser utilizado consultas
        /// </summary>
        public object ParametroID { get; set; }

        /// <summary>
        /// Lista de propriedades a ser retornadas no Select
        /// </summary>
        public IList<string> Propriedades { get; set; }

        /// <summary>
        /// Lista de critérios tipo chave/valor a ser pesquisados no Select
        /// </summary>
        public IList<Criterio> Criterios { get; set; }

        /// <summary>
        /// Adicionar nova propriedade à lista
        /// </summary>
        public void AddPropriedades(string propriedade)
        {
            IList<string> c;
            if (Propriedades == null)
            {
                c = new List<string>();
            }
            else
            {
                c = Propriedades;
            }

            c.Add(propriedade);
            Propriedades = c;
        }

        /**
        /// <summary>
        /// Adicionar novo join ao select
        /// </summary>
        public void AddJoin(string Entity)
        {
            IList<string> c;
            if (Joins == null)
            {
                c = new List<string>();
            }
            else
            {
                c = Joins;
            }

            c.Add(Entity);
            Joins = c;
        }
        **/

        /// <summary>
        /// Adicionar novo critério à lista
        /// </summary>
        public void AddCriterio(string chave, object valor)
        {
            try
            {
                IList<Criterio> c;
                if (Criterios == null)
                {
                    c = new List<Criterio>();
                }
                else
                {
                    c = Criterios;
                }
                
                c.Add(new Criterio(chave, valor));
                Criterios = c;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// Adicionar nova data para select simples ou entre duas datas 
        /// </summary>
        public void AddDataInicial(string chave, object valor)
        {
            try
            {
                DataInicial = new Criterio(chave, valor);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// Adicionar data de fim para select com duas datas
        /// </summary>
        public void AddDataFinal(string chave, object valor)
        {
            try
            {
                DataFinal = new Criterio(chave, valor);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }        

    }
}