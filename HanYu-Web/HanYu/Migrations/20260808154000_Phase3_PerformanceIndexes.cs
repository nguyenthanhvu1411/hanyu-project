using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HanYu.Migrations
{
    /// <inheritdoc />
    public partial class Phase3_PerformanceIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_in_app_notifications_expires_at",
                table: "in_app_notifications");

            migrationBuilder.CreateIndex(
                name: "ix_user_lesson_progress_user_completed",
                table: "user_lesson_progress",
                columns: new[] { "user_id", "completed_at" },
                filter: "completed_at IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "ix_user_lesson_progress_user_status_last_accessed",
                table: "user_lesson_progress",
                columns: new[] { "user_id", "status", "last_accessed_at" });

            migrationBuilder.CreateIndex(
                name: "ix_learning_activities_user_type_started",
                table: "learning_activities",
                columns: new[] { "user_id", "activity_type", "started_at" });

            migrationBuilder.CreateIndex(
                name: "ix_in_app_notifications_expires_not_null",
                table: "in_app_notifications",
                column: "expires_at",
                filter: "expires_at IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "ix_in_app_notifications_user_unread",
                table: "in_app_notifications",
                columns: new[] { "user_id", "created_at" },
                filter: "read_at IS NULL");

            migrationBuilder.CreateIndex(
                name: "ix_audit_logs_action_occurred",
                table: "audit_logs",
                columns: new[] { "action", "occurred_at" });

            migrationBuilder.CreateIndex(
                name: "ix_audit_logs_entity_type_pubid_occurred",
                table: "audit_logs",
                columns: new[] { "entity_type", "entity_public_id", "occurred_at" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_user_lesson_progress_user_completed",
                table: "user_lesson_progress");

            migrationBuilder.DropIndex(
                name: "ix_user_lesson_progress_user_status_last_accessed",
                table: "user_lesson_progress");

            migrationBuilder.DropIndex(
                name: "ix_learning_activities_user_type_started",
                table: "learning_activities");

            migrationBuilder.DropIndex(
                name: "ix_in_app_notifications_expires_not_null",
                table: "in_app_notifications");

            migrationBuilder.DropIndex(
                name: "ix_in_app_notifications_user_unread",
                table: "in_app_notifications");

            migrationBuilder.DropIndex(
                name: "ix_audit_logs_action_occurred",
                table: "audit_logs");

            migrationBuilder.DropIndex(
                name: "ix_audit_logs_entity_type_pubid_occurred",
                table: "audit_logs");

            migrationBuilder.CreateIndex(
                name: "ix_in_app_notifications_expires_at",
                table: "in_app_notifications",
                column: "expires_at");
        }
    }
}
