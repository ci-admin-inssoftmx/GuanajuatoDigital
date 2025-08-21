using System;
using System.Collections.Generic;

namespace GuanajuatoAdminUsuarios.Entity;

public partial class Accidente
{
    public int IdAccidente { get; set; }

    public DateTime? Fecha { get; set; }

    public TimeSpan? Hora { get; set; }

    public int? IdMunicipio { get; set; }

    public int? IdCarretera { get; set; }

    public int? IdTramo { get; set; }

    public string Kilometro { get; set; }

    public int? EstatusReporte { get; set; }

    public int? IdClasificacionAccidente { get; set; }

    public int? IdCausaAccidente { get; set; }

    public string DescripcionCausas { get; set; }

    public int? IdFactorAccidente { get; set; }

    public int? IdFactorOpcionAccidente { get; set; }

    public DateTime? FechaActualizacion { get; set; }

    public int? ActualizadoPor { get; set; }

    public int? Estatus { get; set; }

    public string MontoCamino { get; set; }

    public string MontoCarga { get; set; }

    public string MontoPropietarios { get; set; }

    public string MontoOtros { get; set; }

    public double? MontoVehiculo { get; set; }

    public int? IdElabora { get; set; }

    public int? IdSupervisa { get; set; }

    public int? IdAutoriza { get; set; }

    public int? IdElaboraConsignacion { get; set; }

    public string? Latitud { get; set; }

    public string? Longitud { get; set; }

    public int? IdCiudad { get; set; }

    public int? IdCertificado { get; set; }

    public string EntregaOtros { get; set; }

    public string EntregaObjetos { get; set; }

    public string ConsignacionHechos { get; set; }

    public string NumeroOficio { get; set; }

    public int? IdAgenciaMinisterio { get; set; }

    public string RecibeMinisterio { get; set; }

    public int? IdAutoridadEntrega { get; set; }

    public int? IdAutoridadDisposicion { get; set; }

    public int? Armas { get; set; }

    public int? Drogas { get; set; }

    public int? Valores { get; set; }

    public int? Prendas { get; set; }

    public int? Otros { get; set; }

    public int? IdEstatusReporte { get; set; }

    public int? IdOficinaDelegacion { get; set; }

    public string ArmasTexto { get; set; }

    public string DrogasTexto { get; set; }

    public string ValoresTexto { get; set; }

    public string PrendasTexto { get; set; }

    public string OtrosTexto { get; set; }

    public int? Convenio { get; set; }

    public string ObservacionesConvenio { get; set; }

    public int? IdEntidadCompetencia { get; set; }

    public int? Transito { get; set; }

    public string NumeroReporteMigrado { get; set; }

    public string NumeroReporte { get; set; }

    public string UrlAccidenteArchivosLugar { get; set; }

    public virtual ICollection<AccidentesEmergencia> AccidentesEmergencia { get; } = new List<AccidentesEmergencia>();
}
