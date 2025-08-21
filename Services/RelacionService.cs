using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Office2010.ExcelAc;
using GuanajuatoAdminUsuarios.Interfaces;
using GuanajuatoAdminUsuarios.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Org.BouncyCastle.Asn1.X509.Qualified;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using static GuanajuatoAdminUsuarios.RESTModels.ConsultarDocumentoResponseModel;

namespace GuanajuatoAdminUsuarios.Services
{
    public class RelacionService :IRelacionService
    {

        private readonly ISqlClientConnectionBD _sqlClientConnectionBD;

        public RelacionService(ISqlClientConnectionBD sqlClientConnectionBD)
        {
            _sqlClientConnectionBD = sqlClientConnectionBD;
        }

         string GetQuery(int id)
        {
            var result = "";

            if (id == 1)
            result = "select a.idAutoridadDisposicion as id , a.nombreAutoridadDisposicion as [desc] , b.nombreAutoridadDisposicion as parent , c.Corporacion as corp from catAutoridadesDisposicion a left join catAutoridadesDisposicion b on a.parent=b.idAutoridadDisposicion join CatCorporaciones c on c.IdCorporacion=a.transito   where a.estatus =1 and a.isparent is null and a.parent is null";
            if (id == 2)
                result = "select a.idAutoridadEntrega as id , a.autoridadEntrega as [desc] , b.autoridadEntrega as parent , c.Corporacion as corp from catAutoridadesEntrega a left join catAutoridadesEntrega b on a.parent=b.idAutoridadEntrega join CatCorporaciones c on c.IdCorporacion=a.transito where a.estatus =1 and a.isparent is null and a.parent is null";
            if (id == 3)
                result = "select a.idCarretera as id , a.carretera as [desc] , b.carretera as parent , c.Corporacion as corp from catcarreteras a left join catcarreteras b on a.parent=b.idCarretera join CatCorporaciones c on c.IdCorporacion=a.transito where a.estatus =1 and a.isparent is null and a.parent is null";
            if (id == 4)
                result = "select a.idCausaAccidente as id , a.causaAccidente as [desc] , b.causaAccidente as parent , c.Corporacion as corp from catCausasAccidentes a left join catCausasAccidentes b on a.parent=b.idCausaAccidente join CatCorporaciones c on c.IdCorporacion=a.transito where a.estatus =1 and a.isparent is null and a.parent is null";
            if (id == 5)
                result = "select a.idColor as id , a.color as [desc] , b.color as parent , c.Corporacion as corp from catColores a left join catColores b on a.parent=b.idColor join CatCorporaciones c on c.IdCorporacion=a.transito where a.estatus =1 and a.isparent is null and a.parent is null";
            if (id == 6)
                result = "select a.idFactorAccidente as id , a.factorAccidente as [desc] , b.factorAccidente as parent , c.Corporacion as corp from catFactoresAccidentes a left join catFactoresAccidentes b on a.parent=b.idFactorAccidente join CatCorporaciones c on c.IdCorporacion=a.transito where a.estatus =1 and a.isparent is null and a.parent is null";
            if (id == 7)
                result = "select a.idHospital as id , a.NombreHospital as [desc] , b.NombreHospital as parent , c.Corporacion as corp from catHospitales a left join catHospitales b on a.parent=b.idHospital join CatCorporaciones c on c.IdCorporacion=a.transito where a.estatus =1 and a.isparent is null and a.parent is null";
            if (id == 8)
                result = "select a.idInstitucionTraslado as id , a.institucionTraslado as [desc] , b.institucionTraslado as parent , c.Corporacion as corp from catinstitucionestraslado a left join catinstitucionestraslado b on a.parent=b.idInstitucionTraslado join CatCorporaciones c on c.IdCorporacion=a.transito where a.estatus =1 and a.isparent is null and a.parent is null";
            if (id == 9)
                result = "select a.idCatMotivoInfraccion as id , a.nombre as [desc] , b.nombre as parent , c.Corporacion as corp from catmotivosinfraccion a left join catmotivosinfraccion b on a.parent=b.idCatMotivoInfraccion join CatCorporaciones c on c.IdCorporacion=a.transito where a.estatus =1 and a.isparent is null and a.parent is null";
            if (id == 10)
                result = "select a.idOficial as id , a.nombre + ' ' + a.apellidoPaterno as [desc] , b.nombre + ' ' + b.apellidoPaterno as parent , c.Corporacion as corp from catoficiales a left join catoficiales b on a.parent=b.idOficial join CatCorporaciones c on c.IdCorporacion=a.transito where a.estatus =1 and a.isparent is null and a.parent is null";
            if (id == 12)
                result = "select a.idTramo as id , a.tramo as [desc] , b.tramo as parent , c.Corporacion as corp from cattramos a left join cattramos b on a.parent=b.idTramo join CatCorporaciones c on c.IdCorporacion=a.transito where a.estatus =1 and a.isparent is null and a.parent is null";

            return result;
        }
        string GetQueryParent(int id)
        {
            var result = "";

            if (id == 1)
               result = "select a.idAutoridadDisposicion as id , a.nombreAutoridadDisposicion as [desc] , b.nombreAutoridadDisposicion as parent , c.Corporacion as corp from catAutoridadesDisposicion a left join catAutoridadesDisposicion b on a.parent=b.idAutoridadDisposicion join CatCorporaciones c on c.IdCorporacion=a.transito   where a.estatus =1 and a.isparent is null and a.parent = @id";
            if (id == 2)
                result = "select a.idAutoridadEntrega as id , a.autoridadEntrega as [desc] , b.autoridadEntrega as parent , c.Corporacion as corp from catAutoridadesEntrega a left join catAutoridadesEntrega b on a.parent=b.idAutoridadEntrega join CatCorporaciones c on c.IdCorporacion=a.transito where a.estatus =1 and a.isparent is null and a.parent = @id";
            if (id == 3)
                result = "select a.idCarretera as id , a.carretera as [desc] , b.carretera as parent , c.Corporacion as corp from catcarreteras a left join catcarreteras b on a.parent=b.idCarretera join CatCorporaciones c on c.IdCorporacion=a.transito where a.estatus =1 and a.isparent is null and a.parent = @id";
            if (id == 4)
                result = "select a.idCausaAccidente as id , a.causaAccidente as [desc] , b.causaAccidente as parent , c.Corporacion as corp from catCausasAccidentes a left join catCausasAccidentes b on a.parent=b.idCausaAccidente join CatCorporaciones c on c.IdCorporacion=a.transito where a.estatus =1 and a.isparent is null and a.parent = @id";
            if (id == 5)
                result = "select a.idColor as id , a.color as [desc] , b.color as parent , c.Corporacion as corp from catColores a left join catColores b on a.parent=b.idColor join CatCorporaciones c on c.IdCorporacion=a.transito where a.estatus =1 and a.isparent is null and a.parent = @id";
            if (id == 6)
                result = "select a.idFactorAccidente as id , a.factorAccidente as [desc] , b.factorAccidente as parent , c.Corporacion as corp from catFactoresAccidentes a left join catFactoresAccidentes b on a.parent=b.idFactorAccidente join CatCorporaciones c on c.IdCorporacion=a.transito where a.estatus =1 and a.isparent is null and a.parent = @id";
            if (id == 7)
                result = "select a.idHospital as id , a.NombreHospital as [desc] , b.NombreHospital as parent , c.Corporacion as corp from catHospitales a left join catHospitales b on a.parent=b.idHospital join CatCorporaciones c on c.IdCorporacion=a.transito where a.estatus =1 and a.isparent is null and a.parent = @id";
            if (id == 8)
                result = "select a.idInstitucionTraslado as id , a.institucionTraslado as [desc] , b.institucionTraslado as parent , c.Corporacion as corp from catinstitucionestraslado a left join catinstitucionestraslado b on a.parent=b.idInstitucionTraslado join CatCorporaciones c on c.IdCorporacion=a.transito where a.estatus =1 and a.isparent is null and a.parent = @id";
            if (id == 9)
                result = "select a.idCatMotivoInfraccion as id , a.nombre as [desc] , b.nombre as parent , c.Corporacion as corp from catmotivosinfraccion a left join catmotivosinfraccion b on a.parent=b.idCatMotivoInfraccion join CatCorporaciones c on c.IdCorporacion=a.transito where a.estatus =1 and a.isparent is null and a.parent = @id";
            if (id == 10)
                result = "select a.idOficial as id , a.nombre + ' ' + a.apellidoPaterno as [desc] , b.nombre + ' ' + b.apellidoPaterno as parent , c.Corporacion as corp from catoficiales a left join catoficiales b on a.parent=b.idOficial join CatCorporaciones c on c.IdCorporacion=a.transito where a.estatus =1 and a.isparent is null and a.parent = @id";
            if (id == 12)
                result = "select a.idTramo as id , a.tramo as [desc] , b.tramo as parent , c.Corporacion as corp from cattramos a left join cattramos b on a.parent=b.idTramo join CatCorporaciones c on c.IdCorporacion=a.transito where a.estatus =1 and a.isparent is null and a.parent = @id";

                return result;
        }

        string getQueryCatalog(int Id)
        {
            var result = "";
            if (Id == 1)
                result = "select idAutoridadDisposicion as value,nombreAutoridadDisposicion as text from catAutoridadesDisposicion where estatus=1 and isparent = 1";
            if (Id == 2)
                result = "select idAutoridadEntrega as value,autoridadEntrega as text from catAutoridadesEntrega where estatus=1 and isparent = 1";
            if (Id == 3)
                result = "select idCarretera as value, carretera as text from catcarreteras where estatus=1 and isparent = 1";
            if (Id == 4)
                result = "select idCausaAccidente as value,causaAccidente  as text from catCausasAccidentes where estatus=1 and isparent = 1";
            if (Id == 5)
                result = "select idColor as value,color  as text from catColores where estatus=1 and isparent = 1";
            if (Id == 6)
                result = "select idFactorAccidente as value,factorAccidente as text from catFactoresAccidentes where estatus=1 and isparent = 1";
            if (Id == 7)
                result = "select idHospital as value , NombreHospital as text from catHospitales where estatus=1 and isparent = 1";
            if (Id == 8)
                result = "select idInstitucionTraslado as value, institucionTraslado as text from catinstitucionestraslado where estatus=1 and isparent = 1";
            if (Id == 9)
                result = "select idCatMotivoInfraccion as value, nombre as text from catmotivosinfraccion where estatus=1 and isparent = 1";
            if (Id == 10)
                result = "select idOficial as value, nombre + ' ' + apellidoPaterno  as text from catoficiales where estatus=1 and isparent = 1";
            if (Id == 11)
                result = "select * from cattiposvehiculo where estatus=1 and isparent = 1";
            if (Id == 12)
                result = "select idTramo as value, tramo  as text from cattramos where estatus=1 and isparent = 1";



            return result;
        }

        string getQueryInsertParent(int id)
        {
            var result = "";

            if (id == 1)
                 result = "insert into catAutoridadesDisposicion (nombreAutoridadDisposicion,fechaActualizacion,actualizadoPor,estatus,isparent) values (@desc           ,getdate()         ,             1,      1,1)";
            if (id == 2)
                result = "insert into catAutoridadesEntrega (autoridadEntrega,fechaActualizacion,actualizadoPor,estatus,isparent) values (@desc           ,getdate()         ,             1,      1,1)";
            if (id == 3)
                result = "insert into catcarreteras (carretera,fechaActualizacion,actualizadoPor,estatus,isparent) values (@desc           ,getdate()         ,             1,      1,1)";
            if (id == 4)
                result = "insert into catCausasAccidentes (causaAccidente,fechaActualizacion,actualizadoPor,estatus,isparent) values (@desc           ,getdate()         ,             1,      1,1)";
            if (id == 5)
                result = "insert into catColores (color,fechaActualizacion,actualizadoPor,estatus,isparent) values (@desc           ,getdate()         ,             1,      1,1)";
            if (id == 6)
                result = "insert into catFactoresAccidentes (factorAccidente,fechaActualizacion,actualizadoPor,estatus,isparent) values (@desc           ,getdate()         ,             1,      1,1)";
            if (id == 7)
                result = "insert into catHospitales (NombreHospital,fechaActualizacion,actualizadoPor,estatus,isparent) values (@desc           ,getdate()         ,             1,      1,1)";
            if (id == 8)
                result = "insert into catinstitucionestraslado (institucionTraslado,fechaActualizacion,actualizadoPor,estatus,isparent) values (@desc           ,getdate()         ,             1,      1,1)";
            if (id == 9)
                result = "insert into catmotivosinfraccion (nombre,IdSubConcepto,IdConcepto,calificacionMinima,calificacionMaxima,fundamento,fechaActualizacion,actualizadoPor,estatus,isparent) values (@desc,6,1,10,10,''           ,getdate()         ,             1,      1,1)";
            if (id == 10)
                result = "insert into catoficiales (nombre,apellidoPaterno,apellidoMaterno,fechaActualizacion,actualizadoPor,estatus,isparent) values (@desc,@desc2,''           ,getdate()         ,             1,      1,1)";
            if (id == 12)
                result = "insert into cattramos (tramo,fechaActualizacion,actualizadoPor,estatus,isparent) values (@desc           ,getdate()         ,             1,      1,1)";

            return result;
        }

        string getQueryUpdateParent(int id)
        {
            var result = "";

            if (id == 1)
            result = "update a set parent=@idP from catAutoridadesDisposicion a where idAutoridadDisposicion=@idO";
            if (id == 2)
                result = "update a set parent=@idP from catAutoridadesEntrega a where idAutoridadEntrega=@idO";
            if (id == 3)
                result = "update a set parent=@idP from catcarreteras a where idCarretera=@idO";
            if (id == 4)
                result = "update a set parent=@idP from catCausasAccidentes a where idCausaAccidente=@idO";
            if (id == 5)
                result = "update a set parent=@idP from catColores a where idColor=@idO";
            if (id == 6)
                result = "update a set parent=@idP from catFactoresAccidentes a where idFactorAccidente=@idO";
            if (id == 7)
                result = "update a set parent=@idP from catHospitales a where idHospital=@idO";
            if (id == 8)
                result = "update a set parent=@idP from catinstitucionestraslado a where idInstitucionTraslado=@idO";
            if (id == 9)
                result = "update a set parent=@idP from catmotivosinfraccion a where idCatMotivoInfraccion=@idO";
            if (id == 10)
                result = "update a set parent=@idP from catoficiales a where idOficial=@idO";
            if (id == 12)
                result = "update a set parent=@idP from cattramos a where idCarretera=@idO";

            return result;
        }

        string getQueryUpdateParentnull(int id)
        {
            var result = "";

            if (id == 1)
                result = "update a set parent=null from catAutoridadesDisposicion a where idAutoridadDisposicion=@idO";
            if (id == 2)
                result = "update a set parent=null from catAutoridadesEntrega a where idAutoridadEntrega=@idO";
            if (id == 3)
                result = "update a set parent=null from catcarreteras a where idCarretera=@idO";
            if (id == 4)
                result = "update a set parent=null from catCausasAccidentes a where idCausaAccidente=@idO";
            if (id == 5)
                result = "update a set parent=null from catColores a where idColor=@idO";
            if (id == 6)
                result = "update a set parent=null from catFactoresAccidentes a where idFactorAccidente=@idO";
            if (id == 7)
                result = "update a set parent=null from catHospitales a where idHospital=@idO";
            if (id == 8)
                result = "update a set parent=null from catinstitucionestraslado a where idInstitucionTraslado=@idO";
            if (id == 9)
                result = "update a set parent=null from catmotivosinfraccion a where idCatMotivoInfraccion=@idO";
            if (id == 10)
                result = "update a set parent=null from catoficiales a where idOficial=@idO";
            if (id == 12)
                result = "update a set parent=null from cattramos a where idCarretera=@idO";

            return result;
        }


        public List<AdminRelasionCatalogos> GetData(int id)
        {
            var result = new List<AdminRelasionCatalogos>();

            var Query = GetQuery(id);

            if(string.IsNullOrEmpty(Query))
                return result;

            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
                try
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(Query, connection);
                    command.CommandType = CommandType.Text;
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            var aux = new AdminRelasionCatalogos();

                            aux.Id =  Convert.ToInt32(reader["id"].ToString());
                            aux.Descripcion = reader["desc"].ToString();
                            aux.Padre = reader["parent"].GetType() == typeof(DBNull) ? "":   reader["parent"].ToString();
                            aux.Corporacion = reader["corp"].ToString();
                            result.Add(aux);
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

        public List<AdminRelasionCatalogos> GetDataParent(int id,int par)
        {
            var result = new List<AdminRelasionCatalogos>();

            if (id == 0 || par == 0) return result;

            var Query = GetQueryParent(id);

            if (string.IsNullOrEmpty(Query))
                return result;

            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
                try
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(Query, connection);
                    command.Parameters.AddWithValue("@id", par);
                    command.CommandType = CommandType.Text;
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            var aux = new AdminRelasionCatalogos();

                            aux.Id = Convert.ToInt32(reader["id"].ToString());
                            aux.Descripcion = reader["desc"].ToString();
                            aux.Padre = reader["parent"].GetType() == typeof(DBNull) ? "" : reader["parent"].ToString();
                            aux.Corporacion = reader["corp"].ToString();
                            result.Add(aux);
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


        public List<CatalogModel> GetCatalog(int Id)
        {
            var result = new List<CatalogModel>();
            var Query = getQueryCatalog(Id);

            if (string.IsNullOrEmpty(Query))
                return result;


            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
                try
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(Query, connection);
                    command.CommandType = CommandType.Text;
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            var aux = new CatalogModel();

                            aux.value = reader["value"].ToString();
                            aux.text = reader["text"].ToString();


                            result.Add(aux);
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



        public bool InsertParent(int id, string desc, string desc2)
        {
            var result = false;

            var Query = getQueryInsertParent(id);

            if (string.IsNullOrEmpty(Query))
                return result;

            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
                try
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(Query, connection);
                    command.Parameters.AddWithValue("@desc", desc);
                    if(id==10) command.Parameters.AddWithValue("@desc2", desc2);


                    command.CommandType = CommandType.Text;
                    var ids = command.ExecuteNonQuery();
                    
                    result  = true;

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


        public bool updateParent(int id, int idparent, int idobj)
        {
            var result = false;

            var Query = getQueryUpdateParent(id);

            if (string.IsNullOrEmpty(Query))
                return result;

            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
                try
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(Query, connection);
                    command.Parameters.AddWithValue("@idP", idparent);
                    command.Parameters.AddWithValue("@idO", idobj);


                    command.CommandType = CommandType.Text;
                    var ids = command.ExecuteNonQuery();

                    result = true;

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

        public bool updateParentNull(int id, int idparent, int idobj)
        {
            var result = false;

            var Query = getQueryUpdateParentnull(id);

            if (string.IsNullOrEmpty(Query))
                return result;

            using (SqlConnection connection = new SqlConnection(_sqlClientConnectionBD.GetConnection()))
                try
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(Query, connection);
                    command.Parameters.AddWithValue("@idO", idobj);


                    command.CommandType = CommandType.Text;
                    var ids = command.ExecuteNonQuery();

                    result = true;

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

    }


    public interface IRelacionService
    {
         public List<AdminRelasionCatalogos> GetDataParent(int id, int p);
         public List<AdminRelasionCatalogos> GetData(int id);
         public List<CatalogModel> GetCatalog(int Id);

        public bool InsertParent(int id, string desc, string desc2);

        public bool updateParent(int id, int idparent, int idobj);
        public bool updateParentNull(int id, int idparent, int idobj);

    }

}
