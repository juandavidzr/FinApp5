using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;

using Microsoft.Data.SqlClient;
using System.Data;
using FinApp5.Modelo;

namespace FinApp5.Conexiones
{
    public class CONEXIONMAESTRA
    {
        public static string conexion = "Server=138.128.171.162; Database=prueba; User Id=sa; Password=!6ks4cgmyjD%duB;TrustServerCertificate=true";

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

            System.Net.WebRequest? WebRequest;
            WebRequest = System.Net.WebRequest.Create(Url);
            System.Net.WebResponse objetoResp;
            try
            {
                objetoResp = WebRequest.GetResponse();
                estado = true;
                objetoResp.Close();
            }
            catch (Exception)
            {
                estado = false;
                //DisplayAlert("Error", "Error: " + ex.Message, "OK");
            }
            finally
            {
                WebRequest = null;
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

        public async void SincronizarCreditos(string usuario) //inserta los nuevos creditos en el servidor
        {
            Prestamos prestamo = new Prestamos();
            try
            {
                SqlCommand cmd = new SqlCommand("GrabaCredito", CONEXIONMAESTRA.conectar);
                cmd.CommandType = CommandType.StoredProcedure;
                var prestamosList = await App.SQLiteDB.GetNewPrestamos();
                if (prestamosList != null)
                {
                    foreach (var p in prestamosList)
                    {
                        prestamo = await App.SQLiteDB.GetPrestamosByIdAsync(p.NumPrestamo);
                        if (prestamo != null && p.codigoRuta != null && p.idCliente != null && p.codigoPlan != null && p.desDiaPago != null)
                        {
                            cmd.Parameters.AddWithValue("@strCodigRut", p.codigoRuta.Trim());//1
                            cmd.Parameters.AddWithValue("@strNumIdeCte", p.idCliente.Trim());//2
                            cmd.Parameters.AddWithValue("@dblNetoEnCre", p.cantidadPrestada);//3
                            cmd.Parameters.AddWithValue("@dblPorIntCre", p.interes);//4
                            cmd.Parameters.AddWithValue("@strCodPlaPac", p.codigoPlan.Trim());//5
                            cmd.Parameters.AddWithValue("@intNumCuoCre", p.numeroCuotas);//6
                            cmd.Parameters.AddWithValue("@intNumCreVig", p.cantidadCreVig);//7
                            cmd.Parameters.AddWithValue("@dblSaldoAcCr", p.saldoActualCre);//8
                            cmd.Parameters.AddWithValue("@intNumCuoPag", p.numCuoPag);//9
                            cmd.Parameters.AddWithValue("@intNumCuoPen", p.numCuoPen);//10
                            cmd.Parameters.AddWithValue("@strFecUltPag", p.fecUltPag);//11
                            cmd.Parameters.AddWithValue("@dblValUltPag", p.valUltPag);//12
                            cmd.Parameters.AddWithValue("@strFecVtoCre", p.fecVenCre);//13
                            cmd.Parameters.AddWithValue("@intPosCreEnr", p.posRutCre);//14
                            cmd.Parameters.AddWithValue("@intTieDiaCre", p.tiempoDias);//15
                            cmd.Parameters.AddWithValue("@strDesDiaPag", p.desDiaPago.Trim());//16
                            cmd.Parameters.AddWithValue("@dblValMicSeg", p.valorMicroSeg);//17
                            cmd.Parameters.AddWithValue("@sglSalAcuCte", p.salTotPenCte);//18
                            cmd.Parameters.AddWithValue("@strFecUltCre", p.fechaUltCreOto);//19
                            cmd.Parameters.AddWithValue("@dblValCuoPag", p.valCuotaPag);//20
                            cmd.Parameters.AddWithValue("@intNumDiaPPC", p.diaProPagCre);//21
                            cmd.Parameters.AddWithValue("@dblTotPagCre", p.totalPagCre);//22
                            cmd.Parameters.AddWithValue("@strNomCteCre", p.nombreCliente);//23
                            cmd.Parameters.AddWithValue("@strLoginUsSe", usuario); //24
                            cmd.Parameters.AddWithValue("@NotaCredit", p.observaciones); //25

                            CONEXIONMAESTRA.Abrir();
                            cmd.ExecuteReader();
                            cmd.Parameters.Clear();

                            prestamo.nuevo = 0;
                            var respuesta = App.SQLiteDB.UpdatePrestamoAsync(prestamo);
                        }
                        CONEXIONMAESTRA.Cerrar();
                    }
                }
            }
            catch (Exception ex)
            {
                //_ = DisplayAlert("error", ex.Message, "OK");
                Console.WriteLine(ex.Message);
            }
            finally { CONEXIONMAESTRA.Cerrar(); }
        }
    }
}
