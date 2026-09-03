using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InscriptionEtudiant.Migrations
{
    /// <inheritdoc />
    public partial class AllDataBase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DossierInscriptions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DateDepot = table.Column<DateTime>(type: "datetime2", nullable: false),
                    StatutActuel = table.Column<int>(type: "int", nullable: false),
                    CandidatId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DossierInscriptions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DossierInscriptions_Candidats_CandidatId",
                        column: x => x.CandidatId,
                        principalTable: "Candidats",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Mentions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nom = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Mentions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SerieBacs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nom = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SerieBacs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ChoixFilieres",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrdreChoix = table.Column<int>(type: "int", nullable: false),
                    FiliereId = table.Column<int>(type: "int", nullable: false),
                    DossierInscriptionId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChoixFilieres", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChoixFilieres_DossierInscriptions_DossierInscriptionId",
                        column: x => x.DossierInscriptionId,
                        principalTable: "DossierInscriptions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ChoixFilieres_Filieres_FiliereId",
                        column: x => x.FiliereId,
                        principalTable: "Filieres",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Documents",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TypeDocument = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    NomFichier = table.Column<string>(type: "nvarchar(260)", maxLength: 260, nullable: false),
                    Chemin = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    DateDepot = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DossierInscriptionId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Documents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Documents_DossierInscriptions_DossierInscriptionId",
                        column: x => x.DossierInscriptionId,
                        principalTable: "DossierInscriptions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HistoriqueStatuts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AncienStatut = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    NouveauStatut = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Commentaire = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    DateChangement = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AdministrateurId = table.Column<int>(type: "int", nullable: true),
                    DossierInscriptionId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HistoriqueStatuts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HistoriqueStatuts_Administrateurs_AdministrateurId",
                        column: x => x.AdministrateurId,
                        principalTable: "Administrateurs",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_HistoriqueStatuts_DossierInscriptions_DossierInscriptionId",
                        column: x => x.DossierInscriptionId,
                        principalTable: "DossierInscriptions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ParcoursAcademiques",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AnneeBac = table.Column<int>(type: "int", nullable: false),
                    NoteNationale = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    NoteRegionale = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    SerieBacId = table.Column<int>(type: "int", nullable: false),
                    MentionId = table.Column<int>(type: "int", nullable: false),
                    DossierInscriptionId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ParcoursAcademiques", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ParcoursAcademiques_DossierInscriptions_DossierInscriptionId",
                        column: x => x.DossierInscriptionId,
                        principalTable: "DossierInscriptions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ParcoursAcademiques_Mentions_MentionId",
                        column: x => x.MentionId,
                        principalTable: "Mentions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ParcoursAcademiques_SerieBacs_SerieBacId",
                        column: x => x.SerieBacId,
                        principalTable: "SerieBacs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ChoixFilieres_DossierInscriptionId",
                table: "ChoixFilieres",
                column: "DossierInscriptionId");

            migrationBuilder.CreateIndex(
                name: "IX_ChoixFilieres_FiliereId",
                table: "ChoixFilieres",
                column: "FiliereId");

            migrationBuilder.CreateIndex(
                name: "IX_Documents_DossierInscriptionId",
                table: "Documents",
                column: "DossierInscriptionId");

            migrationBuilder.CreateIndex(
                name: "IX_DossierInscriptions_CandidatId",
                table: "DossierInscriptions",
                column: "CandidatId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_HistoriqueStatuts_AdministrateurId",
                table: "HistoriqueStatuts",
                column: "AdministrateurId");

            migrationBuilder.CreateIndex(
                name: "IX_HistoriqueStatuts_DossierInscriptionId",
                table: "HistoriqueStatuts",
                column: "DossierInscriptionId");

            migrationBuilder.CreateIndex(
                name: "IX_ParcoursAcademiques_DossierInscriptionId",
                table: "ParcoursAcademiques",
                column: "DossierInscriptionId",
                unique: true,
                filter: "[DossierInscriptionId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_ParcoursAcademiques_MentionId",
                table: "ParcoursAcademiques",
                column: "MentionId");

            migrationBuilder.CreateIndex(
                name: "IX_ParcoursAcademiques_SerieBacId",
                table: "ParcoursAcademiques",
                column: "SerieBacId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChoixFilieres");

            migrationBuilder.DropTable(
                name: "Documents");

            migrationBuilder.DropTable(
                name: "HistoriqueStatuts");

            migrationBuilder.DropTable(
                name: "ParcoursAcademiques");

            migrationBuilder.DropTable(
                name: "DossierInscriptions");

            migrationBuilder.DropTable(
                name: "Mentions");

            migrationBuilder.DropTable(
                name: "SerieBacs");
        }
    }
}
