using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Congratulator.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class _002 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TelegramId",
                table: "users",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "birthdays",
                type: "text",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "users",
                keyColumn: "entity_id",
                keyValue: new Guid("bc3b8c2d-0368-4dbc-a7b2-c03c020ddcbd"),
                column: "TelegramId",
                value: null);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TelegramId",
                table: "users");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "birthdays");
        }
    }
}
