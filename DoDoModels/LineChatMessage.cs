namespace DoDoModels
{
    public class LineChatMessage
    {
        public long Id { get; set; }

        public string Message { get; set; }

        public long SenderId { get; set; }

        public bool IsAdmin { get; set; }
    }
}
