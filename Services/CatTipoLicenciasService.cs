using GuanajuatoAdminUsuarios.Interfaces;
using GuanajuatoAdminUsuarios.Models;
using System.Collections.Generic;
using System.Data;
using System;
using System.Data.SqlClient;
namespace GuanajuatoAdminUsuarios.Services
{
    public class CatTipoLicenciasService : ICatTipoLicenciasService
    {
        private readonly ISqlClientConnectionBD _sqlClientConnectionBD;
        public CatTipoLicenciasService(ISqlClientConnectionBD sqlClientConnectionBD)
        {
            _sqlClientConnectionBD = sqlClientConnectionBD;
        }
        public List<CatTipoLicenciasModel> ObtenerTiposLicencia()
        {
            //
            List<CatTipoLicenciasModel> ListaLicencias = new List<CatTipoLicenciasModel>();

            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
                try

                {
                    connection.Open();
                    SqlCommand command = new SqlCommand("SELECT catTipoLicencia.*, estatus.estatusdesc FROM catTipoLicencia JOIN estatus ON catTipoLicencia.estatus = estatus.estatus;", connection);
                    command.CommandType = CommandType.Text;
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            CatTipoLicenciasModel asiento = new CatTipoLicenciasModel();
                            asiento.idTipoLicencia = Convert.ToInt32(reader["idTipoLicencia"].ToString());
                            asiento.tipoLicencia = reader["tipoLicencia"].ToString().ToUpper();
                            ListaLicencias.Add(asiento);

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
            return ListaLicencias;


        }
    }
}

