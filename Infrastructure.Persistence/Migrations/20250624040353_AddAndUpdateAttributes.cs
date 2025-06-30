using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddAndUpdateAttributes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "InboundId",
                table: "Outbounds",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalPrice",
                table: "Outbounds",
                type: "numeric(18,6)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "Orders",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddColumn<string>(
                name: "filePath",
                table: "Orders",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Inventories",
                type: "bool",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "ReceivedDate",
                table: "Acquisitions",
                type: "timestamp",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "filePath",
                table: "Acquisitions",
                type: "text",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Outbounds_InboundId",
                table: "Outbounds",
                column: "InboundId");

            migrationBuilder.AddForeignKey(
                name: "FK_Outbounds_Inbounds_InboundId",
                table: "Outbounds",
                column: "InboundId",
                principalTable: "Inbounds",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Outbounds_Inbounds_InboundId",
                table: "Outbounds");

            migrationBuilder.DropIndex(
                name: "IX_Outbounds_InboundId",
                table: "Outbounds");

            migrationBuilder.DropColumn(
                name: "InboundId",
                table: "Outbounds");

            migrationBuilder.DropColumn(
                name: "TotalPrice",
                table: "Outbounds");

            migrationBuilder.DropColumn(
                name: "filePath",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Inventories");

            migrationBuilder.DropColumn(
                name: "ReceivedDate",
                table: "Acquisitions");

            migrationBuilder.DropColumn(
                name: "filePath",
                table: "Acquisitions");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "Orders",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);
        }
    }
}
