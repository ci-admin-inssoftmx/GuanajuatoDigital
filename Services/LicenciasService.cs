using GuanajuatoAdminUsuarios.Entity;
using GuanajuatoAdminUsuarios.Interfaces;
using GuanajuatoAdminUsuarios.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;

namespace GuanajuatoAdminUsuarios.Services
{
    public class LicenciasService : ILicenciasService
    {
        private GuanajuatoLicenciasContext _contextLicencias;
       // private GuanajuatoIncidenciasMigContext _contextIncidencias;
        public LicenciasService(GuanajuatoLicenciasContext contextLicencias)//GuanajuatoLicenciasContext contextLicencias, GuanajuatoIncidenciasMigContext contextIncidencias)
        {
            _contextLicencias = contextLicencias;
      //      _contextIncidencias = contextIncidencias;
        }

        public List<LicenciaPersonaDatos> ObtenerDatosPersona(string licencia, string curp, string rfc, string nombre, string primer_apellido, string segundo_apellido)
        {

           // var _contextLicencias = new GuanajuatoLicenciasContext();

            string condiciones = "";
            condiciones += string.IsNullOrEmpty(licencia) ? "" : " AND P.NUM_LICENCIA = @licencia ";
            condiciones += string.IsNullOrEmpty(curp) ? "" : " AND P.CURP = @curp ";
            condiciones += string.IsNullOrEmpty(rfc) ? "" : " AND P.RFC like '%'+ @rfc +'%' COLLATE Latin1_general_CI_AI ";
            condiciones += string.IsNullOrEmpty(nombre) ? "" : " AND p.NOMBRE like '%'+ @nombre +'%' COLLATE Latin1_general_CI_AI ";
            condiciones += string.IsNullOrEmpty(primer_apellido) ? "" : " AND p.PRIMER_APELLIDO LIKE '%'+ @primer_apellido +'%'  COLLATE Latin1_general_CI_AI ";
            condiciones += string.IsNullOrEmpty(segundo_apellido) ? "" : " AND p.SEGUNDO_APELLIDO LIKE '%'+ @segundo_apellido +'%'  COLLATE Latin1_general_CI_AI ";

            List<DbParameter> parametros = new List<DbParameter>();
            if (!string.IsNullOrEmpty(licencia))
                parametros.Add(new SqlParameter() { ParameterName = "@licencia", Value = licencia });

            if (!string.IsNullOrEmpty(curp))
                parametros.Add(new SqlParameter() { ParameterName = "@curp", Value = curp });

            if (!string.IsNullOrEmpty(rfc))
                parametros.Add(new SqlParameter() { ParameterName = "@rfc", Value = rfc });

            if (!string.IsNullOrEmpty(nombre))
                parametros.Add(new SqlParameter() { ParameterName = "@nombre", Value = nombre });

            if (!string.IsNullOrEmpty(primer_apellido))
                parametros.Add(new SqlParameter() { ParameterName = "@primer_apellido", Value = primer_apellido });

            if (!string.IsNullOrEmpty(segundo_apellido))
                parametros.Add(new SqlParameter() { ParameterName = "@segundo_apellido", Value = segundo_apellido });
          //  +++modificar el query
            string queryDatosPersona = @"SELECT  p.ID_PERSONA
                                        , p.NOMBRE_COMPLETO 
                                        , p.NOMBRE, p.PRIMER_APELLIDO ,p.SEGUNDO_APELLIDO 
                                        , p.FECHA_NACIMIENTO ,p.CURP ,p.RFC 
                                        , g.ID_GENERO ,g.DESCRIPCION AS GENERO
                                        , p.NUM_LICENCIA , l.ID_LICENCIA 
                                        , en.DESCRIPCION as ESTADO_NACIMIENTO
                                        , n.DESCRIPCION AS NACIONALIDAD
                                        , l.ID_TIPO_LICENCIA, ctl.DESCRIPCION AS TIPOLICENCIA
                                        , l.FECHA_INICIO_VIGENCIA
                                        , l.FECHA_TERMINO_VIGENCIA
                                        , D.ID_MUNICIPIO,	
										tv.AÑOS_VIGENCIA as Vigencia
                                        ,
										case  
											when d.ID_COLONIA = 100001 and ld.ID_LICENCIA is not null then LD.CP
											ELSE
												c.CP END CP,
										case  
											when d.ID_COLONIA = 100001 and ld.ID_LICENCIA is not null then ld.COLONIA
											else
											isnull(d.COLONIA_OTRA,c.DESCRIPCION) end COLONIA,										
										case  
											when d.ID_COLONIA = 100001 and ld.ID_LICENCIA is not null  then ld.CALLE
											else
												D.CALLE end CALLE,
										case  
											when d.ID_COLONIA = 100001 and ld.ID_LICENCIA is not null then ld.NUM_EXTERIOR
											else
												D.NUM_EXT end NUM_EXT,
										case  
											when d.ID_COLONIA = 100001 and ld.ID_LICENCIA is not null then ld.MUNICIPIO
											else
											m.DESCRIPCION end AS MUNICIPIO,
										case  
											when d.ID_COLONIA = 100001 and ld.ID_LICENCIA is not null then ld.NUM_INTERIOR
										 else
											D.NUM_INT end NUM_INT,											
										e.DESCRIPCION AS  ESTADO										
                                        ,P.TELEFONO1 , P.EMAIL 
                                       
									   FROM PERSONAS p 
                                            JOIN CAT_GENERO g on p.ID_GENERO = g.ID_GENERO
	                                        JOIN LICENCIAS l ON p.id_persona = l.ID_PERSONA
	                                        join CAT_DOM_ESTADOS e ON p.ID_ESTADO_NACIMIENTO = e.ID_ESTADO
	                                        join CAT_NACIONALIDAD n ON p.ID_NACIONALIDAD = n.ID_NACIONALIDAD
	                                        join CAT_TIPOS_LICENCIA ctl ON l.ID_TIPO_LICENCIA = ctl.ID_TIPO_LICENCIA
	                                        JOIN DOMICILIOS d ON l.ID_DOMICILIO = d.ID_DOMICILIO
	                                        JOIN CAT_DOM_MUNICIPIOS m ON d.ID_MUNICIPIO = m.ID_MUNICIPIO
	                                        join CAT_DOM_COLONIAS c on d.ID_COLONIA = c.ID_COLONIA 
											join CAT_DOM_ESTADOS en ON d.ID_ESTADO = en.ID_ESTADO
											join CAT_TIPOS_LICENCIA_VIGENCIA tv on tv.ID_TIPO_LICENCIA_VIGENCIA = l.ID_TIPO_LICENCIA_VIGENCIA
											join LICENCIAS_DOMICILIOS Ld on ld.ID_LICENCIA = l.ID_LICENCIA

										WHERE l.ID_LICENCIA = (SELECT MAX(l2.ID_LICENCIA)
                                                                FROM LICENCIAS l2
																WHERE p.ID_PERSONA = l2.ID_PERSONA
																AND L2.ESTATUS = 9							
																)

                                                
			                              " + condiciones ;
			_contextLicencias.Database.SetCommandTimeout(180);
			//   LicenciaPersonaDatos persona = _contextLicencias.personaDatos.FromSqlRaw(queryDatosPersona, parametros.ToArray()).FirstOrDefault();
			var encontrados = _contextLicencias.personaDatos.FromSqlRaw(queryDatosPersona, parametros.ToArray()).ToList();
            return encontrados;

        }

        public LicenciaPersonaDatos ObtenerDatosPersonaBD1(string licencia, string curp, string rfc, string nombre, string primer_apellido, string segundo_apellido)
        {

            var _contextIncidencias = new GuanajuatoLicenciasContext();

            string condiciones = "";
            condiciones += string.IsNullOrEmpty(licencia) ? "" : " AND l.NUM_LICENCIA = @licencia ";
            condiciones += string.IsNullOrEmpty(curp) ? "" : " AND P.CURP = @curp ";
            condiciones += string.IsNullOrEmpty(rfc) ? "" : " AND P.RFC like '%'+ @rfc +'%' COLLATE Latin1_general_CI_AI ";
            condiciones += string.IsNullOrEmpty(nombre) ? "" : " AND p.NOMBRE like '%'+ @nombre +'%' COLLATE Latin1_general_CI_AI ";
            condiciones += string.IsNullOrEmpty(primer_apellido) ? "" : " AND p.PRIMER_APELLIDO LIKE '%'+ @primer_apellido +'%'  COLLATE Latin1_general_CI_AI ";
            condiciones += string.IsNullOrEmpty(segundo_apellido) ? "" : " AND p.SEGUNDO_APELLIDO LIKE '%'+ @segundo_apellido +'%'  COLLATE Latin1_general_CI_AI ";

            List<DbParameter> parametros = new List<DbParameter>();
            if (!string.IsNullOrEmpty(licencia))
                parametros.Add(new SqlParameter() { ParameterName = "@licencia", Value = licencia });

            if (!string.IsNullOrEmpty(curp))
                parametros.Add(new SqlParameter() { ParameterName = "@curp", Value = curp });

            if (!string.IsNullOrEmpty(rfc))
                parametros.Add(new SqlParameter() { ParameterName = "@rfc", Value = rfc });

            if (!string.IsNullOrEmpty(nombre))
                parametros.Add(new SqlParameter() { ParameterName = "@nombre", Value = nombre });

            if (!string.IsNullOrEmpty(primer_apellido))
                parametros.Add(new SqlParameter() { ParameterName = "@primer_apellido", Value = primer_apellido });

            if (!string.IsNullOrEmpty(segundo_apellido))
                parametros.Add(new SqlParameter() { ParameterName = "@segundo_apellido", Value = segundo_apellido });

            string queryDatosPersona = @"SELECT TOP 1 l.ID_PERSONA 
                                        ,p.NOMBRE_COMPLETO ,p.NOMBRE ,p.PRIMER_APELLIDO ,p.SEGUNDO_APELLIDO
                                        ,p.FECHA_NACIMIENTO ,p.CURP ,p.RFC 
                                        ,CASE WHEN p.GENERO = 'H' THEN 1
	                                          WHEN p.GENERO = 'M' THEN 2
	                                          ELSE 6
	                                          END AS ID_GENERO
                                        ,CASE WHEN p.GENERO = 'H' THEN 'MASCULINO'
	                                          WHEN p.GENERO = 'M' THEN 'FEMENINO'
	                                          ELSE 'INDEFINIDO'
	                                          END AS GENERO
                                        , l.NUM_LICENCIA , l.ID_LICENCIA, p.ESTADO_NACIMIENTO  , p.NACIONALIDAD 
                                        , NULL ID_TIPO_LICENCIA , l.TIPO_LICENCIA AS TIPOLICENCIA
                                        , l.FECHA_INICIO_VIGENCIA , l.FECHA_TERMINO_VIGENCIA 
                                        , NULL ID_MUNICIPIO , d.MUNICIPIO 
                                        , NULL CP , d.COLONIA , d.CALLE , d.NUM_EXT , d.NUM_INT
                                        , p.TELEFONO1, p.EMAIL 
                                        FROM LICENCIAS_BD1 l  
	                                        LEFT OUTER JOIN DOMICILIOS_BD1 d ON l.ID_DOMICILIO = d.ID_DOMICILIO
	                                        JOIN PERSONAS_BD1 p ON p.ID_PERSONA = l.ID_PERSONA 
	                                        JOIN DOMICILIOS_BD1 db ON db.ID_PERSONA = d.ID_PERSONA 
	                                        WHERE l.FOLIO_LICENCIA is not NULL  " + condiciones + " ORDER BY l.ID_LICENCIA DESC, d.ID_DOMICILIO DESC ";

            LicenciaPersonaDatos persona = _contextIncidencias.personaDatos.FromSqlRaw(queryDatosPersona, parametros.ToArray()).FirstOrDefault();
            return persona;
        }

        public LicenciaPersonaDatos ObtenerDatosPersonaBD2(string licencia, string curp, string rfc, string nombre, string primer_apellido, string segundo_apellido)
        {
            var _contextIncidencias = new GuanajuatoLicenciasContext();
            string condiciones = "";
            condiciones += string.IsNullOrEmpty(licencia) ? "" : " AND l.NUM_LICENCIA = @licencia ";
            condiciones += string.IsNullOrEmpty(curp) ? "" : " AND P.CURP = @curp ";
            condiciones += string.IsNullOrEmpty(rfc) ? "" : " AND P.RFC like '%'+ @rfc +'%' COLLATE Latin1_general_CI_AI ";
            condiciones += string.IsNullOrEmpty(nombre) ? "" : " AND p.NOMBRE like '%'+ @nombre +'%' COLLATE Latin1_general_CI_AI ";
            condiciones += string.IsNullOrEmpty(primer_apellido) ? "" : " AND p.PRIMER_APELLIDO LIKE '%'+ @primer_apellido +'%'  COLLATE Latin1_general_CI_AI ";
            condiciones += string.IsNullOrEmpty(segundo_apellido) ? "" : " AND p.SEGUNDO_APELLIDO LIKE '%'+ @segundo_apellido +'%'  COLLATE Latin1_general_CI_AI ";

            List<DbParameter> parametros = new List<DbParameter>();
            if (!string.IsNullOrEmpty(licencia))
                parametros.Add(new SqlParameter() { ParameterName = "@licencia", Value = licencia });

            if (!string.IsNullOrEmpty(curp))
                parametros.Add(new SqlParameter() { ParameterName = "@curp", Value = curp });

            if (!string.IsNullOrEmpty(rfc))
                parametros.Add(new SqlParameter() { ParameterName = "@rfc", Value = rfc });

            if (!string.IsNullOrEmpty(nombre))
                parametros.Add(new SqlParameter() { ParameterName = "@nombre", Value = nombre });

            if (!string.IsNullOrEmpty(primer_apellido))
                parametros.Add(new SqlParameter() { ParameterName = "@primer_apellido", Value = primer_apellido });

            if (!string.IsNullOrEmpty(segundo_apellido))
                parametros.Add(new SqlParameter() { ParameterName = "@segundo_apellido", Value = segundo_apellido });

            string queryDatosPersona = @"SELECT TOP 1 l.ID_PERSONA 
                                        ,p.NOMBRE_COMPLETO ,p.NOMBRE ,p.PRIMER_APELLIDO ,p.SEGUNDO_APELLIDO
                                        ,p.FECHA_NACIMIENTO ,p.CURP ,p.RFC 
                                        ,CASE WHEN p.GENERO = 'H' THEN 1
	                                            WHEN p.GENERO = 'M' THEN 2
	                                            ELSE 6
	                                            END AS ID_GENERO
                                        ,CASE WHEN p.GENERO = 'H' THEN 'MASCULINO'
	                                            WHEN p.GENERO = 'M' THEN 'FEMENINO'
	                                            ELSE 'INDEFINIDO'
	                                            END AS GENERO
                                        , l.NUM_LICENCIA , l.ID_LICENCIA, p.ESTADO_NACIMIENTO  , p.NACIONALIDAD 
                                        , NULL ID_TIPO_LICENCIA , l.TIPO_LICENCIA AS TIPOLICENCIA
                                        , l.FECHA_INICIO_VIGENCIA , l.FECHA_TERMINO_VIGENCIA 
                                        , NULL ID_MUNICIPIO , d.MUNICIPIO 
                                        , NULL CP , d.COLONIA , d.CALLE , d.NUM_EXT , d.NUM_INT
                                        , p.TELEFONO1, p.EMAIL 
                                        FROM LICENCIAS_BD2 l  
	                                        LEFT OUTER JOIN DOMICILIOS_BD2 d ON l.ID_DOMICILIO = d.ID_DOMICILIO
	                                        JOIN PERSONAS_BD2 p ON p.ID_PERSONA = l.ID_PERSONA 
	                                        JOIN DOMICILIOS_BD2 db ON db.ID_PERSONA = d.ID_PERSONA 
	                                        WHERE l.FOLIO_LICENCIA is not NULL   " + condiciones + " ORDER BY l.ID_LICENCIA DESC, d.ID_DOMICILIO DESC ";


            LicenciaPersonaDatos persona = _contextIncidencias.personaDatos.FromSqlRaw(queryDatosPersona, parametros.ToArray()).FirstOrDefault();
            return persona;
        }

        public LicenciaPersonaDatos ObtenerDatosPersonaBD3(string licencia, string curp, string rfc, string nombre, string primer_apellido, string segundo_apellido)
        {
            var _contextIncidencias = new GuanajuatoLicenciasContext();
            string condiciones = "";
            condiciones += string.IsNullOrEmpty(licencia) ? "" : " AND l.NUM_LICENCIA = @licencia ";
            condiciones += string.IsNullOrEmpty(curp) ? "" : " AND P.CURP = @curp ";
            condiciones += string.IsNullOrEmpty(rfc) ? "" : " AND P.RFC like '%'+ @rfc +'%' COLLATE Latin1_general_CI_AI ";
            condiciones += string.IsNullOrEmpty(nombre) ? "" : " AND p.NOMBRE like '%'+ @nombre +'%' COLLATE Latin1_general_CI_AI ";
            condiciones += string.IsNullOrEmpty(primer_apellido) ? "" : " AND p.PRIMER_APELLIDO LIKE '%'+ @primer_apellido +'%'  COLLATE Latin1_general_CI_AI ";
            condiciones += string.IsNullOrEmpty(segundo_apellido) ? "" : " AND p.SEGUNDO_APELLIDO LIKE '%'+ @segundo_apellido +'%'  COLLATE Latin1_general_CI_AI ";

            List<DbParameter> parametros = new List<DbParameter>();
            if (!string.IsNullOrEmpty(licencia))
                parametros.Add(new SqlParameter() { ParameterName = "@licencia", Value = licencia });

            if (!string.IsNullOrEmpty(curp))
                parametros.Add(new SqlParameter() { ParameterName = "@curp", Value = curp });

            if (!string.IsNullOrEmpty(rfc))
                parametros.Add(new SqlParameter() { ParameterName = "@rfc", Value = rfc });

            if (!string.IsNullOrEmpty(nombre))
                parametros.Add(new SqlParameter() { ParameterName = "@nombre", Value = nombre });

            if (!string.IsNullOrEmpty(primer_apellido))
                parametros.Add(new SqlParameter() { ParameterName = "@primer_apellido", Value = primer_apellido });

            if (!string.IsNullOrEmpty(segundo_apellido))
                parametros.Add(new SqlParameter() { ParameterName = "@segundo_apellido", Value = segundo_apellido });

            string queryDatosPersona = @"SELECT TOP 1  l.ID_PERSONA 
                                        ,p.NOMBRE_COMPLETO ,p.NOMBRE ,p.PRIMER_APELLIDO ,p.SEGUNDO_APELLIDO
                                        ,p.FECHA_NACIMIENTO ,p.CURP ,p.RFC 
                                        ,CASE WHEN p.GENERO = 'MASCULINO' THEN 1
	                                          WHEN p.GENERO = 'FEMENINO' THEN 2
	                                          ELSE 6
	                                          END AS ID_GENERO
                                        ,P.GENERO
                                        , l.NUM_LICENCIA , l.ID_LICENCIA, p.ESTADO_NACIMIENTO  , p.NACIONALIDAD 
                                        , NULL ID_TIPO_LICENCIA , l.TIPO_LICENCIA AS TIPOLICENCIA
                                        , l.FECHA_INICIO_VIGENCIA , l.FECHA_TERMINO_VIGENCIA 
                                        , NULL ID_MUNICIPIO , d.MUNICIPIO 
                                        , d.CP_GTO CP , d.COLONIA , d.CALLE , d.NUM_EXT , d.NUM_INT
                                        , p.TELEFONO1, p.EMAIL 
                                        FROM LICENCIAS_BD3 l  
	                                        LEFT OUTER JOIN DOMICILIOS_BD3 d ON l.ID_DOMICILIO = d.ID_DOMICILIO
	                                        JOIN PERSONAS_BD3 p ON p.ID_PERSONA = l.ID_PERSONA 
	                                        JOIN DOMICILIOS_BD3 db ON db.ID_PERSONA = d.ID_PERSONA 
	                                        WHERE p.ID_PERSONA > 0   " + condiciones + " ORDER BY l.ID_LICENCIA DESC, d.ID_DOMICILIO DESC  ";


            LicenciaPersonaDatos persona = _contextIncidencias.personaDatos.FromSqlRaw(queryDatosPersona, parametros.ToArray()).FirstOrDefault();
            return persona;
        }

       
    }
}
