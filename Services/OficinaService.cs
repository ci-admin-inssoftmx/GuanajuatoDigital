/*using System.Data.SqlClient;
using System.Data;
using GuanajuatoAdminUsuarios.Models;
using System.Collections.Generic;
using System;
using GuanajuatoAdminUsuarios.Data;

namespace GuanajuatoAdminUsuarios.Services
{
    public class OficinaService
    {

        private ConexionBD _conexion;

        public OficinaService(ConexionBD conexion)
        {
            _conexion = conexion;
        }

        public List<Oficina> GetOficinas()
        {
            var oLista = new List<Oficina>();
            using (var conexion = new SqlConnection(_conexion.CadenaConexion))
            {
                using (SqlCommand cmd = new SqlCommand("sp_getOficinas"))
                {
                    cmd.Connection = conexion;
                    cmd.CommandType = CommandType.StoredProcedure;
                    conexion.Open();
                    cmd.CommandType = CommandType.StoredProcedure;
                    using (var dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            oLista.Add(new Oficina()
                            {
                                Id = Convert.ToInt32(dr["ID"]),
                                Descripcion = dr["DESCRIPCION"].ToString(),
                                Estatus = Convert.ToInt32(dr["Estatus"])


                            });
                        }
                    }
                    conexion.Close();
                }

            }

            return oLista;
        }

        public string GuardaOficina(string descripcion, int idEntidad, int idUsuario)
        {
            // var oApss = new AppsModel();
            string salida = "";

            using (var conexion = new SqlConnection(_conexion.CadenaConexion))
            {
                using (SqlCommand cmd = new SqlCommand("sp_guardaOficina"))
                {
                    cmd.Connection = conexion;
                    cmd.CommandType = CommandType.StoredProcedure;
                    conexion.Open();
                    cmd.Parameters.AddWithValue("Descripcion", descripcion);
                    cmd.Parameters.AddWithValue("IdUsuarioAlta", idUsuario);
                    cmd.Parameters.AddWithValue("IdEntidad", idEntidad);

                    SqlParameter parm = new SqlParameter()
                    {
                        ParameterName = "@returnVal",
                        SqlDbType = SqlDbType.VarChar,
                        Direction = System.Data.ParameterDirection.Output,
                        Size = 250
                    };

                    cmd.Parameters.Add(parm);
                    cmd.CommandType = CommandType.StoredProcedure;
                    using (var dr = cmd.ExecuteReader())
                    {
                        salida = parm.Value.ToString();
                    }
                    conexion.Close();
                }
            }

            return salida;
        }

        public Oficina GetOficinaById(int? idOficina)
        {
            var oficina = new Oficina();
            using (var conexion = new SqlConnection(_conexion.CadenaConexion))
            {
                using (SqlCommand cmd = new SqlCommand("sp_getOficina"))
                {
                    cmd.Connection = conexion;
                    cmd.CommandType = CommandType.StoredProcedure;
                    conexion.Open();
                    cmd.Parameters.AddWithValue("id", idOficina);
                    cmd.CommandType = CommandType.StoredProcedure;
                    using (var dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {

                            oficina.Id = Convert.ToInt32(dr["ID"]);
                            oficina.Descripcion = dr["DESCRIPCION"].ToString();
                            oficina.IdEntidad = Int32.Parse(dr["ID_ENTIDAD"].ToString());
                            oficina.Estatus = Convert.ToInt32(dr["Estatus"]);

                        }
                    }
                    conexion.Close();
                }
            }

            return oficina;
        }

        public string ActualizaOficina(int id, string descripcion, int idEntidad,
                                    int estatus, string idUsuario)
        {
            string salida = "";

            using (var conexion = new SqlConnection(_conexion.CadenaConexion))
            {
                using (SqlCommand cmd = new SqlCommand("sp_actualizaOficina"))
                {
                    cmd.Connection = conexion;
                    cmd.CommandType = CommandType.StoredProcedure;
                    conexion.Open();
                    cmd.Parameters.AddWithValue("id", id);
                    cmd.Parameters.AddWithValue("Descripcion", descripcion);
                    cmd.Parameters.AddWithValue("IdEntidad", idEntidad);
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

                        salida = parm.Value.ToString();
                    }
                    conexion.Close();
                }
            }

            return salida;
        }

    }
}*/
