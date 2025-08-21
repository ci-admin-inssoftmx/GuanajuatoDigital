using GuanajuatoAdminUsuarios.Entity;
using GuanajuatoAdminUsuarios.Exceptions;
using GuanajuatoAdminUsuarios.Helpers;
using GuanajuatoAdminUsuarios.Interfaces.Catalogos;
using GuanajuatoAdminUsuarios.Models.Catalogos.Puestos;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GuanajuatoAdminUsuarios.Services.Catalogos
{
    public class CatPuestosService : ICatPuestosService
    {
        private readonly DBContextInssoft _dbContext;
        private readonly UserSession _userSession;

        public CatPuestosService(UserSession userSession)
        {
            _userSession = userSession ?? throw new ArgumentNullException(nameof(userSession));
            _dbContext = new DBContextInssoft();
        }

        public async Task<PuestoModel> GetByIdAsync(int id)
        {
            CatPuesto puestoEntity = await GetEntityByIdAsync(id);
            if (puestoEntity == null)
                return null;

            return PuestoModel.FromEntity(puestoEntity);
        }
        
        public async Task<List<PuestoModel>> GetAllAsync()
        {
            IQueryable<CatPuesto> puestosEntitiesQuery = GetAllByUserPermission();
            
            var puestosEntities = await puestosEntitiesQuery
                .Include(x => x.Delegacion)
                .ToListAsync();

            var puestosModels = puestosEntities
                .Select(PuestoModel.FromEntity)
                .ToList();

            return puestosModels;
        }

        public async Task<List<PuestoModel>> SearchAsync(PuestoFilterModel filter)
        {
            if (filter.IsEmpty())
                return await GetAllAsync();

            var puestosEntitiesQuery = GetAllByUserPermission();

            if (!string.IsNullOrEmpty(filter.Nombre)) puestosEntitiesQuery = puestosEntitiesQuery.Where(x => x.NombrePuesto.Contains(filter.Nombre));
            if (filter.IdDelegacion.HasValue) puestosEntitiesQuery = puestosEntitiesQuery.Where(t => t.IdDelegacion == filter.IdDelegacion);

            var puestosEntities = await puestosEntitiesQuery
                .Include(x => x.Delegacion)
                .ToListAsync();
            var puestosModel = puestosEntities
                .Select(PuestoModel.FromEntity)
                .ToList();
            
            return puestosModel;
        }

        public async Task<PuestoModel> CreateAsync(PuestoModel puestoModel)
        {
            int coporacion = _userSession.GetCorporacionId();
            CatPuesto puestoEntity = puestoModel.ToEntity(coporacion);

            var delegacion = await _dbContext.CatDelegacionesOficinasTransporte.FindAsync(puestoEntity.IdDelegacion)
                    ?? throw new PuestoException($"{nameof(CatDelegacionesOficinasTransporte)} '{puestoEntity.IdDelegacion}' not found");

            ValidatePuestoForDelegacion(puestoEntity, delegacion.NombreOficina);
            
            puestoEntity.Estatus = 1;
            puestoEntity.FechaCreacion = DateTime.Now;
            puestoEntity.ActualizadoPor = _userSession.GetUsuarioId();

            _dbContext.CatPuestos.Add(puestoEntity);
            await _dbContext.SaveChangesAsync();
            
            return await GetByIdAsync(puestoEntity.IdPuesto);
        }

        public async Task<PuestoModel> UpdateAsync(PuestoModel puestoModel)
        {
            var delegacion = await _dbContext.CatDelegacionesOficinasTransporte.FindAsync(puestoModel.IdDelegacion)
                    ?? throw new PuestoException($"{nameof(CatDelegacionesOficinasTransporte)} '{puestoModel.IdDelegacion}' not found");

            CatPuesto puestoEntity = await GetEntityByIdAsync(puestoModel.Id) 
                ?? throw new PuestoException($"{nameof(CatPuesto)} '{puestoModel.Id}' not found");
            
            puestoEntity.NombrePuesto = puestoModel.Puesto;
            puestoEntity.IdDelegacion = puestoModel.IdDelegacion;
            puestoEntity.Descripcion = puestoModel.Descripcion;
            puestoEntity.ActualizadoPor = _userSession.GetUsuarioId();
            
            ValidatePuestoForDelegacion(puestoEntity, delegacion.NombreOficina);

            _dbContext.CatPuestos.Update(puestoEntity);
            await _dbContext.SaveChangesAsync();
            
            return await GetByIdAsync(puestoEntity.IdPuesto);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var puesto = await _dbContext.CatPuestos.FindAsync(id);
            if (puesto == null)
            {
                return false;
            }

            var existsOficialWithPuesto = await _dbContext.Oficiales.AnyAsync(x => x.IdPuesto == id);
            if (existsOficialWithPuesto)
            {
                throw new PuestoException($"No se puede eliminar el puesto '{puesto.NombrePuesto}' porque tiene oficiales asignados.");
            }   

            _dbContext.CatPuestos.Remove(puesto);
            await _dbContext.SaveChangesAsync();
            return true;
        }

        private async Task<CatPuesto> GetEntityByIdAsync(int id)
        {
            return await _dbContext.CatPuestos
                .Include(t => t.Delegacion)
                .FirstOrDefaultAsync(t => t.IdPuesto == id);
        }

        private IQueryable<CatPuesto> GetAllByUserPermission()
        {
            int corporacion = _userSession.GetCorporacionId();
            bool isAdmin = _userSession.IsAdmin();
            int delegacion = _userSession.GetOficinaDelegacionId();

            IQueryable<CatPuesto> puestosEntitiesQuery = _dbContext.CatPuestos.
                Where(x => x.Transito == corporacion);

            if (!isAdmin) puestosEntitiesQuery = puestosEntitiesQuery.Where(x => x.IdDelegacion == delegacion);

            return puestosEntitiesQuery;
        }

        public IEnumerable<CatPuesto> GetAllPuestos()
        {
            return _dbContext.CatPuestos.Where(p => p.Estatus == 1).ToList();
        }

        private void ValidatePuestoForDelegacion(CatPuesto puesto, string nombreDelegacion)        
        {
            var existingPuesto = _dbContext.CatPuestos
                .Where(x => x.IdPuesto != puesto.IdPuesto) // new puesto is always 0
                .Where(x => x.IdDelegacion == puesto.IdDelegacion)
                .Where(x => x.NombrePuesto == puesto.NombrePuesto)
                .FirstOrDefault();

            if (existingPuesto == null)
                return;

            throw new PuestoException($"Ya existe un puesto con el nombre '{puesto.NombrePuesto}' en la delegación {nombreDelegacion}.");            
        }
    }
}