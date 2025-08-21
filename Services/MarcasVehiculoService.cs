using GuanajuatoAdminUsuarios.Entity;
using GuanajuatoAdminUsuarios.Interfaces;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;

namespace GuanajuatoAdminUsuarios.Services
{
    public class MarcasVehiculoService : IMarcasVehiculos
    {
        private readonly ISqlClientConnectionBD _sqlClientConnectionBD;
        public MarcasVehiculoService(ISqlClientConnectionBD sqlClientConnectionBD)
        {
            _sqlClientConnectionBD = sqlClientConnectionBD;
        }

        /// <summary>
        /// Consulta de Categorias con SQLClient
        /// </summary>
        /// <returns></returns>
        public List<MarcasVehiculo> GetMarcas()
        {
            //
            List<MarcasVehiculo> marcas = new List<MarcasVehiculo>();

            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
                try

                {
                    connection.Open();
                    SqlCommand command = new SqlCommand("Select * from catMarcasVehiculos where estatus=1 ORDER BY fechaActualizacion DESC", connection);
                    command.CommandType = CommandType.Text;
                    //sqlData Reader sirve para la obtencion de datos 
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            MarcasVehiculo marcasVehiculo = new MarcasVehiculo();
                            marcasVehiculo.IdMarcaVehiculo = Convert.ToInt32(reader["idMarcaVehiculo"].ToString());
                            marcasVehiculo.MarcaVehiculo = reader["marcaVehiculo"].ToString();
                            marcas.Add(marcasVehiculo);

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
                return marcas;


        }



        

        public MarcasVehiculo GetMarcayById(int IdMarcaVehiculo)
        {
            MarcasVehiculo marca = new MarcasVehiculo();
            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
                try
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand("Select * from catMarcasVehiculos where idMarcaVehiculo=@idMarcaVehiculo", connection);
                    command.Parameters.Add(new SqlParameter("@idMarcaVehiculo", SqlDbType.Int)).Value = IdMarcaVehiculo;
                    command.CommandType = CommandType.Text;
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            marca.IdMarcaVehiculo = Convert.ToInt32(reader["IdMarcaVehiculo"].ToString());
                            marca.MarcaVehiculo = reader["MarcaVehiculo"].ToString();
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

            return marca;
        }

        public int SaveMarca(MarcasVehiculo marca)
        {
            int result = 0;
            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
            {
                try
                {
                    connection.Open();
                    SqlCommand sqlCommand = new SqlCommand("Insert into catMarcasVehiculos(marcaVehiculo) values(@MarcaVehiculo)", connection);
                    sqlCommand.Parameters.Add(new SqlParameter("@MarcaVehiculo", SqlDbType.VarChar)).Value = marca.MarcaVehiculo;
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
        public int UpdateMarca(MarcasVehiculo marca)
        {
            int result = 0;
            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
            {
                try
                {
                    connection.Open();
                    SqlCommand sqlCommand = new
                        SqlCommand("Update marcasVehiculos set catMarcasVehiculos=@MarcaVehiculo where idMarcaVehiculo=@idMarcaVehiculo",
                        connection);
                    sqlCommand.Parameters.Add(new SqlParameter("@idMarcaVehiculo", SqlDbType.Int)).Value = marca.IdMarcaVehiculo;
                    sqlCommand.Parameters.Add(new SqlParameter("@MarcaVehiculo", SqlDbType.VarChar)).Value = marca.MarcaVehiculo;
                    sqlCommand.CommandType = CommandType.Text;
                    result = sqlCommand.ExecuteNonQuery();
                }
                catch (SqlException ex)
                {
                    //---Log
                    return result;
                }
                finally
                {
                    connection.Close();
                }
            }
            return result;
        }


        public int DeleteMarca(int IdMarca)
        {
            int result = 0;
            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
            {
                try
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand("Delete from catMarcasVehiculos where idMarcaVehiculo=@idMarcaVehiculo", connection);
                    command.Parameters.Add(new SqlParameter("@idMarcaVehiculo", SqlDbType.Int)).Value = IdMarca;
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
        }


    }
}
