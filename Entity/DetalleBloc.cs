using GuanajuatoAdminUsuarios.Entity;

namespace GuanajuatoAdminUsuarios.Data.Entities2;

public partial class DetalleBloc
{
    public const string EstatusCancelado = "Cancelado";
    public const string EstatusAsignado = "Asignado";
    public const string EstatusDisponible = "Disponible";


    public int idEstatusFolio { get;set; }
    public int idUsrUsa { get;set; }
    public int folio { get;set; }
    public int idAccidente { get;set; }
    public int idInfraccion { get;set; }
    public int idDeposito { get;set; }

    public int DetalleId { get; set; }

    public long RegistraId { get; set; }

    public string Serie { get; set; } = null!;

    public int FolioInicial { get; set; }

    public int FolioFinal { get; set; }

    public string Estatus { get; set; } = null!;

    public string Comentarios { get; set; }

    public virtual RegistraBloc Registra { get; set; } = null!;
}
