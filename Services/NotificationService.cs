using System;
using System.Threading.Tasks;

namespace vCashBlazorDemo.Services
{
    public class NotificationService
    {
        // Stub for WASM (No SMTP)

        public async Task SendMmsAsync(string recipient, string subject, string body, string attachmentPath = null, string attachmentName = null)
        {
            await Task.Delay(100); // Simulate network
            Console.WriteLine($"[Stub Notification] To: {recipient}, Subject: {subject}");
            Console.WriteLine($"Body: {body}");
        }
    }
}
