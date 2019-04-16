using Microsoft.EntityFrameworkCore.Migrations;

namespace AniCharades.API.Migrations
{
    public partial class CreateTranslations : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RelationCriterias");

            migrationBuilder.CreateTable(
                name: "RelationCriterias",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Keywords = table.Column<string>(nullable: true),
                    KeywordsMatch = table.Column<int>(nullable: false),
                    Relations = table.Column<string>(nullable: true),
                    Strategies = table.Column<string>(nullable: true),
                    Types = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RelationCriterias", x => x.Id);
                });

            migrationBuilder.DropTable(
                name: "Series");

            migrationBuilder.CreateTable(
               name: "Series",
               columns: table => new
               {
                   Id = table.Column<int>(nullable: false)
                       .Annotation("Sqlite:Autoincrement", true),
                   Title = table.Column<string>(nullable: true),
                   ImageUrl = table.Column<string>(nullable: true),
                   TranslationId = table.Column<int>(nullable: true)
               },
               constraints: table =>
               {
                   table.PrimaryKey("PK_Masters", x => x.Id);
               });

            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "Mangas",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "Animes",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Translation",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Polish = table.Column<string>(nullable: true),
                    English = table.Column<string>(nullable: true),
                    Japanese = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Translation", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Series_TranslationId",
                table: "Series",
                column: "TranslationId");

            migrationBuilder.AddForeignKey(
                name: "FK_Series_Translation_TranslationId",
                table: "Series",
                column: "TranslationId",
                principalTable: "Translation",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Series_Translation_TranslationId",
                table: "Series");

            migrationBuilder.DropTable(
                name: "Translation");

            migrationBuilder.DropIndex(
                name: "IX_Series_TranslationId",
                table: "Series");

            migrationBuilder.DropColumn(
                name: "TranslationId",
                table: "Series");

            migrationBuilder.DropColumn(
                name: "Strategies",
                table: "RelationCriterias");

            migrationBuilder.DropColumn(
                name: "Types",
                table: "RelationCriterias");

            migrationBuilder.DropColumn(
                name: "Title",
                table: "Mangas");

            migrationBuilder.DropColumn(
                name: "Title",
                table: "Animes");

            migrationBuilder.AddColumn<string>(
                name: "Translations",
                table: "Series",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Strategy",
                table: "RelationCriterias",
                nullable: true);
        }
    }
}
