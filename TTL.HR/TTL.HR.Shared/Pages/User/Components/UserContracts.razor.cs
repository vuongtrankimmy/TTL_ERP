using Microsoft.AspNetCore.Components;

namespace TTL.HR.Shared.Pages.User.Components;

public partial class UserContracts
{
    [Parameter] public string? EmployeeId { get; set; }

    private List<ContractModel> _contracts = new()
    {
        new ContractModel { ContractNumber = "HĐLĐ/2021/001", ContractType = "Hợp đồng không xác định thời hạn", StartDate = new DateTime(2021, 1, 1), IsActive = true },
        new ContractModel { ContractNumber = "HĐTV/2020/055", ContractType = "Hợp đồng thử việc", StartDate = new DateTime(2020, 10, 1), EndDate = new DateTime(2020, 12, 31), IsActive = false }
    };

    public class ContractModel
    {
        public string ContractNumber { get; set; } = "";
        public string ContractType { get; set; } = "";
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool IsActive { get; set; }
    }
}
