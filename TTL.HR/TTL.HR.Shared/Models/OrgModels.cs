using System.Collections.Generic;

namespace TTL.HR.Shared.Models
{
    public class OrgNode
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Role { get; set; }
        public string Avatar { get; set; }
        public string Type { get; set; } // Department, Employee
        public bool IsManager { get; set; }
        public List<OrgNode> Children { get; set; } = new List<OrgNode>();
    }
}
