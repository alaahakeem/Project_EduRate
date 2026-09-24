namespace EduRate.Application.Common.Interfaces
{
    public interface IPaymobService
    {
        Task<string> GetAuthTokenAsync();
        Task<int> CreateOrderAsync(string authToken, decimal amount);
        Task<string> GetPaymentKeyAsync(string authToken, int orderId, decimal amount, string email, string firstName, string lastName, string phone, string paymentMethod);
    }
}
