﻿using BankSystem.Data.Entities.Files;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BankSystem.Service.Services.FileScanService
{
    public interface IFileScanService
    {
        Task<IActionResult> UploadAndScanFileAsync(IFormFile file, HttpRequest request);
        Task<IActionResult> GetScanDetailsAsync(string fileHash);
        Task<IActionResult> BlockFileHashAsync(BlockedHashRequest request);
    }
}
