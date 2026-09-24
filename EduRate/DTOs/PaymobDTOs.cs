namespace EduRate.DTOs
{
    // الداتا اللي الطالب بيبعتها عشان يشحن محفظته (المبلغ والطريقة)
    public class PaymentRequestDto
    {
        public decimal Amount { get; set; }
        public string PhoneNumber { get; set; } // مهم لو هيدفع محفظة إلكترونية
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        // 💡 التعديل: استلام طريقة الدفع من الطالب (Card أو Wallet)
        public string PaymentMethod { get; set; }
    }

    // الرد اللي بيرجع من البوابة عشان نوجه الفرونت إند عليه
    public class PaymentResponseDto
    {
        public string RedirectUrl { get; set; } // رابط الدفع اللي الفرونت هيف فتحه
        public string Message { get; set; }
    }
}