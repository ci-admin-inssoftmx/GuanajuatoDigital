using GuanajuatoAdminUsuarios.Interfaces;
using GuanajuatoAdminUsuarios.Models;
using System.Collections.Generic;
using System.Data;
using System;
using System.Data.SqlClient;
namespace GuanajuatoAdminUsuarios.Services
{
    public class TiposCargaService : ITiposCarga
    {
        private readonly ISqlClientConnectionBD _sqlClientConnectionBD;
        public TiposCargaService(ISqlClientConnectionBD sqlClientConnectionBD)
        {
            _sqlClientConnectionBD = sqlClientConnectionBD;
        }

        public List<TiposCargaModel> GetTiposCarga()
        {
            //
            List<TiposCargaModel> ListaTiposCarga = new List<TiposCargaModel>();

            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
                try

                {
                    connection.Open();
                    SqlCommand command = new SqlCommand("SELECT tc.*, e.estatusdesc FROM catTiposcarga AS tc INNER JOIN estatus AS e ON tc.estatus = e.estatus;", connection);
                    command.CommandType = CommandType.Text;
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            TiposCargaModel tipoCarga = new TiposCargaModel();
                            tipoCarga.IdTipoCarga = Convert.ToInt32(reader["IdTipoCarga"].ToString());
                            tipoCarga.TipoCarga = reader["TipoCarga"].ToString();
                            tipoCarga.Estatus = Convert.ToInt32(reader["estatus"].ToString());
                            tipoCarga.EstatusDesc = reader["estatusDesc"].ToString();
                            //tipoCarga.FechaActualizacion = Convert.ToDateTime(reader["FechaActualizacion"].ToString());
                            tipoCarga.Estatus = Convert.ToInt32(reader["estatus"].ToString());
                            tipoCarga.ActualizadoPor = !Convert.IsDBNull(reader["ActualizadoPor"])
                                ? Convert.ToInt32(reader["ActualizadoPor"])
                                : 0; // O cualquier valor predeterminado que desees asignar en caso de que sea null
                            ListaTiposCarga.Add(tipoCarga);

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
            return ListaTiposCarga;


        }
        public List<TiposCargaModel> GetTiposCargaActivos()
        {
            //
            List<TiposCargaModel> ListaTiposCarga = new List<TiposCargaModel>();

            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
                try

                {
                    connection.Open();
                    SqlCommand command = new SqlCommand("SELECT tc.*, e.estatusdesc FROM catTiposcarga AS tc INNER JOIN estatus AS e ON tc.estatus = e.estatus WHERE tc.estatus = 1;", connection);
                    command.CommandType = CommandType.Text;
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            TiposCargaModel tipoCarga = new TiposCargaModel();
                            tipoCarga.IdTipoCarga = Convert.ToInt32(reader["idTipoCarga"].ToString());
                            tipoCarga.TipoCarga = reader["tipoCarga"].ToString();
                            tipoCarga.Estatus = Convert.ToInt32(reader["estatus"].ToString());
                            tipoCarga.EstatusDesc = reader["estatusDesc"].ToString();
                            //tipoCarga.FechaActualizacion = Convert.ToDateTime(reader["FechaActualizacion"].ToString());
                            tipoCarga.ActualizadoPor = Convert.ToInt32(reader["ActualizadoPor"].ToString());
                            ListaTiposCarga.Add(tipoCarga);

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
            return ListaTiposCarga;


        }
    }
}

