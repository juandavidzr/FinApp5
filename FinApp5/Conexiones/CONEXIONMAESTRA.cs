using Microsoft.Data.SqlClient;
using Microsoft.Maui.Networking;

namespace FinApp5.Conexiones
{
    public class CONEXIONMAESTRA
    {
        //public static string conexion = "Server=138.128.171.162; Database=credicristianwil; User Id=sa; Password=!6ks4cgmyjD%duB;TrustServerCertificate=true";
        //public static string conexion = "Server=138.128.171.162; Database=creditosGonzalez; User Id=sa; Password=!6ks4cgmyjD%duB;TrustServerCertificate=true";
        public static string conexion = "Server=138.128.171.162; Database=creditosjgvcc; User Id=sa; Password=!6ks4cgmyjD%duB;TrustServerCertificate=true";
        //public static string conexion = "Server=138.128.171.162; Database=prueba; User Id=sa; Password=!6ks4cgmyjD%duB; Encrypt=True; TrustServerCertificate=True;";
        //;Encrypt=False;TrustServerCertificate=true
        // "Server=138.128.171.162,1433;Database=Finanzas;User Id=sa;Password=TuClaveSegura;Encrypt=False;"

        //public static string conexion = "Server=138.128.171.162,1433; Database=creditosmag1; User Id=sa; Password=!6ks4cgmyjD%duB;Encrypt=False;Connection Timeout=30;TrustServerCertificate=false";

        //string connString = _config.GetConnectionString("SqlServer");

        public static SqlConnection conectar = new SqlConnection(conexion);
        
        public static SqlConnection GetConnection()
        {
            return new SqlConnection(conexion);
        }

        //public static void Abrir()
        //{
        //    try
        //    {
        //        if (conectar.State == System.Data.ConnectionState.Closed)
        //            conectar.Open();
        //    }
        //    catch (Exception ex)
        //    {
        //        // Este log momentáneo ayuda a encontrar el error real
        //        System.Diagnostics.Debug.WriteLine("ERROR al abrir conexión: " + ex.Message);
        //        throw; // re-lanzamos para no ocultarlo                
        //    }
        //}
        //public static void Cerrar()
        //{
        //    if (conectar.State == System.Data.ConnectionState.Open)
        //        conectar.Close();
        //}

        public static void Cerrar()
        {
            try
            {
                if (conectar != null && conectar.State != System.Data.ConnectionState.Closed)
                {
                    conectar.Close();
                }
            }
            catch (InvalidOperationException ex)
            {
                // Puedes registrar el error pero no interrumpir la app
                Console.WriteLine("Error al cerrar conexión: " + ex.Message);
            }
            finally
            {
                if (conectar != null)
                    conectar.Dispose();
            }
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

        public static bool VerificarConexion()
        {
            var estado = Connectivity.NetworkAccess;
            return estado == NetworkAccess.Internet;
        }
    }
}
