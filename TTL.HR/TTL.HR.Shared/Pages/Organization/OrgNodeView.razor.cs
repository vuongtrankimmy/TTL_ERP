using System;
using System.Linq;
using Microsoft.AspNetCore.Components;
using TTL.HR.Application.Modules.Organization.Models;

namespace TTL.HR.Shared.Pages.Organization
{
    public partial class OrgNodeView
    {
        [Parameter] public required OrgNode Node { get; set; }
        [Parameter] public EventCallback<OrgNode> OnNodeClick { get; set; }
        [Parameter] public bool IsReadOnly { get; set; }

        private int CountTotalEmployees(OrgNode node)
        {
            int count = node.Type == "Employee" ? 1 : 0;
            foreach (var child in node.Children)
            {
                count += CountTotalEmployees(child);
            }
            return count;
        }
    }
}
