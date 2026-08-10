# HanYu Media Storage — Backblaze B2

HanYu uses Neon/PostgreSQL for structured data and Backblaze B2 for binary media.
The database stores only media URLs/object keys; image/audio/video/document bytes are never stored in PostgreSQL.

## 1. Create buckets

Create a public bucket for learning media:

- Bucket name: `hanyu-public` (or another globally available name)
- Files in bucket: Public
- Encryption: enabled/default is fine

Optional: create a second private bucket for exports:

- Bucket name: `hanyu-private`
- Files in bucket: Private

Recommended public object prefixes:

```text
images/YYYY/MM/...
audio/YYYY/MM/...
videos/YYYY/MM/...
documents/YYYY/MM/...
seed/course-covers/...
```

## 2. Create an Application Key

In Backblaze B2 → Application Keys, create a standard Application Key for the HanYu backend.
Do not use the master application key for the S3-compatible API.

For the public media bucket grant the backend the capabilities required to upload/delete files.
If the key is restricted to a single bucket and the SDK needs to list buckets, enable `listAllBucketNames` as well.

Copy the two values immediately:

- `keyID` → `Storage:AccessKey`
- `applicationKey` → `Storage:SecretKey`

Never put these values in `admin-web`, Git, or committed `appsettings*.json` files.

## 3. Get the S3 endpoint and region

Backblaze B2 S3-compatible endpoints use:

```text
https://s3.<region>.backblazeb2.com
```

Example only:

```text
Region: us-west-004
ServiceUrl: https://s3.us-west-004.backblazeb2.com
```

Use the exact region shown for your bucket.

For a public bucket, HanYu can use the virtual-hosted S3 URL as its browser-facing base URL:

```text
https://<bucket>.s3.<region>.backblazeb2.com
```

Example only:

```text
https://hanyu-public.s3.us-west-004.backblazeb2.com
```

If you later put Cloudflare/CDN/custom domain in front of B2, change only `Storage:PublicBaseUrl`; existing storage code does not need to change.

## 4. Configure .NET User Secrets

Run from `HanYu-Web/HanYu`:

```powershell
dotnet user-secrets set "Storage:ServiceUrl" "https://s3.<REGION>.backblazeb2.com"
dotnet user-secrets set "Storage:AccessKey" "<B2_KEY_ID>"
dotnet user-secrets set "Storage:SecretKey" "<B2_APPLICATION_KEY>"
dotnet user-secrets set "Storage:Region" "<REGION>"
dotnet user-secrets set "Storage:ForcePathStyle" "false"
dotnet user-secrets set "Storage:PublicBucketName" "hanyu-public"
dotnet user-secrets set "Storage:PublicBaseUrl" "https://hanyu-public.s3.<REGION>.backblazeb2.com"
dotnet user-secrets set "Storage:ExportBucketName" "hanyu-private"
```

List the configured keys (values are local machine secrets, not committed):

```powershell
dotnet user-secrets list
```

## 5. Development content seed

`ContentSeed:Enabled` is enabled in Development.
When HSK1–HSK6 exist and public B2 storage is configured, `CourseContentSeeder` uploads six generated cover images to storage first and then creates the six Course rows.

Database example:

```text
courses.cover_image_url = https://hanyu-public.s3.<region>.backblazeb2.com/seed/course-covers/course-hsk1.svg
```

The database never contains the SVG/image bytes.

## 6. Admin upload endpoints

All endpoints require the Admin policy and accept `multipart/form-data` with a field named `file`.

```text
POST /api/v1/admin/uploads/images
POST /api/v1/admin/uploads/audio
POST /api/v1/admin/uploads/videos
POST /api/v1/admin/uploads/documents
```

Current backend limits:

- images: 10 MB
- audio: 50 MB
- video: 200 MB
- documents: 50 MB

Accepted types are validated server-side. SVG is intentionally not accepted from arbitrary admin uploads; seed-generated SVG is trusted server content and is uploaded directly through `IPublicFileStorage`.

## 7. Architecture

```text
admin-web
   |
   | multipart upload
   v
HanYu API
   |
   | IPublicFileStorage
   v
S3PublicFileStorage
   |
   | S3-compatible API
   v
Backblaze B2

Neon PostgreSQL
   └── stores URL/ObjectKey only
```
