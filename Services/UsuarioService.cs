/*using GuanajuatoAdminUsuarios.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace GuanajuatoAdminUsuarios.Data
{
    public class UsuarioService
    {

        private ConexionBD _conexion;

        public UsuarioService(ConexionBD conexion)
        {
            _conexion = conexion;
        }

        public List<Usuario> ListarUsuarios()
        {
            var oLista = new List<Usuario>();
            using (var conexion = new SqlConnection(_conexion.CadenaConexion))
            {
              
                using (SqlCommand cmd = new SqlCommand("sp_GetUsuarios")) {
                    cmd.Connection = conexion;
                    cmd.CommandType = CommandType.StoredProcedure;
                    conexion.Open();
                using (var dr = cmd.ExecuteReader())
                {
                        while (dr.Read())
                        {
                            oLista.Add(new Usuario()
                            {
                                IdUsuario = Convert.ToInt32(dr["ID"]),
                                Nombre = dr["NOMBRE"].ToString(),
                                Paterno = dr["PATERNO"].ToString(),
                                Materno = dr["MATERNO"].ToString(),
                                Estatus = Convert.ToInt32(dr["Estatus"])


                            });
                        }
                    }
                    conexion.Close();
                }
                
            }

            return oLista;
        }

        public string GuardaUsuario(string nombre, string paterno, string materno, string email, string login,
                                    string clave, string idUsuario)
        {
            string salida = "";
            using (var conexion = new SqlConnection(_conexion.CadenaConexion))
            {
                using (SqlCommand cmd = new SqlCommand("sp_guardaUsuario"))
                {
                    cmd.Connection = conexion;
                    cmd.CommandType = CommandType.StoredProcedure;
                    conexion.Open();
                    {
                        cmd.Parameters.AddWithValue("Nombre", nombre);
                        cmd.Parameters.AddWithValue("Paterno", paterno);
                        cmd.Parameters.AddWithValue("Materno", materno);
                        cmd.Parameters.AddWithValue("Email", email);
                        cmd.Parameters.AddWithValue("Login", login);
                        cmd.Parameters.AddWithValue("Clave", clave);
                        cmd.Parameters.AddWithValue("IdUsuarioAlta", idUsuario);

                        SqlParameter parm = new SqlParameter()
                        {
                            ParameterName = "@returnVal",
                            SqlDbType = SqlDbType.VarChar,
                            Direction = System.Data.ParameterDirection.Output,
                            Size = 250
                        };

                        //  cmd.Parameters.AddWithValue("returnVal","");
                        cmd.Parameters.Add(parm);
                        cmd.CommandType = CommandType.StoredProcedure;
                        using (var dr = cmd.ExecuteReader())
                        {
                            //while (dr.Read())
                            //{
                            //   salida = dr["ID"].ToString();

                            //}
                            salida = parm.Value.ToString();
                        }

                    }
                    conexion.Close();

                }
            }

            return salida;
        }


        public Usuario GetUsuario(int? idUsuario)
        {
            var oUsuario = new Usuario();
            using (var conexion = new SqlConnection(_conexion.CadenaConexion))
            {
                using (SqlCommand cmd = new SqlCommand("sp_GetUsuario"))
                {
                    cmd.Connection = conexion;
                    cmd.CommandType = CommandType.StoredProcedure;
                    conexion.Open();
                    cmd.Parameters.AddWithValue("idUsuario", idUsuario);
                cmd.CommandType = CommandType.StoredProcedure;
                    using (var dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {

                            oUsuario.IdUsuario = Convert.ToInt32(dr["ID"]);
                            oUsuario.Nombre = dr["NOMBRE"].ToString();
                            oUsuario.Paterno = dr["PATERNO"].ToString();
                            oUsuario.Materno = dr["MATERNO"].ToString();
                            oUsuario.Email = dr["EMAIL"].ToString();
                            oUsuario.Login = dr["LOGIN"].ToString();
                            oUsuario.Clave = dr["PASSWORD"].ToString();
                            oUsuario.Estatus = Convert.ToInt32(dr["Estatus"]);

                        }
                    }
                    conexion.Close();
                }
               
            }

            return oUsuario;
        }



        public string ActualizaUsuario(int id, string nombre, string paterno, string materno, string email, string login,
                                    int estatus, string idUsuario)
        {
            string salida = "";

            using (var conexion = new SqlConnection(_conexion.CadenaConexion))
            {
                using (SqlCommand cmd = new SqlCommand("sp_actualizaUsuario"))
                {
                    cmd.Connection = conexion;
                    cmd.CommandType = CommandType.StoredProcedure;
                    conexion.Open();
                    cmd.Parameters.AddWithValue("id", id);
                    cmd.Parameters.AddWithValue("Nombre", nombre);
                    cmd.Parameters.AddWithValue("Paterno", paterno);
                    cmd.Parameters.AddWithValue("Materno", materno);
                    cmd.Parameters.AddWithValue("Email", email);
                    cmd.Parameters.AddWithValue("Login", login);
                    cmd.Parameters.AddWithValue("Estatus", estatus);
                    cmd.Parameters.AddWithValue("IdUsuarioAlta", idUsuario);

                    SqlParameter parm = new SqlParameter()
                    {
                        ParameterName = "@returnVal",
                        SqlDbType = SqlDbType.VarChar,
                        Direction = System.Data.ParameterDirection.Output,
                        Size = 250
                    };

                    //  cmd.Parameters.AddWithValue("returnVal","");
                    cmd.Parameters.Add(parm);
                    cmd.CommandType = CommandType.StoredProcedure;
                    using (var dr = cmd.ExecuteReader())
                    {
                        //while (dr.Read())
                        //{
                        //   salida = dr["ID"].ToString();

                        //}
                        salida = parm.Value.ToString();
                    }
                    conexion.Close();
                }
            }

            return salida;
        }



    }
}
*/

