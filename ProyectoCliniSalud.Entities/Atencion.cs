using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProyectoCliniSalud.Entities
{
    [Table("Atenciones")]
    public class Atencion
    {
        [Key]
        [Column("id_atencion")]
        public int IdAtencion { get; set; }

        [Required]
        [Column("id_cita")]
        public int IdCita { get; set; }

        [ForeignKey("IdCita")]
        public Cita? Cita { get; set; }

        [Required]
        [StringLength(500)]
        [Column("diagnostico")]
        public string Diagnostico { get; set; } = string.Empty;

        [Required]
        [Column("tratamiento")]
        public string Tratamiento { get; set; } = string.Empty;

        [Required]
        [Column("fecha_atencion")]
        public DateTime FechaAtencion { get; set; } = DateTime.Now;
    }
}