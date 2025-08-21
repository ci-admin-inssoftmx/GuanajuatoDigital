using System.ComponentModel;

namespace GuanajuatoAdminUsuarios.Framework.Catalogs
{
    public class CatEnumerator
    {
        public enum catClasificacionGrua : int
        {
            [Description("Clasificacion1")]
            Clasificacion1 = 1,
            [Description("Clasificacion2")]
            Clasificacion2 = 2,
            [Description("Clasificacion3")]
            Clasificacion3 = 3,
            [Description("Clasificacion4")]
            Clasificacion4 = 4,
        }
        public enum catTipoGrua : int
        {
            [Description("Industrial")]
            Industrial = 1,
            [Description("Pluma")]
            Pluma = 2,
            [Description("Telescópica")]
            Telescopica = 3,
            [Description("Plataforma")]
            Plataforma = 4,
        }
        
        public enum catSituacionGrua : int
        {
            [Description("Situacion1")]
            Situacion1 = 1,
            [Description("Situacion2")]
            Situacion2 = 2,
            [Description("Situacion3")]
            Situacion3 = 3,
            [Description("Situacion4")]
            Situacion4 = 4,
        }

        public enum catEstatusInfraccion : int
        {
            [Description("En proceso")]
            EnProceso = 1,
            [Description("Capturada")]
            Capturada = 2,
            [Description("Pagada")]
            Pagada = 3,
            [Description("Pagada con descuento")]
            PagadaConDescuento = 4,
            [Description("Solventada")]
            Solventada = 5,
            [Description("Pagada con recargo")]
            PagadaConRecargo = 6,
            [Description("Enviada")]
            Enviada = 7,
        }
    }
}
