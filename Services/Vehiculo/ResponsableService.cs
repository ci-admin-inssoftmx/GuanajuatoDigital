using GuanajuatoAdminUsuarios.Interfaces;
using GuanajuatoAdminUsuarios.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace GuanajuatoAdminUsuarios.Services
{
    public class ResponsableService : IResponsableService
    {
        private readonly ISqlClientConnectionBD _sqlClientConnectionBD;
        public ResponsableService(ISqlClientConnectionBD sqlClientConnectionBD)
        {
            _sqlClientConnectionBD = sqlClientConnectionBD;
        }

        public List<ResponsableModel> GetResponsables()
        {
            List<ResponsableModel> responsables = new List<ResponsableModel>();

            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
                try

                {
                    connection.Open();
                    SqlCommand command = new SqlCommand("Select * from catResponsablePensiones where estatus=1", connection);
                    command.CommandType = CommandType.Text;
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            ResponsableModel responsable = new ResponsableModel();
                            responsable.idResponsable = Convert.ToInt32(reader["idResponsable"].ToString());
                            responsable.responsable = reader["responsable"].ToString();
                            responsables.Add(responsable);
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
            return responsables;
        }
    }
}
