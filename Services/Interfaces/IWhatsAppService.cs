namespace FinalProjectWebAPI.Services.Interfaces
{
    public interface IWhatsAppService
    {
        Task SendMessageAsync(string phoneNumber, string message);
    }
}