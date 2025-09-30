using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinApp5.Services
{
    class CreditosService
    {

        private readonly HttpClient _http;

        public CreditosService(IHttpClientFactory httpClientFactory)
        {
            _http = httpClientFactory.CreateClient("FinApi");
        }

        public async Task<string> ProbarConexion()
        {
            var response = await _http.GetAsync("api/Creditos/test");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }

    }
}
