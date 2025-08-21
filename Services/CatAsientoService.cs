using GuanajuatoAdminUsuarios.Interfaces;
using GuanajuatoAdminUsuarios.Models;
using System.Collections.Generic;
using System.Data;
using System;
using System.Data.SqlClient;

namespace GuanajuatoAdminUsuarios.Services
{
    public class CatAsientoService : ICatAsientoService
    {
        private readonly ISqlClientConnectionBD _sqlClientConnectionBD;
        public CatAsientoService(ISqlClientConnectionBD sqlClientConnectionBD)
        {
            _sqlClientConnectionBD = sqlClientConnectionBD;
        }
        public List<CatAsientoModel> ObtenerAsientos()
        {
            //
            List<CatAsientoModel> ListaAsientos = new List<CatAsientoModel>();

            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
                try

                {
                    connection.Open();
                    SqlCommand command = new SqlCommand("SELECT catAsientos.*, estatus.estatusdesc FROM catAsientos JOIN estatus ON catAsientos.estatus = estatus.estatus;", connection);
                    command.CommandType = CommandType.Text;
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            CatAsientoModel asiento = new CatAsientoModel();
                            asiento.IdAsiento = Convert.ToInt32(reader["IdAsiento"].ToString());
                            asiento.Asiento = reader["Asiento"].ToString();
                            ListaAsientos.Add(asiento);

                        }

                    }

                }
                catch (SqlException ex)
                {
                   
                }
                finally
                {
                    connection.Close();
                }
            return ListaAsientos;


        }
    }
}
