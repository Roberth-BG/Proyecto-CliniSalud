using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProyectoCliniSalud.Entities
{
    [Table("Testimonios")]
    public class Testimonio
    {
        [Key]
        [Column("id_testimonio")]
        public int IdTestimonio { get; set; }

        [Required]
        [Column("id_paciente")]
        public int IdPaciente { get; set; }

        [ForeignKey("IdPaciente")]
        public Paciente? Paciente { get; set; }

        [Column("id_medico")]
        public int? IdMedico { get; set; }

        [ForeignKey("IdMedico")]
        public Medico? Medico { get; set; }

        [Required]
        [Column("calificacion")]
        public int Calificacion { get; set; } = 5;

        [Required]
        [StringLength(400)]
        [Column("comentario")]
        public string Comentario { get; set; } = string.Empty;

        [Required]
        [StringLength(20)]
        [Column("estado")]
        public string Estado { get; set; } = "Pendiente";

        [Required]
        [Column("fecha")]
        public DateTime Fecha { get; set; } = DateTime.Now;
    }
}