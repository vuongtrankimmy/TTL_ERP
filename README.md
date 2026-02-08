# 📘 TÀI LIỆU KỸ THUẬT BACKEND ERP

## Công ty TNHH Tân Tân Lộc – [http://www.tantanloc.com.vn/](http://www.tantanloc.com.vn/)

---

## 1. Mục tiêu hệ thống

Hệ thống ERP (Enterprise Resource Planning) cho **Tân Tân Lộc** nhằm:

* Quản lý tập trung toàn bộ dữ liệu vận hành doanh nghiệp
* Chuẩn hóa quy trình sản xuất – kinh doanh – tài chính
* Giảm phụ thuộc Excel thủ công, tăng tính chính xác & realtime
* Sẵn sàng mở rộng quy mô (multi-branch, multi-warehouse)

Backend ERP đóng vai trò **xương sống**, cung cấp API, xử lý nghiệp vụ, bảo mật và tích hợp hệ thống.

---

## 2. Phạm vi nghiệp vụ (ERP Modules)

### 2.1 Core Modules

* 👤 Nhân sự (HRM)
* 🏭 Sản xuất (Manufacturing)
* 📦 Kho vận (Inventory / Warehouse)
* 🛒 Mua hàng (Procurement)
* 💰 Bán hàng (Sales / CRM)
* 📊 Kế toán – tài chính (Accounting / Finance)
* 🧾 Báo cáo & Dashboard

### 2.2 Supporting Modules

* 🔐 Phân quyền & bảo mật
* 📑 Workflow phê duyệt
* 🔔 Notification (Email / Zalo / App)
* 📂 Quản lý file & chứng từ

---

## 3. Kiến trúc tổng thể Backend

### 3.1 Kiến trúc đề xuất

**Modular Monolith → Microservice-ready**

```
[ Client (Web / Mobile) ]
        ↓
[ API Gateway ]
        ↓
[ ERP Backend Core ]
 ├─ Auth & Identity
 ├─ HRM Module
 ├─ Inventory Module
 ├─ Manufacturing Module
 ├─ Sales Module
 ├─ Accounting Module
 ├─ Reporting Module
        ↓
[ Database Layer ]
```

### 3.2 Nguyên tắc thiết kế

* Clean Architecture
* Domain-Driven Design (DDD)
* SOLID
* Tách Business Logic khỏi Infrastructure

---

## 4. Công nghệ sử dụng (Backend Stack)

### 4.1 Nền tảng

* **.NET 8 / ASP.NET Core Web API**
* C#

### 4.2 Database

* **SQL Server** (Core ERP Data)
* MongoDB (Logs / Audit / Reports)

### 4.3 Infrastructure

* Docker / Docker Compose
* IIS / Linux Server
* Redis (Cache, Session)
* RabbitMQ (Event / Queue)

### 4.4 DevOps

* GitHub / GitLab
* CI/CD (GitHub Actions)
* Environment: Dev / Staging / Production

---

## 5. Authentication & Authorization

### 5.1 Authentication

* JWT Bearer Token
* Refresh Token
* Optional: SSO (OAuth2)

### 5.2 Authorization

* RBAC (Role-Based Access Control)
* Permission theo module & chức năng

Ví dụ Role:

* Admin
* Kế toán
* Nhân sự
* Quản lý kho
* Quản lý sản xuất

---

## 6. Thiết kế Database (High-level)

### 6.1 Core Tables

* Users
* Roles
* Permissions
* Departments
* Employees

### 6.2 Inventory

* Warehouses
* Products
* ProductCategories
* StockTransactions

### 6.3 Manufacturing

* BOM (Bill of Materials)
* ProductionOrders
* ProductionSteps
* MachineLogs

### 6.4 Accounting

* Accounts
* JournalEntries
* Invoices
* Payments

---

## 7. API Design Standards

### 7.1 RESTful Convention

```
GET    /api/products
POST   /api/products
PUT    /api/products/{id}
DELETE /api/products/{id}
```

### 7.2 Response Format

```json
{
  "success": true,
  "data": {},
  "message": ""
}
```

### 7.3 Error Handling

* Global Exception Middleware
* Error Code chuẩn hóa

---

## 8. Workflow & Approval Engine

Áp dụng cho:

* Đơn mua hàng
* Phiếu xuất kho
* Lệnh sản xuất
* Thanh toán

Workflow động:

* Theo phòng ban
* Theo giá trị tiền
* Theo vai trò

---

## 9. Logging – Audit – Monitoring

### 9.1 Logging

* Serilog
* Centralized Log

### 9.2 Audit Trail

* Ai làm gì, khi nào, dữ liệu cũ & mới

### 9.3 Monitoring

* Health Check API
* Metrics

---

## 10. Reporting & BI

* Báo cáo realtime
* Export Excel / PDF
* Dashboard theo vai trò

Nguồn dữ liệu:

* SQL Views
* Materialized Reports

---

## 11. Bảo mật hệ thống

* HTTPS
* Rate Limiting
* SQL Injection Protection
* Data Encryption (Sensitive Fields)
* Backup định kỳ

---

## 12. Khả năng mở rộng trong tương lai

* Microservices
* Mobile App
* AI dự báo sản xuất & tồn kho
* Tích hợp ERP – CRM – Kế toán ngoài

---

## 13. Kết luận

Backend ERP cho **Tân Tân Lộc** được thiết kế theo tiêu chuẩn **enterprise**, đảm bảo:

* Ổn định
* Bảo mật
* Dễ mở rộng
* Phù hợp doanh nghiệp sản xuất tại Việt Nam

---

📌 *Tài liệu này dùng cho: CTO, Backend Developer, System Architect, DevOps*
