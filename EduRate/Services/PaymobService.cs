using Microsoft.Extensions.Configuration;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace EduRate.Services
{
    public interface IPaymobService
    {
        Task<string> GetAuthTokenAsync();
        Task<int> CreateOrderAsync(string authToken, decimal amount);

        // 💡 التعديل هنا: تم إضافة paymentMethod في الـ Interface
        Task<string> GetPaymentKeyAsync(string authToken, int orderId, decimal amount, string email, string firstName, string lastName, string phone, string paymentMethod);
    }

    public class PaymobService : IPaymobService
    {
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;

        public PaymobService(IConfiguration configuration, HttpClient httpClient)
        {
            _configuration = configuration;
            _httpClient = httpClient;
        }

        // 1. الحصول على التوكن من Paymob
        public async Task<string> GetAuthTokenAsync()
        {
            var apiKey = _configuration["Paymob:ApiKey"]; // هنحط المفتاح في appsettings.json
            var payload = new { api_key = apiKey };
            var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("https://accept.paymob.com/api/auth/tokens", content);
            var jsonResponse = await response.Content.ReadAsStringAsync();

            using var doc = JsonDocument.Parse(jsonResponse);
            return doc.RootElement.GetProperty("token").GetString();
        }

        // 2. إنشاء طلب دفع (Order)
        public async Task<int> CreateOrderAsync(string authToken, decimal amount)
        {
            var payload = new
            {
                auth_token = authToken,
                delivery_needed = false,
                amount_cents = (amount * 100).ToString(), // Paymob بيتعامل بالقروش (اضربي في 100)
                currency = "EGP",
                items = new object[] { }
            };

            var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("https://accept.paymob.com/api/ecommerce/orders", content);
            var jsonResponse = await response.Content.ReadAsStringAsync();

            using var doc = JsonDocument.Parse(jsonResponse);
            return doc.RootElement.GetProperty("id").GetInt32();
        }

        // 3. طلب مفتاح الدفع (Payment Key)
        public async Task<string> GetPaymentKeyAsync(string authToken, int orderId, decimal amount, string email, string firstName, string lastName, string phone, string paymentMethod)
        {
            // 💡 تحديد الـ Integration ID بناءً على اختيار الطالب
            var integrationId = paymentMethod == "Card"
                ? _configuration["Paymob:IntegrationIdCard"]
                : _configuration["Paymob:IntegrationIdWallet"];

            var payload = new
            {
                auth_token = authToken,
                amount_cents = (amount * 100).ToString(),
                expiration = 3600,
                order_id = orderId.ToString(),
                billing_data = new
                {
                    apartment = "NA",
                    email = email ?? "test@edurate.com",
                    floor = "NA",
                    first_name = firstName ?? "Student",
                    street = "NA",
                    building = "NA",
                    phone_number = phone ?? "+201000000000",
                    shipping_method = "NA",
                    postal_code = "NA",
                    city = "Cairo",
                    country = "EG",
                    last_name = lastName ?? "EduRate",
                    state = "Cairo"
                },
                currency = "EGP",
                integration_id = int.Parse(integrationId) // 💡 الرقم هيتغير أوتوماتيك هنا
            };

            var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("https://accept.paymob.com/api/acceptance/payment_keys", content);
            var jsonResponse = await response.Content.ReadAsStringAsync();

            using var doc = JsonDocument.Parse(jsonResponse);
            return doc.RootElement.GetProperty("token").GetString();
        }
    }
}