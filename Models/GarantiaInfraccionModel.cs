namespace GuanajuatoAdminUsuarios.Models
{
    public class GarantiaInfraccionModel : EntityModel
    {
        public int? idInfraccion { get; set; }

        public int? idGarantia { get; set; }
        public int? idCatGarantia { get; set; }
        public int? idTipoPlaca { get; set; }
        public int? idTipoLicencia { get; set; }
        public string numPlaca { get; set; }
        public string numLicencia { get; set; }
        public string vehiculoDocumento { get; set; }
        public string garantia { get; set; }
        public string tipoPlaca { get; set; }
        public string tipoLicencia { get; set; }

    }
}
