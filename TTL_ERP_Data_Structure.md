# Tài liệu Cấu trúc Dữ liệu Hệ thống TTL ERP (HRM Module)

Hệ thống được xây dựng trên nền tảng .NET (Blazor) và sử dụng **MongoDB** làm cơ sở dữ liệu chính. Tất cả các thực thể (Entities) đều kế thừa từ một lớp nền tảng để đảm bảo tính đồng nhất về metadata.

---

## 1. Cấu trúc Nền tảng (Base)
Tất cả các thực thể trong hệ thống đều kế thừa từ `BaseEntity`.

### BaseEntity
| Trường | Kiểu dữ liệu | Mô tả |
| :--- | :--- | :--- |
| `Id` | `string` (ObjectId) | Khóa chính duy nhất |
| `CreatedAt` | `DateTime` | Thời điểm tạo (UTC) |
| `CreatedBy` | `string` (ObjectId) | ID người tạo |
| `UpdatedAt` | `DateTime?` | Thời điểm cập nhật cuối |
| `UpdatedBy` | `string` (ObjectId) | ID người cập nhật cuối |
| `IsDeleted` | `bool` | Cờ xóa vật lý (Soft delete) |

---

## 2. Quản lý Nhân sự (Human Resource)

### Employee (Nhân viên)
Thực thể trung tâm lưu trữ thông tin định danh và tài khoản.
- **Fields**: `Code`, `FullName`, `Email`, `CompanyEmail`, `Phone`, `AvatarUrl`, `DepartmentId`, `PositionId`, `ReportToId`, `Status`, `Type`, `JoinDate`, `TerminationDate`, `Username`, `Roles`.
- **Sub-document**: `PersonalDetails` (Lưu thông tin cá nhân bảo mật).

### PersonalInfo (Thông tin cá nhân)
- **Fields**: `DOB` (Ngày sinh), `Gender`, `Address`, `Hometown`, `IdCardNumber` (CCCD), `TaxCode`, `BankAccount`, `BankName`.

---

## 3. Tổ chức (Organization)

### Department (Phòng ban)
- **Fields**: `Name`, `Code`, `Description`, `ParentId` (Phân cấp), `ManagerId` (Trưởng phòng), `Level`, `Path` (Materialized Path).

### Position (Chức vụ/Vị trí)
- **Fields**: `Title`, `Code`, `Description`, `BaseSalaryRangeMin`, `BaseSalaryRangeMax`, `DepartmentId`.

---

## 4. Tuyển dụng (Recruitment)

### JobPosting (Tin tuyển dụng)
- **Fields**: `Title`, `Code`, `DepartmentId`, `Description`, `Requirements`, `Quantity`, `FilledQuantity`, `StartDate`, `EndDate`, `Status`, `Skills`, `AssigneeId` (Recruiter phụ trách).

### Candidate (Ứng viên)
- **Fields**: `FullName`, `Email`, `Phone`, `ResumeUrl`, `JobPostingId`, `Status`, `InterviewScore`, `Notes`.

---

## 5. Hợp đồng lao động (Contracts)

### ContractTemplate (Mẫu hợp đồng)
- **Fields**: `Name`, `Code`, `Type`, `ContentHtml`, `FilePath` (File .docx), `Status`, `Icon`, `Color`.

### EmployeeContract (Hợp đồng nhân viên)
- **Fields**: `EmployeeId`, `TemplateId`, `ContractNumber`, `StartDate`, `EndDate`, `BaseSalary`, `AllowanceTotal`, `Status`, `SignedFileUrl`.

---

## 6. Quản lý Tài sản (Assets)

### CompanyAsset (Tài sản công ty)
- **Fields**: `Name`, `Code`, `SerialNumber`, `Type`, `PurchaseDate`, `PurchasePrice`, `DepreciationPerYear`, `Status`, `CurrentlyAssignedTo`.

### AssetAllocation (Cấp phát tài sản)
- **Fields**: `AssetId`, `EmployeeId`, `HandoverDate`, `HandoverCondition`, `ReturnDate`, `ReturnCondition`, `Remarks`.

---

## 7. Chấm công & Nghỉ phép (Attendance)

### AttendanceRecord (Dữ liệu chấm công)
- **Fields**: `EmployeeId`, `Date`, `CheckInTime`, `CheckOutTime`, `Location`, `ImageUrl`, `IsLate`, `IsEarlyLeave`, `WorkingHours`.

### LeaveRequest (Đơn nghỉ phép)
- **Fields**: `EmployeeId`, `Type` (Bệnh, Phép năm...), `FromDate`, `ToDate`, `TotalDays`, `Reason`, `Status` (Pending/Approved), `ApproverId`.

### WorkShift (Ca làm việc)
- **Fields**: `Name`, `Code`, `StartTime`, `EndTime`, `BreakTime`, `WorkingDays`.

---

## 8. Lương & Phúc lợi (Payroll)

### SalaryComponent (Phần tử lương)
- **Fields**: `Name`, `Code`, `Type` (Thu nhập/Khấu trừ), `IsFixed`, `IsTaxable`.

### SalarySlip (Phiếu lương)
- **Fields**: `EmployeeId`, `PeriodId` (Tháng/Năm), `TotalEarning`, `TotalDeduction`, `NetSalary` (Thực lĩnh), `Details` (Danh sách chi tiết), `Status`.

---

## 9. Đào tạo (Training)

### Course (Khóa học)
- **Fields**: `Title`, `Code`, `Description`, `DurationHours`, `TrainerName`, `IsMandatory`, `MaterialUrl`, `Status`.

### TrainingRecord (Lịch sử đào tạo)
- **Fields**: `EmployeeId`, `CourseId`, `EnrolledDate`, `CompletionDate`, `RecordStatus`, `Score`, `CertificateUrl`.

---

## 10. Hệ thống (System)

### Role & Permission
- **Permission**: `Name`, `Code`, `Module`.
- **Role**: `Name`, `Code`, `PermissionIds` (Mảng lưu các ID quyền).

### SystemConfig
- Lưu cấu hình chung: `CompanyName`, `TaxCode`, `DefaultAnnualLeaveDays`, `ProbationMonths`, `StandardCheckIn/Out`.

---
*Tài liệu này được xuất tự động dựa trên phân tích mã nguồn và dữ liệu seed hiện tại.*
