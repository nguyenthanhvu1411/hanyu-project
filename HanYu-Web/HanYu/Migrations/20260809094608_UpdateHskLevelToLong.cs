using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace HanYu.Migrations
{
    /// <inheritdoc />
    public partial class UpdateHskLevelToLong : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<long>(
                name: "hsk_level_id",
                table: "vocabularies",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(short),
                oldType: "smallint");

            migrationBuilder.AlterColumn<long>(
                name: "hsk_level_id",
                table: "quiz_question_banks",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(short),
                oldType: "smallint",
                oldNullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "hsk_level_id",
                table: "lessons",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(short),
                oldType: "smallint");

            migrationBuilder.AlterColumn<int>(
                name: "sort_order",
                table: "hsk_levels",
                type: "integer",
                nullable: false,
                oldClrType: typeof(short),
                oldType: "smallint");

            migrationBuilder.AlterColumn<long>(
                name: "id",
                table: "hsk_levels",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(short),
                oldType: "smallint")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "created_at",
                table: "hsk_levels",
                type: "timestamptz",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<Guid>(
                name: "created_by_id",
                table: "hsk_levels",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "deleted_at",
                table: "hsk_levels",
                type: "timestamptz",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "deleted_by_id",
                table: "hsk_levels",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "public_id",
                table: "hsk_levels",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "updated_at",
                table: "hsk_levels",
                type: "timestamptz",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<Guid>(
                name: "updated_by_id",
                table: "hsk_levels",
                type: "uuid",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "created_at",
                table: "hsk_levels");

            migrationBuilder.DropColumn(
                name: "created_by_id",
                table: "hsk_levels");

            migrationBuilder.DropColumn(
                name: "deleted_at",
                table: "hsk_levels");

            migrationBuilder.DropColumn(
                name: "deleted_by_id",
                table: "hsk_levels");

            migrationBuilder.DropColumn(
                name: "public_id",
                table: "hsk_levels");

            migrationBuilder.DropColumn(
                name: "updated_at",
                table: "hsk_levels");

            migrationBuilder.DropColumn(
                name: "updated_by_id",
                table: "hsk_levels");

            migrationBuilder.AlterColumn<short>(
                name: "hsk_level_id",
                table: "vocabularies",
                type: "smallint",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<short>(
                name: "hsk_level_id",
                table: "quiz_question_banks",
                type: "smallint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AlterColumn<short>(
                name: "hsk_level_id",
                table: "lessons",
                type: "smallint",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<short>(
                name: "sort_order",
                table: "hsk_levels",
                type: "smallint",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<short>(
                name: "id",
                table: "hsk_levels",
                type: "smallint",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint")
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);
        }
    }
}
