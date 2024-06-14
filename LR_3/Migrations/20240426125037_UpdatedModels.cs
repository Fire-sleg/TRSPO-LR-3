using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace LR_3.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedModels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: new Guid("4fe17d72-daa2-4c36-af2e-7a50ad234de2"));

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: new Guid("9e3235c7-93f3-4583-951b-9cad26626ac7"));

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: new Guid("c3f51667-10ca-4ea2-9c33-68b02199554f"));

            migrationBuilder.AddColumn<string>(
                name: "ProductIds",
                table: "Recipes",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Mass",
                table: "Products",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<int>(
                name: "NumberOfProducts",
                table: "Products",
                type: "int",
                nullable: false,
                defaultValue: 0);

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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

            migrationBuilder.DropColumn(
                name: "ProductIds",
                table: "Recipes");

            migrationBuilder.DropColumn(
                name: "Mass",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "NumberOfProducts",
                table: "Products");

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "CreatedDate", "Description", "Kcal", "Name", "UpdatedDate" },
                values: new object[,]
                {
                    { new Guid("4fe17d72-daa2-4c36-af2e-7a50ad234de2"), new DateTime(2024, 4, 24, 22, 33, 17, 471, DateTimeKind.Local).AddTicks(743), "Description", 25, "Цибуля", null },
                    { new Guid("9e3235c7-93f3-4583-951b-9cad26626ac7"), new DateTime(2024, 4, 24, 22, 33, 17, 471, DateTimeKind.Local).AddTicks(740), "Description", 15, "Картопля", null },
                    { new Guid("c3f51667-10ca-4ea2-9c33-68b02199554f"), new DateTime(2024, 4, 24, 22, 33, 17, 471, DateTimeKind.Local).AddTicks(656), "Description", 10, "Курячі яйця", null }
                });
        }
    }
}
