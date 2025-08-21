namespace GuanajuatoAdminUsuarios.Models
{
    public class Pagination
    {
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public string? Filter { get; set; }
        public string? Sort { get; set; }
        public string? SortCamp { get; set; }   
    }
}
