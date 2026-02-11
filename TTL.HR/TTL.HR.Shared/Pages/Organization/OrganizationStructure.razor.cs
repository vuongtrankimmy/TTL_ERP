using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using TTL.HR.Application.Modules.Organization.Models;

namespace TTL.HR.Shared.Pages.Organization
{
    public partial class OrganizationStructure
    {
        [Inject] public IJSRuntime JSRuntime { get; set; } = null!;
        private string searchQuery = "";
        private OrgNode? rootNode;
        private OrgNode? selectedNode;
        private int AllEmployeesCount = 0;
        private bool _isLoading = true;
        private bool _jsInitialized = false;

        // Add Node Modal state
        private bool showAddModal = false;
        private OrgNode newNode = new OrgNode();
        private string newNodeParentId = "";
        private List<OrgNode> flatNodeList = new List<OrgNode>();

        protected override async System.Threading.Tasks.Task OnInitializedAsync()
        {
            InitializeData();
            RefreshFlatList();
            // Simulate tree layout calculation/API
            await System.Threading.Tasks.Task.Delay(1500);
            _isLoading = false;
            StateHasChanged();
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

        private void InitializeData()
        {
            // Root: Ban Giám đốc
            rootNode = new OrgNode { 
                Id = "1", 
                Role = "Ban Giám đốc (CEO Office)", 
                Name = "Nguyễn Văn Anh", 
                Type = "Department", 
                Avatar = "assets/media/avatars/300-1.jpg",
                IsManager = true,
                Status = "Chính thức",
                CccdNumber = "001092001234",
                DateOfBirth = new DateOnly(1992, 5, 15),
                Gender = "Nam",
                PlaceOfOrigin = "Xuân Trường, Nam Định",
                Residence = "Số 123, Đường ABC, Phường XYZ, Quận Cầu Giấy, Hà Nội",
                Phone = "0987.654.321",
                PersonalEmail = "nv.anh@gmail.com",
                WorkEmail = "anh.nv@ttl.com.vn",
                EmployeeCode = "NV-001",
                ContractType = "Chính thức - Không thời hạn",
                OfficialJoinDate = new DateOnly(2021, 1, 1),
                Workplace = "Trụ sở chính - Hà Nội"
            };

            // Khối Vận hành
            var cooNode = new OrgNode { 
                Id = "2", 
                Role = "Khối Vận hành (COO)", 
                Name = "Trần Thị Bình", 
                Type = "Department", 
                Avatar = "assets/media/avatars/300-2.jpg",
                IsManager = true,
                Status = "Chính thức"
            };
            
            // Khối Tài chính
            var cfoNode = new OrgNode { 
                Id = "3", 
                Role = "Khối Tài chính (CFO)", 
                Name = "Lê Văn Cường", 
                Type = "Department", 
                Avatar = "assets/media/avatars/300-3.jpg",
                IsManager = true
            };

            // Khối Kinh doanh
            var ccoNode = new OrgNode { 
                Id = "4", 
                Role = "Khối Kinh doanh (CCO)", 
                Name = "Phạm Hoàng Dũng", 
                Type = "Department", 
                Avatar = "assets/media/avatars/300-4.jpg",
                IsManager = true
            };

            rootNode.Children.Add(cooNode);
            rootNode.Children.Add(cfoNode);
            rootNode.Children.Add(ccoNode);

            // COO: 2 phòng (Nhân sự, IT) - 5 người
            AddSubDepartments(cooNode, new[] { "Phòng Nhân sự", "Phòng IT" }, 4);
            
            // CFO: 1 phòng (Kế toán) - 4 người
            AddSubDepartments(cfoNode, new[] { "Phòng Kế toán" }, 4);
            
            // CCO: 1 phòng (Kinh doanh) - 7 người
            AddSubDepartments(ccoNode, new[] { "Phòng Kinh doanh" }, 7);

            AllEmployeesCount = CountNodes(rootNode);
            selectedNode = rootNode; // Default selected
        }

        private void AddSubDepartments(OrgNode parent, string[] names, int totalEmp)
        {
            int perDept = totalEmp / names.Length;
            int idCounter = int.Parse(parent.Id) * 10;

            foreach (var name in names)
            {
                var deptNode = new OrgNode { 
                    Id = (++idCounter).ToString(), 
                    Role = name, 
                    Name = GetRandomName("Trưởng phòng"), 
                    Type = "Department", 
                    Avatar = $"assets/media/avatars/300-{idCounter % 30 + 1}.jpg",
                    IsManager = true
                };
                parent.Children.Add(deptNode);

                // Add Employees to this dept
                for (int i = 0; i < perDept - 1; i++)
                {
                    deptNode.Children.Add(new OrgNode {
                        Id = (++idCounter).ToString(),
                        Role = GetRandomRole(name),
                        Name = GetRandomName("Nhân viên"),
                        Type = "Employee",
                        Avatar = $"assets/media/avatars/300-{idCounter % 30 + 1}.jpg",
                        EmployeeCode = $"NV-{idCounter}",
                        CccdNumber = $"00120200{idCounter}",
                        DateOfBirth = new DateOnly(1995, 10, 20),
                        Gender = i % 2 == 0 ? "Nam" : "Nữ",
                        Phone = $"09{idCounter:D8}",
                        ContractType = "Thử việc",
                        Status = "Thử việc"
                    });
                }
            }
        }

        private string GetRandomName(string prefix)
        {
            string[] surnames = { "Nguyễn", "Trần", "Lê", "Phạm", "Hoàng", "Huỳnh", "Phan", "Vũ", "Võ", "Đặng" };
            string[] midnames = { "Văn", "Thị", "Hồng", "Minh", "Quang", "Anh", "Hoàng", "Gia", "Bảo", "Ngọc" };
            string[] names = { "Anh", "Bắc", "Cường", "Dũng", "Em", "Giang", "Hương", "Khánh", "Linh", "Minh", "Nam", "Oanh", "Phong", "Quân", "Sơn", "Trang", "Uyên", "Vinh", "Xuân", "Yến" };
            
            var rand = new Random();
            return $"{surnames[rand.Next(surnames.Length)]} {midnames[rand.Next(midnames.Length)]} {names[rand.Next(names.Length)]}";
        }

        private string GetRandomRole(string dept)
        {
            if (dept.Contains("IT")) return "Software Engineer";
            if (dept.Contains("Nhân sự")) return "HR Specialist";
            if (dept.Contains("Kế toán")) return "Accountant";
            if (dept.Contains("Bán hàng")) return "Sales Executive";
            if (dept.Contains("Marketing")) return "Content Creator";
            return "Chuyên viên";
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
