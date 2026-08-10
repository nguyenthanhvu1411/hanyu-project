# ─── Build Stage ───────────────────────────────────────────────────────────────
# Uses the full SDK to restore, build and publish the application.
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy project file first to leverage Docker layer caching for NuGet restore.
COPY HanYu/HanYu.csproj HanYu/

RUN dotnet restore HanYu/HanYu.csproj

# Copy remaining source files.
COPY HanYu/ HanYu/

WORKDIR /src/HanYu

# Publish in Release mode — no SDK, no source, no debug symbols in the final image.
RUN dotnet publish HanYu.csproj \
    --configuration Release \
    --no-restore \
    --output /app/publish \
    /p:UseAppHost=false

# ─── Runtime Stage ─────────────────────────────────────────────────────────────
# Minimal ASP.NET runtime — no SDK, no compiler, smaller attack surface.
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final

# Run as non-root user (principle of least privilege).
# UID 1000 is the conventional first non-root user.
RUN addgroup --system --gid 1000 hanyu && \
    adduser  --system --uid 1000 --ingroup hanyu --no-create-home hanyu

WORKDIR /app

# Copy published output from build stage.
COPY --from=build /app/publish .

# Ensure the app user owns the working directory.
RUN chown -R hanyu:hanyu /app

USER hanyu

# Expose HTTP port only. TLS is terminated at the reverse proxy (Nginx/Cloudflare).
EXPOSE 8080

# Use the ASP.NET Core environment variable to specify production config.
ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "HanYu.dll"]

# ─── Production Notes ──────────────────────────────────────────────────────────
# Secrets must be injected via environment variables or secrets manager at runtime:
#
#   ConnectionStrings__DefaultConnection=<neon-postgres-url>
#   Jwt__SecretKey=<min-32-byte-secret>
#   Jwt__Issuer=HanYu.Api
#   Jwt__Audience=HanYu.Client
#   Storage__AccessKey=<s3-key>
#   Storage__SecretKey=<s3-secret>
#   Security__AllowedOrigins__0=https://hanyu.vn
#   Security__AllowedOrigins__1=https://admin.hanyu.vn
#   Security__KnownProxies__0=<nginx-ip>
