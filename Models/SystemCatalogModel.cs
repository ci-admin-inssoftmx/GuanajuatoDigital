using System.Collections.Generic;

namespace GuanajuatoAdminUsuarios.Models
{
    public class SystemCatalogModel
    {
        public SystemCatalogModel()
        {
            CatalogList = new List<SystemCatalogListModel>();
        }

        public string CatalogName { get; set; }
        public List<SystemCatalogListModel> CatalogList { get; set; }

    }

    public class SystemCatalogListModel
    {
        public string GuidId { get; set; }
        public int Id { get; set; }
        public string Text { get; set; }
        public bool? Transito { get; set; }
        public int Corp { get; set; }
    }
}
