using GuanajuatoAdminUsuarios.Interfaces;
using GuanajuatoAdminUsuarios.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace GuanajuatoAdminUsuarios.Services
{
    public class TipoCortesiaService : ITipoCortesiaService
    {
        private readonly ISqlClientConnectionBD _sqlClientConnectionBD;
        public TipoCortesiaService(ISqlClientConnectionBD sqlClientConnectionBD)
        {
            _sqlClientConnectionBD = sqlClientConnectionBD;
        }

        public List<TipoCortesiaModel> GetTipoCortesias()
        {
            List<TipoCortesiaModel> ListTipoCortesias = new List<TipoCortesiaModel>();

            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
                try

                {
                    connection.Open();
                    SqlCommand command = new SqlCommand("Select * from catTipoCortesia where estatus=1", connection);
                    command.CommandType = CommandType.Text;
                    //sqlData Reader sirve para la obtencion de datos 
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            TipoCortesiaModel tipoCortesia = new TipoCortesiaModel();
                            tipoCortesia.idTipoCortesia = Convert.ToInt32(reader["id"].ToString());
                            tipoCortesia.tipoCortesia = reader["nombreCortesia"].ToString().ToUpper();
                            ListTipoCortesias.Add(tipoCortesia);
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
            return ListTipoCortesias;
        }

    }
}
