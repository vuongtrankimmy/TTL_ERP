using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;

namespace PayrollImporter
{
    class Program
    {
        static async Task Main(string[] args)
        {
            string connectionString = "mongodb://localhost:27017";
            string databaseName = "HR";

            // Try to find .env file
            string envPath = @"d:\MONEY\2026\TAN_TAN_LOC\TTL_API\.env";
            if (File.Exists(envPath))
            {
                var lines = File.ReadAllLines(envPath);
                foreach (var line in lines)
                {
                    if (line.StartsWith("MONGODB_CONNECTION_STRING="))
                        connectionString = line.Split('=')[1].Trim().Trim('"');
                    if (line.StartsWith("MONGODB_DATABASE_NAME="))
                        databaseName = line.Split('=')[1].Trim().Trim('"');
                }
            }

            Console.WriteLine($"Connecting to {connectionString}...");
            var client = new MongoClient(connectionString);
            var db = client.GetDatabase(databaseName);

            // 1. Insert Period
            var periodsCol = db.GetCollection<BsonDocument>("payroll_periods");
            var period = new BsonDocument
            {
                { "_id", "65dae2f30000000000000999" },
                { "Name", "Lương tháng 02/2026" },
                { "Month", 2 },
                { "Year", 2026 },
                { "StartDate", DateTime.Parse("2026-02-01T00:00:00Z") },
                { "EndDate", DateTime.Parse("2026-02-28T00:00:00Z") },
                { "PaymentDate", DateTime.Parse("2026-03-05T00:00:00Z") },
                { "Status", "Open" },
                { "TotalNetSalary", 2500000000.0 },
                { "TotalInsurance", 250000000.0 },
                { "TotalTax", 150000000.0 },
                { "EmployeeCount", 100 },
                { "Note", "Dữ liệu mẫu cho kiểm thử hệ thống" },
                { "IsDeleted", false }
            };

            await periodsCol.DeleteManyAsync(new BsonDocument("_id", "65dae2f30000000000000999"));
            await periodsCol.InsertOneAsync(period);
            Console.WriteLine("Inserted Payroll Period.");

            // 2. Insert Payrolls
            var payrollsCol = db.GetCollection<BsonDocument>("payrolls");
            string json = File.ReadAllText(@"d:\MONEY\2026\TAN_TAN_LOC\TTL_ERP\data_seed\payrolls_100.json");
            
            var payrollsJson = JsonSerializer.Deserialize<List<Dictionary<string, object>>>(json);
            var docs = new List<BsonDocument>();

            foreach (var p in payrollsJson)
            {
                var doc = new BsonDocument();
                foreach (var kvp in p)
                {
                    if (kvp.Value is JsonElement el)
                    {
                        if (el.ValueKind == JsonValueKind.Number)
                        {
                            if (el.TryGetDecimal(out decimal d)) doc.Add(kvp.Key, (double)d);
                            else doc.Add(kvp.Key, el.GetDouble());
                        }
                        else if (el.ValueKind == JsonValueKind.String) doc.Add(kvp.Key, el.GetString());
                        else if (el.ValueKind == JsonValueKind.True) doc.Add(kvp.Key, true);
                        else if (el.ValueKind == JsonValueKind.False) doc.Add(kvp.Key, false);
                        else if (el.ValueKind == JsonValueKind.Array)
                        {
                            var arr = new BsonArray();
                            foreach (var item in el.EnumerateArray())
                            {
                                var subDoc = new BsonDocument();
                                foreach (var subProp in item.EnumerateObject())
                                {
                                    if (subProp.Value.ValueKind == JsonValueKind.Number)
                                        subDoc.Add(subProp.Name, (double)subProp.Value.GetDecimal());
                                    else
                                        subDoc.Add(subProp.Name, subProp.Value.ToString());
                                }
                                arr.Add(subDoc);
                            }
                            doc.Add(kvp.Key, arr);
                        }
                    }
                }
                doc.Add("IsDeleted", false);
                docs.Add(doc);
            }

            await payrollsCol.DeleteManyAsync(new BsonDocument("PeriodId", "65dae2f30000000000000999"));
            await payrollsCol.InsertManyAsync(docs);
            Console.WriteLine($"Inserted {docs.Count} Payroll records.");
        }
    }
}
