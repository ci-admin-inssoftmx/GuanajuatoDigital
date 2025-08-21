using Microsoft.AspNetCore.Http;

namespace GuanajuatoAdminUsuarios.Models
{
    public class DatosAccidenteModel
    { 
        public string montoCamino { get; set; }
        public string montoCarga { get; set; }
        public string montoPropietarios { get; set; }
        public string montoOtros { get; set; }
        public string Latitud { get; set; }
        public string Longitud { get; set; }
        public int IdCiudad { get; set; }
        public int IdEntidadCompetencia { get; set; }

        
        public int IdCertificado { get; set; }
        public string entregaObjetos { get; set; }
        public string entregaOtros { get; set; }
        public string consignacionHechos { get; set; }
        public string numeroOficio { get; set; }
        public int IdAgenciaMinisterio { get; set; }
        public string RecibeMinisterio { get; set; }
        public int IdElabora { get; set; }
        public int IdSupervisa { get; set; }
        public int IdAutoriza { get; set; }
        public string trayectoria { get; set; }        
        public int IdElaboraConsignacion { get; set; }
        public int IdAutoridadEntrega { get; set; }
        public int IdAutoridadDisposicion { get; set; }
        public int EstadoArmas { get; set; }
        public int EstadoDrogas { get; set; }
        public int EstadoValores { get; set; }
        public int EstadoPrendas { get; set; }
        public int EstadoOtros { get; set; }
        public int EstadoConvenio { get; set; }

        public bool ArmasBool =>EstadoArmas == 1;
        public bool DrogasBool => EstadoDrogas == 1;
        public bool ValoresBool => EstadoValores == 1;
        public bool PrendasBool => EstadoPrendas == 1;
        public bool OtrosBool => EstadoOtros == 1;
        public bool convenioBool => EstadoConvenio == 1;

        
        public string ArmasTexto { get; set; }
        public string DrogasTexto { get; set; }
        public string ValoresTexto { get; set; }
        public string PrendasTexto { get; set; }
        public string OtrosTexto { get; set; }
        public string observacionesConvenio { get; set; }

        public int IdEstatusReporte { get; set; }

        // Archivo boleta del accidente

        public IFormFile boleta { get; set; }
        public string boletaPath { get; set; }
        public string boletaStr { get; set; }
        // Archivo parte del accidente

        public IFormFile parte { get; set; }
        public string archivoPartePath { get; set; }
        public string archivoParteStr { get; set; }

        // Archivo involucrados

        public IFormFile archivoInvolucrado { get; set; }
        public string archivoInvolucradoPath { get; set; }
        public string archivoInvolucradoStr { get; set; }
        public int idInvolucrado { get; set; }

        
    }
}
