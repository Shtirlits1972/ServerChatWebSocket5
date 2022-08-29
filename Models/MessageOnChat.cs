namespace ServerChatWebSocket5.Models
{
    public class MessageOnChat
    {
        public string UserId { get; set; }
        public string TextMessage { get; set; }

        public override string ToString()
        {
            return $" UserId = {UserId},  TextMessage = {TextMessage} ";
        }
    }
}
