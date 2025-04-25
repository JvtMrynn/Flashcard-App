using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using FlashcardApp.Models;

namespace FlashcardApp.Services
{
    public class FirebaseAuthService
    {
        private const string FirebaseApiKey = "";
        private const string SignInUrl = $"https://identitytoolkit.googleapis.com/v1/accounts:signInWithPassword?key={FirebaseApiKey}";

        public async Task<FirebaseAuthResponse> LoginAsync(string email, string password)
        {
            var request = new FirebaseAuthRequest { email = email, password = password };
            var json = JsonSerializer.Serialize(request);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            using var client = new HttpClient();
            var response = await client.PostAsync(SignInUrl, content);
            var result = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
                return JsonSerializer.Deserialize<FirebaseAuthResponse>(result);
            else
                throw new Exception($"Login failed: {result}");
        }

        public async Task<FirebaseAuthResponse> RegisterAsync(string email, string password)
        {
            const string RegisterUrl = $"https://identitytoolkit.googleapis.com/v1/accounts:signUp?key={FirebaseApiKey}";

            var request = new FirebaseAuthRequest
            {
                email = email,
                password = password,
                returnSecureToken = true
            };

            var json = JsonSerializer.Serialize(request);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            using var client = new HttpClient();
            var response = await client.PostAsync(RegisterUrl, content);
            var result = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
                return JsonSerializer.Deserialize<FirebaseAuthResponse>(result);
            else
                throw new Exception($"Registration failed: {result}");
        }
    }
}
