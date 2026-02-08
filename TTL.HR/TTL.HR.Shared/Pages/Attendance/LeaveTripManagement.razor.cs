using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Components;

namespace TTL.HR.Shared.Pages.Attendance
{
    public partial class LeaveTripManagement
    {
        private List<LeaveTripRequest> _requests = new()
        {
            new LeaveTripRequest { Id = 1, EmployeeName = "Nguyễn Văn A", Department = "Phòng Kỹ thuật", Avatar = "assets/media/avatars/300-1.jpg", Type = "Nghỉ phép", SubType = "Năm", TypeClass = "badge-light-primary", StartDate = "12/05/2024", EndDate = "14/05/2024", Duration = "3 ngày", Reason = "Giải quyết việc gia đình ở quê.", Status = "Chờ phê duyệt", StatusClass = "badge-light-warning", IsPending = true },
            new LeaveTripRequest { Id = 2, EmployeeName = "Trần Thị B", Department = "Marketing", Avatar = "assets/media/avatars/300-2.jpg", Type = "Công tác", SubType = "Nội địa", TypeClass = "badge-light-info", StartDate = "15/05/2024", EndDate = "18/05/2024", Duration = "4 ngày", Reason = "Tham dự hội thảo marketing tại Đà Nẵng.", Status = "Đã phê duyệt", StatusClass = "badge-light-success", IsPending = false },
            new LeaveTripRequest { Id = 3, EmployeeName = "Lê Văn C", Department = "Kinh doanh", Avatar = "", AvatarColor = "bg-light-danger text-danger", Type = "Nghỉ ốm", SubType = "Có giấy", TypeClass = "badge-light-danger", StartDate = "10/05/2024", EndDate = "10/05/2024", Duration = "1 ngày", Reason = "Sốt cao, đi khám bệnh viện.", Status = "Đã phê duyệt", StatusClass = "badge-light-success", IsPending = false },
            new LeaveTripRequest { Id = 4, EmployeeName = "Phạm Thị D", Department = "Nhân sự", Avatar = "assets/media/avatars/300-3.jpg", Type = "Làm từ xa", SubType = "WFH", TypeClass = "badge-light-success", StartDate = "20/05/2024", EndDate = "22/05/2024", Duration = "3 ngày", Reason = "Con ốm, cần làm việc tại nhà.", Status = "Chờ phê duyệt", StatusClass = "badge-light-warning", IsPending = true },
            new LeaveTripRequest { Id = 5, EmployeeName = "Hoàng Văn E", Department = "IT", Avatar = "", AvatarColor = "bg-light-warning text-warning", Type = "Nghỉ phép", SubType = "Không lương", TypeClass = "badge-light-primary", StartDate = "25/05/2024", EndDate = "30/05/2024", Duration = "6 ngày", Reason = "Đi du lịch nước ngoài tự túc.", Status = "Từ chối", StatusClass = "badge-light-danger", IsPending = false },
            new LeaveTripRequest { Id = 6, EmployeeName = "Vũ Thị F", Department = "Kế toán", Avatar = "assets/media/avatars/300-5.jpg", Type = "Thai sán", SubType = "Sinh con", TypeClass = "badge-light-purple", StartDate = "01/06/2024", EndDate = "30/11/2024", Duration = "6 tháng", Reason = "Nghỉ thai sản theo chế độ.", Status = "Chờ phê duyệt", StatusClass = "badge-light-warning", IsPending = true },
            new LeaveTripRequest { Id = 7, EmployeeName = "Đặng Văn G", Department = "Kho vận", Avatar = "", AvatarColor = "bg-light-info text-info", Type = "Công tác", SubType = "Nước ngoài", TypeClass = "badge-light-info", StartDate = "10/06/2024", EndDate = "20/06/2024", Duration = "10 ngày", Reason = "Đào tạo tại trụ sở chính Singapore.", Status = "Chờ phê duyệt", StatusClass = "badge-light-warning", IsPending = true },
            new LeaveTripRequest { Id = 8, EmployeeName = "Bùi Thị H", Department = "Phòng Kỹ thuật", Avatar = "assets/media/avatars/300-6.jpg", Type = "Nghỉ phép", SubType = "Năm", TypeClass = "badge-light-primary", StartDate = "05/06/2024", EndDate = "05/06/2024", Duration = "0.5 ngày", Reason = "Đi công chứng giấy tờ buổi sáng.", Status = "Đã phê duyệt", StatusClass = "badge-light-success", IsPending = false },
            new LeaveTripRequest { Id = 9, EmployeeName = "Ngô Văn I", Department = "Bảo vệ", Avatar = "", AvatarColor = "bg-light-dark text-dark", Type = "Nghỉ ốm", SubType = "Tai nạn", TypeClass = "badge-light-danger", StartDate = "01/05/2024", EndDate = "05/05/2024", Duration = "5 ngày", Reason = "Tai nạn xe máy nhẹ, cần nghỉ ngơi.", Status = "Đã phê duyệt", StatusClass = "badge-light-success", IsPending = false },
            new LeaveTripRequest { Id = 10, EmployeeName = "Lý Thị K", Department = "Hành chính", Avatar = "assets/media/avatars/300-9.jpg", Type = "Việc riêng", SubType = "Hiếu hỉ", TypeClass = "badge-light-dark", StartDate = "15/06/2024", EndDate = "17/06/2024", Duration = "3 ngày", Reason = "Đám cưới em gái ruột.", Status = "Chờ phê duyệt", StatusClass = "badge-light-warning", IsPending = true },
            new LeaveTripRequest { Id = 11, EmployeeName = "Đỗ Văn L", Department = "Marketing", Avatar = "assets/media/avatars/300-10.jpg", Type = "Làm từ xa", SubType = "Hybrid", TypeClass = "badge-light-success", StartDate = "Every Fri", EndDate = "", Duration = "1 ngày/tuần", Reason = "Đăng ký lịch làm việc Hybrid cố định.", Status = "Chờ phê duyệt", StatusClass = "badge-light-warning", IsPending = true },
            new LeaveTripRequest { Id = 12, EmployeeName = "Hồ Thị M", Department = "Kinh doanh", Avatar = "", AvatarColor = "bg-light-primary text-primary", Type = "Công tác", SubType = "Gặp khách", TypeClass = "badge-light-info", StartDate = "28/05/2024", EndDate = "28/05/2024", Duration = "1 ngày", Reason = "Gặp khách hàng VIP tại Vũng Tàu.", Status = "Đã phê duyệt", StatusClass = "badge-light-success", IsPending = false },
            new LeaveTripRequest { Id = 13, EmployeeName = "Dương Văn N", Department = "Kỹ thuật", Avatar = "assets/media/avatars/300-12.jpg", Type = "Nghỉ bù", SubType = "OT", TypeClass = "badge-light-purple", StartDate = "02/06/2024", EndDate = "02/06/2024", Duration = "1 ngày", Reason = "Nghỉ bù cho chủ nhật làm thêm giờ.", Status = "Chờ phê duyệt", StatusClass = "badge-light-warning", IsPending = true },
            new LeaveTripRequest { Id = 14, EmployeeName = "Cao Thị P", Department = "Nhân sự", Avatar = "", AvatarColor = "bg-light-success text-success", Type = "Nghỉ phép", SubType = "Nửa ngày", TypeClass = "badge-light-primary", StartDate = "05/06/2024", EndDate = "05/06/2024", Duration = "0.5 ngày", Reason = "Khám sức khỏe định kỳ công ty.", Status = "Từ chối", StatusClass = "badge-light-danger", IsPending = false },
            new LeaveTripRequest { Id = 15, EmployeeName = "Trương Văn Q", Department = "Kho vận", Avatar = "assets/media/avatars/300-15.jpg", Type = "Công tác", SubType = "Kiểm kê", TypeClass = "badge-light-info", StartDate = "30/06/2024", EndDate = "05/07/2024", Duration = "6 ngày", Reason = "Kiểm kê kho hàng chi nhánh miền Bắc.", Status = "Chờ phê duyệt", StatusClass = "badge-light-warning", IsPending = true }
        };

        private bool _showDetail = false;
        private bool _showActionModal = false;
        private bool _isLoading = true;
        private LeaveTripRequest? _selectedRequest;
        private LeaveTripRequest? _requestToProcess;

        protected override async System.Threading.Tasks.Task OnInitializedAsync()
        {
            // Simulate API loading
            await System.Threading.Tasks.Task.Delay(1300);
            _isLoading = false;
        }

        private string _modalTitle = "";
        private string _modalHeading = "";
        private string _modalMessage = "";
        private string _modalColor = "primary";
        private string _modalIcon = "";
        private string _modalConfirmLabel = "";
        private bool _showReasonInput = false;
        private string _actionType = ""; // "APPROVE" or "REJECT"

        private void OpenDetail(LeaveTripRequest req)
        {
            _selectedRequest = req;
            _showDetail = true;
        }

        private void CloseDetail()
        {
            _showDetail = false;
            _selectedRequest = null;
        }

        private void RequestApprove(LeaveTripRequest req)
        {
            _requestToProcess = req;
            _actionType = "APPROVE";
            _modalTitle = "Phê duyệt yêu cầu";
            _modalHeading = "Xác nhận phê duyệt?";
            _modalMessage = $"Bạn có chắc chắn muốn phê duyệt yêu cầu {req.Type} của {req.EmployeeName}?";
            _modalColor = "success";
            _modalIcon = "ki-outline ki-check-circle";
            _modalConfirmLabel = "Đồng ý Duyệt";
            _showReasonInput = false;
            _showActionModal = true;
        }

        private void RequestReject(LeaveTripRequest req)
        {
            _requestToProcess = req;
            _actionType = "REJECT";
            _modalTitle = "Từ chối yêu cầu";
            _modalHeading = "Xác nhận từ chối?";
            _modalMessage = $"Hành động này sẽ từ chối yêu cầu {req.Type} của {req.EmployeeName}. Vui lòng nhập lý do từ chối bên dưới.";
            _modalColor = "danger";
            _modalIcon = "ki-outline ki-cross-circle";
            _modalConfirmLabel = "Xác nhận Từ chối";
            _showReasonInput = true;
            _showActionModal = true;
        }

        private void ConfirmAction(string reason)
        {
            if (_requestToProcess == null) return;

            if (_actionType == "APPROVE")
            {
                _requestToProcess.Status = "Đã phê duyệt";
                _requestToProcess.StatusClass = "badge-light-success";
                _requestToProcess.IsPending = false;
                _requestToProcess.ManagerNote = reason;
            }
            else if (_actionType == "REJECT")
            {
                _requestToProcess.Status = "Từ chối";
                _requestToProcess.StatusClass = "badge-light-danger";
                _requestToProcess.IsPending = false;
                _requestToProcess.ManagerNote = reason;
            }

            if (_selectedRequest == _requestToProcess)
            {
                CloseDetail();
            }
            
            CloseModal();
        }

        private void CloseModal()
        {
            _showActionModal = false;
            _requestToProcess = null;
        }

        private class LeaveTripRequest
        {
            public int Id { get; set; }
            public string EmployeeName { get; set; } = "";
            public string Department { get; set; } = "";
            public string Avatar { get; set; } = "";
            public string AvatarColor { get; set; } = "";
            public string Type { get; set; } = "";
            public string SubType { get; set; } = "";
            public string TypeClass { get; set; } = "";
            public string StartDate { get; set; } = "";
            public string EndDate { get; set; } = "";
            public string Duration { get; set; } = "";
            public string Reason { get; set; } = "";
            public string Status { get; set; } = "";
            public string StatusClass { get; set; } = "";
            public bool IsPending { get; set; } = false;
            public string? ManagerNote { get; set; }
        }
    }
}
