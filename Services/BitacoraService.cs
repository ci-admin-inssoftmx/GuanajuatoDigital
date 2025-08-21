using GuanajuatoAdminUsuarios.Interfaces;
using GuanajuatoAdminUsuarios.RESTModels;
using Newtonsoft.Json;
using System.Data;
using System;
using System.Runtime.InteropServices;

using System.Data.SqlClient;
using GuanajuatoAdminUsuarios.Models;
using System.Collections.Generic;
using PdfSharp.Pdf.Content.Objects;
using System.Globalization;
using DocumentFormat.OpenXml.Office2010.Excel;
using Microsoft.AspNetCore.Http;

namespace GuanajuatoAdminUsuarios.Services
{
    public class BitacoraService : IBitacoraService
    {

        private readonly ISqlClientConnectionBD _sqlClientConnectionBD;
        private readonly IHttpContextAccessor _Http;

        public BitacoraService(ISqlClientConnectionBD sqlClientConnectionBD, IHttpContextAccessor Http)
        {
            _sqlClientConnectionBD = sqlClientConnectionBD;
            _Http = Http;
        }






        public void insertBitacora(decimal id, string ip, string textoCamb, string operacion, string consulta, decimal operador)
        {
            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))

                try
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(@"insert into bitacoradeinfracciones 
                        (BDIIDINFRACCION,
                        BDIFECHA,
                        BDIHORA,
                        BDIIP,
                        BDITEXTOCAMBIO,
                        BDIOPERACION,
                        BDICONSULTA,
                        BDIIDUSUARIO
                        ) values
                        ( @id ,FORMAT(getdate(),'yyyy-MM-dd'),FORMAT(getdate(),'HH:mm'),@ip,@textoCamb,@operacion,@consulta,@capturista)"
                    , connection);


                    command.Parameters.Add(new SqlParameter("@id", SqlDbType.Decimal)).Value = id;
                    command.Parameters.Add(new SqlParameter("@ip", SqlDbType.VarChar)).Value = ip;
                    command.Parameters.Add(new SqlParameter("@textoCamb", SqlDbType.VarChar)).Value = textoCamb;
                    command.Parameters.Add(new SqlParameter("@operacion", SqlDbType.VarChar)).Value = operacion;
                    command.Parameters.Add(new SqlParameter("@consulta", SqlDbType.VarChar)).Value = consulta;
                    command.Parameters.Add(new SqlParameter("@capturista", SqlDbType.Decimal)).Value = operador;

                    command.CommandType = CommandType.Text;
                    SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection);
                }
                catch (Exception ex)
                {

                }
                finally
                {
                    connection.Close();
                }




        }

        public async void insertBitacora(decimal id, string ip, string textoCamb, string operacion, string consulta, decimal operador, Object objecto)
        {
            var Objectt = JsonConvert.SerializeObject(objecto);
            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))

                try
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(@"insert into bitacoradeinfracciones 
                        (BDIIDINFRACCION,
                        BDIFECHA,
                        BDIHORA,
                        BDIIP,
                        BDITEXTOCAMBIO,
                        BDIOPERACION,
                        BDICONSULTA,
                        BDIIDUSUARIO,
                        BDOBJETO) values
                        ( @id ,FORMAT(getdate(),'yyyy-MM-dd'),FORMAT(getdate(),'HH:mm'),@ip,@textoCamb,@operacion,@consulta,@capturista, @object)"
                    , connection);


                    command.Parameters.Add(new SqlParameter("@id", SqlDbType.Decimal)).Value = id;
                    command.Parameters.Add(new SqlParameter("@ip", SqlDbType.VarChar)).Value = ip;
                    command.Parameters.Add(new SqlParameter("@textoCamb", SqlDbType.VarChar)).Value = textoCamb;
                    command.Parameters.Add(new SqlParameter("@operacion", SqlDbType.VarChar)).Value = operacion;
                    command.Parameters.Add(new SqlParameter("@consulta", SqlDbType.VarChar)).Value = consulta;
                    command.Parameters.Add(new SqlParameter("@capturista", SqlDbType.Decimal)).Value = operador;
                    command.Parameters.Add(new SqlParameter("@object", SqlDbType.VarChar)).Value = Objectt ?? " ";

                    command.CommandType = CommandType.Text;
                    SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection);
                }
                catch (Exception ex)
                {

                }
                finally
                {
                    connection.Close();
                }




        }





        public List<BitacoraInfraccionesModel> getBitacoraData(string id, string nombre)
        {

            var result = new List<BitacoraInfraccionesModel>();


            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))

                try
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(@"
                                            select  BDIOPERACION,BDIFECHA,BDIHORA,BDIIP,b.folioInfraccion
                                            ,b.documento,b.monto,c.nombre, BDITEXTOCAMBIO,d.NOMBRE_COMPLETO nombreUsuario
											from bitacoradeinfracciones a
											join infracciones b on b.idInfraccion=BDIIDINFRACCION
											left join cattipocambio c on c.id = a.BDIIDTIPOCAMBIO
											left join 
											(
											select aa.* from 
											TESTDATA.[GUANAJUATO_ADMIN_USUARIOS].dbo.cat_usuarios as  aa
											join TESTDATA.[GUANAJUATO_ADMIN_USUARIOS].dbo.seguridad as bb
											on aa.id = bb.id_usuario and bb.id_sistema=2
											)
											d on d.id=a.BDIIDUSUARIO
											where BDIIDINFRACCION=@id order by BDIID asc
                                            --order by BDIFECHA asc , BDIHORA asc

"
                    , connection);
                    /*
                     select  BDIOPERACION,BDIFECHA,BDIHORA,BDIIP,b.folioInfraccion
                                            ,b.documento,b.monto,c.nombre, BDITEXTOCAMBIO,d.nombreUsuario
											from bitacoradeinfracciones a
											join infracciones b on b.idInfraccion=BDIIDINFRACCION
											left join cattipocambio c on c.id = a.BDIIDTIPOCAMBIO
											left join UsuariosSitteg d on d.idUsuario=a.BDIIDUSUARIO
											where BDIIDINFRACCION=@id
                                            order by BDIFECHA asc --, BDIHORA asc
                     */

                    command.Parameters.Add(new SqlParameter("@id", SqlDbType.Decimal)).Value = id;
                    command.CommandType = CommandType.Text;
                    SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection);

                    while (reader.Read())
                    {


                        string fechaString = reader["BDIFECHA"].ToString();
                        var guion = fechaString.IndexOf("-") > -1;
                        if (!guion)
                        {
                            fechaString = fechaString.Substring(0, 4) + "-" + fechaString.Substring(4, 2) + "-" + fechaString.Substring(6);
                        }
                        var hora = reader["BDIHORA"].ToString();
                        var formatedhour = hora.Substring(0, 2) + ":" + hora.Substring(2);
                        var element = new BitacoraInfraccionesModel
                        {
                            operacion = reader["BDIOPERACION"].ToString(),
                            hora = formatedhour,
                            ip = reader["BDIIP"].ToString(),
                            nombre = nombre,
                            folio = reader["folioInfraccion"].ToString(),
                            documento = reader["documento"].GetType() == typeof(DBNull) ? "-" : reader["documento"].ToString(),
                            monto = reader["monto"].GetType() == typeof(DBNull) ? "-" : reader["monto"].ToString(),
                            Cambio = reader["nombre"].GetType() == typeof(DBNull) ? "-" : reader["nombre"].ToString(),
                            DescripcionSitteg = reader["BDITEXTOCAMBIO"].GetType() == typeof(DBNull) ? "-" : reader["BDITEXTOCAMBIO"].ToString(),
                            SittegUsuario = reader["nombreUsuario"].GetType() == typeof(DBNull) ? "-" : reader["nombreUsuario"].ToString()
                        };



                        // Intentar parsear la fecha
                        if (DateTime.TryParse(fechaString, out DateTime fecha))
                        {
                            // Formatear la fecha al formato dd/MM/yyyy
                            string fechaFormateada = fecha.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
                            element.fecha = fechaFormateada;
                        }
                        else
                        {
                            // Manejar caso de fecha inválida, si es necesario
                            Console.WriteLine("Fecha inválida: " + fechaString);
                            element.fecha = fechaString;
                        }
                        result.Add(element);
                    }

                }
                catch (Exception ex)
                {

                }
                finally
                {
                    connection.Close();
                }




            return result;

        }
        public async void BitacoraGeneral(string tabla, string consulta, string Operacion, string usuario = "", string ip = "")
        {

            if (string.IsNullOrEmpty(usuario)) { usuario = _Http.HttpContext.User.FindFirst(CustomClaims.Usuario).Value; }
            if (string.IsNullOrEmpty(ip)) { ip = _Http.HttpContext.User.FindFirst(CustomClaims.Ip).Value; }


            var sql = @"insert into bitacora
                        ([BITFECHAHORA],[BITIP],[BITTABLA],[BITOPERACION],[BITCONSULTA],[BITUSUARIO])
                        values
                        (getdate(),@ip,@tabla,@operacion,@consulta,@usuario)";

            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(sql, connection);
                command.Parameters.Add(new SqlParameter("@ip", SqlDbType.VarChar)).Value = ip;
                command.Parameters.Add(new SqlParameter("@tabla", SqlDbType.VarChar)).Value = tabla;
                command.Parameters.Add(new SqlParameter("@operacion", SqlDbType.VarChar)).Value = Operacion;
                command.Parameters.Add(new SqlParameter("@consulta", SqlDbType.VarChar)).Value = consulta;
                command.Parameters.Add(new SqlParameter("@usuario", SqlDbType.VarChar)).Value = usuario;
                command.CommandType = CommandType.Text;
                SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection);
            }
        }


        //MOD BITACORA F2
        public async void BitacoraAccidentes(int IdAccidente, string Accion, Object Objeto)
        {

            var usuario = _Http.HttpContext.User.FindFirst(CustomClaims.IdUsuario).Value;
            var ip = _Http.HttpContext.User.FindFirst(CustomClaims.Ip).Value;
            var IdDElegacion = _Http.HttpContext.User.FindFirst(CustomClaims.OficinaDelegacion).Value;
            var Object = JsonConvert.SerializeObject(Objeto);

            var sql = @"insert into Bitacora_Accidentes
                        ([IdUsuario],[IdDelegacion],[IdAccidente],[Ip],[Accion],[Objeto])
                        values
                        (@usuario,@idDelegacion,@IdAccidente,@ip,@Accion,@Objeto)";

            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(sql, connection);
                command.Parameters.Add(new SqlParameter("@usuario", SqlDbType.Int)).Value = usuario;
                command.Parameters.Add(new SqlParameter("@idDelegacion", SqlDbType.Int)).Value = IdDElegacion;
                command.Parameters.Add(new SqlParameter("@IdAccidente", SqlDbType.Int)).Value = IdAccidente;
                command.Parameters.Add(new SqlParameter("@ip", SqlDbType.VarChar)).Value = ip;
                command.Parameters.Add(new SqlParameter("@Accion", SqlDbType.VarChar)).Value = Accion;
                command.Parameters.Add(new SqlParameter("@Objeto", SqlDbType.VarChar)).Value = Object;
                command.CommandType = CommandType.Text;
                SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection);
            }
        }

        public async void BitacoraAccidentes(int IdAccidente, string Accion, string Objeto)
        {

            var usuario = _Http.HttpContext.User.FindFirst(CustomClaims.IdUsuario).Value;
            var ip = _Http.HttpContext.User.FindFirst(CustomClaims.Ip).Value;
            var IdDElegacion = _Http.HttpContext.User.FindFirst(CustomClaims.OficinaDelegacion).Value;


            var sql = @"insert into Bitacora_Accidentes
                        ([IdUsuario],[IdDelegacion],[IdAccidnte],[Ip],[Accion],[Objeto])
                        values
                        (@usuario,@idDelegacion,@IdAccidente,@ip,@Accion,@Objeto)";

            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(sql, connection);
                command.Parameters.Add(new SqlParameter("@usuario", SqlDbType.Int)).Value = usuario;
                command.Parameters.Add(new SqlParameter("@idDelegacion", SqlDbType.Int)).Value = IdDElegacion;
                command.Parameters.Add(new SqlParameter("@IdAccidente", SqlDbType.Int)).Value = IdAccidente;
                command.Parameters.Add(new SqlParameter("@ip", SqlDbType.VarChar)).Value = ip;
                command.Parameters.Add(new SqlParameter("@Accion", SqlDbType.VarChar)).Value = Accion;
                command.Parameters.Add(new SqlParameter("@Objeto", SqlDbType.VarChar)).Value = Objeto;
                command.CommandType = CommandType.Text;
                SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection);
            }
        }

        public async void BitacoraGenerales(string Accion, Object Objeto)
        {

            var usuario = _Http.HttpContext.User.FindFirst(CustomClaims.IdUsuario).Value;
            var ip = _Http.HttpContext.User.FindFirst(CustomClaims.Ip).Value;
            var IdDElegacion = _Http.HttpContext.User.FindFirst(CustomClaims.OficinaDelegacion).Value;
            var Object = JsonConvert.SerializeObject(Objeto);

            var sql = @"insert into Bitacora_General
                        ([IdUsuario],[IdDelegacion],[Ip],[Accion],[Objeto])
                        values
                        (@usuario,@idDelegacion,@ip,@Accion,@Objeto)";

            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(sql, connection);
                command.Parameters.Add(new SqlParameter("@usuario", SqlDbType.Int)).Value = usuario;
                command.Parameters.Add(new SqlParameter("@idDelegacion", SqlDbType.Int)).Value = IdDElegacion;
                command.Parameters.Add(new SqlParameter("@ip", SqlDbType.VarChar)).Value = ip;
                command.Parameters.Add(new SqlParameter("@Accion", SqlDbType.VarChar)).Value = Accion;
                command.Parameters.Add(new SqlParameter("@Objeto", SqlDbType.VarChar)).Value = Object;
                command.CommandType = CommandType.Text;
                SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection);
            }
        }

        public async void BitacoraGenerales(string Accion, Object Objeto, string usuario, string ip, string IdDElegacion)
        {
            var Object = JsonConvert.SerializeObject(Objeto);

            var sql = @"insert into Bitacora_General
                        ([IdUsuario],[IdDelegacion],[Ip],[Accion],[Objeto])
                        values
                        (@usuario,@idDelegacion,@ip,@Accion,@Objeto)";

            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(sql, connection);
                command.Parameters.Add(new SqlParameter("@usuario", SqlDbType.Int)).Value = usuario;
                command.Parameters.Add(new SqlParameter("@idDelegacion", SqlDbType.Int)).Value = IdDElegacion;
                command.Parameters.Add(new SqlParameter("@ip", SqlDbType.VarChar)).Value = ip;
                command.Parameters.Add(new SqlParameter("@Accion", SqlDbType.VarChar)).Value = Accion;
                command.Parameters.Add(new SqlParameter("@Objeto", SqlDbType.VarChar)).Value = Object;
                command.CommandType = CommandType.Text;
                SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection);
            }
        }

        public async void BitacoraGenerales(string Accion, string Objeto)
        {

            var usuario = _Http.HttpContext.User.FindFirst(CustomClaims.IdUsuario).Value;
            var ip = _Http.HttpContext.User.FindFirst(CustomClaims.Ip).Value;
            var IdDElegacion = _Http.HttpContext.User.FindFirst(CustomClaims.OficinaDelegacion).Value;


            var sql = @"insert into Bitacora_General
                        ([IdUsuario],[IdDelegacion],[Ip],[Accion],[Objeto])
                        values
                        (@usuario,@idDelegacion,@ip,@Accion,@Objeto)";

            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(sql, connection);
                command.Parameters.Add(new SqlParameter("@usuario", SqlDbType.Int)).Value = usuario;
                command.Parameters.Add(new SqlParameter("@idDelegacion", SqlDbType.Int)).Value = IdDElegacion;
                command.Parameters.Add(new SqlParameter("@ip", SqlDbType.VarChar)).Value = ip;
                command.Parameters.Add(new SqlParameter("@Accion", SqlDbType.VarChar)).Value = Accion;
                command.Parameters.Add(new SqlParameter("@Objeto", SqlDbType.VarChar)).Value = Objeto;
                command.CommandType = CommandType.Text;
                SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection);
            }
        }

        public async void BitacoraDepositos(int idDeposito, string modulo, string Accion, Object Objeto)
        {

            var usuario = _Http.HttpContext.User.FindFirst(CustomClaims.IdUsuario).Value;
            var ip = _Http.HttpContext.User.FindFirst(CustomClaims.Ip).Value;
            var IdDElegacion = _Http.HttpContext.User.FindFirst(CustomClaims.OficinaDelegacion).Value;
            var Object = JsonConvert.SerializeObject(Objeto);

            var sql = @"insert into Bitacora_Depositos
                        ([IdUsuario],[IdDelegacion],[idDeposito],[Modulo],[Ip],[Accion],[Objeto])
                        values
                        (@usuario,@idDelegacion,@idDeposito,@Modulo,@ip,@Accion,@Objeto)";

            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(sql, connection);
                command.Parameters.Add(new SqlParameter("@usuario", SqlDbType.Int)).Value = usuario;
                command.Parameters.Add(new SqlParameter("@idDelegacion", SqlDbType.Int)).Value = IdDElegacion;
                command.Parameters.Add(new SqlParameter("@idDeposito", SqlDbType.Int)).Value = idDeposito;
                command.Parameters.Add(new SqlParameter("@Modulo", SqlDbType.VarChar)).Value = modulo;
                command.Parameters.Add(new SqlParameter("@ip", SqlDbType.VarChar)).Value = ip;
                command.Parameters.Add(new SqlParameter("@Accion", SqlDbType.VarChar)).Value = Accion;
                command.Parameters.Add(new SqlParameter("@Objeto", SqlDbType.VarChar)).Value = Object;
                command.CommandType = CommandType.Text;
                SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection);
            }
        }

        public async void BitacoraWS(string WS, string Accion, Object Objeto)
        {

            var usuario = _Http.HttpContext.User.FindFirst(CustomClaims.IdUsuario).Value;
            var ip = _Http.HttpContext.User.FindFirst(CustomClaims.Ip).Value;
            var IdDElegacion = _Http.HttpContext.User.FindFirst(CustomClaims.OficinaDelegacion).Value;
            var Object = JsonConvert.SerializeObject(Objeto);

            var sql = @"insert into Bitacora_WS
                        ([IdUsuario],[IdDelegacion],[Ws],[Ip],[Accion],[Objeto])
                        values
                        (@usuario,@idDelegacion,@Ws,@ip,@Accion,@Objeto)";

            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(sql, connection);
                command.Parameters.Add(new SqlParameter("@usuario", SqlDbType.Int)).Value = usuario;
                command.Parameters.Add(new SqlParameter("@idDelegacion", SqlDbType.Int)).Value = IdDElegacion;
                command.Parameters.Add(new SqlParameter("@Ws", SqlDbType.VarChar)).Value = WS;
                command.Parameters.Add(new SqlParameter("@ip", SqlDbType.VarChar)).Value = ip;
                command.Parameters.Add(new SqlParameter("@Accion", SqlDbType.VarChar)).Value = Accion;
                command.Parameters.Add(new SqlParameter("@Objeto", SqlDbType.VarChar)).Value = Object;
                command.CommandType = CommandType.Text;
                SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection);
            }
        }
        public async void BitacoraWS(string WS, string Accion, Object Objeto, string usuario, string ip, string IdDElegacion)
        {

            var Object = JsonConvert.SerializeObject(Objeto);

            var sql = @"insert into Bitacora_WS
                        ([IdUsuario],[IdDelegacion],[Ws],[Ip],[Accion],[Objeto])
                        values
                        (@usuario,@idDelegacion,@Ws,@ip,@Accion,@Objeto)";
            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(sql, connection);
                command.Parameters.Add(new SqlParameter("@usuario", SqlDbType.Int)).Value = usuario;
                command.Parameters.Add(new SqlParameter("@idDelegacion", SqlDbType.Int)).Value = IdDElegacion;
                command.Parameters.Add(new SqlParameter("@Ws", SqlDbType.VarChar)).Value = WS;
                command.Parameters.Add(new SqlParameter("@ip", SqlDbType.VarChar)).Value = ip;
                command.Parameters.Add(new SqlParameter("@Accion", SqlDbType.VarChar)).Value = Accion;
                command.Parameters.Add(new SqlParameter("@Objeto", SqlDbType.VarChar)).Value = Object;
                command.CommandType = CommandType.Text;
                SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection);
            }
        }



    }
}
