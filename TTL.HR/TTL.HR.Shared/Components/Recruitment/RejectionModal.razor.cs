using Microsoft.AspNetCore.Components;
using TTL.HR.Application.Modules.Recruitment.Models;

namespace TTL.HR.Shared.Components.Recruitment;

public partial class RejectionModal
{
    [Parameter] public bool IsOpen { get; set; }
    [Parameter] public ApplicantItem? Applicant { get; set; }
    [Parameter] public string Reason { get; set; } = "";
    [Parameter] public EventCallback OnClose { get; set; }
    [Parameter] public EventCallback<string> OnSave { get; set; }
}
