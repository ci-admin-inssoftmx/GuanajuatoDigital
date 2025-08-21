using GuanajuatoAdminUsuarios.Data.Entities2;
using GuanajuatoAdminUsuarios.Entity;
using GuanajuatoAdminUsuarios.Helpers;
using GuanajuatoAdminUsuarios.Interfaces.Blocs;
using GuanajuatoAdminUsuarios.Models;
using GuanajuatoAdminUsuarios.Models.Blocs;
using Kendo.Mvc.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Threading.Tasks;


namespace GuanajuatoAdminUsuarios.Services.Blocs
{
    public class BlocsService : IBlocsService
    {
        private readonly DBContextInssoft dbContext;
        private readonly IHttpContextAccessor http;
        private readonly int corp;

        public BlocsService( IHttpContextAccessor _http )
        {
            dbContext = new DBContextInssoft();
            http = _http;
            corp = http.HttpContext.User.FindFirst(CustomClaims.TipoOficina).Value.toInt();
        }

        public List<BlocListItemModel> getLisBlocsConsulta()
        {

            var detalleagrupado = dbContext.DetalleBlocs.GroupBy(s => s.RegistraId)
                                  .Select(s => new { registroId = s.First().RegistraId, conteo = s.Where(s => s.idEstatusFolio == 1).Count() });


            return (from bloc in dbContext.RegistraBlocs.Where(s=>s.transito==corp)
                    join delegacion in dbContext.CatDelegaciones on bloc.Delegacion equals delegacion.IdDelegacion
                    join estatusbloc in dbContext.catEstatusBlocks on bloc.idEstatusBlock equals estatusbloc.id
                    join detalle in detalleagrupado on bloc.RegistraId equals detalle.registroId
                    join tipo in dbContext.CatTipoBlocs on bloc.TipoBlocId equals tipo.TipoBlocId
                    select new BlocListItemModel()
                    {
                        RegistraId = bloc.RegistraId,
                        Serie = bloc.Serie,
                        TotalBoletas = bloc.TotalBoletas,
                        Rango = $"{bloc.FolioInicial} - {bloc.FolioFinal}",
                        Disponibles = detalle.conteo.ToString(),
                        Oficina = delegacion.Delegacion,
                        TipoBlock = tipo.Tipo,
                        Asigna = bloc.AsignadorBloc,
                        FechAsignacion = string.Format("{0:dd/mm/yyyy HH:mm:ss}", bloc.FechaAsignacion),
                        OficialAsignado = bloc.OficialAsignado,
                        Estado = bloc.idEstatusBlock,
                        Estadodesc = estatusbloc.descripcion,
                        img = ""

                    }).ToList();
        }


        static string  ReturnBase64(string path)
        {
            if (string.IsNullOrEmpty(path)) return "";
           // if (!EsFormatoImagen(path)) return "";
            if (!System.IO.File.Exists(path)) return "";

            try
            {
                byte[] data = System.IO.File.ReadAllBytes(path);

                var provider = new Microsoft.AspNetCore.StaticFiles.FileExtensionContentTypeProvider();
                if (!provider.TryGetContentType(path, out var mimeType))
                {
                    mimeType = "application/octet-stream";
                }
                string b64 = Convert.ToBase64String(data);

                return "data:" + mimeType + ";base64," + b64;

            }
            catch (Exception ex)
            {
                return "";
            }

        }
        static bool EsFormatoImagen(string rutaArchivo)
        {
            var extensionesImagen = new[] { ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".tiff" };
            return extensionesImagen.Any(ext => rutaArchivo.EndsWith(ext, StringComparison.OrdinalIgnoreCase));
        }


        public List<BlocsAltaModel> getLisBlocsAlta()
        {


            return (from rebl in dbContext.RegistraBlocs.Where(s=>s.transito==corp)
                    join debl in dbContext.catEstatusBlocks on rebl.idEstatusBlock equals debl.id
                    select new BlocsAltaModel()
                    {
                        Boleta = rebl.RegistraId,
                        Serie = rebl.Serie,
                        CanDelete = rebl.idEstatusBlock<3,
                    }).ToList();
        }

        public List<BlocsInventarioModel> getLisBlocsInventario()
        {
            return (from rebl in dbContext.RegistraBlocs.Where(s => s.transito == corp)
                    join debl in dbContext.DetalleBlocs on rebl.RegistraId equals debl.RegistraId
                    join cade in dbContext.CatDelegaciones on rebl.Delegacion equals cade.IdDelegacion
                    join cabl in dbContext.CatTipoBlocs on rebl.TipoBlocId equals cabl.TipoBlocId
 
                    select new BlocsInventarioModel()
                    {
                        idDetalle = debl.DetalleId,
                        Serie = rebl.Serie,
                        TotalBoletas = debl.folio,
                        Folio = debl.folio.ToString(),
                        Oficina = cade.Delegacion,
                        TipoBlock = cabl.Tipo,
                        Asigna = rebl.AsignadorBloc,
                        FechAsignacion = string.Format("{0:dd/mm/yyyy HH:mm:ss}", rebl.FechaAsignacion),
                        OficialAsignado = rebl.OficialAsignado,
                        FechaCarga = string.Format("{0:dd/mm/yyyy HH:mm:ss}",rebl.FechaCarga),
                        Estado = debl.idEstatusFolio == 1 ? "DISPONIBLE" : debl.idEstatusFolio == 2? "ASIGNADO":"CANCELADO",
                        idEstatusDetalle = debl.idEstatusFolio,
                        folioInfraccion = ""



                    }).ToList();
        }

        public List<BlocsCatalogoModel> getCatBlocs()
        {
            return (from cabl in dbContext.CatTipoBlocs
                    select new BlocsCatalogoModel()
                    {
                        TipoBlocId = cabl.TipoBlocId,
                        Tipo = cabl.Tipo
                    }).ToList();
        }

        public List<BlocsCatEdosModel> getCatEstado()
        {
            return new List<BlocsCatEdosModel>()
            { 
                new BlocsCatEdosModel()
                {
                    IdEstado=1,
                    Estado = "Disponible"
                },
                new BlocsCatEdosModel()
                {
                    IdEstado=2,
                    Estado = "Asignado"
                },
                new BlocsCatEdosModel()
                {
                    IdEstado=1,
                    Estado = "Cancelado"
                }
            };   
        }

        public int IdDelegacion(string Delegacion)
        {
            return (from del in dbContext.CatDelegaciones
                    where (del.Delegacion == Delegacion)
                    select del.IdDelegacion
                    ).FirstOrDefault();
        }


        public int ExistFolio(int folio, int delegacion, int idOfical, int idflujo)
        {
            long result = 0;
             var dato = dbContext.RegistraBlocs.Where(s => s.IdOficial == idOfical && s.Delegacion == delegacion && s.TipoBlocId == idflujo)
                .Join(dbContext.DetalleBlocs, s => s.RegistraId, d => d.RegistraId, (rb, db) => db).Where(w => w.folio == folio && w.idEstatusFolio == 1).FirstOrDefault();

                if(dato != null)
            {
                result = dato.DetalleId; 
            }


            return (int)result;
        }

        public void UpdateFolio(int idFolio, int idOperacio, int flujoid)
        {

            try
            {
				var detfolio = dbContext.DetalleBlocs.Where(s => s.DetalleId == idFolio).FirstOrDefault();
				var registrob = dbContext.RegistraBlocs.Where(s => s.RegistraId == detfolio.RegistraId).FirstOrDefault();
				var countdet = dbContext.DetalleBlocs.Where(s => s.RegistraId == registrob.RegistraId && s.idEstatusFolio == 1).Count();

				if (flujoid == 1)
					detfolio.idInfraccion = idOperacio;
				if (flujoid == 2)
					detfolio.idAccidente = idOperacio;
				if (flujoid == 3)
					detfolio.idDeposito = idOperacio;

				if (countdet < 2)
				{
					registrob.idEstatusBlock = 5;
					dbContext.RegistraBlocs.Update(registrob);
				}

				detfolio.idEstatusFolio = 2;
				dbContext.DetalleBlocs.Update(detfolio);

				dbContext.SaveChanges();
			}
            catch (Exception ex) { }


		}

		public string NuevoBloc(RegistraBloc bloc)
        {
            string Resul;
            try
            {
                dbContext.RegistraBlocs.Add(bloc);
                dbContext.SaveChanges();

                long idReg = bloc.RegistraId;



                for(var i = bloc.FolioInicial; i <= bloc.FolioFinal; i++)
                {
                    DetalleBloc detalleBloc = new DetalleBloc()
                    {
                        Serie = bloc.Serie,
                        RegistraId = bloc.RegistraId,
                        FolioInicial = 0,
                        FolioFinal = 0,
                        Estatus = "",
                        idEstatusFolio = 1,
                        folio=i,
                    };
                    dbContext.DetalleBlocs.Add(detalleBloc);
                    dbContext.SaveChanges();
                }


                Resul = "OK";
            }
            catch (Exception ex)
            {
                Resul = "Err";
            }
            return Resul;
        }


       public bool ValidarFolios(int folioI, int foliofinal, int iddelegacion,int idFlujo)
        {
            bool can = false;

            can = dbContext.RegistraBlocs.Where(k => k.transito == iddelegacion && k.TipoBlocId== idFlujo)
                .Join(dbContext.DetalleBlocs, s => s.RegistraId, w => w.RegistraId, (rb, db) => db)
                .Where(w => w.folio >= folioI && w.folio <= foliofinal).Any();


            return can;
        }

        public string CalculaSerie(int idCatBloc)
        {
            string NvoSerie = "";
            int SerieNum = 1;
            try
            {
                CatTipoBloc catTipoBloc = (from cabl in dbContext.CatTipoBlocs
                                           where (cabl.TipoBlocId == idCatBloc)
                                           select cabl
                                          ).FirstOrDefault();

                RegistraBloc registraBloc = (from lebl in dbContext.RegistraBlocs
                                             where lebl.Serie.Contains(catTipoBloc.Abreviatura)
                                             orderby lebl.FechaCarga descending
                                             select lebl).FirstOrDefault();
                if (registraBloc != null)
                {
                    string SerieBloc = registraBloc.Serie.Substring(3);
                    SerieNum = Convert.ToInt32(SerieBloc) + 1;
                }
                NvoSerie = catTipoBloc.Abreviatura + SerieNum.ToString().Trim();

            }
            catch (Exception ex)
            {

            }
            return NvoSerie;
        }

        public string ElimBloc(int idBloc)
        {
            string Resul="";
            try
            {
                RegistraBloc registraBloc = (from lebl in dbContext.RegistraBlocs
                                             where lebl.RegistraId == idBloc                                             
                                             select lebl).FirstOrDefault();
                if (registraBloc == null)
                {
                    return "Err";
                }
                var hasused = dbContext.DetalleBlocs.Where(s=>s.idEstatusFolio ==2 && s.RegistraId == registraBloc.RegistraId).Count()>0;
                registraBloc.idEstatusBlock = hasused ? 4 : 3;
                dbContext.RegistraBlocs.Update(registraBloc);
                List<DetalleBloc> detalleBloc = dbContext.DetalleBlocs.Where(s=>s.RegistraId==idBloc && s.idEstatusFolio==1).ToList();
                if (detalleBloc.Count() == 0 )
                {
                    return "Err";
                }
                foreach(DetalleBloc it in detalleBloc)
                {
                    it.idEstatusFolio = 3;
                    dbContext.DetalleBlocs.Update(it);
                }

                    dbContext.SaveChanges();
                    Resul = "Ok";
                
            }
            catch (Exception ex)
            {
                Resul = "Err";
            }
            return Resul;
        }

        public BlocsAsignaDto AsignaBloc(string NSerie)
        {
            return (from lebl in dbContext.RegistraBlocs
                    join debl in dbContext.DetalleBlocs on lebl.RegistraId equals debl.RegistraId
                    join cade in dbContext.CatDelegaciones on lebl.Delegacion equals cade.IdDelegacion
                    join tibo in dbContext.CatTipoBlocs on lebl.TipoBlocId equals tibo.TipoBlocId
                    where (lebl.Serie == NSerie)
                    select new BlocsAsignaDto()
                    {
                        registraId = lebl.RegistraId,
                        txSerie = lebl.Serie,
                        txNBoletas = lebl.TotalBoletas.ToString(),
                        txDeleg = cade.Delegacion,
                        txTipoBl = tibo.Tipo,
                        txAsigna = lebl.AsignadorBloc,
                        FolioInicial = debl.FolioInicial,
                        FolioFinal = debl.FolioFinal,
                        ImgBloc = lebl.UrlEvidencia,
                        Estado = debl.Estatus
                        
                    }).FirstOrDefault();
        }

        public List<BlocsOficialDto> getCatOficial()
        {
            return (from caof in dbContext.Oficiales
                    select new BlocsOficialDto()
                    {
                        idOficial = caof.IdOficial,
                        Nombre = caof.ApellidoPaterno.Trim() + " " + caof.ApellidoMaterno.Trim() + " " + caof.Nombre.Trim()
                    }).ToList();
        }

        public string AsignarOficialBloc(long regId, int idOficial, string urlEvidencia, int folioInicial, int folioFinal, int idUsuario, string nombreUsuario,int iddelegacion,string delegacion)
        {
            string result = "";
            try
            {
                Oficiales oficiales = (
                    from oficial in dbContext.Oficiales
                    where oficial.IdOficial == idOficial
                    select oficial
                ).FirstOrDefault();

                if (oficiales == null)                
                    return "Err";
                

                RegistraBloc registraBloc = (
                    from bloc in dbContext.RegistraBlocs
                    where bloc.RegistraId == regId
                    select bloc
                ).FirstOrDefault();

                if (registraBloc == null )
                    return "Err";
                
                string nombreOficial = $"{oficiales.ApellidoPaterno.Trim()} {oficiales.ApellidoMaterno.Trim()} {oficiales.Nombre.Trim()}";

                registraBloc.OficialAsignado = nombreOficial;
                registraBloc.UrlEvidencia = urlEvidencia == "" ? registraBloc.UrlEvidencia : urlEvidencia;
                registraBloc.FechaAsignacion = DateTime.Now;
                registraBloc.AsignadorBloc = nombreUsuario;
                registraBloc.ActualizadoPor = idUsuario;
                registraBloc.idEstatusBlock = 2;
                registraBloc.IdOficial = idOficial;
                registraBloc.Delegacion = iddelegacion;

                dbContext.RegistraBlocs.Update(registraBloc);                
                
                
                dbContext.SaveChanges();

                result = "Ok";
                return result;
            }
            catch (Exception ex)
            {
                result = "Err";
            }
            return result;
        }

        public BlocsBlocbySerieDto BlocBySerie(string Serie)
        {
            return (from rebl in dbContext.RegistraBlocs
                    join del in dbContext.Delegaciones on rebl.Delegacion equals del.IdDelegacion
                    where (rebl.Serie == Serie)
                    select new BlocsBlocbySerieDto()
                    {
                        Serie = rebl.Serie,
                        Delegacion = del.Delegacion,
                        Asigna = rebl.AsignadorBloc,
                        FormatoBloc = rebl.UrlEvidencia,
                        TipoBlocId = rebl.TipoBlocId
                    }
                   ).FirstOrDefault();
        }

        public async Task<string> CancelarlBloc(long RegId, string Comen, int IdUsuario)
        {
            string Resul = "";
            try
            {

                RegistraBloc registraBloc = (from lebl in dbContext.RegistraBlocs
                                             where lebl.RegistraId == RegId
                                             select lebl).FirstOrDefault();
                if (registraBloc == null)
                {
                    return "Err";
                }
                var hasused = dbContext.DetalleBlocs.Where(s => s.idEstatusFolio == 2 && s.RegistraId == registraBloc.RegistraId).Count() > 0;
                registraBloc.idEstatusBlock = hasused ? 4 : 3;
                registraBloc.ActualizadoPor = IdUsuario;
                registraBloc.FechaActualizacion = DateTime.Now;


                dbContext.RegistraBlocs.Update(registraBloc);
                List<DetalleBloc> detalleBloc = dbContext.DetalleBlocs.Where(s => s.RegistraId == RegId && s.idEstatusFolio == 1).ToList();
                if (detalleBloc.Count() == 0)
                {
                    return "Err";
                }
                foreach (DetalleBloc it in detalleBloc)
                {
                    it.idEstatusFolio = 3;
                    dbContext.DetalleBlocs.Update(it);
                }

                dbContext.SaveChanges();
                Resul = "Ok";

            }
            catch (Exception ex)
            {
                Resul = "Err";
            }
            return Resul;
        }



        public async Task<string> CanselarFolio(long id, int IdUsuario)
        {
            string Resul = "";
            try
            {
                int idInt = int.Parse(id.ToString());

                DetalleBloc registraBloc = (from lebl in dbContext.DetalleBlocs
                                             where lebl.DetalleId == idInt
                                             select lebl).FirstOrDefault();

                RegistraBloc rb = dbContext.RegistraBlocs.Where(s => s.RegistraId == registraBloc.RegistraId).FirstOrDefault();

                if (registraBloc == null)
                {
                    return "Err";
                }
                if (rb == null)
                {
                    return "Err";
                }

                registraBloc.idEstatusFolio =  3;
                registraBloc.idUsrUsa = IdUsuario;
                

                dbContext.DetalleBlocs.Update(registraBloc);



                bool counta = dbContext.DetalleBlocs.Where(s => s.RegistraId == rb.RegistraId && s.idEstatusFolio == 1).Count() <=1;
              
                if (counta)
                {
                    bool aniasig = dbContext.DetalleBlocs.Where(s => s.RegistraId == rb.RegistraId && s.idEstatusFolio == 2).Any();

                    rb.idEstatusBlock = aniasig ? 4 : 3;
                    dbContext.RegistraBlocs.Update(rb);

                }


                dbContext.SaveChanges();
                Resul = "Ok";

            }
            catch (Exception ex)
            {
                Resul = "Err";
            }
            return Resul;
        }

        public (string B64, string mimetyte) GetBase64(int idBloc)
        {

            string b64 = "", mimetyte = "0" ;
            var bloc = dbContext.RegistraBlocs.Where(s => s.RegistraId == idBloc).FirstOrDefault();
            if (bloc == null) { return (b64, mimetyte); }
            if ( string.IsNullOrEmpty(bloc.UrlEvidencia) ) { return (b64, mimetyte); }

            var isimg = EsFormatoImagen(bloc.UrlEvidencia);
            mimetyte = isimg ? "1" : "0";
            var b64img = ReturnBase64(bloc.UrlEvidencia);
            b64 = b64img;
            return (b64, mimetyte);
        }
        public BlocModel GetBlocBySerie(string serie)
        {
            var result = 
                (from bloc in dbContext.RegistraBlocs
                join estatusBlock in dbContext.catEstatusBlocks on bloc.idEstatusBlock equals estatusBlock.id
                join delagacion in dbContext.CatDelegaciones on bloc.Delegacion equals delagacion.IdDelegacion
                join tipo in dbContext.CatTipoBlocs on bloc.TipoBlocId equals tipo.TipoBlocId
                where (bloc.Serie == serie)
                select new BlocModel
                {
                    RegistraId = bloc.RegistraId,
                    Serie = bloc.Serie,
                    TotalBoletas = bloc.TotalBoletas,
                    Delegacion = delagacion.Delegacion,
                    TipoBloc = tipo.Tipo,
                    Asigna = bloc.AsignadorBloc,
                    FolioInicial = bloc.FolioInicial,
                    FolioFinal = bloc.FolioFinal,
                    ImgaenBloc = bloc.UrlEvidencia,
                    Estado = estatusBlock.descripcion

                }).FirstOrDefault();


            var foliosUsados = dbContext.DetalleBlocs.Where(s => s.idEstatusFolio != 1 && s.RegistraId == result.RegistraId).Count();

            result.TotalBoletas = result.TotalBoletas - foliosUsados;

            return result;
        }        
    }
}
