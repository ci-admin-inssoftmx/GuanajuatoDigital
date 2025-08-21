using GuanajuatoAdminUsuarios.Models;
using System.Collections.Generic;

namespace GuanajuatoAdminUsuarios.Interfaces
{
    public interface IDiasInhabiles
    {
        List<DiasInhabilesModel> GetDiasInhabiles(int corp);

        DiasInhabilesModel GetDiasById(int IdDia);

        int CrearDiaInhabil(DiasInhabilesModel Dia,int corp);

        int EditDia(DiasInhabilesModel Dia);

        //int DeleteOficial(int oficial);
    }
}
