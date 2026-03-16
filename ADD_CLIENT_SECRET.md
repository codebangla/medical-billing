# 🔑 Adding Keycloak Client Secret

## Quick Fix

### 1. Get Client Secret from Keycloak

1. Open Keycloak: http://localhost:8080
2. Login: **admin** / **admin**
3. Select realm: **MedicalBilling** (top-left dropdown)
4. Go to: **Clients** → **medical-billing-blazor**
5. Click the **Credentials** tab
6. Find **Client secret** field
7. Click the **copy icon** (📋) to copy the secret

### 2. Update Blazor Configuration

Open: `src/MedicalBilling.BlazorServer/appsettings.json`

Replace line 6 with the secret you copied:

```json
{
  "ApiBaseUrl": "http://localhost:5236",
  "Keycloak": {
    "Authority": "http://localhost:8080/realms/MedicalBilling",
    "ClientId": "medical-billing-blazor",
    "ClientSecret": "PASTE_YOUR_SECRET_HERE"
  },
  ...
}
```

### 3. Restart Blazor App

The app should automatically restart if you're in watch mode. If not:

```powershell
# Stop the app (Ctrl+C)
cd c:\Work\WordPress\his\src\MedicalBilling.BlazorServer
dotnet run
```

### 4. Test Login Again

1. Go to: http://localhost:5186
2. Click **Login**
3. Login with: **admin** / **admin123**
4. Should redirect back successfully!

## Alternative: Public Client (No Secret)

If you want to avoid using a client secret:

1. In Keycloak: **Clients** → **medical-billing-blazor** → **Settings**
2. Turn **Client authentication** to **OFF**
3. Click **Save**
4. Keep `"ClientSecret": ""` in appsettings.json
5. Restart Blazor app

This is simpler but less secure (fine for development).
