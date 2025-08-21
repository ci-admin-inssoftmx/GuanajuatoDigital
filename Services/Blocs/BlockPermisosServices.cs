using GuanajuatoAdminUsuarios.Interfaces;
using GuanajuatoAdminUsuarios.Models.Generales;

namespace GuanajuatoAdminUsuarios.Services.Blocs
{
    public class BlockPermisoInfracciones:IBlockPermisoInfraccion
    {
        IAdminBlocksService _adminBlocksService;
        public BlockPermisoInfracciones(IAdminBlocksService adminBlocksService)
        {
            _adminBlocksService = adminBlocksService;
        }

       public (bool can, string pref) getdate() => _adminBlocksService.GetPermisos(BlocksOperacion.INFRACCIONES);

    }
    public interface IBlockPermisoInfraccion
    {
       public (bool can, string pref) getdate();
    }


    public class BlockPermisoAccidentes : IBlockPermisoAccidentes
    {
        IAdminBlocksService _adminBlocksService;
        public BlockPermisoAccidentes(IAdminBlocksService adminBlocksService)
        {
            _adminBlocksService = adminBlocksService;          
        }

        public bool  getdate() => _adminBlocksService.GetPermisos(BlocksOperacion.ACCIDENTES).can;
    }
    public interface IBlockPermisoAccidentes
    {
        public bool  getdate();
    }


    public class BlockPermisoDepositos : IBlockPermisoDepositos
    {
        IAdminBlocksService _adminBlocksService;
        public BlockPermisoDepositos(IAdminBlocksService adminBlocksService)
        {
            _adminBlocksService = adminBlocksService;
        }

        public bool getdate() => _adminBlocksService.GetPermisos(BlocksOperacion.DEPOSITOS).can;
    }
    public interface IBlockPermisoDepositos
    {
        public bool getdate();
    }
}
