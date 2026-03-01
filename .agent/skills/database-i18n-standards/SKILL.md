---
name: Database Multi-language Standards
description: Guidelines for designing and implementing multi-language (i18n) support at the database and application levels in the TTL ERP project.
---

# Database Multi-language Standards - TTL ERP

This document defines the strict architectural standards for supporting multiple languages (Vietnamese, English, etc.) within the TTL ERP system.

## 🏗️ 1. Structural Separation (Primary vs. Translate)
To ensure scalability and clean data management, translatable content must be separated from primary entity data.

- **Primary Collection/Table**: Stores non-translatable fields.
    - Fields: `Id`, `Code`, `Order`, `Type`, `IsActive`, `CreatedAt`, `IsDeleted`, etc.
    - **Prohibited**: Do not store language-specific columns (e.g., `Name`, `NameEn`, `DescriptionVi`) in the primary collection.
- **Translate Collection/Table**: Stores all localized content.
    - **Naming Convention**: `[PrimaryCollectionName]_translate` (e.g., `lookups_translate`).
    - **Required Fields**:
        - `ParentId` (or specific `EntityId` like `LookupId`): Foreign key to the primary record.
        - `LanguageCode`: Standard ISO code (e.g., `vi-VN`, `en-US`).
        - `Name`: The translated name.
        - `Description`: The translated description (optional).

## 🔄 2. Data Retrieval Patterns
- **Projection/Join**: Use aggregation or join queries to merge primary data with the specific language translation based on the user's active context.
- **DTO Mapping**:
    - Trả về dữ liệu đã được dịch sẵn dựa trên `LanguageCode` được yêu cầu.
    - Cấu trúc DTO nên chứa trường `Name` (đã dịch) thay vì trả về tất cả các bản dịch nếu không cần thiết.

## 🛡️ 3. Application-Level Handling (Automatic Detection)
To maintain consistency, all APIs must support automatic language detection through the `ICurrentUserService`.

### Language Resolution Priority:
1.  **Explicit Override**: Tham số `LanguageCode` (hoặc `lang`) được truyền trực tiếp trong Query/Command.
2.  **Authentication Context**: Trích xuất từ JWT Claims (`languageCode` hoặc `lang`).
3.  **Request Header**: Trích xuất từ `Accept-Language` header của trình duyệt/client.
4.  **System Default**: Mặc định là `vi-VN`.

### Interface Definition (`ICurrentUserService`):
```csharp
public interface ICurrentUserService {
    // ... các thuộc tính khác
    string LanguageCode { get; } // Tự động resolve dựa trên priority trên
}
```

## 📡 4. Web API Standard
Các Controller không nên bắt buộc truyền tham số ngôn ngữ nếu không có nhu cầu ghi đè đặc biệt.

- **Endpoint Definition**:
    ```csharp
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id, [FromQuery] string? languageCode = null) {
        // ... xử lý tự động qua Handler
    }
    ```

## 🧠 5. MediatR Handler Pattern
Tất cả các Handler xử lý dữ liệu lookup hoặc translatable content phải thực hiện:

1.  **Inject Context**: Tiêm `ICurrentUserService`.
2.  **Resolve Language**: 
    ```csharp
    var lang = request.LanguageCode ?? _currentUserService.LanguageCode;
    ```
3.  **Join/Fallback Logic**: Sử dụng `LanguageCode` đã resolve để truy vấn `_translate` repository và luôn có logic fallback về `vi-VN`.

## 🛠️ 6. Implementation Example (MongoDB)

### Primary Record (`lookups`)
```json
{
  "_id": "65fc5b5b0000000000000001",
  "Type": "Gender",
  "Code": "Male",
  "Order": 1,
  "IsActive": true
}
```

### Translation Records (`lookups_translate`)
```json
[
  {
    "LookupId": "65fc5b5b0000000000000001",
    "LanguageCode": "vi-VN",
    "Name": "Nam"
  },
  {
    "LookupId": "65fc5b5b0000000000000001",
    "LanguageCode": "en-US",
    "Name": "Male"
  }
]
```

## ✅ 7. Modification Rules
- Khi thêm một thực thể có thể dịch, **luôn luôn** tạo collection `_translate` tương ứng.
- Không sử dụng hardcoded strings trong giao diện; luôn đi qua Lookup/Translation service.
- Follow `vi-VN` là ngôn ngữ fallback mặc định.
- Mọi API mới phải tuân thủ cơ chế tự động nhận diện ngôn ngữ của `ICurrentUserService`.
