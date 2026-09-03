using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InscriptionEtudiant.Migrations
{
    /// <inheritdoc />
    public partial class CreateAdmission : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Admissions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DossierInscriptionId = table.Column<int>(type: "int", nullable: false),
                    ChoixFiliereId = table.Column<int>(type: "int", nullable: false),
                    AdministrateurId = table.Column<int>(type: "int", nullable: true),
                    Statut = table.Column<int>(type: "int", nullable: false),
                    Commentaire = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Score = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: true),
                    DateDecision = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DateCreation = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    DateModification = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Admissions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Admissions_Administrateurs_AdministrateurId",
                        column: x => x.AdministrateurId,
                        principalTable: "Administrateurs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Admissions_ChoixFilieres_ChoixFiliereId",
                        column: x => x.ChoixFiliereId,
                        principalTable: "ChoixFilieres",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Admissions_DossierInscriptions_DossierInscriptionId",
                        column: x => x.DossierInscriptionId,
                        principalTable: "DossierInscriptions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Admissions_AdministrateurId",
                table: "Admissions",
                column: "AdministrateurId");

            migrationBuilder.CreateIndex(
                name: "IX_Admissions_ChoixFiliereId",
                table: "Admissions",
                column: "ChoixFiliereId");

            migrationBuilder.CreateIndex(
                name: "IX_Admissions_DossierInscriptionId",
                table: "Admissions",
                column: "DossierInscriptionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Admissions");
        }
    }
}
