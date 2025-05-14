using Microsoft.Data.SqlClient;

namespace FinApp5.Conexiones
{
    public class CONEXIONMAESTRA
    {
        public static string conexion = "Server=138.128.171.162; Database=prueba; User Id=sa; Password=!6ks4cgmyjD%duB;TrustServerCertificate=true";
        //public static string conexion = "Server=138.128.171.162; Database=credicristianwil; User Id=sa; Password=!6ks4cgmyjD%duB;TrustServerCertificate=true";

        public static SqlConnection conectar = new SqlConnection(conexion);
        public static void Abrir()
        {
            if (conectar.State == System.Data.ConnectionState.Closed)
                conectar.Open();
        }
        public static void Cerrar()
        {
            if (conectar.State == System.Data.ConnectionState.Open)
                conectar.Close();
        }
        public static bool VerificarCon()
        {
            bool estado = false;
            System.Uri Url = new System.Uri("https://www.google.com/");

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    var response = client.GetAsync(Url).Result;
                    estado = response.IsSuccessStatusCode;
                }
            }
            catch (Exception)
            {
                estado = false;
            }
            return estado;
        }
        private static readonly HttpClient httpClient = new HttpClient
        {
            Timeout = TimeSpan.FromSeconds(10) // Ajusta el timeout según sea necesario
        };
        public static async Task<bool> VerificarConexionAsync()
        {
            try
            {
                using HttpResponseMessage response = await httpClient.GetAsync("https://www.google.com/");
                return response.IsSuccessStatusCode;
            }
            catch (HttpRequestException) // Error de red
            {
                return false;
            }
            catch (TaskCanceledException) // Timeout
            {
                return false;
            }
        }
    }
}
