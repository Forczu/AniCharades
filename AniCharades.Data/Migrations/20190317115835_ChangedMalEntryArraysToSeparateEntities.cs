using Microsoft.EntityFrameworkCore.Migrations;

namespace AniCharades.API.Migrations
{
    public partial class ChangedMalEntryArraysToSeparateEntities : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
               name: "NEW_Series",
               columns: table => new
               {
                   Id = table.Column<int>(nullable: false)
                       .Annotation("Sqlite:Autoincrement", true),
                   Title = table.Column<string>(nullable: true),
                   ImageUrl = table.Column<string>(nullable: true),
                   Translations = table.Column<string>(nullable: true)
               },
               constraints: table =>
               {
                   table.PrimaryKey("PK_Masters", x => x.Id);
               });
            migrationBuilder.Sql("INSERT INTO NEW_Series(Id, Title, ImageUrl, Translations) SELECT Id, Title, ImageUrl, Translations FROM Series;");
            migrationBuilder.Sql("PRAGMA foreign_keys=\"0\"", true);
            migrationBuilder.Sql("DROP TABLE Series", true);
            migrationBuilder.Sql("ALTER TABLE NEW_Series RENAME TO Series", true);
            migrationBuilder.Sql("PRAGMA foreign_keys=\"1\"", true);

            migrationBuilder.CreateTable(
                name: "Animes",
                columns: table => new
                {
                    MalId = table.Column<long>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    SeriesId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Animes", x => x.MalId);
                    table.ForeignKey(
                        name: "FK_Animes_Series_SeriesId",
                        column: x => x.SeriesId,
                        principalTable: "Series",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Mangas",
                columns: table => new
                {
                    MalId = table.Column<long>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    SeriesId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Mangas", x => x.MalId);
                    table.ForeignKey(
                        name: "FK_Mangas_Series_SeriesId",
                        column: x => x.SeriesId,
                        principalTable: "Series",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Animes_SeriesId",
                table: "Animes",
                column: "SeriesId");

            migrationBuilder.CreateIndex(
                name: "IX_Mangas_SeriesId",
                table: "Mangas",
                column: "SeriesId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Animes");

            migrationBuilder.DropTable(
                name: "Mangas");

            migrationBuilder.AddColumn<string>(
                name: "AnimePositions",
                table: "Series",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MangaPositions",
                table: "Series",
                nullable: true);
        }
    }
}
