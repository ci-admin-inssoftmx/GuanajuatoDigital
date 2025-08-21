namespace GuanajuatoAdminUsuarios.Utils
{
    public static class CatalogosEnums
    {
        public enum EstatusTransitoTransporte
        {
            Captura_de_solicitud = 1,
            Captura_solicitud_sin_grúa = 2,
            captura_grúas = 3,
            Ingreso_a_depósitos = 4,
            Liberación_autorizada = 5,
            Salida_registrada = 6

        }

        public enum EstatusBusquedaVehiculo
        {
            RegistroEstatal = 1,
            Sitteg = 2,
            NoEncontrado = 3
        }

        public enum TipoPersona
        {
            Fisica = 1,
            Moral = 2
        }
    }
}
