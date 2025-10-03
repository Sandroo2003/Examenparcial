using System.ComponentModel.DataAnnotations;
    
public class Curso
{
    public int Id { get; set; }
    [Required]
    [StringLength(10)]
    public string Codigo { get; set; }
    [Required]
    public string Nombre { get; set; }
    [Range(1, int.MaxValue)]
    public int Creditos { get; set; }
    [Range(1, int.MaxValue)]
    public int CupoMaximo { get; set; }
    public TimeSpan HorarioInicio { get; set; }
    public TimeSpan HorarioFin { get; set; }
    public bool Activo { get; set; }

    public ICollection<Matricula> Matriculas { get; set; }
}