using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TourSite.Repository.Data.migrations
{
    /// <inheritdoc />
    public partial class forth : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "HotelName",
                table: "Emails",
                type: "nvarchar(max)",
                nullable: true,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "RoomNumber",
                table: "Emails",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HotelName",
                table: "Emails");

            migrationBuilder.DropColumn(
                name: "RoomNumber",
                table: "Emails");
        }
    }
}
