# Keycloak Setup Instructions

## Quick Start with Docker

### 1. Start Keycloak
```powershell
docker run -d --name keycloak-medical -p 8080:8080 `
  -e KEYCLOAK_ADMIN=admin `
  -e KEYCLOAK_ADMIN_PASSWORD=admin `
  quay.io/keycloak/keycloak:latest start-dev
```

Wait 30 seconds for Keycloak to start, then access: http://localhost:8080

### 2. Login to Keycloak Admin Console
- URL: http://localhost:8080
- Username: **admin**
- Password: **admin**

### 3. Create Realm

1. Click dropdown in top-left (says "master")
2. Click "Create Realm"
3. Name: **MedicalBilling**
4. Click "Create"

### 4. Create Client

1. Go to "Clients" → "Create client"
2. Client ID: **medical-billing-api**
3. Click "Next"
4. Enable "Client authentication"
5. Enable "Authorization"
6. Click "Save"

### 5. Create Roles

1. Go to "Realm roles" → "Create role"
2. Create these roles:
   - **Admin**
   - **Seller**
   - **User**

### 6. Create Test Users

**Admin User:**
1. Go to "Users" → "Add user"
2. Username: **admin**
3. Email: admin@medicalbilling.com
4. First name: Admin
5. Last name: User
6. Click "Create"
7. Go to "Credentials" tab
8. Set password: **admin**
9. Turn off "Temporary"
10. Click "Set password"
11. Go to "Role mapping" tab
12. Click "Assign role"
13. Select **Admin** role

**Seller User:**
1. Username: **seller1**
2. Password: **seller123**
3. Assign **Seller** role

**Regular User:**
1. Username: **user1**
2. Password: **user123**
3. Assign **User** role

## Test Credentials

Once setup is complete, use these credentials to login:

### Admin (Full Access)
- Username: `admin`
- Password: `admin`
- Access: All features

### Seller (Limited Access)
- Username: `seller1`
- Password: `seller123`
- Access: Own products, billing

### User (Read-Only)
- Username: `user1`
- Password: `user123`
- Access: View only

## Verify Setup

1. Open: http://localhost:8080/realms/MedicalBilling
2. You should see the realm configuration
3. The application will use this for authentication

## Application URLs

After Keycloak is configured:
- **Keycloak Admin**: http://localhost:8080
- **API Swagger**: https://localhost:5001/swagger
- **Blazor App**: http://localhost:5000

## Troubleshooting

**Keycloak not starting:**
```powershell
docker logs keycloak-medical
```

**Reset Keycloak:**
```powershell
docker stop keycloak-medical
docker rm keycloak-medical
# Then run the start command again
```
