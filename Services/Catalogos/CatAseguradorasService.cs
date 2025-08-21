using DocumentFormat.OpenXml.Spreadsheet;
using GuanajuatoAdminUsuarios.Entity;
using GuanajuatoAdminUsuarios.Interfaces;
using GuanajuatoAdminUsuarios.Models.Catalogos.Aseguradoras;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GuanajuatoAdminUsuarios.Services.Catalogos
{
    public class CatAseguradorasService : ICatAseguradorasService
    {
        private readonly DBContextInssoft _context;

        public CatAseguradorasService()
        {
            _context = new DBContextInssoft();
        }

        public async Task<List<CatAseguradoras>> GetAllAsync()
        {
            return await _context.CatAseguradoras
                .Select(a => new CatAseguradoras
                {
                    IdAseguradora = a.IdAseguradora,
                    NombreAseguradora = a.NombreAseguradora,
                    Estatus = a.Estatus
                })
                .ToListAsync();
        }

        public async Task<AseguradoraModel> GetByIdAsync(int id)
        {
            var aseguradora = await _context.CatAseguradoras.FirstOrDefaultAsync(x => x.IdAseguradora == id);
            if (aseguradora == null) return null;

            return new AseguradoraModel
            {
                IdAseguradora = aseguradora.IdAseguradora,
                NombreAseguradora = aseguradora.NombreAseguradora,
                Estatus = aseguradora.Estatus
            };
        }

        public async Task AddAsync(AseguradoraModel aseguradoraDto)
        {
            var aseguradora = new CatAseguradoras
            {
                NombreAseguradora = aseguradoraDto.NombreAseguradora,
                ActualizadoPor = 0,
                FechaActualizacion = DateTime.Now,
                Estatus = 1
            };

            _context.CatAseguradoras.Add(aseguradora);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(CatAseguradoras CatAseguradora, int actualizadoPor)
        {
            var aseguradora = await _context.CatAseguradoras.FindAsync(CatAseguradora.IdAseguradora);
            if (aseguradora != null)
            {
                aseguradora.NombreAseguradora = CatAseguradora.NombreAseguradora;
                aseguradora.FechaActualizacion = DateTime.Now;
                aseguradora.ActualizadoPor = actualizadoPor;
                aseguradora.Estatus = CatAseguradora.Estatus;
                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteAsync(int id)
        {
            var aseguradora = await _context.CatAseguradoras.FindAsync(id);
            if (aseguradora != null)
            {
                _context.CatAseguradoras.Remove(aseguradora);
                await _context.SaveChangesAsync();
            }
        }

        async Task<List<CatAseguradoras>> ICatAseguradorasService.GetAllAsync()
        {
            var query = GetAllListAsync();

            var result = await query.ToListAsync();
            return result;
        }

        public async Task<List<CatAseguradoras>> SearchAsync(AseguradoraFilterModel filter)
        {
            var query = _context.CatAseguradoras
                .AsNoTracking(); // Si no necesitas seguimiento de cambios

            if (!string.IsNullOrEmpty(filter.NombreAseguradora))
            {
                query = query.Where(t => t.NombreAseguradora.Contains(filter.NombreAseguradora));
            }
            if (filter.Estatus.HasValue && filter.Estatus.Value != -1)
            {
                query = query.Where(t => t.Estatus == filter.Estatus.Value);
            }

            return await query.ToListAsync();
        }


        public IQueryable<CatAseguradoras> GetAllListAsync()
        {
            return _context.CatAseguradoras
                .Where(x => x.Estatus == 1)
                .Select(a => new CatAseguradoras
                {
                    IdAseguradora = a.IdAseguradora,
                    NombreAseguradora = a.NombreAseguradora,
                    Estatus = a.Estatus,
                }).AsQueryable();
        }
    }

}
