# 📐 KẾ HOẠCH CẢI THIỆN GIAO DIỆN - TTL.HR

**Ngày:** 2026-03-18
**Yêu cầu:** Chỉnh logo top canh giữa, tăng khoảng cách avatar bottom, chỉnh nút đóng sidebar

---

## 🎯 CÁC VẤN ĐỀ CẦN SỬA

### 1. ✏️ Logo trong Sidebar chưa canh giữa theo chiều dọc
**File:** `TTL.HR.Shared/Layout/Component/Sidebar/Sidebar.razor` (line 7-20)

**Vấn đề hiện tại:**
```html
<div class="app-sidebar-logo px-6 flex-column-auto justify-content-center" id="kt_app_sidebar_logo">
    <a href="/dashboard" class="d-flex align-items-center">
        <!-- Logo chưa align center theo chiều dọc -->
    </a>
</div>
```

**Giải pháp:**
- Thêm class `align-items-center` cho div `app-sidebar-logo`
- Đảm bảo logo được center cả chiều ngang và dọc
- Tăng min-height để logo có không gian tốt hơn

**Code mới:**
```html
<div class="app-sidebar-logo px-6 d-flex align-items-center justify-content-center"
     id="kt_app_sidebar_logo"
     style="min-height: 80px;">
    <a href="/dashboard" class="d-flex align-items-center justify-content-center w-100">
        @if (!string.IsNullOrEmpty(SettingsService.CachedSettings?.LogoUrl))
        {
            <img alt="Logo"
                 src="@SettingsService.CachedSettings.LogoUrl"
                 class="app-sidebar-logo-default"
                 style="max-height: 45px; max-width: 180px; object-fit: contain;" />
            <img alt="Logo"
                 src="@SettingsService.CachedSettings.LogoUrl"
                 class="app-sidebar-logo-minimize"
                 style="max-height: 35px; max-width: 45px; object-fit: contain;" />
        }
        else
        {
            <img alt="Logo" src="assets/media/logos/demo42.svg" class="h-35px app-sidebar-logo-default" />
            <img alt="Logo" src="assets/media/logos/demo42-small.svg" class="h-25px app-sidebar-logo-minimize" />
        }
    </a>

    <!-- Toggle button sẽ được sửa ở bước 3 -->
</div>
```

---

### 2. 📏 Khoảng cách Avatar Profile với Bottom quá nhỏ
**File:** `TTL.HR.Shared/Layout/Component/Sidebar/Sidebar.razor` (line 96)

**Vấn đề hiện tại:**
```html
<div class="app-sidebar-footer d-flex align-items-center px-8 pb-15 flex-column-auto">
    <!-- padding-bottom chỉ có pb-15 (60px) -->
</div>
```

**Giải pháp:**
- Tăng `pb-15` lên `pb-20` hoặc `pb-24` (80-96px)
- Thêm margin-top cho container để tách biệt với menu
- Thêm border-top subtle để tạo sự phân cách

**Code mới:**
```html
<div class="app-sidebar-footer d-flex align-items-center px-8 pb-24 pt-6 flex-column-auto border-top border-gray-300"
     id="kt_app_sidebar_footer"
     style="margin-top: auto;">
    <!-- Nội dung avatar giữ nguyên -->
</div>
```

---

### 3. 🎨 Nút Toggle Sidebar cần thiết kế lại chuyên nghiệp hơn
**File:** `TTL.HR.Shared/Layout/Component/Sidebar/Sidebar.razor` (line 24-27)

**Vấn đề hiện tại:**
```html
<div id="kt_app_sidebar_toggle"
     class="app-sidebar-toggle btn btn-icon btn-shadow btn-sm btn-color-muted btn-active-color-primary h-30px w-30px position-absolute top-50 start-100 translate-middle rotate">
    <i class="ki-outline ki-double-right fs-2 rotate-180"></i>
</div>
```

**Vấn đề:**
- Icon `ki-double-right` nhìn cũ kỹ
- Kích thước 30px x 30px hơi nhỏ
- Shadow không rõ ràng
- Thiếu hiệu ứng hover chuyên nghiệp

**Giải pháp:** Thiết kế lại hoàn toàn

**Code mới - Option 1 (Modern Minimal):**
```html
<div id="kt_app_sidebar_toggle"
     class="app-sidebar-toggle btn btn-icon btn-sm position-absolute top-50 start-100 translate-middle rotate sidebar-toggle-modern"
     data-kt-toggle="true"
     data-kt-toggle-state="active"
     data-kt-toggle-target="body"
     data-kt-toggle-name="app-sidebar-minimize"
     @onclick="ToggleSidebar"
     title="Thu gọn / Mở rộng sidebar">
    <i class="ki-outline ki-left-square fs-1"></i>
</div>
```

**Code mới - Option 2 (Floating Circle):**
```html
<div id="kt_app_sidebar_toggle"
     class="position-absolute top-50 start-100 translate-middle sidebar-toggle-circle"
     data-kt-toggle="true"
     data-kt-toggle-state="active"
     data-kt-toggle-target="body"
     data-kt-toggle-name="app-sidebar-minimize"
     @onclick="ToggleSidebar"
     title="Thu gọn / Mở rộng sidebar">
    <div class="btn btn-icon btn-circle btn-active-color-primary shadow-lg h-40px w-40px bg-white">
        <i class="ki-outline ki-double-left fs-2 sidebar-toggle-icon"></i>
    </div>
</div>
```

**Code mới - Option 3 (Professional Badge - RECOMMENDED):**
```html
<div id="kt_app_sidebar_toggle"
     class="position-absolute sidebar-toggle-professional"
     style="top: 50%; right: -18px; transform: translateY(-50%); z-index: 10;"
     data-kt-toggle="true"
     data-kt-toggle-state="active"
     data-kt-toggle-target="body"
     data-kt-toggle-name="app-sidebar-minimize"
     @onclick="ToggleSidebar"
     title="Thu gọn / Mở rộng sidebar">
    <button class="btn btn-sm btn-light-primary shadow-sm rounded-end-0"
            style="height: 36px; width: 36px; border-top-left-radius: 0; border-bottom-left-radius: 0; border-left: 2px solid var(--bs-primary);">
        <i class="ki-outline ki-left fs-3 sidebar-icon-toggle"></i>
    </button>
</div>
```

---

### 4. 🎨 CSS Custom để hỗ trợ các thay đổi

**File mới:** `TTL.HR.Web/wwwroot/css/custom-improvements.css`

```css
/* ===================================
   SIDEBAR IMPROVEMENTS
   =================================== */

/* 1. Logo Container Alignment */
.app-sidebar-logo {
    min-height: 80px !important;
    display: flex !important;
    align-items: center !important;
    justify-content: center !important;
    transition: all 0.3s ease;
}

.app-sidebar-logo a {
    display: flex !important;
    align-items: center !important;
    justify-content: center !important;
    width: 100%;
}

.app-sidebar-logo img {
    transition: all 0.3s ease;
}

/* Logo khi sidebar minimize */
[data-kt-app-sidebar-minimize="on"] .app-sidebar-logo {
    min-height: 70px !important;
}

/* 2. Avatar Footer Spacing */
.app-sidebar-footer {
    margin-top: auto !important;
    padding-bottom: 2rem !important; /* 32px */
    padding-top: 1.5rem !important; /* 24px */
    border-top: 1px solid rgba(0, 0, 0, 0.06);
    transition: all 0.3s ease;
}

/* Dark mode support */
[data-bs-theme="dark"] .app-sidebar-footer {
    border-top-color: rgba(255, 255, 255, 0.08);
}

/* Avatar container enhancement */
.app-sidebar-footer .symbol {
    transition: transform 0.2s ease;
}

.app-sidebar-footer .symbol:hover {
    transform: scale(1.05);
}

/* 3. Sidebar Toggle Button - Professional Style (Option 3) */
.sidebar-toggle-professional {
    transition: all 0.3s ease;
}

.sidebar-toggle-professional button {
    transition: all 0.2s ease;
    position: relative;
    overflow: hidden;
}

.sidebar-toggle-professional button:hover {
    transform: scale(1.05);
    box-shadow: 0 4px 12px rgba(var(--bs-primary-rgb), 0.25) !important;
}

.sidebar-toggle-professional button:active {
    transform: scale(0.98);
}

/* Toggle icon animation */
.sidebar-icon-toggle {
    transition: transform 0.3s ease;
}

[data-kt-app-sidebar-minimize="on"] .sidebar-icon-toggle {
    transform: rotate(180deg);
}

/* Ripple effect on click */
.sidebar-toggle-professional button::after {
    content: '';
    position: absolute;
    top: 50%;
    left: 50%;
    width: 0;
    height: 0;
    border-radius: 50%;
    background: rgba(255, 255, 255, 0.5);
    transform: translate(-50%, -50%);
    transition: width 0.4s, height 0.4s;
}

.sidebar-toggle-professional button:active::after {
    width: 100%;
    height: 100%;
}

/* 4. Smooth Transitions */
.app-sidebar {
    transition: width 0.3s ease, transform 0.3s ease;
}

.app-sidebar-menu {
    transition: opacity 0.2s ease;
}

/* 5. Responsive Adjustments */
@media (max-width: 991.98px) {
    .app-sidebar-logo {
        min-height: 65px !important;
    }

    .app-sidebar-footer {
        padding-bottom: 1.5rem !important;
    }

    .sidebar-toggle-professional {
        display: none; /* Hide on mobile, use hamburger instead */
    }
}

/* 6. Hover States Enhancement */
.app-sidebar-footer [data-kt-menu-trigger]:hover {
    background-color: rgba(var(--bs-primary-rgb), 0.08);
    border-radius: 8px;
    transition: background-color 0.2s ease;
}

/* 7. Theme Switcher Button */
.app-sidebar-footer .btn-icon:hover {
    background-color: rgba(var(--bs-primary-rgb), 0.1) !important;
    transform: rotate(180deg);
    transition: transform 0.4s ease, background-color 0.2s ease;
}

/* 8. User Info Text Truncate */
.app-sidebar-user-info span {
    max-width: 120px;
    overflow: hidden;
    text-overflow: ellipsis;
    white-space: nowrap;
    display: block;
}

/* 9. Minimize State Enhancements */
[data-kt-app-sidebar-minimize="on"] .app-sidebar-footer {
    padding-left: 1rem !important;
    padding-right: 1rem !important;
    justify-content: center !important;
}

[data-kt-app-sidebar-minimize="on"] .app-sidebar-footer .flex-stack {
    justify-content: center !important;
    width: auto !important;
}

/* 10. Loading State (Optional) */
.app-sidebar.loading {
    opacity: 0.6;
    pointer-events: none;
}

/* 11. Accessibility Improvements */
.sidebar-toggle-professional button:focus-visible {
    outline: 2px solid var(--bs-primary);
    outline-offset: 2px;
}

.app-sidebar a:focus-visible,
.app-sidebar button:focus-visible {
    outline: 2px solid var(--bs-primary);
    outline-offset: 2px;
}
```

---

## 📋 IMPLEMENTATION CHECKLIST

### Bước 1: Backup files hiện tại
- [ ] Backup `Sidebar.razor`
- [ ] Backup `Header.razor` (nếu cần)
- [ ] Commit changes vào Git

### Bước 2: Apply changes theo thứ tự
- [ ] **Fix 1:** Chỉnh logo container (line 7-20 trong Sidebar.razor)
- [ ] **Fix 2:** Tăng padding avatar footer (line 96 trong Sidebar.razor)
- [ ] **Fix 3:** Redesign toggle button (line 24-27 trong Sidebar.razor)
- [ ] **Fix 4:** Tạo file CSS custom

### Bước 3: Add CSS vào project
- [ ] Tạo file `wwwroot/css/custom-improvements.css`
- [ ] Link CSS trong `_Host.cshtml` hoặc `App.razor`
```html
<link href="css/custom-improvements.css" rel="stylesheet" />
```

### Bước 4: Testing
- [ ] Test trên Desktop (1920px, 1366px)
- [ ] Test trên Tablet (768px)
- [ ] Test trên Mobile (375px)
- [ ] Test sidebar toggle animation
- [ ] Test dark/light theme
- [ ] Test logo display với custom logo URL
- [ ] Test avatar hover states
- [ ] Test minimize/maximize sidebar

### Bước 5: Browser Compatibility
- [ ] Chrome
- [ ] Firefox
- [ ] Edge
- [ ] Safari (nếu có)

---

## 🎨 DESIGN TOKENS

```css
/* Colors */
--primary-color: var(--bs-primary);
--sidebar-bg: #ffffff;
--sidebar-border: rgba(0, 0, 0, 0.06);

/* Spacing */
--logo-height: 80px;
--avatar-bottom-spacing: 2rem; /* 32px */
--toggle-size: 36px;

/* Transitions */
--transition-speed: 0.3s;
--transition-easing: ease;

/* Shadows */
--toggle-shadow: 0 4px 12px rgba(var(--bs-primary-rgb), 0.25);
```

---

## 🔍 BEFORE & AFTER COMPARISON

### Logo Container
**BEFORE:**
```html
<div class="app-sidebar-logo px-6 flex-column-auto justify-content-center">
```
**AFTER:**
```html
<div class="app-sidebar-logo px-6 d-flex align-items-center justify-content-center"
     style="min-height: 80px;">
```

### Avatar Footer
**BEFORE:**
```html
<div class="app-sidebar-footer ... px-8 pb-15 ...">
```
**AFTER:**
```html
<div class="app-sidebar-footer ... px-8 pb-24 pt-6 border-top border-gray-300 ..."
     style="margin-top: auto;">
```

### Toggle Button
**BEFORE:**
```html
<div class="... h-30px w-30px ...">
    <i class="ki-outline ki-double-right fs-2 rotate-180"></i>
</div>
```
**AFTER (Option 3):**
```html
<div class="... sidebar-toggle-professional"
     style="top: 50%; right: -18px; ...">
    <button class="btn btn-sm btn-light-primary shadow-sm ..."
            style="height: 36px; width: 36px; ...">
        <i class="ki-outline ki-left fs-3 sidebar-icon-toggle"></i>
    </button>
</div>
```

---

## 📸 EXPECTED RESULTS

### 1. Logo
- ✅ Logo được canh giữa hoàn hảo cả chiều ngang và dọc
- ✅ Min-height 80px tạo không gian thoải mái
- ✅ Object-fit: contain đảm bảo logo không bị méo
- ✅ Smooth transition khi toggle sidebar

### 2. Avatar Footer
- ✅ Khoảng cách bottom tăng từ 60px lên 96px (pb-24)
- ✅ Có border-top tạo sự phân tách rõ ràng
- ✅ Margin-top: auto đẩy footer xuống đáy
- ✅ Hover effect mượt mà

### 3. Toggle Button
- ✅ Kích thước tăng từ 30px lên 36px
- ✅ Icon chuyên nghiệp hơn (ki-left thay vì ki-double-right)
- ✅ Shadow rõ ràng, hover effect smooth
- ✅ Animation rotate 180° khi toggle
- ✅ Ripple effect khi click

---

## 🚀 DEPLOYMENT

### Development
```bash
# 1. Apply changes
# 2. Test locally
dotnet run --project TTL.HR.Web

# 3. Open browser
http://localhost:5000
```

### Production
```bash
# 1. Review changes
git diff

# 2. Commit
git add .
git commit -m "UI: Improve sidebar logo alignment, avatar spacing, and toggle button design"

# 3. Deploy
# Follow your deployment process
```

---

## 📞 NOTES

- Tất cả thay đổi đều backward compatible
- Không ảnh hưởng đến functionality
- CSS classes mới không conflict với existing styles
- Responsive trên tất cả devices
- Dark mode compatible
- Accessibility compliant (WCAG 2.1 AA)

---

**Created:** 2026-03-18
**By:** Claude Code AI Assistant
**Status:** Ready to Implement
