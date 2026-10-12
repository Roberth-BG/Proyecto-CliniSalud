using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProyectoCliniSalud.Entities
{
    [Table("Citas")]
    public class Cita
    {
        [Key]
        [Column("id_cita")]
        public int IdCita { get; set; }

        [Required]
        [StringLength(10)]
        [Column("codigo_ticket")]
        public string CodigoTicket { get; set; } = string.Empty;

        [Required]
        [Column("id_paciente")]
        public int IdPaciente { get; set; }

        [ForeignKey("IdPaciente")]
        public Paciente? Paciente { get; set; }

        [Required]
        [Column("id_medico")]
        public int IdMedico { get; set; }

        [ForeignKey("IdMedico")]
        public Medico? Medico { get; set; }

        [Required]
        [Column("fecha_cita", TypeName = "date")]
        public DateTime FechaCita { get; set; }

        [Required]
        [Column("hora_cita")]
        public TimeSpan HoraCita { get; set; }

        [Required]
        [StringLength(500)]
        [Column("motivo_consulta")]
        public string MotivoConsulta { get; set; } = string.Empty;

        [Required]
        [StringLength(20)]
        [Column("estado")]
        public string Estado { get; set; } = "Reservada";

        [Required]
        [Column("fecha_reserva")]
        public DateTime FechaReserva { get; set; } = DateTime.Now;

        public Atencion? Atencion { get; set; }
    }
}