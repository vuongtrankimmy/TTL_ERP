using System;
using System.Threading.Tasks;
using TTL.HR.Application.Modules.Common.Interfaces;
using TTL.HR.Application.Modules.HumanResource.Models;

namespace TTL.HR.Application.Modules.Common.Services
{
    public class MockOcrService : IOcrService
    {
        public async Task<CccdData?> ProcessCccdAsync(byte[] imageBytes, string fileName)
        {
            // Simulate network delay and AI processing
            await Task.Delay(2000);

            // In a real implementation, you would send this to FPT AI, VNPT AI, or use Tesseract
            // For now, we return a "smart" mock based on the fact that an image was provided
            if (imageBytes == null || imageBytes.Length == 0) return null;

            return new CccdData
            {
                Name = "NGUYỄN VĂN MẪU",
                IdCard = "034200001234",
                DOB = new DateTime(1995, 5, 15),
                Gender = "Nam",
                Hometown = "Thanh Hà, Hải Dương",
                Address = "Số 123, Đường Láng, Đống Đa, Hà Nội",
                Nationality = "Việt Nam",
                Ethnicity = "Kinh",
                Religion = "Không",
                IssueDate = "10/05/2022",
                IssuePlace = "Cục Cảnh sát QLHC về TTXH"
            };
        }
    }
}
