using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;


namespace PortalAcademico.Data;

public class ApplicationDbContext : IdentityDbContext<IdentityUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) { }

    public DbSet<Curso> Cursos { get; set; }
    public DbSet<Matricula> Matriculas { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Curso>()
            .HasIndex(c => c.Codigo)
            .IsUnique();

        builder.Entity<Matricula>()
            .HasIndex(m => new { m.CursoId, m.UsuarioId })
            .IsUnique();

        builder.Entity<Curso>()
            .HasCheckConstraint("CK_Curso_Creditos", "Creditos > 0")
            .HasCheckConstraint("CK_Curso_Horario", "HorarioInicio < HorarioFin");
    }
}