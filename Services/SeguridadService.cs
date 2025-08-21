/*using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GuanajuatoAdminUsuarios.Models;
using System.Data.SqlClient;
using System.Data;

namespace GuanajuatoAdminUsuarios.Data
{
    public class SeguridadService
    {
        private ConexionBD _conexion;

        public SeguridadService(ConexionBD conexion)
        {
            _conexion = conexion;
        }

        public Usuario GetLogin(string usuario, string password)
        {
            var oUsuario = new Usuario();
            using var conexion = new SqlConnection(_conexion.CadenaConexion);

            using SqlCommand cmd = new SqlCommand("sp_Login");
                
                    cmd.Connection = conexion;
                    cmd.CommandType = CommandType.StoredProcedure;
                    conexion.Open();
                    cmd.Parameters.AddWithValue("@usuario", usuario);
                    cmd.Parameters.AddWithValue("@password", password);
                    cmd.CommandType = CommandType.StoredProcedure;
            using var dr = cmd.ExecuteReader();
                        while (dr.Read())
                        {


                            oUsuario.IdUsuario = Convert.ToInt32(dr["ID"]);
                            oUsuario.Nombre = dr["NOMBRE"].ToString();
                            oUsuario.Paterno = dr["PATERNO"].ToString();
                            oUsuario.Materno = dr["MATERNO"].ToString();
                        }
                    

            return oUsuario;
        }


       


    }
}
*/