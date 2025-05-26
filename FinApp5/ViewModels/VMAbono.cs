using FinApp5.Conexiones;
using FinApp5.Modelo;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinApp5.ViewModels
{
    public class VMAbono : BaseViewModel
    {
        //private readonly ConexionService _conexionService;
        private readonly Musuarios? Usuario;
        public VMAbono(INavigation? navigation, Musuarios? usuario)
        {
            Navigation = navigation;
            Usuario = usuario;
        }

        public int SincronizarAbono(Mmovimiento mmovimiento, Musuarios Usuario)
        {
            int row = 0;
            try
            {
                if (CONEXIONMAESTRA.VerificarCon())
                {
                    CONEXIONMAESTRA.Abrir();
                    SqlCommand cmd = new SqlCommand("RegistraAboMovCon", CONEXIONMAESTRA.conectar);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@strCodigoRut", Usuario.CodigoCobr);
                    cmd.Parameters.AddWithValue("@strCodTipMov", "98");
                    cmd.Parameters.AddWithValue("@strCodConMov", "88888");
                    cmd.Parameters.AddWithValue("@dblValAboCre", mmovimiento.ValorMovto);
                    cmd.Parameters.AddWithValue("@strObservaRA", mmovimiento.strObservaRA);
                    cmd.Parameters.AddWithValue("@strNombreCte", mmovimiento.NombreCteCre);
                    cmd.Parameters.AddWithValue("@lngNumCreAfe", mmovimiento.NumeroCreAfe);
                    cmd.Parameters.AddWithValue("@strLoginUsSe", Usuario.Usuario);
                    cmd.Parameters.AddWithValue("@strComentAbo", "");
                    row = cmd.ExecuteNonQuery() * -1;
                    if (row > 0)
                        App.SQLiteDB.marcarAbonoSincronizado(mmovimiento.idMovimiento);
                }
            }
            catch (Exception ex)
            {
                Task task = DisplayAlert("Error", "Error" + ex.Message, "OK");
                throw;
            }
            finally { CONEXIONMAESTRA.Cerrar(); }
            return row;
        }

        public async Task SincronizarAbonosAsync()
        {
            if (Usuario?.CodigoCobr != null && CONEXIONMAESTRA.VerificarCon())
            {
               await App.SQLiteDB.SincronizarClientes(Usuario.CodigoCobr); // Inserta nuevos clientes en el servidor 

                if (!string.IsNullOrEmpty(Usuario?.Usuario))
                {
                   await App.SQLiteDB.SincronizarCreditos(Usuario.Usuario); // Inserta nuevos créditos en el servidor
                }
                else
                {
                    Console.WriteLine("⚠️ Error: Usuario.Usuario es null.");
                }

                int movimientoNew = await App.SQLiteDB.CountNewAbonos(); // ✅ Ahora correctamente asíncrono

                if (movimientoNew > 0)
                {
                    int row = 0;
                    var movimiento = await App.SQLiteDB.GetAbonosNewOffLine(); // ✅ Aseguramos que sea asíncrono
                    VMAbono abono = new(null, Usuario);

                    foreach (var item in movimiento)
                    {
                        row += await Task.Run(() => abono.SincronizarAbono(item, Usuario));
                    }

                    await DisplayAlert("Exitoso", $"{row} Registro(s) guardado(s) con éxito", "OK");
                }
            }
        }

        private void OnConexionCambiada(bool tieneInternet)
        {
            if (!tieneInternet)
            {
                Task task = DisplayAlert("Conexión", "Te quedaste sin internet.", "OK");
            }
            else
            {
                Task task = DisplayAlert("Conexión", "Conexión establecida.", "OK");
            }
        }
    }
}
