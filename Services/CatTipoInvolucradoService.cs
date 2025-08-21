using GuanajuatoAdminUsuarios.Interfaces;
using GuanajuatoAdminUsuarios.Models;
using System.Collections.Generic;
using GuanajuatoAdminUsuarios.Interfaces;
using GuanajuatoAdminUsuarios.Models;
using System.Collections.Generic;
using System.Data;
using System;
using System.Data.SqlClient;
using GuanajuatoAdminUsuarios.Entity;
using Microsoft.Extensions.DependencyModel;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace GuanajuatoAdminUsuarios.Services
{
    public class CatTipoInvolucradoService : ICatTipoInvolucradoService
    {
        private readonly ISqlClientConnectionBD _sqlClientConnectionBD;
        public CatTipoInvolucradoService(ISqlClientConnectionBD sqlClientConnectionBD)
        {
            _sqlClientConnectionBD = sqlClientConnectionBD;
        }
        public List<CatTipoInvolucradoModel> ObtenerTipos()
        {
            //
            List<CatTipoInvolucradoModel> ListaTipos = new List<CatTipoInvolucradoModel>();

            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
                try

                {
                    connection.Open();
                    SqlCommand command = new SqlCommand("SELECT catTipoInvolucrado.*, estatus.estatusdesc FROM catTipoInvolucrado JOIN estatus ON catTipoInvolucrado.estatus = estatus.estatus", connection);
                    command.CommandType = CommandType.Text;
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            CatTipoInvolucradoModel tipo = new CatTipoInvolucradoModel();
                            tipo.IdTipoInvolucrado = Convert.ToInt32(reader["IdTipoInvolucrado"].ToString());
                            tipo.TipoInvolucrado = reader["TipoInvolucrado"].ToString();
                            ListaTipos.Add(tipo);

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
            return ListaTipos;


        }
    }
}
