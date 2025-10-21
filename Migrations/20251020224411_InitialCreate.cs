using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace BackendAPI.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Estados",
                columns: table => new
                {
                    Nombre = table.Column<string>(type: "text", nullable: false),
                    Descripcion = table.Column<string>(type: "text", nullable: false),
                    Ambito = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Estados", x => x.Nombre);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    Nombre = table.Column<string>(type: "text", nullable: false),
                    Descripcion = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.Nombre);
                });

            migrationBuilder.CreateTable(
                name: "Sismografos",
                columns: table => new
                {
                    IdentificadorSismografo = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FechaAdquisicion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    NroSerie = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sismografos", x => x.IdentificadorSismografo);
                });

            migrationBuilder.CreateTable(
                name: "TiposMotivo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Descripcion = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TiposMotivo", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Empleados",
                columns: table => new
                {
                    Mail = table.Column<string>(type: "text", nullable: false),
                    Apellido = table.Column<string>(type: "text", nullable: false),
                    Nombre = table.Column<string>(type: "text", nullable: false),
                    Telefono = table.Column<string>(type: "text", nullable: false),
                    RolNombre = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Empleados", x => x.Mail);
                    table.ForeignKey(
                        name: "FK_Empleados_Roles_RolNombre",
                        column: x => x.RolNombre,
                        principalTable: "Roles",
                        principalColumn: "Nombre",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EstacionesSismologicas",
                columns: table => new
                {
                    CodigoEstacion = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DocumentoCertificacionAdq = table.Column<bool>(type: "boolean", nullable: false),
                    FechaSolicitudCertificacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Latitud = table.Column<double>(type: "double precision", nullable: false),
                    Nombre = table.Column<string>(type: "text", nullable: false),
                    NroCertificacionAdquirida = table.Column<int>(type: "integer", nullable: false),
                    SismografoIdentificadorSismografo = table.Column<int>(type: "integer", nullable: false),
                    EmpleadoMail = table.Column<string>(type: "text", nullable: true),
                    EstadoNombre = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EstacionesSismologicas", x => x.CodigoEstacion);
                    table.ForeignKey(
                        name: "FK_EstacionesSismologicas_Empleados_EmpleadoMail",
                        column: x => x.EmpleadoMail,
                        principalTable: "Empleados",
                        principalColumn: "Mail");
                    table.ForeignKey(
                        name: "FK_EstacionesSismologicas_Estados_EstadoNombre",
                        column: x => x.EstadoNombre,
                        principalTable: "Estados",
                        principalColumn: "Nombre",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EstacionesSismologicas_Sismografos_SismografoIdentificadorS~",
                        column: x => x.SismografoIdentificadorSismografo,
                        principalTable: "Sismografos",
                        principalColumn: "IdentificadorSismografo",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Usuarios",
                columns: table => new
                {
                    NombreUsuario = table.Column<string>(type: "text", nullable: false),
                    Contraseña = table.Column<string>(type: "text", nullable: false),
                    EmpleadoMail = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuarios", x => x.NombreUsuario);
                    table.ForeignKey(
                        name: "FK_Usuarios_Empleados_EmpleadoMail",
                        column: x => x.EmpleadoMail,
                        principalTable: "Empleados",
                        principalColumn: "Mail");
                });

            migrationBuilder.CreateTable(
                name: "OrdenesDeInspeccion",
                columns: table => new
                {
                    NumeroOrden = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FechaHoraCierre = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    FechaHoraFinalizacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    FechaHoraInicio = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ObservacionCierre = table.Column<string>(type: "text", nullable: false),
                    EmpleadoMail = table.Column<string>(type: "text", nullable: true),
                    EstadoNombre = table.Column<string>(type: "text", nullable: true),
                    EstacionSismologicaCodigoEstacion = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrdenesDeInspeccion", x => x.NumeroOrden);
                    table.ForeignKey(
                        name: "FK_OrdenesDeInspeccion_Empleados_EmpleadoMail",
                        column: x => x.EmpleadoMail,
                        principalTable: "Empleados",
                        principalColumn: "Mail");
                    table.ForeignKey(
                        name: "FK_OrdenesDeInspeccion_EstacionesSismologicas_EstacionSismolog~",
                        column: x => x.EstacionSismologicaCodigoEstacion,
                        principalTable: "EstacionesSismologicas",
                        principalColumn: "CodigoEstacion",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrdenesDeInspeccion_Estados_EstadoNombre",
                        column: x => x.EstadoNombre,
                        principalTable: "Estados",
                        principalColumn: "Nombre");
                });

            migrationBuilder.CreateTable(
                name: "Sesiones",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FechaHoraInicio = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    FechaHoraFin = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UsuarioNombreUsuario = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sesiones", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Sesiones_Usuarios_UsuarioNombreUsuario",
                        column: x => x.UsuarioNombreUsuario,
                        principalTable: "Usuarios",
                        principalColumn: "NombreUsuario",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CambiosEstado",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    EstadoNombre = table.Column<string>(type: "text", nullable: false),
                    FechaHoraFin = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    FechaHoraInicio = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    OrdenDeInspeccionNumeroOrden = table.Column<int>(type: "integer", nullable: true),
                    SismografoIdentificadorSismografo = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CambiosEstado", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CambiosEstado_Estados_EstadoNombre",
                        column: x => x.EstadoNombre,
                        principalTable: "Estados",
                        principalColumn: "Nombre",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CambiosEstado_OrdenesDeInspeccion_OrdenDeInspeccionNumeroOr~",
                        column: x => x.OrdenDeInspeccionNumeroOrden,
                        principalTable: "OrdenesDeInspeccion",
                        principalColumn: "NumeroOrden");
                    table.ForeignKey(
                        name: "FK_CambiosEstado_Sismografos_SismografoIdentificadorSismografo",
                        column: x => x.SismografoIdentificadorSismografo,
                        principalTable: "Sismografos",
                        principalColumn: "IdentificadorSismografo");
                });

            migrationBuilder.CreateTable(
                name: "MotivosFueraDeServicio",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TipoMotivoId = table.Column<int>(type: "integer", nullable: false),
                    Comentario = table.Column<string>(type: "text", nullable: true),
                    CambioEstadoId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MotivosFueraDeServicio", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MotivosFueraDeServicio_CambiosEstado_CambioEstadoId",
                        column: x => x.CambioEstadoId,
                        principalTable: "CambiosEstado",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_MotivosFueraDeServicio_TiposMotivo_TipoMotivoId",
                        column: x => x.TipoMotivoId,
                        principalTable: "TiposMotivo",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CambiosEstado_EstadoNombre",
                table: "CambiosEstado",
                column: "EstadoNombre");

            migrationBuilder.CreateIndex(
                name: "IX_CambiosEstado_OrdenDeInspeccionNumeroOrden",
                table: "CambiosEstado",
                column: "OrdenDeInspeccionNumeroOrden");

            migrationBuilder.CreateIndex(
                name: "IX_CambiosEstado_SismografoIdentificadorSismografo",
                table: "CambiosEstado",
                column: "SismografoIdentificadorSismografo");

            migrationBuilder.CreateIndex(
                name: "IX_Empleados_RolNombre",
                table: "Empleados",
                column: "RolNombre");

            migrationBuilder.CreateIndex(
                name: "IX_EstacionesSismologicas_EmpleadoMail",
                table: "EstacionesSismologicas",
                column: "EmpleadoMail");

            migrationBuilder.CreateIndex(
                name: "IX_EstacionesSismologicas_EstadoNombre",
                table: "EstacionesSismologicas",
                column: "EstadoNombre");

            migrationBuilder.CreateIndex(
                name: "IX_EstacionesSismologicas_SismografoIdentificadorSismografo",
                table: "EstacionesSismologicas",
                column: "SismografoIdentificadorSismografo");

            migrationBuilder.CreateIndex(
                name: "IX_MotivosFueraDeServicio_CambioEstadoId",
                table: "MotivosFueraDeServicio",
                column: "CambioEstadoId");

            migrationBuilder.CreateIndex(
                name: "IX_MotivosFueraDeServicio_TipoMotivoId",
                table: "MotivosFueraDeServicio",
                column: "TipoMotivoId");

            migrationBuilder.CreateIndex(
                name: "IX_OrdenesDeInspeccion_EmpleadoMail",
                table: "OrdenesDeInspeccion",
                column: "EmpleadoMail");

            migrationBuilder.CreateIndex(
                name: "IX_OrdenesDeInspeccion_EstacionSismologicaCodigoEstacion",
                table: "OrdenesDeInspeccion",
                column: "EstacionSismologicaCodigoEstacion");

            migrationBuilder.CreateIndex(
                name: "IX_OrdenesDeInspeccion_EstadoNombre",
                table: "OrdenesDeInspeccion",
                column: "EstadoNombre");

            migrationBuilder.CreateIndex(
                name: "IX_Sesiones_UsuarioNombreUsuario",
                table: "Sesiones",
                column: "UsuarioNombreUsuario");

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_EmpleadoMail",
                table: "Usuarios",
                column: "EmpleadoMail");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MotivosFueraDeServicio");

            migrationBuilder.DropTable(
                name: "Sesiones");

            migrationBuilder.DropTable(
                name: "CambiosEstado");

            migrationBuilder.DropTable(
                name: "TiposMotivo");

            migrationBuilder.DropTable(
                name: "Usuarios");

            migrationBuilder.DropTable(
                name: "OrdenesDeInspeccion");

            migrationBuilder.DropTable(
                name: "EstacionesSismologicas");

            migrationBuilder.DropTable(
                name: "Empleados");

            migrationBuilder.DropTable(
                name: "Estados");

            migrationBuilder.DropTable(
                name: "Sismografos");

            migrationBuilder.DropTable(
                name: "Roles");
        }
    }
}
