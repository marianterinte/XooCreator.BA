# Railway Deployment Guide

This document describes how to deploy XooCreator.BA to Railway.

## Deployment Configuration

The application uses **nixpacks** for building on Railway. The configuration files are:

- `nixpacks.toml` - Main build configuration for Railway
- `Procfile` - Process definition (backup)
- `Dockerfile` - Docker containerization (backup method)
- `.github/workflows/railway-deploy.yml` - Automated deployment via GitHub Actions

## Environment Variables Required

Configure these environment variables in your Railway service:

- `DATABASE_URL` - PostgreSQL connection string (automatically provided by Railway if you add a PostgreSQL service)
- `PORT` - Port to bind to (automatically set by Railway)

## Manual Deployment

To deploy manually using Railway CLI:

1. Install Railway CLI:
   ```bash
   npm install -g @railway/cli
   ```

2. Login and link to your service:
   ```bash
   railway login
   railway link [your-service-id]
   ```

3. Deploy from the XooCreator.BA subdirectory:
   ```bash
   railway up --root=./XooCreator.BA
   ```

## Automatic Deployment

The project includes GitHub Actions workflow for automatic deployment on pushes to the `main` branch.

Required GitHub secrets:
- `RAILWAY_TOKEN` - Your Railway API token
- `RAILWAY_SERVICE_NAME` - Your Railway service name

## Database Setup

The application automatically runs database migrations on startup. Make sure you have a PostgreSQL service attached to your Railway project.

## Troubleshooting

1. **Build fails**: Check that nixpacks can find the .csproj file
2. **App won't start**: Verify the `PORT` environment variable is being used
3. **Database connection**: Ensure `DATABASE_URL` is correctly set

## Local Testing

To test the build process locally:

```bash
cd XooCreator.BA
dotnet restore
dotnet build
dotnet publish -c Release -o ./out
dotnet ./out/XooCreator.BA.dll
```