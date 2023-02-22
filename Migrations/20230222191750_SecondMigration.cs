using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JumpStarter.Migrations
{
    public partial class SecondMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Donations",
                table: "UsersAndProjects",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Donations",
                table: "UsersAndProjects");
        }
    }
}
