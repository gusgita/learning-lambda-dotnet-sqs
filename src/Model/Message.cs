using System;

namespace Model
{
    public class Message
    {
        public string Subject { get; set; }
        public string Body { get; set; }
        public Status ProcessingStatus { get; set; }
    }
}
