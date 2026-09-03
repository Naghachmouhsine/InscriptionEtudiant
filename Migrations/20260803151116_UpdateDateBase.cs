using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InscriptionEtudiant.Migrations
{
    /// <inheritdoc />
    public partial class UpdateDateBase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "FiliereAffecteeId",
                table: "DossierInscriptions",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_DossierInscriptions_FiliereAffecteeId",
                table: "DossierInscriptions",
                column: "FiliereAffecteeId");

            migrationBuilder.AddForeignKey(
                name: "FK_DossierInscriptions_Filieres_FiliereAffecteeId",
                table: "DossierInscriptions",
                column: "FiliereAffecteeId",
                principalTable: "Filieres",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DossierInscriptions_Filieres_FiliereAffecteeId",
                table: "DossierInscriptions");

            migrationBuilder.DropIndex(
                name: "IX_DossierInscriptions_FiliereAffecteeId",
                table: "DossierInscriptions");

            migrationBuilder.DropColumn(
                name: "FiliereAffecteeId",
                table: "DossierInscriptions");
        }
    }
}
