using GuanajuatoAdminUsuarios.Interfaces;
using GuanajuatoAdminUsuarios.Models;
using Kendo.Mvc.UI;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using static GuanajuatoAdminUsuarios.RESTModels.ConsultarDocumentoResponseModel;
using System.Data;
//using Telerik.SvgIcons;
using System.Data.SqlClient;
using GuanajuatoAdminUsuarios.Models.Generales;
using System.Linq;
using GuanajuatoAdminUsuarios.Helpers;
using GuanajuatoAdminUsuarios.Entity;

namespace GuanajuatoAdminUsuarios.Services
{
    public class AdminBlocksService : IAdminBlocksService
    {
        private readonly ISqlClientConnectionBD _sqlClientConnectionBD;
        private readonly IHttpContextAccessor _Http;
        public AdminBlocksService(ISqlClientConnectionBD sqlClientConnectionBD , IHttpContextAccessor http)
        {
            _sqlClientConnectionBD = sqlClientConnectionBD;
            _Http = http;
        }
        public List<AdminBlocksModel> GetdataGrid()
        {

            var result = new List<AdminBlocksModel>();

            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
                try

                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(@"select idAdminBlocks,Corporacion,prefijoInfracciones,accidentes
                                                          ,infracciones,depositos  from catAdminBlocks a
                                                          join CatCorporaciones b on a.idcorporacion=b.IdCorporacion", connection);
                    command.CommandType = System.Data.CommandType.Text;
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            AdminBlocksModel service = new AdminBlocksModel();
                            service.Id = Convert.ToInt32(reader["idAdminBlocks"]);
                            service.Corporacion = Convert.ToString(reader["Corporacion"]);
                            service.prefijoInfracciones = Convert.ToString(reader["prefijoInfracciones"]);
                            service.accidentes = Convert.ToBoolean(reader["accidentes"])? EstatusOperacion.ACTIVO : EstatusOperacion.INACTIVO;
                            service.infraccion = Convert.ToBoolean(reader["infracciones"]) ? EstatusOperacion.ACTIVO : EstatusOperacion.INACTIVO;
                            service.depositos = Convert.ToBoolean(reader["depositos"]) ? EstatusOperacion.ACTIVO : EstatusOperacion.INACTIVO;

                            result.Add(service);

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



            return result;

        }
        public AdminBlocksViewModel Getdata(int IdAlertamiento)
        {
            var service = new AdminBlocksViewModel();

            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
                try
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(@"select idAdminBlocks,Corporacion,prefijoInfracciones,accidentes
                                                          ,infracciones,depositos  from catAdminBlocks a
                                                          join CatCorporaciones b on a.idcorporacion=b.IdCorporacion
                            where a.idAdminBlocks=@corp", connection);
                    command.CommandType = System.Data.CommandType.Text;
                    command.Parameters.AddWithValue("@corp", IdAlertamiento);
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            service.Id = Convert.ToInt32(reader["idAdminBlocks"]);
                            service.Corporacion = Convert.ToString(reader["Corporacion"]);
                            service.prefijoInfracciones = Convert.ToString(reader["prefijoInfracciones"]);
                            service.accidentes = Convert.ToBoolean(reader["accidentes"]) ;
                            service.infraccion = Convert.ToBoolean(reader["infracciones"]) ;
                            service.depositos = Convert.ToBoolean(reader["depositos"]) ;
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

            return service;
        }
        List<int> getFoliosCorpBlocks()
        {
            var result = new List<int>();

            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
                try

                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(@"select idcorporacion from catAdminBlocks a", connection);
                    command.CommandType = System.Data.CommandType.Text;
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            int service = default;
                            service = Convert.ToInt32(reader["idcorporacion"]);
                            result.Add(service);

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

            return result;
        }
         List<CatalogModel> getCorporaciones()
        {
            var result = new List<CatalogModel>();

            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
                try
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(@"select IdCorporacion,Corporacion from CatCorporaciones ", connection);
                    command.CommandType = System.Data.CommandType.Text;
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            CatalogModel service = new CatalogModel();
                            service.text = Convert.ToString(reader["Corporacion"]);
                            service.value = Convert.ToString(reader["idcorporacion"]);
                            result.Add(service);
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

            return result;
        }
        public List<CatalogModel> GetDropDownCorporaciones()
        {
            var corpUsadas = getFoliosCorpBlocks();
            var catalogocorp = getCorporaciones();

            var result  = catalogocorp.Where(s=>!corpUsadas.Contains(s.value.toInt())).ToList();

            return result;


        }

        public void CrearBlockCorporaciones(int Corporaciones, string prefijoInfracciones, bool infracciones, bool accidente, bool Depositos)
        {
            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
                try
                {

                    int t = 0; 
                        int.TryParse(_Http.HttpContext.User.FindFirst(CustomClaims.IdUsuario).Value,out t);

                    connection.Open();
                    SqlCommand command = new SqlCommand(@"insert into catAdminBlocks (idcorporacion,prefijoInfracciones,accidentes,infracciones,depositos,actualizadoPor) values
                                            (@corp,@idpref,@acc,@inf,@dep,@usr)", connection);
                    command.CommandType = System.Data.CommandType.Text;
                    command.Parameters.AddWithValue("@corp", Corporaciones);
                    command.Parameters.AddWithValue("@idpref", prefijoInfracciones??"");
                    command.Parameters.AddWithValue("@acc", accidente);
                    command.Parameters.AddWithValue("@inf", infracciones);
                    command.Parameters.AddWithValue("@dep", Depositos);
                    command.Parameters.AddWithValue("@usr", t);
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {

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
        }

        public void EditarBlockCorporaciones(int Id, string prefijoInfracciones, bool infracciones, bool accidente, bool Depositos)
        {
            int t = 0;
            int.TryParse(_Http.HttpContext.User.FindFirst(CustomClaims.IdUsuario).Value, out t);
            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
                try
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(@"update a set 
                            a.prefijoInfracciones = @pref,a.accidentes = @acc,a.infracciones = @inf,a.depositos = @dep , 
                            a.fechaActualizacion = getdate() , actualizadoPor =@usr
                            from [dbo].[catAdminBlocks] a                            
                            where a.idAdminBlocks=@corp", connection);
                    command.CommandType = System.Data.CommandType.Text;
                    command.Parameters.AddWithValue("@corp", Id);
                    command.Parameters.AddWithValue("@pref", prefijoInfracciones??"");
                    command.Parameters.AddWithValue("@acc", accidente);
                    command.Parameters.AddWithValue("@inf", infracciones);
                    command.Parameters.AddWithValue("@dep", Depositos);
                    command.Parameters.AddWithValue("@usr", t);
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {

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

        }


        public (bool can, string pref) GetPermisos(string permiso)
        {
            var can = false;
            var prefijo = "";
            var service = new AdminBlocksViewModel();
            var t = _Http.HttpContext.User.FindFirst(CustomClaims.TipoOficina).Value;



            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
                try
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(@"select idAdminBlocks,prefijoInfracciones,accidentes
                                                          ,infracciones,depositos  from catAdminBlocks a
                                                          
                            where a.idcorporacion=@corp", connection);
                    command.CommandType = System.Data.CommandType.Text;
                    command.Parameters.AddWithValue("@corp", t);
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            service.Id = Convert.ToInt32(reader["idAdminBlocks"]);
                            service.prefijoInfracciones = Convert.ToString(reader["prefijoInfracciones"]);
                            service.accidentes = Convert.ToBoolean(reader["accidentes"]);
                            service.infraccion = Convert.ToBoolean(reader["infracciones"]);
                            service.depositos = Convert.ToBoolean(reader["depositos"]);
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


            if(service.Id != null && service.Id > 0)
            {
                if(BlocksOperacion.ACCIDENTES == permiso)
                {
                    can = service.accidentes;    
                }
                if (BlocksOperacion.DEPOSITOS == permiso)
                {
                    can = service.depositos;
                }
                if (BlocksOperacion.INFRACCIONES == permiso)
                {
                    can = service.infraccion;
                    prefijo = service.prefijoInfracciones??"";
                }
            }


            return (can: can, pref: prefijo);
        }

    }
}
