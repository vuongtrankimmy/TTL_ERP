const { MongoClient, ObjectId } = require('mongodb');

const url = 'mongodb://127.0.0.1:27030/?directConnection=true';
const client = new MongoClient(url);
const dbName = 'HR';

async function main() {
    try {
        await client.connect();
        const db = client.db(dbName);
        const collection = db.collection('leave_requests');

        // Clear existing test data
        await collection.deleteMany({});

        const leaveTypes = [
            { id: "65ebf3a40000000000000101", name: "Nghỉ phép năm", color: "primary" },
            { id: "65ebf3a40000000000000102", name: "Nghỉ ốm (Hưởng BHXH)", color: "warning" },
            { id: "65ebf3a40000000000000103", name: "Nghỉ không lương", color: "danger" },
            { id: "65ebf3a40000000000000104", name: "Đi công tác", color: "info" }
        ];

        const employees = [
            { id: "69912b1b13b75cbbfcbbb5d1", name: "Đặng Văn G 7", dept: "Kỹ thuật" },
            { id: "69912b1b13b75cbbfcbbb5d2", name: "Bùi Thị H 8", dept: "Marketing" },
            { id: "69912b1b13b75cbbfcbbb5d4", name: "Lý Thị K 10", dept: "Sản xuất" },
            { id: "69912b1b13b75cbbfcbbb5d5", name: "Trịnh Văn L 11", dept: "Kế toán" },
            { id: "69912b1b13b75cbbfcbbb5d6", name: "Đỗ Thị M 12", dept: "Nhân sự" },
            { id: "69912b1b13b75cbbfcbbb5d7", name: "Hồ Văn N 13", dept: "Kho" },
            { id: "69912b1b13b75cbbfcbbb5d8", name: "Huỳnh Thị O 14", dept: "Kỹ thuật" },
            { id: "69912b1b13b75cbbfcbbb5d9", name: "Dương Văn P 15", dept: "Marketing" },
            { id: "69912b1b13b75cbbfcbbb5da", name: "Võ Thị Q 16", dept: "Sản xuất" },
            { id: "69912b1b13b75cbbfcbbb5db", name: "Ngô Văn R 17", dept: "IT" },
            { id: "69912b1b13b75cbbfcbbb5dc", name: "Cao Thị S 18", dept: "Dịch vụ" },
            { id: "69912b1b13b75cbbfcbbb5dd", name: "Đinh Văn T 19", dept: "Kỹ thuật" }
        ];

        const reasons = [
            "Giải quyết việc riêng gia đình",
            "Bị sốt xuất huyết, cần nghỉ điều trị",
            "Đi đám cưới em ruột ở quê",
            "Đi công tác triển khai dự án tại Đà Nẵng",
            "Nghỉ phép đi du lịch cùng gia đình",
            "Đưa con đi khám bệnh định kỳ",
            "Xử lý việc ở quê có giỗ",
            "Gặp gỡ đối tác khách hàng tại HCM",
            "Nghỉ phép năm tái tạo năng lượng",
            "Bận việc gia đình đột xuất",
            "Khám sức khỏe tổng quát",
            "Đi dự hội thảo chuyên môn"
        ];

        const samples = [];
        const now = new Date();

        for (let i = 0; i < employees.length; i++) {
            const emp = employees[i];
            const type = leaveTypes[i % leaveTypes.length];
            const status = i < 5 ? "Pending" : (i < 9 ? "Approved" : "Rejected");

            const startDate = new Date(2026, 1, 20 + i);
            const endDate = new Date(startDate);
            endDate.setDate(startDate.getDate() + (i % 3) + 1);

            const req = {
                EmployeeId: new ObjectId(emp.id),
                EmployeeName: emp.name,
                EmployeeCode: `EMP-200${i + 7}`,
                DepartmentName: emp.dept,
                LeaveTypeId: new ObjectId(type.id),
                LeaveTypeName: type.name,
                LeaveTypeColor: type.color,
                StartDate: startDate,
                EndDate: endDate,
                TotalDays: (i % 3) + 1,
                Reason: reasons[i],
                Status: status,
                IsDeleted: false,
                CreatedAt: new Date(now.getTime() - (i * 3600000)),
                UpdatedAt: new Date(),
                CurrentLevel: status === "Pending" ? 0 : 2,
                TotalLevelsRequired: 2
            };

            if (status !== "Pending") {
                req.ApprovedBy = new ObjectId("65d48721fabcde0002000000");
                req.ApprovedByName = "Nguyễn Văn Quản Lý";
                req.ApprovedAt = new Date(now.getTime() - (i * 1800000));
                req.Comment = status === "Approved" ? "Duyệt. Nhớ bàn giao công việc đầy đủ." : "Không duyệt do đang trong giai đoạn cao điểm dự án.";
            }

            samples.push(req);
        }

        const result = await collection.insertMany(samples);
        console.log(`${result.insertedCount} leave requests were inserted.`);

    } catch (err) {
        console.error(err);
    } finally {
        await client.close();
    }
}

main();
