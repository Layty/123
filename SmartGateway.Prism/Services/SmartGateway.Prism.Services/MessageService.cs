using SmartGateway.Prism.Services.Interfaces;

namespace SmartGateway.Prism.Services
{
    public class MessageService : IMessageService
    {
        public string GetMessage()
        {
            return "Hello from the Message Service";
        }
    }
}
