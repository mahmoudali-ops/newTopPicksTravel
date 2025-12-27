using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TourSite.Repository.Data.migrations
{
    /// <inheritdoc />
    public partial class seveth : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Slug",
                table: "Transfers",
                type: "nvarchar(450)",
                nullable: true,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Slug",
                table: "CategoryTours",
                type: "nvarchar(450)",
                nullable: true,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Transfers_Slug",
                table: "Transfers",
                column: "Slug",
                unique: true,
                filter: "[Slug] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_CategoryTours_Slug",
                table: "CategoryTours",
                column: "Slug",
                unique: true,
                filter: "[Slug] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Transfers_Slug",
                table: "Transfers");

            migrationBuilder.DropIndex(
                name: "IX_CategoryTours_Slug",
                table: "CategoryTours");

            migrationBuilder.DropColumn(
                name: "Slug",
                table: "Transfers");

            migrationBuilder.DropColumn(
                name: "Slug",
                table: "CategoryTours");
        }
    }
}
