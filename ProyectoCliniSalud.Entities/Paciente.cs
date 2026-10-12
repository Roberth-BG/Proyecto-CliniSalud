using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProyectoCliniSalud.Entities
{
    [Table("Pacientes")]
    public class Paciente
    {
        [Key]
        [Column("id_paciente")]
        public int IdPaciente { get; set; }

        [Required]
        [Column("id_usuario")]
        public int IdUsuario { get; set; }

        [ForeignKey("IdUsuario")]
        public Usuario? Usuario { get; set; }

        [Required]
        [StringLength(12)]
        [Column("dni")]
        public string Dni { get; set; } = string.Empty;

        [Required]
        [StringLength(80)]
        [Column("nombres")]
        public string Nombres { get; set; } = string.Empty;

        [Required]
        [StringLength(80)]
        [Column("apellidos")]
        public string Apellidos { get; set; } = string.Empty;

        [Required]
        [StringLength(15)]
        [Column("telefono")]
        public string Telefono { get; set; } = string.Empty;

        [StringLength(255)]
        [Column("foto_url")]
        public string? FotoUrl { get; set; }

        public ICollection<Cita> Citas { get; set; } = new List<Cita>();
        public ICollection<Testimonio> Testimonios { get; set; } = new List<Testimonio>();
    }
}