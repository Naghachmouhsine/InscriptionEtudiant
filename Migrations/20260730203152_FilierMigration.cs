using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InscriptionEtudiant.Migrations
{
    /// <inheritdoc />
    public partial class FilierMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Filieres",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nom = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Capacite = table.Column<int>(type: "int", nullable: false),
                    DateDebutInscription = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateFinInscription = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Filieres", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Filieres");
        }
    }
}
