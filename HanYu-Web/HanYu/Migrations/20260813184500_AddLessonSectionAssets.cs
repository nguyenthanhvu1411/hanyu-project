using HanYu.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace HanYu.Migrations;

[DbContext(typeof(HanYuDbContext))]
[Migration("20260813184500_AddLessonSectionAssets")]
public partial class AddLessonSectionAssets : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "lesson_section_assets",
            columns: table => new
            {
                id = table.Column<long>(type: "bigint", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                public_id = table.Column<Guid>(type: "uuid", nullable: false),
                lesson_section_id = table.Column<long>(type: "bigint", nullable: false),
                lesson_asset_id = table.Column<long>(type: "bigint", nullable: false),
                sort_order = table.Column<int>(type: "integer", nullable: false),
                caption_vi = table.Column<string>(type: "text", nullable: true),
                is_required = table.Column<bool>(type: "boolean", nullable: false),
                created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                created_by_id = table.Column<Guid>(type: "uuid", nullable: true),
                updated_by_id = table.Column<Guid>(type: "uuid", nullable: true),
                deleted_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                deleted_by_id = table.Column<Guid>(type: "uuid", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_lesson_section_assets", x => x.id);
                table.ForeignKey(
                    name: "fk_lesson_section_assets_lesson_assets_lesson_asset_id",
                    column: x => x.lesson_asset_id,
                    principalTable: "lesson_assets",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "fk_lesson_section_assets_lesson_sections_lesson_section_id",
                    column: x => x.lesson_section_id,
                    principalTable: "lesson_sections",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "ix_lesson_section_assets_lesson_asset_id",
            table: "lesson_section_assets",
            column: "lesson_asset_id");

        migrationBuilder.CreateIndex(
            name: "ix_lesson_section_assets_lesson_section_id_lesson_asset_id",
            table: "lesson_section_assets",
            columns: new[] { "lesson_section_id", "lesson_asset_id" },
            unique: true);

        migrationBuilder.CreateIndex(
            name: "ix_lesson_section_assets_lesson_section_id_sort_order",
            table: "lesson_section_assets",
            columns: new[] { "lesson_section_id", "sort_order" });
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(name: "lesson_section_assets");
    }
}
