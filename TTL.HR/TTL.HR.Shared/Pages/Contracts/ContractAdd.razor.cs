using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using TTL.HR.Application.Modules.HumanResource.Interfaces;
using TTL.HR.Application.Modules.HumanResource.Models;
using Entities = TTL.HR.Application.Modules.HumanResource.Entities;
using TTL.HR.Application.Modules.Common.Interfaces;
using TTL.HR.Application.Modules.Common.Models;

namespace TTL.HR.Shared.Pages.Contracts
{
    public partial class ContractAdd
    {
        [Parameter] public string? Id { get; set; }

        [Inject] public NavigationManager Navigation { get; set; } = default!;
        [Inject] public IJSRuntime JSRuntime { get; set; } = default!;
        [Inject] public IContractService ContractService { get; set; } = default!;
        [Inject] public IMasterDataService MasterDataService { get; set; } = default!;

        private List<LookupModel> contractTypeLookups = new();
        private List<LookupModel> templateStatusLookups = new();
        private ContractTemplateModel TemplateModel = new();
        private string PreviewContent = "";
        private bool IsLoading = false;

        protected override async Task OnInitializedAsync()
        {
            // Fix: Xử lý triệt để lỗi màn hình mờ khi chuyển từ trang khác sang (linh hồn của sự mờ)
            await JSRuntime.InvokeVoidAsync("eval", @"
                document.querySelectorAll('.drawer-overlay, .modal-backdrop').forEach(el => el.remove());
                document.body.style.overflow = 'auto';
                document.body.removeAttribute('data-kt-drawer');
            ");

            contractTypeLookups = await MasterDataService.GetCachedLookupsAsync("ContractType");
            templateStatusLookups = await MasterDataService.GetCachedLookupsAsync("TemplateStatus");

            if (!string.IsNullOrEmpty(Id))
            {
                await LoadTemplate();
            }
            else
            {
                TemplateModel = new ContractTemplateModel
                {
                    StatusId = templateStatusLookups.FirstOrDefault(x => x.Name == "Active")?.Id ?? "65dae2f30000000000000401", // Default to Active
                    Icon = "bi bi-file-earmark-text",
                    Color = "primary",
                    ContentHtml = TemplateContent
                };
            }
        }

        private async Task LoadTemplate()
        {
            IsLoading = true;
            try
            {
                Console.WriteLine($"Loading template with ID: {Id}");
                var model = await ContractService.GetTemplateAsync(Id!);
                if (model != null)
                {
                    TemplateModel = model;
                    TemplateContent = model.ContentHtml;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading template: {ex.Message}");
                await JSRuntime.InvokeVoidAsync("Swal.fire", "Lỗi", "Không thể tải dữ liệu mẫu hợp đồng.", "error");
            }
            finally
            {
                IsLoading = false;
            }
        }

        // Mock Data for Preview
        private Dictionary<string, string> MockData = new Dictionary<string, string> {
            {"Ten_Cong_Ty", "CÔNG TY CỔ PHẦN ERP TECH"},
            {"Dia_Chi_Cong_Ty", "Tầng 10, Tòa nhà Bitexco, Q.1, TP.HCM"},
            {"SDT_Cong_Ty", "028.9999.8888"},
            {"MST_Cong_Ty", "0312345678"},
            {"STK_Cong_Ty", "123456789 Tại VCB - CN TP.HCM"},
            {"Nguoi_Dai_Dien", "HOÀNG BÁ TRUNG"},
            {"Chuc_Vu_Nguoi_Dai_Dien", "Tổng Giám Đốc"},
            {"Ten_Nhan_Vien", "NGUYỄN VĂN AN"},
            {"Ma_Nhan_Vien", "ERP001"},
            {"Ngay_Sinh", "01/01/1995"},
            {"Quoc_Tich", "Việt Nam"},
            {"Nghe_Nghiep", "Kỹ sư phần mềm"},
            {"Dia_Chi_Thuong_Tru", "123 Đường Nguyễn Huệ, P. Bến Nghé, Q.1, TP.HCM"},
            {"So_CCCD", "079095000123"},
            {"So_So_Lao_Dong", "SLD-2024-001"},
            {"Ma_Hop_Dong", "HĐ-2024/001"},
            {"Loai_Hop_Dong", "Xác định thời hạn"},
            {"Thoi_Han_Hop_Dong", "12"},
            {"Ngay_Bat_Dau", "01/03/2026"},
            {"Ngay_Ket_Thuc", "01/03/2027"},
            {"Dia_Diem_Lam_Viec", "Tại trụ sở công ty"},
            {"Phong_Ban", "Phát triển sản phẩm"},
            {"Chuc_Vu", "Chuyên viên cao cấp"},
            {"Thoi_Gian_Lam_Viec", "Giờ hành chính (44h/tuần)"},
            {"Ngay_Ky", "05"},
            {"Thang_Ky", "02"},
            {"Nam_Ky", "2026"},
            {"Dia_Diem_Ky", "TP.HCM"},
            {"Muc_Luong", "25,000,000"},
            {"Phu_Cap", "3,000,000"}
        };

        // Main HTML Content of the Template
        private string TemplateContent = @"
            <div style='text-align:center'>
                <p><strong>CỘNG HÒA XàHỘI CHỦ NGHĨA VIỆT NAM</strong></p>
                <p>Độc lập – Tự do – Hạnh phúc</p>
                <p style='text-align:right'><span class='badge badge-light-primary fs-7 fw-bold px-2 py-1 align-middle'>{{Dia_Diem_Ky}}</span>, ngày <span class='badge badge-light-primary fs-7 fw-bold px-2 py-1 align-middle'>{{Ngay_Ky}}</span> tháng <span class='badge badge-light-primary fs-7 fw-bold px-2 py-1 align-middle'>{{Thang_Ky}}</span> năm <span class='badge badge-light-primary fs-7 fw-bold px-2 py-1 align-middle'>{{Nam_Ky}}</span></p>
                
                <p class='mt-4'><strong>HỢP ĐỒNG LAO ĐỘNG</strong></p>
                <p><em>Số: <span class='badge badge-light-primary fs-7 fw-bold px-2 py-1 align-middle'>{{Ma_Hop_Dong}}</span>/HĐLĐ</em></p>
            </div>

            <div style='text-align:justify'>
                <p class='mt-4'><em>Hôm nay, ngày <span class='badge badge-light-primary fs-7 fw-bold px-2 py-1 align-middle'>{{Ngay_Ky}}</span> tháng <span class='badge badge-light-primary fs-7 fw-bold px-2 py-1 align-middle'>{{Thang_Ky}}</span> năm <span class='badge badge-light-primary fs-7 fw-bold px-2 py-1 align-middle'>{{Nam_Ky}}</span> Tại <span class='badge badge-light-primary fs-7 fw-bold px-2 py-1 align-middle'>{{Dia_Diem_Ky}}</span> </em></p>
                
                <p><strong>BÊN A: <span class='badge badge-light-primary fs-7 fw-bold px-2 py-1 align-middle'>{{Ten_Cong_Ty}}</span></strong></p>
                <p>Đại diện Ông/Bà: <span class='badge badge-light-primary fs-7 fw-bold px-2 py-1 align-middle'>{{Nguoi_Dai_Dien}}</span></p>
                <p>Chức vụ: <span class='badge badge-light-primary fs-7 fw-bold px-2 py-1 align-middle'>{{Chuc_Vu_Nguoi_Dai_Dien}}</span></p>
                <p>Địa chỉ: <span class='badge badge-light-primary fs-7 fw-bold px-2 py-1 align-middle'>{{Dia_Chi_Cong_Ty}}</span></p>
                <p>Điện thoại: <span class='badge badge-light-primary fs-7 fw-bold px-2 py-1 align-middle'>{{SDT_Cong_Ty}}</span></p>
                <p>Mã số thuế: <span class='badge badge-light-primary fs-7 fw-bold px-2 py-1 align-middle'>{{MST_Cong_Ty}}</span></p>
                <p>Số tài khoản: <span class='badge badge-light-primary fs-7 fw-bold px-2 py-1 align-middle'>{{STK_Cong_Ty}}</span></p>
                <br>
                <p><strong>BÊN B: NGƯỜI LAO ĐỘNG</strong></p>
                <p>Ông/Bà: <span class='badge badge-light-primary fs-7 fw-bold px-2 py-1 align-middle'>{{Ten_Nhan_Vien}}</span></p>
                <p>Sinh năm: <span class='badge badge-light-primary fs-7 fw-bold px-2 py-1 align-middle'>{{Ngay_Sinh}}</span></p>
                <p>Quốc tich: <span class='badge badge-light-primary fs-7 fw-bold px-2 py-1 align-middle'>{{Quoc_Tich}}</span></p>
                <p>Nghề nghiệp: <span class='badge badge-light-primary fs-7 fw-bold px-2 py-1 align-middle'>{{Nghe_Nghiep}}</span></p>
                <p>Địa chỉ thường trú: <span class='badge badge-light-primary fs-7 fw-bold px-2 py-1 align-middle'>{{Dia_Chi_Thuong_Tru}}</span></p>
                <p>Số CMTND: <span class='badge badge-light-primary fs-7 fw-bold px-2 py-1 align-middle'>{{So_CCCD}}</span></p>
                <p>Số sổ lao động (nếu có): <span class='badge badge-light-primary fs-7 fw-bold px-2 py-1 align-middle'>{{So_So_Lao_Dong}}</span></p>

                <p class='mt-4'><em>Cùng thỏa thuận ký kết <u>Hợp đồng lao động</u> (HĐLĐ) và cam kết làm đúng những điều khoản sau đây:</em></p>

                <p class='mt-4'><strong>Điều 1: Điều khoản chung</strong></p>
                <p>1. Loại HĐLĐ: <span class='badge badge-light-primary fs-7 fw-bold px-2 py-1 align-middle'>{{Loai_Hop_Dong}}</span></p>
                <p>2. Thời hạn HĐLĐ <span class='badge badge-light-primary fs-7 fw-bold px-2 py-1 align-middle'>{{Thoi_Han_Hop_Dong}}</span> tháng</p>
                <p>3. Thời điểm từ: ngày <span class='badge badge-light-primary fs-7 fw-bold px-2 py-1 align-middle'>{{Ngay_Bat_Dau}}</span> đến ngày <span class='badge badge-light-primary fs-7 fw-bold px-2 py-1 align-middle'>{{Ngay_Ket_Thuc}}</span> </p>
                <p>4. Địa điểm làm việc: <span class='badge badge-light-primary fs-7 fw-bold px-2 py-1 align-middle'>{{Dia_Diem_Lam_Viec}}</span></p>
                <p>5. Bộ phận công tác: Phòng <span class='badge badge-light-primary fs-7 fw-bold px-2 py-1 align-middle'>{{Phong_Ban}}</span>. Chức danh chuyên môn (vị trí công tác): <span class='badge badge-light-primary fs-7 fw-bold px-2 py-1 align-middle'>{{Chuc_Vu}}</span></p>
                <p>6. Nhiệm vụ công việc như sau:</p>
                <p>- Thực hiện công việc theo đúng chức danh chuyên môn của mình dưới sự quản lý, điều hành của Ban Giám đốc (và các cá nhân được bổ nhiệm hoặc ủy quyền phụ trách).</p>
                <p>- Phối hợp cùng với các bộ phận, phòng ban khác trong Công ty để phát huy tối đa hiệu quả công việc.</p>
                <p>- Hoàn thành những công việc khác tùy thuộc theo yêu cầu kinh doanh của Công ty và theo quyết định của Ban Giám đốc (và các cá nhân được bổ nhiệm hoặc ủy quyền phụ trách).</p>

                <p class='mt-4'><strong>Điều 2: Chế độ làm việc</strong></p>
                <p>1. Thời gian làm việc: <span class='badge badge-light-primary fs-7 fw-bold px-2 py-1 align-middle'>{{Thoi_Gian_Lam_Viec}}</span></p>
                <p>2. Từ ngày thứ 2 đến sáng ngày thứ 7:</p>
                <p>- Buổi sáng : 8h00 – 12h00</p>
                <p>- Buổi chiều: 13h30 – 17h30</p>
                <p>- Sáng ngày thứ 7: Làm việc từ 08h00 đến 12h00</p>
                <p>3. Do tính chất công việc, nhu cầu kinh doanh hay nhu cầu của tổ chức/bộ phận, Công ty có thể cho áp dụng thời gian làm việc linh hoạt. Những nhân viên được áp dụng thời gian làm việc linh hoạt có thể không tuân thủ lịch làm việc cố định bình thường mà làm theo ca kíp, nhưng vẫn phải đảm bảo đủ số giờ làm việc theo quy định.</p>
                <p>4. Thiết bị và công cụ làm việc sẽ được Công ty cấp phát tùy theo nhu cầu của công việc.</p>
                <p>Điều kiện an toàn và vệ sinh lao động tại nơi làm việc theo quy định của pháp luật hiện hành.</p>

                <p class='mt-4'><strong>Điều 3: Nghĩa vụ và quyền lợi của người lao động</strong></p>
                <p><strong>3.1 Nghĩa vụ</strong></p>
                <p>a) Thực hiện công việc với sự tận tâm, tận lực và mẫn cán, đảm bảo hoàn thành công việc với hiệu quả cao nhất theo sự phân công, điều hành (bằng văn bản hoặc bằng miệng) của Ban Giám đốc trong Công ty (và các cá nhân được Ban Giám đốc bổ nhiệm hoặc ủy quyền phụ trách).</p>
                <p>b) Hoàn thành công việc được giao và sẵn sàng chấp nhận mọi sự điều động khi có yêu cầu.</p>
                <p>c) Nắm rõ và chấp hành nghiêm túc kỷ luật lao động, an toàn lao động, vệ sinh lao động, PCCC, văn hóa công ty, nội quy lao động và các chủ trương, chính sách của Công ty.</p>
                <p>d) Bồi thường vi phạm và vật chất theo quy chế, nội quy của Công ty và pháp luật Nhà nước quy định.</p>
                <p>e) Tham dự đầy đủ, nhiệt tình các buổi huấn luyện, đào tạo, hội thảo do Bộ phận hoặc Công ty tổ chức.</p>
                <p>f) Thực hiện đúng cam kết trong HĐLĐ và các thỏa thuận bằng văn bản khác với Công ty.</p>
                <p>g) Đóng các loại bảo hiểm, các khoản thuế.... đầy đủ theo quy định của pháp luật.</p>
                <p>h) Chế độ đào tạo: Theo quy định của Công ty và yêu cầu công việc. Trong trường hợp CBNV được cử đi đào tạo thì nhân viên phải hoàn thành khoá học đúng thời hạn, phải cam kết sẽ phục vụ lâu dài cho Công ty sau khi kết thúc khoá học và được hưởng nguyên lương, các quyền lợi khác được hưởng như người đi làm.</p>
                <p>i) Nếu sau khi kết thúc khóa đào tạo mà nhân viên không tiếp tục hợp tác với Công ty thì nhân viên phải hoàn trả lại 100% phí đào tạo và các khoản chế độ đã được nhận trong thời gian đào tạo.</p>

                <p class='mt-4'><strong>3.2 Quyền lợi</strong></p>
                <p>a) Tiền lương và phụ cấp:</p>
                <p>- Mức lương chính: <span class='badge badge-light-primary fs-7 fw-bold px-2 py-1 align-middle'>{{Muc_Luong}}</span> VNĐ/tháng.</p>
                <p>- Phụ cấp trách nhiệm: <span class='badge badge-light-primary fs-7 fw-bold px-2 py-1 align-middle'>{{Phu_Cap}}</span> VNĐ/tháng</p>
                <p>- Phụ cấp hiệu suất công việc: Theo đánh giá của quản lý.</p>
                <p>- Lương hiệu quả: Theo quy định của phòng ban, công ty.</p>
                <p>- Công tác phí: Tùy từng vị trí, người lao động được hưởng theo quy định của công ty.</p>
                <p>- Hình thức trả lương: Lương thời gian.</p>
                <p>b) Các quyền lợi khác:</p>
                <p>- Khen thưởng: Người lao động được khuyến khích bằng vật chất và tinh thần khi có thành tích trong công tác hoặc theo quy định của công ty.</p>
                <p>- Chế độ nâng lương: Theo quy định của Nhà nước và quy chế tiền lương của Công ty. Người lao động hoàn thành tốt nhiệm vụ được giao, không vi phạm kỷ luật và/hoặc không trong thời gian xử lý kỷ luật lao động và đủ điều kiện về thời gian theo quy chế lương thì được xét nâng lương.</p>
                <p>- Chế độ nghỉ: Theo quy định chung của Nhà nước</p>
                <p>+ Nghỉ hàng tuần: 1,5 ngày (Chiều Thứ 7 và ngày Chủ nhật).</p>
                <p>+ Nghỉ hàng năm: Những nhân viên được ký Hợp đồng chính thức và có thâm niên công tác 12 tháng thì sẽ được nghỉ phép năm có hưởng lương (01 ngày phép/01 tháng, 12 ngày phép/01 năm). Nhân viên có thâm niên làm việc dưới 12 tháng thì thời gian nghỉ hằng năm được tính theo tỷ lệ tương ứng với số thời gian làm việc<em>.</em></p>
                <p>+ Nghỉ ngày Lễ: Các ngày nghỉ Lễ pháp định. Các ngày nghỉ lễ nếu trùng với ngày Chủ nhật thì sẽ được nghỉ bù vào ngày trước hoặc ngày kế tiếp tùy theo tình hình cụ thể mà Ban lãnh đạo Công ty sẽ chỉ đạo trực tiếp.</p>
                <p>- Chế độ Bảo hiểm xã hội theo quy định của nhà nước. (5)</p>
                <p>- Các chế độ được hưởng: Người lao động được hưởng các chế độ ngừng việc, trợ cấp thôi việc hoặc bồi thường theo quy định của Pháp luật hiện hành.</p>
                <p>- Thỏa thuận khác: Công ty được quyền chấm dứt HĐLĐ trước thời hạn đối với Người lao động có kết quả đánh giá hiệu suất công việc dưới mức quy định trong 03 tháng liên tục.</p>

                <p class='mt-4'><strong>Điều 4: Nghĩa vụ và quyền hạn của người sử dụng lao động</strong></p>
                <p><strong>4.1 Nghĩa vụ</strong></p>
                <p>Thực hiện đầy đủ những điều kiện cần thiết đã cam kết trong Hợp đồng lao động để người lao động đạt hiệu quả công việc cao. Bảo đảm việc làm cho người lao động theo Hợp đồng đã ký.</p>
                <p>Thanh toán đầy đủ, đúng thời hạn các chế độ và quyền lợi cho người lao động theo Hợp đồng lao động.</p>

                <p class='mt-4'><strong>4.2 Quyền hạn</strong></p>
                <p>a) Điều hành người lao động hoàn thành công việc theo Hợp đồng (bố trí, điều chuyển công việc cho người lao động theo đúng chức năng chuyên môn).</p>
                <p>b) Có quyền chuyển tạm thời lao động, ngừng việc, thay đổi, tạm thời chấm dứt Hợp đồng lao động và áp dụng các biện pháp kỷ luật theo quy định của Pháp luật hiện hành và theo nội quy của Công ty trong thời gian hợp đồng còn giá trị.</p>
                <p>c) Tạm hoãn, chấm dứt Hợp đồng, kỷ luật người lao động theo đúng quy định của Pháp luật, và nội quy lao động của Công ty.</p>
                <p>d) Có quyền đòi bồi thường, khiếu nại với cơ quan liên đới để bảo vệ quyền lợi của mình nếu người lao động vi phạm Pháp luật hay các điều khoản của hợp đồng này.</p>

                <p class='mt-4'><strong>Điều 5: Đơn phương chấm dứt hợp đồng:</strong></p>
                <p><strong>5.1 Người sử dụng lao động</strong></p>
                <p>a) Theo quy định tại điều 38 Bộ luật Lao động thì người sử dụng lao động có quyền đơn phương chấm dứt hợp đồng lao động trong những trường hợp sau đây:</p>
                <p>b) Người lao động thường xuyên không hoàn thành công việc theo hợp đồng.</p>
                <p>c) Người lao động bị xử lý kỷ luật sa thải theo quy định tại điều 85 của Bộ luật Lao động.</p>
                <p>d) Người lao động làm theo hợp đồng lao động không xác định thời hạn ốm đau đã điều trị 12 tháng liền, người lao động làm theo hợp đồng lao động xác định thời hạn ốm đau đã điều trị 06 tháng liền và người lao động làm theo hợp đồng lao động dưới 01 năm ốm đau đã điều trị quá nửa thời hạn hợp đồng, mà khả năng lao động chưa hồi phục. Khi sức khoẻ của người lao động bình phục, thì được xem xét để giao kết tiếp hợp đồng lao động.</p>
                <p>e) Do thiên tai, hỏa hoạn, hoặc những lý do bất khả kháng khác mà người sử dụng lao động đã tìm mọi biện pháp khắc phục nhưng vẫn buộc phải thu hẹp sản xuất, giảm chỗ làm việc.</p>
                <p>f) Doanh nghiệp, cơ quan, tổ chức chấm dứt hoạt động.</p>
                <p>g) Người lao động vi phạm kỷ luật mức sa thải.</p>
                <p>i) Người lao động có hành vi gây thiệt hại nghiêm trọng về tài sản và lợi ích của Công ty.</p>
                <p>k) Người lao động đang thi hành kỷ luật mức chuyển công tác mà tái phạm.</p>
                <p>l) Người lao động tự ý bỏ việc 5 ngày/1 tháng và 20 ngày/1 năm.</p>
                <p>m) Người lao động vi phạm Pháp luật Nhà nước.</p>
                <p>Trong thời hạn 07 ngày, kể từ ngày chấm dứt Hợp đồng lao động, hai bên có trách nhiệm thanh toán đầy đủ các khoản có liên quan đến quyền lợi của mỗi bên, trường hợp đặc biệt, có thể kéo dài nhưng không quá 30 ngày.</p>
                <p>Trong trường hợp doanh nghiệp bị phá sản thì các khoản có liên quan đến quyền lợi của người lao động được thanh toán theo quy định của Luật Phá sản doanh nghiệp.</p>

                <p class='mt-4'><strong>5.2 Người lao động</strong></p>
                <p>a) Khi người lao động đơn phương chấm dứt Hợp đồng lao động trước thời hạn phải tuân thủ theo điều 37 Bộ luật Lao động và phải dựa trên các căn cứ sau:</p>
                <p>b) Không được bố trí theo đúng công việc, địa điểm làm việc hoặc không được bảo đảm các điều kiện làm việc đã thỏa thuận trong hợp đồng.</p>
                <p>c) Không được trả công đầy đủ hoặc trả công không đúng thời hạn đã thoả thuận trong hợp đồng.</p>
                <p>d) Bị ngược đãi, bị cưỡng bức lao động.</p>
                <p>e) Bản thân hoặc gia đình thật sự có hoàn cảnh khó khăn không thể tiếp tục thực hiện hợp đồng.</p>
                <p>f) Được bầu làm nhiệm vụ chuyên trách ở các cơ quan dân cử hoặc được bổ nhiệm giữ chức vụ trong bộ máy Nhà nước.</p>
                <p>g) Người lao động nữ có thai phải nghỉ việc theo chỉ định của thầy thuốc.</p>
                <p>h) Người lao động bị ốm đau, tai nạn đã điều trị 03 tháng liền mà khả năng lao động chưa được hồi phục.</p>
                <p>i) Ngoài những căn cứ trên, người lao động còn phải đảm bảo thời hạn báo trước như sau:</p>
                <p>- Đối với các trường hợp quy định tại các điểm a, b, c và g: ít nhất 03 ngày;</p>
                <p>- Đối với các trường hợp quy định tại điểm d và điểm đ: ít nhất 30 ngày;</p>
                <p>- Đối với trường hợp quy định tại điểm e: theo thời hạn quy định tại Điều 112 của BLLĐ</p>
                <p>- Đối với các lý do khác, người lao động phải đảm bảo thông báo trước</p>
                <p>+ Ít nhất 45 ngày đối với hợp đồng lao động không xác định thời hạn.</p>
                <p>+ Ít nhất 30 ngày đối với hợp đồng lao động xác định thời hạn từ 01 - 03 năm.</p>
                <p>+ Ít nhất 03 ngày đối với hợp đồng lao động theo mùa vụ, theo một công việc nhất định mà thời hạn dưới 01 năm.</p>
                <p>k) Ngoài những căn cứ trên, người lao động còn phải đảm bảo thời hạn báo trước theo quy định. Người lao động có ý định thôi việc vì các lý do khác thì phải thông báo bằng văn bản cho đại diện của Công ty là Phòng Hành chính Nhân sự biết trước ít nhất là 15 ngày.</p>

                <p class='mt-4'><strong>Điều 6: Những thỏa thuận khác</strong></p>
                <p>Trong quá trình thực hiện hợp đồng nếu một bên có nhu cầu thay đổi nội dung trong hợp đồng phải báo cho bên kia trước ít nhất 03 ngày và ký kết bản Phụ lục hợp đồng theo quy định của Pháp luật. Trong thời gian tiến hành thỏa thuận hai bên vẫn tuân theo hợp đồng lao động đã ký kết.</p>
                <p>Người lao động đọc kỹ, hiểu rõ và cam kết thực hiện các điều khoản và quy định ghi tại Hợp đồng lao động.</p>

                <p class='mt-4'><strong>Điều 7: Điều khoản thi hành</strong></p>
                <p>Những vấn đề về lao động không ghi trong Hợp đồng lao động này thì áp dụng theo quy định của Thỏa ước tập thể, nội quy lao động và Pháp luật lao động.</p>
                <p>Khi hai bên ký kết Phụ lục hợp đồng lao động thì nội dung của Phụ lục hợp đồng lao động cũng có giá trị như các nội dung của bản hợp đồng này.</p>
                <p>Hợp đồng này được lập thành 02 (hai) bản có giá trị như nhau, Hành chính nhân sự giữ 01 (một) bản, Người lao động giữ 01 (một) bản và có hiệu lực kể từ ngày <span class='badge badge-light-primary fs-7 fw-bold px-2 py-1 align-middle'>{{Ngay_Ky}}</span> tháng <span class='badge badge-light-primary fs-7 fw-bold px-2 py-1 align-middle'>{{Thang_Ky}}</span> năm <span class='badge badge-light-primary fs-7 fw-bold px-2 py-1 align-middle'>{{Nam_Ky}}</span>.</p>
                <p>Hợp đồng được lập tại: <span class='badge badge-light-primary fs-7 fw-bold px-2 py-1 align-middle'>{{Dia_Diem_Ky}}</span></p>

                <table cellspacing='0' style='width:100%; border-collapse: collapse;' class='mt-10'>
                    <tbody>
                        <tr>
                            <td width='50%' style='text-align: center; vertical-align: top;'>
                                <p><strong>NGƯỜI LAO ĐỘNG</strong></p>
                                <p><em>(Ký, ghi rõ họ tên)</em></p>
                                <div style='height: 120px;'></div>
                                <p><strong><span class='badge badge-light-primary fs-7 fw-bold px-2 py-1 align-middle'>{{Ten_Nhan_Vien}}</span></strong></p>
                            </td>
                            <td width='50%' style='text-align: center; vertical-align: top;'>
                                <p><strong>NGƯỜI SỬ DỤNG LAO ĐỘNG</strong></p>
                                <p><em>(Ký, đóng dấu, ghi rõ họ tên)</em></p>
                                <div style='height: 120px;'></div>
                                <p><strong><span class='badge badge-light-primary fs-7 fw-bold px-2 py-1 align-middle'>{{Nguoi_Dai_Dien}}</span></strong></p>
                            </td>
                        </tr>
                    </tbody>
                </table>
            </div>";


        private async Task ShowPreview()
        {
            // 0. Get latest content from editor
            try 
            {
               TemplateContent = await JSRuntime.InvokeAsync<string>("getEditorContent");
            }
            catch (Exception) { /* If editor not ready, use existing TemplateContent */ }

            // 1. Reset and clone template
            string processedHtml = TemplateContent;

            // 2. Clean up the HTML from Metronic badges/styles for the printed document
            // This removes the <span class='badge...'> wrapper but keeps the content inside
            foreach(var item in MockData)
            {
                // Variable to replace
                var variablePlaceholder = "{{" + item.Key + "}}";
                var mockValue = item.Value;

                // Pattern to match the badge wrapper: <span class='...'>{{variable}}</span>
                // This is greedy to ensure we catch the entire wrapper span
                string regexPattern = @"<span[^>]*>\s*{{\s*" + item.Key + @"\s*}}\s*</span>";
                
                // Replace with highlighted bold text for a clear preview look
                processedHtml = Regex.Replace(processedHtml, regexPattern, $"<strong class='dynamic-value-highlight'>{mockValue}</strong>", RegexOptions.IgnoreCase);
                
                // Fallback for any variables not wrapped in spans
                processedHtml = processedHtml.Replace(variablePlaceholder, $"<strong class='dynamic-value-highlight'>{mockValue}</strong>");
            }

            // 3. Final sanitization: Remove any remaining badge classes or alignment styles that don't fit print
            // but keep the basic layout
            processedHtml = processedHtml.Replace("badge-light-primary", "");
            processedHtml = processedHtml.Replace("badge", "");

            PreviewContent = processedHtml;
            
            // Show modal via JS
            await JSRuntime.InvokeVoidAsync("showModal", "#preview_modal");
        }

        private async Task ShowHelp()
        {
            await JSRuntime.InvokeVoidAsync("showModal", "#help_modal");
        }

        private async Task SaveTemplate() { 
            Console.WriteLine("SaveTemplate called.");
            IsLoading = true;
            bool success = false;
            try
            {
                // Validation
                if (string.IsNullOrWhiteSpace(TemplateModel.Name))
                {
                     await JSRuntime.InvokeVoidAsync("Swal.fire", "Cảnh báo", "Vui lòng nhập tên mẫu hợp đồng.", "warning");
                     IsLoading = false;
                     return;
                }
                if (string.IsNullOrWhiteSpace(TemplateModel.Code))
                {
                     await JSRuntime.InvokeVoidAsync("Swal.fire", "Cảnh báo", "Vui lòng nhập số hiệu mẫu (Code).", "warning");
                     IsLoading = false;
                     return;
                }
                if (string.IsNullOrWhiteSpace(TemplateModel.TypeId))
                {
                     await JSRuntime.InvokeVoidAsync("Swal.fire", "Cảnh báo", "Vui lòng chọn loại hợp đồng.", "warning");
                     IsLoading = false;
                     return;
                }
                if (string.IsNullOrWhiteSpace(TemplateModel.StatusId))
                {
                     await JSRuntime.InvokeVoidAsync("Swal.fire", "Cảnh báo", "Vui lòng chọn trạng thái.", "warning");
                     IsLoading = false;
                     return;
                }

                // Get content from editor
                Console.WriteLine("Fetching editor content...");
                TemplateContent = await JSRuntime.InvokeAsync<string>("getEditorContent");
                Console.WriteLine($"Editor content retrieved. Length: {TemplateContent?.Length ?? 0}");
                
                TemplateModel.ContentHtml = TemplateContent;
                
                if (string.IsNullOrEmpty(Id))
                {
                    Console.WriteLine("Creating new template...");
                    var result = await ContractService.CreateTemplateAsync(new Entities.ContractTemplate {
                        Name = TemplateModel.Name,
                        Code = TemplateModel.Code,
                        Description = TemplateModel.Description,
                        TypeId = TemplateModel.TypeId,
                        StatusId = TemplateModel.StatusId,
                        ContentHtml = TemplateModel.ContentHtml,
                        Icon = TemplateModel.Icon,
                        Color = TemplateModel.Color
                    });
                    success = result != null;
                    Console.WriteLine($"Create result: {success}");
                }
                else
                {
                    Console.WriteLine($"Updating template. Param Id: {Id}, Model Id: {TemplateModel.Id}");
                    
                    if (string.IsNullOrEmpty(TemplateModel.Id))
                    {
                        await JSRuntime.InvokeVoidAsync("Swal.fire", "Lỗi", "Không tìm thấy ID của bản ghi để cập nhật. Vui lòng tải lại trang.", "error");
                        IsLoading = false;
                        return;
                    }

                    // Fix: Use TemplateModel.Id because Id parameter might be a Code (e.g. "HD-CTV")
                    // The backend needs the real ObjectId for updates
                    var result = await ContractService.UpdateTemplateAsync(TemplateModel.Id, new Entities.ContractTemplate {
                        Id = TemplateModel.Id,
                        Name = TemplateModel.Name,
                        Code = TemplateModel.Code,
                        Description = TemplateModel.Description,
                        TypeId = TemplateModel.TypeId,
                        StatusId = TemplateModel.StatusId,
                        ContentHtml = TemplateModel.ContentHtml,
                        Icon = TemplateModel.Icon,
                        Color = TemplateModel.Color
                    });
                    success = result != null;
                    Console.WriteLine($"Update result: {success}");
                }

                if (!success)
                {
                    await JSRuntime.InvokeVoidAsync("Swal.fire", "Lỗi", "Không thể lưu mẫu hợp đồng (API trả về null). Vui lòng thử lại.", "error");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving template: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
                await JSRuntime.InvokeVoidAsync("Swal.fire", "Lỗi", $"Đã xảy ra lỗi: {ex.Message}", "error");
                success = false;
            }
            finally
            {
                IsLoading = false;
                StateHasChanged();
            }

            if (success)
            {
                await JSRuntime.InvokeVoidAsync("Swal.fire", "Thành công", "Đã lưu mẫu hợp đồng.", "success");
                Navigation.NavigateTo("/contracts"); 
            }
        }
    }
}
