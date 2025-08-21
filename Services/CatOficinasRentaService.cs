using GuanajuatoAdminUsuarios.Entity;
using GuanajuatoAdminUsuarios.Interfaces;
using GuanajuatoAdminUsuarios.Models;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;

namespace GuanajuatoAdminUsuarios.Services
{
    public class CatOficinasRentaService : ICatOficinasRentaService
    {
        private readonly ISqlClientConnectionBD _sqlClientConnectionBD;
        public CatOficinasRentaService(ISqlClientConnectionBD sqlClientConnectionBD)
        {
            _sqlClientConnectionBD = sqlClientConnectionBD;
        }

        public List<CatOficinasRentaModel> ObtenerOficinasActivas()
        {
            //
            List<CatOficinasRentaModel> ListaOficinas = new List<CatOficinasRentaModel>();

            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
                try

                {
                    connection.Open();
                    SqlCommand command = new SqlCommand("SELECT catOficinasRenta.*, estatus.estatusdesc FROM catOficinasRenta JOIN estatus ON catOficinasRenta.estatus = estatus.estatus" +
                        " WHERE catOficinasRenta.estatus = 1 ORDER BY NombreOficina ASC", connection);
                    command.CommandType = CommandType.Text;
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            CatOficinasRentaModel oficina = new CatOficinasRentaModel();
                            oficina.IdOficinaRenta = Convert.IsDBNull(reader["idOficinaRenta"]) ? 0 : Convert.ToInt32(reader["idOficinaRenta"]);
                            oficina.IdDelegacion = Convert.IsDBNull(reader["idDelegacion"]) ? 0 : Convert.ToInt32(reader["idDelegacion"]);
                            oficina.NombreOficina = reader["NombreOficina"].ToString();
                            oficina.FechaActualizacion = Convert.IsDBNull(reader["FechaActualizacion"]) ? DateTime.MinValue : Convert.ToDateTime(reader["FechaActualizacion"]);
                            oficina.ActualizadoPor = Convert.IsDBNull(reader["ActualizadoPor"]) ? 0 : Convert.ToInt32(reader["ActualizadoPor"]);
                            oficina.Estatus = Convert.IsDBNull(reader["Estatus"]) ? 0 : Convert.ToInt32(reader["Estatus"]);
                            oficina.estatusDesc = reader["estatusDesc"].ToString();


                            ListaOficinas.Add(oficina);
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
            return ListaOficinas;


        }

    }
}

