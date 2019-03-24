using Microsoft.EntityFrameworkCore.Migrations;

namespace AniCharades.API.Migrations
{
    public partial class CreateRelationStrategyCriteria : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RelationStrategyCriterias",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Keywords = table.Column<string>(nullable: true),
                    KeywordsMatch = table.Column<int>(nullable: false),
                    Relations = table.Column<string>(nullable: true),
                    Strategy = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RelationStrategyCriterias", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RelationStrategyCriterias");
        }
    }
}
