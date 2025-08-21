using System.Text.Json;

namespace GuanajuatoAdminUsuarios.Models.Commons
{
    public class ResponseError
    {
        public int StatusCode { get; set; }
        public string Message { get; set; }
        public override string ToString()
        {
            return JsonSerializer.Serialize(this);
        }
    }

    public class ResponseErrorTechnical
    {
        public int StatusCode { get; set; }
        public string Message { get; set; }
        public string Trace { get; set; }
        public override string ToString()
        {
            return JsonSerializer.Serialize(this);
        }
    }
}
