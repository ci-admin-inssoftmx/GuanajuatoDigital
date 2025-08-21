using DocumentFormat.OpenXml.Office2010.Excel;
using GuanajuatoAdminUsuarios.Entity;
using GuanajuatoAdminUsuarios.Framework.Catalogs;
using GuanajuatoAdminUsuarios.Interfaces;
using GuanajuatoAdminUsuarios.Interfaces.Catalogos;
using GuanajuatoAdminUsuarios.Models;
using GuanajuatoAdminUsuarios.Models.Generales;
using GuanajuatoAdminUsuarios.RESTModels;
using GuanajuatoAdminUsuarios.Util;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;

namespace GuanajuatoAdminUsuarios.Services

{
    public class AdminCatalogosService : IAdminCatalogosService
    {
        private readonly ISqlClientConnectionBD _sqlClientConnectionBD;
        public AdminCatalogosService(
                ISqlClientConnectionBD sqlClientConnectionBD)
        {
            _sqlClientConnectionBD = sqlClientConnectionBD;
        }

       


        public List<AdminCatalogosModel> ObtieneListaCatalogos()
        {
            //
            List<AdminCatalogosModel> listCatalogos = new List<AdminCatalogosModel>();
            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
                try

                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(@"Select * from Catalogos", connection);
                    command.CommandType = CommandType.Text;
                    //sqlData Reader sirve para la obtencion de datos 
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            AdminCatalogosModel catalogo = new AdminCatalogosModel();
                            catalogo.idCatalogo = Convert.ToInt32(reader["idCatalogo"].ToString());
                            catalogo.catalogo = reader["catalogo"].ToString();

                            listCatalogos.Add(catalogo);

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
            return listCatalogos;
        }
        public List<AdminCatalogosModel> BusquedaPorCatalogo(int? idCatalogo, int? idDependencia)
        {
            List<AdminCatalogosModel> elementosCatalogo = new List<AdminCatalogosModel>();

            var columnMapping = new Dictionary<string, string>
    {
                //mapeo de campos para elmodelo.elemnto//
        { "catCarreteras", "carretera" }, 
        { "catTramos", "tramo" },        
        { "catDependencias", "nombreDependencia" },
        { "catMunicipios", "municipio" },
        { "catEntidades", "nombreEntidad" },
       // { "catOficiales", "tramo" },
        { "catMarcasVehiculos", "marcaVehiculo" },
        { "catSubmarcasVehiculos", "nombreSubmarca" },
        { "catTiposVehiculo", "tipoVehiculo" },
        { "catSalariosMinimos", "salario" },
                { "catColores", "color" },
                { "catMotivosInfraccion", "nombre" },
        { "catDiasInhabiles", "fecha" },
        { "catAgenciasMinisterio", "nombreAgencia" },
        { "catAutoridadesDisposicion", "nombreAutoridadDisposicion" },
        { "catAutoridadesEntrega", "autoridadentrega" },
        { "catInstitucionesTraslado", "institucionTraslado" },
        { "catClasificacionAccidentes", "nombreClasificacion" },
        { "catCausasAccidentes", "causaAccidente" },
       // { "catDelegacionesOficinasTransporte", "nombreOficina" },
        { "catHospitales", "NombreHospital" },
        { "catFactoresAccidentes", "factorAccidente" }
    };

            string tableName = "";
            string elementColumn = "";

            switch (idCatalogo)
            {
                case 1:
                    tableName = "catCarreteras";
                    elementColumn = columnMapping[tableName];
                    break;
                case 2:
                    tableName = "catTramos";
                    elementColumn = columnMapping[tableName];
                    break;
                case 3:
                    tableName = "catDependencias";
                    elementColumn = columnMapping[tableName];
                    break;
                case 4:
                    tableName = "catMunicipios";
                    elementColumn = columnMapping[tableName];
                    break;
                case 5:
                    tableName = "catEntidades";
                    elementColumn = columnMapping[tableName];
                    break;
                case 6:
                    tableName = "catOficiales";
                    elementColumn = columnMapping[tableName];
                    break;
                case 7:
                    tableName = "catMarcasVehiculos";
                    elementColumn = columnMapping[tableName];
                    break;
                case 8:
                    tableName = "catSubmarcasVehiculos";
                    elementColumn = columnMapping[tableName];
                    break;
                case 9:
                    tableName = "catTiposVehiculo";
                    elementColumn = columnMapping[tableName];
                    break;
                case 10:
                    tableName = "catSalariosMinimos";
                    elementColumn = columnMapping[tableName];
                    break;
                case 11:
                    tableName = "catColores";
                    elementColumn = columnMapping[tableName];
                    break;
                case 12:
                    tableName = "catMotivosInfraccion";
                    elementColumn = columnMapping[tableName];
                    break;
                case 13:
                    tableName = "catDiasInhabiles";
                    elementColumn = columnMapping[tableName];
                    break;
                case 14:
                    tableName = "catAgenciasMinisterio";
                    elementColumn = columnMapping[tableName];
                    break;
                case 15:
                    tableName = "catAutoridadesDisposicion";
                    elementColumn = columnMapping[tableName];
                    break;
                case 16:
                    tableName = "catAutoridadesEntrega";
                    elementColumn = columnMapping[tableName];
                    break;
                case 17:
                    tableName = "catInstitucionesTraslado";
                    elementColumn = columnMapping[tableName];
                    break;
                case 18:
                    tableName = "catClasificacionAccidentes";
                    elementColumn = columnMapping[tableName];
                    break;
                case 19:
                    tableName = "catCausasAccidentes";
                    elementColumn = columnMapping[tableName];
                    break;
                case 20:
                    tableName = "catDelegacionesOficinasTransporte";
                    elementColumn = columnMapping[tableName];
                    break;
                case 21:
                    tableName = "catHospitales";
                    elementColumn = columnMapping[tableName];
                    break;
                case 22:
                    tableName = "catFactoresAccidentes";
                    elementColumn = columnMapping[tableName];
                    break;
                   default:
                    return elementosCatalogo;
            }

            // Construir la consulta SQL dinámicamente
            string query = $@"
                            SELECT t.*, d.nombreDependencia 
                            FROM {tableName} t
                            INNER JOIN catDependencias d ON d.idDependencia = @transito
                            WHERE t.transito = @transito AND t.estatus = 1";
            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
            {
                try
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.CommandType = CommandType.Text;
                        command.Parameters.AddWithValue("@transito",idDependencia);

                        using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                        {
                            while (reader.Read())
                            {
                                AdminCatalogosModel elemento = new AdminCatalogosModel();

                                // Asignar dinámicamente el valor de la columna 'elementColumn' a la propiedad 'elemento'
                                var propInfo = typeof(AdminCatalogosModel).GetProperty("elemento");
                                if (propInfo != null)
                                {
                                    propInfo.SetValue(elemento, reader[elementColumn]?.ToString());
                                }

                                // Asignar el valor de 'dependencia' si es necesario
                                elemento.dependencia = reader["nombreDependencia"].ToString();

                                elementosCatalogo.Add(elemento);
                            }
                        }
                    }
                }
                catch (SqlException ex)
                {
                    // Guardar la excepción en un log de errores o manejarla según corresponda
                    // LogError(ex);
                }
                finally
                {
                    connection.Close();
                }
            }

            return elementosCatalogo;
        }


    }
}
