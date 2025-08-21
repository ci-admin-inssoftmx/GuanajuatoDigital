using GuanajuatoAdminUsuarios.Entity;
using GuanajuatoAdminUsuarios.Interfaces.Catalogos;
using GuanajuatoAdminUsuarios.Models.CatCampos;
using System;
using System.Linq;

namespace GuanajuatoAdminUsuarios.Services.Catalogos
{
    public class CatCamposObligService : ICatCamposObligService
    {
        private readonly DBContextInssoft dbContext;
        public CatCamposObligService()
        {
            dbContext = new DBContextInssoft();
        }

        public CamposModPermitidoDto CamposPermitido(int IdDegeg, int IdMpio, int? IdCampo, string? NombreCampo)
        {
            try
            {
                IdCampo ??= dbContext.CatCampos.FirstOrDefault(x => x.NombreCampo.ToLower() == NombreCampo.ToLower())?.IdCampo;
                var result = (from caob in dbContext.CatCamposObligatorios
                              where caob.IdDelegacion == IdDegeg
                                  && caob.IdMunicipio == IdMpio
                                  && caob.IdCampo == IdCampo
                              select new CamposModPermitidoDto()
                              {
                                  Accidentes = caob.Accidentes,
                                  Infracciones = caob.Infracciones,
                                  Depositos = caob.Depositos
                              }
                     ).FirstOrDefault();

                return result;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

    }

}
