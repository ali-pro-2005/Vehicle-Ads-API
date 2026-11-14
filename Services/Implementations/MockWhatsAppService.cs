using FinalProjectWebAPI.Services.Interfaces;

namespace FinalProjectWebAPI.Services.Implementations
{
    public class MockWhatsAppService : IWhatsAppService
    {
        public Task SendMessageAsync(string phoneNumber, string message)
        {
            // هنا يمكنك وضع كود Twilio أو Meta API لاحقاً.
            // حالياً نطبع الرسالة في الـ Console للمحاكاة.
            Console.WriteLine($"=========================================");
            Console.WriteLine($"WhatsApp Message Sent to: {phoneNumber}");
            Console.WriteLine($"Message: {message}");
            Console.WriteLine($"=========================================");

            return Task.CompletedTask;
        }
    }
}