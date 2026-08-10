using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HanYu.Migrations
{
    /// <inheritdoc />
    public partial class AddIdentityPermissions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "description",
                table: "roles",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "code",
                table: "roles",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "display_name",
                table: "roles",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "ix_roles_code",
                table: "roles",
                column: "code",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_roles_code",
                table: "roles");

            migrationBuilder.DropColumn(
                name: "code",
                table: "roles");

            migrationBuilder.DropColumn(
                name: "display_name",
                table: "roles");

            migrationBuilder.AlterColumn<string>(
                name: "description",
                table: "roles",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(1000)",
                oldMaxLength: 1000,
                oldNullable: true);
        }
    }
}
