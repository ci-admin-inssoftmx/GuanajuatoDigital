using GuanajuatoAdminUsuarios.Data.Entities2;

namespace GuanajuatoAdminUsuarios.Models.Blocs
{
    public class BlocListItemModel
    {
        public long RegistraId { get; set; }
        public string img { get; set; }
        public string Rango { get; set; }
        public string Disponibles { get; set; }
        public string Serie { get; set; }
        public int TotalBoletas { get; set; }
        public string Oficina { get; set; }
        public string TipoBlock { get; set; }
        public string Asigna { get; set; }
        public string FechAsignacion { get; set; }
        public string OficialAsignado { get; set; }
        public int Estado { get; set; }
        public string Estadodesc { get; set; }
        public bool IsCancelado => Estadodesc == DetalleBloc.EstatusCancelado;
        public bool IsAsignado => !IsCancelado && !string.IsNullOrWhiteSpace(OficialAsignado);

        public string EstadoLabel => IsAsignado ? DetalleBloc.EstatusAsignado : Estadodesc;

    }
}
