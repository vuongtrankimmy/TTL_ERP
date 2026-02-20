using Microsoft.AspNetCore.Components.Forms;
using TTL.HR.Application.Modules.Common.Models;

namespace TTL.HR.Application.Modules.Common.Interfaces
{
    public interface IFileService
    {
        Task<string?> UploadFileAsync(IBrowserFile file, string folder = "general");
    }
}
