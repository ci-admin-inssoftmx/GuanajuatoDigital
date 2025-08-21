using System;
using System.Collections.Generic;

namespace GuanajuatoAdminUsuarios.Entity;

public partial class CatAseguradoras
{
    public int IdAseguradora { get; set; }

    public string NombreAseguradora { get; set; }

    public DateTime FechaActualizacion { get; set; }

    public int ActualizadoPor { get; set; }

    public int Estatus { get; set; }
    public virtual ICollection<AccidentesDatosAseguradora> AccidentesDatosAseguradoras { get; } = new List<AccidentesDatosAseguradora>();
}
