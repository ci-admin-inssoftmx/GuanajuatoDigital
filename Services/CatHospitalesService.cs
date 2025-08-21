using GuanajuatoAdminUsuarios.Interfaces;
using GuanajuatoAdminUsuarios.Models;
using System.Collections.Generic;
using System.Data;
using System;
using System.Data.SqlClient;
using GuanajuatoAdminUsuarios.Entity;
using DocumentFormat.OpenXml.Bibliography;

namespace GuanajuatoAdminUsuarios.Services
{
    public class CatHospitalesService : ICatHospitalesService
    {
        
            private readonly ISqlClientConnectionBD _sqlClientConnectionBD;
            public CatHospitalesService(ISqlClientConnectionBD sqlClientConnectionBD)
            {
                _sqlClientConnectionBD = sqlClientConnectionBD;
            }

            public List<CatHospitalesModel> GetHospitales(int corp)
            {
                //
                List<CatHospitalesModel> ListaHospitales = new List<CatHospitalesModel>();
			var corporation = corp < 2 ? 1 : corp;

			using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
                    try

                    {
                        connection.Open();
                        SqlCommand command = new SqlCommand(@"SELECT h.*, e.estatusDesc, m.Municipio 
                                                              FROM catHospitales AS h 
                                                              INNER JOIN estatus AS e ON h.estatus = e.estatus 
                                                              INNER JOIN catMunicipios AS m ON h.idMunicipio = m.idMunicipio 
                                                                where h.transito = @corp
                                                              ORDER BY NombreHospital ASC;", connection);
                        command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("corp", corporation);
                        using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                        {
                            while (reader.Read())
                            {
                            CatHospitalesModel hospital = new CatHospitalesModel();
                            hospital.IdHospital = reader["IdHospital"] != DBNull.Value ? Convert.ToInt32(reader["IdHospital"]) : 0;
                            hospital.Corp = reader["transito"] != DBNull.Value ? Convert.ToInt32(reader["transito"]) : 0;

                            hospital.IdMunicipio = reader["IdMunicipio"] != DBNull.Value ? Convert.ToInt32(reader["IdMunicipio"]) : 0;
                            hospital.NombreHospital = reader["NombreHospital"] != DBNull.Value ? reader["NombreHospital"].ToString() : "";
                            hospital.Estatus = reader["Estatus"] != DBNull.Value ? Convert.ToInt32(reader["Estatus"]) : 0;
                            hospital.Municipio = reader["Municipio"] != DBNull.Value ? reader["Municipio"].ToString() : "";
                            hospital.estatusDesc = reader["estatusDesc"] != DBNull.Value ? reader["estatusDesc"].ToString() : "";
                            hospital.FechaActualizacion = reader["FechaActualizacion"] != DBNull.Value ? Convert.ToDateTime(reader["FechaActualizacion"]) : DateTime.MinValue;
                            hospital.ActualizadoPor = reader["ActualizadoPor"] != DBNull.Value ? Convert.ToInt32(reader["ActualizadoPor"]) : 0;


                            ListaHospitales.Add(hospital);

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
                return ListaHospitales;


            }


        }

    }



