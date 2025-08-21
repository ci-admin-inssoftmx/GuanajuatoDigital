namespace GuanajuatoAdminUsuarios.Models
{
    public class AdminCatalogosModel
    {
        public int idCatalogo { get; set; }
        public int idDependencia { get; set; }
        public string catalogo { get; set; }
        public string elemento { get; set; }
        public string dependencia { get; set; }


    }

    public enum CatalogoViews
    {
        Carreteras = 1,
        Tramos = 2,
        Dependencias = 3,
        Municipios = 4,
        Entidades = 5,
        OficialesDeTransito = 6,
        MarcasDeVehiculos = 7,
        SubmarcasDeVehiculos = 8,
        TipoDeVehiculos = 9,
        UMAS = 10,
        Colores = 11,
        MotivosDeInfraccion = 12,
        DiasInhabiles = 13,
        AgenciasDelMinisterioPublico = 14,
        AutoridadesDisposicion = 15,
        AutoridadesEntrega = 16,
        InstitucionesTraslado = 17,
        ClasificacionDeAccidentes = 18,
        CausasDeAccidentes = 19,
        DelegacionesOficinasTransporte = 20,
        Hospitales = 21,
        FactoresDeAccidentes = 22,
        FactoresOpciones = 23,
        OficinasRenta = 24,
        TiposCarga = 25,
        TipoServicio = 26,
        SubtipoServicio = 27,
        Turnos = 28,
        Aseguradoras= 29


    }

}
