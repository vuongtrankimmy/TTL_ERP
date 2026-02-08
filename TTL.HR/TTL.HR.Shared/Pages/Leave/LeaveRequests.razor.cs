using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Components;

namespace TTL.HR.Shared.Pages.Leave
{
    public partial class LeaveRequests
    {
        private bool _showDetail = false;
        private bool _showCreate = false;
        private bool _showActionModal = false;
        private bool _isLoading = true;
        private LeaveRequestItem? _selectedRequest;
        private LeaveRequestItem? _requestToProcess;
        
        protected override async System.Threading.Tasks.Task OnInitializedAsync()
        {
            // Simulate API loading
            await System.Threading.Tasks.Task.Delay(1500);
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

        private void openDetail(LeaveRequestItem item)
        {
            _selectedRequest = item;
            _showDetail = true;
            _showCreate = false;
        }

        private void closeDetail() => _showDetail = false;

        private void openCreate()
        {
            _showCreate = true;
            _showDetail = false;
        }

        private void closeCreate() => _showCreate = false;

        private void requestApprove(LeaveRequestItem item)
        {
            _requestToProcess = item;
            _actionType = "APPROVE";
            _modalTitle = "Phê duyệt nghỉ phép";
            _modalHeading = "Xác nhận phê duyệt?";
            _modalMessage = $"Bạn có chắc chắn muốn phê duyệt yêu cầu {item.Type} của {item.Name}?";
            _modalColor = "success";
            _modalIcon = "ki-outline ki-check-circle";
            _modalConfirmLabel = "Xác nhận Duyệt";
            _showReasonInput = false;
            _showActionModal = true;
        }

        private void requestReject(LeaveRequestItem item)
        {
            _requestToProcess = item;
            _actionType = "REJECT";
            _modalTitle = "Từ chối nghỉ phép";
            _modalHeading = "Xác nhận từ chối?";
            _modalMessage = $"Hành động này sẽ từ chối yêu cầu {item.Type} của {item.Name}. Vui lòng nhập lý do từ chối.";
            _modalColor = "danger";
            _modalIcon = "ki-outline ki-cross-circle";
            _modalConfirmLabel = "Xác nhận Từ chối";
            _showReasonInput = true;
            _showActionModal = true;
        }

        private void confirmAction(string reason)
        {
            if (_requestToProcess == null) return;

            if (_actionType == "APPROVE")
            {
                _requestToProcess.Status = "Đã phê duyệt";
                _requestToProcess.StatusColor = "badge-light-success";
                _requestToProcess.ManagerNote = reason;
            }
            else if (_actionType == "REJECT")
            {
                _requestToProcess.Status = "Đã từ chối";
                _requestToProcess.StatusColor = "badge-light-danger";
                _requestToProcess.ManagerNote = reason;
            }

            if (_selectedRequest == _requestToProcess)
            {
                _selectedRequest = null; // Forces update or can keep and just close
                _showDetail = false;
            }

            closeModal();
        }

        private void closeModal()
        {
            _showActionModal = false;
            _requestToProcess = null;
        }

        private List<LeaveRequestItem> _leaveRequests = new()
        {
            new() { Id = "REQ001", Name = "Nguyễn Văn Lộc", Department = "Kỹ thuật", Avatar = "assets/media/avatars/300-1.jpg", Type = "Nghỉ phép năm", TypeColor = "badge-light-primary", DateRange = "10/02 - 12/02", Duration = "3 ngày", Reason = "Giải quyết việc gia đình cá nhân quan trọng.", Status = "Đã phê duyệt", StatusColor = "badge-light-success" },
            new() { Id = "REQ002", Name = "Lê Thị Mai", Department = "Marketing", Avatar = "assets/media/avatars/300-2.jpg", Type = "Nghỉ ốm", TypeColor = "badge-light-danger", DateRange = "05/02 - 06/02", Duration = "2 ngày", Reason = "Bị sốt xuất huyết, có giấy chỉ định của bác sĩ.", Status = "Chờ phê duyệt", StatusColor = "badge-light-warning" },
            new() { Id = "REQ003", Name = "Phạm Hoàng", Department = "Kỹ thuật", Avatar = "", AvatarBg = "bg-info", Type = "Đi công tác", TypeColor = "badge-light-info", DateRange = "15/02 - 20/02", Duration = "6 ngày", Reason = "Hỗ trợ triển khai dự án tại chi nhánh miền Nam.", Status = "Đã phê duyệt", StatusColor = "badge-light-success" },
            new() { Id = "REQ004", Name = "Trần Minh", Department = "Nhân sự", Avatar = "assets/media/avatars/300-3.jpg", Type = "Làm việc từ xa", TypeColor = "badge-light-dark", DateRange = "07/02", Duration = "1 ngày", Reason = "Làm việc tại nhà để xử lý hồ sơ nhân sự bảo mật.", Status = "Đã phê duyệt", StatusColor = "badge-light-success" },
            new() { Id = "REQ005", Name = "Hoàng Nam", Department = "Kinh doanh", Avatar = "assets/media/avatars/300-5.jpg", Type = "Nghỉ không lương", TypeColor = "badge-light-secondary", DateRange = "12/02 - 14/02", Duration = "3 ngày", Reason = "Về quê có việc hiếu hỷ đột xuất.", Status = "Đã từ chối", StatusColor = "badge-light-danger" },
            new() { Id = "REQ006", Name = "Kiều Linh", Department = "Thiết kế", Avatar = "", AvatarBg = "bg-warning", Type = "Nghỉ thai sản", TypeColor = "badge-light-warning", DateRange = "01/02 - 01/08", Duration = "6 tháng", Reason = "Chế độ nghỉ thai sản theo quy định luật lao động.", Status = "Đã phê duyệt", StatusColor = "badge-light-success" },
            new() { Id = "REQ007", Name = "Vũ Long", Department = "Vận hành", Avatar = "assets/media/avatars/300-6.jpg", Type = "Đi đào tạo", TypeColor = "badge-light-primary", DateRange = "22/02 - 25/02", Duration = "4 ngày", Reason = "Tham gia khóa học nâng cao quản trị hệ thống Cloud.", Status = "Chờ phê duyệt", StatusColor = "badge-light-warning" },
            new() { Id = "REQ008", Name = "Đặng Thu", Department = "Tài chính", Avatar = "assets/media/avatars/300-9.jpg", Type = "Nghỉ kết hôn", TypeColor = "badge-light-success", DateRange = "20/02 - 23/02", Duration = "4 ngày", Reason = "Nghỉ lễ kết hôn (3 ngày định mức + 1 ngày phép).", Status = "Đã phê duyệt", StatusColor = "badge-light-success" },
            new() { Id = "REQ009", Name = "Bùi Cường", Department = "Kỹ thuật", Avatar = "", AvatarBg = "bg-primary", Type = "Nghỉ việc riêng", TypeColor = "badge-light-info", DateRange = "09/02", Duration = "1 ngày", Reason = "Xử lý thủ tục hành chính tại cơ quan nhà nước.", Status = "Chờ phê duyệt", StatusColor = "badge-light-warning" },
            new() { Id = "REQ010", Name = "Ngô Diệp", Department = "Hành chính", Avatar = "assets/media/avatars/300-11.jpg", Type = "Giải trình đi muộn", TypeColor = "badge-light-secondary", DateRange = "04/02", Duration = "1 giờ", Reason = "Xe hỏng đột xuất trên đường đến văn phòng.", Status = "Đã phê duyệt", StatusColor = "badge-light-success" }
        };

        private class LeaveRequestItem
        {
            public string Id { get; set; } = "";
            public string Name { get; set; } = "";
            public string Department { get; set; } = "";
            public string Avatar { get; set; } = "";
            public string AvatarBg { get; set; } = "bg-primary";
            public string Type { get; set; } = "";
            public string TypeColor { get; set; } = "";
            public string DateRange { get; set; } = "";
            public string Duration { get; set; } = "";
            public string Reason { get; set; } = "";
            public string Status { get; set; } = "";
            public string StatusColor { get; set; } = "";
            public string? ManagerNote { get; set; }
        }
    }
}
