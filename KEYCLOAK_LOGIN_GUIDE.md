# 🔐 Keycloak Login Guide - Medical Billing Application

## Quick Access Information

### Keycloak Admin Console
- **URL:** http://localhost:8080
- **Admin Username:** `admin`
- **Admin Password:** `admin`

### Test User Credentials

Once Keycloak is configured, use these credentials to login to the Medical Billing application:

#### 👨‍💼 Admin User (Full Access)
```
Username: admin
Password: admin
```
**Access Level:** Full administrative access
- Manage sellers, products, patients
- Create/edit/delete all records
- View all billing procedures and invoices

#### 🏥 Seller User (Medical Provider)
```
Username: seller1
Password: seller123
```
**Access Level:** Seller operations
- Manage own products/services
- Create billing procedures
- View own patients
- Generate invoices

#### 👤 Regular User (Read-Only)
```
Username: user1
Password: user123
```
**Access Level:** View-only
- View sellers and products
- View billing information
- No create/edit/delete permissions

## Step-by-Step Setup

### 1. Start Keycloak (if not running)

```powershell
# Start Keycloak in Docker
docker run -d --name keycloak-medical -p 8080:8080 `
  -e KEYCLOAK_ADMIN=admin `
  -e KEYCLOAK_ADMIN_PASSWORD=admin `
  quay.io/keycloak/keycloak:latest start-dev
```

Wait 30-60 seconds for Keycloak to fully start.

### 2. Access Keycloak Admin Console

1. Open your browser
2. Navigate to: **http://localhost:8080**
3. Click "Administration Console"
4. Login with:
   - Username: `admin`
   - Password: `admin`

### 3. Create Medical Billing Realm

1. Click the dropdown in the top-left corner (currently shows "master")
2. Click **"Create Realm"**
3. Enter Realm name: `MedicalBilling`
4. Click **"Create"**

### 4. Create API Client

1. In the MedicalBilling realm, go to **"Clients"**
2. Click **"Create client"**
3. Enter:
   - Client ID: `medical-billing-api`
   - Client type: OpenID Connect
4. Click **"Next"**
5. Enable:
   - ✅ Client authentication
   - ✅ Authorization
   - ✅ Service accounts roles
6. Click **"Save"**

### 5. Create Roles

1. Go to **"Realm roles"**
2. Click **"Create role"**
3. Create these three roles:

**Role 1: Admin**
- Role name: `Admin`
- Description: Full administrative access

**Role 2: Seller**
- Role name: `Seller`
- Description: Medical service provider access

**Role 3: User**
- Role name: `User`
- Description: Read-only access

### 6. Create Test Users

#### Create Admin User:
1. Go to **"Users"** → **"Add user"**
2. Fill in:
   - Username: `admin`
   - Email: `admin@medicalbilling.com`
   - First name: `Admin`
   - Last name: `User`
   - Email verified: ✅ ON
3. Click **"Create"**
4. Go to **"Credentials"** tab
5. Click **"Set password"**
6. Enter password: `admin`
7. Turn OFF "Temporary"
8. Click **"Save"**
9. Go to **"Role mapping"** tab
10. Click **"Assign role"**
11. Select **"Admin"** role
12. Click **"Assign"**

#### Create Seller User:
Repeat the same process with:
- Username: `seller1`
- Password: `seller123`
- Role: **Seller**

#### Create Regular User:
Repeat the same process with:
- Username: `user1`
- Password: `user123`
- Role: **User**

## Application Access

### After Keycloak Setup:

1. **Start the API:**
```powershell
cd c:\Work\WordPress\his\src\MedicalBilling.API
dotnet run
```

2. **Start Blazor (in new terminal):**
```powershell
cd c:\Work\WordPress\his\src\MedicalBilling.BlazorServer
dotnet run
```

3. **Access the Application:**
   - Open browser to: **http://localhost:5000**
   - You'll be redirected to Keycloak login
   - Use one of the test credentials above
   - After login, you'll be redirected back to the app

## Login Page Preview

When you access the Medical Billing application, you'll see:

1. **Keycloak Login Page** with:
   - Medical Billing realm name
   - Username/email field
   - Password field
   - "Sign In" button
   - Professional blue/white theme

2. **After Login:**
   - Redirected to Medical Billing dashboard
   - Navigation menu with:
     - Sellers
     - Products
     - Patients
     - Billing Procedures
     - Invoices
   - User info displayed in top-right
   - Logout option

## Troubleshooting

### Keycloak Not Starting
```powershell
# Check if container is running
docker ps

# View logs
docker logs keycloak-medical

# Restart if needed
docker restart keycloak-medical
```

### Can't Access Keycloak
- Wait 60 seconds after starting
- Check if port 8080 is available
- Try: http://127.0.0.1:8080

### Login Fails
- Verify realm name is exactly: `MedicalBilling`
- Check user was created in correct realm
- Verify password was set (not temporary)
- Check role was assigned

## Quick Test

To verify everything is working:

1. **Test Keycloak Admin:**
   - Go to: http://localhost:8080
   - Login: admin / admin
   - Should see admin console

2. **Test Application Login:**
   - Go to: http://localhost:5000
   - Should redirect to Keycloak
   - Login: admin / admin
   - Should redirect back to app

3. **Test API:**
   - Go to: https://localhost:5001/swagger
   - Click "Authorize"
   - Get token from Keycloak
   - Test API endpoints

## Security Notes

⚠️ **These are development credentials only!**

For production:
- Change all passwords
- Use strong passwords
- Enable HTTPS
- Configure proper realm settings
- Set up email verification
- Enable 2FA
- Review security policies

---

**Need Help?** See [KEYCLOAK_SETUP.md](file:///c:/Work/WordPress/his/KEYCLOAK_SETUP.md) for more details.
