using System.Text;
using HanYu.Application.Interfaces.Course;
using HanYu.Infrastructure.Course;
using HanYu.Infrastructure.Options;
using HanYu.Infrastructure.Observability;
using HanYu.Infrastructure.Security;
using Amazon;
using Amazon.S3;
using HanYu.Application.Features.Identity.Login;
using HanYu.Application.Features.Identity.Logout;
using HanYu.Application.Features.Identity.Me;
using HanYu.Application.Features.Identity.Admin.Users.GetUsers;
using HanYu.Application.Features.Identity.Admin.Users.GetUserById;
using HanYu.Application.Features.Identity.Admin.Roles.GetRoles;
using HanYu.Application.Features.Identity.Admin.Roles.GetRoleById;
using HanYu.Application.Features.Identity.Admin.Roles.CreateRole;
using HanYu.Application.Features.Identity.Admin.Roles.UpdateRole;
using HanYu.Application.Features.Identity.Admin.Roles.DeleteRole;
using HanYu.Application.Features.Identity.RefreshToken;
using HanYu.Application.Features.Identity.Register;
using HanYu.Application.Features.Identity.VerifyEmail;
using HanYu.Application.Features.Identity.ForgotPassword;
using HanYu.Application.Features.Identity.ResetPassword;
using HanYu.Application.Features.Identity.ChangePassword;
using HanYu.Application.Features.Identity.ResendVerificationEmail;
using HanYu.Application.Features.Identity.DataPrivacy;
using HanYu.Application.Features.Identity.Phone;
using HanYu.Application.Features.Identity.Preferences.GetPreferences;
using HanYu.Application.Features.Identity.Preferences.ResetPreferences;
using HanYu.Application.Features.Identity.Preferences.UpdateAudio;
using HanYu.Application.Features.Identity.Preferences.UpdateDisplay;
using HanYu.Application.Features.Identity.Preferences.UpdateFlashcardMode;
using HanYu.Application.Features.Identity.Preferences.UpdateTheme;
using HanYu.Application.Features.Identity.Profile.GetProfile;
using HanYu.Application.Features.Identity.Account;
using HanYu.Application.Features.Identity.Profile.Onboarding;
using HanYu.Application.Features.Identity.Profile.UpdateProfile;
using HanYu.Application.Features.Identity.RevokeSession;
using HanYu.Application.Features.Identity.SecurityEvents;
using HanYu.Application.Features.Identity.Sessions;
using HanYu.Application.Features.Identity.TwoFactor.Disable;
using HanYu.Application.Features.Identity.TwoFactor.Enable;
using HanYu.Application.Features.Identity.TwoFactor.RecoveryCodes;
using HanYu.Application.Features.Identity.TwoFactor.RegenerateKey;
using HanYu.Application.Features.Identity.TwoFactor.Setup;
using HanYu.Application.Features.Identity.TwoFactor.Login;
using HanYu.Application.Interfaces.Authentication;
using HanYu.Application.Interfaces.DateTime;
using HanYu.Application.Interfaces.Messaging;
using HanYu.Application.Interfaces.Persistence;
using HanYu.Application.Interfaces.Storage;
using HanYu.Application.Interfaces.Learning;
using HanYu.Application.Interfaces.Email;
using HanYu.Application.Interfaces.Identity;
using HanYu.Application.Interfaces.Operations;
using HanYu.Domain.Entities.Identity;
using HanYu.Infrastructure.BackgroundJobs.DataExport;
using HanYu.Infrastructure.ExternalServices.Email;
using HanYu.Infrastructure.Identity;
using HanYu.Infrastructure.Authentication;
using HanYu.Infrastructure.Operations;
using HanYu.Infrastructure.Learning;
using HanYu.Infrastructure.Messaging.Sms;
using HanYu.Infrastructure.Storage;
using HanYu.Infrastructure.Persistence;
using HanYu.Infrastructure.Persistence.Seeding;
using HanYu.Infrastructure.Persistence.Seeding.Identity;
using HanYu.Infrastructure.Services;
using HanYu.Domain.Constants;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using HanYu.Application.Interfaces.Caching;
using HanYu.Infrastructure.Caching;
using Microsoft.Extensions.Hosting;

namespace HanYu.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        AddSecurityOptions(
            services,
            configuration);

        AddPersistence(
            services,
            configuration);

        AddIdentity(
            services);

        AddAuthentication(
            services,
            configuration);

        AddEmail(
            services,
            configuration);

        AddSms(
            services,
            configuration);

        AddStorage(
            services,
            configuration);

        AddFoundationServices(
            services);

        AddInfrastructureServices(
            services);

        AddFeatureServices(
            services);

        AddVocabulary(
            services,
            configuration);

        AddLesson(
            services,
            configuration);

        AddReview(
            services);

        AddQuiz(
            services);

        AddNotification(
            services);

        AddGamification(
            services);

        AddContent(
            services);

        AddAnalytics(
            services);

        AddAi(
            services);

        AddOperations(
            services);

        AddBackgroundJobs(
            services);

        AddDevelopmentSeeding(
            services,
            configuration);

        // Network security: CORS, ForwardedHeaders, HTTPS
        services.AddHanYuNetworkSecurity(configuration);

        // Tiered rate limiting by endpoint type
        services.AddHanYuRateLimiting();

        // Health checks: /health/live + /health/ready
        services.AddHanYuHealthChecks(configuration);

        return services;
    }

    /// <summary>
    /// Overload that also registers OpenTelemetry (requires IHostEnvironment for env-aware config).
    /// Call this from Program.cs instead of the base overload.
    /// </summary>
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration,
        IHostEnvironment environment)
    {
        services.AddInfrastructure(configuration);

        // Distributed tracing + Prometheus metrics
        services.AddHanYuOpenTelemetry(configuration, environment);

        return services;
    }

    // ============================================================
    // Persistence
    // ============================================================

    private static void AddPersistence(
        IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString =
            configuration.GetConnectionString(
                "DefaultConnection")
            ?? throw new InvalidOperationException(
                "Connection string 'DefaultConnection' was not found.");

        var dbOptions = configuration
            .GetSection(DatabaseOptions.SectionName)
            .Get<DatabaseOptions>() ?? new DatabaseOptions();

        services.AddDbContext<HanYuDbContext>(
            options =>
            {
                options
                    .UseNpgsql(
                        connectionString,
                        npgsql =>
                        {
                            // Retry on transient failures (network blips, connection resets).
                            // Do NOT use retry for non-idempotent operations; those are wrapped
                            // in idempotency keys at the application level.
                            npgsql.EnableRetryOnFailure(
                                maxRetryCount: dbOptions.MaxRetryCount,
                                maxRetryDelay: TimeSpan.FromSeconds(
                                    dbOptions.MaxRetryDelaySeconds),
                                errorCodesToAdd: null);

                            npgsql.CommandTimeout(
                                dbOptions.CommandTimeoutSeconds);
                        })
                    .UseSnakeCaseNamingConvention()
                    .ConfigureWarnings(warnings =>
                        warnings.Ignore(
                            Microsoft.EntityFrameworkCore.Diagnostics
                                .CoreEventId
                                .PossibleIncorrectRequiredNavigationWithQueryFilterInteractionWarning));
            });

        services.AddScoped<IHanYuDbContext>(
            provider =>
                provider.GetRequiredService<HanYuDbContext>());
    }

    private static void AddDevelopmentSeeding(
        IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<IdentitySeedOptions>(
            configuration.GetSection(
                IdentitySeedOptions.SectionName));

        services.AddScoped<IdentitySeeder>();
    }

    // ============================================================
    // ASP.NET Core Identity
    // ============================================================

    private static void AddIdentity(
        IServiceCollection services)
    {
        services
            .AddIdentityCore<User>(
                options =>
                {
                    // =========================
                    // User
                    // =========================

                    options.User.RequireUniqueEmail = true;

                    options.User.AllowedUserNameCharacters =
                        "abcdefghijklmnopqrstuvwxyz" +
                        "ABCDEFGHIJKLMNOPQRSTUVWXYZ" +
                        "0123456789-._@+";

                    // =========================
                    // Password
                    // =========================

                    // Production hardening: minimum 12 chars per OWASP 2025 guidance.
                    // Ref: https://cheatsheetseries.owasp.org/cheatsheets/Authentication_Cheat_Sheet.html
                    options.Password.RequiredLength = 12;

                    options.Password.RequiredUniqueChars = 1;

                    options.Password.RequireDigit = true;

                    options.Password.RequireLowercase = true;

                    options.Password.RequireUppercase = true;

                    options.Password.RequireNonAlphanumeric = false;

                    // =========================
                    // Lockout
                    // =========================

                    options.Lockout.AllowedForNewUsers = true;

                    options.Lockout.MaxFailedAccessAttempts = 5;

                    options.Lockout.DefaultLockoutTimeSpan =
                        TimeSpan.FromMinutes(15);

                    // =========================
                    // SignIn
                    // =========================

                    // Hiện tại có thể login ngay sau khi register.
                    // Khi feature VerifyEmail hoàn thành thì đổi thành true.
                    options.SignIn.RequireConfirmedEmail = false;

                    options.SignIn.RequireConfirmedPhoneNumber = false;

                    options.SignIn.RequireConfirmedAccount = false;
                })
            .AddRoles<Role>()
            .AddEntityFrameworkStores<HanYuDbContext>()
            .AddDefaultTokenProviders();

        services.AddScoped<
            IIdentityService,
            IdentityService>();
    }

    // ============================================================
    // JWT Authentication
    // ============================================================

    private static void AddAuthentication(
        IServiceCollection services,
        IConfiguration configuration)
    {
        var jwtSection =
            configuration.GetSection(
                JwtOptions.SectionName);

        services.Configure<JwtOptions>(
            jwtSection);

        var jwtOptions =
            jwtSection.Get<JwtOptions>()
            ?? throw new InvalidOperationException(
                "JWT configuration was not found.");

        ValidateJwtOptions(jwtOptions);

        var signingKey =
            new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(
                    jwtOptions.SecretKey));

        services
            .AddAuthentication(
                options =>
                {
                    options.DefaultAuthenticateScheme =
                        JwtBearerDefaults.AuthenticationScheme;

                    options.DefaultChallengeScheme =
                        JwtBearerDefaults.AuthenticationScheme;

                    options.DefaultScheme =
                        JwtBearerDefaults.AuthenticationScheme;
                })
            .AddJwtBearer(
                options =>
                {
                    options.RequireHttpsMetadata = true;

                    options.SaveToken = false;

                    options.TokenValidationParameters =
                        new TokenValidationParameters
                        {
                            // =========================
                            // Issuer
                            // =========================

                            ValidateIssuer = true,

                            ValidIssuer =
                                jwtOptions.Issuer,

                            // =========================
                            // Audience
                            // =========================

                            ValidateAudience = true,

                            ValidAudience =
                                jwtOptions.Audience,

                            // =========================
                            // Signature
                            // =========================

                            ValidateIssuerSigningKey = true,

                            IssuerSigningKey =
                                signingKey,

                            // =========================
                            // Lifetime
                            // =========================

                            ValidateLifetime = true,

                            RequireExpirationTime = true,

                            ClockSkew =
                                TimeSpan.FromSeconds(30)
                        };
                });

        services.AddAuthorization(options =>
        {
            options.AddPolicy(
                Policies.AdminOnly,
                policy =>
                {
                    policy.RequireAuthenticatedUser();
                    policy.RequireRole(Roles.SuperAdmin, Roles.Admin);
                });

            options.AddPolicy(
                Policies.AuthenticatedUser,
                policy =>
                {
                    policy.RequireAuthenticatedUser();
                });
        });
    }

    // ============================================================
    // Infrastructure Services
    // ============================================================

    private static void AddSms(
        IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<SmsOptions>(
            configuration.GetSection(
                SmsOptions.SectionName));

        services.AddScoped<
            ISmsService,
            TwilioSmsService>();
    }

    private static void AddStorage(
        IServiceCollection services,
        IConfiguration configuration)
    {
        var section =
            configuration.GetSection(
                StorageOptions.SectionName);

        services.Configure<StorageOptions>(
            section);

        var options =
            section.Get<StorageOptions>()
            ?? throw new InvalidOperationException(
                "Storage configuration missing.");

        services.AddSingleton<IAmazonS3>(_ =>
        {
            var config =
                new AmazonS3Config();

            if (!string.IsNullOrWhiteSpace(
                    options.ServiceUrl))
            {
                config.ServiceURL =
                    options.ServiceUrl;

                config.ForcePathStyle =
                    options.ForcePathStyle;
            }
            else
            {
                config.RegionEndpoint =
                    RegionEndpoint.GetBySystemName(
                        options.Region);
            }

            return new AmazonS3Client(config);
        });

        services.AddScoped<
            IPrivateFileStorage,
            S3PrivateFileStorage>();
    }

    private static void AddBackgroundJobs(
        IServiceCollection services)
    {
        services.AddScoped<
            IIdentityDataExportBuilder,
            IdentityDataExportBuilder>();

        services.AddScoped<
            UserDataExportProcessor>();

        services.AddHostedService<
            UserDataExportWorker>();

        // Periodic cleanup: expired tokens, notifications, stale quiz attempts
        services.AddHostedService<
            HanYu.Infrastructure.BackgroundJobs.Cleanup.CleanupWorker>();
    }

    private static void AddInfrastructureServices(
        IServiceCollection services)
    {
        services.AddHttpContextAccessor();

        services.AddScoped<
            IJwtTokenService,
            JwtTokenService>();

        services.AddScoped<
            ICurrentUserService,
            CurrentUserService>();

        services.AddScoped<
            IIdentitySessionService,
            IdentitySessionService>();

        services.AddScoped<
            ISecurityEventService,
            SecurityEventService>();

        services.AddScoped<
            IAuthenticationTokenIssuer,
            AuthenticationTokenIssuer>();

        services.AddScoped<
            ITwoFactorService,
            TwoFactorService>();

        services.AddScoped<
            IUserProfileService,
            UserProfileService>();

        services.AddScoped<
            IUserPreferenceService,
            UserPreferenceService>();

        services.AddScoped<
            IAccountService,
            AccountService>();

        services.AddScoped<
            IPhoneService,
            PhoneService>();

        services.AddScoped<
            IDataPrivacyService,
            DataPrivacyService>();

        // Hiện UserManager<User> đã xử lý password hashing.
        // Vẫn đăng ký PasswordService nếu các feature khác cần dùng abstraction này.
        services.AddScoped<
            IPasswordService,
            PasswordService>();

        services.AddSingleton<
            IDateTimeProvider,
            DateTimeProvider>();

        services.AddScoped<
            IAdminUserService,
            AdminUserService>();

        services.AddScoped<
            ISessionService,
            SessionService>();

        services.AddScoped<
            IAuditService,
            AuditService>();
    }

    // ============================================================
    // Application Feature Services
    // ============================================================

    private static void AddFeatureServices(
        IServiceCollection services)
    {
        // =========================
        // Identity / Authentication
        // =========================

        services.AddScoped<RegisterService>();
        services.AddScoped<LoginService>();
        services.AddScoped<RefreshTokenService>();
        services.AddScoped<LogoutService>();
        services.AddScoped<GetCurrentUserService>();

        // Course Services
        services.AddScoped<
            HanYu.Application.Interfaces.Caching.ICourseCacheInvalidator,
            HanYu.Infrastructure.Course.CourseCacheInvalidator>();
        services.AddScoped<IAdminCourseService, AdminCourseService>();
        services.AddScoped<IPublicCourseService, PublicCourseService>();

        // Admin Users
        services.AddScoped<GetUsersHandler>();
        services.AddScoped<GetUserByIdHandler>();
        services.AddScoped<HanYu.Application.Features.Identity.Admin.Users.LockUser.LockUserHandler>();
        services.AddScoped<HanYu.Application.Features.Identity.Admin.Users.UnlockUser.UnlockUserHandler>();
        services.AddScoped<HanYu.Application.Features.Identity.Admin.Users.UpdateUser.UpdateUserHandler>();
        services.AddScoped<HanYu.Application.Features.Identity.Admin.Users.DeleteUser.DeleteUserHandler>();
        services.AddScoped<HanYu.Application.Features.Identity.Admin.Users.RestoreUser.RestoreUserHandler>();
        services.AddScoped<HanYu.Application.Features.Identity.Admin.Users.UpdateRoles.UpdateUserRolesHandler>();

        // Admin Roles
        services.AddScoped<GetRolesHandler>();
        services.AddScoped<GetRoleByIdHandler>();
        services.AddScoped<CreateRoleHandler>();
        services.AddScoped<UpdateRoleHandler>();
        services.AddScoped<DeleteRoleHandler>();

        // Admin Sessions
        services.AddScoped<HanYu.Application.Features.Identity.Admin.Sessions.GetSessionsHandler>();
        services.AddScoped<HanYu.Application.Features.Identity.Admin.Sessions.GetSessionByIdHandler>();
        services.AddScoped<HanYu.Application.Features.Identity.Admin.Sessions.RevokeSessionHandler>();

        services.AddScoped<VerifyEmailService>();
        services.AddScoped<ResendVerificationEmailService>();
        services.AddScoped<ForgotPasswordService>();
        services.AddScoped<ResetPasswordService>();
        services.AddScoped<ChangePasswordService>();

        services.AddScoped<GetSessionsService>();
        services.AddScoped<GetCurrentSessionService>();
        services.AddScoped<RevokeSessionService>();
        services.AddScoped<RevokeAllOtherSessionsService>();
        services.AddScoped<GetSecurityEventsService>();
        services.AddScoped<LogSecurityEventService>();

        // =========================
        // Two Factor Authentication
        // =========================

        services.AddScoped<SetupTwoFactorService>();
        services.AddScoped<EnableTwoFactorService>();
        services.AddScoped<DisableTwoFactorService>();
        services.AddScoped<TwoFactorLoginService>();
        services.AddScoped<GenerateRecoveryCodesService>();
        services.AddScoped<RegenerateAuthenticatorKeyService>();

        // =========================
        // User Profile
        // =========================

        services.AddScoped<GetUserProfileService>();
        services.AddScoped<UpdateUserProfileService>();
        services.AddScoped<CompleteOnboardingService>();

        // =========================
        // User Preferences
        // =========================

        services.AddScoped<GetUserPreferencesService>();
        services.AddScoped<UpdateDisplayPreferencesService>();
        services.AddScoped<UpdateAudioPreferencesService>();
        services.AddScoped<UpdateThemeService>();
        services.AddScoped<UpdateFlashcardModeService>();
        services.AddScoped<ResetUserPreferencesService>();

        // Account
        services.AddScoped<ChangeEmailService>();
        services.AddScoped<ChangeUsernameService>();
        services.AddScoped<DeleteAccountService>();
        services.AddScoped<RestoreAccountService>();


        // Phone
        services.AddScoped<UpdatePhoneNumberService>();
        services.AddScoped<VerifyPhoneNumberService>();
        services.AddScoped<SendPhoneVerificationService>();

        // Privacy
        services.AddScoped<GetConsentsService>();
        services.AddScoped<UpdateConsentService>();
        services.AddScoped<RequestDataExportService>();
        services.AddScoped<GetDataExportsService>();
        services.AddScoped<RequestAccountDeletionService>();
        services.AddScoped<GetDataExportDownloadService>();
    }

    // ============================================================
    // Validation
    // ============================================================

    private static void ValidateJwtOptions(
        JwtOptions jwtOptions)
    {
        if (string.IsNullOrWhiteSpace(
                jwtOptions.SecretKey))
        {
            throw new InvalidOperationException(
                "JWT SecretKey is required.");
        }

        if (Encoding.UTF8.GetByteCount(
                jwtOptions.SecretKey) < 32)
        {
            throw new InvalidOperationException(
                "JWT SecretKey must be at least 32 bytes long.");
        }

        if (string.IsNullOrWhiteSpace(
                jwtOptions.Issuer))
        {
            throw new InvalidOperationException(
                "JWT Issuer is required.");
        }

        if (string.IsNullOrWhiteSpace(
                jwtOptions.Audience))
        {
            throw new InvalidOperationException(
                "JWT Audience is required.");
        }

        if (jwtOptions.AccessTokenExpirationMinutes <= 0)
        {
            throw new InvalidOperationException(
                "JWT AccessTokenExpirationMinutes must be greater than 0.");
        }

        if (jwtOptions.RefreshTokenExpirationDays <= 0)
        {
            throw new InvalidOperationException(
                "JWT RefreshTokenExpirationDays must be greater than 0.");
        }
    }

    // ─── Security / Options Validation ───────────────────────────────────────────

    private static void AddSecurityOptions(
        IServiceCollection services,
        IConfiguration configuration)
    {
        // SecurityOptions: CORS origins, proxy trust, request size limits
        services
            .AddOptions<SecurityOptions>()
            .Bind(configuration.GetSection(SecurityOptions.SectionName))
            .ValidateOnStart(); // Fail fast on bad config at startup

        // DatabaseOptions: pool size, timeouts, retry counts
        services
            .AddOptions<DatabaseOptions>()
            .Bind(configuration.GetSection(DatabaseOptions.SectionName))
            .Validate(
                opts =>
                    opts.MaxPoolSize > 0 &&
                    opts.CommandTimeoutSeconds > 0 &&
                    opts.MaxRetryCount >= 0,
                "DatabaseOptions has invalid values.")
            .ValidateOnStart();

        // JwtOptions: already validated eagerly in AddAuthentication.
        // Also register via IOptions for typed injection throughout the app.
        services
            .AddOptions<JwtOptions>()
            .Bind(configuration.GetSection(JwtOptions.SectionName))
            .Validate(
                opts =>
                    !string.IsNullOrWhiteSpace(opts.SecretKey) &&
                    Encoding.UTF8.GetByteCount(opts.SecretKey) >= 32 &&
                    !string.IsNullOrWhiteSpace(opts.Issuer) &&
                    !string.IsNullOrWhiteSpace(opts.Audience) &&
                    opts.AccessTokenExpirationMinutes > 0 &&
                    opts.RefreshTokenExpirationDays > 0,
                "JwtOptions configuration is invalid or JWT SecretKey is too short (min 32 bytes).")
            .ValidateOnStart();
    }

    private static void AddEmail(
        IServiceCollection services,
        IConfiguration configuration)
    {
        var emailSection =
            configuration.GetSection(
                EmailOptions.SectionName);

        services.Configure<EmailOptions>(
            emailSection);

        services.AddScoped<
            IEmailService,
            EmailService>();
    }

    private static void AddFoundationServices(
        IServiceCollection services)
    {
        services.AddMemoryCache();
        services.AddDistributedMemoryCache();

        services.AddSingleton<
            ICacheService,
            MemoryCacheService>();

        services.AddScoped<
            ILearningAdminService,
            LearningAdminService>();

        services.AddScoped<
            ILearningPublicService,
            LearningPublicService>();
    }

    private static void AddVocabulary(
        IServiceCollection services,
        IConfiguration configuration)
    {
        services
            .AddOptions<VocabularyCacheOptions>()
            .Bind(
                configuration.GetSection(
                    VocabularyCacheOptions.SectionName))
            .Validate(
                options =>
                    options.DetailMinutes > 0 &&
                    options.ListMinutes > 0 &&
                    options.TaxonomyMinutes > 0 &&
                    options.GenerationDays > 0,
                "VocabularyCache configuration is invalid.")
            .ValidateOnStart();

        services.AddScoped<
            HanYu.Application.Interfaces.Vocabulary.IVocabularyAdminService,
            HanYu.Infrastructure.Vocabulary.VocabularyAdminService>();

        services.AddScoped<
            HanYu.Application.Interfaces.Vocabulary.IVocabularyPublicService,
            HanYu.Infrastructure.Vocabulary.VocabularyPublicService>();
    }

    private static void AddLesson(
        IServiceCollection services,
        IConfiguration configuration)
    {
        services
            .AddOptions<HanYu.Infrastructure.Options.LessonCacheOptions>()
            .Bind(
                configuration.GetSection(
                    HanYu.Infrastructure.Options.LessonCacheOptions.SectionName))
            .Validate(
                options =>
                    options.DetailMinutes > 0 &&
                    options.ListMinutes > 0 &&
                    options.GenerationDays > 0,
                "LessonCache configuration is invalid.")
            .ValidateOnStart();

        services.AddScoped<
            HanYu.Application.Interfaces.Lesson.ILessonAdminService,
            HanYu.Infrastructure.Lesson.LessonAdminService>();

        services.AddScoped<
            HanYu.Application.Interfaces.Lesson.ILessonPublicService,
            HanYu.Infrastructure.Lesson.LessonPublicService>();
    }

    private static void AddReview(
        IServiceCollection services)
    {
        services.AddScoped<
            HanYu.Application.Interfaces.Review.IReviewScheduler,
            HanYu.Infrastructure.Review.ReviewScheduler>();

        services.AddScoped<
            HanYu.Application.Interfaces.Review.IReviewService,
            HanYu.Infrastructure.Review.ReviewService>();

        services.AddScoped<
            HanYu.Application.Interfaces.Review.IFlashcardService,
            HanYu.Infrastructure.Review.FlashcardService>();

        services.AddScoped<
            HanYu.Application.Interfaces.Review.IReviewAdminService,
            HanYu.Infrastructure.Review.ReviewAdminService>();
    }

    private static void AddQuiz(
        IServiceCollection services)
    {
        services.AddScoped<
            HanYu.Application.Interfaces.Quiz.IQuizAdminService,
            HanYu.Infrastructure.Quiz.QuizAdminService>();

        services.AddScoped<
            HanYu.Application.Interfaces.Quiz.IQuizPublicService,
            HanYu.Infrastructure.Quiz.QuizPublicService>();
    }

    private static void AddNotification(IServiceCollection services)
    {
        services.AddScoped<
            HanYu.Application.Interfaces.Notification.INotificationPublicService,
            HanYu.Infrastructure.Notification.NotificationPublicService>();

        services.AddScoped<
            HanYu.Application.Interfaces.Notification.INotificationAdminService,
            HanYu.Infrastructure.Notification.NotificationAdminService>();

        services.AddScoped<
            HanYu.Application.Interfaces.Notification.INotificationSender,
            HanYu.Infrastructure.Notification.NotificationSender>();
    }

    private static void AddGamification(IServiceCollection services)
    {
        services.AddScoped<
            HanYu.Application.Interfaces.Gamification.IGamificationService,
            HanYu.Infrastructure.Gamification.GamificationService>();

        services.AddScoped<
            HanYu.Application.Interfaces.Gamification.IGamificationAdminService,
            HanYu.Infrastructure.Gamification.GamificationAdminService>();

        services.AddScoped<
            HanYu.Application.Interfaces.Gamification.IAchievementEvaluator,
            HanYu.Infrastructure.Gamification.AchievementEvaluator>();
    }

    private static void AddContent(IServiceCollection services)
    {
        services.AddScoped<
            HanYu.Application.Interfaces.Content.IContentAdminService,
            HanYu.Infrastructure.Content.ContentAdminService>();

        services.AddScoped<
            HanYu.Application.Interfaces.Content.IContentPublicService,
            HanYu.Infrastructure.Content.ContentPublicService>();
    }

    private static void AddAnalytics(IServiceCollection services)
    {
        services.AddScoped<
            HanYu.Application.Interfaces.Analytics.ILearningAnalyticsCollector,
            HanYu.Infrastructure.Analytics.LearningAnalyticsCollector>();

        services.AddScoped<
            HanYu.Application.Interfaces.Analytics.IAnalyticsAdminService,
            HanYu.Infrastructure.Analytics.AnalyticsAdminService>();

        services.AddScoped<
            HanYu.Application.Interfaces.Analytics.IAnalyticsPublicService,
            HanYu.Infrastructure.Analytics.AnalyticsPublicService>();
    }

    private static void AddAi(IServiceCollection services)
    {
        services.AddScoped<
            HanYu.Application.Interfaces.AI.IAiAdminService,
            HanYu.Infrastructure.AI.AiAdminService>();

        services.AddScoped<
            HanYu.Application.Interfaces.AI.IAiPublicService,
            HanYu.Infrastructure.AI.AiPublicService>();
        services.AddScoped<
            HanYu.Application.Interfaces.AI.IAiProvider,
            HanYu.Infrastructure.AI.MockAiProvider>();
    }

    private static void AddOperations(IServiceCollection services)
    {
        services.AddScoped<
            HanYu.Application.Interfaces.Operations.IOperationsAdminService,
            HanYu.Infrastructure.Operations.OperationsAdminService>();

        services.AddScoped<
            HanYu.Application.Interfaces.Operations.IAuditWriter,
            HanYu.Infrastructure.Operations.AuditWriter>();

        services.AddScoped<
            HanYu.Application.Interfaces.Operations.IProductEventTracker,
            HanYu.Infrastructure.Operations.ProductEventTracker>();
    }
}