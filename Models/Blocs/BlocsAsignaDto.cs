namespace GuanajuatoAdminUsuarios.Models.Blocs
{
    public class BlocsAsignaDto
    {
        public long registraId { get; set; }
        public string txSerie { get; set; }
        public string txNBoletas { get; set; }
        public string txDeleg { get; set; }
        public string txTipoBl { get; set; }
        public string txAsigna { get; set; }
        public int FolioInicial { get; set; }
        public int FolioFinal { get; set; }
        public string ImgBloc { get; set; }
        public string Estado { get; set; }        
    }
}
