using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace cisiro.Migrations
{
    public partial class lasrlas : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "coverLetter",
                table: "application");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "coverLetter",
                table: "application",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
