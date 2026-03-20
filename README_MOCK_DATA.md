# 📚 Mock Data Mode - Documentation Index

## 🚀 BẮT ĐẦU Ở ĐÂY

### Người Dùng Mới? → [QUICK_RUN_GUIDE.md](./QUICK_RUN_GUIDE.md) ⭐
3 bước đơn giản để chạy ngay!

### Đã Quen? → [setup_mock_mode.ps1](./setup_mock_mode.ps1)
Chỉ cần chạy script này.

---

## 📖 Documentation Map

| File | Khi Nào Đọc | Priority |
|------|-------------|----------|
| [QUICK_RUN_GUIDE.md](./QUICK_RUN_GUIDE.md) | **BẮT ĐẦU Ở ĐÂY** | ⭐⭐⭐ |
| [START_HERE.md](./START_HERE.md) | Người mới, muốn hiểu overview | ⭐⭐⭐ |
| [FINAL_CHECKLIST.md](./FINAL_CHECKLIST.md) | Trước khi chạy lần đầu | ⭐⭐⭐ |
| [FIXES_APPLIED.md](./FIXES_APPLIED.md) | Muốn biết lỗi nào đã fix | ⭐⭐ |
| [MOCK_MODE_QUICKSTART.md](./MOCK_MODE_QUICKSTART.md) | Quick reference | ⭐⭐ |
| [MOCK_DATA_README.md](./MOCK_DATA_README.md) | Overview tổng quan | ⭐⭐ |
| [MOCK_DATA_GUIDE.md](./MOCK_DATA_GUIDE.md) | Chi tiết đầy đủ 7000+ words | ⭐ |
| [TROUBLESHOOTING.md](./TROUBLESHOOTING.md) | Khi gặp lỗi | ⭐⭐⭐ |

---

## 🛠️ Scripts

| Script | Mục Đích |
|--------|----------|
| [setup_mock_mode.ps1](./setup_mock_mode.ps1) | **Setup tự động** - Chạy cái này! |
| [check_errors.ps1](./check_errors.ps1) | Kiểm tra lỗi |
| [export_mongodb_to_mock.js](./export_mongodb_to_mock.js) | Export DB → JSON |

---

## 📂 Code Files

| File | Mô Tả |
|------|-------|
| [MockDataProvider.cs](./TTL.HR/TTL.HR.Application/Infrastructure/MockData/MockDataProvider.cs) | Load mock data từ JSON |
| [MockHttpClient.cs](./TTL.HR/TTL.HR.Application/Infrastructure/MockData/MockHttpClient.cs) | Mock API calls |
| [Program.cs](./TTL.HR/TTL.HR.Web/Program.cs) | DI integration |
| [appsettings.json](./TTL.HR/TTL.HR.Web/appsettings.json) | Configuration |

---

## 🎯 Quick Navigation

### Tôi muốn...

**...chạy ngay không cần đọc gì:**
→ [QUICK_RUN_GUIDE.md](./QUICK_RUN_GUIDE.md)

**...hiểu Mock Mode là gì:**
→ [START_HERE.md](./START_HERE.md)

**...fix lỗi build:**
→ [FIXES_APPLIED.md](./FIXES_APPLIED.md)

**...fix lỗi runtime:**
→ [TROUBLESHOOTING.md](./TROUBLESHOOTING.md)

**...checklist đầy đủ:**
→ [FINAL_CHECKLIST.md](./FINAL_CHECKLIST.md)

**...chi tiết kỹ thuật:**
→ [MOCK_DATA_GUIDE.md](./MOCK_DATA_GUIDE.md)

**...quick reference:**
→ [MOCK_MODE_QUICKSTART.md](./MOCK_MODE_QUICKSTART.md)

---

## ⚡ Super Quick Start

```powershell
# 1. Update connection trong export_mongodb_to_mock.js
# 2. Chạy:
.\setup_mock_mode.ps1

# 3. Done! ✅
```

---

## 🎓 Learning Path

### Beginner
1. [QUICK_RUN_GUIDE.md](./QUICK_RUN_GUIDE.md) - Chạy thử
2. [START_HERE.md](./START_HERE.md) - Hiểu cơ bản
3. [MOCK_MODE_QUICKSTART.md](./MOCK_MODE_QUICKSTART.md) - Reference nhanh

### Intermediate
4. [FINAL_CHECKLIST.md](./FINAL_CHECKLIST.md) - Setup đầy đủ
5. [MOCK_DATA_README.md](./MOCK_DATA_README.md) - Overview chi tiết
6. [TROUBLESHOOTING.md](./TROUBLESHOOTING.md) - Giải quyết vấn đề

### Advanced
7. [MOCK_DATA_GUIDE.md](./MOCK_DATA_GUIDE.md) - Deep dive
8. [Code files](./TTL.HR/TTL.HR.Application/Infrastructure/MockData/) - Source code

---

## 🔍 Search by Topic

### Setup & Installation
- [QUICK_RUN_GUIDE.md](./QUICK_RUN_GUIDE.md)
- [FINAL_CHECKLIST.md](./FINAL_CHECKLIST.md)
- [setup_mock_mode.ps1](./setup_mock_mode.ps1)

### Configuration
- [MOCK_DATA_README.md](./MOCK_DATA_README.md) - Section "Configuration"
- [MOCK_DATA_GUIDE.md](./MOCK_DATA_GUIDE.md) - Section "Cấu Hình"
- [appsettings.json](./TTL.HR/TTL.HR.Web/appsettings.json)

### Troubleshooting
- [TROUBLESHOOTING.md](./TROUBLESHOOTING.md) - Đầy đủ
- [FIXES_APPLIED.md](./FIXES_APPLIED.md) - Lỗi đã fix
- [check_errors.ps1](./check_errors.ps1) - Auto check

### Technical Details
- [MOCK_DATA_GUIDE.md](./MOCK_DATA_GUIDE.md)
- [MockDataProvider.cs](./TTL.HR/TTL.HR.Application/Infrastructure/MockData/MockDataProvider.cs)
- [MockHttpClient.cs](./TTL.HR/TTL.HR.Application/Infrastructure/MockData/MockHttpClient.cs)

### Quick Reference
- [MOCK_MODE_QUICKSTART.md](./MOCK_MODE_QUICKSTART.md)
- [START_HERE.md](./START_HERE.md) - Section "TL;DR"

---

## 📊 File Sizes

| File | Lines | Size |
|------|-------|------|
| QUICK_RUN_GUIDE.md | ~80 | ~2 KB |
| START_HERE.md | ~300 | ~10 KB |
| MOCK_DATA_README.md | ~500 | ~20 KB |
| MOCK_DATA_GUIDE.md | ~800 | ~35 KB |
| TROUBLESHOOTING.md | ~600 | ~25 KB |
| FINAL_CHECKLIST.md | ~400 | ~15 KB |
| FIXES_APPLIED.md | ~300 | ~12 KB |

---

## 🎯 Recommended Reading Order

### First Time Setup
1. [QUICK_RUN_GUIDE.md](./QUICK_RUN_GUIDE.md) → Chạy thử
2. [FINAL_CHECKLIST.md](./FINAL_CHECKLIST.md) → Checklist
3. [START_HERE.md](./START_HERE.md) → Hiểu overview

### Daily Use
1. [MOCK_MODE_QUICKSTART.md](./MOCK_MODE_QUICKSTART.md) → Quick ref
2. [setup_mock_mode.ps1](./setup_mock_mode.ps1) → Toggle on/off

### When Errors Occur
1. [check_errors.ps1](./check_errors.ps1) → Auto check
2. [TROUBLESHOOTING.md](./TROUBLESHOOTING.md) → Fix guide
3. [FIXES_APPLIED.md](./FIXES_APPLIED.md) → Known fixes

---

## 🏆 Best Practices

1. **Always check errors first:**
   ```powershell
   .\check_errors.ps1
   ```

2. **Update mock data regularly:**
   ```bash
   node export_mongodb_to_mock.js
   ```

3. **Use scripts for consistency:**
   ```powershell
   .\setup_mock_mode.ps1
   ```

4. **Read documentation when stuck:**
   - Quick fix: [QUICK_RUN_GUIDE.md](./QUICK_RUN_GUIDE.md)
   - Deep dive: [TROUBLESHOOTING.md](./TROUBLESHOOTING.md)

---

## 📞 Support Workflow

### Step 1: Self-Check
```powershell
.\check_errors.ps1
```

### Step 2: Read Docs
- Build error? → [FIXES_APPLIED.md](./FIXES_APPLIED.md)
- Runtime error? → [TROUBLESHOOTING.md](./TROUBLESHOOTING.md)
- Not sure? → [START_HERE.md](./START_HERE.md)

### Step 3: Debug
```bash
dotnet build -v detailed > build.log
cat build.log
```

### Step 4: Ask for Help
Include:
- Output từ `check_errors.ps1`
- Build log
- Error messages
- What you tried

---

## ✅ Success Checklist

After reading docs, you should be able to:

- [ ] Hiểu Mock Mode là gì
- [ ] Export data từ MongoDB
- [ ] Bật/tắt Mock Mode
- [ ] Chạy app offline
- [ ] Fix common errors
- [ ] Update mock data khi cần

---

## 🎓 FAQ

**Q: Nên bắt đầu từ file nào?**
A: [QUICK_RUN_GUIDE.md](./QUICK_RUN_GUIDE.md)

**Q: Làm sao biết đã setup đúng?**
A: Chạy `.\check_errors.ps1`

**Q: Gặp lỗi thì đọc gì?**
A: [TROUBLESHOOTING.md](./TROUBLESHOOTING.md)

**Q: Mock Mode là gì?**
A: [START_HERE.md](./START_HERE.md)

**Q: Checklist đầy đủ ở đâu?**
A: [FINAL_CHECKLIST.md](./FINAL_CHECKLIST.md)

---

## 🔗 External Resources

- [.NET Documentation](https://docs.microsoft.com/dotnet/)
- [MongoDB Driver](https://www.mongodb.com/docs/drivers/node/)
- [Blazor Documentation](https://docs.microsoft.com/aspnet/core/blazor/)

---

**Version:** 1.0.0
**Last Updated:** 2026-03-19
**Total Files:** 13 documents + 3 scripts

---

## 🎯 ONE-LINE SUMMARY

**Muốn chạy ngay?** → `.\setup_mock_mode.ps1` → Done! ✅
