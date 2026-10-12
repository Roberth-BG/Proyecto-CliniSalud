using Microsoft.EntityFrameworkCore;
using ProyectoCliniSalud.Entities;

namespace ProyectoCliniSalud.DataAccess.Data
{
    public class CliniSaludDbContext : DbContext
    {
        public CliniSaludDbContext(DbContextOptions<CliniSaludDbContext> options) : base(options)
        {
        }

        public DbSet<Rol> Roles { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Especialidad> Especialidades { get; set; }
        public DbSet<Medico> Medicos { get; set; }
        public DbSet<Paciente> Pacientes { get; set; }
        public DbSet<Cita> Citas { get; set; }
        public DbSet<Atencion> Atenciones { get; set; }
        public DbSet<Testimonio> Testimonios { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            foreach (var relationship in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
            {
                relationship.DeleteBehavior = DeleteBehavior.Restrict;
            }

            modelBuilder.Entity<Usuario>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<Rol>()
                .HasIndex(r => r.NombreRol)
                .IsUnique();

            modelBuilder.Entity<Especialidad>()
                .HasIndex(e => e.NombreEspecialidad)
                .IsUnique();

            modelBuilder.Entity<Medico>()
                .HasIndex(m => m.Dni)
                .IsUnique();

            modelBuilder.Entity<Medico>()
                .HasIndex(m => m.Cmp)
                .IsUnique();

            modelBuilder.Entity<Paciente>()
                .HasIndex(p => p.Dni)
                .IsUnique();

            modelBuilder.Entity<Cita>()
                .HasIndex(c => c.CodigoTicket)
                .IsUnique();
        }
    }
}