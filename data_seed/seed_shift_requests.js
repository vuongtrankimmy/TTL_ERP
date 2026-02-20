const { MongoClient, ObjectId } = require('mongodb');

const url = 'mongodb://127.0.0.1:27030/?directConnection=true';
const client = new MongoClient(url);
const dbName = 'HR';

async function main() {
    try {
        await client.connect();
        const db = client.db(dbName);
        const collection = db.collection('shift_change_requests');

        // Clear existing test data to avoid duplicates if running multiple times
        // await collection.deleteMany({ Reason: { $regex: "test" } });

        const shifts = [
            { id: "65ebf3a40000000000000001", name: "Ca Hành Chính" },
            { id: "65ebf3a40000000000000003", name: "Ca Sáng (Kho/SX)" },
            { id: "65ebf3a40000000000000004", name: "Ca Chiều (Kho/SX)" },
            { id: "65ebf3a40000000000000005", name: "Ca Đêm (Bảo Vệ)" }
        ];

        const employees = [
            { id: "69912b1b13b75cbbfcbbb5d1", name: "Đặng Văn G 7" },
            { id: "69912b1b13b75cbbfcbbb5d2", name: "Bùi Thị H 8" },
            { id: "69912b1b13b75cbbfcbbb5d4", name: "Lý Thị K 10" },
            { id: "69912b1b13b75cbbfcbbb5d5", name: "Trịnh Văn L 11" },
            { id: "69912b1b13b75cbbfcbbb5d6", name: "Đỗ Thị M 12" },
            { id: "69912b1b13b75cbbfcbbb5d7", name: "Hồ Văn N 13" },
            { id: "69912b1b13b75cbbfcbbb5d8", name: "Huỳnh Thị O 14" },
            { id: "69912b1b13b75cbbfcbbb5d9", name: "Dương Văn P 15" },
            { id: "69912b1b13b75cbbfcbbb5da", name: "Võ Thị Q 16" },
            { id: "69912b1b13b75cbbfcbbb5db", name: "Ngô Văn R 17" }
        ];

        const reasons = [
            "Con ốm cần về sớm đi khám bệnh",
            "Đổi ca để đi học thêm kỹ năng buổi tối",
            "Nhà có giỗ, xin đổi ca để về quê",
            "Hỗ trợ đồng nghiệp đổi ca do họ bận việc riêng",
            "Muốn làm ca đêm để hưởng phụ cấp đêm",
            "Sức khỏe không tốt, xin đổi sang ca hành chính",
            "Có lịch hẹn nha khoa vào buổi chiều",
            "Cần thời gian giải quyết thủ tục giấy tờ ở phường",
            "Muốn thay đổi môi trường làm việc giữa các ca",
            "Đổi ca để tham gia hoạt động đoàn thể của công ty"
        ];

        const samples = [];
        const now = new Date();

        for (let i = 0; i < employees.length; i++) {
            const emp = employees[i];
            const fromShift = shifts[i % shifts.length];
            const toShift = shifts[(i + 1) % shifts.length];
            const status = i < 4 ? "Pending" : (i < 8 ? "Approved" : "Rejected");

            const req = {
                EmployeeId: new ObjectId(emp.id),
                EmployeeName: emp.name,
                Date: new Date(2026, 1, 20 + i), // Spread dates
                FromShiftId: new ObjectId(fromShift.id),
                FromShiftName: fromShift.name,
                ToShiftId: new ObjectId(toShift.id),
                ToShiftName: toShift.name,
                Reason: reasons[i],
                Status: status,
                IsDeleted: false,
                CreatedAt: new Date(now.getTime() - (i * 3600000)), // Spread creation time
                UpdatedAt: new Date()
            };

            if (status !== "Pending") {
                req.ApproverId = new ObjectId("65d48721fabcde0002000000");
                req.ApproverName = "Nguyễn Văn Quản Lý";
                req.ApprovedAt = new Date(now.getTime() - (i * 1800000));
                req.Comment = status === "Approved" ? "Duyệt. Nhớ bàn giao công việc." : "Không duyệt do thiếu nhân sự ca này.";
            }

            samples.push(req);
        }

        const result = await collection.insertMany(samples);
        console.log(`${result.insertedCount} documents were inserted.`);

    } catch (err) {
        console.error(err);
    } finally {
        await client.close();
    }
}

main();
