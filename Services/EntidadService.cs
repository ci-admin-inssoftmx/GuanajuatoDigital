/*using GuanajuatoAdminUsuarios.Models;
using System.Collections.Generic;
using System.Data.SqlClient;
using System;
using System.Data;

namespace GuanajuatoAdminUsuarios.Data
{
    public class EntidadService
    {
        private ConexionBD _conexion;

        public EntidadService(ConexionBD conexion)
        {
            _conexion = conexion;
        }


        public List<Entidad> GetEntidades()
        {
            var oLista = new List<Entidad>();
            using var conexion = new SqlConnection(_conexion.CadenaConexion);
            using SqlCommand cmd = new SqlCommand("sp_getEntidades");
                    cmd.Connection = conexion;
                    cmd.CommandType = CommandType.StoredProcedure;
                    conexion.Open();
                    using var dr = cmd.ExecuteReader();
                    
                        while (dr.Read())
                        {
                            oLista.Add(new Entidad()
                            {
                                Id = Convert.ToInt32(dr["ID"]),
                                Descripcion = dr["DESCRIPCION"].ToString(),
                            });
                        }

            return oLista;
        }
    }
}*/
