using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PortalAcademico.Data;
using Microsoft.EntityFrameworkCore;

public class CursosController : Controller
{
    private readonly ApplicationDbContext _context;

    public CursosController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index(string nombre, int? minCreditos, int? maxCreditos, TimeSpan? inicio, TimeSpan? fin)
    {
        var cursos = _context.Cursos.Where(c => c.Activo);

        if (!string.IsNullOrEmpty(nombre))
            cursos = cursos.Where(c => c.Nombre.Contains(nombre));

        if (minCreditos.HasValue)
            cursos = cursos.Where(c => c.Creditos >= minCreditos.Value);

        if (maxCreditos.HasValue)
            cursos = cursos.Where(c => c.Creditos <= maxCreditos.Value);

        if (inicio.HasValue)
            cursos = cursos.Where(c => c.HorarioInicio >= inicio.Value);

        if (fin.HasValue)
            cursos = cursos.Where(c => c.HorarioFin <= fin.Value);

        return View(await cursos.ToListAsync());
    }

    public async Task<IActionResult> Detalle(int id)
    {
        var curso = await _context.Cursos.FindAsync(id);
        if (curso == null || !curso.Activo)
            return NotFound();

        return View(curso);
    }
}