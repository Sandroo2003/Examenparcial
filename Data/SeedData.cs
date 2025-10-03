using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

public static class SeedData
{
    public static async Task InicializarAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

        await context.Database.MigrateAsync();

        if (!await roleManager.RoleExistsAsync("Coordinador"))
        {
            await roleManager.CreateAsync(new IdentityRole("Coordinador"));
        }

        var user = await userManager.FindByEmailAsync("coordinador@uni.edu.pe");
        if (user == null)
        {
            user = new IdentityUser { UserName = "coordinador@uni.edu.pe", Email = "coordinador@uni.edu.pe", EmailConfirmed = true };
            await userManager.CreateAsync(user, "Password123!");
            await userManager.AddToRoleAsync(user, "Coordinador");
        }

        if (!context.Cursos.Any())
        {
            context.Cursos.AddRange(
                new Curso { Codigo = "MAT101", Nombre = "Matemática I", Creditos = 4, CupoMaximo = 30, HorarioInicio = new TimeSpan(8, 0, 0), HorarioFin = new TimeSpan(10, 0, 0), Activo = true },
                new Curso { Codigo = "INF102", Nombre = "Introducción a la Programación", Creditos = 3, CupoMaximo = 25, HorarioInicio = new TimeSpan(10, 0, 0), HorarioFin = new TimeSpan(12, 0, 0), Activo = true },
                new Curso { Codigo = "FIS103", Nombre = "Física I", Creditos = 4, CupoMaximo = 20, HorarioInicio = new TimeSpan(14, 0, 0), HorarioFin = new TimeSpan(16, 0, 0), Activo = true }
            );
            await context.SaveChangesAsync();
        }
    }
}
