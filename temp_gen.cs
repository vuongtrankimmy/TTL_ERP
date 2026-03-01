using System;
using System.IO;
using System.Collections.Generic;
using System.Text;

namespace DataGenerator
{
    public class Program
    {
        public static void Main()
        {
            var sb = new StringBuilder();
            sb.AppendLine("log_id,employee_id,employee_name,department,method,action,device_id,device_name,check_time,location,gps_lat,gps_lng,confidence");
            
            var random = new Random(42);
            var employees = new[] 
            { 
                ("NV-001", "Phan Thanh Tùng", "Kỹ thuật"),
                ("NV-002", "Lý Hải", "Nhân sự"),
                ("NV-003", "Trần Minh", "Kinh doanh"),
                ("NV-101", "Nguyễn Văn A", "Sản xuất"),
                ("NV-102", "Lê Thị B", "Sản xuất"),
                ("NV-201", "Phạm Văn C", "Kế toán"),
                ("NV-202", "Hoàng Văn D", "Bảo vệ"),
                ("NV-008", "Đỗ Thị E", "Hành chính"),
                ("NV-009", "Vũ Văn F", "Kho vận"),
                ("NV-010", "Bùi Thị G", "Tiếp tân")
            };
            
            var year = 2026;
            int logId = 1;

            foreach (int month in new[] { 1, 2 })
            {
                var daysInMonth = DateTime.DaysInMonth(year, month);
                for (int day = 1; day <= daysInMonth; day++)
                {
                    var date = new DateTime(year, month, day);
                    bool isWeekend = date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday;

                    foreach (var emp in employees)
                    {
                        if (isWeekend && random.Next(10) > 1) continue;

                        // Check-in
                        var checkInTime = date.AddHours(7).AddMinutes(30 + random.Next(60));
                        sb.AppendLine($"\"LOG{logId++:D4}\",\"{emp.Item1}\",\"{emp.Item2}\",\"{emp.Item3}\",\"FaceID\",\"Check-In\",\"DEV-01\",\"Máy Cổng Chính\",\"{checkInTime:dd/MM/yyyy HH:mm:ss}\",\"Văn phòng chính\",\"10.7626\",\"106.6601\",\"0.99\"");

                        // Mid-day
                        if (emp.Item3 == "Kinh doanh" || random.Next(25) == 1)
                        {
                            var midTime = date.AddHours(12).AddMinutes(random.Next(30));
                            sb.AppendLine($"\"LOG{logId++:D4}\",\"{emp.Item1}\",\"{emp.Item2}\",\"{emp.Item3}\",\"App\",\"Check-Out\",\"MOBILE\",\"iPhone\",\"{midTime:dd/MM/yyyy HH:mm:ss}\",\"Ngoài văn phòng\",\"10.7712\",\"106.6901\",\"0.95\"");
                            var midTimeIn = midTime.AddHours(1);
                            sb.AppendLine($"\"LOG{logId++:D4}\",\"{emp.Item1}\",\"{emp.Item2}\",\"{emp.Item3}\",\"App\",\"Check-In\",\"MOBILE\",\"iPhone\",\"{midTimeIn:dd/MM/yyyy HH:mm:ss}\",\"Về văn phòng\",\"10.7626\",\"106.6601\",\"0.96\"");
                        }

                        // Check-out
                        var checkOutTime = date.AddHours(17).AddMinutes(random.Next(90));
                        sb.AppendLine($"\"LOG{logId++:D4}\",\"{emp.Item1}\",\"{emp.Item2}\",\"{emp.Item3}\",\"FaceID\",\"Check-Out\",\"DEV-01\",\"Máy Cổng Chính\",\"{checkOutTime:dd/MM/yyyy HH:mm:ss}\",\"Văn phòng chính\",\"10.7626\",\"106.6601\",\"0.98\"");
                    }
                }
            }

            File.WriteAllText("d:/MONEY/2026/TAN_TAN_LOC/TTL_API/attendance_full_jan_feb_2026.csv", sb.ToString(), Encoding.UTF8);
            Console.WriteLine("Successfully created: d:/MONEY/2026/TAN_TAN_LOC/TTL_API/attendance_full_jan_feb_2026.csv");
        }
    }
}
