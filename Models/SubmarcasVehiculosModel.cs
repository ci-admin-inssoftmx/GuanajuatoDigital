namespace GuanajuatoAdminUsuarios.Models
{
    public class SubmarcasVehiculosModel
    {
        public int IdSubmarca { get; set; }

        public string NombreSubmarca { get; set; }

        public int? IdMarcaVehiculo { get; set; }

        public int? ActualizadoPor { get; set; }

        public int? Estatus { get; set; }

        public string estatusDesc { get; set; }

        public string MarcaVehiculo { get; set; }

         public bool ValorEstatusSubmarcas { get; set; }

    }
}
