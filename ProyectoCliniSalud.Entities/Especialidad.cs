using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProyectoCliniSalud.Entities
{
    [Table("Especialidades")]
    public class Especialidad
    {
        [Key]
        [Column("id_especialidad")]
        public int IdEspecialidad { get; set; }

        [Required]
        [StringLength(100)]
        [Column("nombre_especialidad")]
        public string NombreEspecialidad { get; set; } = string.Empty;

        [StringLength(255)]
        [Column("descripcion")]
        public string? Descripcion { get; set; }

        [Required]
        [Column("activo")]
        public bool Activo { get; set; } = true;

        public ICollection<Medico> Medicos { get; set; } = new List<Medico>();
    }
}