using Microsoft.EntityFrameworkCore;
using SIGO.Api.Canales.Model;
using SIGO.Api.Departamentos.Model;
using SIGO.Api.Estados.Model;
using SIGO.Api.Oficios.Model;
using SIGO.Api.Prioridades.Model;
using SIGO.Api.Roles.Model;
using SIGO.Api.TiposOficios.Model;
using SIGO.Api.Usuarios.Model;

namespace SIGO.Api
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Departamento> Departamentos => Set<Departamento>();
        public DbSet<Rol> Roles => Set<Rol>();
        public DbSet<Usuario> Usuarios => Set<Usuario>();
        public DbSet<UsuarioRol> UsuarioRoles => Set<UsuarioRol>();
        public DbSet<Prioridad> Prioridades => Set<Prioridad>();
        public DbSet<Estado> Estados => Set<Estado>();
        public DbSet<TipoOficio> TiposOficios => Set<TipoOficio>();
        public DbSet<Canal> Canales => Set<Canal>();
        public DbSet<Oficio> Oficios => Set<Oficio>();
        public DbSet<OficioResponsable> OficioResponsables => Set<OficioResponsable>();
        public DbSet<OficioBitacora> OficioBitacoras => Set<OficioBitacora>();
        public DbSet<OficioAdjunto> OficioAdjuntos => Set<OficioAdjunto>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<UsuarioRol>(e =>
            {
                e.HasKey(ur => new { ur.IdUsuario, ur.IdRol });
                e.HasOne<Usuario>().WithMany(u => u.Roles).HasForeignKey(ur => ur.IdUsuario);
                e.HasOne(ur => ur.Rol).WithMany().HasForeignKey(ur => ur.IdRol);
            });

            modelBuilder.Entity<Usuario>(e =>
            {
                e.HasIndex(u => u.Correo).IsUnique();
                e.HasOne(u => u.Departamento).WithMany().HasForeignKey(u => u.IdDepartamento);
            });

            modelBuilder.Entity<Oficio>(e =>
            {
                e.HasIndex(o => new { o.Numero, o.Anio }).IsUnique();          // IX_Oficio_Numero_Anio
                e.HasIndex(o => new { o.IdEstadoActual, o.IdPrioridad });      // IX_Oficio_Estado_Prioridad
                e.HasIndex(o => o.IdDepartamentoDestino);                      // IX_Oficio_DepDestino
                e.HasIndex(o => o.FechaIngreso);                               // IX_Oficio_FechaIngreso
                e.HasIndex(o => o.FechaVencimientoSla);                        // IX_Oficio_FechaVencimiento

                e.HasOne(o => o.TipoOficio).WithMany().HasForeignKey(o => o.IdTipoOficio);
                e.HasOne(o => o.DepartamentoOrigen).WithMany().HasForeignKey(o => o.IdDepartamentoOrigen);
                e.HasOne(o => o.DepartamentoDestino).WithMany().HasForeignKey(o => o.IdDepartamentoDestino);
                e.HasOne(o => o.Prioridad).WithMany().HasForeignKey(o => o.IdPrioridad);
                e.HasOne(o => o.EstadoActual).WithMany().HasForeignKey(o => o.IdEstadoActual);
                e.HasOne(o => o.Canal).WithMany().HasForeignKey(o => o.IdCanal);
                e.HasOne(o => o.Creador).WithMany().HasForeignKey(o => o.CreadoPor);
            });

            modelBuilder.Entity<OficioResponsable>(e =>
            {
                e.HasOne<Oficio>().WithMany(o => o.Responsables).HasForeignKey(r => r.IdOficio)
                    .OnDelete(DeleteBehavior.Cascade);
                e.HasOne(r => r.Usuario).WithMany().HasForeignKey(r => r.IdUsuario);
                e.HasOne(r => r.Asignador).WithMany().HasForeignKey(r => r.AsignadoPor);
            });

            modelBuilder.Entity<OficioBitacora>(e =>
            {
                e.HasOne<Oficio>().WithMany(o => o.Bitacora).HasForeignKey(b => b.IdOficio)
                    .OnDelete(DeleteBehavior.Cascade);
                e.HasOne(b => b.EstadoOrigen).WithMany().HasForeignKey(b => b.IdEstadoOrigen);
                e.HasOne(b => b.EstadoDestino).WithMany().HasForeignKey(b => b.IdEstadoDestino);
                e.HasOne(b => b.Usuario).WithMany().HasForeignKey(b => b.IdUsuario);
            });

            modelBuilder.Entity<OficioAdjunto>(e =>
            {
                e.HasOne<Oficio>().WithMany(o => o.Adjuntos).HasForeignKey(a => a.IdOficio)
                    .OnDelete(DeleteBehavior.Cascade);
                e.HasOne(a => a.Subidor).WithMany().HasForeignKey(a => a.SubidoPor);
            });

            // SQL Server rejects multiple cascade paths. Solo cascadean las filas propias del
            // agregado (Oficio → responsables/bitácora/adjuntos, Usuario → UsuarioRol);
            // todo lo demás queda en Restrict — borrar un catálogo exige no tener referencias.
            foreach (var fk in modelBuilder.Model.GetEntityTypes().SelectMany(t => t.GetForeignKeys()))
            {
                var esFilaPropia =
                    (fk.PrincipalEntityType.ClrType == typeof(Oficio)
                        && (fk.DeclaringEntityType.ClrType == typeof(OficioResponsable)
                            || fk.DeclaringEntityType.ClrType == typeof(OficioBitacora)
                            || fk.DeclaringEntityType.ClrType == typeof(OficioAdjunto)))
                    || (fk.PrincipalEntityType.ClrType == typeof(Usuario)
                        && fk.DeclaringEntityType.ClrType == typeof(UsuarioRol));

                fk.DeleteBehavior = esFilaPropia ? DeleteBehavior.Cascade : DeleteBehavior.Restrict;
            }
        }
    }
}
