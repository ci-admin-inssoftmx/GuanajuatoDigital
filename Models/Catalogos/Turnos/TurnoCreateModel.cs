using GuanajuatoAdminUsuarios.Entity;

namespace GuanajuatoAdminUsuarios.Models.Catalogos.Turnos
{
    public class TurnoCreateModel : TurnoBaseModel
    {
        public override CatTurno ToEntity()
        {
            return new CatTurno
            {
                IdTurno = default,
                NombreTurno = Nombre,
                InicioTurno = HoraInicio.GetValueOrDefault(),
                FinTurno = HoraFin.GetValueOrDefault(),
                IdDelegacion = IdDelegacion.GetValueOrDefault(),
            };
        }
    }
}
