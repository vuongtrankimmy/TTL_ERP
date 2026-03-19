using Microsoft.AspNetCore.Components;
using TTL.HR.Application.Modules.HumanResource.Models;
using TTL.HR.Application.Modules.Common.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TTL.HR.Shared.Pages.Employees.Components
{
    public partial class EmployeeStepCompetence : ComponentBase
    {
        [Parameter] public EmployeeModel Model { get; set; } = new();
        [Parameter] public bool IsProcessing { get; set; }
        [Parameter] public bool IsReadOnly { get; set; }
        [Parameter] public Dictionary<string, string> Errors { get; set; } = new();
        [Parameter] public List<LookupModel> SchoolLookups { get; set; } = new();
        [Parameter] public List<LookupModel> DegreeLookups { get; set; } = new();
        [Parameter] public List<LookupModel> MajorLookups { get; set; } = new();

        [Parameter] public EventCallback OnSave { get; set; }

        private async Task HandleSave()
        {
            if (OnSave.HasDelegate)
            {
                await OnSave.InvokeAsync();
            }
        }
        private void AddEducation()
        {
            Model.Education ??= new List<EducationDetailDto>();
            Model.Education.Add(new EducationDetailDto());
        }

        private void RemoveEducation(EducationDetailDto item)
        {
            Model.Education?.Remove(item);
        }

        private void AddExperience()
        {
            Model.Experience ??= new List<ExperienceDetailDto>();
            Model.Experience.Add(new ExperienceDetailDto { StartDate = System.DateTime.Now, EndDate = System.DateTime.Now });
        }

        private void RemoveExperience(ExperienceDetailDto item)
        {
            Model.Experience?.Remove(item);
        }

        private void AddDependent()
        {
            if (Model.PersonalDetails == null) Model.PersonalDetails = new EmployeePersonalDetails();
            Model.PersonalDetails.Dependents ??= new List<DependentDetailDto>();
            Model.PersonalDetails.Dependents.Add(new DependentDetailDto());
        }

        private void RemoveDependent(DependentDetailDto item)
        {
            Model.PersonalDetails?.Dependents?.Remove(item);
        }
    }
}
