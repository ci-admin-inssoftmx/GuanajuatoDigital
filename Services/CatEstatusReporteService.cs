using GuanajuatoAdminUsuarios.Interfaces;
using GuanajuatoAdminUsuarios.Models;
using System.Collections.Generic;
using System.Data;
using System;
using System.Data.SqlClient;
using GuanajuatoAdminUsuarios.Interfaces;

namespace GuanajuatoAdminUsuarios.Services
{
    public class CatEstatusReporteService : ICatEstatusReporteService
    {
        private readonly ISqlClientConnectionBD _sqlClientConnectionBD;
        public CatEstatusReporteService(ISqlClientConnectionBD sqlClientConnectionBD)
        {
            _sqlClientConnectionBD = sqlClientConnectionBD;
        }
        public List<CatEstatusReporteModel> ObtenerEstatusReporte()
        {
            //
            List<CatEstatusReporteModel> ListaEstatus = new List<CatEstatusReporteModel>();

            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
                try

                {
                    connection.Open();
                    SqlCommand command = new SqlCommand("SELECT catEstatusReporteAccidente.*, estatus.estatusdesc FROM catEstatusReporteAccidente LEFT JOIN estatus ON catEstatusReporteAccidente.estatus = estatus.estatus;", connection);
                    command.CommandType = CommandType.Text;
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            CatEstatusReporteModel estatus = new CatEstatusReporteModel();
                            estatus.idEstatusReporte = Convert.ToInt32(reader["idEstatusReporte"].ToString());
                            estatus.estatusReporte = reader["estatusReporte"].ToString();
                            ListaEstatus.Add(estatus);

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
            return ListaEstatus;


        }
    }
}

