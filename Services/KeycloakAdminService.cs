// ... (other using statements)
using KeycloakAuthDemo.Options;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text;

namespace KeycloakAuthDemo.Services
{
    public class KeycloakAdminService
    {
        private readonly HttpClient _http;
        private readonly KeycloakOptions _options;
        private string? _token;
        public KeycloakAdminService(HttpClient http, IOptions<KeycloakOptions> options)
        {
            _http = http;
            _options = options.Value;
        }

        private async Task<string> GetAdminTokenAsync()
        {
            if (!string.IsNullOrEmpty(_token)) return _token;

            var content = new FormUrlEncodedContent(new[]
            {
        new KeyValuePair<string, string>("grant_type", "password"),
        new KeyValuePair<string, string>("client_id", _options.ClientId),
        new KeyValuePair<string, string>("client_secret", _options.ClientSecret),
        new KeyValuePair<string, string>("username", _options.Username),
        new KeyValuePair<string, string>("password", _options.Password)
    });

            var response = await _http.PostAsync(
                $"{_options.Authority}/realms/{_options.Realm}/protocol/openid-connect/token",
                content);

            response.EnsureSuccessStatusCode();
            var doc = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
            _token = doc.RootElement.GetProperty("access_token").GetString()!;
            return _token;
        }


        public async Task<bool> CreateUserAsync(CreateUserDto dto)
        {
            var token = await GetAdminTokenAsync();
            _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var payload = new
            {
                username = dto.Username,
                email = dto.Email,
                enabled = true,
                credentials = new[]
                {
                    new { type = "password", value = dto.Password, temporary = false }
                }
            };

            var response = await _http.PostAsync(
                $"{_options.Authority}/admin/realms/{_options.Realm}/users",
                new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json")
            );

            return response.IsSuccessStatusCode;
        }

        public async Task<IEnumerable<object>> GetUsersAsync()
        {
            var token = await GetAdminTokenAsync();
            _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _http.GetAsync(
                $"{_options.Authority}/admin/realms/{_options.Realm}/users");

            response.EnsureSuccessStatusCode();
            return JsonSerializer.Deserialize<IEnumerable<object>>(await response.Content.ReadAsStringAsync())!;
        }
    }
}
