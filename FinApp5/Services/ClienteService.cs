using Azure.Core;
using FinApp5.Modelo;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace FinAppMaui.Services
{
    public class ClienteService
    {
        private readonly HttpClient _http;

        // Usamos el HttpClient configurado en MauiProgram
        public ClienteService(IHttpClientFactory httpClientFactory)
        {
            _http = httpClientFactory.CreateClient("FinAppApi");
        }

        // Método para subir cliente con foto
        public async Task<HttpResponseMessage> SubirClienteAsync(Mcliente cliente)
        {
            try
            {
                using var content = new MultipartFormDataContent();

                // Agregar los datos del cliente como form-data
                content.Add(new StringContent(cliente.cteNumIdenti ?? ""), "cteNumIdenti");
                content.Add(new StringContent(cliente.cteNombApel ?? ""), "cteNombApel");
                content.Add(new StringContent(cliente.cteDireccion ?? ""), "cteDireccion");
                content.Add(new StringContent(cliente.cteDirCobCte ?? ""), "cteDirCobCte");
                content.Add(new StringContent(cliente.cteTeleFijo ?? ""), "cteTeleFijo");
                content.Add(new StringContent(cliente.cteTeleCelu ?? ""), "cteTeleCelu");
                content.Add(new StringContent(cliente.cteCodBarDom ?? ""), "cteCodBarDom");
                content.Add(new StringContent(cliente.cteCodBarCob ?? ""), "cteCodBarCob");
                content.Add(new StringContent(cliente.cteCodRutReg ?? ""), "cteCodRutReg");
                content.Add(new StringContent(cliente.cteNotasGenerales ?? ""), "cteNotasGenerales");
                content.Add(new StringContent(cliente.latitud ?? "0"), "latitud");
                content.Add(new StringContent(cliente.longitud ?? "0"), "longitud");

                if (cliente.Foto != null && cliente.Foto.Length > 0)
                {
                    var imageContent = new ByteArrayContent(cliente.Foto);
                    imageContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("image/jpeg");
                    content.Add(imageContent, "foto", "cliente.jpg");
                }
                if (cliente.IDFoto != null && cliente.IDFoto.Length > 0)
                {
                    var imageIDContent = new ByteArrayContent(cliente.IDFoto);
                    imageIDContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("image/jpeg");
                    content.Add(imageIDContent, "IDFoto", "IDcliente.jpg");
                }
                // Agregar la foto si existe



                // Llamada a la API
                var response = await _http.PostAsync("api/clientes/subir", content);

                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Error al subir cliente: {error}");
                }

                return response;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Excepción en SubirClienteAsync: {ex.Message}");
                throw;
            }
        }

        public async Task<HttpResponseMessage> ActualizarClienteAsync(Mcliente cliente)
        {
            try
            {
                using var content = new MultipartFormDataContent();

                // Agregar los datos del cliente
                content.Add(new StringContent(cliente.cteNumIdenti ?? ""), "cteNumIdenti");
                content.Add(new StringContent(cliente.cteCodRutReg ?? ""), "cteCodRutReg");
                content.Add(new StringContent(cliente.cteNotasGenerales ?? ""), "cteNotasGenerales");
                content.Add(new StringContent(cliente.latitud ?? "0"), "latitud");
                content.Add(new StringContent(cliente.longitud ?? "0"), "longitud");

                // Agregar fotos (si existen)
                if (cliente.Foto != null && cliente.Foto.Length > 0)
                {
                    var imageContent = new ByteArrayContent(cliente.Foto);
                    imageContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("image/jpeg");
                    content.Add(imageContent, "foto", "cliente.jpg");
                }

                if (cliente.IDFoto != null && cliente.IDFoto.Length > 0)
                {
                    var imageIDContent = new ByteArrayContent(cliente.IDFoto);
                    imageIDContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("image/jpeg");
                    content.Add(imageIDContent, "IDFoto", "IDcliente.jpg");
                }
                
                var response = await _http.PutAsync("api/clientes/actualizar", content);

                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"❌ Error al actualizar cliente: {error}");
                }

                return response;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"⚠️ Excepción en ActualizarClienteAsync: {ex.Message}");
                throw;
            }
        }

    }
}
