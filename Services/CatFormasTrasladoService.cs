using GuanajuatoAdminUsuarios.Entity;
using GuanajuatoAdminUsuarios.Interfaces;
using GuanajuatoAdminUsuarios.Models;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;

namespace GuanajuatoAdminUsuarios.Services
{
    public class CatFormasTrasladoService : ICatFormasTrasladoService
    {
        private readonly ISqlClientConnectionBD _sqlClientConnectionBD;
        public CatFormasTrasladoService(ISqlClientConnectionBD sqlClientConnectionBD)
        {
            _sqlClientConnectionBD = sqlClientConnectionBD;
        }

        public List<CatFormasTrasladoModel> ObtenerFormasActivas()
        {
            //
            List<CatFormasTrasladoModel> forma = new List<CatFormasTrasladoModel>();

            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
                try

                {
                    connection.Open();
                    SqlCommand command = new SqlCommand("SELECT ft.*, e.estatusdesc FROM catFormasTraslado AS ft JOIN estatus AS e ON ft.estatus = e.estatus WHERE ft.estatus = 1 ", connection);
                    command.CommandType = CommandType.Text;
                    //sqlData Reader sirve para la obtencion de datos 
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            CatFormasTrasladoModel formatraslado = new CatFormasTrasladoModel();
                            formatraslado.idFormaTraslado = Convert.ToInt32(reader["idFormaTraslado"].ToString());
                            formatraslado.formaTraslado = reader["formaTraslado"].ToString();
                            //formatraslado.FechaActualizacion = Convert.ToDateTime(reader["FechaActualizacion"].ToString());
                            //formatraslado.ActualizadoPor = Convert.ToInt32(reader["ActualizadoPor"].ToString());
                            formatraslado.estatus = Convert.ToInt32(reader["estatus"].ToString());
                            formatraslado.estatusDesc = reader["estatusDesc"].ToString();
                            forma.Add(formatraslado);
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
            return forma;


        }

    }
}
