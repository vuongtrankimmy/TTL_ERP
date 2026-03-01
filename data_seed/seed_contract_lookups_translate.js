const { MongoClient } = require('mongodb');

const url = 'mongodb://127.0.0.1:27030/?directConnection=true';
const dbName = 'HR';
const client = new MongoClient(url);

const NAMES = {
    // ContractType
    Probation: { vi: 'Hợp đồng thử việc', en: 'Probation Contract' },
    Definite: { vi: 'Hợp đồng xác định thời hạn', en: 'Fixed-Term Contract' },
    Indefinite: { vi: 'Hợp đồng không xác định thời hạn', en: 'Indefinite Contract' },
    Seasonal: { vi: 'Hợp đồng theo mùa vụ', en: 'Seasonal Contract' },
    PartTime: { vi: 'Hợp đồng bán thời gian', en: 'Part-Time Contract' },
    Freelance: { vi: 'Hợp đồng dịch vụ/freelance', en: 'Freelance Contract' },
    Internship: { vi: 'Hợp đồng thực tập', en: 'Internship Contract' },
    TemporaryStaff: { vi: 'Nhân viên tạm thời', en: 'Temporary Staff' },
    Outsourcing: { vi: 'Hợp đồng outsourcing', en: 'Outsourcing Contract' },
    ProjectBased: { vi: 'Hợp đồng theo dự án', en: 'Project-Based Contract' },
    Consultant: { vi: 'Hợp đồng tư vấn', en: 'Consulting Contract' },
    OneDayProbation: { vi: 'Hợp đồng thử việc 1 ngày', en: '1-Day Probation Contract' },
    //ContractStatus
    Active: { vi: 'Đang hiệu lực', en: 'Active' },
    Expired: { vi: 'Hết hạn', en: 'Expired' },
    Terminated: { vi: 'Đã chấm dứt', en: 'Terminated' },
    Draft: { vi: 'Nháp', en: 'Draft' },
    PendingRenewal: { vi: 'Chờ gia hạn', en: 'Pending Renewal' },
    // TemplateStatus
    Published: { vi: 'Đang dùng', en: 'Published' },
    Archived: { vi: 'Lưu trữ', en: 'Archived' },
    Deprecated: { vi: 'Ngừng sử dụng', en: 'Deprecated' },
};

async function main() {
    try {
        await client.connect();
        const db = client.db(dbName);

        const lookups = await db.collection('lookups').find({
            Type: { $in: ['ContractType', 'ContractStatus', 'TemplateStatus'] }
        }).toArray();

        console.log(`Found ${lookups.length} contract-related lookups`);

        const existingTranslates = await db.collection('lookups_translate').find({
            LookupId: { $in: lookups.map(l => l._id.toString()) }
        }).toArray();

        console.log(`Existing translates for these lookups: ${existingTranslates.length}`);

        const toInsert = [];
        for (const lookup of lookups) {
            const names = NAMES[lookup.Code];
            if (!names) {
                console.warn(`  No translation mapping for Code: ${lookup.Code} (Type: ${lookup.Type})`);
                continue;
            }

            // Check if already has vi-VN
            const hasVi = existingTranslates.some(t => t.LookupId === lookup._id.toString() && t.LanguageCode === 'vi-VN');
            const hasEn = existingTranslates.some(t => t.LookupId === lookup._id.toString() && t.LanguageCode === 'en-US');

            if (!hasVi) {
                toInsert.push({
                    LookupId: lookup._id.toString(),
                    LanguageCode: 'vi-VN',
                    Name: names.vi,
                    IsDeleted: false,
                    CreatedAt: new Date(),
                    UpdatedAt: new Date()
                });
            }
            if (!hasEn) {
                toInsert.push({
                    LookupId: lookup._id.toString(),
                    LanguageCode: 'en-US',
                    Name: names.en,
                    IsDeleted: false,
                    CreatedAt: new Date(),
                    UpdatedAt: new Date()
                });
            }
        }

        if (toInsert.length > 0) {
            await db.collection('lookups_translate').insertMany(toInsert);
            console.log(`Inserted ${toInsert.length} lookup translations for contract types/statuses.`);
        } else {
            console.log('All translations already exist, nothing to insert.');
        }

        console.log('Done!');
    } catch (err) {
        console.error('Error:', err);
    } finally {
        await client.close();
    }
}

main();
