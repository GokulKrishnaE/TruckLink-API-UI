using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TruckLink.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class newColumnsAddedForJOb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ContactInfo",
                table: "Jobs",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Jobs",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ContactInfo",
                table: "Jobs");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Jobs");
        }
    }
}
