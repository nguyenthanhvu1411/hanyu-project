using System;
using HanYu.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace HanYu.Migrations;

[DbContext(typeof(HanYuDbContext))]
[Migration("20260819160000_AddSystemSettings")]
public partial class AddSystemSettings : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "system_settings",
            columns: table => new
            {
                id = table.Column<long>(type: "bigint", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                public_id = table.Column<Guid>(type: "uuid", nullable: false),
                key = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                display_name = table.Column<string>(type: "character varying(160)", maxLength: 160, nullable: false),
                group = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                value = table.Column<string>(type: "character varying(8000)", maxLength: 8000, nullable: false),
                value_type = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                created_at = table.Column<DateTimeOffset>(type: "timestamptz", nullable: false),
                updated_at = table.Column<DateTimeOffset>(type: "timestamptz", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_system_settings", x => x.id);
            });

        migrationBuilder.CreateIndex(
            name: "ix_system_settings_group",
            table: "system_settings",
            column: "group");

        migrationBuilder.CreateIndex(
            name: "ix_system_settings_key",
            table: "system_settings",
            column: "key",
            unique: true);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(name: "system_settings");
    }
}
