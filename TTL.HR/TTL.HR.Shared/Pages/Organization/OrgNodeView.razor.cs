using System;
using System.Linq;
using Microsoft.AspNetCore.Components;
using TTL.HR.Shared.Models;

namespace TTL.HR.Shared.Pages.Organization
{
    public partial class OrgNodeView
    {
        [Parameter] public OrgNode Node { get; set; }
        [Parameter] public EventCallback<OrgNode> OnNodeClick { get; set; }

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
