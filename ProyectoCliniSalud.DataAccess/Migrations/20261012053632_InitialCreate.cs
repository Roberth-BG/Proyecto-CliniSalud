using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProyectoCliniSalud.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Especialidades",
                columns: table => new
                {
                    id_especialidad = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    nombre_especialidad = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    descripcion = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    activo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Especialidades", x => x.id_especialidad);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    id_rol = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    nombre_rol = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    activo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.id_rol);
                });

            migrationBuilder.CreateTable(
                name: "Usuarios",
                columns: table => new
                {
                    id_usuario = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    password_hash = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    id_rol = table.Column<int>(type: "int", nullable: false),
                    codigo_verificacion = table.Column<string>(type: "nvarchar(6)", maxLength: 6, nullable: true),
                    expiracion_codigo = table.Column<DateTime>(type: "datetime2", nullable: true),
                    cuenta_verificada = table.Column<bool>(type: "bit", nullable: false),
                    intentos_fallidos = table.Column<int>(type: "int", nullable: false),
                    fecha_bloqueo = table.Column<DateTime>(type: "datetime2", nullable: true),
                    activo = table.Column<bool>(type: "bit", nullable: false),
                    fecha_registro = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuarios", x => x.id_usuario);
                    table.ForeignKey(
                        name: "FK_Usuarios_Roles_id_rol",
                        column: x => x.id_rol,
                        principalTable: "Roles",
                        principalColumn: "id_rol",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Medicos",
                columns: table => new
                {
                    id_medico = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    id_usuario = table.Column<int>(type: "int", nullable: false),
                    dni = table.Column<string>(type: "nvarchar(12)", maxLength: 12, nullable: false),
                    nombres = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    apellidos = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    cmp = table.Column<string>(type: "nvarchar(12)", maxLength: 12, nullable: false),
                    telefono = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: true),
                    consultorio = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    id_especialidad = table.Column<int>(type: "int", nullable: false),
                    foto_url = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Medicos", x => x.id_medico);
                    table.ForeignKey(
                        name: "FK_Medicos_Especialidades_id_especialidad",
                        column: x => x.id_especialidad,
                        principalTable: "Especialidades",
                        principalColumn: "id_especialidad",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Medicos_Usuarios_id_usuario",
                        column: x => x.id_usuario,
                        principalTable: "Usuarios",
                        principalColumn: "id_usuario",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Pacientes",
                columns: table => new
                {
                    id_paciente = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    id_usuario = table.Column<int>(type: "int", nullable: false),
                    dni = table.Column<string>(type: "nvarchar(12)", maxLength: 12, nullable: false),
                    nombres = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    apellidos = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    telefono = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    foto_url = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pacientes", x => x.id_paciente);
                    table.ForeignKey(
                        name: "FK_Pacientes_Usuarios_id_usuario",
                        column: x => x.id_usuario,
                        principalTable: "Usuarios",
                        principalColumn: "id_usuario",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Citas",
                columns: table => new
                {
                    id_cita = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    codigo_ticket = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    id_paciente = table.Column<int>(type: "int", nullable: false),
                    id_medico = table.Column<int>(type: "int", nullable: false),
                    fecha_cita = table.Column<DateTime>(type: "date", nullable: false),
                    hora_cita = table.Column<TimeSpan>(type: "time", nullable: false),
                    motivo_consulta = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    estado = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    fecha_reserva = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Citas", x => x.id_cita);
                    table.ForeignKey(
                        name: "FK_Citas_Medicos_id_medico",
                        column: x => x.id_medico,
                        principalTable: "Medicos",
                        principalColumn: "id_medico",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Citas_Pacientes_id_paciente",
                        column: x => x.id_paciente,
                        principalTable: "Pacientes",
                        principalColumn: "id_paciente",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Testimonios",
                columns: table => new
                {
                    id_testimonio = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    id_paciente = table.Column<int>(type: "int", nullable: false),
                    id_medico = table.Column<int>(type: "int", nullable: true),
                    calificacion = table.Column<int>(type: "int", nullable: false),
                    comentario = table.Column<string>(type: "nvarchar(400)", maxLength: 400, nullable: false),
                    estado = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    fecha = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Testimonios", x => x.id_testimonio);
                    table.ForeignKey(
                        name: "FK_Testimonios_Medicos_id_medico",
                        column: x => x.id_medico,
                        principalTable: "Medicos",
                        principalColumn: "id_medico",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Testimonios_Pacientes_id_paciente",
                        column: x => x.id_paciente,
                        principalTable: "Pacientes",
                        principalColumn: "id_paciente",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Atenciones",
                columns: table => new
                {
                    id_atencion = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    id_cita = table.Column<int>(type: "int", nullable: false),
                    diagnostico = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    tratamiento = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    fecha_atencion = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Atenciones", x => x.id_atencion);
                    table.ForeignKey(
                        name: "FK_Atenciones_Citas_id_cita",
                        column: x => x.id_cita,
                        principalTable: "Citas",
                        principalColumn: "id_cita",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Atenciones_id_cita",
                table: "Atenciones",
                column: "id_cita",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Citas_codigo_ticket",
                table: "Citas",
                column: "codigo_ticket",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Citas_id_medico",
                table: "Citas",
                column: "id_medico");

            migrationBuilder.CreateIndex(
                name: "IX_Citas_id_paciente",
                table: "Citas",
                column: "id_paciente");

            migrationBuilder.CreateIndex(
                name: "IX_Especialidades_nombre_especialidad",
                table: "Especialidades",
                column: "nombre_especialidad",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Medicos_cmp",
                table: "Medicos",
                column: "cmp",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Medicos_dni",
                table: "Medicos",
                column: "dni",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Medicos_id_especialidad",
                table: "Medicos",
                column: "id_especialidad");

            migrationBuilder.CreateIndex(
                name: "IX_Medicos_id_usuario",
                table: "Medicos",
                column: "id_usuario",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Pacientes_dni",
                table: "Pacientes",
                column: "dni",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Pacientes_id_usuario",
                table: "Pacientes",
                column: "id_usuario",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Roles_nombre_rol",
                table: "Roles",
                column: "nombre_rol",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Testimonios_id_medico",
                table: "Testimonios",
                column: "id_medico");

            migrationBuilder.CreateIndex(
                name: "IX_Testimonios_id_paciente",
                table: "Testimonios",
                column: "id_paciente");

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_email",
                table: "Usuarios",
                column: "email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_id_rol",
                table: "Usuarios",
                column: "id_rol");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Atenciones");

            migrationBuilder.DropTable(
                name: "Testimonios");

            migrationBuilder.DropTable(
                name: "Citas");

            migrationBuilder.DropTable(
                name: "Medicos");

            migrationBuilder.DropTable(
                name: "Pacientes");

            migrationBuilder.DropTable(
                name: "Especialidades");

            migrationBuilder.DropTable(
                name: "Usuarios");

            migrationBuilder.DropTable(
                name: "Roles");
        }
    }
}
