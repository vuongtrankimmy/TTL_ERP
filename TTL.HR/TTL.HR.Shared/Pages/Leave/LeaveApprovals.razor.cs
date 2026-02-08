using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace TTL.HR.Shared.Pages.Leave
{
    public partial class LeaveApprovals
    {
        private string searchQuery = "";
        private bool _showDetail = false;
        private bool _showActionModal = false;
        private bool _isLoading = true;
        private ApprovalItem? _selectedRequest;
        private ApprovalItem? _requestToProcess;

        protected override async Task OnInitializedAsync()
        {
            // Simulate API loading
            await Task.Delay(1000);
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

        private void ViewDetail(int id)
        {
            _selectedRequest = _approvals.FirstOrDefault(x => x.Id == id);
            _showDetail = true;
        }

        private void CloseDetail() => _showDetail = false;

        private void RequestApprove(ApprovalItem item)
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

        private void RequestReject(ApprovalItem item)
        {
            _requestToProcess = item;
            _actionType = "REJECT";
            _modalTitle = "Từ chối nghỉ phép";
            _modalHeading = "Xác nhận từ chối?";
            _modalMessage = $"Hành động này sẽ từ chối yêu cầu {item.Type} của {item.Name}. Vui lòng nhập lý do từ chối bên dưới.";
            _modalColor = "danger";
            _modalIcon = "ki-outline ki-cross-circle";
            _modalConfirmLabel = "Xác nhận Từ chối";
            _showReasonInput = true;
            _showActionModal = true;
        }

        private async Task ConfirmAction(string reason)
        {
            if (_requestToProcess == null) return;

            // In a real app, call API here
            if (_actionType == "APPROVE")
            {
                _requestToProcess.Status = "Approved";
                _requestToProcess.ManagerNote = reason;
                _approvals.Remove(_requestToProcess);
                await JSRuntime.InvokeVoidAsync("console.log", "Approved request " + _requestToProcess.Id);
            }
            else if (_actionType == "REJECT")
            {
                _requestToProcess.Status = "Rejected";
                _requestToProcess.ManagerNote = reason;
                _approvals.Remove(_requestToProcess);
                await JSRuntime.InvokeVoidAsync("console.log", "Rejected request " + _requestToProcess.Id);
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

        private List<ApprovalItem> _approvals = new()
        {
            new() { Id = 1, Name = "Nguyễn Văn An", Department = "Phòng Kỹ thuật", Avatar = "assets/media/avatars/300-1.jpg", Type = "Nghỉ phép năm", DateRange = "15/10 - 16/10", Duration = "2.0", Reason = "Việc gia đình ở quê cần giải quyết gấp." },
            new() { Id = 2, Name = "Lê Thị Mai", Department = "Phòng Marketing", Avatar = "assets/media/avatars/300-2.jpg", Type = "Nghỉ ốm", DateRange = "12/10 - 12/10", Duration = "1.0", Reason = "Sốt xuất huyết, có giấy khám của bác sĩ đính kèm." },
            new() { Id = 3, Name = "Trần Đức Hòa", Department = "Phòng Kinh doanh", Avatar = "", Type = "Đi công tác", DateRange = "20/10 - 22/10", Duration = "3.0", Reason = "Gặp đối tác tại Đà Nẵng để ký kết hợp đồng dự án mới." },
            new() { Id = 4, Name = "Phạm Minh Tuấn", Department = "Phòng Nhân sự", Avatar = "assets/media/avatars/300-3.jpg", Type = "Nghỉ không lương", DateRange = "18/10", Duration = "1.0", Reason = "Giải quyết thủ tục hành chính đất đai." },
            new() { Id = 5, Name = "Kiều Linh", Department = "Phòng Marketing", Avatar = "", Type = "Nghỉ thai sản", DateRange = "01/11 - 30/04", Duration = "180.0", Reason = "Nghỉ chế độ thai sản theo quy định luật lao động." },
            new() { Id = 6, Name = "Vũ Hoàng Long", Department = "Phòng Kỹ thuật", Avatar = "assets/media/avatars/300-4.jpg", Type = "Làm việc từ xa", DateRange = "05/11 - 06/11", Duration = "2.0", Reason = "Hỗ trợ deploy hệ thống ban đêm, xin làm remote bù." },
            new() { Id = 7, Name = "Bùi Hồng", Department = "Phòng HC-NS", Avatar = "", Type = "Nghỉ ốm", DateRange = "28/10", Duration = "0.5", Reason = "Đi tái khám định kỳ tại bệnh viện." },
            new() { Id = 8, Name = "Trần Văn Cường", Department = "Phòng Kỹ thuật", Avatar = "assets/media/avatars/300-5.jpg", Type = "Nghỉ phép năm", DateRange = "01/11 - 05/11", Duration = "5.0", Reason = "Đưa gia đình đi du lịch nước ngoài theo kế hoạch." },
            new() { Id = 9, Name = "Mai Duy", Department = "Phòng Kinh doanh", Avatar = "", Type = "Đi công tác", DateRange = "10/11 - 13/11", Duration = "4.0", Reason = "Training sản phẩm mới cho chi nhánh khu vực miền Nam." },
            new() { Id = 10, Name = "Lý Thái Tổ", Department = "Ban Giám Đốc", Avatar = "assets/media/avatars/300-6.jpg", Type = "Nghỉ phép năm", DateRange = "20/12 - 31/12", Duration = "10.0", Reason = "Nghỉ phép tiêu chuẩn cuối năm." },
            new() { Id = 11, Name = "Đỗ Thùy Trang", Department = "Phòng Kế toán", Avatar = "assets/media/avatars/300-7.jpg", Type = "Nghỉ không lương", DateRange = "25/11", Duration = "0.5", Reason = "Việc riêng gia đình không thể sắp xếp khác." },
            new() { Id = 12, Name = "Hoàng Gia Bảo", Department = "IT Support", Avatar = "", Type = "Nghỉ ốm", DateRange = "02/11 - 04/11", Duration = "3.0", Reason = "Viêm họng cấp cần nghỉ ngơi điều trị." },
            new() { Id = 13, Name = "Phan Anh", Department = "Phòng Thiết kế", Avatar = "assets/media/avatars/300-8.jpg", Type = "Đi công tác", DateRange = "15/11 - 16/11", Duration = "1.5", Reason = "Khảo sát mặt bằng thi công thực tế cho dự án mới." }
        };

        private class ApprovalItem
        {
            public int Id { get; set; }
            public string Name { get; set; } = "";
            public string Department { get; set; } = "";
            public string Avatar { get; set; } = "";
            public string Type { get; set; } = "";
            public string DateRange { get; set; } = "";
            public string Duration { get; set; } = "";
            public string Reason { get; set; } = "";
            public string Status { get; set; } = "Pending";
            public string? ManagerNote { get; set; }
        }
    }
}
