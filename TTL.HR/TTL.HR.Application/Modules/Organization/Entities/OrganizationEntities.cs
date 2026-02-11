using System.Collections.Generic;
using TTL.HR.Application.Modules.Common.Entities;

namespace TTL.HR.Application.Modules.Organization.Entities
{
    public class Department : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        
        public string ParentId { get; set; } = string.Empty;
        
        public string ManagerId { get; set; } = string.Empty;

        public int Level { get; set; }
        public string Path { get; set; } = string.Empty;
    }

    public class Position : BaseEntity
    {
        public string Title { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal BaseSalaryRangeMin { get; set; }
        public decimal BaseSalaryRangeMax { get; set; }
        
        public string DepartmentId { get; set; } = string.Empty;
    }
}
