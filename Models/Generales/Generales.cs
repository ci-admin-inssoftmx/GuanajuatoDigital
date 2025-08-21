using System.Collections.Generic;

namespace GuanajuatoAdminUsuarios.Models.Generales
{
    public static class OperacionBitacora
    {
        public const string lOGIN = "log In" ;
            public const string CREACION = "Creacion" ;
            public const string EDICION = "Edicion" ;
            public const string BORRADOL = "Borrado logico" ;
            public const string BORRADO = "Borrado" ;
            public const string PETICIONL = "Peticion a licencias" ;
            public const string PETICIONP = "Peticion a Padron Estatal" ;
            public const string PETICIONR = "Peticion a Repuve" ;
        public const string PETICIONF = "Peticion Finanzas";

    }

    public static class EstatusOperacion
    {
        public const string ACTIVO = "Activo";
        public const string INACTIVO = "Inactivo";

    }

    public static class BlocksOperacion
    {
        public const string ACCIDENTES = "ACCIDENTES";
        public const string INFRACCIONES = "INFRACCIONES";
        public const string DEPOSITOS = "DEPOSITOS";

    }


}
