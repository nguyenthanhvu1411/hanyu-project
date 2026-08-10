using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HanYu.Migrations
{
    /// <inheritdoc />
    public partial class LinkLessonsToCourseChapters : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "course_chapter_id",
                table: "lessons",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "concurrency_token",
                table: "course_prerequisites",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "concurrency_token",
                table: "course_chapters",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "ix_lessons_course_chapter_id",
                table: "lessons",
                column: "course_chapter_id");

            migrationBuilder.CreateIndex(
                name: "ix_lessons_course_chapter_id_sort_order",
                table: "lessons",
                columns: new[] { "course_chapter_id", "sort_order" },
                unique: true,
                filter: "course_chapter_id IS NOT NULL AND deleted_at IS NULL");

            migrationBuilder.CreateIndex(
                name: "ix_lessons_course_chapter_id_status_sort_order",
                table: "lessons",
                columns: new[] { "course_chapter_id", "status", "sort_order" });

            migrationBuilder.AddForeignKey(
                name: "fk_lessons_course_chapters_course_chapter_id",
                table: "lessons",
                column: "course_chapter_id",
                principalTable: "course_chapters",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_lessons_course_chapters_course_chapter_id",
                table: "lessons");

            migrationBuilder.DropIndex(
                name: "ix_lessons_course_chapter_id",
                table: "lessons");

            migrationBuilder.DropIndex(
                name: "ix_lessons_course_chapter_id_sort_order",
                table: "lessons");

            migrationBuilder.DropIndex(
                name: "ix_lessons_course_chapter_id_status_sort_order",
                table: "lessons");

            migrationBuilder.DropColumn(
                name: "course_chapter_id",
                table: "lessons");

            migrationBuilder.DropColumn(
                name: "concurrency_token",
                table: "course_prerequisites");

            migrationBuilder.DropColumn(
                name: "concurrency_token",
                table: "course_chapters");
        }
    }
}
