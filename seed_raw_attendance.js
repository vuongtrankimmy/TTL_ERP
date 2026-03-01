
const fs = require('fs');
const path = require('path');
const { MongoClient, ObjectId } = require('mongodb');

async function seedRawAttendance() {
    const uri = "mongodb://localhost:27017";
    const client = new MongoClient(uri);

    try {
        await client.connect();
        const db = client.db("HR");
        const employeesColl = db.collection("employees");
        const attendanceColl = db.collection("attendances");

        const monthNames = ["jan", "feb", "mar"];
        const year = 2026;

        // Load employee mapping
        const employees = await employeesColl.find({ IsDeleted: { $ne: true } }).toArray();
        const empMap = new Map();
        employees.forEach(e => {
            empMap.set(e.Code, {
                id: e._id.toString(),
                name: e.FullName,
                code: e.Code
            });
        });

        console.log(`Loaded ${empMap.size} employees for mapping.`);

        for (const mName of monthNames) {
            const fileName = `attendance_${mName}_2026.csv`;
            const filePath = path.join('d:\\MONEY\\2026\\TAN_TAN_LOC\\TTL_ERP', fileName);

            if (!fs.existsSync(filePath)) {
                console.warn(`File not found: ${filePath}`);
                continue;
            }

            const content = fs.readFileSync(filePath, 'utf8').replace(/^\ufeff/, '');
            const lines = content.split(/\r?\n/).filter(line => line.trim() !== "");

            // Skip header
            const dataLines = lines.slice(1);
            const batch = [];

            dataLines.forEach(line => {
                // simple csv split by quote and comma
                const parts = line.split('","').map(p => p.replace(/"/g, ''));
                if (parts.length < 13) return;

                const [logId, empCode, empName, dept, method, action, devId, devName, timeStr, loc, lat, lng, confidence] = parts;

                const emp = empMap.get(empCode);
                if (!emp) return;

                // Parse time dd/MM/yyyy HH:mm:ss
                const [dPart, tPart] = timeStr.split(' ');
                const [d, m, y] = dPart.split('/').map(Number);
                const [h, min, s] = tPart.split(':').map(Number);

                // Attendance date is local, but store as Date object (Mongo stores as UTC)
                // We'll treat this as UTC for now or adjust based on system preference. 
                // In clean architecture the handler usually converts local to UTC.
                // Here we simulate the raw data storage which often uses a specific zone.
                const localDate = new Date(y, m - 1, d, h, min, s);
                // System uses "SE Asia Standard Time" (UTC+7)
                // To get the equivalent UTC Date for Mongo:
                const utcDate = new Date(localDate.getTime() - (7 * 60 * 60 * 1000));

                const attendance = {
                    EmployeeId: new ObjectId(emp.id),
                    EmployeeName: emp.name,
                    EmployeeCode: emp.code,
                    Date: new Date(Date.UTC(y, m - 1, d)), // Consistent date only field
                    CheckIn: action === "Giờ vào" ? utcDate : null,
                    CheckOut: action === "Giờ ra" ? utcDate : null,
                    Status: "RawData",
                    Source: "Machine",
                    Method: method,
                    DeviceName: devName,
                    Latitude: parseFloat(lat),
                    Longitude: parseFloat(lng),
                    Confidence: parseFloat(confidence),
                    CreatedAt: new Date(),
                    IsDeleted: false,
                    ExtraData: {
                        log_id: logId,
                        department: dept,
                        device_id: devId,
                        location: loc
                    }
                };

                batch.push(attendance);
            });

            if (batch.length > 0) {
                const result = await attendanceColl.insertMany(batch);
                console.log(`Inserted ${result.insertedCount} raw records from ${fileName}`);
            }
        }
    } finally {
        await client.close();
    }
}

seedRawAttendance().catch(console.error);
