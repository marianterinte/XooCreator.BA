Deployment to Railway (Manual Dockerfile or Nixpacks)
=====================================================

Problem Observed
----------------
Railway build logs showed Nixpacks selecting a .NET 6 SDK:
  Welcome to .NET 6.0! SDK Version: 6.0.413
and then failing with:
  MSBUILD : error MSB1003: Specify a project or solution file. The current working directory does not contain a project or solution file.

Causes:
1. No global.json: Nixpacks defaulted to an older cached SDK (6.x) instead of 8.x.
2. dotnet restore was executed in a directory that did not contain the project because the working directory for the generated Dockerfile wasn't aligned with the project root.
3. You rely on a single project (no solution file) so commands must explicitly reference XooCreator.BA.csproj or run in same directory.

Fixes Added
-----------
1. Added global.json pinning SDK 8.0.401.
2. Added explicit Dockerfile targeting .NET 8 with two-stage build.
3. Provided nixpacks.toml already configured to run: dotnet restore XooCreator.BA.csproj (ensure root path correct).

Recommended Railway Settings
----------------------------
Option A: Use Dockerfile (simplest, deterministic)
  - In Railway project settings, set Deployment mode to Dockerfile.
  - Ensure root is repository root (where Dockerfile exists).
  - Railway will build and run with .NET 8.

Option B: Use Nixpacks with nixpacks.toml
  - Keep nixpacks.toml.
  - Ensure project root in Railway is the same directory containing XooCreator.BA.csproj.
  - Build will now pick .NET 8 because of global.json.

Environment Variables Needed
----------------------------
  PORT (Railway auto injects)
  DATABASE_URL (postgres://user:pass@host:port/dbname) - provided by Railway Postgres plugin
  ASPNETCORE_ENVIRONMENT=Production (Railway default) or Development for tests

Local Test Commands
-------------------
  dotnet restore XooCreator.BA.csproj
  dotnet publish XooCreator.BA.csproj -c Release -o out
  dotnet out/XooCreator.BA.dll

If Using Dockerfile Locally
---------------------------
  docker build -t xoo-app .
  docker run -p 8080:8080 -e PORT=8080 xoo-app

Logs & Troubleshooting
----------------------
If restore still fails in Railway:
  1. Open build logs and check dotnet --info shows 8.0.*
  2. If not, verify global.json is at repo root.
  3. Make sure there is no custom build command overriding default.
  4. Confirm working directory contains XooCreator.BA.csproj (list files in install phase).

Migrations
----------
Program.cs auto-runs context.Database.Migrate() on startup. Ensure your Postgres is reachable; if using SSL verify Railway requires SslMode=Require (already set). If local debugging fails due to SSL, unset DATABASE_URL locally.

