using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace EduRate.Domain.Entities
{
    public class PromoCode
    {
        public int Id { get; set; }

        public string Code { get; set; } = string.Empty;

        [Column(TypeName = "decimal(18,2)")]
        public decimal DiscountPercentage { get; set; }

        public DateTime ExpiryDate { get; set; }
        public int MaxUsageCount { get; set; }
        public int CurrentUsageCount { get; set; } = 0;

        public bool IsActive { get; set; } = true;

        public int? TeacherId { get; set; }
        public Teacher? Teacher { get; set; }

        public int? CenterId { get; set; }
        public Center? Center { get; set; }
    }
}
