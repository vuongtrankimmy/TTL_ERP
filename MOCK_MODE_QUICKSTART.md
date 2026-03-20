# 🚀 Mock Data Mode - Quick Start

## TL;DR - Cách Nhanh Nhất

```powershell
# Chạy script tự động
.\setup_mock_mode.ps1

# Chọn option 1 để bật Mock Mode
# App sẽ chạy OFFLINE, không cần API backend
```

---

## 📋 3 Bước Đơn Giản

### 1️⃣ Export Database
```bash
node export_mongodb_to_mock.js
```

### 2️⃣ Bật Mock Mode
Sửa `appsettings.json`:
```json
{
    "MockDataSettings": {
        "Enabled": true
    }
}
```

### 3️⃣ Chạy App
```bash
cd TTL.HR/TTL.HR.Web
dotnet run
```

✅ **Done!** App chạy hoàn toàn OFFLINE!

---

## 🔄 Toggle On/Off

### Bật Mock (Offline)
```json
"MockDataSettings": { "Enabled": true }
```

### Tắt Mock (Online - API thật)
```json
"MockDataSettings": { "Enabled": false }
```

---

## 📊 Kết Quả

### Khi Mock Mode BẬT:
```
===============================================
🚀 TTL HR APPLICATION
📡 Mock Data Mode: ENABLED ✅
===============================================

✅ Đã load mock data thành công. Số collections: 45
🎭 [MOCK] GET: core/Employees
🎭 [MOCK] GET: core/Departments
```

### Khi Mock Mode TẮT:
```
===============================================
🚀 TTL HR APPLICATION
📡 Mock Data Mode: DISABLED ❌
===============================================

Connecting to API: http://gateway.tantanloc.com/
```

---

## ⚠️ Lưu Ý

- ✅ **Mock Mode**: Read-only, không lưu data thật
- ✅ **Production**: PHẢI tắt Mock Mode
- ✅ **Update data**: Chạy lại `export_mongodb_to_mock.js`

---

## 📖 Chi Tiết

Xem hướng dẫn đầy đủ tại: [MOCK_DATA_GUIDE.md](./MOCK_DATA_GUIDE.md)
