using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProyectoCliniSalud.Entities
{
    [Table("Medicos")]
    public class Medico
    {
        [Key]
        [Column("id_medico")]
        public int IdMedico { get; set; }

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
        [StringLength(12)]
        [Column("cmp")]
        public string Cmp { get; set; } = string.Empty;

        [StringLength(15)]
        [Column("telefono")]
        public string? Telefono { get; set; }

        [Required]
        [StringLength(20)]
        [Column("consultorio")]
        public string Consultorio { get; set; } = string.Empty;

        [Required]
        [Column("id_especialidad")]
        public int IdEspecialidad { get; set; }

        [ForeignKey("IdEspecialidad")]
        public Especialidad? Especialidad { get; set; }

        [StringLength(255)]
        [Column("foto_url")]
        public string? FotoUrl { get; set; }

        public ICollection<Cita> Citas { get; set; } = new List<Cita>();
        public ICollection<Testimonio> Testimonios { get; set; } = new List<Testimonio>();
    }
}