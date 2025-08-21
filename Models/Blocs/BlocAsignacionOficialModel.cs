using System;

namespace GuanajuatoAdminUsuarios.Models.Blocs
{
    public class BlocAsignacionOficialModel : BlocModel
    {
        internal static BlocAsignacionOficialModel CreateFrom(BlocModel blockModel)
        {
            return new BlocAsignacionOficialModel
            {
                RegistraId = blockModel.RegistraId,
                Serie = blockModel.Serie,
                TotalBoletas = blockModel.TotalBoletas,
                Delegacion = blockModel.Delegacion,
                TipoBloc = blockModel.TipoBloc,
                Asigna = blockModel.Asigna,
                FolioInicial = blockModel.FolioInicial,
                FolioFinal = blockModel.FolioFinal,
                ImgaenBloc = blockModel.ImgaenBloc,
                Estado = blockModel.Estado
            };
        }
    }
}
