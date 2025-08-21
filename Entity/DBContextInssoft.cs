using Microsoft.EntityFrameworkCore;
using GuanajuatoAdminUsuarios.DBConect;
using GuanajuatoAdminUsuarios.Data.Entities2;

namespace GuanajuatoAdminUsuarios.Entity;

public partial class DBContextInssoft : DbContext
{
    private readonly DbConect dbconect;
    public DBContextInssoft()
    {

        dbconect = new DbConect();
    }

    public DBContextInssoft(DbContextOptions<DBContextInssoft> options)
        : base(options)
    {
    }

    public virtual DbSet<AccidentesDatosAseguradora> AccidentesDatosAseguradoras { get; set; }

    public virtual DbSet<Dependencias> Dependencias { get; set; }

    public virtual DbSet<MarcasVehiculo> MarcasVehiculos { get; set; }

    public virtual DbSet<Oficiales> Oficiales { get; set; }

    public virtual DbSet<SubmarcasVehiculo> SubmarcasVehiculos { get; set; }

    public virtual DbSet<Estatus> Estatus { get; set; }

    public virtual DbSet<Delegaciones> Delegaciones { get; set; }

    public virtual DbSet<CatColores> Colores { get; set; }

    public virtual DbSet<TipoVehiculos> TipoVehiculos { get; set; }

    public virtual DbSet<CatSalariosMinimos> CatSalariosMinimos { get; set; }

    public virtual DbSet<DiasInhabiles> DiasInhabiles { get; set; }

    public virtual DbSet<CatMunicipios> CatMunicipios { get; set; }

    public virtual DbSet<TiposCarga> TiposCarga { get; set; }

    public virtual DbSet<MotivosInfraccion> MotivosInfraccion { get; set; }
    public virtual DbSet<CatMotivosInfraccion> CatMotivosInfracciones { get; set; }

    public virtual DbSet<CatAutoridadesDisposicion> CatAutoridadesDisposicion { get; set; }

    public virtual DbSet<CatAutoridadesEntrega> CatAutoridadesEntrega { get; set; }

    public virtual DbSet<CatAseguradoras> CatAseguradoras { get; set; }

    public virtual DbSet<CatInstitucionesTraslado> CatInstitucionesTraslado { get; set; }

    public virtual DbSet<CatOficinasRenta> CatOficinasRenta { get; set; }

    public virtual DbSet<CatAgenciasMinisterio> CatAgenciasMinisterio { get; set; }

    public virtual DbSet<CatClasificacionAccidentes> CatClasificacionAccidentes { get; set; }

    public virtual DbSet<CatFactoresAccidentes> CatFactoresAccidentes { get; set; }

    public virtual DbSet<CatCausasAccidentes> CatCausasAccidentes { get; set; }

    public virtual DbSet<CatFactoresOpcionesAccidentes> CatFactoresOpcionesAccidentes { get; set; }

    public virtual DbSet<CatHospitales> CatHospitales { get; set; }

    public virtual DbSet<CatDelegacionesOficinasTransporte> CatDelegacionesOficinasTransporte { get; set; }

    
    public virtual DbSet<CatConceptoInfraccion> CatConceptosInfraccion { get; set; }
    public virtual DbSet<CatSubConceptoInfraccion> CatSubConceptosInfraccion { get; set; }

    public virtual DbSet<CatDelegacione> CatDelegaciones { get; set; }
    public virtual DbSet<CatTipoBloc> CatTipoBlocs { get; set; }

    public virtual DbSet<CatCampos> CatCampos { get; set; }

    public virtual DbSet<CatCamposObligatorios> CatCamposObligatorios { get; set; }

    public virtual DbSet<DetalleBloc> DetalleBlocs { get; set; }
    public virtual DbSet<catEstatusBlocks> catEstatusBlocks { get; set; }

    public virtual DbSet<RegistraBloc> RegistraBlocs { get; set; }

    public virtual DbSet<Accidente> Accidentes { get; set; }
    public virtual DbSet<Infracciones> Infracciones{ get; set; }

    public virtual DbSet<AccidentesEmergencia> AccidentesEmergencias { get; set; }
  
    public virtual DbSet<InfraccionesEmergencia> InfraccionesEmergencias { get; set; }

    public virtual DbSet<CatTurno> CatTurnos { get; set; }

    public virtual DbSet<CatPuesto> CatPuestos { get; set; }

    public virtual DbSet<TurnoInfraccion> TurnosInfracciones { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer(dbconect.DbConection);
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {

        modelBuilder.Entity<catEstatusBlocks>(entity =>
        {
            entity.HasKey(e => e.id);
            entity.Property(e => e.descripcion).HasColumnName("descripcion");
        });


        modelBuilder.Entity<Dependencias>(entity =>
        {
            entity.HasKey(e => e.IdDependencia).HasName("PK__dependen__A67AC7BE849A3403");

            entity.ToTable("catDependencias");

            entity.Property(e => e.IdDependencia).HasColumnName("idDependencia");
            entity.Property(e => e.ActualizadoPor).HasColumnName("actualizadoPor");
            entity.Property(e => e.Estatus).HasColumnName("estatus");
            entity.Property(e => e.FechaActualizacion)
                .HasColumnType("datetime")
                .HasColumnName("fechaActualizacion");
            entity.Property(e => e.NombreDependencia)
                .HasMaxLength(150)
                .IsUnicode(false)
                .HasColumnName("nombreDependencia");
        });

        modelBuilder.Entity<MarcasVehiculo>(entity =>
        {
            entity.HasKey(e => e.IdMarcaVehiculo).HasName("PK__marcasVe__33AE0F9E9DCD9C8C");

            entity.ToTable("marcasVehiculos");

            entity.Property(e => e.IdMarcaVehiculo).HasColumnName("IdMarcaVehiculo");
            entity.Property(e => e.Estatus).HasColumnName("estatus");
            entity.Property(e => e.FechaActualizacion)
                .HasColumnType("datetime")
                .HasColumnName("fechaActualizacion");
            entity.Property(e => e.MarcaVehiculo)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("marcaVehiculo");
            entity.Property(e => e.ModificadoPor).HasColumnName("modificadoPor");
        });

        modelBuilder.Entity<Oficiales>(entity =>
        {
            entity.HasKey(e => e.IdOficial).HasName("PK__oficiale__7BFD0DB1E280E5C4");

            entity.ToTable("catOficiales");

            entity.Property(e => e.IdOficial).HasColumnName("idOficial");

            entity.Property(e => e.ActualizadoPor).HasColumnName("actualizadoPor");
            entity.Property(e => e.ApellidoMaterno)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("apellidoMaterno");
            entity.Property(e => e.ApellidoPaterno)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("apellidoPaterno");
            entity.Property(e => e.Estatus).HasColumnName("estatus");
            entity.Property(e => e.FechaActualizacion)
                .HasColumnType("datetime")
                .HasColumnName("fechaActualizacion");
            entity.Property(e => e.Nombre)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("nombre");
            entity.Property(e => e.Rango)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("rango");

        });

        modelBuilder.Entity<SubmarcasVehiculo>(entity =>
        {
            entity.HasKey(e => e.IdSubmarca);

            entity.ToTable("submarcasVehiculos");

            entity.Property(e => e.IdSubmarca).HasColumnName("IdSubmarca");
            entity.Property(e => e.ActualizadoPor).HasColumnName("actualizadoPor");
            entity.Property(e => e.estatus).HasColumnName("estatus");
            entity.Property(e => e.FechaActualizacion)
              .HasColumnType("datetime")
              .HasColumnName("fechaActualizacion");
            entity.Property(e => e.IdMarcaVehiculo).HasColumnName("IdMarcaVehiculo");
            entity.Property(e => e.NombreSubmarca)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("nombreSubmarca");
        });

        modelBuilder.Entity<Estatus>(entity =>
        {
            entity.HasKey(e => e.estatus);

            entity.ToTable("estatus");

            entity.Property(e => e.estatus).HasColumnName("estatus");
            entity.Property(e => e.estatusDesc)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("estatusDesc");
        });

        modelBuilder.Entity<Delegaciones>(entity =>
        {
            entity.HasKey(e => e.IdDelegacion);

            entity.ToTable("delegaciones");
            entity.Property(e => e.IdDelegacion).HasColumnName("idDelegacion");

            entity.Property(e => e.Delegacion).HasColumnName("delegacion")
                .HasMaxLength(100)
                .IsUnicode(false);

            entity.Property(e => e.ActualizadoPor).HasColumnName("actualizadoPor");

            entity.Property(e => e.FechaActualizacion)
                .HasColumnName("fechaActualizacion")
                .HasColumnType("datetime");

            entity.Property(e => e.Estatus).HasColumnName("estatus");

            entity.Property(e => e.IdMunicipio)
               .IsUnicode(false)
               .HasColumnName("idMunicipio");

            entity.Property(e => e.Transito)
               .IsUnicode(false)
               .HasColumnName("Transito");

            entity.HasOne(e => e.Municipio)
                .WithMany()
                .HasForeignKey(e => e.IdMunicipio);
        });

        modelBuilder.Entity<CatColores>(entity =>
        {
            entity.HasKey(e => e.IdColor);

            entity.ToTable("catColores");
            entity.Property(e => e.IdColor).HasColumnName("IdColor");

            entity.Property(e => e.color).HasColumnName("color")
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.ActualizadoPor).HasColumnName("actualizadoPor");
            entity.Property(e => e.FechaActualizacion)
             .HasColumnType("datetime")
             .HasColumnName("fechaActualizacion");
            entity.Property(e => e.Estatus).HasColumnName("Estatus");

        });

        modelBuilder.Entity<CatCampos>(entity =>
        {
            entity.HasKey(e => e.IdCampo);

            entity.ToTable("catCampos");
            entity.Property(e => e.IdCampo).HasColumnName("IdCampo");

            entity.Property(e => e.NombreCampo).HasColumnName("nombreCampo")
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.ActualizadoPor).HasColumnName("actualizadoPor");
            entity.Property(e => e.FechaActualizacion)
             .HasColumnType("datetime")
             .HasColumnName("fechaActualizacion");

        });

        modelBuilder.Entity<CatCamposObligatorios>(entity =>
        {
            entity.HasKey(e => e.IdCampoObligatorio);
            entity.ToTable("catCamposObligatorios");

            entity.Property(e => e.IdCampoObligatorio)
                .HasColumnName("idCampoObligatorio");

            entity.Property(e => e.IdDelegacion)
                .HasColumnName("idDelegacion");

            entity.Property(e => e.IdMunicipio).HasColumnName("idMunicipio");

            entity.Property(e => e.IdCampo).HasColumnName("idCampo");

            entity.Property(e => e.Accidentes).HasColumnName("accidentes");

            entity.Property(e => e.Infracciones).HasColumnName("infracciones");

            entity.Property(e => e.Depositos)
                .HasColumnName("depositos");

            entity.Property(e => e.ActualizadoPor)
                .HasColumnName("actualizadoPor");

            entity.Property(e => e.FechaActualizacion)
             .HasColumnType("datetime")
             .HasColumnName("fechaActualizacion");

        });


        modelBuilder.Entity<TipoVehiculos>(entity =>
        {
            entity.HasKey(e => e.IdTipoVehiculo);

            entity.ToTable("tiposVehiculo");



            entity.Property(e => e.IdTipoVehiculo).HasColumnName("IdTipoVehiculo");

            entity.Property(e => e.TipoVehiculo).HasColumnName("TipoVehiculo")
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.ActualizadoPor).HasColumnName("actualizadoPor");
            entity.Property(e => e.FechaActualizacion)
             .HasColumnType("datetime")
             .HasColumnName("fechaActualizacion");
            entity.Property(e => e.Estatus).HasColumnName("Estatus");

        });

        modelBuilder.Entity<CatSalariosMinimos>(entity =>
        {
            entity.HasKey(e => e.IdSalario);

            entity.ToTable("catSalariosMinimos");



            entity.Property(e => e.IdSalario).HasColumnName("IdSalario");

            entity.Property(e => e.Area).HasColumnName("Area")
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.Salario)
                .HasColumnType("float")
                .HasColumnName("Salario");
            entity.Property(e => e.Fecha)
                .HasColumnType("date")
                .HasColumnName("Fecha");
            entity.Property(e => e.ActualizadoPor).HasColumnName("actualizadoPor");
            entity.Property(e => e.FechaActualizacion)
                .HasColumnType("datetime")
                .HasColumnName("fechaActualizacion");
            entity.Property(e => e.Estatus).HasColumnName("Estatus");
            //   entity.Property(e => e.Anio)
            //    .HasColumnType("int")
            //    .HasColumnName("anio");

        });

        modelBuilder.Entity<DiasInhabiles>(entity =>
        {
            entity.HasKey(e => e.idDiaInhabil);

            entity.ToTable("catDiasInhabiles");

            entity.Property(e => e.idDiaInhabil)
                .HasColumnName("idDiaInhabil");

            entity.Property(e => e.fecha)
                .HasColumnName("fecha");

            entity.Property(e => e.idMunicipio)
                .HasColumnName("idMunicipio");

            entity.Property(e => e.todosMunicipiosDesc)
                .HasColumnName("todosMunicipiosDesc")
                .HasMaxLength(10)
                .IsUnicode(false);

            entity.Property(e => e.ActualizadoPor)
                .HasColumnName("actualizadoPor");

            entity.Property(e => e.FechaActualizacion)
                .HasColumnName("fechaActualizacion");

            entity.Property(e => e.Estatus)
                .HasColumnName("estatus");
        });

        modelBuilder.Entity<CatMunicipios>(entity =>
        {
            entity.HasKey(e => e.IdMunicipio);

            entity.ToTable("catMunicipios");

            entity.Property(e => e.IdMunicipio).HasColumnName("idMunicipio");
            entity.Property(e => e.Municipio)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("municipio");

            entity.Property(e => e.ActualizadoPor).HasColumnName("actualizadoPor");

            entity.Property(e => e.FechaActualizacion)
             .HasColumnType("datetime")
             .HasColumnName("fechaActualizacion");

            entity.Property(e => e.Estatus).HasColumnName("estatus");
        });

        modelBuilder.Entity<TiposCarga>(entity =>
        {
            entity.HasKey(e => e.IdTipoCarga);

            entity.ToTable("catTiposcarga");
            entity.Property(e => e.IdTipoCarga).HasColumnName("IdTipoCarga");

            entity.Property(e => e.TipoCarga).HasColumnName("TipoCarga")
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.ActualizadoPor).HasColumnName("actualizadoPor");
            entity.Property(e => e.FechaActualizacion)
             .HasColumnType("datetime")
             .HasColumnName("fechaActualizacion");
            entity.Property(e => e.Estatus).HasColumnName("Estatus");

        });

        modelBuilder.Entity<MotivosInfraccion>(entity =>
        {
            entity.HasKey(e => e.IdMotivoInfraccion);

            entity.ToTable("motivosInfraccion");
            entity.Property(e => e.IdMotivoInfraccion).HasColumnName("IdMotivoInfraccion");

            entity.Property(e => e.Nombre).HasColumnName("Nombre")
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.CalificacionMinima).HasColumnName("CalificacionMinima");
            entity.Property(e => e.CalificacionMaxima).HasColumnName("CalificacionMaxima");
            entity.Property(e => e.Calificacion).HasColumnName("Calificacion");
            entity.Property(e => e.ActualizadoPor).HasColumnName("actualizadoPor");
            entity.Property(e => e.FechaActualizacion)
             .HasColumnType("datetime")
             .HasColumnName("fechaActualizacion");
            entity.Property(e => e.Estatus).HasColumnName("Estatus");

        });

        modelBuilder.Entity<CatAutoridadesDisposicion>(entity =>
        {
            entity.HasKey(e => e.IdAutoridadDisposicion);

            entity.ToTable("catAutoridadesDisposicion");
            entity.Property(e => e.IdAutoridadDisposicion).HasColumnName("IdAutoridadDisposicion");

            entity.Property(e => e.NombreAutoridadDisposicion).HasColumnName("NombreAutoridadDisposicion")
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.ActualizadoPor).HasColumnName("actualizadoPor");
            entity.Property(e => e.FechaActualizacion)
             .HasColumnType("datetime")
             .HasColumnName("fechaActualizacion");
            entity.Property(e => e.Estatus).HasColumnName("Estatus");

        });

        modelBuilder.Entity<CatAutoridadesEntrega>(entity =>
        {
            entity.HasKey(e => e.IdAutoridadEntrega);

            entity.ToTable("catAutoridadesEntrega");
            entity.Property(e => e.IdAutoridadEntrega).HasColumnName("IdAutoridadEntrega");

            entity.Property(e => e.AutoridadEntrega).HasColumnName("AutoridadEntrega")
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.ActualizadoPor).HasColumnName("actualizadoPor");
            entity.Property(e => e.FechaActualizacion)
             .HasColumnType("datetime")
             .HasColumnName("fechaActualizacion");
            entity.Property(e => e.Estatus).HasColumnName("Estatus");

        });

        modelBuilder.Entity<CatInstitucionesTraslado>(entity =>
        {
            entity.HasKey(e => e.IdInstitucionTraslado);

            entity.ToTable("catInstitucionesTraslado");
            entity.Property(e => e.IdInstitucionTraslado).HasColumnName("IdInstitucionTraslado");

            entity.Property(e => e.InstitucionTraslado).HasColumnName("InstitucionTraslado")
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.ActualizadoPor).HasColumnName("actualizadoPor");
            entity.Property(e => e.FechaActualizacion)
             .HasColumnType("datetime")
             .HasColumnName("fechaActualizacion");
            entity.Property(e => e.Estatus).HasColumnName("Estatus");

        });

        OnModelCreatingPartial(modelBuilder);

        modelBuilder.Entity<CatOficinasRenta>(entity =>
        {
            entity.HasKey(e => e.IdOficinaRenta);

            entity.ToTable("catOficinasRenta");
            entity.Property(e => e.IdOficinaRenta).HasColumnName("IdOficinaRenta");

            entity.Property(e => e.NombreOficina).HasColumnName("NombreOficina")
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.IdDelegacion).HasColumnName("IdDelegacion");
            entity.Property(e => e.ActualizadoPor).HasColumnName("actualizadoPor");
            entity.Property(e => e.FechaActualizacion)
             .HasColumnType("datetime")
             .HasColumnName("fechaActualizacion");
            entity.Property(e => e.Estatus).HasColumnName("Estatus");

        });

        OnModelCreatingPartial(modelBuilder);

        modelBuilder.Entity<CatAgenciasMinisterio>(entity =>
        {
            entity.HasKey(e => e.IdAgenciaMinisterio);

            entity.ToTable("catAgenciasMinisterio");
            entity.Property(e => e.IdAgenciaMinisterio).HasColumnName("IdAgenciaMinisterio");

            entity.Property(e => e.NombreAgencia).HasColumnName("NombreAgencia")
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.IdDelegacion).HasColumnName("IdDelegacion");
            //entity.Property(e => e.Transito).HasColumnName("transito");

            entity.Property(e => e.ActualizadoPor).HasColumnName("actualizadoPor");
            entity.Property(e => e.FechaActualizacion)
             .HasColumnType("datetime")
             .HasColumnName("fechaActualizacion");
            entity.Property(e => e.Estatus).HasColumnName("Estatus");

        });
        OnModelCreatingPartial(modelBuilder);

        modelBuilder.Entity<CatClasificacionAccidentes>(entity =>
        {
            entity.HasKey(e => e.IdClasificacionAccidente);

            entity.ToTable("catClasificacionAccidentes");
            entity.Property(e => e.IdClasificacionAccidente).HasColumnName("IdClasificacionAccidente");

            entity.Property(e => e.NombreClasificacion).HasColumnName("NombreClasificacion")
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.ActualizadoPor).HasColumnName("actualizadoPor");
            entity.Property(e => e.FechaActualizacion)
             .HasColumnType("datetime")
             .HasColumnName("fechaActualizacion");
            entity.Property(e => e.Estatus).HasColumnName("Estatus");

        });

        modelBuilder.Entity<CatFactoresAccidentes>(entity =>
        {
            entity.HasKey(e => e.IdFactorAccidente);

            entity.ToTable("catFactoresAccidentes");
            entity.Property(e => e.IdFactorAccidente).HasColumnName("IdFactorAccidente");

            entity.Property(e => e.FactorAccidente).HasColumnName("FactorAccidente")
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.ActualizadoPor).HasColumnName("actualizadoPor");
            entity.Property(e => e.FechaActualizacion)
             .HasColumnType("datetime")
             .HasColumnName("fechaActualizacion");
            entity.Property(e => e.Estatus).HasColumnName("Estatus");

        });

        modelBuilder.Entity<CatCausasAccidentes>(entity =>
        {
            entity.HasKey(e => e.IdCausaAccidente);

            entity.ToTable("catCausasAccidentes");
            entity.Property(e => e.IdCausaAccidente).HasColumnName("IdCausaAccidente");

            entity.Property(e => e.CausaAccidente).HasColumnName("CausaAccidente")
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.ActualizadoPor).HasColumnName("actualizadoPor");
            entity.Property(e => e.FechaActualizacion)
             .HasColumnType("datetime")
             .HasColumnName("fechaActualizacion");
            entity.Property(e => e.Estatus).HasColumnName("Estatus");

        });

        modelBuilder.Entity<CatFactoresOpcionesAccidentes>(entity =>
        {
            entity.HasKey(e => e.IdFactorOpcionAccidente);

            entity.ToTable("catFactoresOpcionesAccidentes");
            entity.Property(e => e.IdFactorOpcionAccidente).HasColumnName("IdFactorOpcionAccidente");

            entity.Property(e => e.FactorOpcionAccidente).HasColumnName("FactorOpcionAccidente")
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.IdFactorAccidente).HasColumnName("IdFactorAccidente");
            entity.Property(e => e.ActualizadoPor).HasColumnName("actualizadoPor");
            entity.Property(e => e.FechaActualizacion)
             .HasColumnType("datetime")
             .HasColumnName("fechaActualizacion");
            entity.Property(e => e.Estatus).HasColumnName("Estatus");

        });

        modelBuilder.Entity<CatHospitales>(entity =>
        {
            entity.HasKey(e => e.IdHospital);

            entity.ToTable("catHospitales");
            entity.Property(e => e.IdHospital).HasColumnName("IdHospital");

            entity.Property(e => e.NombreHospital).HasColumnName("NombreHospital")
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.IdMunicipio).HasColumnName("IdMunicipio");
            entity.Property(e => e.ActualizadoPor).HasColumnName("actualizadoPor");
            entity.Property(e => e.FechaActualizacion)
             .HasColumnType("datetime")
             .HasColumnName("fechaActualizacion");

            entity.Property(e => e.Estatus).HasColumnName("Estatus");

        });

        modelBuilder.Entity<CatDelegacionesOficinasTransporte>(entity =>
        {
            entity.ToTable("catDelegacionesOficinasTransporte");
            entity.HasKey(e => e.IdOficinaTransporte);
            entity.Property(e => e.IdOficinaTransporte).HasColumnName("idOficinaTransporte");

            entity.Property(e => e.NombreOficina).HasColumnName("nombreOficina")
                .HasMaxLength(100)
                .IsUnicode(false);

            entity.Property(e => e.JefeOficina).HasColumnName("jefeOficina")
               .HasMaxLength(100)
               .IsUnicode(false);

            entity.Property(e => e.IdMunicipio).HasColumnName("IdMunicipio");

            entity.Property(e => e.ActualizadoPor).HasColumnName("actualizadoPor");

            entity.Property(e => e.FechaActualizacion)
             .HasColumnType("datetime")
             .HasColumnName("fechaActualizacion");

            entity.Property(e => e.Estatus).HasColumnName("estatus");

            entity.HasOne(e => e.Delegacion)
              .WithMany(e => e.DelegacionesOficinasTransporte)
              .HasForeignKey(e => e.IdDelegacion);

        });

        OnModelCreatingPartial(modelBuilder);

        modelBuilder.Entity<Infracciones>(entity =>
            {
                entity.HasKey(e => e.IdInfraccion);

                entity.ToTable("infracciones");
                entity.Property(e => e.IdInfraccion).HasColumnName("IdInfraccion");
                entity.Property(e => e.FolioInfraccion).HasColumnName("folioInfraccion")
                 .HasMaxLength(100)
                    .IsUnicode(false);
                entity.Property(e => e.Placas).HasColumnName("placas")
                 .HasMaxLength(100)
                    .IsUnicode(false);
                entity.Property(e => e.IdOficial).HasColumnName("idOficial");
                entity.Property(e => e.IdDependencia).HasColumnName("idDependencia");
                entity.Property(e => e.IdDelegacion).HasColumnName("idDelegacion");
                entity.Property(e => e.Oficial).HasColumnName("oficial")
                    .HasMaxLength(150)
                    .IsUnicode(false);
                entity.Property(e => e.Municipio).HasColumnName("municipio")
                   .HasMaxLength(150)
                   .IsUnicode(false);
                entity.Property(e => e.FechaInfraccion)
                .HasColumnType("datetime")
                .HasColumnName("fechaInfraccion");
                entity.Property(e => e.Carretera).HasColumnName("carretera")
                    .HasMaxLength(100)
                    .IsUnicode(false);
                entity.Property(e => e.Tramo).HasColumnName("tramo")
                       .HasMaxLength(100)
                       .IsUnicode(false);
                entity.Property(e => e.KmCarretera).HasColumnName("kmCarretera")
                       .HasMaxLength(100)
                       .IsUnicode(false);
                entity.Property(e => e.IdVehiculo).HasColumnName("idVehiculo");
                entity.Property(e => e.IdConductor).HasColumnName("IdConductor");
                entity.Property(e => e.Conductor).HasColumnName("conductor")
                      .HasMaxLength(150)
                      .IsUnicode(false);
                entity.Property(e => e.Propietario).HasColumnName("propietario")
                      .HasMaxLength(100)
                      .IsUnicode(false);
                entity.Property(e => e.IdAplicacion).HasColumnName("idAplicacion");
                entity.Property(e => e.InfraccionCortesia).HasColumnName("infraccionCortesia");
                entity.Property(e => e.IdGarantia).HasColumnName("idGarantia");
                entity.Property(e => e.EstatusProceso).HasColumnName("estatusProceso");
                entity.Property(e => e.ActualizadoPor).HasColumnName("actualizadoPor");
                entity.Property(e => e.FechaActualizacion)
                 .HasColumnType("datetime")
                 .HasColumnName("fechaActualizacion");

                entity.Property(e => e.Estatus).HasColumnName("Estatus");


            });

        modelBuilder.Entity<CatMotivosInfraccion>(entity =>
        {
            entity.HasKey(e => e.idCatMotivoInfraccion);

            entity.ToTable("catMotivosInfraccion");

            entity.Property(e => e.idCatMotivoInfraccion).HasColumnName("idCatMotivoInfraccion");

            entity.Property(e => e.Nombre)
                .HasMaxLength(100)
                .IsUnicode(false).
                HasColumnName("nombre");

            entity.Property(e => e.IdConcepto).HasColumnName("IdConcepto");
            entity.Property(e => e.IdSubConcepto).HasColumnName("IdSubConcepto");


            entity.Property(e => e.ActualizadoPor).HasColumnName("actualizadoPor");
            entity.Property(e => e.Estatus).HasColumnName("estatus");
            entity.Property(e => e.FechaActualizacion)
              .HasColumnType("datetime")
              .HasColumnName("fechaActualizacion");

            entity.Property(e => e.CalificacionMinima).HasColumnName("calificacionMinima");
            entity.Property(e => e.CalificacionMaxima).HasColumnName("calificacionMaxima");
            entity.Property(e => e.Fundamento)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("fundamento");
        });

        modelBuilder.Entity<CatConceptoInfraccion>(entity =>
        {
            entity.HasKey(e => e.idConcepto).HasName("idConcepto");

            entity.ToTable("catConceptoInfraccion");

            entity.Property(e => e.idConcepto).HasColumnName("idConcepto");
            entity.Property(e => e.concepto)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("concepto");
            entity.Property(e => e.ActualizadoPor).HasColumnName("actualizadoPor");
            entity.Property(e => e.FechaActualizacion)
             .HasColumnType("datetime")
             .HasColumnName("fechaActualizacion");
            entity.Property(e => e.Estatus).HasColumnName("estatus");
        });

        modelBuilder.Entity<CatSubConceptoInfraccion>(entity =>
        {
            entity.HasKey(e => e.idSubConcepto).HasName("idSubConcepto");
            entity.ToTable("catSubConceptoInfraccion");
            entity.Property(e => e.idSubConcepto).HasColumnName("idSubConcepto");

            entity.Property(e => e.subConcepto)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("subConcepto");

            entity.Property(e => e.idConcepto).HasColumnName("idConcepto");
            entity.Property(e => e.ActualizadoPor).HasColumnName("actualizadoPor");
            entity.Property(e => e.FechaActualizacion)
             .HasColumnType("datetime")
             .HasColumnName("fechaActualizacion");
            entity.Property(e => e.Estatus).HasColumnName("estatus");
        });

        modelBuilder.Entity<CatDelegacione>(entity =>
        {
            entity.HasKey(e => e.IdDelegacion);

            entity.ToTable("catDelegaciones");

            entity.Property(e => e.IdDelegacion).HasColumnName("idDelegacion");
            entity.Property(e => e.ActualizadoPor).HasColumnName("actualizadoPor");
            entity.Property(e => e.Delegacion)
                .HasMaxLength(100)
                .HasColumnName("delegacion");

            entity.Property(e => e.Estatus).HasColumnName("estatus");

            entity.Property(e => e.FechaActualizacion)
                .HasColumnType("datetime")
                .HasColumnName("fechaActualizacion");

            entity.Property(e => e.Transito)
                .HasColumnName("transito");
        });

        modelBuilder.Entity<CatTipoBloc>(entity =>
        {
            entity.HasKey(e => e.TipoBlocId).HasName("PK_catTipoBloc_tipoBlocId");

            entity.ToTable("catTipoBlocs");

            entity.Property(e => e.TipoBlocId).HasColumnName("tipoBlocId");
            entity.Property(e => e.Abreviatura)
                .IsRequired()
                .HasMaxLength(4)
                .HasColumnName("abreviatura");
            entity.Property(e => e.Tipo)
                .IsRequired()
                .HasMaxLength(30)
                .HasColumnName("tipo");
        });

        modelBuilder.Entity<DetalleBloc>(entity =>
        {
            entity.HasKey(e => e.DetalleId).HasName("PK_detalleBlocs_detalleId");

            entity.ToTable("detalleBlocs");

            entity.Property(e => e.DetalleId).HasColumnName("detalleId");
            entity.Property(e => e.Comentarios)
               .HasMaxLength(150)
               .HasColumnName("comentarios");
            entity.Property(e => e.Estatus)
                .IsRequired()
                .HasMaxLength(25)
                .HasColumnName("estatus");
            entity.Property(e => e.FolioFinal).HasColumnName("folioFinal");
            
            entity.Property(e => e.idEstatusFolio).HasColumnName("idEstatusFolio");
            entity.Property(e => e.idUsrUsa).HasColumnName("idUsrUsa");
            entity.Property(e => e.folio).HasColumnName("folio");
            entity.Property(e => e.idAccidente).HasColumnName("idAccidente");
            entity.Property(e => e.idInfraccion).HasColumnName("idInfraccion");
            entity.Property(e => e.idDeposito).HasColumnName("idDeposito");



            entity.Property(e => e.FolioInicial).HasColumnName("folioInicial");
            entity.Property(e => e.RegistraId).HasColumnName("registraId");
            entity.Property(e => e.Serie)
                .IsRequired()
                .HasColumnName("serie");

            entity.HasOne(d => d.Registra).WithMany(p => p.DetalleBlocs)
                .HasForeignKey(d => d.RegistraId)
                .HasConstraintName("FK_detalleBlocs_registraId");
        });

        modelBuilder.Entity<RegistraBloc>(entity =>
        {
            entity.HasKey(e => e.RegistraId).HasName("PK_registraBlocs_serie");

            entity.ToTable("registraBlocs");

            entity.Property(e => e.RegistraId).HasColumnName("registraId");
            entity.Property(e => e.IdOficial).HasColumnName("IdOficial");
            entity.Property(e => e.transito).HasColumnName("transito");
            entity.Property(e => e.ActualizadoPor).HasColumnName("actualizadoPor");
            entity.Property(e => e.idEstatusBlock).HasColumnName("idEstatusBlock");
            entity.Property(e => e.idUsrAlta).HasColumnName("idUsrAlta");

            entity.Property(e => e.AsignadorBloc)
                .IsRequired()
                .HasColumnName("asignadorBloc");
            entity.Property(e => e.Delegacion).HasColumnName("delegacion");
            entity.Property(e => e.FechaActualizacion)
                .HasColumnType("datetime")
                .HasColumnName("fechaActualizacion");
            entity.Property(e => e.FechaAsignacion)
                .HasColumnType("datetime")
                .HasColumnName("fechaAsignacion");
            entity.Property(e => e.FechaCarga)
                .HasColumnType("datetime")
                .HasColumnName("fechaCarga");
            entity.Property(e => e.FolioFinal).HasColumnName("folioFinal");
            entity.Property(e => e.FolioInicial).HasColumnName("folioInicial");
            entity.Property(e => e.Municipio).HasColumnName("municipio");
            entity.Property(e => e.OficialAsignado).HasColumnName("oficialAsignado");
            entity.Property(e => e.ResponsableCarga)
                .IsRequired()
                .HasColumnName("responsableCarga");
            entity.Property(e => e.Serie)
                .IsRequired()
                .HasColumnName("serie");
            entity.Property(e => e.TipoBlocId).HasColumnName("tipoBlocId");
            entity.Property(e => e.TotalBoletas).HasColumnName("totalBoletas");
            entity.Property(e => e.UrlEvidencia).HasColumnName("urlEvidencia");

            entity.HasOne(d => d.TipoBloc).WithMany(p => p.RegistraBlocs)
                .HasForeignKey(d => d.TipoBlocId)
                .HasConstraintName("FK_registraBlocs_tipoBlocId");
        });

        modelBuilder.Entity<Accidente>(entity =>
        {
            entity.HasKey(e => e.IdAccidente).HasName("PK_accidentes_");

            entity.ToTable("accidentes");

            entity.Property(e => e.IdAccidente).HasColumnName("idAccidente");
            entity.Property(e => e.ActualizadoPor).HasColumnName("actualizadoPor");
            entity.Property(e => e.Armas).HasColumnName("armas");
            entity.Property(e => e.ArmasTexto)
                .HasMaxLength(300)
                .HasColumnName("armasTexto");
            entity.Property(e => e.ConsignacionHechos)
                .HasMaxLength(150)
                .HasColumnName("consignacionHechos");
            entity.Property(e => e.Convenio).HasColumnName("convenio");
            entity.Property(e => e.DescripcionCausas)
                .HasMaxLength(1200)
                .HasColumnName("descripcionCausas");
            entity.Property(e => e.Drogas).HasColumnName("drogas");
            entity.Property(e => e.DrogasTexto)
                .HasMaxLength(300)
                .HasColumnName("drogasTexto");
            entity.Property(e => e.EntregaObjetos)
                .HasMaxLength(150)
                .HasColumnName("entregaObjetos");
            entity.Property(e => e.EntregaOtros)
                .HasMaxLength(150)
                .HasColumnName("entregaOtros");
            entity.Property(e => e.Estatus).HasColumnName("estatus");
            entity.Property(e => e.EstatusReporte).HasColumnName("estatusReporte");
            entity.Property(e => e.Fecha)
                .HasColumnType("date")
                .HasColumnName("fecha");
            entity.Property(e => e.FechaActualizacion)
                .HasColumnType("datetime")
                .HasColumnName("fechaActualizacion");
            entity.Property(e => e.Hora).HasColumnName("hora");
            entity.Property(e => e.IdAgenciaMinisterio).HasColumnName("idAgenciaMinisterio");
            entity.Property(e => e.IdAutoridadDisposicion).HasColumnName("idAutoridadDisposicion");
            entity.Property(e => e.IdAutoridadEntrega).HasColumnName("idAutoridadEntrega");
            entity.Property(e => e.IdAutoriza).HasColumnName("idAutoriza");
            entity.Property(e => e.IdCarretera).HasColumnName("idCarretera");
            entity.Property(e => e.IdCausaAccidente).HasColumnName("idCausaAccidente");
            entity.Property(e => e.IdCertificado).HasColumnName("idCertificado");
            entity.Property(e => e.IdCiudad).HasColumnName("idCiudad");
            entity.Property(e => e.IdClasificacionAccidente).HasColumnName("idClasificacionAccidente");
            entity.Property(e => e.IdElabora).HasColumnName("idElabora");
            entity.Property(e => e.IdElaboraConsignacion).HasColumnName("idElaboraConsignacion");
            entity.Property(e => e.IdEntidadCompetencia).HasColumnName("idEntidadCompetencia");
            entity.Property(e => e.IdEstatusReporte).HasColumnName("idEstatusReporte");
            entity.Property(e => e.IdFactorAccidente).HasColumnName("idFactorAccidente");
            entity.Property(e => e.IdFactorOpcionAccidente).HasColumnName("idFactorOpcionAccidente");
            entity.Property(e => e.IdMunicipio).HasColumnName("idMunicipio");
            entity.Property(e => e.IdOficinaDelegacion).HasColumnName("idOficinaDelegacion");
            entity.Property(e => e.IdSupervisa).HasColumnName("idSupervisa");
            entity.Property(e => e.IdTramo).HasColumnName("idTramo");
            entity.Property(e => e.Kilometro)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("kilometro");
            entity.Property(e => e.Latitud).HasColumnName("latitud");
            entity.Property(e => e.Longitud).HasColumnName("longitud");
            entity.Property(e => e.MontoCamino)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("montoCamino");
            entity.Property(e => e.MontoCarga)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("montoCarga");
            entity.Property(e => e.MontoOtros)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("montoOtros");
            entity.Property(e => e.MontoPropietarios)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("montoPropietarios");
            entity.Property(e => e.MontoVehiculo).HasColumnName("montoVehiculo");
            entity.Property(e => e.NumeroOficio)
                .HasMaxLength(50)
                .HasColumnName("numeroOficio");
            entity.Property(e => e.NumeroReporte)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("numeroReporte");
            entity.Property(e => e.NumeroReporteMigrado)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("numeroReporteMigrado");
            entity.Property(e => e.ObservacionesConvenio)
                .HasMaxLength(300)
                .HasColumnName("observacionesConvenio");
            entity.Property(e => e.Otros).HasColumnName("otros");
            entity.Property(e => e.OtrosTexto)
                .HasMaxLength(300)
                .HasColumnName("otrosTexto");
            entity.Property(e => e.Prendas).HasColumnName("prendas");
            entity.Property(e => e.PrendasTexto)
                .HasMaxLength(300)
                .HasColumnName("prendasTexto");
            entity.Property(e => e.RecibeMinisterio)
                .HasMaxLength(150)
                .HasColumnName("recibeMinisterio");
            entity.Property(e => e.Transito).HasColumnName("transito");
            entity.Property(e => e.Valores).HasColumnName("valores");
            entity.Property(e => e.ValoresTexto)
                .HasMaxLength(300)
                .HasColumnName("valoresTexto");
            entity.Property(e => e.UrlAccidenteArchivosLugar)
                .HasColumnName("urlAccidenteArchivosLugar");
        });

        modelBuilder.Entity<AccidentesEmergencia>(entity =>
        {
            entity.HasKey(e => e.EmergenciasId).HasName("PK_accidentesEmergencias_emergenciaId");

            entity.ToTable("accidentesEmergencias");

            entity.Property(e => e.EmergenciasId).HasColumnName("emergenciasId");
            entity.Property(e => e.Delegacion).HasColumnName("delegacion");
            entity.Property(e => e.FolioEmergencia)
                .IsRequired()
                .HasMaxLength(25)
                .HasColumnName("folioEmergencia");
            entity.Property(e => e.IdAccidente).HasColumnName("idAccidente");
            entity.Property(e => e.Municipio).HasColumnName("municipio");
            entity.Property(e => e.Oficina).HasColumnName("oficina");

            entity.HasOne(d => d.IdAccidenteNavigation).WithMany(p => p.AccidentesEmergencia)
                .HasForeignKey(d => d.IdAccidente)
                .HasConstraintName("FK_accidentesEmergencias_idInfraccion");
        });


        modelBuilder.Entity<InfraccionesEmergencia>(entity =>
        {
            entity.HasKey(e => e.EmergenciasId).HasName("PK_infraccionesEmergencias_emergenciaId");

            entity.ToTable("infraccionesEmergencias");

            entity.Property(e => e.EmergenciasId).HasColumnName("emergenciasId");
            entity.Property(e => e.Delegacion).HasColumnName("delegacion");
            entity.Property(e => e.FolioEmergencia)
                .IsRequired()
                .HasMaxLength(25)
                .HasColumnName("folioEmergencia");
            entity.Property(e => e.IdInfraccion).HasColumnName("idInfraccion");
            entity.Property(e => e.Municipio).HasColumnName("municipio");
            entity.Property(e => e.Oficina).HasColumnName("oficina");

            entity.HasOne(d => d.IdInfraccionNavigation).WithMany(p => p.InfraccionesEmergencia)
                .HasForeignKey(d => d.IdInfraccion)
                .HasConstraintName("FK_infraccionesEmergencias_idInfraccion");
        });

        modelBuilder.Entity<CatTurno>(entity =>
        {
            entity.ToTable("catTurnos");

            entity.HasKey(e => e.IdTurno);
            entity.Property(e => e.IdTurno)
                .HasColumnName("idTurno")
                .ValueGeneratedOnAdd();

            entity.Property(e => e.IdDelegacion)
                .HasColumnName("idDelegacion")
                .IsRequired();

            entity.Property(e => e.IdMunicipio)
                .HasColumnName("idMunicipio")
                .IsRequired();

            entity.Property(e => e.NombreTurno)
                .HasColumnName("nombreTurno")
                .IsRequired()
                .HasMaxLength(150);

            entity.Property(e => e.InicioTurno)
                .HasColumnName("inicioTurno")
                .IsRequired();

            entity.Property(e => e.FinTurno)
                .HasColumnName("finTurno")
                .IsRequired();

            entity.Property(e => e.FechaActualizacion)
                .HasColumnName("fechaActualizacion")
                .IsRequired();

            entity.Property(e => e.ActualizadoPor)
                .HasColumnName("actualizadoPor")
                .IsRequired();

            entity.HasOne(e => e.Delegacion)
                .WithMany()
                .HasForeignKey(e => e.IdDelegacion);

            entity.HasOne(e => e.Municipio)
                .WithMany()
                .HasForeignKey(e => e.IdMunicipio);
        });

        modelBuilder.Entity<TurnoInfraccion>(entity =>
        {
            entity.ToTable("turnoInfracciones");

            entity.HasKey(e => e.IdTurnoInfraccion);
            entity.Property(e => e.IdTurnoInfraccion)
                .HasColumnName("idTurnoInfraccion")
                .ValueGeneratedOnAdd();

            entity.Property(e => e.IdTurno)
                .HasColumnName("idTurno")
                .IsRequired();

            entity.Property(e => e.IdInfraccion)
                .HasColumnName("idInfraccion")
                .IsRequired();
        });

        modelBuilder.Entity<AccidentesDatosAseguradora>(entity =>
        {
            entity.HasKey(e => e.IdDatosAseguradora).HasName("PK_accidentesDatosAseguradora_idDatosAseguradora");

            entity.ToTable("accidentesDatosAseguradora");

            entity.Property(e => e.IdDatosAseguradora).HasColumnName("idDatosAseguradora");
            entity.Property(e => e.FechaExpiracion)
                .HasColumnType("date")
                .HasColumnName("fechaExpiracion");
            entity.Property(e => e.IdAccidente).HasColumnName("idAccidente");
            entity.Property(e => e.IdAseguradora).HasColumnName("idAseguradora");
            entity.Property(e => e.IdPersona).HasColumnName("idPersona");
            entity.Property(e => e.Poliza)
                .HasMaxLength(45)
                .HasColumnName("poliza");
            entity.Property(e => e.fechaActualizacion)
              .HasColumnType("datetime")
              .HasColumnName("fechaActualizacion");
            entity.HasOne(d => d.IdAseguradoraNavigation).WithMany(p => p.AccidentesDatosAseguradoras)
                .HasForeignKey(d => d.IdAseguradora)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_accidentesDatosAseguradora_idAseguradora");
        });

        modelBuilder.Entity<CatAseguradoras>(entity =>
        {
            entity.HasKey(e => e.IdAseguradora).HasName("PK_catAseguradoras_idAseguradora");

            entity.ToTable("catAseguradoras");

            entity.Property(e => e.IdAseguradora).HasColumnName("idAseguradora");
            entity.Property(e => e.ActualizadoPor).HasColumnName("actualizadoPor");
            entity.Property(e => e.Estatus).HasColumnName("estatus");
            entity.Property(e => e.FechaActualizacion)
                .HasColumnType("date")
                .HasColumnName("fechaActualizacion");
            entity.Property(e => e.NombreAseguradora)
                .IsRequired()
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("nombreAseguradora");
        });

        modelBuilder.Entity<CatPuesto>(entity =>
        {
            entity.ToTable("catPuestos");
            entity.HasKey(e => e.IdPuesto);

            entity.HasOne(e => e.Delegacion)
                .WithMany()
                .HasForeignKey(e => e.IdDelegacion);
        });
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
