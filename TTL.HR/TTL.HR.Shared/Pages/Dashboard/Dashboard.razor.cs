using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Threading.Tasks;
using TTL.HR.Application.Modules.Dashboard.Interfaces;
using TTL.HR.Application.Modules.Dashboard.Models;
using ApexCharts;
using System.Collections.Generic;
using System.Linq;

namespace TTL.HR.Shared.Pages.Dashboard
{
    public partial class Dashboard
    {
        [Inject] public IDashboardService DashboardService { get; set; } = default!;
        [Inject] public IJSRuntime JS { get; set; } = default!;
        
        private DashboardOverviewModel? _overview;
        private bool _isLoading = true;
        private string? _errorMessage;

        // Chart Data & Options
        private List<AttendanceData> _attendanceData = new();
        private ApexChartOptions<AttendanceData> _attendanceChartOptions = new();
        private ApexChartOptions<DepartmentStat> _deptChartOptions = new();
        private ApexChartOptions<PayrollTrendDto> _payrollChartOptions = new();

        protected override void OnInitialized()
        {
            SetupChartOptions();
        }

        private void SetupChartOptions()
        {
            _attendanceChartOptions = new ApexChartOptions<AttendanceData>
            {
                Debug = true,
                Chart = new Chart { Toolbar = new Toolbar { Show = false } },
                Colors = new List<string> { "#3699FF", "#FFA800", "#F64E60" },
                Legend = new Legend { Position = LegendPosition.Bottom },
                PlotOptions = new PlotOptions { Pie = new PlotOptionsPie { Donut = new PlotOptionsDonut { Labels = new DonutLabels { Show = true, Total = new DonutLabelTotal { Show = true, Label = "Tổng" } } } } }
            };

            _deptChartOptions = new ApexChartOptions<DepartmentStat>
            {
                Debug = true,
                Chart = new Chart { Toolbar = new Toolbar { Show = false } },
                PlotOptions = new PlotOptions { Bar = new PlotOptionsBar { Horizontal = true } },
                Colors = new List<string> { "#8950FC" }
            };

            _payrollChartOptions = new ApexChartOptions<PayrollTrendDto>
            {
                Debug = true,
                Chart = new Chart { Toolbar = new Toolbar { Show = false }, Sparkline = new ChartSparkline { Enabled = false } },
                Stroke = new Stroke { Curve = Curve.Smooth, Width = 3 },
                Fill = new Fill { Type = FillType.Gradient, Gradient = new FillGradient { ShadeIntensity = 1, OpacityFrom = 0.7, OpacityTo = 0.3 } },
                Colors = new List<string> { "#1BC5BD" },
                Xaxis = new XAxis { Labels = new XAxisLabels { Show = true } },
                Yaxis = new List<YAxis> { new YAxis { Show = false } }
            };
        }

        private void PrepareChartData()
        {
            if (_overview == null) return;

            _attendanceData = new List<AttendanceData>
            {
                new AttendanceData { Label = "Diện diện", Value = _overview.AttendanceToday.Present },
                new AttendanceData { Label = "Đi muộn", Value = _overview.AttendanceToday.Late },
                new AttendanceData { Label = "Vắng mặt", Value = _overview.AttendanceToday.Absent }
            };
        }

        protected override async Task OnInitializedAsync()
        {
            try
            {
                _errorMessage = null;
                _overview = await DashboardService.GetOverviewAsync();
                
                PrepareChartData();
                
                // Allow DOM to settle
                await Task.Delay(300);
                _isLoading = false;
                StateHasChanged();
            }
            catch (System.Exception ex)
            {
                _isLoading = false;
                _errorMessage = ex.Message;
                // Try to show as toast as well
                try {
                    await JS.InvokeVoidAsync("toastr.error", "Lỗi: " + ex.Message);
                } catch { }
                // Logger or UI feedback is enough
                // System.Console.WriteLine($"Dashboard Error: {ex.Message}");
            }
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            // Error handling is now in catch above, commenting out generic fallback
        }

        public class AttendanceData
        {
            public string Label { get; set; } = string.Empty;
            public double Value { get; set; }
        }
    }
}
