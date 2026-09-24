using System.Text;
using System.Text.Json;
using EduRate.Application.Common.Interfaces;
using Microsoft.Extensions.Configuration;

namespace EduRate.Infrastructure.Services
{
    public class PaymobService : IPaymobService
    {
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;

        public PaymobService(IConfiguration configuration, HttpClient httpClient)
        {
            _configuration = configuration;
            _httpClient = httpClient;
        }

        public async Task<string> GetAuthTokenAsync()
        {
            var apiKey = _configuration["Paymob:ApiKey"];
            var payload = new { api_key = apiKey };
            var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("https://accept.paymob.com/api/auth/tokens", content);
            var jsonResponse = await response.Content.ReadAsStringAsync();

            using var doc = JsonDocument.Parse(jsonResponse);
            return doc.RootElement.GetProperty("token").GetString()!;
        }

        public async Task<int> CreateOrderAsync(string authToken, decimal amount)
        {
            var payload = new
            {
                auth_token = authToken,
                delivery_needed = false,
                amount_cents = (amount * 100).ToString(), // Paymob works in piastres (x100)
                currency = "EGP",
                items = Array.Empty<object>()
            };

            var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("https://accept.paymob.com/api/ecommerce/orders", content);
            var jsonResponse = await response.Content.ReadAsStringAsync();

            using var doc = JsonDocument.Parse(jsonResponse);
            return doc.RootElement.GetProperty("id").GetInt32();
        }

        public async Task<string> GetPaymentKeyAsync(string authToken, int orderId, decimal amount, string email, string firstName, string lastName, string phone, string paymentMethod)
        {
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
                integration_id = int.Parse(integrationId!)
            };

            var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("https://accept.paymob.com/api/acceptance/payment_keys", content);
            var jsonResponse = await response.Content.ReadAsStringAsync();

            using var doc = JsonDocument.Parse(jsonResponse);
            return doc.RootElement.GetProperty("token").GetString()!;
        }
    }
}
