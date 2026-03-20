# 🎭 Mock Data Mode - Complete Guide

## 📋 Tổng Quan

Hệ thống Mock Data cho phép chạy **website frontend hoàn toàn OFFLINE** mà không cần:
- ❌ Kết nối API backend
- ❌ Kết nối database
- ❌ Internet connection

**Use Cases:**
- ✅ Demo cho khách hàng
- ✅ Phát triển frontend độc lập
- ✅ Testing offline
- ✅ Training nhân viên mới

---

## 🚀 Quick Start (3 Bước)

### Bước 1: Kiểm Tra Lỗi
```powershell
.\check_errors.ps1
```

### Bước 2: Setup Tự Động
```powershell
.\setup_mock_mode.ps1
# → Chọn option 1: Bật Mock Mode
```

### Bước 3: Chạy App
```bash
cd TTL.HR\TTL.HR.Web
dotnet run
```

**Kết quả:**
```
===============================================
🚀 TTL HR APPLICATION
📡 Mock Data Mode: ENABLED ✅
===============================================

✅ Đã load mock data thành công. Số collections: 45
🎭 [MOCK] GET: core/Employees
```

---

## 📂 Cấu Trúc Files

```
TTL_ERP/
├── 📄 export_mongodb_to_mock.js       # Export DB → JSON
├── 📄 setup_mock_mode.ps1             # Setup tự động
├── 📄 check_errors.ps1                # Kiểm tra lỗi
├── 📘 MOCK_DATA_README.md             # File này
├── 📘 MOCK_DATA_GUIDE.md              # Chi tiết đầy đủ
├── 📘 MOCK_MODE_QUICKSTART.md         # Quick reference
├── 📘 TROUBLESHOOTING.md              # Khắc phục lỗi
│
└── TTL.HR/
    ├── TTL.HR.Application/
    │   └── Infrastructure/MockData/
    │       ├── MockDataProvider.cs    # ✅ Load JSON data
    │       └── MockHttpClient.cs      # ✅ Mock API calls
    │
    └── TTL.HR.Web/
        ├── appsettings.json           # ✅ Config Mock Mode
        ├── Program.cs                 # ✅ DI integration
        └── wwwroot/MockData/
            └── mongodb_export.json    # ✅ Mock data
```

---

## 🎯 Các File Quan Trọng

### 1. Export Script
**File:** `export_mongodb_to_mock.js`

Export toàn bộ 45+ collections từ MongoDB:
```javascript
// Core HR
employees, departments, positions, contracts...

// Attendance & Leave
attendances, leave_requests, overtime_requests...

// Payroll
payrolls, salary_components, benefits...

// System
roles, permissions, settings, notifications...
```

**Chạy:**
```bash
node export_mongodb_to_mock.js
```

### 2. Mock Data Provider
**File:** `TTL.HR.Application/Infrastructure/MockData/MockDataProvider.cs`

Load và quản lý mock data:
```csharp
public class MockDataProvider
{
    // Load từ JSON file
    public async Task<bool> LoadMockDataAsync()

    // Lấy toàn bộ collection
    public List<T> GetCollection<T>(string collectionName)

    // Lấy item theo ID
    public T? GetById<T>(string collectionName, string id)
}
```

### 3. Mock HTTP Client
**File:** `TTL.HR.Application/Infrastructure/MockData/MockHttpClient.cs`

Thay thế HttpClient thật:
```csharp
public class MockHttpClient : HttpClient
{
    // Hỗ trợ GET, POST, PUT, DELETE
    // Pagination, search, filter
    // Response format giống API thật
}
```

### 4. Configuration
**File:** `appsettings.json`

```json
{
    "MockDataSettings": {
        "Enabled": false,              // true = Mock, false = API thật
        "MockDataPath": "MockData/mongodb_export.json"
    }
}
```

### 5. Program.cs Integration
**File:** `TTL.HR.Web/Program.cs`

```csharp
// Đọc config
var useMockData = builder.Configuration.GetValue<bool>("MockDataSettings:Enabled");

if (useMockData)
{
    // MOCK MODE
    builder.Services.AddSingleton<MockDataProvider>();
    builder.Services.AddScoped<HttpClient>(sp => new MockHttpClient(...));
}
else
{
    // API MODE
    builder.Services.AddScoped(sp => new HttpClient { BaseAddress = ... });
}
```

---

## 🔄 Toggle Mock Mode

### Cách 1: Sửa File Config
```json
// Bật Mock
"MockDataSettings": { "Enabled": true }

// Tắt Mock
"MockDataSettings": { "Enabled": false }
```

### Cách 2: Environment Variable
```powershell
# Windows
$env:MockDataSettings__Enabled = "true"

# Linux/Mac
export MockDataSettings__Enabled=true
```

### Cách 3: Script Tự Động
```powershell
.\setup_mock_mode.ps1
# → Menu chọn bật/tắt
```

---

## 📊 Data Support

### Collections (45+)
- 👥 **Core HR**: employees, departments, positions, contracts
- ⏰ **Attendance**: attendances, leave_requests, overtime
- 💰 **Payroll**: payrolls, salary_components, benefits
- 📝 **Recruitment**: job_postings, candidates
- 📚 **Training**: courses, enrollments
- 🎯 **Performance**: reviews, kpi_goals
- 🏢 **System**: roles, permissions, settings
- 🌍 **Geo**: countries, provinces, districts, wards

### API Endpoints Supported
```
✅ GET    /core/Employees
✅ GET    /core/Employees/{id}
✅ POST   /core/Employees
✅ PUT    /core/Employees/{id}
✅ DELETE /core/Employees/{id}

✅ Pagination: ?page=1&pageSize=10
✅ Search: ?searchTerm=keyword
✅ All endpoints tương tự cho các modules khác
```

---

## ⚠️ Limitations

### Mock Mode Là READ-ONLY
```csharp
// ✅ GET - Hoạt động bình thường
var employees = await GetEmployeesAsync();

// ⚠️  POST/PUT/DELETE - Chỉ return success, không lưu thật
await CreateEmployeeAsync(employee);  // Success message, nhưng không lưu
await UpdateEmployeeAsync(employee);  // Success message, nhưng không update
await DeleteEmployeeAsync(id);        // Success message, nhưng không delete
```

### Không Có
- ❌ Real-time validation từ backend
- ❌ Business logic từ backend
- ❌ File upload thực tế
- ❌ Email gửi thực tế
- ❌ Database triggers
- ❌ Background jobs

---

## 🔧 Troubleshooting

### Lỗi Build
```powershell
# Kiểm tra lỗi
.\check_errors.ps1

# Xem chi tiết
dotnet build -v detailed > build_log.txt
```

### Lỗi Runtime
```powershell
# Check console output
dotnet run

# Look for:
# ✅ "Mock Data Mode: ENABLED"
# ✅ "Đã load mock data thành công"
# ❌ "Mock data file không tồn tại"
```

### Data Không Đúng
```bash
# Export lại data mới
node export_mongodb_to_mock.js

# Copy vào wwwroot
cp TTL.HR/TTL.HR.Web/MockData/mongodb_export.json \
   TTL.HR/TTL.HR.Web/wwwroot/MockData/
```

**Xem thêm:** [TROUBLESHOOTING.md](./TROUBLESHOOTING.md)

---

## 📚 Documentation

| File | Mục Đích |
|------|----------|
| [MOCK_DATA_README.md](./MOCK_DATA_README.md) | **Overview (file này)** |
| [MOCK_MODE_QUICKSTART.md](./MOCK_MODE_QUICKSTART.md) | Quick reference |
| [MOCK_DATA_GUIDE.md](./MOCK_DATA_GUIDE.md) | Chi tiết đầy đủ |
| [TROUBLESHOOTING.md](./TROUBLESHOOTING.md) | Khắc phục lỗi |

---

## 🎬 Demo Scenarios

### Scenario 1: Demo Offline Cho Khách Hàng
```powershell
# 1. Export production data
node export_mongodb_to_mock.js

# 2. Bật mock mode
.\setup_mock_mode.ps1  # → Option 1

# 3. Chạy app
cd TTL.HR\TTL.HR.Web
dotnet run

# ✅ App chạy hoàn toàn offline
# ✅ Không cần internet, VPN, database
# ✅ Data là production data thật
```

### Scenario 2: Develop Feature Mới
```powershell
# 1. Bật Development environment
# → appsettings.Development.json có Mock = true

# 2. Chạy
dotnet run --environment Development

# ✅ Frontend dev không phụ thuộc backend
# ✅ Backend team có thể làm việc song song
```

### Scenario 3: Testing Performance
```bash
# Test với 10,000 employees
# Modify export script để tạo data lớn

dotnet run

# ✅ Test pagination với data lớn
# ✅ Test search performance
# ✅ Không ảnh hưởng production DB
```

---

## 🔐 Security Notes

### Trước Khi Export
```javascript
// Sanitize sensitive data
employee.Password = null;          // ❌ Xóa password
employee.SSN = "XXX-XX-XXXX";      // ❌ Mask SSN
employee.BankAccount = null;       // ❌ Xóa số tài khoản
```

### .gitignore
```gitignore
# Không commit mock data có thông tin nhạy cảm
wwwroot/MockData/*.json
MockData/*.json
```

### Production
```json
// Production PHẢI tắt mock
{
    "MockDataSettings": {
        "Enabled": false  // ⚠️  QUAN TRỌNG!
    }
}
```

---

## 📞 Support

### Kiểm Tra Trước
1. ✅ Chạy `.\check_errors.ps1`
2. ✅ Đọc console output khi start app
3. ✅ Verify `mongodb_export.json` tồn tại
4. ✅ Check `MockDataSettings.Enabled = true`

### Debug Commands
```bash
# Build verbose
dotnet build -v detailed

# Clean rebuild
dotnet clean && dotnet build

# Check dependencies
dotnet list package

# Run debug
dotnet run --configuration Debug
```

---

## ✅ Checklist Setup

- [ ] Chạy `node export_mongodb_to_mock.js`
- [ ] File `mongodb_export.json` đã tạo
- [ ] Copy vào `wwwroot/MockData/`
- [ ] `appsettings.json` có `MockDataSettings`
- [ ] `MockDataSettings.Enabled = true`
- [ ] `Program.cs` đã integrate MockDataProvider
- [ ] Build thành công
- [ ] App start với message "Mock Data Mode: ENABLED ✅"

---

## 🎯 Next Steps

1. **Setup lần đầu:**
   ```powershell
   .\check_errors.ps1
   .\setup_mock_mode.ps1
   ```

2. **Update data định kỳ:**
   ```bash
   node export_mongodb_to_mock.js
   ```

3. **Switch modes:**
   ```json
   "Enabled": true  // Mock
   "Enabled": false // API
   ```

---

**Version:** 1.0.0
**Last Updated:** 2026-03-19
**Author:** TTL Development Team

---

## 📖 Quick Links

- [Quick Start Guide](./MOCK_MODE_QUICKSTART.md)
- [Full Documentation](./MOCK_DATA_GUIDE.md)
- [Troubleshooting](./TROUBLESHOOTING.md)
- [Export Script](./export_mongodb_to_mock.js)
- [Setup Script](./setup_mock_mode.ps1)
- [Check Errors](./check_errors.ps1)
