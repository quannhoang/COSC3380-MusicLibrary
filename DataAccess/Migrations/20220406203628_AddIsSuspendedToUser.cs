using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MusicLibrary.DataAccess.Migrations
{
    public partial class AddIsSuspendedToUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsSuspended",
                table: "User",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsSuspended",
                table: "User");
        }
    }
}
