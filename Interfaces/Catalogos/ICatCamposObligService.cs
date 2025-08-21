using GuanajuatoAdminUsuarios.Models.CatCampos;

namespace GuanajuatoAdminUsuarios.Interfaces.Catalogos
{
    public interface ICatCamposObligService
    {
        CamposModPermitidoDto CamposPermitido(int IdDegeg, int IdMpio, int? IdCampo, string? NombreCampo = null);
    }
}
