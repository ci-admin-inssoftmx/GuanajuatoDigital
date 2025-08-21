

using iTextSharp.text;
using GuanajuatoAdminUsuarios.Models;
using System.Collections.Generic;
using System;

namespace GuanajuatoAdminUsuarios.Interfaces
{
    public interface IBitacoraService
    {

        void insertBitacora(decimal id, string ip, string textoCamb, string operacion, string consulta, decimal operador);
        void insertBitacora(decimal id, string ip, string textoCamb, string operacion, string consulta, decimal operador, Object objecto);


        List<BitacoraInfraccionesModel> getBitacoraData(string id,string nombre);


        void BitacoraGeneral(string tabla, string consulta, string Operacion, string usuario = "", string ip = "");
       void BitacoraWS(string WS, string Accion, Object Objeto);
         void BitacoraWS(string WS, string Accion, Object Objeto, string usuario, string ip, string IdDElegacion);


        void BitacoraAccidentes(int IdAccidente, string Accion, Object Objeto);
        

        void BitacoraGenerales(string Accion, Object Objeto);
        
        void BitacoraDepositos(int idDeposito, string Modulo, string Accion, Object Objeto);

        void BitacoraGenerales(string Accion, Object Objeto, string usuario, string ip, string IdDElegacion);



    }
}
