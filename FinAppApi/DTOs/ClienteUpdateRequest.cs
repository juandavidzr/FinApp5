using Microsoft.AspNetCore.Http;

namespace FinAppApi.DTOs
{
    public class ClienteUpdateRequest
    {
        public string? cteNumIdenti { get; set; }
        public string? cteCodRutReg { get; set; }
        public string? cteNotasGenerales { get; set; }
        public string? latitud { get; set; }
        public string? longitud { get; set; }
        public IFormFile? Foto { get; set; }
        public IFormFile? IDFoto { get; set; }
        public IFormFile? lugarFoto { get; set; }

    }
}