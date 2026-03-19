#pragma warning disable CS8600, CS8601, CS8604, CS8625
using Microsoft.AspNetCore.Components;
using TTL.HR.Application.Modules.HumanResource.Models;
using TTL.HR.Application.Modules.Common.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TTL.HR.Shared.Pages.Employees.Components
{
    public partial class EmployeeStepPersonal : ComponentBase
    {
        [Parameter] public EmployeeModel Model { get; set; } = new();
        [Parameter] public bool IsProcessing { get; set; }
        [Parameter] public bool IsReadOnly { get; set; }
        [Parameter] public Dictionary<string, string> Errors { get; set; } = new();

        [Parameter] public List<LookupModel> GenderLookups { get; set; } = new();
        [Parameter] public List<LookupModel> MaritalStatusLookups { get; set; } = new();
        [Parameter] public List<LookupModel> EthnicityLookups { get; set; } = new();
        [Parameter] public List<LookupModel> ReligionLookups { get; set; } = new();
        [Parameter] public List<CountryModel> NationalityLookups { get; set; } = new();
        [Parameter] public List<LookupModel> ProvinceLookups { get; set; } = new();


        [Parameter] public EventCallback OnSave { get; set; }

        private bool _isAddressSameAsResidence;
        private bool IsAddressSameAsResidence 
        { 
            get => _isAddressSameAsResidence; 
            set 
            {
                if (_isAddressSameAsResidence != value)
                {
                    _isAddressSameAsResidence = value;
                    if (_isAddressSameAsResidence) SyncAddressWithResidence();
                }
            }
        }

        protected override void OnInitialized()
        {
            // Detect if addresses are already the same to set checkbox state
            if (Model.PersonalDetails != null && 
                Model.PersonalDetails.ProvinceId == Model.PersonalDetails.CurrentProvinceId &&
                Model.PersonalDetails.DistrictId == Model.PersonalDetails.CurrentDistrictId &&
                Model.PersonalDetails.WardId == Model.PersonalDetails.CurrentWardId &&
                !string.IsNullOrEmpty(Model.PersonalDetails.Residence))
            {
                _isAddressSameAsResidence = true;
            }
        }

        private async Task HandleSave()
        {
            if (IsAddressSameAsResidence) SyncAddressWithResidence();
            
            if (OnSave.HasDelegate)
            {
                await OnSave.InvokeAsync();
            }
        }

        private void HandleResidenceUpdate(string addr)
        {
            Model.Residence = addr;
            if (Model.PersonalDetails != null)
            {
                Model.PersonalDetails.Residence = addr;
            }
            if (IsAddressSameAsResidence) SyncAddressWithResidence();
        }

        private void HandleCurrentAddressUpdate(string addr)
        {
            Model.Address = addr;
            if (Model.PersonalDetails != null)
            {
                Model.PersonalDetails.Address = addr;
            }
        }

        private void SyncAddressWithResidence()
        {
            if (Model.PersonalDetails == null) return;

            Model.PersonalDetails.CurrentCountryId = Model.PersonalDetails.CountryId;
            Model.PersonalDetails.CurrentProvinceId = Model.PersonalDetails.ProvinceId;
            Model.PersonalDetails.CurrentDistrictId = Model.PersonalDetails.DistrictId;
            Model.PersonalDetails.CurrentWardId = Model.PersonalDetails.WardId;
            Model.PersonalDetails.CurrentStreetId = Model.PersonalDetails.StreetId;
            Model.PersonalDetails.CurrentStreet = Model.PersonalDetails.Street ?? string.Empty;
            
            Model.Address = Model.Residence;
            Model.PersonalDetails.Address = Model.PersonalDetails.Residence;
            
            StateHasChanged();
        }
    }
}
