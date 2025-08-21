namespace GuanajuatoAdminUsuarios.Models
{
    public class AdminBlocksModel
    {
        public int Id { get; set; }
        public string Corporacion { get; set; }
        public string prefijoInfracciones { get; set; }
        public string infraccion { get; set; }
        public string accidentes { get; set; }
        public string depositos { get; set; }
    }


    public class AdminBlocksViewModel
    {
        public int Id { get; set; }
        public string Corporacion { get; set; }
        public string prefijoInfracciones { get; set; }
        public bool infraccion { get; set; }
        public bool accidentes { get; set; }
        public bool depositos { get; set; }
    }

}
