using GuanajuatoAdminUsuarios.Entity;
using GuanajuatoAdminUsuarios.Models.Catalogos.Puestos;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GuanajuatoAdminUsuarios.Interfaces.Catalogos
{
    public interface ICatPuestosService
    {
        IEnumerable<CatPuesto> GetAllPuestos();
        Task<PuestoModel> CreateAsync(PuestoModel puesto);
        Task<PuestoModel> UpdateAsync(PuestoModel puesto);
        Task<bool> DeleteAsync(int id);
        Task<PuestoModel> GetByIdAsync(int id);
        Task<List<PuestoModel>> GetAllAsync();        
        Task<List<PuestoModel>> SearchAsync(PuestoFilterModel filter);                
    }
}