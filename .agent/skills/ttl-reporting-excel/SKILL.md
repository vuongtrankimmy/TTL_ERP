---
name: ttl-reporting-excel
description: Generating Excel reports using ClosedXML.
---

# TTL Reporting & Excel Export

Standardized way to generate and return Excel files from the API.

## 1. Prerequisites
- Use **ClosedXML** library.
- Dependency: `Infrastructure` project should have `ClosedXML` package.

## 2. Implementation Pattern
Create a service or handler that generates a `XLWorkbook`.

```csharp
using var workbook = new XLWorkbook();
var worksheet = workbook.Worksheets.Add("Data");

// Header
worksheet.Cell(1, 1).Value = "Column A";
worksheet.Cell(1, 1).Style.Font.Bold = true;

// Data
int row = 2;
foreach (var item in data)
{
    worksheet.Cell(row, 1).Value = item.Value;
    row++;
}

using var stream = new MemoryStream();
workbook.SaveAs(stream);
var content = stream.ToArray();
```

## 3. Returning from Controller
Return as a `FileContentResult`.

```csharp
[HttpGet("export")]
public async Task<IActionResult> Export()
{
    var content = await _exportService.GenerateExcelAsync();
    return File(
        content, 
        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", 
        "Report.xlsx"
    );
}
```

## 4. Frontend Download
On the Blazor side, use `JSRuntime` to trigger the browser download after receiving the blob.
- Use a helper `FileUtil.js` to handle `SaveAs` for byte arrays.
