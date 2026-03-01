
const employees = db.employees.find({}, { _id: 1, Code: 1 }).toArray();
const empMap = {};
employees.forEach(e => {
    empMap[e._id.toString()] = e.Code;
});

const attendances = db.attendances.find({ $or: [{ EmployeeCode: null }, { EmployeeCode: "" }] }).toArray();
let count = 0;
attendances.forEach(att => {
    if (att.EmployeeId) {
        let code = empMap[att.EmployeeId.toString()];
        if (code) {
            db.attendances.updateOne({ _id: att._id }, { $set: { EmployeeCode: code } });
            count++;
        }
    }
});
console.log(`Updated ${count} attendance records.`);
