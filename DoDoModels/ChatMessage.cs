namespace DoDoModels
{
    public class ChatMessage
    {
        public long Id { get; set; }

        public string Message { get; set; }

        public long SenderId { get; set; }

        public long ReceiverId { get; set; }
    }
}
