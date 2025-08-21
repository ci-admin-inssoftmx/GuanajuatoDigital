using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;

namespace GuanajuatoAdminUsuarios.Models
{
    public class PDFLiberacionVehiculoModel
    {
        public string Propietario { get; set; }
        public string Autoriza { get; set; }
        public string Observaciones { get; set; }
        public string Placas { get; set; }
        public string Marca { get; set; }
        public string SubMarca { get; set; }
        public string Serie { get; set; }
        public string Color { get; set; }

        public DateTime FechaIngreso { get; set; }
        public string Pension { get; set; }
        public string Folio { get; set; }
        public int Id { get; set; }
        public string AcreditacionPersonalidadUrl { get; set; }
        public string AcreditacionPropiedadUrl { get; set; }
        public string ReciboInfraccionUrl { get; set; }
        public string LogoUrl { get; set; }
        public string Paginador1Url { get; set; }
        public string Paginador2Url { get; set; }
        public string Paginador3Url { get; set; }

        [NotMapped]
        public string AcreditacionPersonalidadBase64
        {
            get { return ConvertImageToBase64(AcreditacionPersonalidadUrl); }
        }
        [NotMapped]
        public string AcreditacionPropiedadBase64
        {
            get { return ConvertImageToBase64(AcreditacionPropiedadUrl); }
        }
        [NotMapped]
        public string ReciboInfraccionBase64
        {
            get { return ConvertImageToBase64(ReciboInfraccionUrl); }
        }
        [NotMapped]
        public string LogoBase64
        {
            get { return ConvertImageToBase64(LogoUrl); }
        }
        [NotMapped]
        public string Paginador164
        {
            get { return ConvertImageToBase64(Paginador1Url); }
        }
        [NotMapped]
        public string Paginador264
        {
            get { return ConvertImageToBase64(Paginador2Url); }
        }
        [NotMapped]
        public string Paginador364
        {
            get { return ConvertImageToBase64(Paginador3Url); }
        }
        private string ConvertImageToBase64(string imagePath)
        {
            if (File.Exists(imagePath))
            {
                byte[] imageBytes = File.ReadAllBytes(imagePath);
                return Convert.ToBase64String(imageBytes);
            }
            return null;
        }
    }
}
