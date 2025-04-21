using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace InventoryManager.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { new Guid("9535c06c-27d7-42b0-8b10-e0202e6bf6b6"), null, "Employee", "EMPLOYEE" },
                    { new Guid("f2b1b83f-d0a8-4916-94ad-fde172bf1923"), null, "Administrator", "ADMINISTRATOR" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Email", "EmailConfirmed", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { new Guid("31d9ba1b-47f4-4a8a-98de-37ca4a1adec5"), 0, "50dfd704-c9df-440a-a9e9-cbc8e56f6bf2", null, false, false, null, null, "ADMIN", "AQAAAAIAAYagAAAAEPAXkGj/Q4A4lvUaHi6W9v+ugxv6sL49zjuM5qdjEkEgrkfKbRsprK4hJhCKb+q8dg==", null, false, "deaa77d3-19cd-43cb-86e6-3f49b2cfe9c2", false, "Admin" });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { new Guid("f2b1b83f-d0a8-4916-94ad-fde172bf1923"), new Guid("31d9ba1b-47f4-4a8a-98de-37ca4a1adec5") });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("9535c06c-27d7-42b0-8b10-e0202e6bf6b6"));

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { new Guid("f2b1b83f-d0a8-4916-94ad-fde172bf1923"), new Guid("31d9ba1b-47f4-4a8a-98de-37ca4a1adec5") });

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("f2b1b83f-d0a8-4916-94ad-fde172bf1923"));

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("31d9ba1b-47f4-4a8a-98de-37ca4a1adec5"));
        }
    }
}
