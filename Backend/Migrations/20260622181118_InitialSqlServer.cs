using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DOTNET.Migrations
{
    /// <inheritdoc />
    public partial class InitialSqlServer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Stock = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CategoryId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Products_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "Description", "Name" },
                values: new object[,]
                {
                    { 1, "Dispositivos electrónicos y gadgets", "Electrónica" },
                    { 2, "Prendas de vestir y accesorios", "Ropa" },
                    { 3, "Productos alimenticios y abarrotes", "Alimentos" }
                });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "CategoryId", "CreatedAt", "Description", "IsActive", "Name", "Price", "Stock" },
                values: new object[,]
                {
                    { 1, 1, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, true, "Laptop HP Pavillion", 899.99m, 10 },
                    { 2, 2, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, true, "Polo Nike Park", 35.00m, 50 },
                    { 3, 3, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, true, "Arroz Extra 1kg", 3.50m, 200 },
                    { 4, 1, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, true, "Audífonos Sony WH-CH520", 45.00m, 25 },
                    { 5, 1, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, true, "Mouse Inalámbrico Logitech", 19.99m, 40 },
                    { 6, 1, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, true, "Monitor Gamer 24'' ASUS", 179.50m, 8 },
                    { 7, 2, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, true, "Casaca Impermeable Columbia", 120.00m, 15 },
                    { 8, 2, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, true, "Medias Deportivas Adidas (3 pack)", 12.50m, 100 },
                    { 9, 3, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, true, "Aceite de Oliva Virgen 500ml", 14.20m, 60 },
                    { 10, 3, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, true, "Café Altomayo Gourmet 250g", 8.50m, 85 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Products_CategoryId",
                table: "Products",
                column: "CategoryId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Products");

            migrationBuilder.DropTable(
                name: "Categories");
        }
    }
}
