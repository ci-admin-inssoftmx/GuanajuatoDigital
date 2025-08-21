using System;
using System.Collections.Generic;

namespace GuanajuatoAdminUsuarios.Entity;

public partial class InfraccionesEmergencia
{
    public long EmergenciasId { get; set; }

    public int Oficina { get; set; }

    public int Delegacion { get; set; }

    public int Municipio { get; set; }

    public int IdInfraccion { get; set; }

    public string FolioEmergencia { get; set; }

    public virtual Infracciones IdInfraccionNavigation { get; set; }
}
