using System;
using System.Threading.Tasks;
using Microsoft.JSInterop;
using Microsoft.Extensions.Configuration;
using TTL.HR.Application.Modules.Common.Interfaces;
using TTL.HR.Application.Modules.HumanResource.Models;

namespace TTL.HR.Application.Infrastructure.Services
{
    /**
     * LocalOcrService - The Neural Failover Hub
     * Triple-Core Resilience: OpenAI GPT-4o -> Hugging Face (Florence-2) -> Ollama (LLaVA)
     * Automatic Engine Selection based on Quota, Availability, and Cost.
     */
    public class LocalOcrService : IOcrService
    {
        private readonly GptOcrService _gptService;
        private readonly HuggingFaceOcrService _hfService;
        private readonly OllamaOcrService _ollamaService;
        private readonly string _preferredEngine;

        public LocalOcrService(IConfiguration configuration)
        {
            _gptService = new GptOcrService(configuration);
            _hfService = new HuggingFaceOcrService(configuration);
            _ollamaService = new OllamaOcrService(configuration);
            _preferredEngine = configuration["OcrSettings:PreferredEngine"] ?? "Auto";
        }

        public async Task<CccdData?> ProcessCccdAsync(byte[] imageBytes, string fileName)
        {
            try
            {
                // 1. SELECT NEURAL ENGINE
                return _preferredEngine switch
                {
                    "GPT" => await _gptService.ExtractWithGptVisionAsync(imageBytes),
                    "HF" => await _hfService.ExtractWithFlorenceAsync(imageBytes),
                    "Ollama" => await _ollamaService.ExtractWithOllamaAsync(imageBytes),
                    _ => await ExecuteFailoverChainAsync(imageBytes) // "Auto" Mode
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[NEURAL HUB ERROR] Critical Failure: {ex.Message}");
                return null;
            }
        }

        private async Task<CccdData?> ExecuteFailoverChainAsync(byte[] imageBytes)
        {
            // TIER 1: GPT-4o VISION (PREMIUM)
            Console.WriteLine("[NEURAL HUB] Attempting Tier-1: GPT-4o Vision...");
            var gptResult = await _gptService.ExtractWithGptVisionAsync(imageBytes);
            if (gptResult != null && !string.IsNullOrEmpty(gptResult.IdCard)) return gptResult;

            // TIER 2: HUGGING FACE (FREE CLOUD - FLORENCE-2)
            Console.WriteLine("[NEURAL HUB] Quota/Error detected. Calling Tier-2 Backup: Hugging Face...");
            var hfResult = await _hfService.ExtractWithFlorenceAsync(imageBytes);
            if (hfResult != null && !string.IsNullOrEmpty(hfResult.IdCard)) return hfResult;

            // TIER 3: OLLAMA (FREE LOCAL - LLAVA)
            Console.WriteLine("[NEURAL HUB] Cloud Link Failure. Re-igniting Tier-3: Ollama Local...");
            return await _ollamaService.ExtractWithOllamaAsync(imageBytes);
        }
    }
}
