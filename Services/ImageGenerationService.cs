using System;

namespace vCashBlazorDemo.Services
{
    public class ImageGenerationService
    {
        // Stub for WASM (GDI+ not supported)
        
        public ImageGenerationService()
        {
        }

        public string GenerateCheckImage(string makerName, decimal amount, string routing, string account, string checkNum)
        {
            Console.WriteLine("[Stub] Generating Check Image in memory...");
            // Return a placeholder string or data URL if we had logic
            return "check_image_placeholder.jpg"; 
        }
    }
}
