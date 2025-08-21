using Microsoft.AspNetCore.Mvc.Rendering;

namespace GuanajuatoAdminUsuarios.Models
{
    public class GenericDropDownModel
    {
        public int Id { get; set; }
        public SelectList ModelList { get; set; }
    }
}
