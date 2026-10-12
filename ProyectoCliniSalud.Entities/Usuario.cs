using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProyectoCliniSalud.Entities
{
    [Table("Usuarios")]
    public class Usuario
    {
        [Key]
        [Column("id_usuario")]
        public int IdUsuario { get; set; }

        [Required]
        [StringLength(100)]
        [Column("email")]
        public string Email { get; set; } = string.Empty;

        [Required]
        [StringLength(255)]
        [Column("password_hash")]
        public string PasswordHash { get; set; } = string.Empty;

        [Required]
        [Column("id_rol")]
        public int IdRol { get; set; }

        [ForeignKey("IdRol")]
        public Rol? Rol { get; set; }

        [StringLength(6)]
        [Column("codigo_verificacion")]
        public string? CodigoVerificacion { get; set; }

        [Column("expiracion_codigo")]
        public DateTime? ExpiracionCodigo { get; set; }

        [Required]
        [Column("cuenta_verificada")]
        public bool CuentaVerificada { get; set; } = false;

        [Required]
        [Column("intentos_fallidos")]
        public int IntentosFallidos { get; set; } = 0;

        [Column("fecha_bloqueo")]
        public DateTime? FechaBloqueo { get; set; }

        [Required]
        [Column("activo")]
        public bool Activo { get; set; } = true;

        [Required]
        [Column("fecha_registro")]
        public DateTime FechaRegistro { get; set; } = DateTime.Now;

        public Medico? Medico { get; set; }
        public Paciente? Paciente { get; set; }
    }
}