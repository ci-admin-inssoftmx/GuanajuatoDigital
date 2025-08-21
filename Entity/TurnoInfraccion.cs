namespace GuanajuatoAdminUsuarios.Entity
{
	public class TurnoInfraccion
	{
		public long IdTurnoInfraccion { get; set; }
		public long IdTurno { get; set; }
		public int IdInfraccion { get; set; }

		// no declaramos navigations properties
		// hasta que sean necesarias
	}
}
