// Setup script for TC 3.1: Insert proper ObjectId employees
const { MongoClient, ObjectId } = require('mongodb');

const MONGO_URI = 'mongodb://127.0.0.1:27030/';

async function setup() {
    const client = new MongoClient(MONGO_URI);
    await client.connect();
    const db = client.db('TTL_Core_DB');
    const col = db.collection('employees');

    // Insert two employees with proper ObjectId _id
    await col.insertMany([
        {
            _id: new ObjectId(),
            Code: "DUMMY01",
            FullName: "Past Employee",
            Email: "past@test.com",
            IsDeleted: false,
            IsAccountActive: true,
            StatusId: null,
            JoinDate: new Date("2024-01-01T00:00:00Z"),
            CreatedAt: new Date(),
            UpdatedAt: null,
            DeletedAt: null,
            Username: "dummy01",
            Salary: 10000000.0,
            Phone: "", AvatarUrl: "", TimekeepingCode: "",
            DepartmentId: null, PositionId: null, ReportToId: null,
            ContractTypeId: null, Workplace: "", ContractEndDate: null,
            CompanyEmail: "",
            EmergencyContact: { Name: '', Relation: '', Phone: '' },
            PersonalDetails: { Gender: '', Address: '', Hometown: '', IdCardNumber: '', TaxCode: '', BankAccount: '', BankName: '', Nationality: 'Việt Nam', Ethnicity: 'Kinh', Religion: 'Không', MaritalStatus: 'Độc thân', PlaceOfOrigin: '', Residence: '', SocialInsuranceId: '', Dependents: [], Latitude: 0, Longitude: 0 },
            Education: [], Experience: [], Roles: []
        },
        {
            _id: new ObjectId(),
            Code: "DUMMY02",
            FullName: "Future Employee",
            Email: "future@test.com",
            IsDeleted: false,
            IsAccountActive: true,
            StatusId: null,
            JoinDate: new Date("2027-01-01T00:00:00Z"),
            CreatedAt: new Date(),
            UpdatedAt: null,
            DeletedAt: null,
            Username: "dummy02",
            Salary: 10000000.0,
            Phone: "", AvatarUrl: "", TimekeepingCode: "",
            DepartmentId: null, PositionId: null, ReportToId: null,
            ContractTypeId: null, Workplace: "", ContractEndDate: null,
            CompanyEmail: "",
            EmergencyContact: { Name: '', Relation: '', Phone: '' },
            PersonalDetails: { Gender: '', Address: '', Hometown: '', IdCardNumber: '', TaxCode: '', BankAccount: '', BankName: '', Nationality: 'Việt Nam', Ethnicity: 'Kinh', Religion: 'Không', MaritalStatus: 'Độc thân', PlaceOfOrigin: '', Residence: '', SocialInsuranceId: '', Dependents: [], Latitude: 0, Longitude: 0 },
            Education: [], Experience: [], Roles: []
        }
    ]);

    const emps = await col.find({}).toArray();
    console.log("Employees inserted:", emps.map(e => `${e.FullName} (JoinDate: ${e.JoinDate.toISOString()}, _id type: ${typeof e._id})`));
    await client.close();
    console.log("Setup complete.");
}

setup().catch(console.error);
