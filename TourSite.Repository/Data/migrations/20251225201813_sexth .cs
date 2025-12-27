using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TourSite.Repository.Data.migrations
{
    /// <inheritdoc />
    public partial class sexth : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Slug",
                table: "Tours",
                type: "nvarchar(max)",
                nullable: true,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Slug",
                table: "Tours");
        }
    }
}
