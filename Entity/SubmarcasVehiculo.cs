using System;
using System.Collections.Generic;

namespace GuanajuatoAdminUsuarios.Entity;

public partial class SubmarcasVehiculo
{
    public int IdSubmarca { get; set; }

    public string NombreSubmarca { get; set; }

    public int? IdMarcaVehiculo { get; set; }

    public DateTime? FechaActualizacion { get; set; }

    public int? ActualizadoPor { get; set; }

    public int? estatus { get; set; }

    public virtual ICollection<MarcasVehiculo> MarcaVehiculo { get; } = new List<MarcasVehiculo>();

}
