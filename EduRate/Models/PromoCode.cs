using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace EduRate.Models
{
    public class PromoCode
    {
        public int Id { get; set; }

        public string Code { get; set; } // الكود نفسه (مثال: EDURATE20)

        [Column(TypeName = "decimal(18,2)")]
        public decimal DiscountPercentage { get; set; } // نسبة الخصم

        public DateTime ExpiryDate { get; set; } // تاريخ انتهاء الصلاحية
        public int MaxUsageCount { get; set; } // أقصى عدد لاستخدام الكود (مثال: 100 طالب بس)
        public int CurrentUsageCount { get; set; } = 0; // كم طالب استخدمه لحد دلوقتي

        public bool IsActive { get; set; } = true;

        // 💡 اختياري: لو الكوبون ده معمول لمدرس معين بس أو سنتر معين
        public int? TeacherId { get; set; }
        public Teacher? Teacher { get; set; }

        public int? CenterId { get; set; }
        public Center? Center { get; set; }
    }
}