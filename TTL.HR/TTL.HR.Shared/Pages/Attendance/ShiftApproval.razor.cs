using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Components;

namespace TTL.HR.Shared.Pages.Attendance
{
    public partial class ShiftApproval
    {
        private bool _showDetail = false;
        private bool _isLoading = true;
        private ShiftRequest? _selectedRequest;
        private int activeTab = 0;

        protected override async System.Threading.Tasks.Task OnInitializedAsync()
        {
            // Simulate API loading
            await System.Threading.Tasks.Task.Delay(1200);
            _isLoading = false;
        }

        private void SetTab(int index)
        {
            activeTab = index;
        }

        private IEnumerable<ShiftRequest> FilteredRequests
        {
            get
            {
                return activeTab switch
                {
                    0 => _requests.Where(x => x.IsPending),
                    1 => _requests.Where(x => x.Status == "Đã duyệt"),
                    2 => _requests.Where(x => x.Status == "Từ chối"),
                    _ => _requests
                };
            }
        }

        private bool _showActionModal = false;
        private string _modalTitle = "";
        private string _modalHeading = "";
        private string _modalMessage = "";
        private string _modalColor = "primary";
        private string _modalIcon = "";
        private string _modalConfirmLabel = "";
        private bool _showReasonInput = false;
        private string _actionType = ""; // "APPROVE" or "REJECT"
        private ShiftRequest? _requestToProcess;

        private void OpenDetail(ShiftRequest req)
        {
            _selectedRequest = req;
            _showDetail = true;
        }

        private void CloseDetail()
        {
            _showDetail = false;
            _selectedRequest = null;
        }

        private void ApproveRequest(ShiftRequest req)
        {
            _requestToProcess = req;
            _actionType = "APPROVE";
            _modalTitle = "Phê duyệt yêu cầu";
            _modalHeading = "Xác nhận phê duyệt?";
            _modalMessage = $"Bạn có chắc chắn muốn phê duyệt yêu cầu đổi ca cho nhân viên {req.Name} ({req.EmployeeId})?";
            _modalColor = "success";
            _modalIcon = "ki-outline ki-check-circle";
            _modalConfirmLabel = "Đồng ý Duyệt";
            _showReasonInput = false;
            _showActionModal = true;
        }

        private void RejectRequest(ShiftRequest req)
        {
            _requestToProcess = req;
            _actionType = "REJECT";
            _modalTitle = "Từ chối yêu cầu";
            _modalHeading = "Xác nhận từ chối?";
            _modalMessage = $"Hành động này sẽ từ chối yêu cầu đổi sang {req.TargetShift} của {req.Name}. Vui lòng nhập lý do bên dưới.";
            _modalColor = "danger";
            _modalIcon = "ki-outline ki-cross-circle";
            _modalConfirmLabel = "Xác nhận Từ chối";
            _showReasonInput = true;
            _showActionModal = true;
        }

        private void CloseModal()
        {
            _showActionModal = false;
            _requestToProcess = null;
        }

        private void ConfirmAction(string reason)
        {
            if (_requestToProcess == null) return;

            if (_actionType == "APPROVE")
            {
                _requestToProcess.Status = "Đã duyệt";
                _requestToProcess.StatusClass = "badge-light-success";
                _requestToProcess.IsPending = false;
            }
            else if (_actionType == "REJECT")
            {
                _requestToProcess.Status = "Từ chối";
                _requestToProcess.StatusClass = "badge-light-danger";
                _requestToProcess.IsPending = false;
                _requestToProcess.ManagerNote = reason; // Store the reason
            }

            if (_selectedRequest == _requestToProcess)
            {
                CloseDetail();
            }
            
            CloseModal();
        }

        private class ShiftRequest
        {
            public int Id { get; set; }
            public string Name { get; set; } = "";
            public string EmployeeId { get; set; } = "";
            public string Department { get; set; } = "";
            public string Avatar { get; set; } = "";
            public string AvatarColor { get; set; } = ""; // For initial avatars
            public string CurrentShift { get; set; } = "";
            public string CurrentShiftClass { get; set; } = "";
            public string TargetShift { get; set; } = "";
            public string TargetShiftClass { get; set; } = "";
            public string Reason { get; set; } = "";
            public string ApplyDate { get; set; } = "";
            public string Status { get; set; } = "";
            public string StatusClass { get; set; } = "";
            public bool IsPending { get; set; } = false;
            public string? ManagerNote { get; set; }
        }

        private List<ShiftRequest> _requests = new()
        {
            new ShiftRequest { Id = 1, Name = "Nguyễn Văn An", EmployeeId = "NV-0284", Department = "Phòng KT", Avatar = "assets/media/avatars/300-1.jpg", CurrentShift = "CA SÁNG", CurrentShiftClass = "badge-light-info", TargetShift = "CA ĐÊM", TargetShiftClass = "badge-light-dark", Reason = "Việc gia đình đột xuất, cần trông con nhỏ.", ApplyDate = "12/10/2023", Status = "Chờ duyệt", StatusClass = "badge-light-warning", IsPending = true },
            new ShiftRequest { Id = 2, Name = "Trần Thị Bích", EmployeeId = "NV-0285", Department = "Phòng KD", Avatar = "assets/media/avatars/300-2.jpg", CurrentShift = "CA CHIỀU", CurrentShiftClass = "badge-light-warning", TargetShift = "CA SÁNG", TargetShiftClass = "badge-light-info", Reason = "Hẹn gặp khách hàng quan trọng vào buổi sáng.", ApplyDate = "12/10/2023", Status = "Đã duyệt", StatusClass = "badge-light-success", IsPending = false },
            new ShiftRequest { Id = 3, Name = "Lê Văn Cường", EmployeeId = "NV-0286", Department = "Phòng NS", Avatar = "", AvatarColor = "bg-light-danger text-danger", CurrentShift = "CA SÁNG", CurrentShiftClass = "badge-light-info", TargetShift = "CA CHIỀU", TargetShiftClass = "badge-light-warning", Reason = "Đi khám bệnh định kỳ tại bệnh viện.", ApplyDate = "13/10/2023", Status = "Từ chối", StatusClass = "badge-light-danger", IsPending = false },
            new ShiftRequest { Id = 4, Name = "Phạm Thị Dung", EmployeeId = "NV-0287", Department = "Phòng KT", Avatar = "assets/media/avatars/300-3.jpg", CurrentShift = "CA ĐÊM", CurrentShiftClass = "badge-light-dark", TargetShift = "CA SÁNG", TargetShiftClass = "badge-light-info", Reason = "Sức khỏe không đảm bảo làm ca đêm.", ApplyDate = "13/10/2023", Status = "Chờ duyệt", StatusClass = "badge-light-warning", IsPending = true },
            new ShiftRequest { Id = 5, Name = "Hoàng Văn Em", EmployeeId = "NV-0288", Department = "Kho Vận", Avatar = "", AvatarColor = "bg-light-success text-success", CurrentShift = "CA CHIỀU", CurrentShiftClass = "badge-light-warning", TargetShift = "CA ĐÊM", TargetShiftClass = "badge-light-dark", Reason = "Đổi ca hỗ trợ đồng nghiệp bị ốm.", ApplyDate = "14/10/2023", Status = "Chờ duyệt", StatusClass = "badge-light-warning", IsPending = true },
            new ShiftRequest { Id = 6, Name = "Võ Thị F", EmployeeId = "NV-0289", Department = "Hành Chính", Avatar = "assets/media/avatars/300-5.jpg", CurrentShift = "CA SÁNG", CurrentShiftClass = "badge-light-info", TargetShift = "OFF", TargetShiftClass = "badge-light-secondary", Reason = "Nghỉ bù cho ngày làm việc cuối tuần trước.", ApplyDate = "14/10/2023", Status = "Đã duyệt", StatusClass = "badge-light-success", IsPending = false },
            new ShiftRequest { Id = 7, Name = "Đặng Văn G", EmployeeId = "NV-0290", Department = "Kế Toán", Avatar = "", AvatarColor = "bg-light-primary text-primary", CurrentShift = "CA HC", CurrentShiftClass = "badge-light-primary", TargetShift = "REMOTE", TargetShiftClass = "badge-light-purple", Reason = "Xe hỏng, xin phép làm việc tại nhà.", ApplyDate = "15/10/2023", Status = "Chờ duyệt", StatusClass = "badge-light-warning", IsPending = true },
            new ShiftRequest { Id = 8, Name = "Bùi Thị Hạnh", EmployeeId = "NV-0291", Department = "Phòng KD", Avatar = "assets/media/avatars/300-6.jpg", CurrentShift = "CA SÁNG", CurrentShiftClass = "badge-light-info", TargetShift = "CA CHIỀU", TargetShiftClass = "badge-light-warning", Reason = "Đưa con đi tiêm phòng.", ApplyDate = "15/10/2023", Status = "Từ chối", StatusClass = "badge-light-danger", IsPending = false },
            new ShiftRequest { Id = 9, Name = "Lý Văn I", EmployeeId = "NV-0292", Department = "Phòng KT", Avatar = "", AvatarColor = "bg-light-info text-info", CurrentShift = "CA ĐÊM", CurrentShiftClass = "badge-light-dark", TargetShift = "OFF", TargetShiftClass = "badge-light-secondary", Reason = "Nghỉ sau đợt tăng ca dự án gấp.", ApplyDate = "16/10/2023", Status = "Đã duyệt", StatusClass = "badge-light-success", IsPending = false },
            new ShiftRequest { Id = 10, Name = "Trương Văn K", EmployeeId = "NV-0293", Department = "Ban GĐ", Avatar = "assets/media/avatars/300-9.jpg", CurrentShift = "CA HC", CurrentShiftClass = "badge-light-primary", TargetShift = "CÔNG TÁC", TargetShiftClass = "badge-light-success", Reason = "Đi công tác gặp đối tác tại HCM.", ApplyDate = "16/10/2023", Status = "Chờ duyệt", StatusClass = "badge-light-warning", IsPending = true },
            new ShiftRequest { Id = 11, Name = "Đoàn Văn Lam", EmployeeId = "NV-0294", Department = "Phòng KT", Avatar = "assets/media/avatars/300-10.jpg", CurrentShift = "CA SÁNG", CurrentShiftClass = "badge-light-info", TargetShift = "CA GÃY", TargetShiftClass = "badge-light-primary", Reason = "Tham gia khóa đào tạo nội bộ buổi trưa.", ApplyDate = "17/10/2023", Status = "Chờ duyệt", StatusClass = "badge-light-warning", IsPending = true },
            new ShiftRequest { Id = 12, Name = "Cao Văn Minh", EmployeeId = "NV-0295", Department = "Marketing", Avatar = "", AvatarColor = "bg-light-warning text-warning", CurrentShift = "CA CHIỀU", CurrentShiftClass = "badge-light-warning", TargetShift = "CA SÁNG", TargetShiftClass = "badge-light-info", Reason = "Chuyển lịch họp team sang buổi sáng.", ApplyDate = "17/10/2023", Status = "Chờ duyệt", StatusClass = "badge-light-warning", IsPending = true },
        };
    }
}
