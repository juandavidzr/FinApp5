using FinApp5.Modelo;
using Microsoft.Extensions.Hosting;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace FinAppMaui.Services
{
    public class AuthService
    {
        private readonly HttpClient _http;

        public AuthService(IHttpClientFactory httpClientFactory)
        {
            _http = httpClientFactory.CreateClient("FinAppApi");
        }

        //public async Task<bool> LoginAsync(string usuario, string password)
        public async Task<Musuarios?> LoginAsync(string usuario, string password)
        {
            try
            {
                var response = await _http.PostAsJsonAsync("api/Auth/login", new
                {
                    Usuario = usuario,
                    Password = password
                });

                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Error al iniciar sesión: {error}");
                }

                var content = await response.Content.ReadAsStringAsync();

                Console.WriteLine($"Status: {response.StatusCode}");
                Console.WriteLine($"Response: {content}");
                if (response.IsSuccessStatusCode)
                {
                    var musuario = await response.Content.ReadFromJsonAsync<Musuarios>();
                    return musuario;
                }
                //return response.IsSuccessStatusCode;
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Excepción: {ex.Message}");
                return null;
            }
        }
    }
}
