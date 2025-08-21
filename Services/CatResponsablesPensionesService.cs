using GuanajuatoAdminUsuarios.Interfaces;
using GuanajuatoAdminUsuarios.Models;
using System.Collections.Generic;
using System.Data;
using System;
using System.Data.SqlClient;

namespace GuanajuatoAdminUsuarios.Services
{
    public class CatResponsablesPensionesService : ICatResponsablesPensiones
    {
        private readonly ISqlClientConnectionBD _sqlClientConnectionBD;
        public CatResponsablesPensionesService(ISqlClientConnectionBD sqlClientConnectionBD)
        {
            _sqlClientConnectionBD = sqlClientConnectionBD;
        }

        public List<CatResponsablesPensionesModel> ObtenerResponsables()
        {
            //
            List<CatResponsablesPensionesModel> ListaResponsables = new List<CatResponsablesPensionesModel>();

            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
                try

                {
                    connection.Open();
                    SqlCommand command = new SqlCommand("SELECT catResponsablePensiones.*, estatus.estatusdesc FROM catResponsablePensiones JOIN estatus ON catResponsablePensiones.estatus = estatus.estatus" +
                        " WHERE catResponsablePensiones.estatus = 1 ORDR BY responsable ASC", connection);
                    command.CommandType = CommandType.Text;
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            CatResponsablesPensionesModel responsable = new CatResponsablesPensionesModel();
                            responsable.IdResponsable = Convert.IsDBNull(reader["IdResponsable"]) ? 0 : Convert.ToInt32(reader["IdResponsable"]);
                            responsable.Responsable = reader["responsable"].ToString();
                            responsable.FechaActualizacion = Convert.IsDBNull(reader["FechaActualizacion"]) ? DateTime.MinValue : Convert.ToDateTime(reader["FechaActualizacion"]);
                            responsable.ActualizadoPor = Convert.IsDBNull(reader["ActualizadoPor"]) ? 0 : Convert.ToInt32(reader["ActualizadoPor"]);
                            responsable.Estatus = Convert.IsDBNull(reader["Estatus"]) ? 0 : Convert.ToInt32(reader["Estatus"]);
                            responsable.estatusDesc = reader["estatusDesc"].ToString();


                            ListaResponsables.Add(responsable);
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
            return ListaResponsables;


        }

    }

}

