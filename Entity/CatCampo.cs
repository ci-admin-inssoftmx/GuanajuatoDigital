using System;
using System.Collections.Generic;

namespace GuanajuatoAdminUsuarios.Entity;

public partial class CatCampo
{
    public int IdCampo { get; set; }

    public string NombreCampo { get; set; }

    public DateTime FechaActualizacion { get; set; }

    public int ActualizadoPor { get; set; }

    public virtual ICollection<CatCamposObligatorio> CatCamposObligatorios { get; } = new List<CatCamposObligatorio>();
}
