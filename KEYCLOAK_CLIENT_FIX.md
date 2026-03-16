# 🔧 Keycloak Client Configuration Fix

## Issue
The Blazor app can't connect to Keycloak because the client needs additional configuration.

## Steps to Fix in Keycloak Admin Console

### 1. Go to Your Client Settings
1. Open Keycloak: **http://localhost:8080**
2. Login with **admin** / **admin**
3. Select realm: **MedicalBilling**
4. Go to **Clients** → Click **medical-billing-blazor**

### 2. Update Client Settings

**Settings Tab:**
- Client authentication: **ON** ✅
- Authorization: **OFF**
- Authentication flow:
  - ✅ Standard flow
  - ✅ Direct access grants
  - ❌ Implicit flow
  - ❌ Service accounts roles

**Valid redirect URIs:**
```
http://localhost:5186/*
http://localhost:5186/signin-oidc
```

**Valid post logout redirect URIs:**
```
http://localhost:5186/*
```

**Web origins:**
```
http://localhost:5186
```

Click **Save**

### 3. Get Client Secret (if needed)

1. Go to **Credentials** tab
2. Copy the **Client secret** value
3. Update `appsettings.json` if the secret is not empty

### 4. Verify Realm is Accessible

Open this URL in your browser:
```
http://localhost:8080/realms/MedicalBilling/.well-known/openid-configuration
```

You should see JSON configuration. If you get an error, the realm doesn't exist or Keycloak isn't running properly.

## Alternative: Disable Client Authentication

If you want simpler setup:

1. Go to **Clients** → **medical-billing-blazor**
2. **Settings** tab
3. Turn **Client authentication** to **OFF**
4. Click **Save**
5. Update `appsettings.json`:
   ```json
   "ClientSecret": ""
   ```

## Test After Configuration

1. Restart Blazor app
2. Go to http://localhost:5186
3. Click **Login**
4. Should redirect to Keycloak login page
5. Login with **admin** / **admin123**
6. Should redirect back to Blazor app

## Current Settings in Blazor App

```json
{
  "Keycloak": {
    "Authority": "http://localhost:8080/realms/MedicalBilling",
    "ClientId": "medical-billing-blazor",
    "ClientSecret": ""
  }
}
```
