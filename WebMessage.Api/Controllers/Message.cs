namespace WebMessage.Api.Controllers
{
    public class Message
    {
        public string Text { get; set; }
        public long Number { get; set; }
        public DateTime Date { get; set; }
    }

    public class MessageDto
    {
        public string Text { get; set; }
        public long Number { get; set; }
    }
}