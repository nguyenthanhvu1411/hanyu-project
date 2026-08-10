using System;
using System.Net;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace HanYu.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "achievements",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    code = table.Column<string>(type: "character varying(60)", maxLength: 60, nullable: false),
                    name_vi = table.Column<string>(type: "character varying(160)", maxLength: 160, nullable: false),
                    description_vi = table.Column<string>(type: "text", nullable: true),
                    icon_url = table.Column<string>(type: "text", nullable: true),
                    xp_reward = table.Column<int>(type: "integer", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    sort_order = table.Column<int>(type: "integer", nullable: false),
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
                    table.PrimaryKey("pk_achievements", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "ai_response_cache",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    feature_type = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    cache_key = table.Column<string>(type: "character varying(160)", maxLength: 160, nullable: false),
                    model = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    prompt_version = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    response_json = table.Column<string>(type: "jsonb", nullable: false),
                    hit_count = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    last_accessed_at = table.Column<DateTimeOffset>(type: "timestamptz", nullable: true),
                    expires_at = table.Column<DateTimeOffset>(type: "timestamptz", nullable: true),
                    public_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamptz", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamptz", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_ai_response_cache", x => x.id);
                    table.CheckConstraint("ck_ai_response_cache_hit_count", "hit_count >= 0");
                });

            migrationBuilder.CreateTable(
                name: "audio_assets",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    storage_path = table.Column<string>(type: "text", nullable: false),
                    public_url = table.Column<string>(type: "text", nullable: true),
                    kind = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    mime_type = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    file_size_bytes = table.Column<long>(type: "bigint", nullable: true),
                    duration_ms = table.Column<int>(type: "integer", nullable: true),
                    voice = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: true),
                    provider = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: true),
                    language_code = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    checksum = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
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
                    table.PrimaryKey("pk_audio_assets", x => x.id);
                    table.CheckConstraint("ck_audio_assets_duration_ms", "duration_ms IS NULL OR duration_ms >= 0");
                    table.CheckConstraint("ck_audio_assets_file_size", "file_size_bytes IS NULL OR file_size_bytes >= 0");
                });

            migrationBuilder.CreateTable(
                name: "audit_logs",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    user_id = table.Column<Guid>(type: "uuid", nullable: true),
                    action = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    entity_type = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    entity_id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    entity_public_id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    old_values_json = table.Column<string>(type: "jsonb", nullable: true),
                    new_values_json = table.Column<string>(type: "jsonb", nullable: true),
                    changed_properties_json = table.Column<string>(type: "jsonb", nullable: true),
                    ip_address = table.Column<IPAddress>(type: "inet", nullable: true),
                    user_agent = table.Column<string>(type: "text", nullable: true),
                    correlation_id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    occurred_at = table.Column<DateTimeOffset>(type: "timestamptz", nullable: false),
                    public_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_audit_logs", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "content_import_jobs",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    import_type = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    original_file_name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    storage_path = table.Column<string>(type: "text", nullable: false),
                    status = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    total_rows = table.Column<int>(type: "integer", nullable: false),
                    processed_rows = table.Column<int>(type: "integer", nullable: false),
                    success_rows = table.Column<int>(type: "integer", nullable: false),
                    failed_rows = table.Column<int>(type: "integer", nullable: false),
                    started_at = table.Column<DateTimeOffset>(type: "timestamptz", nullable: true),
                    completed_at = table.Column<DateTimeOffset>(type: "timestamptz", nullable: true),
                    error_message = table.Column<string>(type: "text", nullable: true),
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
                    table.PrimaryKey("pk_content_import_jobs", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "content_reports",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    entity_type = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    entity_id = table.Column<long>(type: "bigint", nullable: false),
                    reason = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    description = table.Column<string>(type: "text", nullable: true),
                    status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    resolved_by_user_id = table.Column<Guid>(type: "uuid", nullable: true),
                    resolved_at = table.Column<DateTimeOffset>(type: "timestamptz", nullable: true),
                    resolution_note = table.Column<string>(type: "text", nullable: true),
                    public_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamptz", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamptz", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_content_reports", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "daily_learning_stats",
                columns: table => new
                {
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    stat_date = table.Column<DateOnly>(type: "date", nullable: false),
                    learning_seconds = table.Column<int>(type: "integer", nullable: false),
                    lessons_started = table.Column<int>(type: "integer", nullable: false),
                    lessons_completed = table.Column<int>(type: "integer", nullable: false),
                    vocabulary_reviewed = table.Column<int>(type: "integer", nullable: false),
                    vocabulary_learned = table.Column<int>(type: "integer", nullable: false),
                    correct_reviews = table.Column<int>(type: "integer", nullable: false),
                    wrong_reviews = table.Column<int>(type: "integer", nullable: false),
                    quiz_attempts = table.Column<int>(type: "integer", nullable: false),
                    quiz_passed = table.Column<int>(type: "integer", nullable: false),
                    ai_interactions = table.Column<int>(type: "integer", nullable: false),
                    xp_earned = table.Column<int>(type: "integer", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamptz", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_daily_learning_stats", x => new { x.user_id, x.stat_date });
                });

            migrationBuilder.CreateTable(
                name: "flashcard_sessions",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    mode = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    source_type = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    source_id = table.Column<long>(type: "bigint", nullable: true),
                    status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    started_at = table.Column<DateTimeOffset>(type: "timestamptz", nullable: false),
                    completed_at = table.Column<DateTimeOffset>(type: "timestamptz", nullable: true),
                    current_index = table.Column<int>(type: "integer", nullable: false),
                    total_items = table.Column<int>(type: "integer", nullable: false),
                    correct_items = table.Column<int>(type: "integer", nullable: false),
                    public_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamptz", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamptz", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_flashcard_sessions", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "hsk_levels",
                columns: table => new
                {
                    id = table.Column<short>(type: "smallint", nullable: false),
                    code = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    name_vi = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    sort_order = table.Column<short>(type: "smallint", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_hsk_levels", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "in_app_notifications",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    type = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    title = table.Column<string>(type: "character varying(220)", maxLength: 220, nullable: false),
                    message = table.Column<string>(type: "text", nullable: false),
                    action_url = table.Column<string>(type: "text", nullable: true),
                    metadata_json = table.Column<string>(type: "jsonb", nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "timestamptz", nullable: false),
                    read_at = table.Column<DateTimeOffset>(type: "timestamptz", nullable: true),
                    expires_at = table.Column<DateTimeOffset>(type: "timestamptz", nullable: true),
                    public_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_in_app_notifications", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "notification_preferences",
                columns: table => new
                {
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    in_app_enabled = table.Column<bool>(type: "boolean", nullable: false),
                    email_enabled = table.Column<bool>(type: "boolean", nullable: false),
                    learning_reminder_enabled = table.Column<bool>(type: "boolean", nullable: false),
                    review_reminder_enabled = table.Column<bool>(type: "boolean", nullable: false),
                    security_notification_enabled = table.Column<bool>(type: "boolean", nullable: false),
                    preferred_reminder_time = table.Column<TimeOnly>(type: "time without time zone", nullable: true),
                    timezone = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamptz", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_notification_preferences", x => x.user_id);
                });

            migrationBuilder.CreateTable(
                name: "parts_of_speech",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    code = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    name_vi = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    name_en = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
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
                    table.PrimaryKey("pk_parts_of_speech", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "product_events",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    user_id = table.Column<Guid>(type: "uuid", nullable: true),
                    session_id = table.Column<Guid>(type: "uuid", nullable: true),
                    event_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    entity_type = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: true),
                    entity_public_id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    properties_json = table.Column<string>(type: "jsonb", nullable: true),
                    page_path = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    referrer = table.Column<string>(type: "text", nullable: true),
                    device_type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    occurred_at = table.Column<DateTimeOffset>(type: "timestamptz", nullable: false),
                    public_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_product_events", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "quiz_question_banks",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    name_vi = table.Column<string>(type: "character varying(160)", maxLength: 160, nullable: false),
                    description_vi = table.Column<string>(type: "text", nullable: true),
                    hsk_level_id = table.Column<short>(type: "smallint", nullable: true),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
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
                    table.PrimaryKey("pk_quiz_question_banks", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "quiz_tags",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    slug = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    name_vi = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    description_vi = table.Column<string>(type: "text", nullable: true),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
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
                    table.PrimaryKey("pk_quiz_tags", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "review_events",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    vocabulary_id = table.Column<long>(type: "bigint", nullable: false),
                    flashcard_session_id = table.Column<long>(type: "bigint", nullable: true),
                    rating = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    was_correct = table.Column<bool>(type: "boolean", nullable: false),
                    response_time_ms = table.Column<int>(type: "integer", nullable: true),
                    mastery_before = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: false),
                    mastery_after = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: false),
                    interval_before_minutes = table.Column<int>(type: "integer", nullable: true),
                    interval_after_minutes = table.Column<int>(type: "integer", nullable: false),
                    reviewed_at = table.Column<DateTimeOffset>(type: "timestamptz", nullable: false),
                    public_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_review_events", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "roles",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    normalized_name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    concurrency_stamp = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_roles", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "topics",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    slug = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    name_vi = table.Column<string>(type: "character varying(160)", maxLength: 160, nullable: false),
                    description_vi = table.Column<string>(type: "text", nullable: true),
                    sort_order = table.Column<int>(type: "integer", nullable: false),
                    status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
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
                    table.PrimaryKey("pk_topics", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "user_achievements",
                columns: table => new
                {
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    achievement_id = table.Column<long>(type: "bigint", nullable: false),
                    unlocked_at = table.Column<DateTimeOffset>(type: "timestamptz", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_user_achievements", x => new { x.user_id, x.achievement_id });
                });

            migrationBuilder.CreateTable(
                name: "user_learning_summaries",
                columns: table => new
                {
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    total_learning_seconds = table.Column<int>(type: "integer", nullable: false),
                    total_lessons_completed = table.Column<int>(type: "integer", nullable: false),
                    total_vocabulary_learned = table.Column<int>(type: "integer", nullable: false),
                    total_vocabulary_mastered = table.Column<int>(type: "integer", nullable: false),
                    total_reviews = table.Column<int>(type: "integer", nullable: false),
                    total_quiz_attempts = table.Column<int>(type: "integer", nullable: false),
                    total_quiz_passed = table.Column<int>(type: "integer", nullable: false),
                    total_xp = table.Column<int>(type: "integer", nullable: false),
                    current_hsk_level = table.Column<short>(type: "smallint", nullable: false),
                    overall_mastery_percent = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: false),
                    last_learning_at = table.Column<DateTimeOffset>(type: "timestamptz", nullable: true),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamptz", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_user_learning_summaries", x => x.user_id);
                    table.CheckConstraint("ck_user_learning_summaries_mastery", "overall_mastery_percent >= 0 AND overall_mastery_percent <= 100");
                });

            migrationBuilder.CreateTable(
                name: "user_streaks",
                columns: table => new
                {
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    current_streak = table.Column<int>(type: "integer", nullable: false),
                    longest_streak = table.Column<int>(type: "integer", nullable: false),
                    last_learning_date = table.Column<DateOnly>(type: "date", nullable: true),
                    current_streak_started_at = table.Column<DateOnly>(type: "date", nullable: true),
                    total_active_days = table.Column<int>(type: "integer", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamptz", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_user_streaks", x => x.user_id);
                });

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    public_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamptz", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamptz", nullable: false),
                    deleted_at = table.Column<DateTimeOffset>(type: "timestamptz", nullable: true),
                    last_login_at = table.Column<DateTimeOffset>(type: "timestamptz", nullable: true),
                    user_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    normalized_user_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    email = table.Column<string>(type: "character varying(320)", maxLength: 320, nullable: true),
                    normalized_email = table.Column<string>(type: "character varying(320)", maxLength: 320, nullable: true),
                    email_confirmed = table.Column<bool>(type: "boolean", nullable: false),
                    password_hash = table.Column<string>(type: "text", nullable: true),
                    security_stamp = table.Column<string>(type: "text", nullable: true),
                    concurrency_stamp = table.Column<string>(type: "text", nullable: true),
                    phone_number = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    phone_number_confirmed = table.Column<bool>(type: "boolean", nullable: false),
                    two_factor_enabled = table.Column<bool>(type: "boolean", nullable: false),
                    lockout_end = table.Column<DateTimeOffset>(type: "timestamptz", nullable: true),
                    lockout_enabled = table.Column<bool>(type: "boolean", nullable: false),
                    access_failed_count = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_users", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "xp_transactions",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    amount = table.Column<int>(type: "integer", nullable: false),
                    reason = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    source_type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    source_id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "timestamptz", nullable: false),
                    public_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_xp_transactions", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "content_import_rows",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    import_job_id = table.Column<long>(type: "bigint", nullable: false),
                    row_number = table.Column<int>(type: "integer", nullable: false),
                    source_json = table.Column<string>(type: "jsonb", nullable: false),
                    is_successful = table.Column<bool>(type: "boolean", nullable: false),
                    created_entity_id = table.Column<long>(type: "bigint", nullable: true),
                    error_code = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: true),
                    error_message = table.Column<string>(type: "text", nullable: true),
                    processed_at = table.Column<DateTimeOffset>(type: "timestamptz", nullable: false),
                    public_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_content_import_rows", x => x.id);
                    table.ForeignKey(
                        name: "fk_content_import_rows_content_import_jobs_import_job_id",
                        column: x => x.import_job_id,
                        principalTable: "content_import_jobs",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "flashcard_session_items",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    flashcard_session_id = table.Column<long>(type: "bigint", nullable: false),
                    vocabulary_id = table.Column<long>(type: "bigint", nullable: false),
                    sort_order = table.Column<int>(type: "integer", nullable: false),
                    rating = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    was_correct = table.Column<bool>(type: "boolean", nullable: true),
                    response_time_ms = table.Column<int>(type: "integer", nullable: true),
                    answered_at = table.Column<DateTimeOffset>(type: "timestamptz", nullable: true),
                    public_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_flashcard_session_items", x => x.id);
                    table.ForeignKey(
                        name: "fk_flashcard_session_items_flashcard_sessions_flashcard_sessio",
                        column: x => x.flashcard_session_id,
                        principalTable: "flashcard_sessions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "notification_deliveries",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    notification_id = table.Column<long>(type: "bigint", nullable: false),
                    channel = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    destination = table.Column<string>(type: "character varying(320)", maxLength: 320, nullable: true),
                    provider = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    provider_message_id = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    attempt_count = table.Column<int>(type: "integer", nullable: false),
                    last_attempt_at = table.Column<DateTimeOffset>(type: "timestamptz", nullable: true),
                    sent_at = table.Column<DateTimeOffset>(type: "timestamptz", nullable: true),
                    delivered_at = table.Column<DateTimeOffset>(type: "timestamptz", nullable: true),
                    failed_at = table.Column<DateTimeOffset>(type: "timestamptz", nullable: true),
                    failure_code = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: true),
                    failure_reason = table.Column<string>(type: "text", nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "timestamptz", nullable: false),
                    public_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_notification_deliveries", x => x.id);
                    table.ForeignKey(
                        name: "fk_notification_deliveries_in_app_notifications_notification_id",
                        column: x => x.notification_id,
                        principalTable: "in_app_notifications",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "role_claims",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    role_id = table.Column<Guid>(type: "uuid", nullable: false),
                    claim_type = table.Column<string>(type: "text", nullable: true),
                    claim_value = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_role_claims", x => x.id);
                    table.ForeignKey(
                        name: "fk_role_claims_roles_role_id",
                        column: x => x.role_id,
                        principalTable: "roles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "lessons",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    hsk_level_id = table.Column<short>(type: "smallint", nullable: false),
                    topic_id = table.Column<long>(type: "bigint", nullable: true),
                    slug = table.Column<string>(type: "character varying(160)", maxLength: 160, nullable: false),
                    title_vi = table.Column<string>(type: "character varying(220)", maxLength: 220, nullable: false),
                    short_description_vi = table.Column<string>(type: "character varying(320)", maxLength: 320, nullable: true),
                    description_vi = table.Column<string>(type: "text", nullable: true),
                    objective_vi = table.Column<string>(type: "text", nullable: true),
                    cover_image_url = table.Column<string>(type: "text", nullable: true),
                    sort_order = table.Column<int>(type: "integer", nullable: false),
                    estimated_minutes = table.Column<short>(type: "smallint", nullable: false),
                    difficulty = table.Column<short>(type: "smallint", nullable: false),
                    is_featured = table.Column<bool>(type: "boolean", nullable: false),
                    status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    version = table.Column<int>(type: "integer", nullable: false),
                    published_at = table.Column<DateTimeOffset>(type: "timestamptz", nullable: true),
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
                    table.PrimaryKey("pk_lessons", x => x.id);
                    table.CheckConstraint("ck_lessons_difficulty", "difficulty >= 1 AND difficulty <= 5");
                    table.CheckConstraint("ck_lessons_estimated_minutes", "estimated_minutes >= 1 AND estimated_minutes <= 120");
                    table.ForeignKey(
                        name: "fk_lessons_hsk_levels_hsk_level_id",
                        column: x => x.hsk_level_id,
                        principalTable: "hsk_levels",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_lessons_topics_topic_id",
                        column: x => x.topic_id,
                        principalTable: "topics",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "vocabularies",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    hsk_level_id = table.Column<short>(type: "smallint", nullable: false),
                    part_of_speech_id = table.Column<long>(type: "bigint", nullable: true),
                    topic_id = table.Column<long>(type: "bigint", nullable: true),
                    audio_asset_id = table.Column<long>(type: "bigint", nullable: true),
                    simplified = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    traditional = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    pinyin = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    pinyin_normalized = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    primary_meaning_vi = table.Column<string>(type: "text", nullable: false),
                    notes_vi = table.Column<string>(type: "text", nullable: true),
                    difficulty = table.Column<short>(type: "smallint", nullable: false),
                    status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    version = table.Column<int>(type: "integer", nullable: false),
                    published_at = table.Column<DateTimeOffset>(type: "timestamptz", nullable: true),
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
                    table.PrimaryKey("pk_vocabularies", x => x.id);
                    table.ForeignKey(
                        name: "fk_vocabularies_audio_assets_audio_asset_id",
                        column: x => x.audio_asset_id,
                        principalTable: "audio_assets",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_vocabularies_hsk_levels_hsk_level_id",
                        column: x => x.hsk_level_id,
                        principalTable: "hsk_levels",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_vocabularies_parts_of_speech_part_of_speech_id",
                        column: x => x.part_of_speech_id,
                        principalTable: "parts_of_speech",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "fk_vocabularies_topics_topic_id",
                        column: x => x.topic_id,
                        principalTable: "topics",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "user_claims",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    claim_type = table.Column<string>(type: "text", nullable: true),
                    claim_value = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_user_claims", x => x.id);
                    table.ForeignKey(
                        name: "fk_user_claims_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "user_consents",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    consent_type = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    version = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    is_granted = table.Column<bool>(type: "boolean", nullable: false),
                    granted_at = table.Column<DateTimeOffset>(type: "timestamptz", nullable: true),
                    revoked_at = table.Column<DateTimeOffset>(type: "timestamptz", nullable: true),
                    public_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_user_consents", x => x.id);
                    table.ForeignKey(
                        name: "fk_user_consents_asp_net_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "user_data_export_jobs",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    status = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    storage_path = table.Column<string>(type: "text", nullable: true),
                    requested_at = table.Column<DateTimeOffset>(type: "timestamptz", nullable: false),
                    completed_at = table.Column<DateTimeOffset>(type: "timestamptz", nullable: true),
                    expires_at = table.Column<DateTimeOffset>(type: "timestamptz", nullable: true),
                    error_message = table.Column<string>(type: "text", nullable: true),
                    public_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamptz", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamptz", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_user_data_export_jobs", x => x.id);
                    table.ForeignKey(
                        name: "fk_user_data_export_jobs_asp_net_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "user_learning_goals",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    target_hsk_level = table.Column<short>(type: "smallint", nullable: false),
                    target_date = table.Column<DateOnly>(type: "date", nullable: true),
                    daily_goal_minutes = table.Column<short>(type: "smallint", nullable: false),
                    daily_vocabulary_goal = table.Column<short>(type: "smallint", nullable: true),
                    weekly_lesson_goal = table.Column<short>(type: "smallint", nullable: true),
                    status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    started_at = table.Column<DateTimeOffset>(type: "timestamptz", nullable: false),
                    completed_at = table.Column<DateTimeOffset>(type: "timestamptz", nullable: true),
                    paused_at = table.Column<DateTimeOffset>(type: "timestamptz", nullable: true),
                    public_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamptz", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamptz", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_user_learning_goals", x => x.id);
                    table.CheckConstraint("ck_user_learning_goals_daily_minutes", "daily_goal_minutes >= 1");
                    table.CheckConstraint("ck_user_learning_goals_hsk", "target_hsk_level >= 1 AND target_hsk_level <= 6");
                    table.ForeignKey(
                        name: "fk_user_learning_goals_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "user_login_histories",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    is_successful = table.Column<bool>(type: "boolean", nullable: false),
                    ip_address = table.Column<IPAddress>(type: "inet", nullable: true),
                    user_agent = table.Column<string>(type: "text", nullable: true),
                    device_name = table.Column<string>(type: "character varying(160)", maxLength: 160, nullable: true),
                    browser = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    operating_system = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    failure_reason = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    attempted_at = table.Column<DateTimeOffset>(type: "timestamptz", nullable: false),
                    public_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_user_login_histories", x => x.id);
                    table.ForeignKey(
                        name: "fk_user_login_histories_asp_net_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "user_logins",
                columns: table => new
                {
                    login_provider = table.Column<string>(type: "text", nullable: false),
                    provider_key = table.Column<string>(type: "text", nullable: false),
                    provider_display_name = table.Column<string>(type: "text", nullable: true),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_user_logins", x => new { x.login_provider, x.provider_key });
                    table.ForeignKey(
                        name: "fk_user_logins_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "user_preferences",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    show_pinyin = table.Column<bool>(type: "boolean", nullable: false),
                    show_traditional = table.Column<bool>(type: "boolean", nullable: false),
                    auto_play_audio = table.Column<bool>(type: "boolean", nullable: false),
                    audio_playback_rate = table.Column<decimal>(type: "numeric(3,2)", precision: 3, scale: 2, nullable: false),
                    theme = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    default_flashcard_mode = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    reduced_motion = table.Column<bool>(type: "boolean", nullable: false),
                    public_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamptz", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamptz", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_user_preferences", x => x.id);
                    table.CheckConstraint("ck_user_preferences_audio_playback_rate", "audio_playback_rate >= 0.50 AND audio_playback_rate <= 2.00");
                    table.ForeignKey(
                        name: "fk_user_preferences_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "user_profiles",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    display_name = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    avatar_url = table.Column<string>(type: "text", nullable: true),
                    current_hsk_level = table.Column<short>(type: "smallint", nullable: false),
                    daily_goal_minutes = table.Column<short>(type: "smallint", nullable: false),
                    timezone = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    ui_language = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    onboarding_completed = table.Column<bool>(type: "boolean", nullable: false),
                    onboarding_completed_at = table.Column<DateTimeOffset>(type: "timestamptz", nullable: true),
                    public_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamptz", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamptz", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_user_profiles", x => x.id);
                    table.CheckConstraint("ck_user_profiles_current_hsk_level", "current_hsk_level >= 1 AND current_hsk_level <= 6");
                    table.CheckConstraint("ck_user_profiles_daily_goal_minutes", "daily_goal_minutes >= 5 AND daily_goal_minutes <= 180");
                    table.ForeignKey(
                        name: "fk_user_profiles_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "user_roles",
                columns: table => new
                {
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    role_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_user_roles", x => new { x.user_id, x.role_id });
                    table.ForeignKey(
                        name: "fk_user_roles_roles_role_id",
                        column: x => x.role_id,
                        principalTable: "roles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_user_roles_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "user_security_events",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    event_type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    ip_address = table.Column<IPAddress>(type: "inet", nullable: true),
                    user_agent = table.Column<string>(type: "text", nullable: true),
                    metadata_json = table.Column<string>(type: "jsonb", nullable: true),
                    occurred_at = table.Column<DateTimeOffset>(type: "timestamptz", nullable: false),
                    public_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_user_security_events", x => x.id);
                    table.ForeignKey(
                        name: "fk_user_security_events_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "user_sessions",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    session_key = table.Column<Guid>(type: "uuid", nullable: false),
                    device_name = table.Column<string>(type: "character varying(160)", maxLength: 160, nullable: true),
                    device_type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    browser = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    operating_system = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    ip_address = table.Column<IPAddress>(type: "inet", nullable: true),
                    user_agent = table.Column<string>(type: "text", nullable: true),
                    last_activity_at = table.Column<DateTimeOffset>(type: "timestamptz", nullable: false),
                    revoked_at = table.Column<DateTimeOffset>(type: "timestamptz", nullable: true),
                    status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    public_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamptz", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamptz", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_user_sessions", x => x.id);
                    table.ForeignKey(
                        name: "fk_user_sessions_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "user_tokens",
                columns: table => new
                {
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    login_provider = table.Column<string>(type: "text", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    value = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_user_tokens", x => new { x.user_id, x.login_provider, x.name });
                    table.ForeignKey(
                        name: "fk_user_tokens_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "lesson_assets",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    lesson_id = table.Column<long>(type: "bigint", nullable: false),
                    audio_asset_id = table.Column<long>(type: "bigint", nullable: true),
                    asset_type = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    url = table.Column<string>(type: "text", nullable: true),
                    caption_vi = table.Column<string>(type: "text", nullable: true),
                    sort_order = table.Column<int>(type: "integer", nullable: false),
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
                    table.PrimaryKey("pk_lesson_assets", x => x.id);
                    table.ForeignKey(
                        name: "fk_lesson_assets_audio_assets_audio_asset_id",
                        column: x => x.audio_asset_id,
                        principalTable: "audio_assets",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "fk_lesson_assets_lessons_lesson_id",
                        column: x => x.lesson_id,
                        principalTable: "lessons",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "lesson_prerequisites",
                columns: table => new
                {
                    lesson_id = table.Column<long>(type: "bigint", nullable: false),
                    required_lesson_id = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_lesson_prerequisites", x => new { x.lesson_id, x.required_lesson_id });
                    table.CheckConstraint("ck_lesson_prerequisites_not_self", "lesson_id <> required_lesson_id");
                    table.ForeignKey(
                        name: "fk_lesson_prerequisites_lessons_lesson_id",
                        column: x => x.lesson_id,
                        principalTable: "lessons",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_lesson_prerequisites_lessons_required_lesson_id",
                        column: x => x.required_lesson_id,
                        principalTable: "lessons",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "lesson_sections",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    lesson_id = table.Column<long>(type: "bigint", nullable: false),
                    section_type = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    title_vi = table.Column<string>(type: "character varying(220)", maxLength: 220, nullable: true),
                    content_vi = table.Column<string>(type: "text", nullable: true),
                    sort_order = table.Column<int>(type: "integer", nullable: false),
                    is_required = table.Column<bool>(type: "boolean", nullable: false),
                    estimated_seconds = table.Column<int>(type: "integer", nullable: true),
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
                    table.PrimaryKey("pk_lesson_sections", x => x.id);
                    table.ForeignKey(
                        name: "fk_lesson_sections_lessons_lesson_id",
                        column: x => x.lesson_id,
                        principalTable: "lessons",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "quizzes",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    lesson_id = table.Column<long>(type: "bigint", nullable: true),
                    title_vi = table.Column<string>(type: "character varying(220)", maxLength: 220, nullable: false),
                    description_vi = table.Column<string>(type: "text", nullable: true),
                    quiz_type = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    passing_score = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: false),
                    time_limit_seconds = table.Column<int>(type: "integer", nullable: true),
                    max_attempts = table.Column<int>(type: "integer", nullable: false),
                    shuffle_mode = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    feedback_mode = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    allow_retry = table.Column<bool>(type: "boolean", nullable: false),
                    show_correct_answer = table.Column<bool>(type: "boolean", nullable: false),
                    show_explanation = table.Column<bool>(type: "boolean", nullable: false),
                    status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    version = table.Column<int>(type: "integer", nullable: false),
                    published_at = table.Column<DateTimeOffset>(type: "timestamptz", nullable: true),
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
                    table.PrimaryKey("pk_quizzes", x => x.id);
                    table.CheckConstraint("ck_quizzes_passing_score", "passing_score >= 0 AND passing_score <= 100");
                    table.CheckConstraint("ck_quizzes_time_limit", "time_limit_seconds IS NULL OR time_limit_seconds > 0");
                    table.ForeignKey(
                        name: "fk_quizzes_lessons_lesson_id",
                        column: x => x.lesson_id,
                        principalTable: "lessons",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "user_lesson_bookmarks",
                columns: table => new
                {
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    lesson_id = table.Column<long>(type: "bigint", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamptz", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_user_lesson_bookmarks", x => new { x.user_id, x.lesson_id });
                    table.ForeignKey(
                        name: "fk_user_lesson_bookmarks_lessons_lesson_id",
                        column: x => x.lesson_id,
                        principalTable: "lessons",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_user_lesson_bookmarks_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "user_lesson_progress",
                columns: table => new
                {
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    lesson_id = table.Column<long>(type: "bigint", nullable: false),
                    last_section_id = table.Column<long>(type: "bigint", nullable: true),
                    status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    started_at = table.Column<DateTimeOffset>(type: "timestamptz", nullable: true),
                    last_accessed_at = table.Column<DateTimeOffset>(type: "timestamptz", nullable: true),
                    completed_at = table.Column<DateTimeOffset>(type: "timestamptz", nullable: true),
                    last_position = table.Column<int>(type: "integer", nullable: false),
                    completion_percent = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamptz", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_user_lesson_progress", x => new { x.user_id, x.lesson_id });
                    table.CheckConstraint("ck_user_lesson_progress_completion", "completion_percent >= 0 AND completion_percent <= 100");
                    table.ForeignKey(
                        name: "fk_user_lesson_progress_asp_net_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_user_lesson_progress_lessons_lesson_id",
                        column: x => x.lesson_id,
                        principalTable: "lessons",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ai_conversations",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    lesson_id = table.Column<long>(type: "bigint", nullable: true),
                    vocabulary_id = table.Column<long>(type: "bigint", nullable: true),
                    title = table.Column<string>(type: "character varying(220)", maxLength: 220, nullable: true),
                    status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    message_count = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    last_message_at = table.Column<DateTimeOffset>(type: "timestamptz", nullable: false),
                    public_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamptz", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamptz", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_ai_conversations", x => x.id);
                    table.CheckConstraint("ck_ai_conversations_message_count", "message_count >= 0");
                    table.ForeignKey(
                        name: "fk_ai_conversations_asp_net_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_ai_conversations_lessons_lesson_id",
                        column: x => x.lesson_id,
                        principalTable: "lessons",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "fk_ai_conversations_vocabularies_vocabulary_id",
                        column: x => x.vocabulary_id,
                        principalTable: "vocabularies",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "lesson_vocabularies",
                columns: table => new
                {
                    lesson_id = table.Column<long>(type: "bigint", nullable: false),
                    vocabulary_id = table.Column<long>(type: "bigint", nullable: false),
                    sort_order = table.Column<short>(type: "smallint", nullable: false),
                    is_required = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_lesson_vocabularies", x => new { x.lesson_id, x.vocabulary_id });
                    table.ForeignKey(
                        name: "fk_lesson_vocabularies_lessons_lesson_id",
                        column: x => x.lesson_id,
                        principalTable: "lessons",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_lesson_vocabularies_vocabularies_vocabulary_id",
                        column: x => x.vocabulary_id,
                        principalTable: "vocabularies",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "user_vocabulary_notes",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    vocabulary_id = table.Column<long>(type: "bigint", nullable: false),
                    content = table.Column<string>(type: "text", nullable: false),
                    is_pinned = table.Column<bool>(type: "boolean", nullable: false),
                    public_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamptz", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamptz", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_user_vocabulary_notes", x => x.id);
                    table.ForeignKey(
                        name: "fk_user_vocabulary_notes_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_user_vocabulary_notes_vocabularies_vocabulary_id",
                        column: x => x.vocabulary_id,
                        principalTable: "vocabularies",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "user_vocabulary_states",
                columns: table => new
                {
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    vocabulary_id = table.Column<long>(type: "bigint", nullable: false),
                    learning_state = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    is_favorite = table.Column<bool>(type: "boolean", nullable: false),
                    mastery_score = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: false),
                    correct_count = table.Column<int>(type: "integer", nullable: false),
                    wrong_count = table.Column<int>(type: "integer", nullable: false),
                    consecutive_correct = table.Column<int>(type: "integer", nullable: false),
                    distinct_correct_days = table.Column<int>(type: "integer", nullable: false),
                    last_correct_at = table.Column<DateTimeOffset>(type: "timestamptz", nullable: true),
                    last_reviewed_at = table.Column<DateTimeOffset>(type: "timestamptz", nullable: true),
                    next_review_at = table.Column<DateTimeOffset>(type: "timestamptz", nullable: true),
                    current_interval_minutes = table.Column<int>(type: "integer", nullable: true),
                    first_learned_at = table.Column<DateTimeOffset>(type: "timestamptz", nullable: true),
                    mastered_at = table.Column<DateTimeOffset>(type: "timestamptz", nullable: true),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamptz", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_user_vocabulary_states", x => new { x.user_id, x.vocabulary_id });
                    table.CheckConstraint("ck_user_vocabulary_states_mastery", "mastery_score >= 0 AND mastery_score <= 100");
                    table.ForeignKey(
                        name: "fk_user_vocabulary_states_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_user_vocabulary_states_vocabularies_vocabulary_id",
                        column: x => x.vocabulary_id,
                        principalTable: "vocabularies",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "vocabulary_examples",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    vocabulary_id = table.Column<long>(type: "bigint", nullable: false),
                    audio_asset_id = table.Column<long>(type: "bigint", nullable: true),
                    sentence_zh = table.Column<string>(type: "text", nullable: false),
                    sentence_pinyin = table.Column<string>(type: "text", nullable: false),
                    sentence_vi = table.Column<string>(type: "text", nullable: false),
                    difficulty = table.Column<short>(type: "smallint", nullable: false),
                    status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    source_note = table.Column<string>(type: "text", nullable: true),
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
                    table.PrimaryKey("pk_vocabulary_examples", x => x.id);
                    table.ForeignKey(
                        name: "fk_vocabulary_examples_audio_assets_audio_asset_id",
                        column: x => x.audio_asset_id,
                        principalTable: "audio_assets",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "fk_vocabulary_examples_vocabularies_vocabulary_id",
                        column: x => x.vocabulary_id,
                        principalTable: "vocabularies",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "vocabulary_meanings",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    vocabulary_id = table.Column<long>(type: "bigint", nullable: false),
                    meaning_vi = table.Column<string>(type: "text", nullable: false),
                    sense_order = table.Column<short>(type: "smallint", nullable: false),
                    usage_note_vi = table.Column<string>(type: "text", nullable: true),
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
                    table.PrimaryKey("pk_vocabulary_meanings", x => x.id);
                    table.ForeignKey(
                        name: "fk_vocabulary_meanings_vocabularies_vocabulary_id",
                        column: x => x.vocabulary_id,
                        principalTable: "vocabularies",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "vocabulary_relations",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    vocabulary_id = table.Column<long>(type: "bigint", nullable: false),
                    related_vocabulary_id = table.Column<long>(type: "bigint", nullable: false),
                    relation_type = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    note_vi = table.Column<string>(type: "text", nullable: true),
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
                    table.PrimaryKey("pk_vocabulary_relations", x => x.id);
                    table.CheckConstraint("ck_vocabulary_relations_not_self", "vocabulary_id <> related_vocabulary_id");
                    table.ForeignKey(
                        name: "fk_vocabulary_relations_vocabularies_related_vocabulary_id",
                        column: x => x.related_vocabulary_id,
                        principalTable: "vocabularies",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_vocabulary_relations_vocabularies_vocabulary_id",
                        column: x => x.vocabulary_id,
                        principalTable: "vocabularies",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "refresh_tokens",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_session_id = table.Column<long>(type: "bigint", nullable: true),
                    token_hash = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    family_id = table.Column<Guid>(type: "uuid", nullable: false),
                    issued_at = table.Column<DateTimeOffset>(type: "timestamptz", nullable: false),
                    expires_at = table.Column<DateTimeOffset>(type: "timestamptz", nullable: false),
                    used_at = table.Column<DateTimeOffset>(type: "timestamptz", nullable: true),
                    revoked_at = table.Column<DateTimeOffset>(type: "timestamptz", nullable: true),
                    replaced_by_token_id = table.Column<long>(type: "bigint", nullable: true),
                    created_by_ip = table.Column<IPAddress>(type: "inet", nullable: true),
                    revoked_by_ip = table.Column<IPAddress>(type: "inet", nullable: true),
                    user_agent = table.Column<string>(type: "text", nullable: true),
                    revoke_reason = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    public_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_refresh_tokens", x => x.id);
                    table.ForeignKey(
                        name: "fk_refresh_tokens_user_sessions_user_session_id",
                        column: x => x.user_session_id,
                        principalTable: "user_sessions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "fk_refresh_tokens_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "user_blocked_sessions",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_session_id = table.Column<long>(type: "bigint", nullable: false),
                    reason = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    blocked_at = table.Column<DateTimeOffset>(type: "timestamptz", nullable: false),
                    expires_at = table.Column<DateTimeOffset>(type: "timestamptz", nullable: true),
                    blocked_by_user_id = table.Column<Guid>(type: "uuid", nullable: true),
                    ip_address = table.Column<IPAddress>(type: "inet", nullable: true),
                    public_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_user_blocked_sessions", x => x.id);
                    table.ForeignKey(
                        name: "fk_user_blocked_sessions_user_sessions_user_session_id",
                        column: x => x.user_session_id,
                        principalTable: "user_sessions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_user_blocked_sessions_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "user_lesson_section_progress",
                columns: table => new
                {
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    lesson_section_id = table.Column<long>(type: "bigint", nullable: false),
                    is_completed = table.Column<bool>(type: "boolean", nullable: false),
                    started_at = table.Column<DateTimeOffset>(type: "timestamptz", nullable: true),
                    completed_at = table.Column<DateTimeOffset>(type: "timestamptz", nullable: true),
                    time_spent_seconds = table.Column<int>(type: "integer", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamptz", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_user_lesson_section_progress", x => new { x.user_id, x.lesson_section_id });
                    table.CheckConstraint("ck_user_lesson_section_progress_time", "time_spent_seconds >= 0");
                    table.ForeignKey(
                        name: "fk_user_lesson_section_progress_lesson_sections_lesson_section",
                        column: x => x.lesson_section_id,
                        principalTable: "lesson_sections",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "quiz_attempts",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    quiz_id = table.Column<long>(type: "bigint", nullable: false),
                    attempt_number = table.Column<int>(type: "integer", nullable: false),
                    idempotency_key = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    score = table.Column<decimal>(type: "numeric(8,2)", precision: 8, scale: 2, nullable: true),
                    max_score = table.Column<decimal>(type: "numeric(8,2)", precision: 8, scale: 2, nullable: true),
                    percentage = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: true),
                    is_passed = table.Column<bool>(type: "boolean", nullable: true),
                    started_at = table.Column<DateTimeOffset>(type: "timestamptz", nullable: false),
                    submitted_at = table.Column<DateTimeOffset>(type: "timestamptz", nullable: true),
                    expires_at = table.Column<DateTimeOffset>(type: "timestamptz", nullable: true),
                    duration_seconds = table.Column<int>(type: "integer", nullable: true),
                    correct_answers = table.Column<int>(type: "integer", nullable: false),
                    wrong_answers = table.Column<int>(type: "integer", nullable: false),
                    unanswered_questions = table.Column<int>(type: "integer", nullable: false),
                    public_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamptz", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamptz", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_quiz_attempts", x => x.id);
                    table.ForeignKey(
                        name: "fk_quiz_attempts_asp_net_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_quiz_attempts_quizzes_quiz_id",
                        column: x => x.quiz_id,
                        principalTable: "quizzes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "quiz_questions",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    quiz_id = table.Column<long>(type: "bigint", nullable: false),
                    vocabulary_id = table.Column<long>(type: "bigint", nullable: true),
                    question_type = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    prompt = table.Column<string>(type: "text", nullable: false),
                    prompt_pinyin = table.Column<string>(type: "text", nullable: true),
                    correct_answer_text = table.Column<string>(type: "text", nullable: true),
                    explanation_vi = table.Column<string>(type: "text", nullable: true),
                    hint_vi = table.Column<string>(type: "text", nullable: true),
                    points = table.Column<decimal>(type: "numeric(8,2)", precision: 8, scale: 2, nullable: false),
                    sort_order = table.Column<int>(type: "integer", nullable: false),
                    time_limit_seconds = table.Column<int>(type: "integer", nullable: true),
                    is_required = table.Column<bool>(type: "boolean", nullable: false),
                    status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
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
                    table.PrimaryKey("pk_quiz_questions", x => x.id);
                    table.CheckConstraint("ck_quiz_questions_points", "points > 0");
                    table.ForeignKey(
                        name: "fk_quiz_questions_quizzes_quiz_id",
                        column: x => x.quiz_id,
                        principalTable: "quizzes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_quiz_questions_vocabularies_vocabulary_id",
                        column: x => x.vocabulary_id,
                        principalTable: "vocabularies",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "learning_activities",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    activity_type = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    lesson_id = table.Column<long>(type: "bigint", nullable: true),
                    vocabulary_id = table.Column<long>(type: "bigint", nullable: true),
                    quiz_attempt_id = table.Column<long>(type: "bigint", nullable: true),
                    flashcard_session_id = table.Column<long>(type: "bigint", nullable: true),
                    duration_seconds = table.Column<int>(type: "integer", nullable: false),
                    xp_earned = table.Column<int>(type: "integer", nullable: false),
                    is_completed = table.Column<bool>(type: "boolean", nullable: false),
                    metadata_json = table.Column<string>(type: "jsonb", nullable: true),
                    started_at = table.Column<DateTimeOffset>(type: "timestamptz", nullable: false),
                    completed_at = table.Column<DateTimeOffset>(type: "timestamptz", nullable: true),
                    public_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_learning_activities", x => x.id);
                    table.ForeignKey(
                        name: "fk_learning_activities_flashcard_sessions_flashcard_session_id",
                        column: x => x.flashcard_session_id,
                        principalTable: "flashcard_sessions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "fk_learning_activities_lessons_lesson_id",
                        column: x => x.lesson_id,
                        principalTable: "lessons",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "fk_learning_activities_quiz_attempt_quiz_attempt_id",
                        column: x => x.quiz_attempt_id,
                        principalTable: "quiz_attempts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "fk_learning_activities_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_learning_activities_vocabularies_vocabulary_id",
                        column: x => x.vocabulary_id,
                        principalTable: "vocabularies",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "quiz_attempt_questions",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    attempt_id = table.Column<long>(type: "bigint", nullable: false),
                    question_id = table.Column<long>(type: "bigint", nullable: false),
                    sort_order = table.Column<int>(type: "integer", nullable: false),
                    question_snapshot_json = table.Column<string>(type: "jsonb", nullable: false),
                    public_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_quiz_attempt_questions", x => x.id);
                    table.ForeignKey(
                        name: "fk_quiz_attempt_questions_quiz_attempts_attempt_id",
                        column: x => x.attempt_id,
                        principalTable: "quiz_attempts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_quiz_attempt_questions_quiz_questions_question_id",
                        column: x => x.question_id,
                        principalTable: "quiz_questions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "quiz_matching_pairs",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    question_id = table.Column<long>(type: "bigint", nullable: false),
                    left_text = table.Column<string>(type: "text", nullable: false),
                    right_text = table.Column<string>(type: "text", nullable: false),
                    left_pinyin = table.Column<string>(type: "text", nullable: true),
                    right_pinyin = table.Column<string>(type: "text", nullable: true),
                    sort_order = table.Column<short>(type: "smallint", nullable: false),
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
                    table.PrimaryKey("pk_quiz_matching_pairs", x => x.id);
                    table.ForeignKey(
                        name: "fk_quiz_matching_pairs_quiz_questions_question_id",
                        column: x => x.question_id,
                        principalTable: "quiz_questions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "quiz_question_bank_items",
                columns: table => new
                {
                    question_bank_id = table.Column<long>(type: "bigint", nullable: false),
                    question_id = table.Column<long>(type: "bigint", nullable: false),
                    sort_order = table.Column<int>(type: "integer", nullable: false),
                    added_at = table.Column<DateTimeOffset>(type: "timestamptz", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_quiz_question_bank_items", x => new { x.question_bank_id, x.question_id });
                    table.ForeignKey(
                        name: "fk_quiz_question_bank_items_quiz_question_banks_question_bank_",
                        column: x => x.question_bank_id,
                        principalTable: "quiz_question_banks",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_quiz_question_bank_items_quiz_questions_question_id",
                        column: x => x.question_id,
                        principalTable: "quiz_questions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "quiz_question_options",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    question_id = table.Column<long>(type: "bigint", nullable: false),
                    option_text = table.Column<string>(type: "text", nullable: false),
                    option_pinyin = table.Column<string>(type: "text", nullable: true),
                    is_correct = table.Column<bool>(type: "boolean", nullable: false),
                    sort_order = table.Column<short>(type: "smallint", nullable: false),
                    explanation_vi = table.Column<string>(type: "text", nullable: true),
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
                    table.PrimaryKey("pk_quiz_question_options", x => x.id);
                    table.ForeignKey(
                        name: "fk_quiz_question_options_quiz_questions_question_id",
                        column: x => x.question_id,
                        principalTable: "quiz_questions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "quiz_question_tags",
                columns: table => new
                {
                    question_id = table.Column<long>(type: "bigint", nullable: false),
                    tag_id = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_quiz_question_tags", x => new { x.question_id, x.tag_id });
                    table.ForeignKey(
                        name: "fk_quiz_question_tags_quiz_questions_question_id",
                        column: x => x.question_id,
                        principalTable: "quiz_questions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_quiz_question_tags_quiz_tags_tag_id",
                        column: x => x.tag_id,
                        principalTable: "quiz_tags",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "quiz_attempt_answers",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    attempt_id = table.Column<long>(type: "bigint", nullable: false),
                    question_id = table.Column<long>(type: "bigint", nullable: false),
                    selected_option_id = table.Column<long>(type: "bigint", nullable: true),
                    answer_text = table.Column<string>(type: "text", nullable: true),
                    answer_json = table.Column<string>(type: "jsonb", nullable: true),
                    is_correct = table.Column<bool>(type: "boolean", nullable: true),
                    earned_points = table.Column<decimal>(type: "numeric(8,2)", precision: 8, scale: 2, nullable: true),
                    response_time_ms = table.Column<int>(type: "integer", nullable: true),
                    answered_at = table.Column<DateTimeOffset>(type: "timestamptz", nullable: true),
                    public_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamptz", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamptz", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_quiz_attempt_answers", x => x.id);
                    table.ForeignKey(
                        name: "fk_quiz_attempt_answers_quiz_attempt_attempt_id",
                        column: x => x.attempt_id,
                        principalTable: "quiz_attempts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_quiz_attempt_answers_quiz_question_options_selected_option_",
                        column: x => x.selected_option_id,
                        principalTable: "quiz_question_options",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "fk_quiz_attempt_answers_quiz_questions_question_id",
                        column: x => x.question_id,
                        principalTable: "quiz_questions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ai_requests",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    user_id = table.Column<Guid>(type: "uuid", nullable: true),
                    conversation_id = table.Column<long>(type: "bigint", nullable: true),
                    vocabulary_id = table.Column<long>(type: "bigint", nullable: true),
                    lesson_id = table.Column<long>(type: "bigint", nullable: true),
                    quiz_attempt_answer_id = table.Column<long>(type: "bigint", nullable: true),
                    feature_type = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    provider = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    model = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    request_hash = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    prompt_version = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    input_tokens = table.Column<int>(type: "integer", nullable: false),
                    output_tokens = table.Column<int>(type: "integer", nullable: false),
                    total_tokens = table.Column<int>(type: "integer", nullable: false),
                    estimated_cost_usd = table.Column<decimal>(type: "numeric(12,6)", precision: 12, scale: 6, nullable: true),
                    latency_ms = table.Column<int>(type: "integer", nullable: true),
                    status = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    error_code = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: true),
                    error_message = table.Column<string>(type: "text", nullable: true),
                    requested_at = table.Column<DateTimeOffset>(type: "timestamptz", nullable: false),
                    completed_at = table.Column<DateTimeOffset>(type: "timestamptz", nullable: true),
                    public_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_ai_requests", x => x.id);
                    table.CheckConstraint("ck_ai_requests_cost", "estimated_cost_usd IS NULL OR estimated_cost_usd >= 0");
                    table.CheckConstraint("ck_ai_requests_latency", "latency_ms IS NULL OR latency_ms >= 0");
                    table.CheckConstraint("ck_ai_requests_tokens", "input_tokens >= 0 AND output_tokens >= 0 AND total_tokens >= 0");
                    table.ForeignKey(
                        name: "fk_ai_requests_ai_conversations_conversation_id",
                        column: x => x.conversation_id,
                        principalTable: "ai_conversations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "fk_ai_requests_asp_net_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "fk_ai_requests_lessons_lesson_id",
                        column: x => x.lesson_id,
                        principalTable: "lessons",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "fk_ai_requests_quiz_attempt_answer_quiz_attempt_answer_id",
                        column: x => x.quiz_attempt_answer_id,
                        principalTable: "quiz_attempt_answers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "fk_ai_requests_vocabularies_vocabulary_id",
                        column: x => x.vocabulary_id,
                        principalTable: "vocabularies",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "ai_conversation_messages",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    conversation_id = table.Column<long>(type: "bigint", nullable: false),
                    ai_request_id = table.Column<long>(type: "bigint", nullable: true),
                    role = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    content = table.Column<string>(type: "text", nullable: false),
                    metadata_json = table.Column<string>(type: "jsonb", nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "timestamptz", nullable: false),
                    public_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_ai_conversation_messages", x => x.id);
                    table.ForeignKey(
                        name: "fk_ai_conversation_messages_ai_conversations_conversation_id",
                        column: x => x.conversation_id,
                        principalTable: "ai_conversations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_ai_conversation_messages_ai_requests_ai_request_id",
                        column: x => x.ai_request_id,
                        principalTable: "ai_requests",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "ai_feedback",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    ai_request_id = table.Column<long>(type: "bigint", nullable: false),
                    rating = table.Column<short>(type: "smallint", nullable: false),
                    comment = table.Column<string>(type: "text", nullable: true),
                    issue_type = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "timestamptz", nullable: false),
                    public_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_ai_feedback", x => x.id);
                    table.ForeignKey(
                        name: "fk_ai_feedback_ai_requests_ai_request_id",
                        column: x => x.ai_request_id,
                        principalTable: "ai_requests",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_ai_feedback_asp_net_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_achievements_code",
                table: "achievements",
                column: "code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_achievements_is_active_sort_order",
                table: "achievements",
                columns: new[] { "is_active", "sort_order" });

            migrationBuilder.CreateIndex(
                name: "ix_ai_conversation_messages_ai_request_id",
                table: "ai_conversation_messages",
                column: "ai_request_id");

            migrationBuilder.CreateIndex(
                name: "ix_ai_conversation_messages_conversation_id_created_at",
                table: "ai_conversation_messages",
                columns: new[] { "conversation_id", "created_at" });

            migrationBuilder.CreateIndex(
                name: "ix_ai_conversations_lesson_id",
                table: "ai_conversations",
                column: "lesson_id");

            migrationBuilder.CreateIndex(
                name: "ix_ai_conversations_user_id_last_message_at",
                table: "ai_conversations",
                columns: new[] { "user_id", "last_message_at" });

            migrationBuilder.CreateIndex(
                name: "ix_ai_conversations_user_id_status",
                table: "ai_conversations",
                columns: new[] { "user_id", "status" });

            migrationBuilder.CreateIndex(
                name: "ix_ai_conversations_vocabulary_id",
                table: "ai_conversations",
                column: "vocabulary_id");

            migrationBuilder.CreateIndex(
                name: "ix_ai_feedback_ai_request_id_user_id",
                table: "ai_feedback",
                columns: new[] { "ai_request_id", "user_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_ai_feedback_user_id_created_at",
                table: "ai_feedback",
                columns: new[] { "user_id", "created_at" });

            migrationBuilder.CreateIndex(
                name: "ix_ai_requests_conversation_id",
                table: "ai_requests",
                column: "conversation_id");

            migrationBuilder.CreateIndex(
                name: "ix_ai_requests_feature_type_request_hash",
                table: "ai_requests",
                columns: new[] { "feature_type", "request_hash" });

            migrationBuilder.CreateIndex(
                name: "ix_ai_requests_lesson_id",
                table: "ai_requests",
                column: "lesson_id");

            migrationBuilder.CreateIndex(
                name: "ix_ai_requests_quiz_attempt_answer_id",
                table: "ai_requests",
                column: "quiz_attempt_answer_id");

            migrationBuilder.CreateIndex(
                name: "ix_ai_requests_status_requested_at",
                table: "ai_requests",
                columns: new[] { "status", "requested_at" });

            migrationBuilder.CreateIndex(
                name: "ix_ai_requests_user_id_requested_at",
                table: "ai_requests",
                columns: new[] { "user_id", "requested_at" });

            migrationBuilder.CreateIndex(
                name: "ix_ai_requests_vocabulary_id",
                table: "ai_requests",
                column: "vocabulary_id");

            migrationBuilder.CreateIndex(
                name: "ix_ai_response_cache_cache_key",
                table: "ai_response_cache",
                column: "cache_key",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_ai_response_cache_expires_at",
                table: "ai_response_cache",
                column: "expires_at");

            migrationBuilder.CreateIndex(
                name: "ix_ai_response_cache_feature_type_model_prompt_version",
                table: "ai_response_cache",
                columns: new[] { "feature_type", "model", "prompt_version" });

            migrationBuilder.CreateIndex(
                name: "ix_audio_assets_storage_path",
                table: "audio_assets",
                column: "storage_path",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_audit_logs_entity_type_entity_id",
                table: "audit_logs",
                columns: new[] { "entity_type", "entity_id" });

            migrationBuilder.CreateIndex(
                name: "ix_audit_logs_occurred_at",
                table: "audit_logs",
                column: "occurred_at");

            migrationBuilder.CreateIndex(
                name: "ix_audit_logs_user_id_occurred_at",
                table: "audit_logs",
                columns: new[] { "user_id", "occurred_at" });

            migrationBuilder.CreateIndex(
                name: "ix_content_import_jobs_status_created_at",
                table: "content_import_jobs",
                columns: new[] { "status", "created_at" });

            migrationBuilder.CreateIndex(
                name: "ix_content_import_rows_import_job_id_row_number",
                table: "content_import_rows",
                columns: new[] { "import_job_id", "row_number" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_content_reports_status_entity_type",
                table: "content_reports",
                columns: new[] { "status", "entity_type" });

            migrationBuilder.CreateIndex(
                name: "ix_content_reports_user_id_created_at",
                table: "content_reports",
                columns: new[] { "user_id", "created_at" });

            migrationBuilder.CreateIndex(
                name: "ix_daily_learning_stats_user_id_stat_date",
                table: "daily_learning_stats",
                columns: new[] { "user_id", "stat_date" });

            migrationBuilder.CreateIndex(
                name: "ix_flashcard_session_items_flashcard_session_id_sort_order",
                table: "flashcard_session_items",
                columns: new[] { "flashcard_session_id", "sort_order" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_flashcard_sessions_user_id_started_at",
                table: "flashcard_sessions",
                columns: new[] { "user_id", "started_at" });

            migrationBuilder.CreateIndex(
                name: "ix_flashcard_sessions_user_id_status",
                table: "flashcard_sessions",
                columns: new[] { "user_id", "status" });

            migrationBuilder.CreateIndex(
                name: "ix_hsk_levels_code",
                table: "hsk_levels",
                column: "code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_hsk_levels_sort_order",
                table: "hsk_levels",
                column: "sort_order");

            migrationBuilder.CreateIndex(
                name: "ix_in_app_notifications_expires_at",
                table: "in_app_notifications",
                column: "expires_at");

            migrationBuilder.CreateIndex(
                name: "ix_in_app_notifications_user_id_read_at",
                table: "in_app_notifications",
                columns: new[] { "user_id", "read_at" });

            migrationBuilder.CreateIndex(
                name: "ix_learning_activities_flashcard_session_id",
                table: "learning_activities",
                column: "flashcard_session_id");

            migrationBuilder.CreateIndex(
                name: "ix_learning_activities_lesson_id",
                table: "learning_activities",
                column: "lesson_id");

            migrationBuilder.CreateIndex(
                name: "ix_learning_activities_quiz_attempt_id",
                table: "learning_activities",
                column: "quiz_attempt_id");

            migrationBuilder.CreateIndex(
                name: "ix_learning_activities_user_id_activity_type",
                table: "learning_activities",
                columns: new[] { "user_id", "activity_type" });

            migrationBuilder.CreateIndex(
                name: "ix_learning_activities_user_id_started_at",
                table: "learning_activities",
                columns: new[] { "user_id", "started_at" });

            migrationBuilder.CreateIndex(
                name: "ix_learning_activities_vocabulary_id",
                table: "learning_activities",
                column: "vocabulary_id");

            migrationBuilder.CreateIndex(
                name: "ix_lesson_assets_audio_asset_id",
                table: "lesson_assets",
                column: "audio_asset_id");

            migrationBuilder.CreateIndex(
                name: "ix_lesson_assets_lesson_id_sort_order",
                table: "lesson_assets",
                columns: new[] { "lesson_id", "sort_order" });

            migrationBuilder.CreateIndex(
                name: "ix_lesson_prerequisites_required_lesson_id",
                table: "lesson_prerequisites",
                column: "required_lesson_id");

            migrationBuilder.CreateIndex(
                name: "ix_lesson_sections_lesson_id_sort_order",
                table: "lesson_sections",
                columns: new[] { "lesson_id", "sort_order" },
                unique: true,
                filter: "deleted_at IS NULL");

            migrationBuilder.CreateIndex(
                name: "ix_lesson_vocabularies_lesson_id_sort_order",
                table: "lesson_vocabularies",
                columns: new[] { "lesson_id", "sort_order" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_lesson_vocabularies_vocabulary_id",
                table: "lesson_vocabularies",
                column: "vocabulary_id");

            migrationBuilder.CreateIndex(
                name: "ix_lessons_hsk_level_id_status_sort_order",
                table: "lessons",
                columns: new[] { "hsk_level_id", "status", "sort_order" });

            migrationBuilder.CreateIndex(
                name: "ix_lessons_slug",
                table: "lessons",
                column: "slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_lessons_topic_id_status",
                table: "lessons",
                columns: new[] { "topic_id", "status" });

            migrationBuilder.CreateIndex(
                name: "ix_notification_deliveries_notification_id_channel",
                table: "notification_deliveries",
                columns: new[] { "notification_id", "channel" });

            migrationBuilder.CreateIndex(
                name: "ix_notification_deliveries_status_last_attempt_at",
                table: "notification_deliveries",
                columns: new[] { "status", "last_attempt_at" });

            migrationBuilder.CreateIndex(
                name: "ix_parts_of_speech_code",
                table: "parts_of_speech",
                column: "code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_product_events_event_name_occurred_at",
                table: "product_events",
                columns: new[] { "event_name", "occurred_at" });

            migrationBuilder.CreateIndex(
                name: "ix_product_events_user_id_occurred_at",
                table: "product_events",
                columns: new[] { "user_id", "occurred_at" });

            migrationBuilder.CreateIndex(
                name: "ix_quiz_attempt_answers_attempt_id_question_id",
                table: "quiz_attempt_answers",
                columns: new[] { "attempt_id", "question_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_quiz_attempt_answers_question_id",
                table: "quiz_attempt_answers",
                column: "question_id");

            migrationBuilder.CreateIndex(
                name: "ix_quiz_attempt_answers_selected_option_id",
                table: "quiz_attempt_answers",
                column: "selected_option_id");

            migrationBuilder.CreateIndex(
                name: "ix_quiz_attempt_questions_attempt_id_sort_order",
                table: "quiz_attempt_questions",
                columns: new[] { "attempt_id", "sort_order" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_quiz_attempt_questions_question_id",
                table: "quiz_attempt_questions",
                column: "question_id");

            migrationBuilder.CreateIndex(
                name: "ix_quiz_attempts_idempotency_key",
                table: "quiz_attempts",
                column: "idempotency_key",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_quiz_attempts_quiz_id",
                table: "quiz_attempts",
                column: "quiz_id");

            migrationBuilder.CreateIndex(
                name: "ix_quiz_attempts_user_id_quiz_id_attempt_number",
                table: "quiz_attempts",
                columns: new[] { "user_id", "quiz_id", "attempt_number" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_quiz_attempts_user_id_started_at",
                table: "quiz_attempts",
                columns: new[] { "user_id", "started_at" });

            migrationBuilder.CreateIndex(
                name: "ix_quiz_matching_pairs_question_id_sort_order",
                table: "quiz_matching_pairs",
                columns: new[] { "question_id", "sort_order" },
                unique: true,
                filter: "deleted_at IS NULL");

            migrationBuilder.CreateIndex(
                name: "ix_quiz_question_bank_items_question_bank_id_sort_order",
                table: "quiz_question_bank_items",
                columns: new[] { "question_bank_id", "sort_order" });

            migrationBuilder.CreateIndex(
                name: "ix_quiz_question_bank_items_question_id",
                table: "quiz_question_bank_items",
                column: "question_id");

            migrationBuilder.CreateIndex(
                name: "ix_quiz_question_banks_code",
                table: "quiz_question_banks",
                column: "code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_quiz_question_banks_hsk_level_id_is_active",
                table: "quiz_question_banks",
                columns: new[] { "hsk_level_id", "is_active" });

            migrationBuilder.CreateIndex(
                name: "ix_quiz_question_options_question_id_sort_order",
                table: "quiz_question_options",
                columns: new[] { "question_id", "sort_order" },
                unique: true,
                filter: "deleted_at IS NULL");

            migrationBuilder.CreateIndex(
                name: "ix_quiz_question_tags_tag_id",
                table: "quiz_question_tags",
                column: "tag_id");

            migrationBuilder.CreateIndex(
                name: "ix_quiz_questions_quiz_id_sort_order",
                table: "quiz_questions",
                columns: new[] { "quiz_id", "sort_order" },
                unique: true,
                filter: "deleted_at IS NULL");

            migrationBuilder.CreateIndex(
                name: "ix_quiz_questions_vocabulary_id",
                table: "quiz_questions",
                column: "vocabulary_id");

            migrationBuilder.CreateIndex(
                name: "ix_quiz_tags_slug",
                table: "quiz_tags",
                column: "slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_quizzes_lesson_id_status",
                table: "quizzes",
                columns: new[] { "lesson_id", "status" });

            migrationBuilder.CreateIndex(
                name: "ix_refresh_tokens_expires_at",
                table: "refresh_tokens",
                column: "expires_at");

            migrationBuilder.CreateIndex(
                name: "ix_refresh_tokens_family_id",
                table: "refresh_tokens",
                column: "family_id");

            migrationBuilder.CreateIndex(
                name: "ix_refresh_tokens_token_hash",
                table: "refresh_tokens",
                column: "token_hash",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_refresh_tokens_user_id_revoked_at",
                table: "refresh_tokens",
                columns: new[] { "user_id", "revoked_at" });

            migrationBuilder.CreateIndex(
                name: "ix_refresh_tokens_user_session_id",
                table: "refresh_tokens",
                column: "user_session_id");

            migrationBuilder.CreateIndex(
                name: "ix_review_events_user_id_reviewed_at",
                table: "review_events",
                columns: new[] { "user_id", "reviewed_at" });

            migrationBuilder.CreateIndex(
                name: "ix_review_events_user_id_vocabulary_id_reviewed_at",
                table: "review_events",
                columns: new[] { "user_id", "vocabulary_id", "reviewed_at" });

            migrationBuilder.CreateIndex(
                name: "ix_role_claims_role_id",
                table: "role_claims",
                column: "role_id");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "roles",
                column: "normalized_name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_topics_slug",
                table: "topics",
                column: "slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_topics_status_sort_order",
                table: "topics",
                columns: new[] { "status", "sort_order" });

            migrationBuilder.CreateIndex(
                name: "ix_user_achievements_user_id_unlocked_at",
                table: "user_achievements",
                columns: new[] { "user_id", "unlocked_at" });

            migrationBuilder.CreateIndex(
                name: "ix_user_blocked_sessions_user_id_blocked_at",
                table: "user_blocked_sessions",
                columns: new[] { "user_id", "blocked_at" });

            migrationBuilder.CreateIndex(
                name: "ix_user_blocked_sessions_user_session_id",
                table: "user_blocked_sessions",
                column: "user_session_id");

            migrationBuilder.CreateIndex(
                name: "ix_user_claims_user_id",
                table: "user_claims",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_user_consents_user_id_consent_type_version",
                table: "user_consents",
                columns: new[] { "user_id", "consent_type", "version" });

            migrationBuilder.CreateIndex(
                name: "ix_user_data_export_jobs_user_id_status",
                table: "user_data_export_jobs",
                columns: new[] { "user_id", "status" });

            migrationBuilder.CreateIndex(
                name: "ix_user_learning_goals_user_id_status",
                table: "user_learning_goals",
                columns: new[] { "user_id", "status" });

            migrationBuilder.CreateIndex(
                name: "ix_user_lesson_bookmarks_lesson_id",
                table: "user_lesson_bookmarks",
                column: "lesson_id");

            migrationBuilder.CreateIndex(
                name: "ix_user_lesson_bookmarks_user_id_created_at",
                table: "user_lesson_bookmarks",
                columns: new[] { "user_id", "created_at" });

            migrationBuilder.CreateIndex(
                name: "ix_user_lesson_progress_lesson_id",
                table: "user_lesson_progress",
                column: "lesson_id");

            migrationBuilder.CreateIndex(
                name: "ix_user_lesson_progress_user_id_status",
                table: "user_lesson_progress",
                columns: new[] { "user_id", "status" });

            migrationBuilder.CreateIndex(
                name: "ix_user_lesson_section_progress_lesson_section_id",
                table: "user_lesson_section_progress",
                column: "lesson_section_id");

            migrationBuilder.CreateIndex(
                name: "ix_user_login_histories_user_id_attempted_at",
                table: "user_login_histories",
                columns: new[] { "user_id", "attempted_at" });

            migrationBuilder.CreateIndex(
                name: "ix_user_logins_user_id",
                table: "user_logins",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_user_preferences_user_id",
                table: "user_preferences",
                column: "user_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_user_profiles_user_id",
                table: "user_profiles",
                column: "user_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_user_roles_role_id",
                table: "user_roles",
                column: "role_id");

            migrationBuilder.CreateIndex(
                name: "ix_user_security_events_user_id_occurred_at",
                table: "user_security_events",
                columns: new[] { "user_id", "occurred_at" });

            migrationBuilder.CreateIndex(
                name: "ix_user_sessions_session_key",
                table: "user_sessions",
                column: "session_key",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_user_sessions_user_id_last_activity_at",
                table: "user_sessions",
                columns: new[] { "user_id", "last_activity_at" });

            migrationBuilder.CreateIndex(
                name: "ix_user_sessions_user_id_status",
                table: "user_sessions",
                columns: new[] { "user_id", "status" });

            migrationBuilder.CreateIndex(
                name: "ix_user_vocabulary_notes_user_id_vocabulary_id",
                table: "user_vocabulary_notes",
                columns: new[] { "user_id", "vocabulary_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_user_vocabulary_notes_vocabulary_id",
                table: "user_vocabulary_notes",
                column: "vocabulary_id");

            migrationBuilder.CreateIndex(
                name: "ix_user_vocabulary_states_user_id_learning_state",
                table: "user_vocabulary_states",
                columns: new[] { "user_id", "learning_state" });

            migrationBuilder.CreateIndex(
                name: "ix_user_vocabulary_states_user_id_next_review_at",
                table: "user_vocabulary_states",
                columns: new[] { "user_id", "next_review_at" });

            migrationBuilder.CreateIndex(
                name: "ix_user_vocabulary_states_vocabulary_id",
                table: "user_vocabulary_states",
                column: "vocabulary_id");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "users",
                column: "normalized_email");

            migrationBuilder.CreateIndex(
                name: "ix_users_public_id",
                table: "users",
                column: "public_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "users",
                column: "normalized_user_name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_vocabularies_audio_asset_id",
                table: "vocabularies",
                column: "audio_asset_id");

            migrationBuilder.CreateIndex(
                name: "ix_vocabularies_hsk_level_id_status",
                table: "vocabularies",
                columns: new[] { "hsk_level_id", "status" });

            migrationBuilder.CreateIndex(
                name: "ix_vocabularies_part_of_speech_id",
                table: "vocabularies",
                column: "part_of_speech_id");

            migrationBuilder.CreateIndex(
                name: "ix_vocabularies_pinyin_normalized",
                table: "vocabularies",
                column: "pinyin_normalized");

            migrationBuilder.CreateIndex(
                name: "ix_vocabularies_simplified",
                table: "vocabularies",
                column: "simplified");

            migrationBuilder.CreateIndex(
                name: "ix_vocabularies_simplified_pinyin_normalized_hsk_level_id",
                table: "vocabularies",
                columns: new[] { "simplified", "pinyin_normalized", "hsk_level_id" },
                unique: true,
                filter: "deleted_at IS NULL");

            migrationBuilder.CreateIndex(
                name: "ix_vocabularies_topic_id",
                table: "vocabularies",
                column: "topic_id");

            migrationBuilder.CreateIndex(
                name: "ix_vocabularies_traditional",
                table: "vocabularies",
                column: "traditional");

            migrationBuilder.CreateIndex(
                name: "ix_vocabulary_examples_audio_asset_id",
                table: "vocabulary_examples",
                column: "audio_asset_id");

            migrationBuilder.CreateIndex(
                name: "ix_vocabulary_examples_vocabulary_id_status",
                table: "vocabulary_examples",
                columns: new[] { "vocabulary_id", "status" });

            migrationBuilder.CreateIndex(
                name: "ix_vocabulary_meanings_vocabulary_id_sense_order",
                table: "vocabulary_meanings",
                columns: new[] { "vocabulary_id", "sense_order" },
                unique: true,
                filter: "deleted_at IS NULL");

            migrationBuilder.CreateIndex(
                name: "ix_vocabulary_relations_related_vocabulary_id",
                table: "vocabulary_relations",
                column: "related_vocabulary_id");

            migrationBuilder.CreateIndex(
                name: "ix_vocabulary_relations_vocabulary_id_related_vocabulary_id_re",
                table: "vocabulary_relations",
                columns: new[] { "vocabulary_id", "related_vocabulary_id", "relation_type" },
                unique: true,
                filter: "deleted_at IS NULL");

            migrationBuilder.CreateIndex(
                name: "ix_xp_transactions_user_id_created_at",
                table: "xp_transactions",
                columns: new[] { "user_id", "created_at" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "achievements");

            migrationBuilder.DropTable(
                name: "ai_conversation_messages");

            migrationBuilder.DropTable(
                name: "ai_feedback");

            migrationBuilder.DropTable(
                name: "ai_response_cache");

            migrationBuilder.DropTable(
                name: "audit_logs");

            migrationBuilder.DropTable(
                name: "content_import_rows");

            migrationBuilder.DropTable(
                name: "content_reports");

            migrationBuilder.DropTable(
                name: "daily_learning_stats");

            migrationBuilder.DropTable(
                name: "flashcard_session_items");

            migrationBuilder.DropTable(
                name: "learning_activities");

            migrationBuilder.DropTable(
                name: "lesson_assets");

            migrationBuilder.DropTable(
                name: "lesson_prerequisites");

            migrationBuilder.DropTable(
                name: "lesson_vocabularies");

            migrationBuilder.DropTable(
                name: "notification_deliveries");

            migrationBuilder.DropTable(
                name: "notification_preferences");

            migrationBuilder.DropTable(
                name: "product_events");

            migrationBuilder.DropTable(
                name: "quiz_attempt_questions");

            migrationBuilder.DropTable(
                name: "quiz_matching_pairs");

            migrationBuilder.DropTable(
                name: "quiz_question_bank_items");

            migrationBuilder.DropTable(
                name: "quiz_question_tags");

            migrationBuilder.DropTable(
                name: "refresh_tokens");

            migrationBuilder.DropTable(
                name: "review_events");

            migrationBuilder.DropTable(
                name: "role_claims");

            migrationBuilder.DropTable(
                name: "user_achievements");

            migrationBuilder.DropTable(
                name: "user_blocked_sessions");

            migrationBuilder.DropTable(
                name: "user_claims");

            migrationBuilder.DropTable(
                name: "user_consents");

            migrationBuilder.DropTable(
                name: "user_data_export_jobs");

            migrationBuilder.DropTable(
                name: "user_learning_goals");

            migrationBuilder.DropTable(
                name: "user_learning_summaries");

            migrationBuilder.DropTable(
                name: "user_lesson_bookmarks");

            migrationBuilder.DropTable(
                name: "user_lesson_progress");

            migrationBuilder.DropTable(
                name: "user_lesson_section_progress");

            migrationBuilder.DropTable(
                name: "user_login_histories");

            migrationBuilder.DropTable(
                name: "user_logins");

            migrationBuilder.DropTable(
                name: "user_preferences");

            migrationBuilder.DropTable(
                name: "user_profiles");

            migrationBuilder.DropTable(
                name: "user_roles");

            migrationBuilder.DropTable(
                name: "user_security_events");

            migrationBuilder.DropTable(
                name: "user_streaks");

            migrationBuilder.DropTable(
                name: "user_tokens");

            migrationBuilder.DropTable(
                name: "user_vocabulary_notes");

            migrationBuilder.DropTable(
                name: "user_vocabulary_states");

            migrationBuilder.DropTable(
                name: "vocabulary_examples");

            migrationBuilder.DropTable(
                name: "vocabulary_meanings");

            migrationBuilder.DropTable(
                name: "vocabulary_relations");

            migrationBuilder.DropTable(
                name: "xp_transactions");

            migrationBuilder.DropTable(
                name: "ai_requests");

            migrationBuilder.DropTable(
                name: "content_import_jobs");

            migrationBuilder.DropTable(
                name: "flashcard_sessions");

            migrationBuilder.DropTable(
                name: "in_app_notifications");

            migrationBuilder.DropTable(
                name: "quiz_question_banks");

            migrationBuilder.DropTable(
                name: "quiz_tags");

            migrationBuilder.DropTable(
                name: "user_sessions");

            migrationBuilder.DropTable(
                name: "lesson_sections");

            migrationBuilder.DropTable(
                name: "roles");

            migrationBuilder.DropTable(
                name: "ai_conversations");

            migrationBuilder.DropTable(
                name: "quiz_attempt_answers");

            migrationBuilder.DropTable(
                name: "quiz_attempts");

            migrationBuilder.DropTable(
                name: "quiz_question_options");

            migrationBuilder.DropTable(
                name: "users");

            migrationBuilder.DropTable(
                name: "quiz_questions");

            migrationBuilder.DropTable(
                name: "quizzes");

            migrationBuilder.DropTable(
                name: "vocabularies");

            migrationBuilder.DropTable(
                name: "lessons");

            migrationBuilder.DropTable(
                name: "audio_assets");

            migrationBuilder.DropTable(
                name: "parts_of_speech");

            migrationBuilder.DropTable(
                name: "hsk_levels");

            migrationBuilder.DropTable(
                name: "topics");
        }
    }
}
