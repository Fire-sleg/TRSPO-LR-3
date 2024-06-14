using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace LR_3.Migrations
{
    /// <inheritdoc />
    public partial class AddStartData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: new Guid("1e43ccf9-6994-4495-8c52-d853c508441a"));

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: new Guid("4e0f013a-b584-4d3a-b2f5-0ce2424d99f3"));

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: new Guid("af1a1aa5-14a1-4cc6-8e5f-9dcb9b404fa1"));

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "CreatedDate", "Description", "Kcal", "Mass", "Name", "NumberOfProducts", "UpdatedDate" },
                values: new object[,]
                {
                    { new Guid("65e22402-82db-496b-b7d6-400898bbd694"), new DateTime(2024, 4, 26, 16, 26, 3, 834, DateTimeKind.Local).AddTicks(725), "Description", 10, 0.0, "Курячі яйця", 0, null },
                    { new Guid("aee599b9-7f96-41fb-9d4e-af19acdfd94e"), new DateTime(2024, 4, 26, 16, 26, 3, 834, DateTimeKind.Local).AddTicks(778), "Description", 15, 0.0, "Картопля", 0, null },
                    { new Guid("afe7a16e-d41a-4326-8737-b842953d3cb8"), new DateTime(2024, 4, 26, 16, 26, 3, 834, DateTimeKind.Local).AddTicks(781), "Description", 25, 0.0, "Цибуля", 0, null }
                });

            migrationBuilder.InsertData(
                table: "Recipes",
                columns: new[] { "Id", "CreatedDate", "Description", "ImageURL", "Name", "ProductIds", "UpdatedDate" },
                values: new object[,]
                {
                    { new Guid("359eaff6-3a82-4486-8b7f-b99b8fe0afad"), null, "Description", null, "Картопля з цибулею", "[\"aee599b9-7f96-41fb-9d4e-af19acdfd94e\",\"afe7a16e-d41a-4326-8737-b842953d3cb8\"]", null },
                    { new Guid("e454a1c9-20ef-4604-9f92-97641d00495d"), null, "Description", null, "Картопля з яйцем та цибулею", "[\"65e22402-82db-496b-b7d6-400898bbd694\",\"aee599b9-7f96-41fb-9d4e-af19acdfd94e\",\"afe7a16e-d41a-4326-8737-b842953d3cb8\"]", null },
                    { new Guid("fb46e0f1-7382-4ddb-a562-8257ea99d387"), null, "Description", null, "Яєчня", "[\"65e22402-82db-496b-b7d6-400898bbd694\"]", null }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: new Guid("65e22402-82db-496b-b7d6-400898bbd694"));

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: new Guid("aee599b9-7f96-41fb-9d4e-af19acdfd94e"));

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: new Guid("afe7a16e-d41a-4326-8737-b842953d3cb8"));

            migrationBuilder.DeleteData(
                table: "Recipes",
                keyColumn: "Id",
                keyValue: new Guid("359eaff6-3a82-4486-8b7f-b99b8fe0afad"));

            migrationBuilder.DeleteData(
                table: "Recipes",
                keyColumn: "Id",
                keyValue: new Guid("e454a1c9-20ef-4604-9f92-97641d00495d"));

            migrationBuilder.DeleteData(
                table: "Recipes",
                keyColumn: "Id",
                keyValue: new Guid("fb46e0f1-7382-4ddb-a562-8257ea99d387"));

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "CreatedDate", "Description", "Kcal", "Mass", "Name", "NumberOfProducts", "UpdatedDate" },
                values: new object[,]
                {
                    { new Guid("1e43ccf9-6994-4495-8c52-d853c508441a"), new DateTime(2024, 4, 26, 15, 50, 37, 582, DateTimeKind.Local).AddTicks(1329), "Description", 15, 0.0, "Картопля", 0, null },
                    { new Guid("4e0f013a-b584-4d3a-b2f5-0ce2424d99f3"), new DateTime(2024, 4, 26, 15, 50, 37, 582, DateTimeKind.Local).AddTicks(1332), "Description", 25, 0.0, "Цибуля", 0, null },
                    { new Guid("af1a1aa5-14a1-4cc6-8e5f-9dcb9b404fa1"), new DateTime(2024, 4, 26, 15, 50, 37, 582, DateTimeKind.Local).AddTicks(1254), "Description", 10, 0.0, "Курячі яйця", 0, null }
                });
        }
    }
}
