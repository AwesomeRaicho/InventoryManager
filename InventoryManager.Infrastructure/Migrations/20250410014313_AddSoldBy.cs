using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InventoryManager.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddSoldBy : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SoldBy",
                table: "ProductInstances",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SoldBy",
                table: "ProductInstances");
        }
    }
}
