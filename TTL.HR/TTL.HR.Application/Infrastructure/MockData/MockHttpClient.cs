using System.Net;
using System.Net.Http.Json;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using TTL.HR.Application.Modules.Common.Models;
using TTL.HR.Application.Modules.Dashboard.Models;

namespace TTL.HR.Application.Infrastructure.MockData;

/// <summary>
/// Mock HttpMessageHandler để mô phỏng API calls mà không cần kết nối backend
/// </summary>
public class MockHttpMessageHandler : HttpMessageHandler
{
    private readonly MockDataProvider _mockDataProvider;
    private readonly Dictionary<string, string> _endpointToCollectionMap;
    private readonly JsonSerializerSettings _jsonSettings;

    public MockHttpMessageHandler(MockDataProvider mockDataProvider)
    {
        _mockDataProvider = mockDataProvider;
        _jsonSettings = new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore
        };

        // Map API endpoints sang MongoDB collections
        _endpointToCollectionMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            // Core HR
            { "core/Employees", "employees" },
            { "core/Departments", "departments" },
            { "core/Positions", "positions" },
            { "core/Contracts", "contracts" },
            { "core/ContractTemplates", "contract_templates" },

            // Auth/Profile
            { "auth/profile", "employees" },

            // Attendance & Leave
            { "core/Attendance", "attendances" },
            { "core/Attendance/logs", "attendance_logs" },
            { "core/Attendance/monthly-summaries", "monthly_attendance_summaries" },
            { "core/Attendance/periods", "attendance_periods" },
            { "core/WorkSchedules", "work_schedules" },
            { "core/WorkShifts", "work_shifts" },
            { "core/ShiftChangeRequests", "shift_change_requests" },
            { "core/Leave", "leave_requests" },
            { "core/Leave/types", "leave_types" },
            { "core/Leave/balances", "employee_leave_balances" },
            { "core/Overtime", "overtime_requests" },

            // Payroll & Benefits
            { "core/Payroll", "payrolls" },
            { "core/Payroll/periods", "payroll_periods" },
            { "core/SalaryComponents", "salary_components" },
            { "core/Benefits", "benefits" },
            { "core/Benefits/allocations", "benefit_allocations" },

            // Recruitment
            { "core/Recruitment/jobs", "job_postings" },
            { "core/Recruitment/candidates", "candidates" },
            { "core/JobPostings", "job_postings" },
            { "core/Candidates", "candidates" },

            // Training & Performance
            { "core/Courses", "courses" },
            { "core/Courses/enrollments", "course_enrollments" },
            { "core/Performance/periods", "performance_periods" },
            { "core/Performance/kpi-goals", "kpi_goals" },
            { "core/Performance", "performance_reviews" },

            // Assets
            { "core/Assets", "assets" },
            { "core/Assets/categories", "asset_categories" },
            { "core/Assets/allocations", "asset_allocations" },

            // System
            { "core/Administration/Roles", "roles" },
            { "core/Administration/Permissions", "permissions" },
            { "core/Lookups", "lookups" },
            { "core/System/Settings", "system_settings" },
            { "core/Holidays", "holidays" },
            { "core/Notifications", "notifications" },
            { "core/Audit", "audit_logs" },
            { "core/Banks", "banks" },

            // Administrative Divisions
            { "core/Countries", "countries" },
            { "core/AdministrativeDivisions/provinces", "provinces" },
            { "core/AdministrativeDivisions/districts", "districts" },
            { "core/AdministrativeDivisions/wards", "wards" },
            { "core/administrative-divisions/provinces", "provinces" },
            { "core/administrative-divisions/districts", "districts" },
            { "core/administrative-divisions/wards", "wards" },

            { "core/Employees/dashboard", "employees" },
            { "core/Employees/counts", "employees" },
            { "core/Notifications/my", "notifications" },
            { "core/Attendance/me", "attendances" },
            { "core/Attendance/me/attendance", "attendances" },
            { "core/Attendance/timesheets", "attendances" },
            { "core/Attendance/schedules", "work_schedules" },
            { "core/Attendance/shift-requests", "shift_change_requests" },
            { "core/Attendance/shifts", "work_shifts" },
            { "core/Attendance/overtime", "overtime_requests" },
            { "core/Attendance/employee", "attendances" },
            { "core/Payroll/me", "payrolls" },
            { "core/Dashboard/overview", "employees" }, 
        };

        Console.WriteLine("🎭 MockHttpMessageHandler đã được khởi tạo - Chế độ OFFLINE");
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var method = request.Method.Method;
        var requestUri = request.RequestUri?.ToString() ?? "";

        Console.WriteLine($"🎭 [MOCK] {method}: {requestUri}");

        // Handle specific endpoints like Login
        if (method == "POST")
        {
            var uriStr = requestUri.ToLower();
            if (uriStr.Contains("auth/login"))
            {
                return HandleMockLogin(request);
            }
            if (uriStr.Contains("auth/logout"))
            {
                return CreateSuccessResponse(new ApiResponse<object> { Success = true, Message = "Logout Mock" });
            }
            if (uriStr.Contains("core/departments") && uriStr.EndsWith("/assign"))
            {
                return CreateSuccessResponse(new ApiResponse<object> { Success = true, Message = "Gán nhân sự thành công (Chế độ Mock)" });
            }
            if (uriStr.Contains("core/positions") && uriStr.EndsWith("/assign"))
            {
                return CreateSuccessResponse(new ApiResponse<object> { Success = true, Message = "Gán nhân sự thành công (Chế độ Mock)" });
            }
        }

        try
        {
            if (method == "GET")
            {
                return HandleGet(requestUri);
            }
            else if (method == "POST")
            {
                return await HandlePostPut(requestUri, request, "Created successfully (Mock Mode)");
            }
            else if (method == "PUT")
            {
                return await HandlePostPut(requestUri, request, "Updated successfully (Mock Mode)");
            }
            else if (method == "DELETE")
            {
                Console.WriteLine($"   ⚠️  Chế độ mock - dữ liệu không được xóa thực sự");
                return CreateSuccessResponse(new ApiResponse<object>
                {
                    Success = true,
                    Message = "Deleted successfully (Mock Mode)"
                });
            }

            return CreateErrorResponse($"Method {method} not supported in mock");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ [MOCK] Lỗi: {ex.Message}");
            return CreateErrorResponse(ex.Message);
        }
    }

    private HttpResponseMessage HandleMockLogin(HttpRequestMessage request)
    {
        var userDto = new UserDto
        {
            Id = "mock-admin-id-123",
            Username = "admin",
            Email = "admin@tantanloc.com",
            FullName = "Administrator (Mock Mode)",
            Role = "Admin",
            AvatarUrl = "https://ui-avatars.com/api/?name=Admin+Mock&background=random",
            JobTitle = "System Admin",
            Phone = "0123456789"
        };

        var authResponse = new AuthResponse
        {
            AccessToken = "mock-token-abc123-valid",
            RefreshToken = "mock-refresh-token",
            User = userDto
        };

        return CreateSuccessResponse(new ApiResponse<AuthResponse>
        {
            Success = true,
            Data = authResponse,
            Message = "Đăng nhập thành công (Chế độ Mock Offline)"
        });
    }

    private HttpResponseMessage HandleGet(string requestUri)
    {
        Console.WriteLine($"   🌐 Mock GET Request: {requestUri}");
        // Parse request URI
        var uri = new Uri(requestUri, UriKind.RelativeOrAbsolute);
        var path = uri.IsAbsoluteUri ? uri.AbsolutePath : requestUri;
        var query = uri.IsAbsoluteUri ? uri.Query : string.Empty;

        path = path.TrimStart('/');
        var segments = path.Split('/', StringSplitOptions.RemoveEmptyEntries);
        var qString = query; // Use local query variable
        var queryParams = ParseQueryString(qString); // Centralized for all handlers
        var endpoint = ExtractEndpoint(path);
        
        Console.WriteLine($"   📍 Path: {path}");
        Console.WriteLine($"   📍 Endpoint: {endpoint}");
        Console.WriteLine($"   📍 Segments ({segments.Length}): {string.Join(" | ", segments)}");

        // Specialized handler for Department Details to include Members
        if (endpoint.StartsWith("core/Departments/", StringComparison.OrdinalIgnoreCase) && segments.Length >= 3)
        {
            var targetDeptId = segments[^1];
            if (IsObjectId(targetDeptId))
            {
                var dept = GetItemById("departments", targetDeptId);
                if (dept != null)
                {
                    var employees = GetAllItems("employees");
                    var positions = GetAllItems("positions");
                    
                    var members = employees
                        .Where(e => MatchId(e, "DepartmentObjectId", "DepartmentId", targetDeptId))
                        .Select(e => {
                            var posId = GetProperty(e, "PositionId")?.ToString();
                            var pos = positions.FirstOrDefault(p => MatchId(p, "_id", "id", posId));
                            
                            return new {
                                Id = GetProperty(e, "_id")?.ToString() ?? GetProperty(e, "id")?.ToString(),
                                FullName = GetProperty(e, "FullName")?.ToString(),
                                AvatarUrl = GetProperty(e, "AvatarUrl")?.ToString(),
                                PositionName = GetProperty(pos, "Name")?.ToString() ?? "Nhân viên",
                                Email = GetProperty(e, "Email")?.ToString(),
                                Status = GetProperty(e, "Status")?.ToString() == "1" ? "Active" : "Probation",
                                JoinDate = GetProperty(e, "JoinedDate") ?? DateTime.Now
                            };
                        }).ToList();

                    var detail = JObject.FromObject(dept);
                    detail["Members"] = JArray.FromObject(members);
                    detail["ActiveMembers"] = members.Count(m => m.Status == "Active");
                    detail["EmployeeCount"] = members.Count;
                    detail["PendingReviews"] = 0;

                    return CreateSuccessResponse(new ApiResponse<object> { Success = true, Data = TransformItem(detail), Message = "Success" });
                }
            }
        }

        // 1. Handle Administrative Divisions (Nested patterns)
        // Pattern: .../provinces/{id}/districts
        if (segments.Length >= 3 && segments[^1].Equals("districts", StringComparison.OrdinalIgnoreCase) && segments[^3].Equals("provinces", StringComparison.OrdinalIgnoreCase))
        {
            var provinceId = segments[^2];
            var districts = GetAllItems("districts")
                .Where(d => MatchId(d, "ProvinceObjectId", "ProvinceId", provinceId))
                .Select(TransformItem)
                .ToList();
            return CreateSuccessResponse(new ApiResponse<List<object>> { Success = true, Data = districts, Message = "Success" });
        }
        
        // Pattern: .../districts/{id}/wards
        if (segments.Length >= 3 && segments[^1].Equals("wards", StringComparison.OrdinalIgnoreCase) && segments[^3].Equals("districts", StringComparison.OrdinalIgnoreCase))
        {
            var districtId = segments[^2];
            var wards = GetAllItems("wards")
                .Where(w => MatchId(w, "DistrictObjectId", "DistrictId", districtId))
                .Select(TransformItem)
                .ToList();
            return CreateSuccessResponse(new ApiResponse<List<object>> { Success = true, Data = wards, Message = "Success" });
        }

        // 2. Handle /me or /my endpoints
        endpoint = ExtractEndpoint(path);

        // Special case: Dashboard Overview
        if (endpoint.Equals("core/Dashboard/overview", StringComparison.OrdinalIgnoreCase))
        {
            var employees = GetAllItems("employees");
            var departments = GetAllItems("departments");
            var jobs = GetAllItems("job_postings");
            var courses = GetAllItems("courses");

            var stats = new {
                Stats = new {
                    TotalEmployees = employees.Count,
                    TotalDepartments = departments.Count,
                    ActiveJobPostings = jobs.Count,
                    TotalCourses = courses.Count
                },
                PayrollStats = new {
                    TotalBudget = 1500000000m,
                    GrowthPercentage = 5.2,
                    Period = DateTime.Now.ToString("MM/yyyy")
                },
                ContractDistribution = new {
                    OfficialPercentage = 85.0,
                    Label = "Official"
                },
                RecruitmentStats = new {
                    NewCandidates = 12,
                    InterviewsThisWeek = 5,
                    RecentlyHired = 2
                },
                DepartmentDistribution = departments.Select(d => {
                    var dId = GetProperty(d, "id")?.ToString() ?? "";
                    return new DepartmentStat {
                        DepartmentName = GetProperty(d, "Name")?.ToString() ?? "Unknown",
                        EmployeeCount = 3 // Standardize to 3 for better visibility in mock charts
                    };
                }).OrderByDescending(d => d.EmployeeCount).Take(5).ToList(),
                UpcomingTrainings = new List<object> {
                    new { Title = "Kỹ năng giao tiếp chuyên nghiệp", Trainer = "Lê Hồng Nhung", DurationHours = 4, Status = "Sắp diễn ra" },
                    new { Title = "Quy trình ISO 9001:2015", Trainer = "Trần Văn Nam", DurationHours = 8, Status = "Đang thực hiện" }
                },
                PendingApprovals = new List<object> {
                    new { Title = "Đơn nghỉ phép - Nguyễn Văn A", Type = "Leave", Date = DateTime.Today.AddDays(-1), Status = "Pending" },
                    new { Title = "Đơn đổi ca - Trần Thị B", Type = "Shift", Date = DateTime.Today.AddDays(-2), Status = "Pending" }
                },
                ExpiringContracts = new List<object> {
                    new { EmployeeName = "Phạm Thị Thùy", Department = "Hành chính", ExpiryDate = DateTime.Today.AddDays(15) },
                    new { EmployeeName = "Hoàng Văn Thái", Department = "Sản xuất", ExpiryDate = DateTime.Today.AddDays(22) }
                },
                AttendanceToday = new {
                    Present = 92,
                    Absent = 5,
                    Late = 3,
                    Total = employees.Count
                },
                PendingAssetClearances = new List<object> {
                    new { EmployeeName = "Nguyễn Hùng Anh", AssetName = "Laptop Dell Precision", AssetCode = "LP-001", DepartureDate = DateTime.Today.AddDays(-5) }
                },
                BirthdayAlerts = new List<object> {
                    new { EmployeeName = "Nguyễn Minh Tuấn", DOB = DateTime.Today, DaysUntilBirthday = 0 },
                    new { EmployeeName = "Trần Thu Hà", DOB = DateTime.Today.AddDays(3), DaysUntilBirthday = 3 }
                },
                PayrollTrend = new List<object> {
                    new { Month = "01/2026", Amount = 1420000000 },
                    new { Month = "02/2026", Amount = 1480000000 },
                    new { Month = "03/2026", Amount = 1500000000 }
                }
            };
            return CreateSuccessResponse(new ApiResponse<object> { Success = true, Data = stats, Message = "Success" });
        }

        // Special case: Employee Dashboard (Status counts)
        if (endpoint.Equals("core/Employees/dashboard", StringComparison.OrdinalIgnoreCase) || 
            endpoint.Equals("core/Employees/counts", StringComparison.OrdinalIgnoreCase))
        {
            var employees = GetAllItems("employees");
            var stats = new {
                All = employees.Count,
                Active = employees.Count(e => GetProperty(e, "Status")?.ToString() == "1"),
                Probation = employees.Count(e => GetProperty(e, "Status")?.ToString() == "2"),
                Resigned = employees.Count(e => GetProperty(e, "Status")?.ToString() == "3")
            };
            return CreateSuccessResponse(new ApiResponse<object> { Success = true, Data = stats, Message = "Success" });
        }


        // Special case: Overtime Summary
        if (endpoint.Contains("overtime/summary", StringComparison.OrdinalIgnoreCase))
        {
            var summary = new TTL.HR.Application.Modules.Attendance.Models.OvertimeSummaryModel
            {
                PendingCount = 0,
                ApprovedCount = 3,
                RejectedCount = 1,
                TotalHours = 5.5
            };
            return CreateSuccessResponse(new ApiResponse<object> { Success = true, Data = summary, Message = "Success" });
        }

        // Special case: Leave Summary
        if (endpoint.EndsWith("core/Leave/summary", StringComparison.OrdinalIgnoreCase))
        {
            var leaves = GetAllItems("leave_requests");
            var summary = new TTL.HR.Application.Modules.Leave.Models.LeaveStateSummaryModel
            {
                PendingCount = leaves.Count(l => {
                    var s = GetProperty(l, "Status")?.ToString();
                    var sid = GetProperty(l, "StatusId")?.ToString();
                    return s == "Pending" || s == "Chờ phê duyệt" || s == "Chờ duyệt" || sid == "1";
                }),
                ApprovedCount = leaves.Count(l => {
                    var s = GetProperty(l, "Status")?.ToString();
                    var sid = GetProperty(l, "StatusId")?.ToString();
                    return s == "Approved" || s == "Đã phê duyệt" || s == "Đã duyệt" || sid == "2";
                }),
                RejectedCount = leaves.Count(l => {
                    var s = GetProperty(l, "Status")?.ToString();
                    var sid = GetProperty(l, "StatusId")?.ToString();
                    return s == "Rejected" || s == "Từ chối" || sid == "3";
                }),
                CancelledCount = leaves.Count(l => {
                    var s = GetProperty(l, "Status")?.ToString();
                    var sid = GetProperty(l, "StatusId")?.ToString();
                    return s == "Cancelled" || s == "Đã hủy" || sid == "4" || sid == "8"; // 8 often used for withdrawn
                }),
                AssignedToMeCount = 0,
                TotalCount = leaves.Count
            };
            return CreateSuccessResponse(new ApiResponse<object> { Success = true, Data = summary, Message = "Success" });
        }

        // Special case: Payroll Period Detail
        if (endpoint.Contains("Payroll/periods", StringComparison.OrdinalIgnoreCase) && endpoint.EndsWith("/detail", StringComparison.OrdinalIgnoreCase))
        {
            Console.WriteLine($"   🔍 Intercepted specialized Payroll Detail: {endpoint}");
            // Endpoint can be: core/Payroll/periods/{id}/detail 
            // OR core/Payroll/periods/{year}/{month}/detail
            string? periodId = null;
            int? year = null;
            int? month = null;

            if (segments.Length >= 4 && segments[segments.Length - 1].Equals("detail", StringComparison.OrdinalIgnoreCase))
            {
                // Last is "detail", second to last could be ID or month
                var secondToLast = segments[segments.Length - 2];
                
                if (IsObjectId(secondToLast))
                {
                    periodId = secondToLast.Trim();
                }
                else if (int.TryParse(secondToLast, out var m) && segments.Length >= 5)
                {
                    var thirdToLast = segments[segments.Length - 3];
                    if (int.TryParse(thirdToLast, out var y))
                    {
                        year = y;
                        month = m;
                    }
                }
            }

            object? period = null;
            if (periodId != null)
            {
                period = GetItemById("payroll_periods", periodId);
            }
            else if (year.HasValue && month.HasValue)
            {
                var allPeriods = GetAllItems("payroll_periods");
                period = allPeriods.FirstOrDefault(p => {
                    var py = GetProperty(p, "Year")?.ToString();
                    var pm = GetProperty(p, "Month")?.ToString();
                    return py == year.ToString() && pm == month.ToString();
                });
            }

            if (period == null) 
            {
                Console.WriteLine($"   ❌ Payroll Period NOT FOUND for ID: {periodId ?? "null"} or Year: {year?.ToString() ?? "null"}, Month: {month?.ToString() ?? "null"}");
                return CreateNotFoundResponse("Không tìm thấy kỳ lương. Vui lòng kiểm tra lại ID hoặc kỳ (Tháng/Năm).");
            }
            
            Console.WriteLine($"   ✅ Found Period: {GetProperty(period, "Name")}");

            var actualPeriodId = GetProperty(period, "_id")?.ToString() ?? GetProperty(period, "id")?.ToString() ?? periodId;
            bool isDeleted = GetProperty(period, "IsDeleted")?.ToString().Equals("True", StringComparison.OrdinalIgnoreCase) ?? false;
            if (isDeleted) Console.WriteLine($"   ⚠️  Warning: Period {actualPeriodId} is marked as DELETED in mock data");

            var pQueryParams = ParseQueryString(query);
            var departmentId = pQueryParams.TryGetValue("departmentId", out var dId) ? dId : null;
            var searchTerm = pQueryParams.TryGetValue("searchTerm", out var sTerm) ? sTerm : null;

            var allPayrolls = GetAllItems("payrolls");
            var periodPayrolls = allPayrolls.Where(p => {
                // Link by PeriodId
                bool matchesPeriod = MatchId(p, "PeriodObjectId", "PeriodId", actualPeriodId);
                if (!matchesPeriod) return false;
                
                // Filter by department if specified
                if (!string.IsNullOrEmpty(departmentId) && departmentId != "0" && departmentId != "null")
                {
                    if (!MatchId(p, "DepartmentObjectId", "DepartmentId", departmentId))
                        return false;
                }
                    
                // Filter by search term
                if (!string.IsNullOrEmpty(searchTerm))
                {
                    var name = GetProperty(p, "EmployeeName")?.ToString() ?? "";
                    var code = GetProperty(p, "EmployeeCode")?.ToString() ?? "";
                    if (!name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) && 
                        !code.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                        return false;
                }
                
                return true;
            }).ToList();

            var pPageIndex = pQueryParams.TryGetValue("pageIndex", out var pPageIdxStr) && int.TryParse(pPageIdxStr, out var pPi) ? pPi : 1;
            var pPageSize = pQueryParams.TryGetValue("pageSize", out var pPageSizeStr) && int.TryParse(pPageSizeStr, out var pPs) ? pPs : 20;

            var totalCount = periodPayrolls.Count;
            var pagedPayrolls = periodPayrolls.Skip((pPageIndex - 1) * pPageSize).Take(pPageSize).Select(p => TransformItem(p)).ToList();

            var detail = new {
                Period = TransformItem(period),
                Payrolls = new {
                    Items = pagedPayrolls,
                    TotalCount = totalCount,
                    PageIndex = pPageIndex,
                    PageSize = pPageSize
                }
            };
            return CreateSuccessResponse(new ApiResponse<object> { Success = true, Data = detail, Message = "Success" });
        }
        
        // 5. Training - Courses and Analytics
        if (endpoint.Contains("Courses", StringComparison.OrdinalIgnoreCase) || endpoint.Contains("Training", StringComparison.OrdinalIgnoreCase))
        {
            // Details: core/Courses/{id}
            if (segments.Length >= 3 && segments[1].Equals("Courses", StringComparison.OrdinalIgnoreCase) && IsObjectId(segments[2]))
            {
                var courseId = segments[2];
                var course = GetItemById("courses", courseId);
                if (course == null) return CreateNotFoundResponse($"Khóa học {courseId} không tồn tại.");
                
                var enrollments = GetAllItems("course_enrollments").Where(e => MatchId(e, "CourseId", "CourseId", courseId)).ToList();
                var employees = GetAllItems("employees");
                
                var enrichedCourse = JObject.FromObject(course);
                enrichedCourse["EnrolledCount"] = JToken.FromObject(enrollments.Count);
                enrichedCourse["EnrolledEmployeeIds"] = JArray.FromObject(enrollments.Select(e => GetProperty(e, "EmployeeId")).ToList());
                
                // Populate EnrolledEmployees list
                var participantList = new JArray();
                foreach (var enroll in enrollments)
                {
                    var empId = GetProperty(enroll, "EmployeeId")?.ToString();
                    var emp = employees.FirstOrDefault(e => MatchId(e, "_id", "id", empId));
                    if (emp != null)
                    {
                        var participant = new JObject();
                        participant["Id"] = JToken.FromObject(GetProperty(enroll, "_id")?.ToString() ?? "");
                        participant["EmployeeId"] = JToken.FromObject(empId ?? "");
                        participant["EmployeeName"] = JToken.FromObject(GetProperty(emp, "FullName")?.ToString() ?? "");
                        participant["EmployeeCode"] = JToken.FromObject(GetProperty(emp, "EmployeeCode")?.ToString() ?? "");
                        participant["DepartmentName"] = JToken.FromObject(GetProperty(emp, "DepartmentName")?.ToString() ?? "");
                        participant["PositionName"] = JToken.FromObject(GetProperty(emp, "PositionName")?.ToString() ?? "");
                        participant["Status"] = JToken.FromObject(GetProperty(enroll, "Status")?.ToString() ?? "Enrolled");
                        participant["Progress"] = JToken.FromObject(GetProperty(enroll, "Progress") ?? 0);
                        participant["EnrolledDate"] = JToken.FromObject(GetProperty(enroll, "EnrolledDate") ?? DateTime.Now);
                        participantList.Add(participant);
                    }
                }
                enrichedCourse["EnrolledEmployees"] = participantList;
                
                // Set Status string from StatusId if needed
                if (enrichedCourse["Status"] == null || string.IsNullOrEmpty(enrichedCourse["Status"].ToString()))
                {
                    var statusId = GetProperty(course, "StatusId")?.ToString();
                    enrichedCourse["Status"] = JToken.FromObject(statusId switch { "3" => "Completed", "71" => "Active", _ => "Draft" });
                }
                
                return CreateSuccessResponse(new ApiResponse<object> { Success = true, Data = enrichedCourse, Message = "Success" });
            }
            
            // Analytics: core/Training/analytics
            if (endpoint.EndsWith("/analytics", StringComparison.OrdinalIgnoreCase))
            {
                var allCourses = GetAllItems("courses");
                var allEnrollments = GetAllItems("course_enrollments");
                
                var topCourses = allCourses.Take(5).Select(c => {
                    var cid = GetProperty(c, "_id")?.ToString() ?? GetProperty(c, "id")?.ToString() ?? "";
                    return new {
                        CourseName = GetProperty(c, "Title")?.ToString() ?? "Unknown",
                        EnrolledCount = allEnrollments.Count(e => MatchId(e, "CourseId", "CourseId", cid))
                    };
                }).ToList();

                var analytics = new {
                    TotalCourses = allCourses.Count,
                    ActiveCourses = allCourses.Count(c => GetProperty(c, "StatusId")?.ToString() == "71"),
                    TotalEnrollments = allEnrollments.Count,
                    CompletedEnrollments = allEnrollments.Count(e => GetProperty(e, "Status")?.ToString() == "Completed"),
                    CompletionRate = allEnrollments.Count > 0 ? (double)allEnrollments.Count(e => GetProperty(e, "Status")?.ToString() == "Completed") / allEnrollments.Count * 100 : 0,
                    TopCourses = topCourses,
                    EnrollmentTrends = new List<object> { 
                        new { Month = "Jan", Enrolled = 10, Completed = 5 },
                        new { Month = "Feb", Enrolled = 15, Completed = 8 },
                        new { Month = "Mar", Enrolled = 12, Completed = 10 }
                    }
                };
                return CreateSuccessResponse(new ApiResponse<object> { Success = true, Data = analytics, Message = "Success" });
            }
            
            // List: core/Courses
            if (endpoint.Equals("core/Courses", StringComparison.OrdinalIgnoreCase))
            {
                var qParams = ParseQueryString(query);
                var cPageIndex = qParams.TryGetValue("pageIndex", out var cpIdxStr) && int.TryParse(cpIdxStr, out var cpIdx) ? cpIdx : 1;
                var cPageSize = qParams.TryGetValue("pageSize", out var cpSizeStr) && int.TryParse(cpSizeStr, out var cpSize) ? cpSize : 10;
                
                var courses = GetAllItems("courses");
                var enrollments = GetAllItems("course_enrollments");
                
                var enrichedCourses = courses.Select(c => {
                    var cid = GetProperty(c, "_id")?.ToString() ?? GetProperty(c, "id")?.ToString();
                    var enriched = JObject.FromObject(c);
                    var matchedEnrollments = enrollments.Where(e => MatchId(e, "CourseId", "CourseId", cid)).ToList();
                    enriched["EnrolledCount"] = JToken.FromObject(matchedEnrollments.Count);
                    enriched["EnrolledEmployeeIds"] = JArray.FromObject(matchedEnrollments.Select(e => GetProperty(e, "EmployeeId")).ToList());
                    
                    if (enriched["Status"] == null || string.IsNullOrEmpty(enriched["Status"].ToString()))
                    {
                        var statusId = GetProperty(c, "StatusId")?.ToString();
                        enriched["Status"] = JToken.FromObject(statusId switch { "3" => "Completed", "71" => "Active", _ => "Draft" });
                    }
                    return enriched;
                }).Cast<object>().ToList();
                
                var cTotal = enrichedCourses.Count;
                var paged = enrichedCourses.Skip((cPageIndex - 1) * cPageSize).Take(cPageSize).Select(TransformItem).ToList();
                
                var cpagedRes = new PagedResult<object> {
                    Items = paged,
                    PageIndex = cPageIndex,
                    PageSize = cPageSize,
                    TotalCount = cTotal
                };
                return CreateSuccessResponse(new ApiResponse<PagedResult<object>> { Success = true, Data = cpagedRes, Message = "Success" });
            }
        }

            // 4. Administration Roles & Permissions
            if (endpoint.Contains("Administration/Roles", StringComparison.OrdinalIgnoreCase) || 
                endpoint.Contains("Administration/Permissions", StringComparison.OrdinalIgnoreCase))
            {
                var roleParams = ParseQueryString(query);
                var rolesList = GetAllItems("roles");
                var permissionsList = GetAllItems("permissions");
                var employees = GetAllItems("employees");
                var departmentsList = GetAllItems("departments");
                var positionsList = GetAllItems("positions");

                // Case: core/administration/roles/permissions (Permission List for UI pickers)
                if (endpoint.EndsWith("/permissions", StringComparison.OrdinalIgnoreCase) && endpoint.Contains("/roles", StringComparison.OrdinalIgnoreCase))
                {
                    var pList = permissionsList.Select(p => new {
                        Id = GetProperty(p, "_id")?.ToString() ?? GetProperty(p, "id")?.ToString(),
                        Code = GetProperty(p, "Code")?.ToString(),
                        Name = GetProperty(p, "Name")?.ToString(),
                        Group = GetProperty(p, "Module")?.ToString() ?? "Hệ thống"
                    }).ToList();
                    return CreateSuccessResponse(new ApiResponse<List<object>> { Success = true, Data = pList.Cast<object>().ToList(), Message = "Success" });
                }

                // Case: core/administration/permissions (Permissions with assigned roles)
                if (endpoint.Equals("core/Administration/Permissions", StringComparison.OrdinalIgnoreCase))
                {
                    var enrichedPerms = permissionsList.Select(p => {
                        var pid = GetProperty(p, "_id")?.ToString() ?? GetProperty(p, "id")?.ToString();
                        var pCode = GetProperty(p, "Code")?.ToString();
                        var assignedRoles = rolesList.Where(r => {
                            var rPerms = GetProperty(r, "PermissionIds") as JArray;
                            return rPerms != null && rPerms.Any(rp => rp.ToString() == pid || rp.ToString() == pCode);
                        }).Select(r => new {
                            Id = GetProperty(r, "_id")?.ToString() ?? GetProperty(r, "id")?.ToString(),
                            Name = GetProperty(r, "Name")?.ToString()
                        }).ToList();

                        var enriched = JObject.FromObject(p);
                        enriched["Id"] = JToken.FromObject(pid ?? "");
                        enriched["Group"] = JToken.FromObject(GetProperty(p, "Module")?.ToString() ?? "Hệ thống");
                        enriched["AssignedRoles"] = JArray.FromObject(assignedRoles);
                        return enriched;
                    }).Cast<object>().ToList();
                    return CreateSuccessResponse(new ApiResponse<List<object>> { Success = true, Data = enrichedPerms, Message = "Success" });
                }

                // Case: core/administration/roles/{id} (Detail)
                var roleIdSearch = segments.LastOrDefault();
                if (segments.Length > 2 && roleIdSearch != null && (IsObjectId(roleIdSearch) || roleIdSearch.Length > 20) && !roleIdSearch.Equals("Roles", StringComparison.OrdinalIgnoreCase))
                {
                    var role = rolesList.FirstOrDefault(r => MatchId(r, "_id", "id", roleIdSearch));
                    if (role != null)
                    {
                        var enriched = JObject.FromObject(role);
                        var rid = GetProperty(role, "_id")?.ToString() ?? GetProperty(role, "id")?.ToString();
                        
                        // PermissionNames
                        var pIds = GetProperty(role, "PermissionIds") as JArray;
                        if (pIds != null)
                        {
                            var pNames = permissionsList.Where(p => {
                                var pid = GetProperty(p, "_id")?.ToString() ?? GetProperty(p, "id")?.ToString();
                                var pCode = GetProperty(p, "Code")?.ToString();
                                return pIds.Any(rp => rp.ToString() == pid || rp.ToString() == pCode);
                            }).Select(p => GetProperty(p, "Name")?.ToString()).ToList();
                            enriched["PermissionNames"] = JArray.FromObject(pNames);
                            enriched["Permissions"] = JArray.FromObject(pIds);
                        }

                        // Members
                        var members = employees.Where(e => {
                            var eRoles = GetProperty(e, "Roles") as JArray;
                            return eRoles != null && eRoles.Any(er => er.ToString() == rid);
                        }).Select(e => {
                            var deptId = GetProperty(e, "DepartmentId")?.ToString();
                            var posId = GetProperty(e, "PositionId")?.ToString();
                            var fdept = departmentsList.FirstOrDefault(d => MatchId(d, "_id", "id", deptId));
                            var fpos = positionsList.FirstOrDefault(p => MatchId(p, "_id", "id", posId));
                            
                            return new {
                                Id = GetProperty(e, "_id")?.ToString() ?? GetProperty(e, "id")?.ToString(),
                                Name = GetProperty(e, "FullName")?.ToString(),
                                Code = GetProperty(e, "Code")?.ToString(),
                                Email = GetProperty(e, "Email")?.ToString(),
                                Avatar = GetProperty(e, "AvatarUrl")?.ToString(),
                                Department = GetProperty(fdept, "Name")?.ToString() ?? "N/A",
                                Position = GetProperty(fpos, "Name")?.ToString() ?? "N/A"
                            };
                        }).ToList();
                        
                        enriched["Members"] = JArray.FromObject(members);
                        enriched["UsersCount"] = JToken.FromObject(members.Count);
                        enriched["IsSystem"] = JToken.FromObject(GetProperty(role, "IsSystemRole") ?? false);

                        return CreateSuccessResponse(new ApiResponse<object> { Success = true, Data = enriched, Message = "Success" });
                    }
                }

                // Case: core/administration/roles (List)
                if (endpoint.Equals("core/Administration/Roles", StringComparison.OrdinalIgnoreCase))
                {
                    var rPageIndex = roleParams.TryGetValue("pageIndex", out var rIdxStr) && int.TryParse(rIdxStr, out var rIdx) ? rIdx : 1;
                    var rPageSize = roleParams.TryGetValue("pageSize", out var rSizeStr) && int.TryParse(rSizeStr, out var rSize) ? rSize : 20;
                    var searchTerm = roleParams.TryGetValue("searchTerm", out var rSt) ? rSt : 
                                    roleParams.TryGetValue("SearchTerm", out var rSt2) ? rSt2 : null;

                    var filteredRoles = rolesList;
                    if (!string.IsNullOrEmpty(searchTerm))
                    {
                        filteredRoles = filteredRoles.Where(r => 
                            (GetProperty(r, "Name")?.ToString()?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ?? false) ||
                            (GetProperty(r, "Code")?.ToString()?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ?? false)
                        ).ToList();
                    }

                    var enrichedRoles = filteredRoles.Select(r => {
                        var rid = GetProperty(r, "_id")?.ToString() ?? GetProperty(r, "id")?.ToString();
                        var enriched = JObject.FromObject(r);
                        
                        // UsersCount
                        var usersCount = employees.Count(e => {
                            var eRoles = GetProperty(e, "Roles") as JArray;
                            return eRoles != null && eRoles.Any(er => er.ToString() == rid);
                        });
                        
                        enriched["UsersCount"] = JToken.FromObject(usersCount);
                        enriched["IsSystem"] = JToken.FromObject(GetProperty(r, "IsSystemRole") ?? false);
                        enriched["Permissions"] = JArray.FromObject(GetProperty(r, "PermissionIds") ?? new JArray());
                        
                        return enriched;
                    }).Cast<object>().ToList();

                    var rTotal = enrichedRoles.Count;
                    var rPaged = enrichedRoles.Skip((rPageIndex - 1) * rPageSize).Take(rPageSize).Select(TransformItem).ToList();

                    var rolePagedRes = new PagedResult<object> {
                        Items = rPaged,
                        PageIndex = rPageIndex,
                        PageSize = rPageSize,
                        TotalCount = rTotal
                    };
                    return CreateSuccessResponse(new ApiResponse<PagedResult<object>> { Success = true, Data = rolePagedRes, Message = "Success" });
                }
            }

            // Specialized Handle for "Me" / "My" endpoints that expect a list
            if (path.EndsWith("/Payroll/me", StringComparison.OrdinalIgnoreCase))
            {
                var payQueryParams = ParseQueryString(query);
                var empId = payQueryParams.GetValueOrDefault("employeeId") ?? "65bf0c300000000000000001";
                var year = payQueryParams.TryGetValue("year", out var yrStr) && int.TryParse(yrStr, out var yr) ? yr : DateTime.Now.Year;

                var payrolls = _mockDataProvider.GetCollection<object>("payrolls")
                    .Where(p => MatchId(p, "EmployeeId", null, empId))
                    .Where(p => {
                        var pYear = Convert.ToInt32(GetProperty(p, "Year") ?? 0);
                        var isDeleted = GetProperty(p, "IsDeleted") as bool? ?? false;
                        return pYear == year && !isDeleted;
                    })
                    .OrderByDescending(p => GetProperty(p, "Month"))
                    .ToList();
                
                var transformed = payrolls.Select(p => {
                    var item = JObject.FromObject(TransformItem(p));
                    
                    // Add some dummy detail breakdowns if not present
                    if (item["allowanceDetails"] == null || !item["allowanceDetails"].Any())
                    {
                        item["allowanceDetails"] = JArray.FromObject(new List<object> {
                            new { Name = "Phụ cấp xăng xe", Amount = 500000, Note = "Cố định hàng tháng" },
                            new { Name = "Phụ cấp ăn trưa", Amount = 730000, Note = "Theo ngày công thực tế" },
                            new { Name = "Phụ cấp điện thoại", Amount = 200000, Note = "Hỗ trợ công việc" }
                        });
                    }

                    if (item["statusName"] == null || string.IsNullOrEmpty(item["statusName"].ToString()))
                    {
                        var statusId = item["statusId"]?.ToString();
                        item["statusName"] = statusId == "2" ? "Đã chốt" : "Bản nháp";
                        item["statusColor"] = statusId == "2" ? "success" : "warning";
                    }
                    
                    if (item["deductionDetails"] == null || !item["deductionDetails"].Any())
                    {
                        item["deductionDetails"] = JArray.FromObject(new List<object> {
                            new { Name = "Khấu trừ đi muộn", Amount = 50000, Note = "2 lần đi muộn > 15p" },
                            new { Name = "Tạm ứng", Amount = 1000000, Note = "Tạm ứng giữa tháng" }
                        });
                    }
                    
                    return item;
                }).ToList();
                
                var paged = new PagedResult<object>
                {
                    Items = transformed.Cast<object>().ToList(),
                    PageIndex = 1,
                    PageSize = 10,
                    TotalCount = transformed.Count
                };
                return CreateSuccessResponse(new ApiResponse<object> { Success = true, Data = paged, Message = "Success" });
            }

            // Attendance History
            if (path.Contains("/Attendance/employee/", StringComparison.OrdinalIgnoreCase))
            {
                var employeeIndex = Array.FindIndex(segments, s => s.Equals("employee", StringComparison.OrdinalIgnoreCase));
                var empId = employeeIndex >= 0 && segments.Length > employeeIndex + 1 ? segments[employeeIndex + 1] : "65bf0c300000000000000001";
                if (empId.Contains("?")) empId = empId.Split('?')[0];

                Console.WriteLine($"🔍 Mock GET Attendance: path={path}, empId={empId}");
                
                var attQueryParams = ParseQueryString(uri.Query);
                int? month = attQueryParams.TryGetValue("month", out var mStr) && int.TryParse(mStr, out var m) ? m : null;
                int? year = attQueryParams.TryGetValue("year", out var yStr) && int.TryParse(yStr, out var y) ? y : null;

                var attendances = _mockDataProvider.GetCollection<object>("attendances")
                    .Where(a => MatchId(a, "EmployeeId", "EmployeeCode", empId))
                    .Where(a => {
                        var dateProp = GetProperty(a, "Date");
                        if (dateProp == null) return false;
                        if (DateTime.TryParse(dateProp.ToString(), out var dt)) {
                            // Filter by month/year if provided
                            bool monthMatch = !month.HasValue || dt.Month == month.Value;
                            bool yearMatch = !year.HasValue || dt.Year == year.Value;
                            return monthMatch && yearMatch;
                        }
                        return true; // Keep it if we can't parse date, to be safe
                    })
                    .OrderByDescending(a => {
                        var dateProp = GetProperty(a, "Date");
                        DateTime.TryParse(dateProp?.ToString() ?? "", out var dt);
                        return dt;
                    })
                    .ToList();

                // Check if it's a stats request
                if (path.Contains("/stats", StringComparison.OrdinalIgnoreCase))
                {
                    var stats = new 
                    {
                        EmployeeId = empId,
                        TotalEntitledLeave = 12.0,
                        UsedLeaveYear = 3.5,
                        UsedLeaveMonth = 1.0,
                        RemainingLeave = 8.5,
                        TotalWorkingHoursMonth = attendances.Sum(a => Convert.ToDouble(GetProperty(a, "WorkingHours") ?? 0))
                    };
                    return CreateSuccessResponse(new ApiResponse<object> { Success = true, Data = stats, Message = "Success" });
                }

                // Normal detail request - return EmployeeAttendanceDetailDto
                var employee = _mockDataProvider.GetById<object>("employees", empId);
                
                var logs = attendances
                    .GroupBy(a => {
                        var d = GetProperty(a, "Date")?.ToString();
                        return DateTime.TryParse(d, out var dt) ? dt.Date : DateTime.Today.Date;
                    })
                    .Select(g => {
                        DateTime? earliestIn = null;
                        DateTime? latestOut = null;
                        double totalWorkValue = 0;
                        int lateMin = 0;
                        int earlyLeaveMin = 0;
                        double otHr = 0;
                        string status = "";
                        string note = "";

                        foreach (var a in g)
                        {
                            var ciS = GetProperty(a, "CheckIn")?.ToString();
                            var coS = GetProperty(a, "CheckOut")?.ToString();

                            if (DateTime.TryParse(ciS, out var ci))
                                earliestIn = (earliestIn == null || ci < earliestIn) ? ci : earliestIn;
                            
                            if (DateTime.TryParse(coS, out var co))
                                latestOut = (latestOut == null || co > latestOut) ? co : latestOut;

                            totalWorkValue += Convert.ToDouble(GetProperty(a, "WorkingHours") ?? 0);
                            lateMin = Math.Max(lateMin, Convert.ToInt32(GetProperty(a, "LateMinutes") ?? 0));
                            earlyLeaveMin = Math.Max(earlyLeaveMin, Convert.ToInt32(GetProperty(a, "EarlyLeaveMinutes") ?? 0));
                            otHr += Convert.ToDouble(GetProperty(a, "OvertimeHours") ?? 0);
                            
                            var s = GetProperty(a, "Status")?.ToString();
                            if (!string.IsNullOrEmpty(s)) status = s;

                            var n = GetProperty(a, "Note")?.ToString();
                            if (!string.IsNullOrEmpty(n)) note = string.IsNullOrEmpty(note) ? n : note + "; " + n;
                        }

                        return new {
                            Date = g.Key,
                            CheckIn = earliestIn?.ToString("HH:mm"),
                            CheckOut = latestOut?.ToString("HH:mm"),
                            WorkUnits = totalWorkValue > 0 ? totalWorkValue : 1.0,
                            LateMinutes = lateMin,
                            EarlyLeaveMinutes = earlyLeaveMin,
                            OvertimeHours = otHr,
                            Status = string.IsNullOrEmpty(status) ? "Normal" : status,
                            Note = note
                        };
                    })
                    .OrderByDescending(l => l.Date)
                    .ToList();

                var result = new {
                    EmployeeId = empId,
                    EmployeeName = employee != null ? GetProperty(employee, "FullName")?.ToString() ?? "N/A" : "Nhân viên",
                    Logs = logs
                };
                
                Console.WriteLine($"✅ Mock GET Attendance Success: Found {attendances.Count} records, returning {logs.Count} logs");

                return CreateSuccessResponse(new ApiResponse<object> { Success = true, Data = result, Message = "Success" });
            }

            if (path.Contains("/Attendance/timesheets"))
            {
                var qParams = ParseQueryString(query);
                int tsP = 1;
                var tsPage = qParams.TryGetValue("pageIndex", out var tsIdxStr) && int.TryParse(tsIdxStr, out tsP) ? tsP : 1;
                int tsPs = 10;
                var tsPageSize = qParams.TryGetValue("pageSize", out var tsSizeStr) && int.TryParse(tsSizeStr, out tsPs) ? tsPs : 10;
                
                var qMonth = qParams.TryGetValue("month", out var mStr) && int.TryParse(mStr, out var mVal) ? mVal : DateTime.Now.Month;
                var qYear = qParams.TryGetValue("year", out var yStr) && int.TryParse(yStr, out var yVal) ? yVal : DateTime.Now.Year;
                var qDept = qParams.TryGetValue("departmentId", out var dId) ? dId : null;
                var qSearch = qParams.TryGetValue("searchTerm", out var sTerm) ? sTerm.ToLower() : null;

                var allLogs = _mockDataProvider.GetCollection<dynamic>("attendances");
                var allEmployees = _mockDataProvider.GetCollection<dynamic>("employees");
                
                // 1. Filter logs by Month/Year
                var monthLogs = allLogs.Where(l => {
                    var dateStr = GetProperty(l, "Date")?.ToString() ?? "";
                    DateTime dateVal;
                    if (DateTime.TryParse(dateStr, out dateVal))
                    {
                        return dateVal.Month == qMonth && dateVal.Year == qYear;
                    }
                    return false;
                }).ToList();

                // 1. Get All Employees and filter them
                var filteredEmployees = allEmployees.Where(e => {
                    if (!string.IsNullOrEmpty(qDept) && qDept != "all") {
                        if (!MatchId(e, "DepartmentObjectId", "DepartmentId", qDept)) return false;
                    }
                    if (!string.IsNullOrEmpty(qSearch)) {
                        var name = GetProperty(e, "FullName")?.ToString() ?? "";
                        var code = GetProperty(e, "EmployeeCode")?.ToString() ?? "";
                        if (!name.Contains(qSearch, StringComparison.OrdinalIgnoreCase) && 
                            !code.Contains(qSearch, StringComparison.OrdinalIgnoreCase)) return false;
                    }
                    return true;
                }).ToList();

                // 2. Aggregate logs per employee
                var summaryList = new List<object>();
                foreach (var emp in filteredEmployees)
                {
                    var empId = GetProperty(emp, "_id")?.ToString();
                    var empName = GetProperty(emp, "FullName")?.ToString() ?? "Unknown";
                    var empCode = GetProperty(emp, "EmployeeCode")?.ToString() ?? "N/A";

                    var empLogs = monthLogs.Where(l => MatchId(l, "EmployeeId", null, empId)).ToList();
                    
                    // Calculate stats from logs
                    double actualWork = 0;
                    double otHours = 0;
                    int lateCount = 0;
                    int earlyCount = 0;

                    foreach (var log in empLogs)
                    {
                        var whVal = GetProperty(log, "WorkingHours");
                        actualWork += whVal != null ? Convert.ToDouble(whVal) : 0;
                        
                        var otVal = GetProperty(log, "OvertimeHours");
                        otHours += otVal != null ? Convert.ToDouble(otVal) : 0;
                        
                        var sidVal = GetProperty(log, "StatusId");
                        var sid = sidVal != null ? Convert.ToInt32(sidVal) : 1;
                        if (sid == 3) lateCount++; // Late
                        if (sid == 4) earlyCount++; // Early
                    }

                    summaryList.Add(new {
                        Id = empId,
                        EmployeeId = empId,
                        EmployeeCode = empCode,
                        EmployeeName = empName,
                        Avatar = GetProperty(emp, "AvatarUrl")?.ToString() ?? "",
                        Department = GetProperty(emp, "DepartmentName")?.ToString() ?? "Phòng Hành chính - Nhân sự",
                        Role = GetProperty(emp, "PositionName")?.ToString() ?? "Nhân viên",
                        StandardWork = 26.0,
                        ActualWork = actualWork / 8.0,
                        OvertimeHours = otHours,
                        LateCount = lateCount,
                        EarlyLeaveCount = earlyCount,
                        LeaveDays = 0,
                        HolidayDays = 0,
                        Status = "Đang làm việc",
                        StatusId = 1,
                        StatusColor = "success"
                    });
                }
                // (tsTotal and tsPagedItems already unique enough)

                var tsTotal = summaryList.Count;
                var tsPagedItems = summaryList.OrderBy(s => GetProperty(s, "EmployeeCode")).Skip((tsPage - 1) * tsPageSize).Take(tsPageSize).ToList();

                var tsPagedResult = new {
                    Items = tsPagedItems,
                    PageIndex = tsPage,
                    PageSize = tsPageSize,
                    TotalCount = tsTotal,
                    TotalPages = (int)Math.Ceiling(tsTotal / (double)tsPageSize)
                };
                
                return CreateSuccessResponse(new ApiResponse<object> { Success = true, Data = tsPagedResult, Message = "Success" });
            }

            if (path.Contains("/Payroll/periods/") && path.EndsWith("/detail", StringComparison.OrdinalIgnoreCase))
            {
                var tsSearch = queryParams.TryGetValue("searchTerm", out var s) ? s : "";
                var tsDeptId = queryParams.TryGetValue("departmentId", out var d) ? d : "";
                int tsPage = queryParams.TryGetValue("page", out var p1) && int.TryParse(p1, out var pageVal) ? pageVal : 1;
                int tsPageSize = queryParams.TryGetValue("pageSize", out var ps1) && int.TryParse(ps1, out var psVal) ? psVal : 10;

                // 1. Get All Employees and filter them
                var allEmployees = _mockDataProvider.GetCollection<object>("employees")
                    .Select(TransformItem).ToList();
                
                var filteredEmployees = allEmployees.Where(e => {
                    if (!string.IsNullOrEmpty(tsDeptId) && tsDeptId != "all") {
                        if (!MatchId(e, "DepartmentObjectId", "DepartmentId", tsDeptId)) return false;
                    }
                    if (!string.IsNullOrEmpty(tsSearch)) {
                        var name = GetProperty(e, "FullName")?.ToString() ?? "";
                        var code = GetProperty(e, "Code")?.ToString() ?? "";
                        if (!name.Contains(tsSearch, StringComparison.OrdinalIgnoreCase) && 
                            !code.Contains(tsSearch, StringComparison.OrdinalIgnoreCase)) return false;
                    }
                    return true;
                }).ToList();

                var payrollItems = new List<object>();
                foreach (var emp in filteredEmployees)
                {
                    var empId = GetProperty(emp, "id")?.ToString();
                    var basicSalary = Convert.ToDouble(GetProperty(emp, "BasicSalary") ?? 15000000.0);
                    
                    payrollItems.Add(new {
                        Id = Guid.NewGuid().ToString(),
                        EmployeeId = empId,
                        EmployeeCode = GetProperty(emp, "Code"),
                        EmployeeName = GetProperty(emp, "FullName"),
                        DepartmentName = GetProperty(emp, "DepartmentName"),
                        PositionName = GetProperty(emp, "PositionName"),
                        BasicSalary = basicSalary,
                        ActualWorkDays = 26.0,
                        GrossSalary = basicSalary,
                        NetSalary = basicSalary * 0.9, // Simulate tax/insurance
                        StatusName = "Draft",
                        StatusColor = "warning"
                    });
                }

                var payTotal = payrollItems.Count;
                var payPagedItems = payrollItems.Skip((tsPage - 1) * tsPageSize).Take(tsPageSize).ToList();

                var detail = new {
                    PeriodId = segments.Length >= 4 ? segments[3] : "65dae2f30000000000000999",
                    PeriodName = "Bảng lương tháng 03/2026",
                    Status = 1,
                    StatusName = "Draft",
                    TotalAmount = payrollItems.Sum(p => (double)GetProperty(p, "NetSalary")!),
                    TotalEmployees = payTotal,
                    PayrollList = new {
                        Items = payPagedItems,
                        TotalCount = payTotal,
                        PageIndex = tsPage,
                        PageSize = tsPageSize
                    }
                };

                return CreateSuccessResponse(new ApiResponse<object> { Success = true, Data = detail, Message = "Success" });
            }

            // --- WORK SCHEDULES MATCHING ALL EMPLOYEES ---
            if (path.Contains("/Attendance/work-schedules", StringComparison.OrdinalIgnoreCase))
            {
                var qDept = queryParams.TryGetValue("departmentId", out var dId) ? dId : "";
                var qSearch = queryParams.TryGetValue("searchTerm", out var sTerm) ? sTerm : "";
                var qStart = queryParams.TryGetValue("startDate", out var sDateStr) ? DateTime.Parse(sDateStr) : DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek + 1);
                var qEnd = queryParams.TryGetValue("endDate", out var eDateStr) ? DateTime.Parse(eDateStr) : qStart.AddDays(6);
                int pNum = queryParams.TryGetValue("page", out var pNumStr) && int.TryParse(pNumStr, out var pNumV) ? pNumV : 1;
                int pSize = queryParams.TryGetValue("pageSize", out var pSizeStr) && int.TryParse(pSizeStr, out var pSizeV) ? pSizeV : 1000;

                var allEmps = _mockDataProvider.GetCollection<object>("employees").Select(TransformItem).ToList();
                var filtered = allEmps.Where(e => {
                    if (!string.IsNullOrEmpty(qDept) && qDept != "all") {
                        if (!MatchId(e, "DepartmentObjectId", "DepartmentId", qDept)) return false;
                    }
                    if (!string.IsNullOrEmpty(qSearch)) {
                        var name = GetProperty(e, "FullName")?.ToString() ?? "";
                        var code = GetProperty(e, "Code")?.ToString() ?? "";
                        if (!name.Contains(qSearch, StringComparison.OrdinalIgnoreCase) && 
                            !code.Contains(qSearch, StringComparison.OrdinalIgnoreCase)) return false;
                    }
                    return true;
                }).ToList();

                var scheduleItems = new List<object>();
                var shifts = _mockDataProvider.GetCollection<object>("work_shifts").Select(TransformItem).ToList();
                var rawSchedules = _mockDataProvider.GetCollection<object>("work_schedules").Select(TransformItem).ToList();

                foreach (var emp in filtered)
                {
                    var empId = GetProperty(emp, "id")?.ToString();
                    var empSchedules = new List<object>();
                    
                    for (var date = qStart.Date; date <= qEnd.Date; date = date.AddDays(1))
                    {
                        var existing = rawSchedules.FirstOrDefault(s => 
                            MatchId(s, "EmployeeId", null, empId!) && 
                            Convert.ToDateTime(GetProperty(s, "Date") ?? date).Date == date);
                        
                        if (existing != null) {
                            empSchedules.Add(existing);
                        } else {
                            // Default to a shift based on day of week for mock variety
                            var isWeekend = date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday;
                            var shiftIndex = (Math.Abs(empId!.GetHashCode()) + date.Day) % (shifts.Count > 0 ? shifts.Count : 1);
                            var shift = shifts.Count > 0 ? shifts[shiftIndex] : null;

                            empSchedules.Add(new {
                                Date = date,
                                ShiftId = isWeekend ? "" : (GetProperty(shift, "id")?.ToString() ?? ""),
                                ShiftName = isWeekend ? "Nghỉ" : (GetProperty(shift, "Name")?.ToString() ?? "Ca HC"),
                                ShiftCode = isWeekend ? "OFF" : (GetProperty(shift, "Code")?.ToString() ?? "HC"),
                                ShiftColor = isWeekend ? "secondary" : (GetProperty(shift, "Color")?.ToString() ?? "primary"),
                                Status = isWeekend ? "Off" : "Assigned"
                            });
                        }
                    }

                    scheduleItems.Add(new {
                        EmployeeId = empId,
                        EmployeeCode = GetProperty(emp, "Code"),
                        EmployeeName = GetProperty(emp, "FullName"),
                        Department = GetProperty(emp, "DepartmentName"),
                        AvatarUrl = GetProperty(emp, "AvatarUrl"),
                        Schedules = empSchedules
                    });
                }

                var schedulePaged = scheduleItems.Skip((pNum - 1) * pSize).Take(pSize).ToList();
                return CreateSuccessResponse(new ApiResponse<object> {
                    Success = true,
                    Data = new { Items = schedulePaged, TotalCount = scheduleItems.Count, PageIndex = pNum, PageSize = pSize },
                    Message = "Success"
                });
            }

            // --- SHIFT REQUESTS (APPROVALS) MATCHING ALL EMPLOYEES ---
            if (path.Contains("/Attendance/shift-requests", StringComparison.OrdinalIgnoreCase))
            {
                var qSearchS = queryParams.TryGetValue("searchTerm", out var sTermS) ? sTermS : "";
                var qStatusS = queryParams.TryGetValue("status", out var sStatS) ? sStatS : "";
                int pNumS = queryParams.TryGetValue("page", out var pNS) && int.TryParse(pNS, out var pNV) ? pNV : 1;
                int pSizeS = queryParams.TryGetValue("pageSize", out var pSS) && int.TryParse(pSS, out var pSV) ? pSV : 10;

                var allEmps = _mockDataProvider.GetCollection<object>("employees").Select(TransformItem).ToList();
                var rawRequests = _mockDataProvider.GetCollection<object>("shift_change_requests").Select(TransformItem).ToList();

                var matchedRequests = new List<object>();
                
                // First include real mock requests
                foreach (var req in rawRequests) {
                    var emp = allEmps.FirstOrDefault(e => MatchId(e, "id", null, GetProperty(req, "EmployeeId")?.ToString() ?? ""));
                    if (emp != null) {
                        var reqClone = (JObject)JObject.FromObject(req);
                        reqClone["EmployeeName"] = JToken.FromObject(GetProperty(emp, "FullName") ?? "Unknown");
                        reqClone["EmployeeCode"] = JToken.FromObject(GetProperty(emp, "Code") ?? "N/A");
                        reqClone["DepartmentName"] = JToken.FromObject(GetProperty(emp, "DepartmentName") ?? "");
                        reqClone["AvatarUrl"] = JToken.FromObject(GetProperty(emp, "AvatarUrl") ?? "");
                        matchedRequests.Add(reqClone);
                    }
                }

                // If few requests, add simulated ones for other employees to ensure variety
                if (matchedRequests.Count < 5) {
                    var otherEmps = allEmps.Where(e => !matchedRequests.Any(r => MatchId(r, "EmployeeId", null, GetProperty(e, "id")?.ToString() ?? ""))).Take(10).ToList();
                    foreach (var emp in otherEmps) {
                        matchedRequests.Add(new {
                            Id = Guid.NewGuid().ToString(),
                            EmployeeId = GetProperty(emp, "id"),
                            EmployeeCode = GetProperty(emp, "Code"),
                            EmployeeName = GetProperty(emp, "FullName"),
                            DepartmentName = GetProperty(emp, "DepartmentName"),
                            AvatarUrl = GetProperty(emp, "AvatarUrl"),
                            RequestDate = DateTime.Today.AddDays(-1),
                            ShiftId = "HC",
                            ShiftName = "Hành chính",
                            Reason = "Lý do cá nhân",
                            Status = 1,
                            StatusName = "Chờ duyệt",
                            StatusColor = "warning"
                        });
                    }
                }

                // Apply search
                if (!string.IsNullOrEmpty(qSearch)) {
                    matchedRequests = matchedRequests.Where(r => 
                        (GetProperty(r, "EmployeeName")?.ToString() ?? "").Contains(qSearch, StringComparison.OrdinalIgnoreCase) ||
                        (GetProperty(r, "EmployeeCode")?.ToString() ?? "").Contains(qSearch, StringComparison.OrdinalIgnoreCase)
                    ).ToList();
                }

                var shiftTotal = matchedRequests.Count;
                var shiftPaged = matchedRequests.Skip((pNumS - 1) * pSizeS).Take(pSizeS).ToList();

                // Special case for summary endpoint
                if (path.EndsWith("/summary", StringComparison.OrdinalIgnoreCase)) {
                    return CreateSuccessResponse(new ApiResponse<object> {
                        Success = true,
                        Data = new {
                            Total = shiftTotal,
                            Pending = matchedRequests.Count(r => Convert.ToInt32(GetProperty(r, "Status") ?? 0) == 1),
                            Approved = matchedRequests.Count(r => Convert.ToInt32(GetProperty(r, "Status") ?? 0) == 2),
                            Rejected = matchedRequests.Count(r => Convert.ToInt32(GetProperty(r, "Status") ?? 0) == 3)
                        },
                        Message = "Success"
                    });
                }

                return CreateSuccessResponse(new ApiResponse<object> {
                    Success = true,
                    Data = new { Items = shiftPaged, TotalCount = shiftTotal, PageIndex = pNumS, PageSize = pSizeS },
                    Message = "Success"
                });
            }

            if (path.Contains("/Leave/balance/", StringComparison.OrdinalIgnoreCase))
            {
                var empId = segments.Length >= 4 ? segments[3].Split('?')[0] : "65bf0c300000000000000001";
                var balanceRaw = _mockDataProvider.GetCollection<object>("employee_leave_balances")
                    .FirstOrDefault(b => MatchId(b, "EmployeeId", null, empId))
                    ?? new { EmployeeId = empId, Year = 2026, TotalEntitled = 15, Used = 2 };
                
                // Map to LeaveBalanceModel
                var balance = new {
                    EmployeeId = empId,
                    Year = Convert.ToInt32(GetProperty(balanceRaw, "Year") ?? 2026),
                    EntitledDays = Convert.ToDouble(GetProperty(balanceRaw, "TotalEntitled") ?? 15.0),
                    UsedDays = Convert.ToDouble(GetProperty(balanceRaw, "Used") ?? 0.0),
                    RemainingDays = Convert.ToDouble(GetProperty(balanceRaw, "TotalEntitled") ?? 15.0) - Convert.ToDouble(GetProperty(balanceRaw, "Used") ?? 0.0),
                    PendingDays = 1.0
                };
                
                return CreateSuccessResponse(new ApiResponse<object> { Success = true, Data = balance, Message = "Success" });
            }

            if (path.Contains("/Leave") && !path.EndsWith("/types") && !path.Contains("/balance"))
            {
                var leaves = _mockDataProvider.GetCollection<object>("leave_requests")
                    .Where(l => MatchId(l, "EmployeeId", null, "65bf0c300000000000000001"))
                    .Select(TransformItem)
                    .ToList();
                
                var paged = new PagedResult<object> { Items = leaves, TotalCount = leaves.Count, PageIndex = 1, PageSize = 10 };
                return CreateSuccessResponse(new ApiResponse<object> { Success = true, Data = paged, Message = "Success" });
            }

            // Document / Digital Profile sub-endpoints
            if (path.Contains("/Employees/") && path.EndsWith("/documents", StringComparison.OrdinalIgnoreCase))
            {
                var docList = new List<object>
                {
                    new { Id = "doc1", DocumentName = "Bằng đại học.pdf", DocumentTypeName = "Bằng cấp", FileUrl = "/docs/sample-degree.pdf", CreatedAt = DateTime.Now.AddMonths(-12), StatusName = "Approved", StatusColor = "success" },
                    new { Id = "doc2", DocumentName = "Hợp đồng lao động.pdf", DocumentTypeName = "Hợp đồng", FileUrl = "/docs/labor-contract.pdf", CreatedAt = DateTime.Now.AddMonths(-11), StatusName = "Approved", StatusColor = "success" },
                    new { Id = "doc3", DocumentName = "CCCD Mặt trước.jpg", DocumentTypeName = "Cá nhân", FileUrl = "/docs/id-card-front.jpg", CreatedAt = DateTime.Now.AddMonths(-24), StatusName = "Approved", StatusColor = "success" }
                };
                return CreateSuccessResponse(new ApiResponse<List<object>> { Success = true, Data = docList, Message = "Success" });
            }

            if (path.Contains("/Employees/") && path.EndsWith("/digital-profile", StringComparison.OrdinalIgnoreCase))
            {
                var documents = new List<object>
                {
                    new { Id = "doc1", DocumentName = "Bằng đại học.pdf", DocumentTypeName = "Bằng cấp", FileUrl = "/docs/sample-degree.pdf", CreatedAt = DateTime.Now.AddMonths(-12), StatusName = "Đã duyệt", StatusColor = "success", FileSize = 1048576, DocumentType = "Degree", Status = "Approved" },
                    new { Id = "doc2", DocumentName = "Hợp đồng lao động.pdf", DocumentTypeName = "Hợp đồng", FileUrl = "/docs/labor-contract.pdf", CreatedAt = DateTime.Now.AddMonths(-11), StatusName = "Đã duyệt", StatusColor = "success", FileSize = 2097152, DocumentType = "Contract", Status = "Approved" },
                    new { Id = "doc3", DocumentName = "CCCD mặt trước.jpg", DocumentTypeName = "Giấy tờ tùy thân", FileUrl = "/docs/id-card-front.jpg", CreatedAt = DateTime.Now.AddMonths(-12), StatusName = "Đã duyệt", StatusColor = "success", FileSize = 524288, DocumentType = "IDCard", Status = "Approved" }
                };

                return CreateSuccessResponse(new ApiResponse<object> 
                { 
                    Success = true, 
                    Data = new 
                    { 
                        EmployeeId = "65bf0c300000000000000001",
                        CompletionPercentage = 85.5,
                        Documents = documents
                    }, 
                    Message = "Success" 
                });
            }

            if (endpoint.Equals("core/system/settings", StringComparison.OrdinalIgnoreCase))
            {
                var settingsList = GetAllItems("system_settings");
                var settings = settingsList.FirstOrDefault();
                if (settings != null)
                {
                    var jObj = settings is JObject jo ? (JObject)jo.DeepClone() : JObject.FromObject(settings);
                    
                    // 1. Build Hierarchical SidebarMenu
                    var allNavItemsRaw = GetAllItems("navigation_items");
                    var allNavItems = allNavItemsRaw.Select(n => n is JObject jn ? (JObject)jn.DeepClone() : JObject.FromObject(n)).ToList();
                    
                    // Root items: ParentId is null or 0
                    var rootItems = allNavItems.Where(n => 
                        n["ParentId"] == null || 
                        n["ParentId"].Type == JTokenType.Null || 
                        n["ParentId"].ToString() == "0"
                    ).OrderBy(n => n["Order"]?.Value<int>() ?? 0).ToList();

                    foreach (var root in rootItems)
                    {
                        var rid = root["NumericId"]?.ToString();
                        if (!string.IsNullOrEmpty(rid))
                        {
                            var children = allNavItems.Where(n => n["ParentId"]?.ToString() == rid)
                                .OrderBy(n => n["Order"]?.Value<int>() ?? 0).ToList();
                            root["SubItems"] = JArray.FromObject(children);
                        }
                    }
                    jObj["SidebarMenu"] = JArray.FromObject(rootItems);

                    // 2. Enrich with Translations
                    var translations = GetAllItems("language_translations");
                    jObj["Translations"] = JArray.FromObject(translations);

                    // 3. Enrich with Holidays
                    var holidaysList = GetAllItems("holidays");
                    jObj["Holidays"] = JArray.FromObject(holidaysList);

                    // 4. Enrich missing Company Info if empty
                    if (string.IsNullOrEmpty(jObj["CompanyAddress"]?.ToString()))
                        jObj["CompanyAddress"] = "Tòa nhà Bitexco, Q.1, TP. Hồ Chí Minh";
                    if (string.IsNullOrEmpty(jObj["CompanyPhone"]?.ToString()))
                        jObj["CompanyPhone"] = "028.3821.8888";
                    if (string.IsNullOrEmpty(jObj["CompanyEmail"]?.ToString()))
                        jObj["CompanyEmail"] = "contact@tantanloc.com";

                    // 5. Enrich with PitSteps 
                    if (jObj["PitSteps"] == null || !jObj["PitSteps"].Any())
                    {
                        var pitSteps = new List<object>
                        {
                            new { Threshold = 0, Rate = 0.05, Deduction = 0 },
                            new { Threshold = 5000000, Rate = 0.1, Deduction = 250000 },
                            new { Threshold = 10000000, Rate = 0.15, Deduction = 750000 },
                            new { Threshold = 18000000, Rate = 0.15, Deduction = 1650000 },
                            new { Threshold = 32000000, Rate = 0.25, Deduction = 3250000 },
                            new { Threshold = 52000000, Rate = 0.3, Deduction = 5850000 },
                            new { Threshold = 80000000, Rate = 0.35, Deduction = 9850000 }
                        };
                        jObj["PitSteps"] = JArray.FromObject(pitSteps);
                    }

                    return CreateSuccessResponse(new ApiResponse<object> 
                    { 
                        Success = true, 
                        Data = TransformItem(jObj), 
                        Message = "Success" 
                    });
                }
            }

            if (endpoint.Equals("core/system/settings/code-generator", StringComparison.OrdinalIgnoreCase))
            {
                var configs = GetAllItems("code_generator_configs");
                return CreateSuccessResponse(new ApiResponse<List<object>> 
                { 
                    Success = true, 
                    Data = configs.Select(TransformItem).ToList(), 
                    Message = "Success" 
                });
            }

            if (path.EndsWith("/me", StringComparison.OrdinalIgnoreCase) || path.EndsWith("/my", StringComparison.OrdinalIgnoreCase))
            {
                var meCollection = GetCollectionName(endpoint);
                if (!string.IsNullOrEmpty(meCollection))
                {
                    // For employees, we want " GD-001" (Phạm Minh Hùng)
                    if (meCollection == "employees")
                    {
                        var meItem = _mockDataProvider.GetById<object>(meCollection, "65bf0c300000000000000001");
                        if (meItem != null) return CreateSuccessResponse(new ApiResponse<object> { Success = true, Data = TransformItem(meItem), Message = "Success" });
                    }

                    var firstItem = _mockDataProvider.GetFirst<object>(meCollection);
                    if (firstItem != null)
                    {
                        return CreateSuccessResponse(new ApiResponse<object> { Success = true, Data = TransformItem(firstItem), Message = "Success" });
                    }
                }
            }

            var collectionName = GetCollectionName(endpoint);

            if (string.IsNullOrEmpty(collectionName))
            {
                return CreateErrorResponse($"Không tìm thấy collection cho endpoint: {endpoint}");
            }

            // Generic Summary handler for any collection ending in /summary
            if (path.EndsWith("/summary", StringComparison.OrdinalIgnoreCase))
            {
                var summaryAllData = GetAllItems(collectionName);
                int sPending = 0, sApproved = 0, sRejected = 0, sWithdrawn = 0, sTotal = summaryAllData.Count;

                foreach (var item in summaryAllData)
                {
                    var status = GetProperty(item, "Status")?.ToString() ?? "Pending";
                    if (status == "Pending" || status == "1") sPending++;
                    else if (status == "Approved" || status == "2") sApproved++;
                    else if (status == "Rejected" || status == "3") sRejected++;
                    else if (status == "Withdrawn" || status == "4" || status == "Cancelled") sWithdrawn++;
                }

                // Create a generic summary object that fits common summary models
                var summaryObj = new
                {
                    PendingCount = sPending,
                    ApprovedCount = sApproved,
                    RejectedCount = sRejected,
                    WithdrawnCount = sWithdrawn,
                    CancelledCount = sWithdrawn, // Map to same for compatibility
                    TotalCount = sTotal
                };

                return CreateSuccessResponse(new ApiResponse<object> { Success = true, Data = summaryObj, Message = "Success" });
            }

            // Check for ID in path (e.g., /Employees/{id})
            var lastSegment = segments.LastOrDefault();
            var isDetailRequest = lastSegment != null && !lastSegment.Contains('?') && segments.Length > 2;

        if (isDetailRequest && (IsObjectId(lastSegment) || lastSegment.Length > 20))
        {
            // Single item request
            var item = GetItemById(collectionName, lastSegment);
            if (item == null)
            {
                return CreateNotFoundResponse($"Item với ID {lastSegment} không tồn tại");
            }

            return CreateSuccessResponse(new ApiResponse<object>
            {
                Success = true,
                Data = TransformItem(item),
                Message = "Success"
            });
        }

        // List request with filtration and pagination
        var items = GetAllItems(collectionName);

        // Calculate EmployeeCount for departments dynamically
        if (collectionName == "departments")
        {
            var employees = GetAllItems("employees");
            var enrichedItems = new List<object>();
            foreach (var item in items)
            {
                var jObj = item is JObject jo ? (JObject)jo.DeepClone() : JObject.FromObject(item);
                var dId = jObj["_id"]?.ToString() ?? jObj["id"]?.ToString();
                if (!string.IsNullOrEmpty(dId))
                {
                    jObj["EmployeeCount"] = employees.Count(e => MatchId(e, "DepartmentObjectId", "DepartmentId", dId));
                }
                enrichedItems.Add(jObj);
            }
            items = enrichedItems;
        }

        // (Already centralized above)
        int pM = 1;
        var page = queryParams.TryGetValue("pageIndex", out var pageStr) && int.TryParse(pageStr, out pM) ? pM : 
                   queryParams.TryGetValue("page", out var p2Str) && int.TryParse(p2Str, out var p3M) ? p3M : 1;
        int psM = 10;
        var pageSize = queryParams.TryGetValue("pageSize", out var pageSizeStr) && int.TryParse(pageSizeStr, out psM) ? psM : 10;

        // Filter by common query parameters
        if (queryParams.TryGetValue("type", out var typeValue))
            items = items.Where(i => GetProperty(i, "Type")?.ToString() == typeValue).ToList();
            
        if (queryParams.TryGetValue("departmentId", out var deptId))
            items = items.Where(i => MatchId(i, "DepartmentObjectId", "DepartmentId", deptId)).ToList();

        if (queryParams.TryGetValue("searchTerm", out var st))
            items = FilterItemsBySearchTerm(items, st);

        // Apply pagination only if requested
        var hasPagination = queryParams.ContainsKey("pageIndex") || queryParams.ContainsKey("page") || queryParams.ContainsKey("pageSize");
        
        if (!hasPagination)
        {
            var listData = items.Select(TransformItem).ToList();
            return CreateSuccessResponse(new ApiResponse<List<object>>
            {
                Success = true,
                Data = listData,
                Message = "Success"
            });
        }

        // Apply pagination
        var total = items.Count;
        var pagedItems = items.Skip((page - 1) * pageSize).Take(pageSize).Select(TransformItem).ToList();

        var pagedResult = new PagedResult<object>
        {
            Items = pagedItems,
            PageIndex = page,
            PageSize = pageSize,
            TotalCount = total
        };

        return CreateSuccessResponse(new ApiResponse<PagedResult<object>>
        {
            Success = true,
            Data = pagedResult,
            Message = "Success"
        });
    }

    private object? GetProperty(object? item, string propertyName)
    {
        if (item == null) return null;
        
        JToken? token = null;
        if (item is JObject jObj)
        {
            token = jObj[propertyName];
        }
        else if (item is JToken jt)
        {
            token = jt[propertyName];
        }
        else
        {
            try {
                var jo = JObject.FromObject(item);
                token = jo[propertyName];
            } catch { return null; }
        }
        
        if (token == null || token.Type == JTokenType.Null) return null;
        
        // If it's a simple value, return the underlying C# value
        if (token is JValue jv) return jv.Value;
        
        return token;
    }

    private bool MatchId(object item, string objectIdField, string numericIdField, string valueToMatch)
    {
        if (string.IsNullOrEmpty(valueToMatch)) return false;
        
        var jObj = item is JObject jo ? jo : JObject.FromObject(item);
        
        // Fields to check in order of priority
        string[] idFields = { objectIdField, "_id", "id", numericIdField, "Id", "EmployeeId", "employeeId" };
        
        foreach (var field in idFields)
        {
            if (string.IsNullOrEmpty(field)) continue;
            if (jObj.TryGetValue(field, out var val) && val != null)
            {
                var strVal = val.ToString();
                
                // Direct match
                if (strVal.Equals(valueToMatch, StringComparison.OrdinalIgnoreCase)) return true;
                
                // MongoDB $oid match
                if (val is JObject valObj && valObj.TryGetValue("$oid", out var oidValue))
                {
                    if (oidValue.ToString().Equals(valueToMatch, StringComparison.OrdinalIgnoreCase)) return true;
                }
            }
        }
        
        return false;
    }

    private async Task<HttpResponseMessage> HandlePostPut(string requestUri, HttpRequestMessage request, string successMessage)
    {
        Console.WriteLine($"   ⚠️  Chế độ mock - dữ liệu không được lưu thực sự");

        object? value = null;
        if (request.Content != null)
        {
            try
            {
                var contentString = await request.Content.ReadAsStringAsync();
                value = JsonConvert.DeserializeObject(contentString);
            }
            catch { }
        }

        return CreateSuccessResponse(new ApiResponse<object>
        {
            Success = true,
            Data = value ?? new object(),
            Message = successMessage
        });
    }

    private string ExtractEndpoint(string path)
    {
        // 1. Remove hostname and query string
        string absolutePath = path;
        if (Uri.TryCreate(path, UriKind.Absolute, out var uri))
        {
            absolutePath = uri.AbsolutePath;
        }
        else
        {
            absolutePath = path.Split('?')[0];
        }

        absolutePath = absolutePath.TrimStart('/');

        // 2. Identify the service prefix (core, auth, etc.)
        var segments = absolutePath.Split('/', StringSplitOptions.RemoveEmptyEntries);
        int startIndex = -1;
        for (int i = 0; i < segments.Length; i++)
        {
            if (segments[i].Equals("core", StringComparison.OrdinalIgnoreCase) || 
                segments[i].Equals("auth", StringComparison.OrdinalIgnoreCase) ||
                segments[i].Contains("gateway", StringComparison.OrdinalIgnoreCase))
            {
                startIndex = i;
                break;
            }
        }

        if (startIndex != -1)
        {
            // Return full path after prefix (including prefix if it's core/auth)
            // But if it's 'gateway', skip it
            if (segments[startIndex].Contains("gateway", StringComparison.OrdinalIgnoreCase))
                return string.Join("/", segments.Skip(startIndex + 1));
            
            return string.Join("/", segments.Skip(startIndex));
        }

        return absolutePath;
    }

    private string? GetCollectionName(string endpoint)
    {
        // Try exact match
        if (_endpointToCollectionMap.TryGetValue(endpoint, out var collection)) 
            return collection;

        // Try matching prefix for detail endpoints
        foreach (var entry in _endpointToCollectionMap.OrderByDescending(e => e.Key.Length))
        {
            if (endpoint.StartsWith(entry.Key, StringComparison.OrdinalIgnoreCase))
            {
                return entry.Value;
            }
        }

        return null;
    }

    private object TransformItem(object item)
    {
        if (item == null) return new object();

        // Convert to JObject for manipulation
        var jObj = item is JObject jo ? (JObject)jo.DeepClone() : JObject.FromObject(item);
        
        // Map _id to id
        if (jObj.TryGetValue("_id", out var idToken))
        {
            jObj["id"] = idToken;
            jObj.Remove("_id");
        }

        // Clean up nested MongoDB formats recursively
        CleanJToken(jObj);

        // Map StatusId to Status, StatusName, StatusColor if they are missing or null/empty
        if (jObj.TryGetValue("StatusId", out var sidToken))
        {
            var sid = sidToken.ToString();
            
            if (!jObj.TryGetValue("Status", out var sToken) || sToken.Type == JTokenType.Null || string.IsNullOrEmpty(sToken.ToString()))
            {
                jObj["Status"] = sid switch
                {
                    "1" => "Pending",
                    "2" => "Approved",
                    "3" => "Rejected",
                    "4" => "Closed",
                    "7" => "Draft",
                    "8" => "Withdrawn",
                    _ => jObj["Status"]
                };
            }

            if (!jObj.TryGetValue("StatusName", out var snToken) || snToken.Type == JTokenType.Null || string.IsNullOrEmpty(snToken.ToString()))
            {
                jObj["StatusName"] = sid switch
                {
                    "1" => "Chờ duyệt",
                    "2" => "Đã duyệt",
                    "3" => "Từ chối",
                    "4" => "Đã chốt",
                    "7" => "Dự thảo",
                    "8" => "Đã rút",
                    _ => jObj["StatusName"]
                };
            }

            if (!jObj.TryGetValue("StatusColor", out var scToken) || scToken.Type == JTokenType.Null || string.IsNullOrEmpty(scToken.ToString()))
            {
                jObj["StatusColor"] = sid switch
                {
                    "1" => "warning",
                    "2" => "success",
                    "3" => "danger",
                    "4" => "primary",
                    "7" => "info",
                    "8" => "secondary",
                    _ => jObj["StatusColor"]
                };
            }
        }

        // Specific mapping for Payroll
        if (jObj.ContainsKey("BasicSalary") && jObj.ContainsKey("Month") && jObj.ContainsKey("Year"))
        {
            decimal basic = jObj["BasicSalary"]?.Value<decimal>() ?? 0;
            decimal allowance = jObj["Allowance"]?.Value<decimal>() ?? 0;
            decimal bonus = jObj["Bonus"]?.Value<decimal>() ?? 0;
            decimal bhxh = jObj["BhxhAmount"]?.Value<decimal>() ?? 0;
            decimal bhyt = jObj["BhytAmount"]?.Value<decimal>() ?? 0;
            decimal bhtn = jObj["BhtnAmount"]?.Value<decimal>() ?? 0;
            decimal union = jObj["UnionFee"]?.Value<decimal>() ?? 0;
            decimal tax = jObj["TaxAmount"]?.Value<decimal>() ?? 0;
            decimal advance = jObj["AdvanceAmount"]?.Value<decimal>() ?? 0;
            decimal deduction = jObj["Deduction"]?.Value<decimal>() ?? 0;

            decimal totalDeduction = bhxh + bhyt + bhtn + union + tax + advance + deduction;
            decimal netSalary = basic + allowance + bonus - totalDeduction;

            if (!jObj.ContainsKey("TotalDeduction")) jObj["TotalDeduction"] = totalDeduction;
            if (!jObj.ContainsKey("NetSalary")) jObj["NetSalary"] = netSalary;
            if (!jObj.ContainsKey("StatusName") && jObj.ContainsKey("Status")) jObj["StatusName"] = jObj["Status"];
        }

        return jObj;
    }

    private void CleanJToken(JToken token)
    {
        if (token is JObject obj)
        {
            foreach (var property in obj.Properties().ToList())
            {
                var val = property.Value;
                if (val is JObject valObj)
                {
                    if (valObj.TryGetValue("$oid", out var oid))
                        property.Value = oid;
                    else if (valObj.TryGetValue("$date", out var date))
                        property.Value = date;
                    else if (valObj.TryGetValue("$numberDecimal", out var decimalVal))
                    {
                        if (decimal.TryParse(decimalVal.ToString(), System.Globalization.CultureInfo.InvariantCulture, out var d))
                            property.Value = d;
                        else if (double.TryParse(decimalVal.ToString(), System.Globalization.CultureInfo.InvariantCulture, out var db))
                            property.Value = db;
                        else
                            property.Value = decimalVal;
                    }
                    else
                        CleanJToken(valObj);
                }
                else if (val is JArray arr)
                {
                    CleanJToken(arr);
                }
            }
        }
        else if (token is JArray arr)
        {
            foreach (var subItem in arr)
            {
                CleanJToken(subItem);
            }
        }
    }

    private List<object> GetAllItems(string collectionName)
    {
        return _mockDataProvider.GetCollection<object>(collectionName);
    }

    private object? GetItemById(string collectionName, string id)
    {
        id = id?.Trim();
        var items = GetAllItems(collectionName);
        Console.WriteLine($"   🔎 GetItemById in {collectionName}: searching for ID {id} among {items.Count} items");
        var found = items.FirstOrDefault(i => MatchId(i, "_id", "id", id));
        if (found == null) Console.WriteLine($"   ⚠️ GetItemById: Item NOT FOUND in {collectionName} with ID {id}");
        return found;
    }

    private List<object> FilterItemsBySearchTerm(List<object> items, string searchTerm)
    {
        // Simple search implementation - search in JSON string representation
        return items.Where(item =>
        {
            var json = JsonConvert.SerializeObject(item);
            return json.Contains(searchTerm, StringComparison.OrdinalIgnoreCase);
        }).ToList();
    }

    private bool IsObjectId(string value)
    {
        // MongoDB ObjectId is 24 characters hex string
        return value.Length == 24 && value.All(c => "0123456789abcdefABCDEF".Contains(c));
    }

    private HttpResponseMessage CreateSuccessResponse<T>(T data)
    {
        var json = JsonConvert.SerializeObject(data, _jsonSettings);

        return new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(json, Encoding.UTF8, "application/json")
        };
    }

    private HttpResponseMessage CreateErrorResponse(string message)
    {
        var errorResponse = new ApiResponse<object>
        {
            Success = false,
            Message = message
        };

        var json = JsonConvert.SerializeObject(errorResponse, _jsonSettings);

        return new HttpResponseMessage(HttpStatusCode.BadRequest)
        {
            Content = new StringContent(json, Encoding.UTF8, "application/json")
        };
    }

    private HttpResponseMessage CreateNotFoundResponse(string message)
    {
        var errorResponse = new ApiResponse<object>
        {
            Success = false,
            Message = message
        };

        var json = JsonConvert.SerializeObject(errorResponse, _jsonSettings);

        return new HttpResponseMessage(HttpStatusCode.NotFound)
        {
            Content = new StringContent(json, Encoding.UTF8, "application/json")
        };
    }

    /// <summary>
    /// Parse query string thành Dictionary (thay thế System.Web.HttpUtility)
    /// </summary>
    private static Dictionary<string, string> ParseQueryString(string query)
    {
        var result = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        if (string.IsNullOrEmpty(query))
            return result;

        // Remove leading '?' if present
        query = query.TrimStart('?');

        if (string.IsNullOrEmpty(query))
            return result;

        // Split by '&' to get key-value pairs
        var pairs = query.Split('&', StringSplitOptions.RemoveEmptyEntries);

        foreach (var pair in pairs)
        {
            if (string.IsNullOrWhiteSpace(pair))
                continue;

            var parts = pair.Split('=', 2);

            if (parts.Length >= 1)
            {
                var key = Uri.UnescapeDataString(parts[0]);
                var value = parts.Length == 2 ? Uri.UnescapeDataString(parts[1]) : string.Empty;
                result[key] = value;
            }
        }

        return result;
    }
}
