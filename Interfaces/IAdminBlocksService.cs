using GuanajuatoAdminUsuarios.Models;
using System.Collections.Generic;

namespace GuanajuatoAdminUsuarios.Interfaces
{
    public interface IAdminBlocksService
    {

        public List<AdminBlocksModel> GetdataGrid();
        public AdminBlocksViewModel Getdata(int IdAlertamiento);

        public List<CatalogModel> GetDropDownCorporaciones();
        public void CrearBlockCorporaciones(int Corporaciones, string prefijoInfracciones, bool infracciones, bool accidente, bool Depositos);
        public void EditarBlockCorporaciones(int Id, string prefijoInfracciones, bool infracciones, bool accidente, bool Depositos);

        public (bool can, string pref) GetPermisos(string permiso);
    }
}
