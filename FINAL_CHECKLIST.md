# ✅ FINAL CHECKLIST - Mock Data Mode

## 🎯 Trước Khi Chạy Script

### Bước 1: Verify Files
```powershell
# Check tất cả files quan trọng
ls export_mongodb_to_mock.js
ls setup_mock_mode.ps1
ls check_errors.ps1
ls TTL.HR\TTL.HR.Application\Infrastructure\MockData\*.cs
ls TTL.HR\TTL.HR.Web\appsettings.json
```

**Expected:** Tất cả files tồn tại ✅

---

### Bước 2: Update MongoDB Connection String

#### File: `export_mongodb_to_mock.js`

**⚠️ QUAN TRỌNG:** Cập nhật dòng này với thông tin MongoDB của bạn:

```javascript
// Line 12-13
const MONGO_URI = 'mongodb+srv://username:password@cluster.mongodb.net/...';
const DATABASE_NAME = 'YOUR_DATABASE_NAME'; // ⚠️ Thay đúng tên database
```

**Ví dụ:**
```javascript
const MONGO_URI = 'mongodb+srv://trankimmyvuong_db_user:asXBCHnSthpXXSqM@cluster0.0a3ikr3.mongodb.net/?retryWrites=true&w=majority&appName=Cluster0';
const DATABASE_NAME = 'TTL_ERP_Production'; // Thay tên DB thực tế của bạn
```

**Lấy thông tin từ đâu?**
- Check file: `TTL_API/src/Services/CoreService/TTL.CoreService.Api/appsettings.json`
- Hoặc: `TTL_ERP/.env`
- Hoặc: MongoDB Atlas dashboard

---

### Bước 3: Install Dependencies

```bash
# Cài đặt MongoDB driver cho Node.js
npm install mongodb

# Verify
npm list mongodb
```

**Expected:**
```
mongodb@7.1.0
```

---

### Bước 4: Test MongoDB Connection

```bash
# Test connection trước khi export
node -e "const {MongoClient}=require('mongodb');new MongoClient('YOUR_MONGO_URI').connect().then(()=>{console.log('✅ Connected!');process.exit(0)}).catch(e=>{console.log('❌',e.message);process.exit(1)})"
```

**Nếu lỗi:**
- Check username/password
- Check IP whitelist trên MongoDB Atlas
- Check network connection

---

## 🚀 Run Scripts

### Option 1: Chạy Từng Bước (Recommended for first time)

```powershell
# 1. Check lỗi
.\check_errors.ps1

# 2. Export data
node export_mongodb_to_mock.js

# 3. Verify export
ls TTL.HR\TTL.HR.Web\MockData\mongodb_export.json

# 4. Setup mock mode
.\setup_mock_mode.ps1
# → Chọn option 1: Bật Mock Mode

# 5. Chạy app
cd TTL.HR\TTL.HR.Web
dotnet run
```

### Option 2: Tất Cả Trong Một

```powershell
# Script tự động sẽ làm tất cả
.\setup_mock_mode.ps1
```

---

## 📋 Checklist Chi Tiết

### Pre-Flight Checks

- [ ] ✅ Đã cài Node.js (check: `node --version`)
- [ ] ✅ Đã cài npm (check: `npm --version`)
- [ ] ✅ Đã cài .NET SDK (check: `dotnet --version`)
- [ ] ✅ Đã cài MongoDB driver (check: `npm list mongodb`)
- [ ] ✅ MongoDB connection string đã cập nhật
- [ ] ✅ Database name đã cập nhật
- [ ] ✅ Có thể kết nối MongoDB (test connection thành công)

### Build Checks

- [ ] ✅ `dotnet build` → Success, 0 errors
- [ ] ✅ Không có CS0246 errors
- [ ] ✅ MockDataProvider.cs có `using Microsoft.AspNetCore.Hosting`
- [ ] ✅ MockHttpClient.cs không dùng `System.Web`
- [ ] ✅ MockHttpClient.cs có method `ParseQueryString()`

### Export Checks

- [ ] ✅ `node export_mongodb_to_mock.js` → Success
- [ ] ✅ File `TTL.HR/TTL.HR.Web/MockData/mongodb_export.json` tồn tại
- [ ] ✅ File size > 0 bytes (should be 2-50MB)
- [ ] ✅ JSON format valid (check với `Get-Content ... | ConvertFrom-Json`)

### Setup Checks

- [ ] ✅ `.\setup_mock_mode.ps1` chạy thành công
- [ ] ✅ File copy vào `wwwroot/MockData/mongodb_export.json`
- [ ] ✅ `appsettings.json` có `MockDataSettings.Enabled = true`

### Runtime Checks

- [ ] ✅ `dotnet run` → App starts
- [ ] ✅ Console hiển thị "Mock Data Mode: ENABLED ✅"
- [ ] ✅ Console hiển thị "Đã load mock data thành công"
- [ ] ✅ Browser mở được http://localhost:5000
- [ ] ✅ Trang chủ load được
- [ ] ✅ Menu hoạt động
- [ ] ✅ Danh sách nhân viên hiển thị data từ mock

---

## 🔍 Troubleshooting Quick Reference

### Lỗi: "Cannot find module 'mongodb'"
```bash
npm install mongodb
```

### Lỗi: "MongoServerError: Authentication failed"
```javascript
// Kiểm tra lại connection string
const MONGO_URI = '...'; // Username/password đúng?
```

### Lỗi: "CS0246: IWebHostEnvironment"
```csharp
// Thêm vào MockDataProvider.cs
using Microsoft.AspNetCore.Hosting;
```

### Lỗi: "Mock data file không tồn tại"
```powershell
# Check path
ls TTL.HR\TTL.HR.Web\wwwroot\MockData\mongodb_export.json

# Nếu không có, chạy lại
node export_mongodb_to_mock.js
.\setup_mock_mode.ps1
```

### Lỗi: Build failed
```bash
# Clean và rebuild
dotnet clean
dotnet restore
dotnet build -v detailed
```

---

## 🎯 Expected Output

### 1. Export Script
```
🔌 Đang kết nối MongoDB...
✅ Kết nối MongoDB thành công!
📦 Đang export collection: employees...
   ✅ Export 150 documents từ employees
📦 Đang export collection: departments...
   ✅ Export 12 documents từ departments
...
🎉 HOÀN THÀNH!
📁 File mock data đã được lưu tại: ...
📊 Tổng số collections: 45
📝 Tổng số documents: 2,350
```

### 2. Setup Script
```
=============================================
🚀 TTL ERP - MOCK DATA MODE SETUP
=============================================

📦 Bước 1: Export dữ liệu từ MongoDB...
   ✅ Export thành công!

📁 Bước 2: Tạo thư mục MockData...
   ✅ Đã tạo thư mục: ...

📋 Bước 3: Copy file mock data...
   ✅ Đã copy file mock data (Size: 15.34 MB)

💾 Bước 4: Backup configuration...
   ✅ Đã backup: ...

🎭 Đang bật Mock Mode...
   ✅ Mock Mode đã được BẬT

🚀 Đang khởi động ứng dụng...
```

### 3. App Runtime
```
===============================================
🚀 TTL HR APPLICATION
📡 Mock Data Mode: ENABLED ✅
===============================================

✅ Đã load mock data thành công. Số collections: 45
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: http://localhost:5000
info: Microsoft.Hosting.Lifetime[0]
      Application started. Press Ctrl+C to shut down.

🎭 [MOCK] GET: core/Employees?page=1&pageSize=10
🎭 [MOCK] GET: core/Departments
🎭 [MOCK] GET: core/Positions
```

---

## 💡 Pro Tips

### 1. Test Connection First
```bash
# Trước khi export, test connection
node -e "require('mongodb').MongoClient.connect('YOUR_URI').then(()=>console.log('OK')).catch(e=>console.log(e.message))"
```

### 2. Limit Records for Testing
```javascript
// Trong export_mongodb_to_mock.js, line ~60
// Nếu muốn test với ít data:
const data = await collection.find({}).limit(100).toArray();
```

### 3. Check File Size
```powershell
# Nếu file quá lớn (>50MB)
$fileSize = (Get-Item "TTL.HR\TTL.HR.Web\wwwroot\MockData\mongodb_export.json").Length / 1MB
Write-Host "File size: $fileSize MB"
```

### 4. Validate JSON
```powershell
# Kiểm tra JSON valid không
try {
    Get-Content "TTL.HR\TTL.HR.Web\wwwroot\MockData\mongodb_export.json" | ConvertFrom-Json | Out-Null
    Write-Host "✅ JSON valid"
} catch {
    Write-Host "❌ JSON invalid: $($_.Exception.Message)"
}
```

---

## 🚨 Common Mistakes

### ❌ Mistake 1: Quên cập nhật DATABASE_NAME
```javascript
// SAI
const DATABASE_NAME = 'MONGODB_DATABASE_NAME'; // Placeholder!

// ĐÚNG
const DATABASE_NAME = 'TTL_ERP_Production'; // Tên thật
```

### ❌ Mistake 2: File ở sai vị trí
```
❌ TTL.HR/TTL.HR.Web/MockData/mongodb_export.json
✅ TTL.HR/TTL.HR.Web/wwwroot/MockData/mongodb_export.json
   ^^^^^^^^^^ wwwroot is important!
```

### ❌ Mistake 3: Quên enable Mock Mode
```json
// appsettings.json
{
    "MockDataSettings": {
        "Enabled": false  // ❌ Vẫn dùng API thật!
    }
}
```

### ❌ Mistake 4: Chạy production với Mock
```json
// appsettings.json (PRODUCTION)
{
    "MockDataSettings": {
        "Enabled": true  // ❌ NGUY HIỂM! Production phải = false
    }
}
```

---

## ✅ Success Indicators

Bạn biết mọi thứ OK khi:

1. ✅ `dotnet build` → 0 errors
2. ✅ `node export_mongodb_to_mock.js` → "HOÀN THÀNH"
3. ✅ File JSON > 0 bytes
4. ✅ `.\check_errors.ps1` → "HOÀN HẢO! Không có lỗi"
5. ✅ `dotnet run` → "Mock Data Mode: ENABLED ✅"
6. ✅ Browser → Trang load, data hiển thị
7. ✅ Console log → "[MOCK] GET: ..." khi click menu

---

## 🎉 Ready to Run!

Nếu đã check hết checklist trên, bạn sẵn sàng:

```powershell
# GO!
.\setup_mock_mode.ps1
```

**Good luck! 🚀**

---

## 📞 Need Help?

**Đọc theo thứ tự:**
1. [TROUBLESHOOTING.md](./TROUBLESHOOTING.md) - Khắc phục lỗi
2. [FIXES_APPLIED.md](./FIXES_APPLIED.md) - Lỗi đã fix
3. [MOCK_DATA_GUIDE.md](./MOCK_DATA_GUIDE.md) - Hướng dẫn đầy đủ

**Debug commands:**
```bash
# Build verbose
dotnet build -v detailed > build.log

# Test MongoDB
node -e "require('mongodb').MongoClient.connect('URI').then(()=>console.log('OK'))"

# Validate JSON
Get-Content file.json | ConvertFrom-Json
```

---

**Version:** 1.0.0
**Last Updated:** 2026-03-19
**Status:** ✅ Ready to run
