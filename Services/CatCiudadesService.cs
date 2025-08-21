using GuanajuatoAdminUsuarios.Entity;
using GuanajuatoAdminUsuarios.Interfaces;
using GuanajuatoAdminUsuarios.Models;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;

namespace GuanajuatoAdminUsuarios.Services
{
    public class CatCiudadesService : ICatCiudadesService
    {
        private readonly ISqlClientConnectionBD _sqlClientConnectionBD;
        public CatCiudadesService(ISqlClientConnectionBD sqlClientConnectionBD)
        {
            _sqlClientConnectionBD = sqlClientConnectionBD;
        }

        public List<CatCiudadesModel> ObtenerCiudadesActivas()
        {
            //
            List<CatCiudadesModel> ListaCiudades = new List<CatCiudadesModel>();

            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
                try

                {
                    connection.Open();
                    SqlCommand command = new SqlCommand("SELECT catCiudades.*, estatus.estatusdesc FROM catCiudades JOIN estatus ON catCiudades.estatus = estatus.estatus WHERE catCiudades.estatus = 1 ORDER BY Ciudad ASC", connection);
                    command.CommandType = CommandType.Text;
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            CatCiudadesModel ciudad = new CatCiudadesModel();
                            ciudad.IdCiudad = Convert.ToInt32(reader["IdCiudad"].ToString());
                            ciudad.IdEntidad = Convert.ToInt32(reader["IdEntidad"].ToString());
                            ciudad.Ciudad = reader["Ciudad"].ToString();
                            //marcasVehiculo.FechaActualizacion = Convert.ToDateTime(reader["FechaActualizacion"].ToString());
                            ciudad.ActualizadoPor = Convert.ToInt32(reader["ActualizadoPor"].ToString());
                            ciudad.Estatus = Convert.ToInt32(reader["Estatus"].ToString());
                            ciudad.estatusDesc = reader["estatusDesc"].ToString();
                            ListaCiudades.Add(ciudad);
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
            return ListaCiudades;


        }

    }
}
