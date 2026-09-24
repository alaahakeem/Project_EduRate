using System.ComponentModel.DataAnnotations.Schema;

namespace EduRate.Models
{
    public class TeacherCenter
    {
        //  (Foreign Keys)
        public int TeacherId { get; set; }
        public Teacher Teacher { get; set; }

        public int CenterId { get; set; }
        public Center Center { get; set; }

        // التفاصيل الإضافية للعلاقة
        public DateTime JoinDate { get; set; } = DateTime.Now; // تاريخ الانضمام

        [Column(TypeName = "decimal(18,2)")]
        public decimal ProfitPercentage { get; set; } // نسبة المدرس من أرباح الحصة
        public bool IsActive { get; set; } = true; // هل لسه شغال في السنتر ده ولا سابهم؟

        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }
    }
}