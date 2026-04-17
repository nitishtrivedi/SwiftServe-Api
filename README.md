# 🍽️ Jnaya Food Delivery Platform - Backend API

A **multi-tenant SaaS food delivery backend** built using:

- .NET Core Web API
- PostgreSQL (Supabase)
- JWT Authentication
- Role-Based Access Control (RBAC)
- Razorpay Integration
- SignalR (real-time tracking)
- Cloudinary (image uploads)

---

# 🧠 SYSTEM OVERVIEW

## Roles

| Role            | Description |
|-----------------|------------|
| SuperAdmin      | Platform owner (creates tenants) |
| Admin           | Business owner (manages restaurants) |
| Customer        | End user placing orders |
| DeliveryPartner | Handles delivery + GPS tracking |

---

# 🏗️ MULTI-TENANT STRATEGY

- Single database
- Shared schema
- `TenantId` column used everywhere
- Strict filtering at service level

---

# 🔐 AUTHENTICATION

## LOGIN

POST `/api/auth/login`

```json
{
  "email": "superadmin@test.com",
  "password": "Admin@123"
}
```

## REGISTER (Customer)

POST `/api/auth/register`

```json
{
  "name": "Test User",
  "email": "user@test.com",
  "phoneNumber": "7777777777",
  "password": "User@123"
}
```

---

# 👑 SUPER ADMIN

POST `/api/superadmin/create-tenant`

```json
{
  "tenantName": "Pizza Empire",
  "adminName": "Owner One",
  "adminEmail": "owner@test.com",
  "adminPhone": "8888888888",
  "password": "Admin@123"
}
```

---

# 🏪 ADMIN

## Create Restaurant

POST `/api/admin/restaurants`

```json
{
  "name": "Pizza Empire Main",
  "address": "Pune",
  "latitude": 18.5204,
  "longitude": 73.8567
}
```

---

# 🍱 CATEGORY

POST `/api/admin/categories`

```json
{
  "restaurantId": 1,
  "name": "Pizzas",
  "displayOrder": 1
}
```

---

# 🍕 MENU

POST `/api/admin/menu` (form-data)

Fields:
- restaurantId
- categoryId
- name
- description
- price
- file

GET `/api/admin/menu/{restaurantId}`

---

# 🛒 CART

POST `/api/cart`

```json
{
  "restaurantId": 1,
  "menuItemId": 1,
  "quantity": 2
}
```

GET `/api/cart`

---

# 📦 ORDER

POST `/api/orders`

```json
{
  "restaurantId": 1,
  "paymentMethod": "Razorpay",
  "deliveryLatitude": 18.5204,
  "deliveryLongitude": 73.8567
}
```

---

# 💳 PAYMENT

POST `/api/payment/create`

```json
{
  "orderId": 1
}
```

POST `/api/payment/verify`

```json
{
  "orderId": 1,
  "razorpayOrderId": "order_xxx",
  "razorpayPaymentId": "pay_xxx",
  "razorpaySignature": "signature"
}
```

---

# 🚚 DELIVERY

POST `/api/admin/delivery/assign`

```json
{
  "orderId": 1,
  "deliveryPartnerId": 5
}
```

POST `/api/delivery/location/{orderId}`

---

# 📍 TRACKING

GET `/api/tracking/{deliveryPartnerId}`

---

# 📊 DASHBOARD

GET `/api/admin/dashboard`

---

# 🧪 FULL TEST FLOW

1. Login SuperAdmin  
2. Create Tenant  
3. Login Admin  
4. Create Restaurant  
5. Create Category  
6. Create Menu  
7. Register Customer  
8. Add to Cart  
9. Place Order  
10. Create Payment  
11. Verify Payment  

---

# 🔐 JWT CONFIG

```json
"Jwt": {
  "Key": "SUPER_SECRET_KEY",
  "Issuer": "FoodDeliveryApp",
  "Audience": "FoodDeliveryUsers",
  "ExpiryInDays": 7
}
```

---

# 🗄️ DATABASE

```
Host=db.xxxxx.supabase.co;
Port=5432;
Database=postgres;
Username=postgres;
Password=your_password;
SSL Mode=Require;
Trust Server Certificate=true
```

---

# 🚀 STATUS

✔ Multi-tenant ready  
✔ Payment ready  
✔ Tracking ready  

---

# 👨‍💻 Jnaya Platform
