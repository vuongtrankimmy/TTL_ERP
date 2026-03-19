using Microsoft.AspNetCore.Components;
using TTL.HR.Application.Modules.Common.Models;

namespace TTL.HR.Shared.Components.Common
{
    public partial class TTLLocationSelector : IDisposable
    {
        // ─── Parameters ────────────────────────────────────────────────────────
        /// <summary>Mã quốc gia ISO 3166-1 numeric (VD: 704 = Việt Nam)</summary>
        [Parameter] public int? CountryId { get; set; }
        [Parameter] public EventCallback<int?> CountryIdChanged { get; set; }

        /// <summary>Mã tỉnh/thành phố theo quy định hành chính VN (int)</summary>
        [Parameter] public int? ProvinceId { get; set; }
        [Parameter] public EventCallback<int?> ProvinceIdChanged { get; set; }

        /// <summary>Mã quận/huyện theo quy định hành chính VN (int)</summary>
        [Parameter] public int? DistrictId { get; set; }
        [Parameter] public EventCallback<int?> DistrictIdChanged { get; set; }

        /// <summary>Mã phường/xã theo quy định hành chính VN (int)</summary>
        [Parameter] public int? WardId { get; set; }
        [Parameter] public EventCallback<int?> WardIdChanged { get; set; }

        /// <summary>Số thứ tự đường theo lookup (int), null nếu nhập tay tự do</summary>
        [Parameter] public int? StreetId { get; set; }
        [Parameter] public EventCallback<int?> StreetIdChanged { get; set; }

        /// <summary>Tên đường / số nhà (text). Có thể chọn từ datalist hoặc nhập tay.</summary>
        [Parameter] public string? Street { get; set; }
        [Parameter] public EventCallback<string?> StreetChanged { get; set; }

        [Parameter] public EventCallback<string> OnAddressUpdated { get; set; }

        [Parameter] public bool IsRequired { get; set; }
        [Parameter] public bool ShowCountry { get; set; } = true;
        [Parameter] public bool ShowStreet { get; set; } = true;
        [Parameter] public bool Disabled { get; set; }
        [Parameter] public bool IsReadOnly { get; set; }

        [Parameter] public string CountryCssClass { get; set; } = "col-md-3";
        [Parameter] public string ProvinceCssClass { get; set; } = "col-md-3";
        [Parameter] public string DistrictCssClass { get; set; } = "col-md-3";
        [Parameter] public string WardCssClass { get; set; } = "col-md-3";
        [Parameter] public string StreetCssClass { get; set; } = "col-md-3";

        // ─── Default Constants ─────────────────────────────────────────────────
        // ISO 3166-1 numeric code cho Việt Nam
        private const int VN_COUNTRY_ID = 704;

        // ─── Private State ─────────────────────────────────────────────────────
        private List<CountryModel>? _countries;
        private List<LookupModel>? _provinces;
        private List<LookupModel>? _districts;
        private List<LookupModel>? _wards;
        private List<LookupModel>? _streets;
        private bool _isLoadingProvinces;
        private bool _isLoadingDistricts;
        private bool _isLoadingWards;
        private bool _isLoadingStreets;
        private int? _lastCountryId;
        private int? _lastProvinceId;
        private int? _lastDistrictId;
        private int? _lastWardId;
        private string? _lastStreet;
        private bool _isDisposed;

        public void Dispose()
        {
            _isDisposed = true;
        }

        // ─── Lifecycle ─────────────────────────────────────────────────────────
        protected override async Task OnInitializedAsync()
        {
            if (_isDisposed) return;
            _countries = await MasterDataService.GetCachedCountriesAsync();
            await LoadProvinces();

            // Apply SMART DEFAULTS nếu đang tạo mới (ProvinceId chưa có)
            if (ProvinceId == null && !Disabled)
            {
                // Default Country: Việt Nam (704)
                if (CountryId == null)
                {
                    var vn = _countries?.FirstOrDefault(c =>
                        c.IntId == VN_COUNTRY_ID ||
                        c.Code == "VN" ||
                        (c.Name?.Contains("Việt Nam") ?? false));
                    if (vn != null)
                    {
                        CountryId = vn.IntId > 0 ? vn.IntId : VN_COUNTRY_ID;
                        _lastCountryId = CountryId;
                        await CountryIdChanged.InvokeAsync(CountryId);
                    }
                    else
                    {
                        // Dùng hardcode nếu API chưa trả về
                        CountryId = VN_COUNTRY_ID;
                        _lastCountryId = CountryId;
                        await CountryIdChanged.InvokeAsync(CountryId);
                    }
                }

                // Default Province: Hồ Chí Minh
                var hcm = _provinces?.FirstOrDefault(p =>
                    (p.Name?.Contains("Hồ Chí Minh") ?? false) ||
                    (p.Name?.Contains("TP.HCM") ?? false));
                if (hcm != null)
                {
                    ProvinceId = ToIntId(hcm);
                    _lastProvinceId = ProvinceId;
                    await ProvinceIdChanged.InvokeAsync(ProvinceId);

                    if (ProvinceId.HasValue)
                    {
                        var distCount = await LoadDistricts(ProvinceId.Value);
                        if (distCount > 0 && _districts?.Any() == true)
                        {
                            // Default District: Quận 1
                            var q1 = _districts.FirstOrDefault(d => 
                                d.Name?.Contains("Quận 1") == true || 
                                d.Name?.Equals("1") == true);
                            
                            DistrictId = ToIntId(q1 ?? _districts.First());
                            _lastDistrictId = DistrictId;
                            await DistrictIdChanged.InvokeAsync(DistrictId);

                            if (DistrictId.HasValue)
                            {
                                await LoadWards(DistrictId.Value);
                                if (_wards?.Any() == true)
                                {
                                    WardId = ToIntId(_wards.First());
                                    _lastWardId = WardId;
                                    await WardIdChanged.InvokeAsync(WardId);
                                    await LoadStreetsForWard(ProvinceId.Value, WardId);
                                }
                            }
                        }
                        else if (distCount == 0)
                        {
                            await LoadWardsByProvince(ProvinceId.Value);
                            if (_wards?.Any() == true)
                            {
                                WardId = ToIntId(_wards.First());
                                _lastWardId = WardId;
                                await WardIdChanged.InvokeAsync(WardId);
                            }
                        }
                    }
                }

                await NotifyUpdate();
            }
            else if (ProvinceId.HasValue)
            {
                // Chế độ chỉnh sửa — load cascading theo giá trị hiện có
                _lastProvinceId = ProvinceId;
                _lastCountryId = CountryId;

                var distCount = await LoadDistricts(ProvinceId.Value);
                if (distCount > 0 && DistrictId.HasValue)
                {
                    _lastDistrictId = DistrictId;
                    await LoadWards(DistrictId.Value);
                    if (WardId.HasValue)
                    {
                        _lastWardId = WardId;
                        await LoadStreetsForWard(ProvinceId.Value, WardId);
                    }
                }
                else if (distCount == 0)
                {
                    await LoadWardsByProvince(ProvinceId.Value);
                }

                await NotifyUpdate();
            }
        }

        private bool _isSyncingParameters;
        protected override async Task OnParametersSetAsync()
        {
            if (_isSyncingParameters) return;
            _isSyncingParameters = true;
            try
            {
                bool needsNotify = false;

                if (_provinces == null || !_provinces.Any())
                    await LoadProvinces();

                if (ProvinceId.HasValue && ProvinceId != _lastProvinceId)
                {
                    _lastProvinceId = ProvinceId;
                    var distCount = await LoadDistricts(ProvinceId.Value);
                    if (distCount == 0)
                        await LoadWardsByProvince(ProvinceId.Value);
                    needsNotify = true;
                }

                if (DistrictId.HasValue && DistrictId != _lastDistrictId)
                {
                    _lastDistrictId = DistrictId;
                    await LoadWards(DistrictId.Value);
                    needsNotify = true;
                }

                if (WardId.HasValue && WardId != _lastWardId)
                {
                    _lastWardId = WardId;
                    if (ProvinceId.HasValue)
                        await LoadStreetsForWard(ProvinceId.Value, WardId);
                    needsNotify = true;
                }

                if (!string.IsNullOrEmpty(Street) && Street != _lastStreet)
                {
                    _lastStreet = Street;
                    needsNotify = true;
                }

                if (needsNotify)
                    await NotifyUpdate();
            }
            finally
            {
                _isSyncingParameters = false;
            }
        }

        // ─── Load Helpers ──────────────────────────────────────────────────────
        private async Task LoadProvinces()
        {
            _isLoadingProvinces = true;
            StateHasChanged();
            try { _provinces = await MasterDataService.GetProvincesAsync(); }
            finally { _isLoadingProvinces = false; StateHasChanged(); }
        }

        private async Task<int> LoadDistricts(int provinceId)
        {
            _isLoadingDistricts = true;
            Console.WriteLine($"[TTLLocationSelector] Loading districts for ProvinceId={provinceId}");
            StateHasChanged();
            try
            {
                _districts = await MasterDataService.GetDistrictsAsync(provinceId);
                int count = _districts?.Count ?? 0;
                Console.WriteLine($"[TTLLocationSelector] Loaded {count} districts");
                return count;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[TTLLocationSelector] ERROR loading districts: {ex.Message}");
                return 0;
            }
            finally
            {
                _isLoadingDistricts = false;
                await NotifyUpdate();
                StateHasChanged();
            }
        }

        private async Task LoadWards(int districtId)
        {
            _isLoadingWards = true;
            Console.WriteLine($"[TTLLocationSelector] Loading wards for DistrictId={districtId}");
            StateHasChanged();
            try
            {
                _wards = await MasterDataService.GetWardsAsync(districtId);
                Console.WriteLine($"[TTLLocationSelector] Loaded {_wards?.Count ?? 0} wards");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[TTLLocationSelector] ERROR loading wards: {ex.Message}");
            }
            finally
            {
                _isLoadingWards = false;
                await NotifyUpdate();
                StateHasChanged();
            }
        }

        private async Task LoadWardsByProvince(int provinceId)
        {
            _isLoadingWards = true;
            Console.WriteLine($"[TTLLocationSelector] Loading wards by ProvinceId={provinceId} (fallback)");
            StateHasChanged();
            try
            {
                _wards = await MasterDataService.GetWardsAsync(null, provinceId.ToString());
                Console.WriteLine($"[TTLLocationSelector] Loaded {_wards?.Count ?? 0} wards (by province)");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[TTLLocationSelector] ERROR loading wards by province: {ex.Message}");
            }
            finally
            {
                _isLoadingWards = false;
                await NotifyUpdate();
                StateHasChanged();
            }
        }

        private async Task LoadStreetsForWard(int provinceId, int? wardId)
        {
            _isLoadingStreets = true;
            StateHasChanged();
            try
            {
                _streets = await MasterDataService.GetStreetsAsync(provinceId.ToString(), wardId?.ToString());
            }
            finally
            {
                _isLoadingStreets = false;
                await NotifyUpdate();
                StateHasChanged();
            }
        }

        // ─── Event Handlers ────────────────────────────────────────────────────
        private async Task HandleCountryChanged(ChangeEventArgs e)
        {
            CountryId = int.TryParse(e.Value?.ToString(), out var v) ? v : (int?)null;
            _lastCountryId = CountryId;
            ProvinceId = null;
            DistrictId = null;
            WardId = null;
            _districts = null;
            _wards = null;

            await CountryIdChanged.InvokeAsync(CountryId);
            await ProvinceIdChanged.InvokeAsync(null);
            await DistrictIdChanged.InvokeAsync(null);
            await WardIdChanged.InvokeAsync(null);
            await NotifyUpdate();
        }

        private async Task HandleProvinceChanged(ChangeEventArgs e)
        {
            ProvinceId = int.TryParse(e.Value?.ToString(), out var v) ? v : (int?)null;
            _lastProvinceId = ProvinceId;
            DistrictId = null;
            WardId = null;
            StreetId = null;
            Street = null;
            _districts = null;
            _wards = null;
            _streets = null;

            await ProvinceIdChanged.InvokeAsync(ProvinceId);
            await DistrictIdChanged.InvokeAsync(null);
            await WardIdChanged.InvokeAsync(null);
            await StreetIdChanged.InvokeAsync(null);
            await StreetChanged.InvokeAsync(null);

            if (ProvinceId.HasValue)
            {
                var distCount = await LoadDistricts(ProvinceId.Value);
                if (distCount == 0)
                    await LoadWardsByProvince(ProvinceId.Value);
            }

            await NotifyUpdate();
        }

        private async Task HandleDistrictChanged(ChangeEventArgs e)
        {
            DistrictId = int.TryParse(e.Value?.ToString(), out var v) ? v : (int?)null;
            _lastDistrictId = DistrictId;
            WardId = null;
            StreetId = null;
            Street = null;
            _lastWardId = null;
            _lastStreet = null;
            _wards = null;
            _streets = null;

            await DistrictIdChanged.InvokeAsync(DistrictId);
            await WardIdChanged.InvokeAsync(null);
            await StreetIdChanged.InvokeAsync(null);
            await StreetChanged.InvokeAsync(null);

            if (DistrictId.HasValue)
            {
                await LoadWards(DistrictId.Value);

                // FALLBACK: nếu quận không có phường, load thẳng theo tỉnh
                if ((_wards == null || !_wards.Any()) && ProvinceId.HasValue)
                {
                    Console.WriteLine($"[TTLLocationSelector] FALLBACK: No wards for District {DistrictId}, loading by Province {ProvinceId}");
                    await LoadWardsByProvince(ProvinceId.Value);
                }
            }

            await NotifyUpdate();
        }

        private async Task HandleWardChanged(ChangeEventArgs e)
        {
            WardId = int.TryParse(e.Value?.ToString(), out var v) ? v : (int?)null;
            _lastWardId = WardId;
            await WardIdChanged.InvokeAsync(WardId);

            if (WardId.HasValue && ProvinceId.HasValue)
                await LoadStreetsForWard(ProvinceId.Value, WardId);

            await NotifyUpdate();
        }

        private async Task HandleStreetInput(ChangeEventArgs e)
        {
            var rawValue = e.Value?.ToString();
            Street = ToTitleCase(rawValue);
            _lastStreet = Street;

            // Nếu text khớp với option trong datalist → map sang StreetId
            var matched = _streets?.FirstOrDefault(s =>
                string.Equals(s.Name, Street, StringComparison.OrdinalIgnoreCase));
            StreetId = ToIntId(matched);

            await StreetChanged.InvokeAsync(Street);
            await StreetIdChanged.InvokeAsync(StreetId);
            await NotifyUpdate();
        }

        private async Task HandleStreetChanged(ChangeEventArgs e)
        {
            var rawValue = e.Value?.ToString();
            Street = ToTitleCase(rawValue);
            var matched = _streets?.FirstOrDefault(s =>
                string.Equals(s.Name, Street, StringComparison.OrdinalIgnoreCase));
            StreetId = ToIntId(matched);

            await StreetChanged.InvokeAsync(Street);
            await StreetIdChanged.InvokeAsync(StreetId);
            await NotifyUpdate();
        }

        // ─── Utility ───────────────────────────────────────────────────────────
        /// <summary>Chuyển LookupModel → int ID (ưu tiên LookupID, fallback parse string Id)</summary>
        private static int? ToIntId(LookupModel? m)
        {
            if (m == null) return null;
            if (m.LookupID > 0) return m.LookupID;
            return int.TryParse(m.Id, out var v) ? v : (int?)null;
        }

        private string? ToTitleCase(string? input)
        {
            if (string.IsNullOrWhiteSpace(input)) return input;
            try
            {
                var culture = new System.Globalization.CultureInfo("vi-VN");
                return culture.TextInfo.ToTitleCase(input.ToLower());
            }
            catch { return input; }
        }

        private async Task NotifyUpdate()
        {
            if (_isDisposed) return;
            if (_isLoadingDistricts || _isLoadingWards) return;

            var parts = new List<string>();
            if (!string.IsNullOrEmpty(Street)) parts.Add(Street);

            if (WardId.HasValue && _wards != null && _wards.Any())
            {
                var ward = _wards.FirstOrDefault(w => ToIntId(w) == WardId);
                if (ward != null) parts.Add(ward.Name);
            }

            if (DistrictId.HasValue && _districts != null && _districts.Any())
            {
                var district = _districts.FirstOrDefault(d => ToIntId(d) == DistrictId);
                if (district != null) parts.Add(district.Name);
            }

            if (ProvinceId.HasValue && _provinces != null && _provinces.Any())
            {
                var province = _provinces.FirstOrDefault(p => ToIntId(p) == ProvinceId);
                if (province != null) parts.Add(province.Name);
            }

            if (ProvinceId.HasValue)
            {
                if (CountryId.HasValue && _countries != null && _countries.Any())
                {
                    var country = _countries.FirstOrDefault(c => c.IntId == CountryId);
                    parts.Add(country?.Name ?? "Việt Nam");
                }
                else
                {
                    parts.Add("Việt Nam");
                }
            }

            var fullAddress = string.Join(", ", parts);
            if (!string.IsNullOrEmpty(fullAddress))
                await OnAddressUpdated.InvokeAsync(fullAddress);
        }
    }
}
