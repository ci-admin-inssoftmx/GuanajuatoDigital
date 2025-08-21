using System;
using System.Collections.Generic;

namespace GuanajuatoAdminUsuarios.Models;

public partial class DependenciasModel
{
    public int IdDependencia { get; set; }

    public string NombreDependencia { get; set; }
    public DateTime? FechaActualizacion { get; set; }

    public int? ActualizadoPor { get; set; }

    public int? Estatus { get; set; }
    public string estatusDesc { get; set; }
    public bool valorEstatusDep { get; set; }
    public int? Corp { get; set; }

}
