using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PortalAcademico.Data;
using Microsoft.EntityFrameworkCore;

[Authorize]
public class MatriculasController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<IdentityUser> _userManager;

    public MatriculasController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    [HttpPost]
    public async Task<IActionResult> Inscribirse(int CursoId)
    {
        var curso = await _context.Cursos.FindAsync(CursoId);
        if (curso == null || !curso.Activo)
            return NotFound("Curso no encontrado o inactivo.");

        var usuario = await _userManager.GetUserAsync(User);
        if (usuario == null)
            return Unauthorized();

        // Validación: ya inscrito
        bool yaInscrito = await _context.Matriculas.AnyAsync(m => m.CursoId == CursoId && m.UsuarioId == usuario.Id);
        if (yaInscrito)
        {
            TempData["Error"] = "Ya estás inscrito en este curso.";
            return RedirectToAction("Detalle", "Cursos", new { id = CursoId });
        }

        // Validación: cupo máximo
        int inscritos = await _context.Matriculas.CountAsync(m => m.CursoId == CursoId && m.Estado != EstadoMatricula.Cancelada);
        if (inscritos >= curso.CupoMaximo)
        {
            TempData["Error"] = "No hay cupos disponibles.";
            return RedirectToAction("Detalle", "Cursos", new { id = CursoId });
        }

        // Validación: solapamiento de horario
        var matriculasUsuario = await _context.Matriculas
            .Include(m => m.Curso)
            .Where(m => m.UsuarioId == usuario.Id && m.Estado != EstadoMatricula.Cancelada)
            .ToListAsync();

        bool solapado = matriculasUsuario.Any(m =>
            (curso.HorarioInicio < m.Curso.HorarioFin) &&
            (m.Curso.HorarioInicio < curso.HorarioFin));

        if (solapado)
        {
            TempData["Error"] = "El horario se solapa con otro curso ya inscrito.";
            return RedirectToAction("Detalle", "Cursos", new { id = CursoId });
        }

        // Crear matrícula
        var matricula = new Matricula
        {
            CursoId = CursoId,
            UsuarioId = usuario.Id,
            FechaRegistro = DateTime.Now,
            Estado = EstadoMatricula.Pendiente
        };

        _context.Matriculas.Add(matricula);
        await _context.SaveChangesAsync();

        TempData["Exito"] = "Inscripción realizada correctamente.";
        return RedirectToAction("Detalle", "Cursos", new { id = CursoId });
    }
}