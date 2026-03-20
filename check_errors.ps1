# ========================================
# Script Kiểm Tra Lỗi Mock Data Setup
# ========================================

Write-Host "=============================================" -ForegroundColor Cyan
Write-Host "🔍 KIỂM TRA LỖI MOCK DATA MODE" -ForegroundColor Yellow
Write-Host "=============================================" -ForegroundColor Cyan
Write-Host ""

$errorCount = 0
$warningCount = 0

# ========================================
# 1. Kiểm tra file tồn tại
# ========================================
Write-Host "📁 Kiểm tra files..." -ForegroundColor Green

$requiredFiles = @(
    @{Path="export_mongodb_to_mock.js"; Name="Export Script"},
    @{Path="TTL.HR\TTL.HR.Application\Infrastructure\MockData\MockDataProvider.cs"; Name="MockDataProvider"},
    @{Path="TTL.HR\TTL.HR.Application\Infrastructure\MockData\MockHttpClient.cs"; Name="MockHttpClient"},
    @{Path="TTL.HR\TTL.HR.Web\appsettings.json"; Name="AppSettings"},
    @{Path="TTL.HR\TTL.HR.Web\Program.cs"; Name="Program.cs"}
)

foreach ($file in $requiredFiles) {
    if (Test-Path $file.Path) {
        Write-Host "   ✅ $($file.Name): OK" -ForegroundColor Green
    } else {
        Write-Host "   ❌ $($file.Name): THIẾU FILE!" -ForegroundColor Red
        Write-Host "      Path: $($file.Path)" -ForegroundColor Gray
        $errorCount++
    }
}

Write-Host ""

# ========================================
# 2. Kiểm tra using statements
# ========================================
Write-Host "📦 Kiểm tra using statements..." -ForegroundColor Green

$mockDataProvider = "TTL.HR\TTL.HR.Application\Infrastructure\MockData\MockDataProvider.cs"
if (Test-Path $mockDataProvider) {
    $content = Get-Content $mockDataProvider -Raw

    if ($content -match "using Microsoft.AspNetCore.Hosting;") {
        Write-Host "   ✅ MockDataProvider: IWebHostEnvironment OK" -ForegroundColor Green
    } else {
        Write-Host "   ❌ MockDataProvider: Thiếu 'using Microsoft.AspNetCore.Hosting;'" -ForegroundColor Red
        $errorCount++
    }
}

$mockHttpClient = "TTL.HR\TTL.HR.Application\Infrastructure\MockData\MockHttpClient.cs"
if (Test-Path $mockHttpClient) {
    $content = Get-Content $mockHttpClient -Raw

    if ($content -match "System\.Web\.HttpUtility") {
        Write-Host "   ⚠️  MockHttpClient: Đang dùng System.Web (không khuyến nghị)" -ForegroundColor Yellow
        $warningCount++
    } else {
        Write-Host "   ✅ MockHttpClient: Không dùng System.Web" -ForegroundColor Green
    }

    if ($content -match "ParseQueryString") {
        Write-Host "   ✅ MockHttpClient: Có helper ParseQueryString" -ForegroundColor Green
    } else {
        Write-Host "   ❌ MockHttpClient: Thiếu helper ParseQueryString" -ForegroundColor Red
        $errorCount++
    }
}

Write-Host ""

# ========================================
# 3. Kiểm tra configuration
# ========================================
Write-Host "⚙️  Kiểm tra configuration..." -ForegroundColor Green

$appsettings = "TTL.HR\TTL.HR.Web\appsettings.json"
if (Test-Path $appsettings) {
    $config = Get-Content $appsettings -Raw | ConvertFrom-Json

    if ($config.PSObject.Properties["MockDataSettings"]) {
        Write-Host "   ✅ MockDataSettings: Có config" -ForegroundColor Green

        $enabled = $config.MockDataSettings.Enabled
        Write-Host "   ℹ️  Mock Mode: $enabled" -ForegroundColor $(if ($enabled) { "Yellow" } else { "Cyan" })

        if ($config.MockDataSettings.PSObject.Properties["MockDataPath"]) {
            Write-Host "   ✅ MockDataPath: $($config.MockDataSettings.MockDataPath)" -ForegroundColor Green
        } else {
            Write-Host "   ⚠️  MockDataPath: Chưa cấu hình" -ForegroundColor Yellow
            $warningCount++
        }
    } else {
        Write-Host "   ❌ MockDataSettings: Chưa có config!" -ForegroundColor Red
        $errorCount++
    }
} else {
    Write-Host "   ❌ appsettings.json không tồn tại!" -ForegroundColor Red
    $errorCount++
}

Write-Host ""

# ========================================
# 4. Kiểm tra MongoDB package
# ========================================
Write-Host "📦 Kiểm tra Node packages..." -ForegroundColor Green

if (Test-Path "node_modules\mongodb") {
    Write-Host "   ✅ mongodb package: Đã cài đặt" -ForegroundColor Green
} else {
    Write-Host "   ⚠️  mongodb package: Chưa cài đặt" -ForegroundColor Yellow
    Write-Host "      Chạy: npm install mongodb" -ForegroundColor Gray
    $warningCount++
}

Write-Host ""

# ========================================
# 5. Kiểm tra mock data file
# ========================================
Write-Host "💾 Kiểm tra mock data..." -ForegroundColor Green

$mockDataFile = "TTL.HR\TTL.HR.Web\wwwroot\MockData\mongodb_export.json"
if (Test-Path $mockDataFile) {
    $fileSize = (Get-Item $mockDataFile).Length / 1MB
    Write-Host "   ✅ Mock data file: OK (Size: $([math]::Round($fileSize, 2)) MB)" -ForegroundColor Green

    # Kiểm tra file có valid JSON không
    try {
        $null = Get-Content $mockDataFile -Raw | ConvertFrom-Json
        Write-Host "   ✅ JSON format: Valid" -ForegroundColor Green
    } catch {
        Write-Host "   ❌ JSON format: Invalid!" -ForegroundColor Red
        Write-Host "      Error: $($_.Exception.Message)" -ForegroundColor Gray
        $errorCount++
    }
} else {
    Write-Host "   ⚠️  Mock data file: Chưa có" -ForegroundColor Yellow
    Write-Host "      Chạy: node export_mongodb_to_mock.js" -ForegroundColor Gray
    $warningCount++
}

# Kiểm tra thư mục MockData
$mockDataDir = "TTL.HR\TTL.HR.Web\wwwroot\MockData"
if (Test-Path $mockDataDir) {
    Write-Host "   ✅ MockData directory: OK" -ForegroundColor Green
} else {
    Write-Host "   ⚠️  MockData directory: Chưa có" -ForegroundColor Yellow
    Write-Host "      Sẽ tạo tự động khi chạy setup script" -ForegroundColor Gray
    $warningCount++
}

Write-Host ""

# ========================================
# 6. Kiểm tra Program.cs integration
# ========================================
Write-Host "🔗 Kiểm tra Program.cs integration..." -ForegroundColor Green

$programFile = "TTL.HR\TTL.HR.Web\Program.cs"
if (Test-Path $programFile) {
    $content = Get-Content $programFile -Raw

    if ($content -match "MockDataProvider") {
        Write-Host "   ✅ Program.cs: Có reference MockDataProvider" -ForegroundColor Green
    } else {
        Write-Host "   ❌ Program.cs: Thiếu integration với MockDataProvider!" -ForegroundColor Red
        $errorCount++
    }

    if ($content -match "MockHttpClient") {
        Write-Host "   ✅ Program.cs: Có reference MockHttpClient" -ForegroundColor Green
    } else {
        Write-Host "   ❌ Program.cs: Thiếu integration với MockHttpClient!" -ForegroundColor Red
        $errorCount++
    }

    if ($content -match "MockDataSettings") {
        Write-Host "   ✅ Program.cs: Đọc MockDataSettings từ config" -ForegroundColor Green
    } else {
        Write-Host "   ⚠️  Program.cs: Chưa đọc MockDataSettings" -ForegroundColor Yellow
        $warningCount++
    }
}

Write-Host ""

# ========================================
# 7. Test build
# ========================================
Write-Host "🔨 Kiểm tra build..." -ForegroundColor Green

$buildTest = Read-Host "Bạn có muốn test build project không? (y/n)"

if ($buildTest -eq "y" -or $buildTest -eq "Y") {
    Write-Host "   🔄 Đang build..." -ForegroundColor Yellow

    Push-Location "TTL.HR\TTL.HR.Web"
    $buildOutput = dotnet build 2>&1
    $buildSuccess = $LASTEXITCODE -eq 0
    Pop-Location

    if ($buildSuccess) {
        Write-Host "   ✅ Build: THÀNH CÔNG" -ForegroundColor Green
    } else {
        Write-Host "   ❌ Build: THẤT BẠI" -ForegroundColor Red
        Write-Host "   Xem chi tiết lỗi:" -ForegroundColor Gray
        $buildOutput | Select-Object -Last 20 | ForEach-Object {
            Write-Host "      $_" -ForegroundColor Gray
        }
        $errorCount++
    }
} else {
    Write-Host "   ⏭️  Bỏ qua test build" -ForegroundColor Gray
}

Write-Host ""

# ========================================
# KẾT QUẢ
# ========================================
Write-Host "=============================================" -ForegroundColor Cyan
Write-Host "📊 KẾT QUẢ KIỂM TRA" -ForegroundColor Yellow
Write-Host "=============================================" -ForegroundColor Cyan

if ($errorCount -eq 0 -and $warningCount -eq 0) {
    Write-Host "✅ HOÀN HẢO! Không có lỗi." -ForegroundColor Green
    Write-Host ""
    Write-Host "🚀 Bạn có thể chạy app ngay:" -ForegroundColor Cyan
    Write-Host "   cd TTL.HR\TTL.HR.Web" -ForegroundColor Gray
    Write-Host "   dotnet run" -ForegroundColor Gray
} else {
    if ($errorCount -gt 0) {
        Write-Host "❌ Có $errorCount lỗi cần sửa" -ForegroundColor Red
    }

    if ($warningCount -gt 0) {
        Write-Host "⚠️  Có $warningCount cảnh báo" -ForegroundColor Yellow
    }

    Write-Host ""
    Write-Host "📖 Xem hướng dẫn sửa lỗi tại:" -ForegroundColor Cyan
    Write-Host "   TROUBLESHOOTING.md" -ForegroundColor Gray
}

Write-Host ""
Write-Host "=============================================" -ForegroundColor Cyan
Write-Host ""

# Suggestions
if ($errorCount -gt 0 -or $warningCount -gt 0) {
    Write-Host "💡 GỢI Ý KHẮC PHỤC:" -ForegroundColor Yellow
    Write-Host ""

    if (-not (Test-Path "node_modules\mongodb")) {
        Write-Host "1️⃣  Cài đặt MongoDB package:" -ForegroundColor Cyan
        Write-Host "   npm install mongodb" -ForegroundColor Gray
        Write-Host ""
    }

    if (-not (Test-Path $mockDataFile)) {
        Write-Host "2️⃣  Export mock data:" -ForegroundColor Cyan
        Write-Host "   node export_mongodb_to_mock.js" -ForegroundColor Gray
        Write-Host ""
    }

    if ($errorCount -gt 0) {
        Write-Host "3️⃣  Chạy script tự động sửa lỗi:" -ForegroundColor Cyan
        Write-Host "   .\setup_mock_mode.ps1" -ForegroundColor Gray
        Write-Host ""
    }
}
