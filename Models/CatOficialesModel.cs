using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Drawing;
using System.IO;

namespace GuanajuatoAdminUsuarios.Models
{
	public class CatOficialesModel
    {

        public int transito { get; set; }
        public int IdTurno { get; set; }
        public int IdOficial { get; set; }
        public int? IdOficina { get; set; }
        public string nombreOficina { get; set; }       
        public string Rango { get; set; }
      
        public string Nombre { get; set; }

        public string? ApellidoPaterno { get; set; }

        public string? ApellidoMaterno { get; set; }

        public DateTime? FechaActualizacion { get; set; }

        public int? ActualizadoPor { get; set; }

        public int? Estatus { get; set; }

        public string estatusDesc { get; set; }

        public bool ValorEstatusOficiales { get; set; }
        public int? Corp { get; set; }

        public int? IdPuesto { get; set; }
        public string UrlFirma { get; set; }

		public string Puesto { get; set; }
		
        public DateTime? FechaInicio { get; set; }
		
        public DateTime? FechaFin { get; set; }
        [NotMapped]
        public string Base64FirmaImage
        {
            get { return ConvertImageToBase64(UrlFirma); }
        }

        [NotMapped]
        public string UrlImageName
        {
            get { return ExtractImageName(UrlFirma); }
        }
        private string ExtractImageName(string path)
        {
            if (String.IsNullOrEmpty(path))
                return null;

            try
            {
                return Path.GetFileName(path);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al extraer el nombre de la imagen: " + ex.Message);
                return null;
            }
        }

        private string ConvertImageToBase64(string imagePath)
        {
            if (File.Exists(imagePath))
            {
                using (Image image = Image.FromFile(imagePath))
                {
                    using (MemoryStream m = new MemoryStream())
                    {
                        image.Save(m, image.RawFormat);
                        byte[] imageBytes = m.ToArray();
                        return Convert.ToBase64String(imageBytes);
                    }
                }
            }
            return null;
        }
    }
}
