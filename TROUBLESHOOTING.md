# 🔧 Troubleshooting - Mock Data Mode

## ❌ Lỗi Thường Gặp Và Cách Khắc Phục

### 1. Lỗi Build - Missing Using Statements

#### Lỗi CS0246: IWebHostEnvironment
```
CS0246: The type or namespace name 'IWebHostEnvironment' could not be found
```

**Nguyên nhân:** Thiếu using statement cho Microsoft.AspNetCore.Hosting

**Giải pháp:**

File: `TTL.HR.Application/Infrastructure/MockData/MockDataProvider.cs`

```csharp
// Thêm dòng này
using Microsoft.AspNetCore.Hosting;
```

#### Lỗi CS0246: HttpUtility
```
CS0246: The type or namespace name 'HttpUtility' could not be found
```

**Nguyên nhân:** Thiếu using System.Web

**Giải pháp:**

File: `TTL.HR.Application/Infrastructure/MockData/MockHttpClient.cs`

```csharp
// Thêm dòng này
using System.Web;
```

**Hoặc** thay `System.Web.HttpUtility.ParseQueryString` bằng code thay thế:

```csharp
// Thay vì
var queryParams = System.Web.HttpUtility.ParseQueryString(query);

// Dùng
var queryParams = ParseQueryString(query);

// Và thêm helper method
private static Dictionary<string, string> ParseQueryString(string query)
{
    var result = new Dictionary<string, string>();
    if (string.IsNullOrEmpty(query)) return result;

    query = query.TrimStart('?');
    var pairs = query.Split('&');

    foreach (var pair in pairs)
    {
        var parts = pair.Split('=');
        if (parts.Length == 2)
        {
            result[parts[0]] = Uri.UnescapeDataString(parts[1]);
        }
    }

    return result;
}
```

---

### 2. Lỗi NuGet Package

#### Lỗi: Package 'System.Web' không tồn tại

**Giải pháp 1:** Thêm package reference

```bash
cd TTL.HR/TTL.HR.Application
dotnet add package System.Web --version 4.0.0
```

**Giải pháp 2 (Khuyến nghị):** Không dùng System.Web, dùng code thay thế

Cập nhật `MockHttpClient.cs`:

```csharp
// Xóa: using System.Web;

// Trong method GetAsync, thay đổi:
var queryParams = ParseQueryString(query);
var page = queryParams.TryGetValue("page", out var p) && int.TryParse(p, out var pageNum) ? pageNum : 1;
var pageSize = queryParams.TryGetValue("pageSize", out var ps) && int.TryParse(ps, out var pageSizeNum) ? pageSizeNum : 10;
var searchTerm = queryParams.TryGetValue("searchTerm", out var st) ? st : null;

// Thêm helper method
private static Dictionary<string, string> ParseQueryString(string query)
{
    var result = new Dictionary<string, string>();
    if (string.IsNullOrEmpty(query)) return result;

    query = query.TrimStart('?');
    if (string.IsNullOrEmpty(query)) return result;

    var pairs = query.Split('&');
    foreach (var pair in pairs)
    {
        if (string.IsNullOrWhiteSpace(pair)) continue;

        var parts = pair.Split('=');
        if (parts.Length == 2)
        {
            result[parts[0]] = Uri.UnescapeDataString(parts[1]);
        }
        else if (parts.Length == 1)
        {
            result[parts[0]] = string.Empty;
        }
    }

    return result;
}
```

---

### 3. Lỗi Runtime - Mock Data Không Load

#### Lỗi: "Mock data file không tồn tại"

```
⚠️  Mock data file không tồn tại: D:\...\wwwroot\MockData\mongodb_export.json
```

**Nguyên nhân:**
1. Chưa chạy export script
2. File ở sai vị trí
3. Đường dẫn trong config sai

**Giải pháp:**

```bash
# 1. Chạy export script
node export_mongodb_to_mock.js

# 2. Kiểm tra file tồn tại
ls TTL.HR/TTL.HR.Web/MockData/mongodb_export.json

# 3. Copy vào wwwroot
mkdir TTL.HR/TTL.HR.Web/wwwroot/MockData
cp TTL.HR/TTL.HR.Web/MockData/mongodb_export.json TTL.HR/TTL.HR.Web/wwwroot/MockData/

# 4. Verify
ls TTL.HR/TTL.HR.Web/wwwroot/MockData/mongodb_export.json
```

---

### 4. Lỗi Export Script

#### Lỗi: "Cannot find module 'mongodb'"

```
Error: Cannot find module 'mongodb'
```

**Giải pháp:**

```bash
# Cài đặt MongoDB driver
npm install mongodb

# Hoặc nếu dùng package.json
cd TTL_ERP
npm install
```

#### Lỗi: MongoDB Connection Failed

```
MongoServerError: Authentication failed
```

**Giải pháp:**

1. Kiểm tra connection string trong `export_mongodb_to_mock.js`:

```javascript
const MONGO_URI = 'mongodb+srv://username:password@cluster.mongodb.net/...';
const DATABASE_NAME = 'YOUR_DATABASE_NAME'; // ⚠️  QUAN TRỌNG: Thay đúng tên DB
```

2. Kiểm tra IP whitelist trên MongoDB Atlas
3. Kiểm tra username/password

---

### 5. Lỗi API Response Format

#### Lỗi: "Cannot deserialize response"

**Nguyên nhân:** Format response từ MockHttpClient khác với API thật

**Giải pháp:**

Kiểm tra trong `MockHttpClient.cs` rằng response format giống API:

```csharp
// ✅ Đúng format
var pagedResult = new PagedResult<object>
{
    Items = pagedItems,
    Page = page,
    PageSize = pageSize,
    TotalCount = total,
    TotalPages = (int)Math.Ceiling(total / (double)pageSize)
};

return CreateSuccessResponse(new ApiResponse<PagedResult<object>>
{
    Success = true,
    Data = pagedResult,
    Message = "Success"
});
```

---

### 6. Lỗi Missing Models/DTOs

#### Lỗi CS0246: ApiResponse, PagedResult không tồn tại

**Nguyên nhân:** Các model chưa được định nghĩa

**Giải pháp:**

Tạo file `TTL.HR.Application/Modules/Common/Models/ApiResponse.cs`:

```csharp
namespace TTL.HR.Application.Modules.Common.Models;

public class ApiResponse<T>
{
    public bool Success { get; set; }
    public T? Data { get; set; }
    public string? Message { get; set; }
    public List<string>? Errors { get; set; }
}

public class PagedResult<T>
{
    public List<T> Items { get; set; } = new();
    public int Page { get; set; }
    public int PageSize { get; set; }
    public long TotalCount { get; set; }
    public int TotalPages { get; set; }
}
```

---

### 7. Lỗi Dependency Injection

#### Lỗi: "Unable to resolve service for type 'MockDataProvider'"

**Nguyên nhân:** Chưa register service trong Program.cs

**Giải pháp:**

File: `Program.cs`

```csharp
// Đảm bảo có dòng này
builder.Services.AddSingleton<MockDataProvider>();

// Hoặc dùng Scoped nếu cần
builder.Services.AddScoped<MockDataProvider>();
```

---

### 8. Performance Issues

#### Mock Data Load Chậm

**Nguyên nhân:** File JSON quá lớn (>50MB)

**Giải pháp:**

**Option 1:** Tối ưu export script

```javascript
// Trong export_mongodb_to_mock.js
// Giới hạn số documents
const data = await collection.find({}).limit(1000).toArray();
```

**Option 2:** Split collections thành nhiều file

```javascript
// Export mỗi collection thành 1 file riêng
const fileName = `${collectionName}.json`;
fs.writeFileSync(
    path.join(mockDataDir, fileName),
    JSON.stringify(serializedData, null, 2)
);
```

**Option 3:** Nén file JSON

```bash
# Nén file
gzip mongodb_export.json

# Giải nén khi load (cập nhật MockDataProvider)
```

---

## 🚀 Quick Fix All

Script PowerShell để fix tất cả lỗi thường gặp:

```powershell
# fix_mock_errors.ps1

Write-Host "🔧 Fixing Mock Data Errors..." -ForegroundColor Yellow

# 1. Fix using statements
$mockDataProvider = "TTL.HR\TTL.HR.Application\Infrastructure\MockData\MockDataProvider.cs"
$content = Get-Content $mockDataProvider -Raw
if (-not $content.Contains("using Microsoft.AspNetCore.Hosting;")) {
    $content = $content -replace "using System.Text.Json;", "using System.Text.Json;`nusing Microsoft.AspNetCore.Hosting;"
    Set-Content $mockDataProvider $content
    Write-Host "✅ Fixed MockDataProvider.cs" -ForegroundColor Green
}

# 2. Install MongoDB package
if (-not (Test-Path "node_modules\mongodb")) {
    npm install mongodb
    Write-Host "✅ Installed mongodb package" -ForegroundColor Green
}

# 3. Create wwwroot/MockData directory
$mockDataDir = "TTL.HR\TTL.HR.Web\wwwroot\MockData"
if (-not (Test-Path $mockDataDir)) {
    New-Item -ItemType Directory -Path $mockDataDir -Force | Out-Null
    Write-Host "✅ Created MockData directory" -ForegroundColor Green
}

# 4. Run export if needed
if (-not (Test-Path "$mockDataDir\mongodb_export.json")) {
    Write-Host "📦 Running export script..." -ForegroundColor Yellow
    node export_mongodb_to_mock.js
}

Write-Host "`n✅ All fixes applied!" -ForegroundColor Green
Write-Host "Now run: dotnet build" -ForegroundColor Cyan
```

---

## 📞 Hỗ Trợ

Nếu vẫn gặp lỗi:

1. **Check Console Output** - Xem message khi app start
2. **Check Build Output** - Xem lỗi compile
3. **Verify Files** - Đảm bảo tất cả file đã được tạo đúng
4. **Check Configuration** - Verify appsettings.json

**Debug Commands:**

```bash
# Build và xem lỗi chi tiết
dotnet build -v detailed > build_log.txt

# Xem dependencies
dotnet list package

# Clean và rebuild
dotnet clean
dotnet build

# Run với debug
dotnet run --configuration Debug
```

---

**Last Updated:** 2026-03-19
