using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BuyoraDataAccess.Migrations
{
    /// <inheritdoc />
    public partial class add_DisplayOrderColomn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DisplayOrder",
                table: "Category",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DisplayOrder",
                table: "Category");
        }
    }
}
