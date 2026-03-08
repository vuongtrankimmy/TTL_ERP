// Script: seed_asset_categories.js
// Upserts AssetCategories and AssetStatus lookups directly into MongoDB
// Run: node seed_asset_categories.js

const { MongoClient, ObjectId } = require('mongodb');

const MONGO_URI = process.env.MONGO_URI || 'mongodb://localhost:27017';
const DB_NAME = process.env.DB_NAME || 'ttl_erp';

const categories = [
    { _id: new ObjectId('65f3c3c000000000000001'), Name: 'Máy tính xách tay (Laptop)', Code: 'Laptop', Description: 'Thiết bị máy tính xách tay cho nhân viên', CreatedAt: new Date(), IsDeleted: false },
    { _id: new ObjectId('65f3c3c000000000000002'), Name: 'Máy tính để bàn (PC)', Code: 'Desktop', Description: 'Hệ thống máy tính để bàn (Workstation)', CreatedAt: new Date(), IsDeleted: false },
    { _id: new ObjectId('65f3c3c000000000000003'), Name: 'Màn hình', Code: 'Monitor', Description: 'Màn hình hiển thị các loại', CreatedAt: new Date(), IsDeleted: false },
    { _id: new ObjectId('65f3c3c000000000000004'), Name: 'Điện thoại', Code: 'Phone', Description: 'Điện thoại thông minh/Điện thoại bàn', CreatedAt: new Date(), IsDeleted: false },
    { _id: new ObjectId('65f3c3c000000000000005'), Name: 'Phương tiện đi lại', Code: 'Vehicle', Description: 'Xe máy, ô tô công ty', CreatedAt: new Date(), IsDeleted: false },
    { _id: new ObjectId('65f3c3c000000000000006'), Name: 'Nội thất văn phòng', Code: 'Furniture', Description: 'Bàn, ghế, tủ hồ sơ', CreatedAt: new Date(), IsDeleted: false },
    { _id: new ObjectId('65f3c3c000000000000007'), Name: 'Thiết bị văn phòng', Code: 'Office', Description: 'Máy in, máy scan, máy chiếu', CreatedAt: new Date(), IsDeleted: false },
    { _id: new ObjectId('65f3c3c000000000000008'), Name: 'Khác', Code: 'Other', Description: 'Các loại tài sản khác', CreatedAt: new Date(), IsDeleted: false },
];

const assetStatusLookups = [
    { _id: new ObjectId('65dae2f300000000000901'), Type: 'AssetStatus', Code: 'Available', Name: 'Sẵn dùng', Order: 1, IsActive: true, LookupID: 1 },
    { _id: new ObjectId('65dae2f300000000000902'), Type: 'AssetStatus', Code: 'Assigned', Name: 'Đã bàn giao', Order: 2, IsActive: true, LookupID: 2 },
    { _id: new ObjectId('65dae2f300000000000903'), Type: 'AssetStatus', Code: 'Maintenance', Name: 'Đang bảo trì', Order: 3, IsActive: true, LookupID: 3 },
    { _id: new ObjectId('65dae2f300000000000904'), Type: 'AssetStatus', Code: 'Broken', Name: 'Đã hỏng', Order: 4, IsActive: true, LookupID: 4 },
    { _id: new ObjectId('65dae2f300000000000905'), Type: 'AssetStatus', Code: 'Lost', Name: 'Đã mất', Order: 5, IsActive: true, LookupID: 5 },
];

async function main() {
    const client = new MongoClient(MONGO_URI);
    await client.connect();
    console.log('✅ Connected to MongoDB:', DB_NAME);

    const db = client.db(DB_NAME);

    // Upsert asset_categories
    const catColl = db.collection('asset_categories');
    let catUpserted = 0;
    for (const cat of categories) {
        const result = await catColl.replaceOne({ _id: cat._id }, cat, { upsert: true });
        if (result.upsertedCount > 0 || result.modifiedCount > 0) catUpserted++;
    }
    console.log(`✅ asset_categories: upserted/updated ${catUpserted}/${categories.length}`);

    // Upsert AssetStatus lookups
    const lookupColl = db.collection('lookups');
    // Remove existing AssetStatus entries to avoid duplicates on re-run
    await lookupColl.deleteMany({ Type: 'AssetStatus' });
    const insertResult = await lookupColl.insertMany(assetStatusLookups);
    console.log(`✅ lookups (AssetStatus): inserted ${insertResult.insertedCount}`);

    // Also fix LookupIDs for the newly inserted ones
    const allLookups = await lookupColl.find({}).toArray();
    const grouped = {};
    for (const l of allLookups) {
        if (!grouped[l.Type]) grouped[l.Type] = [];
        grouped[l.Type].push(l);
    }

    for (const [type, items] of Object.entries(grouped)) {
        const sorted = items.sort((a, b) => (a.Order || 0) - (b.Order || 0));
        for (let i = 0; i < sorted.length; i++) {
            const expected = i + 1;
            if (sorted[i].LookupID !== expected) {
                await lookupColl.updateOne({ _id: sorted[i]._id }, { $set: { LookupID: expected } });
            }
        }
    }
    console.log('✅ LookupIDs synchronized for all types.');

    await client.close();
    console.log('🎉 Done! Restart your API server for changes to take effect.');
}

main().catch(err => {
    console.error('❌ Error:', err.message);
    process.exit(1);
});
