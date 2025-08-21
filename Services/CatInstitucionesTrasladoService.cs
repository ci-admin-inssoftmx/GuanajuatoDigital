using GuanajuatoAdminUsuarios.Interfaces;
using GuanajuatoAdminUsuarios.Models;
using System.Collections.Generic;
using System.Data;
using System;
using System.Data.SqlClient;

namespace GuanajuatoAdminUsuarios.Services
{
    public class CatInstitucionesTrasladoService : ICatInstitucionesTrasladoService
    {
        private readonly ISqlClientConnectionBD _sqlClientConnectionBD;
        public CatInstitucionesTrasladoService(ISqlClientConnectionBD sqlClientConnectionBD)
        {
            _sqlClientConnectionBD = sqlClientConnectionBD;
        }
        public List<CatInstitucionesTrasladoModel> ObtenerInstitucionesActivas(int corp)
        {
            //
            List<CatInstitucionesTrasladoModel> ListaInstituciones = new List<CatInstitucionesTrasladoModel>();

            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
                try

                {
                    connection.Open();
                    SqlCommand command = new SqlCommand("SELECT catInstitucionesTraslado.*, estatus.estatusdesc FROM catInstitucionesTraslado JOIN estatus ON catInstitucionesTraslado.estatus = estatus.estatus" +
                        " WHERE catInstitucionesTraslado.estatus = 1 AND transito = @corp ORDER BY InstitucionTraslado ASC ", connection);
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("@corp", corp);

                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            CatInstitucionesTrasladoModel institucion = new CatInstitucionesTrasladoModel();
                            institucion.IdInstitucionTraslado = Convert.ToInt32(reader["IdInstitucionTraslado"].ToString());
                            institucion.InstitucionTraslado = reader["InstitucionTraslado"].ToString();
                            ListaInstituciones.Add(institucion);

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
            return ListaInstituciones;


        }
        public List<CatInstitucionesTrasladoModel> ObtenerInstituciones(int corp)
        {
            //
            List<CatInstitucionesTrasladoModel> ListaInstituciones = new List<CatInstitucionesTrasladoModel>();

            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
                try

                {
                    connection.Open();
                    SqlCommand command = new SqlCommand("SELECT catInstitucionesTraslado.*, estatus.estatusdesc FROM catInstitucionesTraslado JOIN estatus ON catInstitucionesTraslado.estatus = estatus.estatus" +
                        " WHERE transito = @corp ORDER BY InstitucionTraslado ASC ", connection);
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("@corp", corp);

                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            CatInstitucionesTrasladoModel institucion = new CatInstitucionesTrasladoModel();
                            institucion.IdInstitucionTraslado = Convert.ToInt32(reader["IdInstitucionTraslado"].ToString());
                            institucion.InstitucionTraslado = reader["InstitucionTraslado"].ToString();
                            ListaInstituciones.Add(institucion);

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
            return ListaInstituciones;


        }
    }
}
