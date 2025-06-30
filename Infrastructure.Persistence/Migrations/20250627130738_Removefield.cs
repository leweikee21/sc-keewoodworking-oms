using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Removefield : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExpiredDate",
                table: "Inventories");

            migrationBuilder.DropColumn(
                name: "Height",
                table: "Inventories");

            migrationBuilder.DropColumn(
                name: "Length",
                table: "Inventories");

            migrationBuilder.DropColumn(
                name: "Width",
                table: "Inventories");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ExpiredDate",
                table: "Inventories",
                type: "timestamp",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Height",
                table: "Inventories",
                type: "numeric(18,6)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Length",
                table: "Inventories",
                type: "numeric(18,6)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Width",
                table: "Inventories",
                type: "numeric(18,6)",
                nullable: true);
        }
    }
}
