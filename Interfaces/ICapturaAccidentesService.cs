using GuanajuatoAdminUsuarios.Models;
using GuanajuatoAdminUsuarios.Models.Files;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GuanajuatoAdminUsuarios.Interfaces
{
    public interface ICapturaAccidentesService
    {
        List<CapturaAccidentesModel> ObtenerAccidentes(int idOficina);
        List<CapturaAccidentesModel> ObtenerAccidentesPagination(int idOficina, Pagination pagination);

        public int GuardarParte1(CapturaAccidentesModel model, int idOficina, string abreviaturaMunicipio, int anio, string nombreOficina = "NRA");
		public Task<bool> GuardarAccidenteEmergencia(int idMunicipio, int idAccidente, int idOficina, int? folioEmergencia);
		public CapturaAccidentesModel ObtenerAccidentePorId(int idAccidente, int idOficina);

        List<CapturaAccidentesModel> BuscarPorParametro(string Placa, string Serie, string Folio,int corp);
        List<CapturaAccidentesModel> BuscarPorParametroid(string id);


        public int ActualizarConVehiculo(int IdVehiculo, int idAccidente, int IdPersona, string Placa, string Serie, bool EsValidacion);
        public int BorrarVehiculoAccidente(int idAccidente, int idVehiculo);
        public int InsertarConductor(int IdVehiculo, int idAccidente, int IdPersona);
        public Task<int> ActualizaInfoAccidente(int idAccidente, DateTime Fecha, TimeSpan Hora, int IdMunicipio, int IdCarretera, int IdTramo, long? IdTurno,string Kilometro, int idOficina, int? IdEmergencia, int? FolioEmergencia, string Latitud, string Longitud,string observacionesTramo, string lugarCalle, string lugarNumero, string lugarColonia);


		public int AgregarValorClasificacion(int IdClasificacionAccidente, int idAccidente);

        List<CapturaAccidentesModel> ObtenerDatosGrid(int idAccidente);

        public int ClasificacionEliminar(int idAccidente, int IdClasificacionAccidente);

        List<CapturaAccidentesModel> AccidentePorID(int idAccidente);

        public int AgregarValorFactorYOpcion(int IdFactorAccidente, int IdFactorOpcionAccidente, int idAccidente);
        int EditarFactorOpcion(int IdFactorAccidente, int IdFactorOpcionAccidente, int IdAccidenteFactorOpcion, int idAccidente);
        List<CapturaAccidentesModel> ObtenerDatosGridFactor(int idAccidente);
        public int AgregarValorCausa(int IdCausaAccidente, int idAccidente);
        public void ActualizaIndiceCuasa(int idAccidenteCausa, int indice);
        public void RecalcularIndex(int IdAccidente);
        public int EditarValorCausa(int IdCausaAccidente, int idAccidenteCausa);

        List<CapturaAccidentesModel> ObtenerDatosGridCausa(int idAccidente);
        public int EliminarValorFactorYOpcion(int IdAccidenteFactorOpcion);
        public int EliminarRegistroInfraccion(int IdInfraccion);
        public int EliminarCausaBD(int IdCausaAccidente, int idAccidente, int idAccidenteCausa);
        public int GuardarDescripcion(int idAccidente, string descripcionCausa, int estatus = 1);
        public int GetEstatusAccidente(int idAccidente);

        List<CapturaAccidentesModel> BusquedaPersonaInvolucrada(BusquedaInvolucradoModel model, string server = null);

        public int AgregarPersonaInvolucrada(int idPersonaInvolucrado, int idAccidente);
        List<CapturaAccidentesModel> VehiculosInvolucradosFiltro(int IdAccidente, Pagination pagination);
        List<CapturaAccidentesModel> VehiculosInvolucrados(int IdAccidente);

        List<CapturaAccidentesModel> VehiculosInvolucradosParaInfracciones(int IdAccidente);

        CapturaAccidentesModel InvolucradoSeleccionado(int idAccidente, int IdVehiculoInvolucrado, int IdPropietarioInvolucrado);
        CapturaAccidentesModel ObtenerConductorPorId(int IdPersona);
        public int GuardarComplementoVehiculo(CapturaAccidentesModel model, int IdVehiculo, int idAccidente, string numerosEconomicos, int idConcesionario, string idGruas);
        int AgregarMontoV(MontoModel model);
        List<CapturaAccidentesModel> InfraccionesVehiculosAccidete(int idAccidente);
        public int RelacionAccidenteInfraccion(int IdVehiculo, int idAccidente, int IdInfraccion);
        public int RelacionAccidenteInfraccion2(int IdVehiculo, int idAccidente, int IdInfraccion);
        public int VerificarExistenciaInfraccion(int IdVehiculo, int idAccidente);

        List<CapturaAccidentesModel> InfraccionesDeAccidente(int idAccidente);
        public int RelacionPersonaVehiculo(int IdPersona, int idAccidente, int IdVehiculoInvolucrado, int IdInvolucrado);
        public int ActualizarInvolucrado(CapturaAccidentesModel model, int idAccidente);
        public int ActualizarInvolucrado2(CapturaAccidentesModel model, int idAccidente);
        public bool InsertaDatosAseguradora(CapturaAccidentesModel model, int idAccidente);
        public int ActualizarRelacionVehiculoPersona(CapturaAccidentesModel model, int idAccidente);

        List<CapturaAccidentesModel> InvolucradosAccidente(int idAccidente);
        int AgregarFechaHoraIngreso(FechaHoraIngresoModel model, int idAccidente);
        int AgregarAccidenteFolioDetencion(FechaHoraIngresoModel model, int idAccidente);
        int GuardarDatosPrevioInfraccion(int idAccidente, string montoCamino, string montoCarga, string montoPropietarios, string montoOtros);
        int AgregarDatosFinales(DatosAccidenteModel datosAccidente, int armasValue, int drogasValue, int valoresValue, int prendasValue, int otrosValue, int idAccidente, int convenioValue);
        int EliminarInvolucradoAcc(int IdVehiculoInvolucrado, int IdPropietarioInvolucrado, int IdAccidente);
        int AnexarBoleta(DatosAccidenteModel datosAccidente, int idAccidente); 
        int AnexarArchivoParte(DatosAccidenteModel datosAccidente, int idAccidente);
        int AnexarArchivoInvolucrado(DatosAccidenteModel datosAccidente, int idAccidente);
        int AnexarArchivoCroquis(DatosAccidenteModel datosAccidente, int idAccidente);

        CatalogModel GetBoletaPath(int idAccidente); 
        CatalogModel GetArchivoPartePath(int idAccidente);
        CatalogModel GetArchivoInvolucradoPath(int IdAccidente,int IdInvolucrado);
		public int EliminarInvolucrado(int idPersona);

        public int EditarInvolucrado(CapturaAccidentesModel model);
        public int RegistrarInfraccion(NuevaInfraccionModel model, int idDependencia);
        public string ObtenerDescripcionCausaDesdeBD(int idAccidente);
        public DatosAccidenteModel ObtenerDatosFinales(int idAccidente);
        public bool ValidarFolio(string folioInfraccion, int idDependencia);

        CapturaAccidentesModel ObtenerDetallePersona(int Id);
        CapturaAccidentesModel DatosInvolucradoEdicion(int Id, int idAccidente, int IdInvolucrado);
        string GetAccidenteFolioDetencion(int idAccidente, int idPersona);

		IEnumerable<FileData> GetArchivosLugarAccidente(int accidenteId);
		
        void GuardarArchivosLugarAccidente(int accidenteId, IEnumerable<IFormFile> files);		
		
        void EliminarArchivosLugarAccidente(int accidenteId, IEnumerable<string> files);
	}

}
