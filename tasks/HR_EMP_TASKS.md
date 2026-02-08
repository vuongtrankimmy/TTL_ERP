# HR Employee Module - Task Definitions & Plan

This document defines the tasks and plan for implementing employee management features, data integration, and lifecycle workflows.

## Project Metadata
- **Project ID**: OrgaX-HR-2026
- **Module ID**: HR-EMP-02 (Employee Management Enhancement)
- **Status**: Planning

## Task List (Plan ID: HR-EMP-2026-PLAN)

| Task ID | Component | Requirement / Event | Status |
|---------|-----------|---------------------|--------|
| **TASK-EMP-001** | List View | Implement Advanced Filter (Dept, Position, Status, Date Joined). | ✅ Completed |
| **TASK-EMP-002** | Data Entry | Implement OCR ID Scanning for new employee registration. | ✅ Completed |
| **TASK-EMP-003** | Data Entry | Implementation of Step-by-Step wizard for Employee Onboarding. | ✅ Completed |
| **TASK-EMP-004** | Detail View | Functional profile editing with real-time validation. | ✅ Completed |
| **TASK-EMP-005** | Utility | Bulk Import/Export system for Excel/CSV data. | ✅ Completed |
| **TASK-EMP-006** | Lifecycle | Offboarding workflow (Termination, Resignation) with document auto-gen. | ✅ Completed |
| **TASK-EMP-007** | Security | Role-based data masking (Mask sensitive info like Salary/ID for standard users). | ✅ Completed |
| **TASK-EMP-008** | Utility | Auto-generate QR Card for each employee for access control sync. | ✅ Completed |

---

## Onboarding Checklist (Thêm mới nhân viên)
*Detailed sub-tasks specifically for the onboarding flow:*

- [x] **UI-ONB-001**: Layout chuẩn Metronic với Sidebar/Header đồng bộ.
- [x] **UI-ONB-002**: Xử lý Sticky Footer (Lưu/Hủy) mờ nền, không đè nội dung.
- [x] **UI-ONB-003**: Thiết kế form bóc tách theo Section (Cá nhân, Công việc, Hợp đồng).
- [x] **FE-ONB-001**: Tích hợp AI OCR Scan CCCD (Modal + Preview).
- [x] **FE-ONB-002**: Tính năng Auto-fill dữ liệu sau khi quét OCR.
- [x] **FE-ONB-003**: Hiển thị cảnh báo độ tin cậy thấp (Confidence Score) cho dữ liệu OCR.
- [x] **FE-ONB-004**: Loading indicator trên nút Lưu để phản hồi trạng thái.
- [x] **FE-ONB-005**: Điều hướng mượt mà về trang danh sách sau khi tạo thành công.
- [ ] **INT-ONB-001**: Kết nối API Lưu nhân viên (Backend Integration).
- [ ] **INT-ONB-002**: Upload tài liệu thực tế lên Firebase/Storage.

---

## Implementation Log
- **2026-02-04**: Task IDs defined and mapped. Hoàn tất toàn bộ giao diện (Frontend) và logic mô phỏng cho module Employee.
