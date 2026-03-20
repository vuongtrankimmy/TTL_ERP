# 🚀 MOCK DATA MODE - BẮT ĐẦU TỪ ĐÂY

## ⚡ Quick Start (Copy & Paste)

```powershell
# 1. Kiểm tra lỗi
.\check_errors.ps1

# 2. Setup tự động
.\setup_mock_mode.ps1

# 3. Chạy app
cd TTL.HR\TTL.HR.Web
dotnet run
```

**Kết quả:**
```
🚀 TTL HR APPLICATION
📡 Mock Data Mode: ENABLED ✅
✅ Đã load mock data thành công!
```

---

## 📚 Documentation

| Đọc File Này | Khi Nào |
|--------------|---------|
| **[START_HERE.md](./START_HERE.md)** | **← BẮT ĐẦU Ở ĐÂY** |
| [FIXES_APPLIED.md](./FIXES_APPLIED.md) | Xem các lỗi đã được sửa |
| [MOCK_MODE_QUICKSTART.md](./MOCK_MODE_QUICKSTART.md) | Quick reference |
| [MOCK_DATA_README.md](./MOCK_DATA_README.md) | Overview đầy đủ |
| [MOCK_DATA_GUIDE.md](./MOCK_DATA_GUIDE.md) | Hướng dẫn chi tiết |
| [TROUBLESHOOTING.md](./TROUBLESHOOTING.md) | Gặp lỗi? Đọc đây |

---

## ✅ Đã Sửa Lỗi

### Lỗi Build
- ✅ **CS0246: IWebHostEnvironment** → Fixed
- ✅ **System.Web dependency** → Removed
- ✅ **ParseQueryString** → Implemented helper

**Chi tiết:** [FIXES_APPLIED.md](./FIXES_APPLIED.md)

---

## 🎯 Mock Mode Là Gì?

**Chạy website frontend KHÔNG CẦN:**
- ❌ API backend
- ❌ Database
- ❌ Internet

**Tất cả data từ file JSON local!**

---

## 📂 Files Quan Trọng

```
TTL_ERP/
├── 📄 START_HERE.md                    ← BẮT ĐẦU Ở ĐÂY
├── 📄 check_errors.ps1                 ← Kiểm tra lỗi
├── 📄 setup_mock_mode.ps1              ← Setup tự động
├── 📄 export_mongodb_to_mock.js        ← Export DB
│
├── 📘 FIXES_APPLIED.md                 ← Các lỗi đã fix
├── 📘 MOCK_DATA_README.md              ← Overview
├── 📘 MOCK_DATA_GUIDE.md               ← Chi tiết
├── 📘 TROUBLESHOOTING.md               ← Khắc phục lỗi
│
└── TTL.HR/
    ├── TTL.HR.Application/
    │   └── Infrastructure/MockData/
    │       ├── MockDataProvider.cs     ✅ Fixed
    │       └── MockHttpClient.cs       ✅ Fixed
    │
    └── TTL.HR.Web/
        ├── appsettings.json            ✅ Updated
        ├── Program.cs                  ✅ Updated
        └── wwwroot/MockData/
            └── mongodb_export.json     ← Mock data
```

---

## 🔄 Workflow

### Lần Đầu Tiên
```powershell
# 1. Check errors
.\check_errors.ps1

# 2. Export data từ MongoDB
node export_mongodb_to_mock.js

# 3. Setup mock mode
.\setup_mock_mode.ps1  # → Chọn option 1

# 4. Run
cd TTL.HR\TTL.HR.Web
dotnet run
```

### Lần Sau
```powershell
# Bật/tắt Mock Mode
.\setup_mock_mode.ps1

# Run
cd TTL.HR\TTL.HR.Web
dotnet run
```

---

## 🎛️ Bật/Tắt Mock Mode

### Cách 1: Script (Khuyến nghị)
```powershell
.\setup_mock_mode.ps1
# → Menu:
#   1. BẬT Mock Mode (Offline)
#   2. TẮT Mock Mode (Online)
```

### Cách 2: Manual
Sửa `appsettings.json`:
```json
{
    "MockDataSettings": {
        "Enabled": true   // true = Mock, false = API thật
    }
}
```

---

## 📊 Khi Nào Dùng Mock Mode?

### ✅ NÊN Dùng
- Demo cho khách hàng (offline)
- Phát triển UI/UX mới
- Training nhân viên
- Test performance
- Làm việc offline

### ❌ KHÔNG NÊN Dùng
- Production deployment
- Test business logic
- Test integration
- Test security

---

## 🔍 Verify Setup

### Check 1: Build
```bash
dotnet build
```
**Expected:** `Build succeeded. 0 Error(s)`

### Check 2: Files
```powershell
.\check_errors.ps1
```
**Expected:** `✅ HOÀN HẢO! Không có lỗi.`

### Check 3: Mock Data
```bash
ls TTL.HR\TTL.HR.Web\wwwroot\MockData\mongodb_export.json
```
**Expected:** File tồn tại (size ~2-50MB)

### Check 4: Run
```bash
dotnet run
```
**Expected:**
```
🚀 TTL HR APPLICATION
📡 Mock Data Mode: ENABLED ✅
```

---

## ⚠️ Lưu Ý Quan Trọng

### Mock Mode = Read-Only
```csharp
// ✅ GET - Hoạt động
var employees = await GetEmployeesAsync();

// ⚠️  POST/PUT/DELETE - Chỉ mock, không lưu thật
await CreateEmployeeAsync(employee);  // Success nhưng không lưu
```

### Production
```json
// appsettings.json (PRODUCTION)
{
    "MockDataSettings": {
        "Enabled": false  // ⚠️ PHẢI = false
    }
}
```

---

## 🆘 Gặp Lỗi?

### Lỗi Build
```powershell
# Đọc đây:
cat FIXES_APPLIED.md

# Hoặc:
cat TROUBLESHOOTING.md
```

### Lỗi Runtime
```powershell
# Check setup:
.\check_errors.ps1

# Xem console output khi dotnet run
```

### Data Không Đúng
```bash
# Export lại:
node export_mongodb_to_mock.js
```

---

## 💡 Tips

### Update Data Thường Xuyên
```bash
# Mỗi khi DB có data mới:
node export_mongodb_to_mock.js
```

### Development vs Production
```json
// appsettings.Development.json
{ "MockDataSettings": { "Enabled": true } }

// appsettings.json (production)
{ "MockDataSettings": { "Enabled": false } }
```

### Performance
```bash
# Nếu file JSON quá lớn (>50MB):
# Modify export script để limit records
```

---

## 🎯 Next Steps

1. **Verify setup:**
   ```powershell
   .\check_errors.ps1
   ```

2. **Export data:**
   ```bash
   node export_mongodb_to_mock.js
   ```

3. **Bật mock mode:**
   ```powershell
   .\setup_mock_mode.ps1  # → Option 1
   ```

4. **Chạy app:**
   ```bash
   cd TTL.HR\TTL.HR.Web
   dotnet run
   ```

5. **Test:**
   - Mở browser: http://localhost:5000
   - Kiểm tra tất cả chức năng
   - Verify console log

---

## 📞 Support

**Có 3 cách để tự giải quyết:**

1. **Check errors:**
   ```powershell
   .\check_errors.ps1
   ```

2. **Read troubleshooting:**
   ```
   TROUBLESHOOTING.md
   ```

3. **Verify files exist:**
   ```powershell
   ls TTL.HR\TTL.HR.Application\Infrastructure\MockData\*.cs
   ls TTL.HR\TTL.HR.Web\wwwroot\MockData\*.json
   ```

---

## ✅ Checklist

**Trước khi chạy app, verify:**

- [ ] Đã chạy `check_errors.ps1` → Không có lỗi
- [ ] Đã chạy `export_mongodb_to_mock.js` → File JSON đã tạo
- [ ] File `mongodb_export.json` tồn tại trong `wwwroot/MockData/`
- [ ] `appsettings.json` có `MockDataSettings.Enabled = true`
- [ ] `dotnet build` → Success
- [ ] Ready to run!

---

## 🚀 TL;DR

```powershell
# All-in-one command:
.\setup_mock_mode.ps1

# Rồi:
cd TTL.HR\TTL.HR.Web
dotnet run

# Done! ✅
```

---

**Version:** 1.0.0
**Status:** ✅ Ready to use
**Last Updated:** 2026-03-19

---

## 📖 Đọc Thêm

- [MOCK_DATA_README.md](./MOCK_DATA_README.md) - Overview
- [MOCK_DATA_GUIDE.md](./MOCK_DATA_GUIDE.md) - Chi tiết đầy đủ
- [TROUBLESHOOTING.md](./TROUBLESHOOTING.md) - Khắc phục lỗi
- [FIXES_APPLIED.md](./FIXES_APPLIED.md) - Lỗi đã fix

**Happy Coding! 🎉**
