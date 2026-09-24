using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace EduRate.Domain.Entities
{
    public class TeacherCenter
    {
        public int TeacherId { get; set; }
        public Teacher Teacher { get; set; } = null!;

        public int CenterId { get; set; }
        public Center Center { get; set; } = null!;

        public DateTime JoinDate { get; set; } = DateTime.Now;

        [Column(TypeName = "decimal(18,2)")]
        public decimal ProfitPercentage { get; set; }
        public bool IsActive { get; set; } = true;

        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }
    }
}
