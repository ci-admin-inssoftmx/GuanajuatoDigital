using System;
using System.Collections.Generic;

namespace GuanajuatoAdminUsuarios.Entity;

public class CatDelegacione
{
    public int IdDelegacion { get; set; }

    public string Delegacion { get; set; }

    public DateTime? FechaActualizacion { get; set; }

    public int? ActualizadoPor { get; set; }

    public int? Estatus { get; set; }

    public int? Transito { get; set; }
    
    public ICollection<CatDelegacionesOficinasTransporte> DelegacionesOficinasTransporte { get; } = new List<CatDelegacionesOficinasTransporte>(); 
}
