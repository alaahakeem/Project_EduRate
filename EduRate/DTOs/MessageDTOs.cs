namespace EduRate.DTOs
{
    public class SendMessageDto
    {
        public int ReceiverId { get; set; } // رقم الشخص اللي هيستقبل الرسالة (سواء كان طالب أو مدرس)
        public string Content { get; set; }
    }

    public class MessageDto
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public string SenderRole { get; set; }
        public DateTime SentAt { get; set; }
        public bool IsRead { get; set; }
    }
}