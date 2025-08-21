using System;
using System.Collections.Generic;
using GuanajuatoAdminUsuarios.Data.Entities2;

namespace GuanajuatoAdminUsuarios.Entity;

public class RegistraBloc
{
    public int IdOficial { get; set; }
    public int transito { get; set; }
    public int idEstatusBlock { get; set; }
    public int idUsrAlta { get; set; }
    public long RegistraId { get; set; }

    public int Delegacion { get; set; }

    public int Municipio { get; set; }

    public int TipoBlocId { get; set; }

    public string Serie { get; set; }

    public string ResponsableCarga { get; set; }

    public DateTime FechaCarga { get; set; }

    public string AsignadorBloc { get; set; }

    public DateTime? FechaAsignacion { get; set; }

    public string OficialAsignado { get; set; }

    public string UrlEvidencia { get; set; }

    public int FolioInicial { get; set; }

    public int FolioFinal { get; set; }

    public int TotalBoletas { get; set; }

    public DateTime FechaActualizacion { get; set; }

    public int? ActualizadoPor { get; set; }

    public virtual ICollection<DetalleBloc> DetalleBlocs { get; } = new List<DetalleBloc>();

    public virtual CatTipoBloc TipoBloc { get; set; }
}
