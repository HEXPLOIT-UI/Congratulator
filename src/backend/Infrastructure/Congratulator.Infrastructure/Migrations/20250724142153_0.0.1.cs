using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Congratulator.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class _001 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    entity_id = table.Column<Guid>(type: "uuid", nullable: false),
                    first_name = table.Column<string>(type: "text", nullable: false),
                    last_name = table.Column<string>(type: "text", nullable: false),
                    login = table.Column<string>(type: "text", nullable: false),
                    role = table.Column<string>(type: "text", nullable: false, defaultValue: "User"),
                    password_hash = table.Column<string>(type: "text", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users", x => x.entity_id);
                });

            migrationBuilder.CreateTable(
                name: "birthdays",
                columns: table => new
                {
                    entity_id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    first_name = table.Column<string>(type: "text", nullable: false),
                    last_name = table.Column<string>(type: "text", nullable: false),
                    date_of_birth = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    photo_path = table.Column<string>(type: "text", nullable: true),
                    is_active = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_birthdays", x => x.entity_id);
                    table.ForeignKey(
                        name: "fk_birthdays_user",
                        column: x => x.UserId,
                        principalTable: "users",
                        principalColumn: "entity_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "users",
                columns: new[] { "entity_id", "first_name", "last_name", "login", "password_hash", "role" },
                values: new object[] { new Guid("bc3b8c2d-0368-4dbc-a7b2-c03c020ddcbd"), "Admin", "Admin", "admin", "$2a$11$oIzawOjBdsMLJV3b0p82yOtCGDNZeRpF687i3TWDqmkLb0MrqRCY.", "Admin" });

            migrationBuilder.CreateIndex(
                name: "IX_birthdays_UserId",
                table: "birthdays",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_users_login",
                table: "users",
                column: "login",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "birthdays");

            migrationBuilder.DropTable(
                name: "users");
        }
    }
}
