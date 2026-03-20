using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
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
            
            var payrolls = JArray.Parse(json);
            var docs = new List<BsonDocument>();

            foreach (JObject p in payrolls)
            {
                var doc = new BsonDocument();
                foreach (var kvp in p)
                {
                    var key = kvp.Key;
                    var token = kvp.Value;

                    if (token == null || token.Type == JTokenType.Null) continue;

                    if (token.Type == JTokenType.Integer || token.Type == JTokenType.Float)
                    {
                        doc.Add(key, token.Value<double>());
                    }
                    else if (token.Type == JTokenType.String)
                    {
                        doc.Add(key, token.Value<string>());
                    }
                    else if (token.Type == JTokenType.Boolean)
                    {
                        doc.Add(key, token.Value<bool>());
                    }
                    else if (token.Type == JTokenType.Array)
                    {
                        var arr = new BsonArray();
                        foreach (var item in (JArray)token)
                        {
                            if (item.Type == JTokenType.Object)
                            {
                                var subDoc = new BsonDocument();
                                foreach (var subKvp in (JObject)item)
                                {
                                    if (subKvp.Value?.Type == JTokenType.Integer || subKvp.Value?.Type == JTokenType.Float)
                                        subDoc.Add(subKvp.Key, subKvp.Value.Value<double>());
                                    else
                                        subDoc.Add(subKvp.Key, subKvp.Value?.ToString() ?? string.Empty);
                                }
                                arr.Add(subDoc);
                            }
                        }
                        doc.Add(key, arr);
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
