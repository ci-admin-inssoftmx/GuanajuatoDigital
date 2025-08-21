using System;
using System.Collections.Generic;

namespace GuanajuatoAdminUsuarios.Entity;

public partial class AccidentesEmergencia
{
    public long EmergenciasId { get; set; }

    public int Oficina { get; set; }

    public int Delegacion { get; set; }

    public int Municipio { get; set; }

    public int IdAccidente { get; set; }

    public string FolioEmergencia { get; set; }

    public virtual Accidente IdAccidenteNavigation { get; set; }
}
