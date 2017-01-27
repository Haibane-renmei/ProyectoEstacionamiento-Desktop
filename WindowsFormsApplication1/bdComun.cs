
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;

namespace AlprNetGuiTest
{
   public class BdComun
    {
        public static MySqlConnection ObtenerConexion()
        {
            MySqlConnection conectar = new MySqlConnection("server=127.0.0.1; database=desarrollodb; Uid=root; pwd=;");

            conectar.Open();
            return conectar;
        }
    }
}
