using Microsoft.AspNetCore.Http;

namespace FinAppApi.DTOs
{
    public class SubirClienteRequest
    {
        public string? cteNumIdenti { get; set; }
        public string? cteNombApel { get; set; }
        public string? cteDireccion { get; set; }
        public string? cteDirCobCte { get; set; }
        public string? cteTeleFijo { get; set; }
        public string? cteTeleCelu { get; set; }
        public string? cteCodBarDom { get; set; }
        public string? cteCodBarCob { get; set; }
        public string? cteCodRutReg { get; set; }
        public string? cteNotasGenerales { get; set; }
        public string? latitud { get; set; }
        public string? longitud { get; set; }

        // Aquí la foto como IFormFile
        public IFormFile? Foto { get; set; }
        public IFormFile? IDFoto { get; set; }
    }
}
