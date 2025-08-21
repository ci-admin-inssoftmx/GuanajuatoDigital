using GuanajuatoAdminUsuarios.Helpers;
using GuanajuatoAdminUsuarios.Interfaces;
using GuanajuatoAdminUsuarios.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
//using Telerik.SvgIcons;

namespace GuanajuatoAdminUsuarios.Services
{
    public class CatAlertamientoService :ICatAlertamientoServices
    {
        private readonly ISqlClientConnectionBD _sqlClientConnectionBD;

        public CatAlertamientoService(ISqlClientConnectionBD dataBase)
        {
            _sqlClientConnectionBD = dataBase;
        }

        public List<AlertamientoGridModel> GetdataGrid(int corp)
        {
            var result = new List<AlertamientoGridModel>();

            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
                try

                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(@"select idAlertamiento,b.Corporacion,aplicacion,cantidadInfracciones
                            from [dbo].[alertamiento] a
                            join CatCorporaciones b on a.idDelegacion=b.IdCorporacion
                            join catAplicacionInfraccion c on a .idAplicacion=c.idAplicacion
                            ", connection);
                    command.CommandType = CommandType.Text;
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            AlertamientoGridModel service = new AlertamientoGridModel();
                            service.idAlertamiento = Convert.ToInt32(reader["idAlertamiento"].ToString());
                            service.Delegacion = reader["Corporacion"].ToString();
                            service.Aplicacion = reader["aplicacion"].ToString();
                            service.Cantidad = Convert.ToInt32(reader["cantidadInfracciones"].ToString());

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

        public AlertamientoGridModel Getdata(int IdAlertamiento)
        {
            var service = new AlertamientoGridModel();

            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
                try

                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(@"select idAlertamiento,delegacion,aplicacion,cantidadInfracciones,transito
                            from [dbo].[alertamiento] a
                            join catDelegaciones b on a.idDelegacion=b.idDelegacion
                            join catAplicacionInfraccion c on a .idAplicacion=c.idAplicacion
                            where a.idAlertamiento=@corp", connection);
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("@corp", IdAlertamiento);
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            
                            service.idAlertamiento = Convert.ToInt32(reader["idAlertamiento"].ToString());
                            service.Delegacion = reader["delegacion"].ToString();
                            service.Aplicacion = reader["aplicacion"].ToString();
                            service.Cantidad = Convert.ToInt32(reader["cantidadInfracciones"].ToString());
                      
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

        public int EditarAlertamiento(int IdAlertamiento, int cantidad)
        {
            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
                try

                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(@"update a set a.cantidadInfracciones=@cantidad
                            from [dbo].[alertamiento] a                            
                            where a.idAlertamiento=@corp", connection);
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("@corp", IdAlertamiento);
                    command.Parameters.AddWithValue("@cantidad", cantidad);
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


            return 1;
        }

        public int CrearAlertamiento(int cantidad, int idAplicacion, int delegacion)
        {
            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
            {
                try
                {
                    connection.Open();

                    // Comprobar si ya existe el registro
                    SqlCommand checkCommand = new SqlCommand(@"
                         SELECT COUNT(*) FROM alertamiento 
                         WHERE idDelegacion = @delegacion 
                         AND anio = CAST(YEAR(GETDATE()) AS VARCHAR(5)) 
                         AND idAplicacion = @aplicada", connection);
                    checkCommand.Parameters.AddWithValue("@delegacion", delegacion);
                    checkCommand.Parameters.AddWithValue("@aplicada", idAplicacion);

                    int count = (int)checkCommand.ExecuteScalar();

                    if (count > 0)
                    {
                        return 0; // Registro ya existe
                    }

                    SqlCommand insertCommand = new SqlCommand(@"
                INSERT INTO alertamiento (idDelegacion, anio, cantidadInfracciones, idAplicacion) 
                VALUES (@delegacion, CAST(YEAR(GETDATE()) AS VARCHAR(5)), @cantidad, @aplicada)", connection);
                    insertCommand.Parameters.AddWithValue("@delegacion", delegacion);
                    insertCommand.Parameters.AddWithValue("@cantidad", cantidad);
                    insertCommand.Parameters.AddWithValue("@aplicada", idAplicacion);

                    insertCommand.ExecuteNonQuery(); // Ejecutar el insert
                }
                catch (SqlException ex)
                {
                    // Manejo de excepciones (puedes agregar logging aquí)
                }
                finally
                {
                    connection.Close();
                }
            }

            return 1; // Inserción exitosa
        }


        List<int> GetCorporacionesAlertamientos()
        {
            var catalogo = new List<int>();
            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))               
                try
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(@"select idDelegacion from alertamiento
                                                            group by idDelegacion
                                                            having count(*)>3", connection);
                    command.CommandType = CommandType.Text;
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            int t;
                            if (int.TryParse(reader["idDelegacion"].ToString(), out t))
                            {
                                catalogo.Add(t);
                            }
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
            return catalogo;
        }

        List<CatalogModel> GetCatalogCorp()
        {
            var catalogo = new List<CatalogModel>();
            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
                try
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(@"select IdCorporacion,Corporacion from CatCorporaciones where estatus=1", connection);
                    command.CommandType = CommandType.Text;
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            var catalog = new CatalogModel();

                            catalog.value = reader["IdCorporacion"].ToString();
                            catalog.text = reader["Corporacion"].ToString();
                            catalogo.Add(catalog);
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
            return catalogo;
        }
        public List<CatalogModel> GetCorpCatalog()
        {
            var corpnoUse = GetCorporacionesAlertamientos();
            var corp = GetCatalogCorp();
            return corp.Where(s => !corpnoUse.Contains(s.value.toInt())).ToList();
        }
        List<int> GetAplicadaAlertamientos(int corp)
        {
            var catalogo = new List<int>();
            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
                try
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(@"select idAplicacion from alertamiento where idDelegacion = @corp", connection);
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("corp", corp);
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            int t;
                            if (int.TryParse(reader["idAplicacion"].ToString(), out t))
                            {
                                catalogo.Add(t);
                            }
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
            return catalogo;
        }

        List<CatalogModel> GetCatalogAplicada()
        {
            var catalogo = new List<CatalogModel>();
            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
                try
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(@"select * from catAplicacionInfraccion where estatus=1", connection);
                    command.CommandType = CommandType.Text;
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            var catalog = new CatalogModel();

                            catalog.value = reader["idAplicacion"].ToString();
                            catalog.text = reader["aplicacion"].ToString();
                            catalogo.Add(catalog);
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
            return catalogo;
        }


        public List<CatalogModel> GetAplicadaCatalog(int corp)
        {
            var corpnoUse = GetAplicadaAlertamientos(corp);
            var aplicada = GetCatalogAplicada();
            return aplicada.Where(s => !corpnoUse.Contains(s.value.toInt())).ToList();
        }

    }
}
