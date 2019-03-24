using Microsoft.EntityFrameworkCore.Migrations;

namespace AniCharades.API.Migrations
{
    public partial class AddTranslationsToSeries : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Translations",
                table: "Series",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Translations",
                table: "Series");
        }
    }
}
