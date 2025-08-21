namespace GuanajuatoAdminUsuarios.Models.Blocs
{
    public class BlocModel
    {
        public long RegistraId { get; set; }
        public string Serie { get; set; }
        public int TotalBoletas { get; set; }
        public string Delegacion { get; set; }
        public string TipoBloc { get; set; }
        public string Asigna { get; set; }
        public int FolioInicial { get; set; }
        public int FolioFinal { get; set; }
        public string ImgaenBloc { get; set; }
        public string Estado { get; set; }
    }
}
