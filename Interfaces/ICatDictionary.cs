using GuanajuatoAdminUsuarios.Models;
using System.Collections.Generic;

namespace GuanajuatoAdminUsuarios.Interfaces
{
    public interface ICatDictionary
    {
        public string GetCatalogSystem<TEnum>(int id);
        public Dictionary<int, string> GetCatalogSystem<TEnum>();
        public SystemCatalogModel GetCatalogSystem(string name);
        public SystemCatalogModel GetCatalog(string catalog, string parameter);
    }
}
