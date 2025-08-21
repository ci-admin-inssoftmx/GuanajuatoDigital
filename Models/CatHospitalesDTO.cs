using GuanajuatoAdminUsuarios.Entity;
using System;
using System.Collections.Generic;

namespace GuanajuatoAdminUsuarios.Models
{
    public class CatHospitalesDTO
    {
        public int idMunicipio2 { get; set; }
        public int idEntidad2 { get; set; }
        public List<CatHospitalesModel> HospitalesModel { get; set; }

    }
}
