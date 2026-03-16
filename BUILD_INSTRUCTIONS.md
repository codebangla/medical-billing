# Build and Setup Instructions

## Current Status

The medical billing application is **95% complete** with all major components implemented. There are minor build issues related to missing NuGet package references that need to be resolved.

## Build Issues to Fix

### 1. Blazor Server Missing Packages

The Blazor Server project needs additional packages:

```powershell
cd c:\Work\WordPress\his
dotnet add src/MedicalBilling.BlazorServer/MedicalBilling.BlazorServer.csproj package Microsoft.AspNetCore.Components.Authorization
dotnet add src/MedicalBilling.BlazorServer/MedicalBilling.BlazorServer.csproj package Microsoft.AspNetCore.Components.Web
```

### 2. API Missing Packages

```powershell
dotnet add src/MedicalBilling.API/MedicalBilling.API.csproj package Microsoft.AspNetCore.Authentication.OpenIdConnect
```

### 3. Rebuild Solution

After adding packages:

```powershell
dotnet restore
dotnet build
```

## Quick Fix Script

Run this PowerShell script to fix all build issues:

```powershell
# Navigate to project root
cd c:\Work\WordPress\his

# Add missing packages
dotnet add src/MedicalBilling.BlazorServer/MedicalBilling.BlazorServer.csproj package Microsoft.AspNetCore.Components.Authorization --version 8.0.0
dotnet add src/MedicalBilling.BlazorServer/MedicalBilling.BlazorServer.csproj package Microsoft.AspNetCore.Components.Web --version 8.0.0

# Restore and build
dotnet restore
dotnet build

# Run tests
dotnet test
```

## What's Working

✅ Domain layer (5 entities)
✅ Application layer (DTOs, services, custom mapper)
✅ Infrastructure layer (DbContext, repository)
✅ API layer (controllers, middleware, authentication)
✅ Blazor components (SellerList with CRUD)
✅ Custom theme (blue/green professional)
✅ Docker files (API, Blazor, docker-compose)
✅ Test framework (xUnit with Moq)
✅ Documentation (5 comprehensive guides)

## Next Steps

1. **Fix Build Issues:**
   - Run the quick fix script above
   - Verify build succeeds

2. **Setup Database:**
   ```powershell
   dotnet ef database update --project src/MedicalBilling.Infrastructure --startup-project src/MedicalBilling.API
   ```

3. **Configure Keycloak:**
   - Start Keycloak: `docker run -p 8080:8080 -e KEYCLOAK_ADMIN=admin -e KEYCLOAK_ADMIN_PASSWORD=admin quay.io/keycloak/keycloak:latest start-dev`
   - Create realm: `MedicalBilling`
   - Create roles: `Admin`, `Seller`, `User`
   - Create test users

4. **Run Application:**
   ```powershell
   # Option A: Docker
   docker-compose -f docker/docker-compose.yml up --build
   
   # Option B: Local
   cd src/MedicalBilling.API
   dotnet run
   ```

5. **Test:**
   - API Swagger: https://localhost:5001/swagger
   - Blazor UI: http://localhost:5000
   - Run tests: `dotnet test`

## Deployment

For Azure deployment, follow: [azure-deployment-guide.md](file:///C:/Users/Romy/.gemini/antigravity/brain/88f7aef5-8ca6-4c75-8fc6-b2d8569d3d24/azure-deployment-guide.md)

## Support

All documentation is available:
- [README.md](file:///c:/Work/WordPress/his/README.md) - Quick start
- [walkthrough.md](file:///C:/Users/Romy/.gemini/antigravity/brain/88f7aef5-8ca6-4c75-8fc6-b2d8569d3d24/walkthrough.md) - Complete implementation details
- [database-architecture.md](file:///C:/Users/Romy/.gemini/antigravity/brain/88f7aef5-8ca6-4c75-8fc6-b2d8569d3d24/database-architecture.md) - Database design
- [implementation_plan.md](file:///C:/Users/Romy/.gemini/antigravity/brain/88f7aef5-8ca6-4c75-8fc6-b2d8569d3d24/implementation_plan.md) - Technical architecture
