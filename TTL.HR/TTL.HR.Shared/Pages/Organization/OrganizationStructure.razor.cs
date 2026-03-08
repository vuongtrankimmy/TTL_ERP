using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using TTL.HR.Application.Modules.Organization.Interfaces;
using TTL.HR.Application.Modules.Organization.Models;
using TTL.HR.Application.Modules.Common.Interfaces;
using TTL.HR.Application.Modules.Common.Models;

namespace TTL.HR.Shared.Pages.Organization
{
    public partial class OrganizationStructure
    {
        [Inject] public IDepartmentService DepartmentService { get; set; } = null!;
        [Inject] public IJSRuntime JSRuntime { get; set; } = null!;
        [Inject] public IMasterDataService MasterDataService { get; set; } = null!;

        private List<TTL.HR.Application.Modules.Common.Models.LookupModel> contractTypeLookups = new();
        private string searchQuery = "";
        private OrgNode? rootNode;
        private OrgNode? selectedNode;
        private int AllEmployeesCount = 0;
        private bool _isLoading = true;
        private bool _jsInitialized = false;
        private bool _isReadOnly = true; // Set to true to lock add/edit functions

        // Add Node Modal state
        private bool showAddModal = false;
        private OrgNode newNode = new OrgNode();
        private string newNodeParentId = "";
        private List<OrgNode> flatNodeList = new List<OrgNode>();

        protected override async Task OnInitializedAsync()
        {
            contractTypeLookups = await MasterDataService.GetCachedLookupsAsync("ContractType");
            await LoadData();
        }

        private async Task LoadData()
        {
            _isLoading = true;
            try
            {
                var structure = await DepartmentService.GetOrganizationStructureAsync();
                if (structure != null && structure.Count > 0)
                {
                    rootNode = structure[0]; // Assuming single root for now
                    AllEmployeesCount = SumEmployees(rootNode);
                }
                else
                {
                    // Fallback to empty or default if API fails/returns empty
                    rootNode = null;
                }
                
                RefreshFlatList();
                selectedNode = rootNode;
            }
            catch (Exception ex)
            {
                System.Console.WriteLine($"OrgStructure Error: {ex.Message}");
            }
            finally
            {
                _isLoading = false;
            }
        }

        private void RefreshFlatList()
        {
            flatNodeList.Clear();
            if (rootNode != null)
                FlattenNodes(rootNode);
        }

        private void FlattenNodes(OrgNode node)
        {
            flatNodeList.Add(node);
            foreach (var child in node.Children)
                FlattenNodes(child);
        }

        private int SumEmployees(OrgNode? node)
        {
            if (node == null) return 0;
            int count = node.EmployeeCount;
            foreach (var child in node.Children) count += SumEmployees(child);
            return count;
        }

        private int CountNodes(OrgNode node)
        {
            int count = 1;
            foreach (var child in node.Children) count += CountNodes(child);
            return count;
        }

        private void HandleNodeClick(OrgNode node) 
        { 
            selectedNode = node;
            StateHasChanged();
            JSRuntime.InvokeVoidAsync("openEmployeeDetail");
        }
        
        private void ToggleAddModal() 
        { 
            newNode = new OrgNode { 
                Id = (rootNode != null ? CountNodes(rootNode) + 1 : 1).ToString(),
                Avatar = "assets/media/avatars/300-10.jpg",
                Type = "Employee"
            };
            newNodeParentId = selectedNode?.Id ?? rootNode?.Id;
            showAddModal = !showAddModal; 
        }

        private void SaveNewNode()
        {
            var parent = flatNodeList.Find(n => n.Id == newNodeParentId);
            if (parent != null)
            {
                parent.Children.Add(newNode);
                AllEmployeesCount = CountNodes(rootNode);
                RefreshFlatList();
                showAddModal = false;
                StateHasChanged();
            }
        }

        private async Task ExportDiagram()
        {
            await JSRuntime.InvokeVoidAsync("exportOrgChart");
        }



        private async Task ZoomIn() => await JSRuntime.InvokeVoidAsync("zoomAtCenter", true);
        private async Task ZoomOut() => await JSRuntime.InvokeVoidAsync("zoomAtCenter", false);
        private async Task ResetPanZoom() => await JSRuntime.InvokeVoidAsync("resetPanZoom");

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (!_isLoading && !_jsInitialized)
            {
                await Task.Delay(100);
                _jsInitialized = true;
                await JSRuntime.InvokeVoidAsync("initOrgChartPanZoom");
            }
        }
    }
}
