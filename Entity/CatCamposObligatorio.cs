using System;
using System.Collections.Generic;

namespace GuanajuatoAdminUsuarios.Entity;

public partial class CatCamposObligatorio
{
    public int IdCampoObligatorio { get; set; }

    public int IdDelegacion { get; set; }

    public int IdMunicipio { get; set; }

    public int IdCampo { get; set; }

    public int Accidentes { get; set; }

    public int Infracciones { get; set; }

    public int Depositos { get; set; }

    public DateTime FechaActualizacion { get; set; }

    public int ActualizadoPor { get; set; }

    public virtual CatCampo IdCampoNavigation { get; set; }
}
