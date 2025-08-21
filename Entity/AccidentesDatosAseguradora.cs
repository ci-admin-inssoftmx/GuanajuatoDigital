using System;
using System.Collections.Generic;

namespace GuanajuatoAdminUsuarios.Entity;

public partial class AccidentesDatosAseguradora
{
    public int IdDatosAseguradora { get; set; }

    public int IdAccidente { get; set; }

    public int IdPersona { get; set; }

    public int? IdAseguradora { get; set; }

    public string Poliza { get; set; }

    public DateTime? FechaExpiracion { get; set; }
    public DateTime? fechaActualizacion { get; set; }

    public virtual CatAseguradoras IdAseguradoraNavigation { get; set; }
}
