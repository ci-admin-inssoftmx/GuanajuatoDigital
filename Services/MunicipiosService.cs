using GuanajuatoAdminUsuarios.Entity;
using GuanajuatoAdminUsuarios.Interfaces;
using System.Collections.Generic;
using System.Data;
using System;
using GuanajuatoAdminUsuarios.Models;
using System.Data.SqlClient;

namespace GuanajuatoAdminUsuarios.Services
{
    public class MunicipiosService : IMunicipiosService
    {
        private readonly ISqlClientConnectionBD _sqlClientConnectionBD;
        public MunicipiosService(ISqlClientConnectionBD sqlClientConnectionBD)
        {
            _sqlClientConnectionBD = sqlClientConnectionBD;
        }


        public List<MunicipiosModel> GetMunicipios()
        {
            //
            List<MunicipiosModel> ListMunicipios = new List<MunicipiosModel>();

            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
                try
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand("Select * from catMunicipios", connection);
                    command.CommandType = CommandType.Text;
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            MunicipiosModel municipio = new MunicipiosModel();
                            municipio.IdMunicipio = Convert.ToInt32(reader["idMunicipio"].ToString());
                            municipio.Municipio = reader["municipio"].ToString();
                            ListMunicipios.Add(municipio);

                        }

                    }

                }
                catch (SqlException ex)
                {
                    //Guardar la excepcion en algun log de errores
                    //ex
                }
                finally
                {
                    connection.Close();
                }
            return ListMunicipios;


        }
    }
}
