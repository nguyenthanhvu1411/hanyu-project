using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace HanYu.Migrations
{
    /// <inheritdoc />
    public partial class AddCourseModule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "courses",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    slug = table.Column<string>(type: "character varying(160)", maxLength: 160, nullable: false),
                    title_vi = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    short_description_vi = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    description_vi = table.Column<string>(type: "text", nullable: true),
                    hsk_level_id = table.Column<long>(type: "bigint", nullable: true),
                    cover_image_url = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: true),
                    sort_order = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    estimated_minutes = table.Column<int>(type: "integer", nullable: true),
                    status = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    is_featured = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    published_at = table.Column<DateTimeOffset>(type: "timestamptz", nullable: true),
                    published_by_id = table.Column<Guid>(type: "uuid", nullable: true),
                    archived_at = table.Column<DateTimeOffset>(type: "timestamptz", nullable: true),
                    archived_by_id = table.Column<Guid>(type: "uuid", nullable: true),
                    concurrency_token = table.Column<Guid>(type: "uuid", nullable: false),
                    public_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamptz", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamptz", nullable: false),
                    created_by_id = table.Column<Guid>(type: "uuid", nullable: true),
                    updated_by_id = table.Column<Guid>(type: "uuid", nullable: true),
                    deleted_at = table.Column<DateTimeOffset>(type: "timestamptz", nullable: true),
                    deleted_by_id = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_courses", x => x.id);
                    table.CheckConstraint("ck_courses_estimated_minutes", "estimated_minutes IS NULL OR estimated_minutes > 0");
                    table.CheckConstraint("ck_courses_sort_order", "sort_order >= 0");
                    table.ForeignKey(
                        name: "fk_courses_hsk_levels_hsk_level_id",
                        column: x => x.hsk_level_id,
                        principalTable: "hsk_levels",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "course_chapters",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    course_id = table.Column<long>(type: "bigint", nullable: false),
                    title_vi = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    description_vi = table.Column<string>(type: "text", nullable: true),
                    sort_order = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    is_active = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    public_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamptz", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamptz", nullable: false),
                    created_by_id = table.Column<Guid>(type: "uuid", nullable: true),
                    updated_by_id = table.Column<Guid>(type: "uuid", nullable: true),
                    deleted_at = table.Column<DateTimeOffset>(type: "timestamptz", nullable: true),
                    deleted_by_id = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_course_chapters", x => x.id);
                    table.CheckConstraint("ck_course_chapters_sort_order", "sort_order >= 0");
                    table.ForeignKey(
                        name: "fk_course_chapters_courses_course_id",
                        column: x => x.course_id,
                        principalTable: "courses",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "course_prerequisites",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    course_id = table.Column<long>(type: "bigint", nullable: false),
                    required_course_id = table.Column<long>(type: "bigint", nullable: false),
                    is_required = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    sort_order = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    public_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamptz", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamptz", nullable: false),
                    created_by_id = table.Column<Guid>(type: "uuid", nullable: true),
                    updated_by_id = table.Column<Guid>(type: "uuid", nullable: true),
                    deleted_at = table.Column<DateTimeOffset>(type: "timestamptz", nullable: true),
                    deleted_by_id = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_course_prerequisites", x => x.id);
                    table.CheckConstraint("ck_course_prerequisites_not_self", "course_id <> required_course_id");
                    table.CheckConstraint("ck_course_prerequisites_sort_order", "sort_order >= 0");
                    table.ForeignKey(
                        name: "fk_course_prerequisites_courses_course_id",
                        column: x => x.course_id,
                        principalTable: "courses",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_course_prerequisites_courses_required_course_id",
                        column: x => x.required_course_id,
                        principalTable: "courses",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "ix_course_chapters_course_id",
                table: "course_chapters",
                column: "course_id");

            migrationBuilder.CreateIndex(
                name: "ix_course_chapters_course_id_sort_order",
                table: "course_chapters",
                columns: new[] { "course_id", "sort_order" },
                unique: true,
                filter: "deleted_at IS NULL");

            migrationBuilder.CreateIndex(
                name: "ix_course_chapters_deleted_at",
                table: "course_chapters",
                column: "deleted_at");

            migrationBuilder.CreateIndex(
                name: "ix_course_chapters_is_active",
                table: "course_chapters",
                column: "is_active");

            migrationBuilder.CreateIndex(
                name: "ix_course_chapters_public_id",
                table: "course_chapters",
                column: "public_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_course_prerequisites_course_id",
                table: "course_prerequisites",
                column: "course_id");

            migrationBuilder.CreateIndex(
                name: "ix_course_prerequisites_course_id_required_course_id",
                table: "course_prerequisites",
                columns: new[] { "course_id", "required_course_id" },
                unique: true,
                filter: "deleted_at IS NULL");

            migrationBuilder.CreateIndex(
                name: "ix_course_prerequisites_course_id_sort_order",
                table: "course_prerequisites",
                columns: new[] { "course_id", "sort_order" });

            migrationBuilder.CreateIndex(
                name: "ix_course_prerequisites_deleted_at",
                table: "course_prerequisites",
                column: "deleted_at");

            migrationBuilder.CreateIndex(
                name: "ix_course_prerequisites_public_id",
                table: "course_prerequisites",
                column: "public_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_course_prerequisites_required_course_id",
                table: "course_prerequisites",
                column: "required_course_id");

            migrationBuilder.CreateIndex(
                name: "ix_courses_code",
                table: "courses",
                column: "code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_courses_deleted_at",
                table: "courses",
                column: "deleted_at");

            migrationBuilder.CreateIndex(
                name: "ix_courses_hsk_level_id",
                table: "courses",
                column: "hsk_level_id");

            migrationBuilder.CreateIndex(
                name: "ix_courses_hsk_level_id_status_is_active",
                table: "courses",
                columns: new[] { "hsk_level_id", "status", "is_active" });

            migrationBuilder.CreateIndex(
                name: "ix_courses_is_active",
                table: "courses",
                column: "is_active");

            migrationBuilder.CreateIndex(
                name: "ix_courses_is_featured",
                table: "courses",
                column: "is_featured");

            migrationBuilder.CreateIndex(
                name: "ix_courses_public_id",
                table: "courses",
                column: "public_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_courses_published_at",
                table: "courses",
                column: "published_at");

            migrationBuilder.CreateIndex(
                name: "ix_courses_slug",
                table: "courses",
                column: "slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_courses_sort_order",
                table: "courses",
                column: "sort_order");

            migrationBuilder.CreateIndex(
                name: "ix_courses_status",
                table: "courses",
                column: "status");

            migrationBuilder.CreateIndex(
                name: "ix_courses_status_is_featured_sort_order",
                table: "courses",
                columns: new[] { "status", "is_featured", "sort_order" });

            migrationBuilder.CreateIndex(
                name: "ix_courses_title_vi",
                table: "courses",
                column: "title_vi");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "course_chapters");

            migrationBuilder.DropTable(
                name: "course_prerequisites");

            migrationBuilder.DropTable(
                name: "courses");
        }
    }
}
