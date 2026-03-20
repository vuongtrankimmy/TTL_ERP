# 📘 Hướng Dẫn Sử Dụng Mock Data Mode

## 🎯 Mục Đích

Chức năng Mock Data cho phép bạn:
- ✅ **Chạy website frontend KHÔNG cần kết nối API backend**
- ✅ **Làm việc offline hoàn toàn**
- ✅ **Demo ứng dụng mà không cần database**
- ✅ **Phát triển frontend độc lập với backend**

---

## 📦 Các File Đã Tạo

### 1. Script Export Database
**File:** `export_mongodb_to_mock.js`
- Export toàn bộ dữ liệu từ MongoDB thành file JSON
- Tự động convert ObjectId sang string
- Lưu vào `TTL.HR/TTL.HR.Web/MockData/mongodb_export.json`

### 2. Mock Data Provider
**File:** `TTL.HR.Application/Infrastructure/MockData/MockDataProvider.cs`
- Load và quản lý mock data từ file JSON
- Cung cấp API để truy xuất dữ liệu

### 3. Mock HTTP Client
**File:** `TTL.HR.Application/Infrastructure/MockData/MockHttpClient.cs`
- Thay thế HttpClient thực
- Mô phỏng GET, POST, PUT, DELETE requests
- Hỗ trợ pagination, search, filter

### 4. Configuration Files
**Files:**
- `appsettings.json` - Production config (Mock = OFF)
- `appsettings.Development.json` - Development config (Mock = ON)

---

## 🚀 Cách Sử Dụng

### Bước 1: Export Dữ Liệu Từ MongoDB

```bash
# Di chuyển vào thư mục TTL_ERP
cd TTL_ERP

# Cài đặt dependencies (chỉ lần đầu)
npm install mongodb

# Chạy script export
node export_mongodb_to_mock.js
```

**Kết quả:**
```
🔌 Đang kết nối MongoDB...
✅ Kết nối MongoDB thành công!
📦 Đang export collection: employees...
   ✅ Export 150 documents từ employees
📦 Đang export collection: departments...
   ✅ Export 12 documents từ departments
...
🎉 HOÀN THÀNH!
📁 File mock data đã được lưu tại: TTL.HR/TTL.HR.Web/MockData/mongodb_export.json
```

### Bước 2: Di Chuyển File Mock Data

```bash
# Tạo thư mục MockData trong wwwroot
mkdir -p TTL.HR/TTL.HR.Web/wwwroot/MockData

# Copy file mock data
cp TTL.HR/TTL.HR.Web/MockData/mongodb_export.json TTL.HR/TTL.HR.Web/wwwroot/MockData/
```

### Bước 3: Bật Mock Mode

#### Cách 1: Sửa `appsettings.json`

```json
{
    "MockDataSettings": {
        "Enabled": true,
        "MockDataPath": "MockData/mongodb_export.json"
    }
}
```

#### Cách 2: Sử dụng Environment Variable

```bash
# Windows PowerShell
$env:MockDataSettings__Enabled = "true"

# Linux/Mac
export MockDataSettings__Enabled=true
```

#### Cách 3: Chạy với Development Profile

```bash
# Development profile tự động bật Mock Mode
dotnet run --environment Development
```

### Bước 4: Chạy Application

```bash
cd TTL.HR/TTL.HR.Web
dotnet run
```

**Output khi Mock Mode được bật:**
```
===============================================
🚀 TTL HR APPLICATION
📡 Mock Data Mode: ENABLED ✅
===============================================

✅ Đã load mock data thành công. Số collections: 45
```

---

## 🔄 Chuyển Đổi Giữa Mock và API Thật

### Bật Mock Mode (Offline)
```json
// appsettings.json
{
    "MockDataSettings": {
        "Enabled": true
    }
}
```

### Tắt Mock Mode (Online - Kết nối API)
```json
// appsettings.json
{
    "MockDataSettings": {
        "Enabled": false
    }
}
```

---

## 📊 Dữ Liệu Mock Hỗ Trợ

### Collections Được Export:

#### 👥 Core HR
- `employees` - Nhân viên
- `departments` - Phòng ban
- `positions` - Chức vụ
- `contracts` - Hợp đồng
- `contract_templates` - Mẫu hợp đồng

#### ⏰ Attendance & Leave
- `attendances` - Chấm công
- `attendance_logs` - Log chấm công
- `monthly_attendance_summaries` - Tổng hợp tháng
- `leave_requests` - Đơn nghỉ phép
- `leave_types` - Loại nghỉ phép
- `overtime_requests` - Làm thêm giờ

#### 💰 Payroll & Benefits
- `payrolls` - Bảng lương
- `payroll_periods` - Kỳ lương
- `salary_components` - Thành phần lương
- `benefits` - Phúc lợi

#### 📝 Recruitment
- `job_postings` - Tin tuyển dụng
- `candidates` - Ứng viên

#### 📚 Training & Performance
- `courses` - Khóa học
- `performance_reviews` - Đánh giá hiệu suất
- `kpi_goals` - Mục tiêu KPI

#### 🏢 System
- `roles` - Vai trò
- `permissions` - Quyền
- `system_settings` - Cài đặt hệ thống
- `notifications` - Thông báo

---

## ⚠️ Lưu Ý Quan Trọng

### Hạn Chế của Mock Mode

1. **Không lưu dữ liệu thực tế**
   - POST, PUT, DELETE chỉ return success message
   - Dữ liệu không được persist vào database

2. **Không có validation từ backend**
   - Business logic validation không được thực thi
   - Cần validate ở frontend

3. **File upload không hoạt động**
   - Upload ảnh/file sẽ mock success
   - File không được lưu thực tế

4. **Authentication đơn giản hóa**
   - Login luôn thành công trong mock mode
   - Không có JWT token thực

### Khi Nào Nên Dùng Mock Mode?

✅ **NÊN dùng khi:**
- Demo cho khách hàng mà không cần setup backend
- Phát triển UI/UX mới
- Test performance frontend
- Làm việc offline (máy bay, tàu hỏa, không có internet)
- Training nhân viên mới

❌ **KHÔNG NÊN dùng khi:**
- Test business logic
- Test integration với backend
- Kiểm tra security
- Production deployment

---

## 🔧 Troubleshooting

### Lỗi: "Mock data file không tồn tại"

**Nguyên nhân:** File `mongodb_export.json` chưa được tạo hoặc đường dẫn sai.

**Giải pháp:**
```bash
# Kiểm tra file tồn tại
ls TTL.HR/TTL.HR.Web/wwwroot/MockData/mongodb_export.json

# Nếu không có, chạy lại export script
node export_mongodb_to_mock.js
```

### Lỗi: "Không thể load mock data"

**Nguyên nhân:** File JSON bị corrupt hoặc format sai.

**Giải pháp:**
```bash
# Validate JSON file
node -e "JSON.parse(require('fs').readFileSync('TTL.HR/TTL.HR.Web/wwwroot/MockData/mongodb_export.json', 'utf8'))"

# Nếu lỗi, export lại
node export_mongodb_to_mock.js
```

### Mock Mode không được bật

**Kiểm tra config:**
```bash
# Kiểm tra giá trị trong appsettings.json
cat TTL.HR/TTL.HR.Web/appsettings.json | grep -A 3 MockDataSettings

# Hoặc dùng PowerShell (Windows)
Select-String -Path "TTL.HR/TTL.HR.Web/appsettings.json" -Pattern "MockDataSettings" -Context 0,3
```

### Data không đúng/thiếu

**Giải pháp:** Export lại data mới nhất
```bash
node export_mongodb_to_mock.js
```

---

## 📈 Best Practices

### 1. Update Mock Data Định Kỳ

```bash
# Tạo script tự động export hàng tuần
# create_weekly_export.sh
#!/bin/bash
cd /path/to/TTL_ERP
node export_mongodb_to_mock.js
git add TTL.HR/TTL.HR.Web/wwwroot/MockData/
git commit -m "chore: update mock data $(date +%Y-%m-%d)"
```

### 2. Versioning Mock Data

```bash
# Lưu nhiều phiên bản mock data
node export_mongodb_to_mock.js
cp TTL.HR/TTL.HR.Web/wwwroot/MockData/mongodb_export.json \
   TTL.HR/TTL.HR.Web/wwwroot/MockData/mongodb_export_$(date +%Y%m%d).json
```

### 3. Environment-Specific Config

```json
// appsettings.Development.json
{
    "MockDataSettings": { "Enabled": true }
}

// appsettings.Staging.json
{
    "MockDataSettings": { "Enabled": false }
}

// appsettings.Production.json
{
    "MockDataSettings": { "Enabled": false }
}
```

---

## 🎓 Ví Dụ Sử Dụng

### Scenario 1: Demo Cho Khách Hàng

```bash
# 1. Export data production
node export_mongodb_to_mock.js

# 2. Bật mock mode
# Sửa appsettings.json: "Enabled": true

# 3. Chạy app
dotnet run

# ✅ App chạy hoàn toàn offline, không cần internet
```

### Scenario 2: Phát Triển Feature Mới

```bash
# 1. Switch sang Development profile
dotnet run --environment Development

# ✅ Tự động dùng mock data
# ✅ Backend team có thể làm việc song song
```

### Scenario 3: Testing Performance

```bash
# 1. Export data với 10,000 employees
# Modify export script để tạo data lớn

# 2. Chạy với mock mode
dotnet run

# ✅ Test pagination, search, filter với data lớn
# ✅ Không làm chậm database production
```

---

## 🔐 Security Notes

⚠️ **QUAN TRỌNG:**

1. **Không commit dữ liệu nhạy cảm**
   ```bash
   # .gitignore
   TTL.HR/TTL.HR.Web/wwwroot/MockData/*.json
   ```

2. **Sanitize data trước khi export**
   - Xóa passwords
   - Xóa tokens
   - Xóa thông tin cá nhân nhạy cảm (CMND, số tài khoản)

3. **Mock mode chỉ dùng cho Development**
   - Production PHẢI tắt mock mode
   - Staging nên tắt mock mode

---

## 📞 Support

Nếu gặp vấn đề, vui lòng:
1. Kiểm tra console log khi start app
2. Verify file `mongodb_export.json` tồn tại
3. Đảm bảo `MockDataSettings.Enabled = true` trong config
4. Liên hệ team DevOps nếu vấn đề còn tồn tại

---

**Version:** 1.0.0
**Last Updated:** 2026-03-19
**Author:** TTL Development Team
