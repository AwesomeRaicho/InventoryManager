using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InventoryManager.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RefreshToken : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CurrentRefreshToken",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "RefreshTokenExpiresAt",
                table: "AspNetUsers",
                type: "datetime2",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("31d9ba1b-47f4-4a8a-98de-37ca4a1adec5"),
                columns: new[] { "ConcurrencyStamp", "CurrentRefreshToken", "PasswordHash", "RefreshTokenExpiresAt", "SecurityStamp" },
                values: new object[] { "4ae8a062-c884-4b5e-aba2-96039f5b988b", null, "AQAAAAIAAYagAAAAEHSiDzlBAbZ2yZMrqxAcSJM/QQnBHsl8V5eZDn5FIYOgrvsXXG0GDHPqUdT3Y2/qYQ==", null, "7ce525a6-616a-4e2f-b634-136e6cfaf67f" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CurrentRefreshToken",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "RefreshTokenExpiresAt",
                table: "AspNetUsers");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("31d9ba1b-47f4-4a8a-98de-37ca4a1adec5"),
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "50dfd704-c9df-440a-a9e9-cbc8e56f6bf2", "AQAAAAIAAYagAAAAEPAXkGj/Q4A4lvUaHi6W9v+ugxv6sL49zjuM5qdjEkEgrkfKbRsprK4hJhCKb+q8dg==", "deaa77d3-19cd-43cb-86e6-3f49b2cfe9c2" });
        }
    }
}
