using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace AlprNetGuiTest
{
   class Busqueda
    {
        public static int getplaca(string placa) {
           
                MySqlConnection conexion = BdComun.ObtenerConexion();
                MySqlCommand comando = new MySqlCommand(string.Format("select placa from carro where placa='{0}'", placa), conexion);
                // retorno = comando.ExecuteNonQuery();
                var result = comando.ExecuteScalar();

                if (result != null)
                {
                    conexion.Close();
                    return 1;
                }
                else { conexion.Close(); return 0; }         
               
                     
            
        }

        public static int EnUni(string placa) {
            string ci="";
            DateTime Hoy = DateTime.Today;
           // string fecha_actual = Hoy.ToString("yyyy-MM-dd");
            string fecha_final = "";
            MySqlConnection conexion = BdComun.ObtenerConexion();
            MySqlCommand comando = new MySqlCommand(string.Format("select ci_usuario from carro where placa='{0}'", placa), conexion);
            MySqlDataReader reader = comando.ExecuteReader();
            while (reader.Read()) {
                ci=reader.GetString(0);
            }
            reader.Close();
          //  System.Console.WriteLine(fecha_actual);
            System.Console.WriteLine(ci);
            System.Console.WriteLine(placa);
            MySqlCommand comando2 = new MySqlCommand(string.Format("Insert into carrosenuni (ci_usuario,placa,fecha_ini) values('{0}','{1}',NOW())", ci,placa), conexion);
            var result = comando2.ExecuteNonQuery();
            //Insert into carrosenuni (ci_usuario,placa,fecha_ini) values ('{0}','{1}','{2}'
            if (result>0)
            {
                System.Console.WriteLine("bien");

                conexion.Close();
                return 1;
            }
            else { System.Console.WriteLine("mal"); conexion.Close(); return 0; }
            
        }
    }
}
