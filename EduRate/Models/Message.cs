namespace EduRate.Models
{
    public class Message
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public DateTime SentAt { get; set; } = DateTime.Now;
        public bool IsRead { get; set; }

        // عشان نعرف مين اللي بعت الرسالة (الطالب ولا المدرس)
        public string SenderRole { get; set; }

        // العلاقات
        public int StudentId { get; set; }
        public Student Student { get; set; }

        public int TeacherId { get; set; }
        public Teacher Teacher { get; set; }
    }
}