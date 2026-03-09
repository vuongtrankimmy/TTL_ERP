const { MongoClient, ObjectId } = require('mongodb');

const url = 'mongodb://127.0.0.1:27030/?directConnection=true';
const dbName = 'HR';
const client = new MongoClient(url);

const BILINGUAL_STYLE = `
<style>
    .contract-container { font-family: 'Inter', sans-serif; line-height: 1.6; color: #181C32; max-width: 800px; margin: auto; padding: 20px; }
    .header { text-align: center; margin-bottom: 30px; }
    .section-title { font-weight: bold; text-transform: uppercase; margin-top: 20px; border-bottom: 1px solid #E4E6EF; padding-bottom: 5px; }
    .row-item { margin-bottom: 10px; display: flex; flex-wrap: wrap; }
    .label { font-weight: 600; width: 200px; }
    .value { flex: 1; }
    .token-badge { background-color: #F1FAFF; color: #009EF7; font-weight: bold; padding: 2px 6px; border-radius: 4px; border: 1px solid #d0eaff; }
    .en-text { font-style: italic; color: #7E8299; font-size: 0.9em; display: block; }
    .signature-area { display: flex; justify-content: space-around; margin-top: 50px; text-align: center; }
</style>
`;

const SHARED_HEADER = `
<div class="header">
    <p><strong>CỘNG HÒA XÃ HỘI CHỦ NGHĨA VIỆT NAM</strong></p>
    <p class="en-text">SOCIALIST REPUBLIC OF VIETNAM</p>
    <p><strong>Độc lập – Tự do – Hạnh phúc</strong></p>
    <p class="en-text">Independence – Freedom – Happiness</p>
    <hr>
    <p style="text-align:right">
        <span class="token-badge">{{Dia_Diem_Ky}}</span>, ngày/date <span class="token-badge">{{Ngay_Ky}}</span> tháng/month <span class="token-badge">{{Thang_Ky}}</span> năm/year <span class="token-badge">{{Nam_Ky}}</span>
    </p>
</div>
`;

const PROBATION_CONTENT = `
${BILINGUAL_STYLE}
<div class="contract-container">
    ${SHARED_HEADER}
    <div class="header">
        <h2 style="margin-bottom:0">HỢP ĐỒNG THỬ VIỆC</h2>
        <h3 class="en-text" style="margin-top:0">PROBATIONARY CONTRACT</h3>
        <p>Số/No: <span class="token-badge">{{Ma_Hop_Dong}}</span>/HDTV</p>
    </div>

    <div class="section-title">BÊN A: NGƯỜI SỬ DỤNG LAO ĐỘNG (EMPLOYER)</div>
    <div class="row-item"><span class="label">Tổ chức (Organization):</span> <span class="value"><span class="token-badge">{{Ten_Cong_Ty}}</span></span></div>
    <div class="row-item"><span class="label">Đại diện (Represented by):</span> <span class="value"><span class="token-badge">{{Nguoi_Dai_Dien}}</span></span></div>
    <div class="row-item"><span class="label">Chức vụ (Position):</span> <span class="value"><span class="token-badge">{{Chuc_Vu_Nguoi_Dai_Dien}}</span></span></div>
    <div class="row-item"><span class="label">Địa chỉ (Address):</span> <span class="value"><span class="token-badge">{{Dia_Chi_Cong_Ty}}</span></span></div>

    <div class="section-title">BÊN B: NGƯỜI LAO ĐỘNG (EMPLOYEE)</div>
    <div class="row-item"><span class="label">Họ tên (Full Name):</span> <span class="value"><span class="token-badge">{{Ten_Nhan_Vien}}</span></span></div>
    <div class="row-item"><span class="label">Ngày sinh (DOB):</span> <span class="value"><span class="token-badge">{{Ngay_Sinh}}</span></span></div>
    <div class="row-item"><span class="label">Số CCCD (ID No):</span> <span class="value"><span class="token-badge">{{So_CCCD}}</span></span></div>
    <div class="row-item"><span class="label">Địa chỉ (Address):</span> <span class="value"><span class="token-badge">{{Dia_Chi_Thuong_Tru}}</span></span></div>

    <div class="section-title">ĐIỀU 1: CÔNG VIỆC VÀ THỜI HẠN (ARTICLE 1: JOB AND DURATION)</div>
    <p>1.1. Vị trí công việc (Job Position): <span class="token-badge">{{Chuc_Vu}}</span> tại phòng <span class="token-badge">{{Phong_Ban}}</span></p>
    <p>1.2. Thời gian thử việc (Probation Period): <span class="token-badge">{{Thoi_Han_Hop_Dong}}</span> tháng từ <span class="token-badge">{{Ngay_Bat_Dau}}</span> đến <span class="token-badge">{{Ngay_Ket_Thuc}}</span></p>

    <div class="section-title">ĐIỀU 2: LƯƠNG VÀ PHÚC LỢI (ARTICLE 2: SALARY AND BENEFITS)</div>
    <p>2.1. Mức lương thử việc (Probation Salary): <span class="token-badge">{{Muc_Luong}}</span> VND/tháng.</p>
    <p>2.2. Phụ cấp (Allowances): <span class="token-badge">{{Phu_Cap}}</span> VND/tháng.</p>

    <div class="signature-area">
        <div>
            <p><strong>NGƯỜI LAO ĐỘNG</strong></p>
            <p class="en-text">EMPLOYEE</p>
            <br><br><br>
            <p><strong>{{Ten_Nhan_Vien}}</strong></p>
        </div>
        <div>
            <p><strong>NGƯỜI SỬ DỤNG LAO ĐỘNG</strong></p>
            <p class="en-text">EMPLOYER</p>
            <br><br><br>
            <p><strong>{{Nguoi_Dai_Dien}}</strong></p>
        </div>
    </div>
</div>
`;

const DEFINITE_CONTENT = `
${BILINGUAL_STYLE}
<div class="contract-container">
    ${SHARED_HEADER}
    <div class="header">
        <h2 style="margin-bottom:0">HỢP ĐỒNG LAO ĐỘNG XÁC ĐỊNH THỜI HẠN</h2>
        <h3 class="en-text" style="margin-top:0">DEFINITE TERM LABOR CONTRACT</h3>
        <p>Số/No: <span class="token-badge">{{Ma_Hop_Dong}}</span>/HĐLĐ</p>
    </div>

    <div class="section-title">THÔNG TIN CÁC BÊN (PARTIES INFORMATION)</div>
    <p><strong>Bên A (Employer):</strong> <span class="token-badge">{{Ten_Cong_Ty}}</span></p>
    <p><strong>Bên B (Employee):</strong> <span class="token-badge">{{Ten_Nhan_Vien}}</span> - MSNV: <span class="token-badge">{{Ma_Nhan_Vien}}</span></p>

    <div class="section-title">ĐIỀU 1: THỜI HẠN HỢP ĐỒNG (ARTICLE 1: CONTRACT DURATION)</div>
    <p>Loại hợp đồng (Contract Type): <span class="token-badge">{{Loai_Hop_Dong}}</span></p>
    <p>Thời hạn (Duration): <span class="token-badge">{{Thoi_Han_Hop_Dong}}</span> tháng có hiệu lực từ <span class="token-badge">{{Ngay_Bat_Dau}}</span></p>

    <div class="section-title">ĐIỀU 2: CHẾ ĐỘ LÀM VIỆC (ARTICLE 2: WORKING CONDITIONS)</div>
    <p>Địa điểm làm việc (Workplace): <span class="token-badge">{{Dia_Diem_Lam_Viec}}</span></p>
    <p>Thời gian làm việc (Working Time): <span class="token-badge">{{Thoi_Gian_Lam_Viec}}</span></p>

    <div class="section-title">ĐIỀU 3: QUYỀN LỢI (ARTICLE 3: BENEFITS)</div>
    <p>Lương cơ bản (Basic Salary): <span class="token-badge">{{Muc_Luong}}</span> VND/tháng.</p>

    <div class="signature-area">
        <div>
            <p><strong>BÊN B / EMPLOYEE</strong></p>
            <br><br><br>
            <p><strong>{{Ten_Nhan_Vien}}</strong></p>
        </div>
        <div>
            <p><strong>BÊN A / EMPLOYER</strong></p>
            <br><br><br>
            <p><strong>{{Nguoi_Dai_Dien}}</strong></p>
        </div>
    </div>
</div>
`;

async function main() {
    try {
        await client.connect();
        const db = client.db(dbName);
        const lookups = await db.collection('lookups').find({ Type: { $in: ["ContractType", "TemplateStatus"] } }).toArray();

        const getLookupID = (type, code) => lookups.find(l => l.Type === type && l.Code === code)?.LookupID;

        const probationTypeId = getLookupID("ContractType", "Probation");
        const definiteTypeId = getLookupID("ContractType", "Definite");
        const indefiniteTypeId = getLookupID("ContractType", "Indefinite");
        const activeStatusId = getLookupID("TemplateStatus", "Active");

        const templates = [
            {
                _id: new ObjectId(),
                Name: "Mẫu Hợp đồng Thử việc (Bilingual)",
                Code: "PROBATION_BILINGUAL",
                Description: "Mẫu hợp đồng thử việc song ngữ Việt - Anh chuẩn mực.",
                TypeId: probationTypeId,
                StatusId: activeStatusId,
                ContentHtml: PROBATION_CONTENT,
                Icon: "bi bi-file-earmark-person",
                Color: "primary",
                IsDeleted: false,
                CreatedAt: new Date(),
                UpdatedAt: new Date()
            },
            {
                _id: new ObjectId(),
                Name: "Mẫu Hợp đồng Lao động xác định thời hạn (Bilingual)",
                Code: "DEFINITE_BILINGUAL",
                Description: "Mẫu hợp đồng lao động xác định thời hạn song ngữ.",
                TypeId: definiteTypeId,
                StatusId: activeStatusId,
                ContentHtml: DEFINITE_CONTENT,
                Icon: "bi bi-file-earmark-check",
                Color: "success",
                IsDeleted: false,
                CreatedAt: new Date(),
                UpdatedAt: new Date()
            },
            {
                _id: new ObjectId(),
                Name: "Mẫu Hợp đồng Lao động không xác định thời hạn (Bilingual)",
                Code: "INDEFINITE_BILINGUAL",
                Description: "Mẫu hợp đồng lao động không xác định thời hạn song ngữ chuẩn mực.",
                TypeId: indefiniteTypeId,
                StatusId: activeStatusId,
                ContentHtml: DEFINITE_CONTENT.replace("LAO ĐỘNG XÁC ĐỊNH THỜI HẠN", "LAO ĐỘNG KHÔNG XÁC ĐỊNH THỜI HẠN").replace("DEFINITE TERM", "INDEFINITE TERM"),
                Icon: "bi bi-file-earmark-medical",
                Color: "danger",
                IsDeleted: false,
                CreatedAt: new Date(),
                UpdatedAt: new Date()
            }
        ];

        const templateCol = db.collection('contract_templates');
        const translateCol = db.collection('contract_templates_translate');

        await templateCol.deleteMany({ Code: { $in: ["PROBATION_BILINGUAL", "DEFINITE_BILINGUAL", "INDEFINITE_BILINGUAL"] } });
        await templateCol.insertMany(templates);

        const translates = [];
        for (const t of templates) {
            // VI Translation
            translates.push({
                ContractTemplateId: t._id.toString(),
                LanguageCode: "vi-VN",
                Name: t.Name,
                Description: t.Description,
                ContentHtml: t.ContentHtml,
                IsDeleted: false,
                CreatedAt: new Date(),
                UpdatedAt: new Date()
            });
            // EN Translation (For now same content as it's already bilingual, but can be customized)
            translates.push({
                ContractTemplateId: t._id.toString(),
                LanguageCode: "en-US",
                Name: t.Name.replace("(Bilingual)", "(Song ngữ)"),
                Description: "Professional bilingual English-Vietnamese probationary contract template.",
                ContentHtml: t.ContentHtml,
                IsDeleted: false,
                CreatedAt: new Date(),
                UpdatedAt: new Date()
            });
        }

        await translateCol.deleteMany({ ContractTemplateId: { $in: templates.map(t => t._id.toString()) } });
        await translateCol.insertMany(translates);

        console.log("Seeded professional bilingual contract templates successfully.");
    } catch (err) {
        console.error("Seeding Error:", err);
    } finally {
        await client.close();
    }
}

main();
