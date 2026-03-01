
const fs = require('fs');
const path = require('path');

const employees = [
    ["EMP-2001", "Nguyễn Văn A 1 (Test)", "Engineering"],
    ["EMP-2002", "Trần Thị B 2", "Engineering"],
    ["EMP-2003", "Lê Văn C 3", "Engineering"],
    ["EMP-2004", "Phạm Thị D 4", "Engineering"],
    ["EMP-2007", "Đặng Văn G 7", "Engineering"],
    ["EMP-2008", "Bùi Thị H 8", "Engineering"],
    ["EMP-2010", "Lý Thị K 10", "Engineering"],
    ["EMP-2011", "Trịnh Văn L 11", "Engineering"],
    ["EMP-2012", "Đỗ Thị M 12", "Engineering"],
    ["EMP-2013", "Hồ Văn N 13", "Engineering"],
    ["EMP-2014", "Huỳnh Thị O 14", "Engineering"],
    ["EMP-2015", "Dương Văn P 15", "Engineering"]
];

const year = 2026;
const monthNames = ["jan", "feb", "mar"];

function formatTime(dateObj) {
    const day = dateObj.getDate().toString().padStart(2, '0');
    const month = (dateObj.getMonth() + 1).toString().padStart(2, '0');
    const yr = dateObj.getFullYear();
    const h = dateObj.getHours().toString().padStart(2, '0');
    const m = dateObj.getMinutes().toString().padStart(2, '0');
    const s = dateObj.getSeconds().toString().padStart(2, '0');
    return `${day}/${month}/${yr} ${h}:${m}:${s}`;
}

monthNames.forEach((mName, index) => {
    const month = index + 1;
    const fileName = `attendance_${mName}_2026.csv`;
    const filePath = path.join('d:\\MONEY\\2026\\TAN_TAN_LOC\\TTL_ERP', fileName);

    const logs = [];
    const daysInMonth = new Date(year, month, 0).getDate();

    for (let day = 1; day <= daysInMonth; day++) {
        const baseDate = new Date(year, month - 1, day);
        const dayOfWeek = baseDate.getDay();
        const isWeekend = (dayOfWeek === 0 || dayOfWeek === 6);

        employees.forEach(emp => {
            if (isWeekend && Math.random() > 0.15) return;
            if (!isWeekend && Math.random() < 0.02) return;

            // Check-in
            const inTime = new Date(baseDate);
            inTime.setHours(7, 15 + Math.floor(Math.random() * 60), Math.floor(Math.random() * 60));

            const method = Math.random() < 0.2 ? "Ứng dụng Di động" : "Nhận diện Khuôn mặt";
            const deviceName = method === "Ứng dụng Di động" ? "Điện thoại cá nhân" : "Máy quét cổng chính";

            logs.push({
                timestamp: inTime,
                data: {
                    employee_code: emp[0],
                    employee_name: emp[1],
                    department: emp[2],
                    method: method,
                    action: "Giờ vào",
                    device_id: "DEV-01",
                    device_name: deviceName,
                    location: "Văn phòng chính",
                    gps_lat: "10.7626",
                    gps_lng: "106.6601",
                    confidence: "0.99"
                }
            });

            // Check-out
            const outTime = new Date(baseDate);
            outTime.setHours(17, Math.floor(Math.random() * 120), Math.floor(Math.random() * 60)); // Up to 19:xx

            logs.push({
                timestamp: outTime,
                data: {
                    employee_code: emp[0],
                    employee_name: emp[1],
                    department: emp[2],
                    method: method,
                    action: "Giờ ra",
                    device_id: "DEV-01",
                    device_name: deviceName,
                    location: "Văn phòng chính",
                    gps_lat: "10.7626",
                    gps_lng: "106.6601",
                    confidence: "0.98"
                }
            });
        });
    }

    // Sort logs globally by timestamp
    logs.sort((a, b) => a.timestamp - b.timestamp);

    // Build CSV content
    const header = "log_id,employee_code,employee_name,department,method,action,device_id,device_name,check_time,location,gps_lat,gps_lng,confidence\n";
    let content = header;

    logs.forEach((log, idx) => {
        const d = log.data;
        const timeStr = formatTime(log.timestamp);
        const logId = `LOG-${month}-${(idx + 1).toString().padStart(4, '0')}`;
        content += `"${logId}","${d.employee_code}","${d.employee_name}","${d.department}","${d.method}","${d.action}","${d.device_id}","${d.device_name}","${timeStr}","${d.location}","${d.gps_lat}","${d.gps_lng}","${d.confidence}"\n`;
    });

    fs.writeFileSync(filePath, '\ufeff' + content, 'utf8');
    console.log(`Generated and Sorted: ${fileName}`);
});
