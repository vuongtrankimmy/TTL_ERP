using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Components;
using TTL.HR.Shared.Models;

namespace TTL.HR.Shared.Pages.Recruitment
{
    public partial class RecruitmentList
    {
        [Inject] public IRecruitmentService RecruitmentService { get; set; } = default!;
        [Inject] public NavigationManager Navigation { get; set; } = default!;

        private bool _isLoading = true;
        private bool IsDeleteModalOpen = false;
        private JobDetail? ItemToDelete;
        private List<JobDetail> _jobs = new();

        protected override async Task OnInitializedAsync()
        {
            await LoadData();
        }

        private async Task LoadData()
        {
            _isLoading = true;
            _jobs = (await RecruitmentService.GetJobsAsync()).ToList();
            _isLoading = false;
        }

        private void HandleEdit(int id)
        {
            Navigation.NavigateTo($"/recruitment/add?id={id}");
        }

        private void PromptDelete(JobDetail job)
        {
            ItemToDelete = job;
            IsDeleteModalOpen = true;
        }

        private void CloseDeleteModal()
        {
            IsDeleteModalOpen = false;
            ItemToDelete = null;
        }

        private async Task ConfirmDelete()
        {
            if (ItemToDelete != null)
            {
                await RecruitmentService.DeleteJobAsync(ItemToDelete.Id.ToString());
                await LoadData();
                CloseDeleteModal();
            }
        }
    }
}
