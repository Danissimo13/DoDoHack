using System;

namespace DoDoModels
{
    public class News
    {
        public long Id { get; set; }

        public string Topic { get; set; }

        public string Body { get; set; }

        public DateTime PublishDate { get; set; }

        public Admin Author { get; set; }
        public long AuthorId { get; set; }
    }
}
