using System.Threading.Tasks;
using TTL.HR.Application.Modules.HumanResource.Models;

namespace TTL.HR.Application.Modules.Common.Interfaces
{
    public interface IOcrService
    {
        /// <summary>
        /// Processes a CCCD/ID Card image and returns the extracted data
        /// </summary>
        /// <param name="imageBytes">The raw bytes of the image</param>
        /// <param name="fileName">The name of the file</param>
        /// <returns>Extracted CCCD data or null if processing fails</returns>
        Task<CccdData?> ProcessCccdAsync(byte[] imageBytes, string fileName);
    }
}
