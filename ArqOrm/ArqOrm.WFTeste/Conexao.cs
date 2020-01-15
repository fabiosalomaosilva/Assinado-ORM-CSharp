using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace ArqOrm.WFTeste
{
    public class Conexao
    {
        public static string ConStr
        {
            get
            { 
                return ConfigurationManager.ConnectionStrings["conexao"].ConnectionString; 
            }
        }
    }
}
