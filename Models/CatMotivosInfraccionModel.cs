using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GuanajuatoAdminUsuarios.Models
{
    public class CatMotivosInfraccionModel :EntityModel
    {
        public int IdCatMotivoInfraccion { get; set; }
        public string Nombre { get; set; }
        public string Fundamento { get; set; }
        public int CalificacionMinima { get; set; }
        public int CalificacionMaxima { get; set; }
        public int? idConcepto { get; set; }
        public string concepto { get; set; }
        public int idSubConcepto { get; set; }
        public int IdVigencia { get; set; }
        public int Estatus { get; set; }

        public string subConcepto { get;set; }
        public bool ValorEstatusMotivosInfraccion { get; set; }
        public string estatusDesc { get; set; }
        public string fechaInicio { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? fechaFinVigencia { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? fechaInicioVigencia { get; set; }

        public DateTime? InicioVigenciaDesde { get; set; }
        public DateTime? InicioVigenciaHasta { get; set; }
        public DateTime? FinVigenciaDesde { get; set; }
        public DateTime? FinVigenciaHasta { get; set; }

        public List<CatMotivosInfraccionModel> ListMotivosInfraccion { get; set; }
        public int? Corp { get; set; }

    }
}
