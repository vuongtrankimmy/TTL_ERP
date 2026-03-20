# ⚡ QUICK RUN GUIDE - Copy & Paste

## 🎯 Nếu Bạn Chỉ Muốn Chạy Ngay

### Bước 1: Cập nhật Connection String

Mở file `export_mongodb_to_mock.js`, tìm dòng 12-13:

```javascript
// THAY ĐỔI 2 DÒNG NÀY:
const MONGO_URI = 'mongodb+srv://username:password@cluster...';
const DATABASE_NAME = 'TTL_ERP_Production'; // ← Tên database thật của bạn
```

### Bước 2: Chạy Setup Script

```powershell
# Copy & paste dòng này:
.\setup_mock_mode.ps1
```

Chọn **Option 1** khi được hỏi.

### Bước 3: Chạy App

```powershell
cd TTL.HR\TTL.HR.Web
dotnet run
```

Mở browser: http://localhost:5000

**DONE! ✅**

---

## 🔧 Nếu Gặp Lỗi

### Lỗi: "npm không được nhận dạng"

```powershell
# Cài Node.js từ:
# https://nodejs.org/

# Sau đó chạy lại setup script
```

### Lỗi: "Cannot find module 'mongodb'"

```bash
npm install mongodb
.\setup_mock_mode.ps1
```

### Lỗi: Build failed

```bash
dotnet clean
dotnet restore
dotnet build
```

### Lỗi: MongoDB connection failed

```javascript
// Kiểm tra lại 2 dòng này trong export_mongodb_to_mock.js:
const MONGO_URI = '...'; // ← Đúng chưa?
const DATABASE_NAME = '...'; // ← Đúng chưa?
```

**Lấy connection string từ đâu?**
- File: `TTL_API/src/Services/CoreService/TTL.CoreService.Api/appsettings.json`
- Tìm: `"MongoDbSettings"` → `"ConnectionString"`

---

## 📞 Vẫn Không Chạy Được?

**Chạy script kiểm tra lỗi:**

```powershell
.\check_errors.ps1
```

Script sẽ cho bạn biết vấn đề ở đâu.

**Hoặc đọc:**
- [FINAL_CHECKLIST.md](./FINAL_CHECKLIST.md) - Checklist đầy đủ
- [TROUBLESHOOTING.md](./TROUBLESHOOTING.md) - Khắc phục lỗi

---

## 🎯 TL;DR

```powershell
# 1. Update connection string trong export_mongodb_to_mock.js
# 2. Chạy:
npm install mongodb
.\setup_mock_mode.ps1

# 3. Chọn Option 1
# 4. Done!
```

**Happy Coding! 🚀**
