using System;

namespace GuanajuatoAdminUsuarios.Models
{
    public class EstadisticaAccidentesModel
    {
        public string Motivo { get; set; }
        public int Contador { get; set; }
    }


    public class ListadoAccidentesPorAccidenteModel
    {
		public int Total { get; set; }
        public int Numero { get; set; }

        public string Numreporteaccidente { get; set; }
        public string Fecha { get; set; }
        public string Hora { get; set; }
        public string Delegacion { get; set; }
        public string municipio { get; set; }
		public string carretera { get; set; }	
		public string tramo { get; set; }
        public string kilometro { get; set; }
        public string latitud { get; set; }
        public string longitud { get; set; }
        public string Vehiculo { get; set; }
        public string NombredelOficial { get; set; }
        public string Dañosalcamino { get; set; }
        public string Dañosacarga { get; set; }
        public string Dañosapropietario { get; set; }
        public string Otrosdaños { get; set; }
        public string Lesionados { get; set; }
        public string Muertos { get; set; }
        public string FactoresOpciones { get; set; }
        public string Causas { get; set; }
        public string CausasDescripcion { get; set; }
		public int NumeroSecuencial { get; set; }
		public int idMunicipio { get; set; }
		public int idDelegacion { get; set; }
        
		public int IdOficial { get; set; }
		public int idCarretera { get; set; }
		public int idTramo { get; set; }
		public int idClasificacionAccidente { get; set; }
		public int idTipoLicencia { get; set; }
		public int idCausaAccidente { get; set; }
		public int idFactorAccidente { get; set; }
		public int IdTipoVehiculo { get; set; }
		public int IdTipoServicio { get; set; }
		public int IdSubtipoServicio { get; set; }

		public int idFactorOpcionAccidente { get; set; }

	}

    public class ListadoAccidentesPorVehiculoModel
    {
        public int Total { get; set; }
        public int Numero { get; set; }

        public string Numreporteaccidente { get; set; }
        public string NumVeh { get; set; }
        public string PlacasVeh { get; set; }
        public string SerieVeh { get; set; }
        public string PropietarioVeh { get; set; }
        public string TipoVeh { get; set; }
        public string ServicioVeh { get; set; }
        public string Marca { get; set; }
        public string Submarca { get; set; }
        public string Modelo { get; set; }
        public string ConductorVeh { get; set; }






        public string? NumVeh1 { get; set; }
        public string? PlacasVeh1 { get; set; }
        public string? SerieVeh1 { get; set; }
        public string? PropietarioVeh1 { get; set; }
        public string? TipoVeh1 { get; set; }
        public string? ServicioVeh1 { get; set; }
        public string? Marca1 { get; set; }
        public string? Submarca1 { get; set; }
        public string? Modelo1 { get; set; }
        public string? ConductorVeh1 { get; set; }

        public string? NumVeh2 { get; set; }
        public string? PlacasVeh2 { get; set; }
        public string? SerieVeh2 { get; set; }
        public string? PropietarioVeh2 { get; set; }
        public string? TipoVeh2 { get; set; }
        public string? ServicioVeh2 { get; set; }
        public string? Marca2 { get; set; }
        public string? Submarca2 { get; set; }
        public string? Modelo2 { get; set; }
        public string? ConductorVeh2 { get; set; }


        public string? NumVeh3 { get; set; }
        public string? PlacasVeh3 { get; set; }
        public string? SerieVeh3 { get; set; }
        public string? PropietarioVeh3 { get; set; }
        public string? TipoVeh3 { get; set; }
        public string? ServicioVeh3 { get; set; }
        public string? Marca3 { get; set; }
        public string? Submarca3 { get; set; }
        public string? Modelo3 { get; set; }
        public string? ConductorVeh3 { get; set; }





        public string? NumVeh4 { get; set; }
        public string? PlacasVeh4 { get; set; }
        public string? SerieVeh4 { get; set; }
        public string? PropietarioVeh4 { get; set; }
        public string? TipoVeh4 { get; set; }
        public string? ServicioVeh4 { get; set; }
        public string? Marca4 { get; set; }
        public string? Submarca4 { get; set; }
        public string? Modelo4 { get; set; }
        public string? ConductorVeh4 { get; set; }


        public string? NumVeh5 { get; set; }
        public string? PlacasVeh5 { get; set; }
        public string? SerieVeh5 { get; set; }
        public string? PropietarioVeh5 { get; set; }
        public string? TipoVeh5 { get; set; }
        public string? ServicioVeh5 { get; set; }
        public string? Marca5 { get; set; }
        public string? Submarca5 { get; set; }
        public string? Modelo5 { get; set; }
        public string? ConductorVeh5 { get; set; }

        public string? NumVeh6 { get; set; }
        public string? PlacasVeh6 { get; set; }
        public string? SerieVeh6 { get; set; }
        public string? PropietarioVeh6 { get; set; }
        public string? TipoVeh6 { get; set; }
        public string? ServicioVeh6 { get; set; }
        public string? Marca6 { get; set; }
        public string? Submarca6 { get; set; }
        public string? Modelo6 { get; set; }
        public string? ConductorVeh6 { get; set; }



        public string? NumVeh7 { get; set; }
        public string? PlacasVeh7 { get; set; }
        public string? SerieVeh7 { get; set; }
        public string? PropietarioVeh7 { get; set; }
        public string? TipoVeh7 { get; set; }
        public string? ServicioVeh7 { get; set; }
        public string? Marca7 { get; set; }
        public string? Submarca7 { get; set; }
        public string? Modelo7 { get; set; }
        public string? ConductorVeh7 { get; set; }

        public string? NumVeh8 { get; set; }
        public string? PlacasVeh8 { get; set; }
        public string? SerieVeh8 { get; set; }
        public string? PropietarioVeh8 { get; set; }
        public string? TipoVeh8 { get; set; }
        public string? ServicioVeh8 { get; set; }
        public string? Marca8 { get; set; }
        public string? Submarca8 { get; set; }
        public string? Modelo8 { get; set; }
        public string? ConductorVeh8 { get; set; }

        public string? NumVeh9 { get; set; }
        public string? PlacasVeh9 { get; set; }
        public string? SerieVeh9 { get; set; }
        public string? PropietarioVeh9 { get; set; }
        public string? TipoVeh9 { get; set; }
        public string? ServicioVeh9 { get; set; }
        public string? Marca9 { get; set; }
        public string? Submarca9 { get; set; }
        public string? Modelo9 { get; set; }
        public string? ConductorVeh9 { get; set; }


        public string? NumVeh10 { get; set; }
        public string? PlacasVeh10 { get; set; }
        public string? SerieVeh10 { get; set; }
        public string? PropietarioVeh10 { get; set; }
        public string? TipoVeh10 { get; set; }
        public string? ServicioVeh10 { get; set; }
        public string? Marca10 { get; set; }
        public string? Submarca10 { get; set; }
        public string? Modelo10 { get; set; }
        public string? ConductorVeh10 { get; set; }


        public string? NumVeh11 { get; set; }
        public string? PlacasVeh11 { get; set; }
        public string? SerieVeh11 { get; set; }
        public string? PropietarioVeh11 { get; set; }
        public string? TipoVeh11 { get; set; }
        public string? ServicioVeh11 { get; set; }
        public string? Marca11 { get; set; }
        public string? Submarca11 { get; set; }
        public string? Modelo11 { get; set; }
        public string? ConductorVeh11 { get; set; }


        public string? NumVeh12 { get; set; }
        public string? PlacasVeh12 { get; set; }
        public string? SerieVeh12 { get; set; }
        public string? PropietarioVeh12 { get; set; }
        public string? TipoVeh12 { get; set; }
        public string? ServicioVeh12 { get; set; }
        public string? Marca12 { get; set; }
        public string? Submarca12 { get; set; }
        public string? Modelo12 { get; set; }
        public string? ConductorVeh12 { get; set; }


        public string? NumVeh13 { get; set; }
        public string? PlacasVeh13 { get; set; }
        public string? SerieVeh13 { get; set; }
        public string? PropietarioVeh13 { get; set; }
        public string? TipoVeh13 { get; set; }
        public string? ServicioVeh13 { get; set; }
        public string? Marca13 { get; set; }
        public string? Submarca13 { get; set; }
        public string? Modelo13 { get; set; }
        public string? ConductorVeh13 { get; set; }


        public string? NumVeh14 { get; set; }
        public string? PlacasVeh14 { get; set; }
        public string? SerieVeh14 { get; set; }
        public string? PropietarioVeh14 { get; set; }
        public string? TipoVeh14 { get; set; }
        public string? ServicioVeh14 { get; set; }
        public string? Marca14 { get; set; }
        public string? Submarca14 { get; set; }
        public string? Modelo14 { get; set; }
        public string? ConductorVeh14 { get; set; }


        public string? NumVeh15 { get; set; }
        public string? PlacasVeh15 { get; set; }
        public string? SerieVeh15 { get; set; }
        public string? PropietarioVeh15 { get; set; }
        public string? TipoVeh15 { get; set; }
        public string? ServicioVeh15 { get; set; }
        public string? Marca15 { get; set; }
        public string? Submarca15 { get; set; }
        public string? Modelo15 { get; set; }
        public string? ConductorVeh15 { get; set; }














        public string DepositoVehículo { get; set; }
        public string Delegacion { get; set; }
        public string Municipio { get; set; }
        public string Carretera { get; set; }
        public string Tramo { get; set; }
        public string Kilómetro { get; set; }
        public string Latitud { get; set; }
        public string Longitud { get; set; }
        public string NombredelOficial { get; set; }
        public string Dañosalcamino { get; set; }
        public string DañosaCarga { get; set; }
        public string Dañosapropietario { get; set; }
        public string Otrosdaños { get; set; }
        public string Lesionados { get; set; }
        public string Muertos { get; set; }
        public string Causas { get; set; }
        public string CausasDescripcion { get; set; }
		public int NumeroContinuo { get; set; }	
		public DateTime fecha { get; set; }
		public TimeSpan hora { get; set; }
		public int idMunicipio { get; set; }
		public int idDelegacion { get; set; }


		public int IdOficial { get; set; }
		public int idCarretera { get; set; }
		public int idTramo { get; set; }
		public int idClasificacionAccidente { get; set; }
		public int idTipoLicencia { get; set; }
		public int idCausaAccidente { get; set; }
		public int idFactorAccidente { get; set; }
		public int IdTipoVehiculo { get; set; }
		public int IdTipoServicio { get; set; }
		public int IdSubtipoServicio { get; set; }

		public int idFactorOpcionAccidente { get; set; }





	}
}
