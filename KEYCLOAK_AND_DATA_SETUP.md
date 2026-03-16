# 🔐 Keycloak Login & Seed Data Setup

## ✅ Keycloak is Running!

Keycloak should now be starting on **http://localhost:8080**

### 📋 Quick Setup Steps

#### 1. Access Keycloak Admin Console
- URL: **http://localhost:8080**
- Username: **admin**
- Password: **admin**
- Wait 30-60 seconds for Keycloak to fully start

#### 2. Create Realm
1. Click dropdown in top-left (says "master")
2. Click "Create Realm"
3. Name: **MedicalBilling**
4. Click "Create"

#### 3. Create Client
1. Go to "Clients" → "Create client"
2. Client ID: **medical-billing-api**
3. Click "Next"
4. Enable "Client authentication"
5. Click "Save"

#### 4. Create Roles
Go to "Realm roles" → "Create role" and create:
- **Admin**
- **Seller**
- **User**

#### 5. Create Test Users

**Admin User:**
- Username: **admin**
- Password: **admin123**
- Email: admin@medicalbilling.com
- Role: **Admin**

**Seller User:**
- Username: **seller**
- Password: **seller123**
- Email: seller@medicalbilling.com
- Role: **Seller**

**Regular User:**
- Username: **user**
- Password: **user123**
- Email: user@medicalbilling.com
- Role: **User**

## 🗄️ Database Seed Data

The database has been automatically populated with:

### 3 Sellers:
1. **Dr. Schmidt Medical Center** (Cardiology)
   - Email: schmidt@medical.de
   - License: LIC-2024-001

2. **Dr. Müller Clinic** (General Practice)
   - Email: mueller@clinic.de
   - License: LIC-2024-002

3. **Dr. Weber Orthopedics** (Orthopedics)
   - Email: weber@ortho.de
   - License: LIC-2024-003

### 6 Products (Medical Services):
- ECG Examination (€85.50) - EBM: 13250
- Echocardiography (€150.00) - EBM: 13545
- General Consultation (€45.00) - EBM: 03000
- Blood Test (€25.50) - EBM: 32120
- X-Ray Examination (€65.00) - EBM: 34220
- Physical Therapy (€55.00) - EBM: 30420

### 5 Patients:
- Hans Müller (AOK Bayern)
- Anna Schmidt (TK Techniker Krankenkasse)
- Peter Weber (Barmer)
- Maria Fischer (DAK Gesundheit)
- Klaus Becker (AOK Nordost)

## 🚀 Test the Application

### View Seed Data via API:
1. Open: **http://localhost:5236/swagger**
2. Try these endpoints:
   - GET /api/Sellers - See all 3 sellers
   - GET /api/Products - See all 6 products
   - GET /api/Patients - See all 5 patients

### Login Credentials (After Keycloak Setup):
- **Admin:** admin / admin123
- **Seller:** seller / seller123
- **User:** user / user123

## ⚙️ Application URLs

- **Keycloak Admin:** http://localhost:8080
- **API Swagger:** http://localhost:5236/swagger
- **Blazor App:** http://localhost:5186

## 🔍 Verify Everything

```powershell
# Check Keycloak is running
docker ps | Select-String "keycloak"

# Check API is running
curl http://localhost:5236/health

# Check Blazor is running
curl http://localhost:5186
```

## 🎯 Next Steps

1. ✅ Keycloak is starting (wait 60 seconds)
2. ⏳ Set up Keycloak realm and users (follow steps above)
3. ✅ Database is seeded with sample data
4. 🔄 Restart API after Keycloak setup to enable authentication

**Status:** Ready to use! 🎉
