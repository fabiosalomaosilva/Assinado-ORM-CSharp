using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace Arquivarnet
{
    public interface IRepositorio
    {
        List<T> ListarTudo<T>() where T : new();
        List<T> ListarPorCampo<T>(Parametros campos)  where T : new();
        List<T> ListarPorCriterios<T>(Parametros campos, SqlOperadorComparacao operador) where T : new();
        List<T> ListarPorUmCriterio<T>(Parametros campo) where T : new();
        T PesquisarID<T>(object valor) where T : new();
        void Inserir<T>(T obj) where T : new();
        void Editar<T>(T obj) where T : new();
        void Excluir<T>(T obj) where T : new();
        }
}