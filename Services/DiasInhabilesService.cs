using GuanajuatoAdminUsuarios.Interfaces;
using GuanajuatoAdminUsuarios.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace GuanajuatoAdminUsuarios.Services
{
    public class DiasInhabilesService : IDiasInhabiles
    { 
    private readonly ISqlClientConnectionBD _sqlClientConnectionBD;
    public DiasInhabilesService(ISqlClientConnectionBD sqlClientConnectionBD)
    {
        _sqlClientConnectionBD = sqlClientConnectionBD;
    }

    /// <summary>
    /// Consulta de Categorias con SQLClient
    /// </summary>
    /// <returns></returns>
    public List<DiasInhabilesModel> GetDiasInhabiles(int corp)
    {
        //
        List<DiasInhabilesModel> diasList = new List<DiasInhabilesModel>();
			var corporation = corp < 2 ? 1 : corp;
			using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
            try

            {
                connection.Open();
                SqlCommand command = new SqlCommand (@"Select ctd.idDiaInhabil, ctd.fecha,ctd.idMunicipio,ctd.todosMunicipiosDesc,ctd.estatus,
		                                            m.municipio,e.estatusDesc
		                                            from catDiasInhabiles ctd 
		                                            LEFT JOIN catMunicipios m ON m.idMunicipio = ctd.idMunicipio
		                                            LEFT JOIN estatus e ON e.estatus = ctd.estatus 
                                                    where ctd.transito = @corp
                                                    order by ctd.idDiaInhabil desc", connection);
                command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("corp", corporation);
                //sqlData Reader sirve para la obtencion de datos 
                using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                {
                    while (reader.Read())
                    {
                            DiasInhabilesModel dia = new DiasInhabilesModel();
                            dia.idDiaInhabil = Convert.ToInt32(reader["idDiaInhabil"].ToString());
                            dia.idMunicipio = Convert.ToInt32(reader["idMunicipio"].ToString());
                            if (reader["fecha"] != DBNull.Value)
                            {
                                if (DateTime.TryParse(reader["fecha"].ToString(), out DateTime parsedDate))
                                {
                                    dia.fecha = parsedDate; 
                                }
                                else
                                {
                              
                                   dia.fecha = DateTime.MinValue;
                                }
                            }
                            dia.todosMunicipiosDesc = reader["todosMunicipiosDesc"].ToString();
                            dia.Municipio = reader["municipio"].ToString();
                            dia.EstatusDesc = reader["estatusDesc"].ToString();



                            diasList.Add(dia);

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
        return diasList;


    }
   
    public DiasInhabilesModel GetDiasById(int IdDia)
    {
            DiasInhabilesModel dia = new DiasInhabilesModel();
        using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
            try
            {
                connection.Open();
                SqlCommand command = new SqlCommand(@"Select ctd.idDiaInhabil, ctd.fecha,ctd.idMunicipio,ctd.todosMunicipiosDesc,ctd.estatus,
		                                            m.municipio,e.estatusDesc
		                                            from catDiasInhabiles ctd 
		                                            LEFT JOIN catMunicipios m ON m.idMunicipio = ctd.idMunicipio
		                                            LEFT JOIN estatus e ON e.estatus = ctd.estatus
                                                    WHERE ctd.idDiaInhabil= @idDiaInhabil", connection);
                command.Parameters.Add(new SqlParameter("@idDiaInhabil", SqlDbType.Int)).Value = IdDia;
                command.CommandType = CommandType.Text;
                using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                {
                    while (reader.Read())
                    {
                            dia.idDiaInhabil = Convert.ToInt32(reader["idDiaInhabil"].ToString());
                            dia.idMunicipio = Convert.ToInt32(reader["idMunicipio"].ToString());
                            dia.Estatus = Convert.ToInt32(reader["estatus"].ToString());
                            dia.fecha = Convert.ToDateTime(reader["fecha"].ToString());
                            dia.todosMunicipiosDesc = reader["todosMunicipiosDesc"].ToString();
                            dia.Municipio = reader["municipio"].ToString();
                            dia.EstatusDesc = reader["estatusDesc"].ToString();

                        }
                }
            }
            catch (Exception ex)
            {
            }
            finally
            {
                connection.Close();
            }

        return dia;
    }

    public int CrearDiaInhabil(DiasInhabilesModel Dia,int corp)
    {
        int result = 0;
			var corporation = corp < 2 ? 1 : corp;
			using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
        {
            try
            {
                connection.Open();
                SqlCommand sqlCommand = new SqlCommand(@"Insert into catDiasInhabiles
                                                      (fecha,idMunicipio,todosMunicipiosDesc, estatus,fechaActualizacion,actualizadoPor,transito) 
                                                     values(@Fecha,@idMunicipio,@todosMunicipiosDesc,@estatus,@fechaActualizacion,@actualizadoPor,@corp)", connection);
                    sqlCommand.Parameters.Add(new SqlParameter("@fecha", SqlDbType.DateTime)).Value = Dia.fecha;
                    sqlCommand.Parameters.Add(new SqlParameter("@idMunicipio", SqlDbType.Int)).Value = Dia.idMunicipio;
                    sqlCommand.Parameters.Add(new SqlParameter("@todosMunicipiosDesc", SqlDbType.VarChar)).Value = Dia.todosMunicipiosDesc;
                    sqlCommand.Parameters.Add(new SqlParameter("@estatus", SqlDbType.Int)).Value = 1;
                    sqlCommand.Parameters.Add(new SqlParameter("@fechaActualizacion", SqlDbType.DateTime)).Value = DateTime.Now;
                    sqlCommand.Parameters.Add(new SqlParameter("@actualizadoPor", SqlDbType.Int)).Value = 1;
                    sqlCommand.Parameters.AddWithValue("corp", corporation);

                    sqlCommand.CommandType = CommandType.Text;
                result = sqlCommand.ExecuteNonQuery();
            }
            catch (SqlException ex)
            {
                return result;
            }
            finally
            {
                connection.Close();
            }
        }
        return result;

    }
    public int EditDia(DiasInhabilesModel Dia)
    {
        int result = 0;
        using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
        {
            try
            {
                connection.Open();
                SqlCommand sqlCommand = new
                    SqlCommand("Update catDiasInhabiles set fecha = @fecha, idMunicipio=@idMunicipio, todosMunicipiosDesc=@todosMunicipiosDesc, estatus = @estatus where idDiaInhabil=@idDiaInhabil",
                    connection);
                sqlCommand.Parameters.Add(new SqlParameter("@idDiaInhabil", SqlDbType.Int)).Value = Dia.idDiaInhabil;
                sqlCommand.Parameters.Add(new SqlParameter("@fecha", SqlDbType.DateTime)).Value = Dia.fecha;
                sqlCommand.Parameters.Add(new SqlParameter("@idMunicipio", SqlDbType.Int)).Value = Dia.idMunicipio;
                sqlCommand.Parameters.Add(new SqlParameter("@todosMunicipiosDesc", SqlDbType.VarChar)).Value = Dia.todosMunicipiosDesc;
                sqlCommand.Parameters.Add(new SqlParameter("@estatus", SqlDbType.Int)).Value = Dia.Estatus;

                sqlCommand.CommandType = CommandType.Text;
                result = sqlCommand.ExecuteNonQuery();
            }
            catch (SqlException ex)
            {
                return result;
            }
            finally
            {
                connection.Close();
            }
        }
        return result;
    }


   /* public int DeleteOficial(int IdOficial)
    {
        int result = 0;
        using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
        {
            try
            {
                connection.Open();
                SqlCommand command = new SqlCommand("Delete from oficiales where idOficial=@idOficial", connection);
                command.Parameters.Add(new SqlParameter("@idOficial", SqlDbType.Int)).Value = IdOficial;
                command.CommandType = CommandType.Text;
                result = command.ExecuteNonQuery();

            }
            catch (Exception ex)
            {
                return result;
            }
            finally
            {
                connection.Close();
            }
        }
        return result;
    }*/


}
}

