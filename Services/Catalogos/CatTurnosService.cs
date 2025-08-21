using GuanajuatoAdminUsuarios.Entity;
using GuanajuatoAdminUsuarios.Exceptions;
using GuanajuatoAdminUsuarios.Helpers;
using GuanajuatoAdminUsuarios.Interfaces.Catalogos;
using GuanajuatoAdminUsuarios.Models.Catalogos.Turnos;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GuanajuatoAdminUsuarios.Services.Catalogos
{
    public class CatTurnosService : ICatTurnosService
    {
        private readonly DBContextInssoft _dbContext;
        private readonly UserSession _userSession;  
        
        public CatTurnosService(UserSession userSession)
        {
            _userSession = userSession ?? throw new ArgumentNullException(nameof(userSession));
            _dbContext = new DBContextInssoft();                 
        }

        public async Task<List<CatTurno>> GetAllByDependenciaAsync(int dependencia)
        {
            var query =  GetAllByDependenciaQueryable(dependencia);
            
            var result = await query.ToListAsync();
            return result;
        }
        async public Task<List<CatTurno>> GetAllByDelegacionAsync(int Oficina)
        {
            var query = GetAllByDelegacionQueryable(Oficina);

            var result = await query.ToListAsync();
            return result;
        }


        public async Task<List<CatTurno>> GetAllByMunicipioAsync(int municipio)
		{
			var result = await _dbContext.CatTurnos
				.Include(t => t.Delegacion)
				.Include(t => t.Municipio)
				.Where(t => t.IdMunicipio == municipio)
                .ToListAsync();

			return result;
		}

		public async Task<List<CatTurno>> SearchAsync(int dependencia, TurnoFilterModel filter)
        {
            if (filter.IsEmpty())            
                return await GetAllByDependenciaAsync(dependencia);


            var query = GetAllByDependenciaQueryable(dependencia);
            
            if (!string.IsNullOrEmpty(filter.Nombre)) query = query.Where(t => t.NombreTurno.Contains(filter.Nombre));
            if (filter.HoraInicio.HasValue) query = query.Where(t => t.InicioTurno >= filter.HoraInicio);
            if (filter.HoraFin.HasValue) query = query.Where(t => t.FinTurno <= filter.HoraFin);
            if (filter.IdDelegacion.HasValue) query = query.Where(t => t.IdDelegacion == filter.IdDelegacion);
            
            var result = await query.ToListAsync();
            return result;
        }

        public async Task<CatTurno> GetByIdAsync(long id)
        {
            return await _dbContext.CatTurnos
                .Include(t => t.Delegacion)
                .Include(t => t.Municipio)
                .FirstOrDefaultAsync(t => t.IdTurno == id);
        }

        public async Task<CatTurno> CreateAsync(CatTurno turno)
        {
            ValidateTurno(turno);

            var delegacion = await _dbContext.CatDelegacionesOficinasTransporte.FindAsync(turno.IdDelegacion)
                    ?? throw new Exception($"{nameof(CatDelegacionesOficinasTransporte)} '{turno.IdDelegacion}' not found");
                        
            turno.IdMunicipio = delegacion.IdMunicipio.GetValueOrDefault();
            turno.FechaActualizacion = DateTime.Now;
            turno.ActualizadoPor = _userSession.GetUsuarioId();

            _dbContext.CatTurnos.Add(turno);
            await _dbContext.SaveChangesAsync();
            return turno;
        }

        public async Task<CatTurno> UpdateAsync(CatTurno turno)
        {
            ValidateTurno(turno);

            var existingTurno = await _dbContext.CatTurnos.FindAsync(turno.IdTurno)
                ?? throw new Exception($"{nameof(CatTurno)} '{turno.IdTurno}' not found");

            var delegacion = _dbContext.CatDelegacionesOficinasTransporte.FirstOrDefault(x => x.IdOficinaTransporte == turno.IdDelegacion)
                    ?? throw new Exception($"{nameof(CatDelegacionesOficinasTransporte)} '{turno.IdDelegacion}' not found");

            existingTurno.IdDelegacion = turno.IdDelegacion;
            existingTurno.IdMunicipio = delegacion.IdMunicipio.GetValueOrDefault();
            existingTurno.NombreTurno = turno.NombreTurno;
            existingTurno.InicioTurno = turno.InicioTurno;
            existingTurno.FinTurno = turno.FinTurno;
            existingTurno.FechaActualizacion = DateTime.Now;
            existingTurno.ActualizadoPor = turno.ActualizadoPor;

            _dbContext.CatTurnos.Update(existingTurno);
            await _dbContext.SaveChangesAsync();
            return existingTurno;
        }

        public async Task<bool> DeleteAsync(long id)
        {
            var turno = await _dbContext.CatTurnos.FindAsync(id);
            if (turno == null)
            {
                return false;
            }

            _dbContext.CatTurnos.Remove(turno);
            await _dbContext.SaveChangesAsync();
            return true;
        }

        public IQueryable<CatTurno> GetAllByDependenciaQueryable(int dependencia)
        {
            var query = _dbContext.CatTurnos
                .Include(t => t.Delegacion)
                .Include(t => t.Municipio)
                .Where(t => t.Delegacion.Transito == dependencia)
                .AsQueryable();

            return query;
        }

        public IQueryable<CatTurno> GetAllByDelegacionQueryable(int dependencia)
        {
            var query = _dbContext.CatTurnos
                .Include(t => t.Delegacion)
                .Include(t => t.Municipio)
                .Where(t => t.Delegacion.IdDelegacion == dependencia)
                .AsQueryable();

            return query;
        }

        private void ValidateTurno(CatTurno turno)
        {
            /*var existingTurnos = _dbContext.CatTurnos
                .Where(t => t.IdDelegacion == turno.IdDelegacion)
                .Where(t => t.IdTurno != turno.IdTurno)
                .ToList();

            foreach (var existingTurno in existingTurnos)
            {
                if (TurnoIsTimeConfict(existingTurno, turno))
                {
                    throw new TurnoException("El turno se solapa en horario con un turno existente.");
                }
            }*/
        }

        private static bool TurnoIsTimeConfict(CatTurno existingTurno, CatTurno updatedTurno) 
            => updatedTurno.InicioTurno < existingTurno.FinTurno && updatedTurno.FinTurno > existingTurno.InicioTurno;
    }
}
