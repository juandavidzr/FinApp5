using Controls.UserDialogs.Maui;
using FinApp5.Conexiones;
using FinApp5.Modelo;
using Microsoft.Data.SqlClient;
using System;
using System.Data;

//using Xamarin.Forms.OpenWhatsApp;

namespace FinApp5.Views;

public partial class Abonos : ContentPage
{
    Musuarios Usuario = new Musuarios();
    Mmovimiento abono = new Mmovimiento();
    Prestamos p = new Prestamos();
    public Abonos(Prestamos Prestamo, Musuarios usuario)
    {
        InitializeComponent();
        Usuario = usuario;
        formasPagos = GetFormasPago();
        cmbFormaPago.ItemsSource = formasPagos;
        tiposAbonos = GetTiposAbono();
        cmbTipoAbono.ItemsSource = tiposAbonos;

        txtId.Text = Prestamo.idCliente;
        txtNombre.Text = Prestamo.nombreCliente;
        txtDireccion.Text = Prestamo.DireccionCobro;
        txtTelefono.Text = Prestamo.TelefonoCell;

        txtIdCredito.Text = Prestamo.NumPrestamo.ToString();
        txtAtrasadas.Text = Prestamo.numCuoAtra.ToString();
        txtFechaCredito.Date = Convert.ToDateTime(Prestamo.fechaPrestamo);
        txtUltimoPago.Text = Prestamo.valUltPag.ToString();
        txtValorCuota.Text = Prestamo.valCuotaPag.ToString();
        txtDia.Text = Prestamo.desDiaPago;
        txtCuotasPendientes.Text = Prestamo.numCuoPen.ToString();
        txtSaldo.Text = Prestamo.saldoActualCre.ToString();
        txtTotal.Text = Prestamo.totalPagCre.ToString();
        txtAbono.Text = Prestamo.valCuotaPag.ToString();
        cmbFormaPago.SelectedIndex = 0;
        cmbTipoAbono.SelectedIndex = 0;
        txtObservaciones.Text = " ";
        btnGrabar.IsEnabled = true;
        p = Prestamo;
    }

    private bool validarDatos() //
    {
        bool respuesta;
        if (string.IsNullOrEmpty(txtAbono.Text))
        {
            respuesta = false;
        }
        else
        {
            int abono = int.Parse(txtAbono.Text);
            int saldo = int.Parse(txtSaldo.Text);
            if (abono > saldo)
                respuesta = false;
            else
                respuesta = true;
        }

        return respuesta;
    }
    private async void btnGrabar_Clicked(object sender, EventArgs e)
    {
        try
        {
            UserDialogs.Instance.Loading();
            await Task.Delay(1000);
            if (validarDatos())
            {
                btnGrabar.IsEnabled = false;
                if ((cmbTipoAbono.SelectedItem as TiposAbono)?.tipoAbono == "Microseguro")
                {
                    abono.strCodTipMov = "02";
                    abono.strCodConMov = "00013";
                }
                else
                {
                    abono.strCodTipMov = "98";
                    abono.strCodConMov = "88888";
                }


               await GrabarAbonoAsync(p, abono);

                // Original line causing the error
                

                // Fixed line
                int saldo = int.TryParse(txtSaldo.Text, out int parsedSaldo) ? parsedSaldo : 0;
                saldo -= Convert.ToInt32(txtAbono.Text.Trim());
                
                txtSaldo.Text = saldo.ToString();

                if (CONEXIONMAESTRA.VerificarCon())
                    await Navigation.PushAsync(new VerAbonos(txtIdCredito.Text, txtNombre.Text, txtSaldo.Text, Usuario));
                else
                    await Navigation.PushAsync(new ListaCreditos(Usuario));
            }
            else
            {
                await DisplayAlert("ERROR", "Por favor digite un valor de abono valido", "OK");
            }
            UserDialogs.Instance.HideHud();
        }
        catch (Exception ex)
        {
            await DisplayAlert("error", ex.Message, "OK");
            throw;
        }
        finally { CONEXIONMAESTRA.Cerrar(); }
    }

    //private async Task GrabarAbonoAsync(Prestamos p, Mmovimiento abono)
    //{
    //    try
    //    {
    //        if (CONEXIONMAESTRA.VerificarCon())
    //        {
    //            await Task.Run(() =>
    //            {
    //                CONEXIONMAESTRA.Abrir();
    //                using (SqlCommand cmd = new SqlCommand("RegistraAboMovCon1", CONEXIONMAESTRA.conectar))
    //                {
    //                    cmd.CommandType = CommandType.StoredProcedure;
    //                    cmd.Parameters.AddWithValue("@strCodigoRut", Usuario.CodigoCobr);
    //                    cmd.Parameters.AddWithValue("@strCodTipMov", abono.strCodTipMov);
    //                    cmd.Parameters.AddWithValue("@strCodConMov", abono.strCodConMov);
    //                    cmd.Parameters.AddWithValue("@dblValAboCre", txtAbono.Text.Trim());
    //                    cmd.Parameters.AddWithValue("@strObservaRA", txtObservaciones.Text.Trim());
    //                    cmd.Parameters.AddWithValue("@strNombreCte", p.nombreCliente);
    //                    cmd.Parameters.AddWithValue("@lngNumCreAfe", p.NumPrestamo);
    //                    cmd.Parameters.AddWithValue("@strLoginUsSe", Usuario.NombApel);
    //                    cmd.Parameters.AddWithValue("@strComentAbo", "abono desde nueva app");

    //                    using (var reader = cmd.ExecuteReader())
    //                    {
    //                        if (reader.Read())
    //                        {
    //                            int update4 = Convert.ToInt32(reader["AfectóUpdateReaMenorOIgualACero"]);
    //                            int update5 = Convert.ToInt32(reader["AfectóUpdateUltimoPago"]);

    //                            if (update4 > 0 && update5 > 0)
    //                            {
    //                                GrabarOffLine(abono, 0);
    //                            }
    //                        }
    //                    }
    //                }
    //            });

    //            await DisplayAlert("Registro guardado", "Registro guardado con éxito", "OK");
    //        }
    //        else
    //        {
    //            await DisplayAlert("Conexión", "Estas trabajando sin conexión", "OK");
    //            GrabarOffLine(abono, 1);
    //        }
    //    }
    //    catch (Exception ex)
    //    {
    //        await DisplayAlert("Error", "Error " + ex.Message, "OK");
    //        throw;
    //    }
    //    finally
    //    {
    //        CONEXIONMAESTRA.Cerrar();
    //    }
    //}

    private async Task GrabarAbonoAsync(Prestamos p, Mmovimiento abono)
    {
        try
        {
            if (CONEXIONMAESTRA.VerificarCon())
            {
                await Task.Run(() =>
                {
                    using (SqlConnection con = CONEXIONMAESTRA.GetConnection())
                    using (SqlCommand cmd = new SqlCommand("RegistraAboMovCon1", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@strCodigoRut", Usuario.CodigoCobr);
                        cmd.Parameters.AddWithValue("@strCodTipMov", abono.strCodTipMov);
                        cmd.Parameters.AddWithValue("@strCodConMov", abono.strCodConMov);
                        cmd.Parameters.AddWithValue("@dblValAboCre", txtAbono.Text.Trim());
                        cmd.Parameters.AddWithValue("@strObservaRA", txtObservaciones.Text.Trim());
                        cmd.Parameters.AddWithValue("@strNombreCte", p.nombreCliente);
                        cmd.Parameters.AddWithValue("@lngNumCreAfe", p.NumPrestamo);
                        cmd.Parameters.AddWithValue("@strLoginUsSe", Usuario.NombApel);
                        cmd.Parameters.AddWithValue("@strComentAbo", "abono desde nueva app");

                        con.Open();
                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                int update4 = Convert.ToInt32(reader["AfectóUpdateReaMenorOIgualACero"]);
                                int update5 = Convert.ToInt32(reader["AfectóUpdateUltimoPago"]);

                                if (update4 > 0 && update5 > 0)
                                {
                                    GrabarOffLine(abono, 0);
                                }
                            }
                        }
                    }
                });

                await DisplayAlert("Registro guardado", "Registro guardado con éxito", "OK");
            }
            else
            {
                await DisplayAlert("Conexión", "Estas trabajando sin conexión", "OK");
                GrabarOffLine(abono, 1);
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", "Error " + ex.Message, "OK");
            throw;
        }
    }


    //private void GrabarAbono(Prestamos p, Mmovimiento abono)
    //{
    //    try
    //    {
    //        if (CONEXIONMAESTRA.VerificarCon())
    //        {
    //            using (SqlConnection con = CONEXIONMAESTRA.GetConnection())
    //            {
    //                con.Open();

    //                using (SqlCommand cmd = new SqlCommand("RegistraAboMovCon1", con))
    //                {
    //                    cmd.CommandType = CommandType.StoredProcedure;

    //                    cmd.Parameters.AddWithValue("@strCodigoRut", Usuario.CodigoCobr ?? string.Empty);
    //                    cmd.Parameters.AddWithValue("@strCodTipMov", abono.strCodTipMov ?? string.Empty);
    //                    cmd.Parameters.AddWithValue("@strCodConMov", abono.strCodConMov ?? string.Empty);
    //                    cmd.Parameters.AddWithValue("@dblValAboCre", txtAbono.Text.Trim());
    //                    cmd.Parameters.AddWithValue("@strObservaRA", txtObservaciones.Text.Trim());
    //                    cmd.Parameters.AddWithValue("@strNombreCte", p.nombreCliente ?? string.Empty);
    //                    cmd.Parameters.AddWithValue("@lngNumCreAfe", p.NumPrestamo);
    //                    cmd.Parameters.AddWithValue("@strLoginUsSe", Usuario.NombApel ?? string.Empty);
    //                    cmd.Parameters.AddWithValue("@strComentAbo", "abono desde nueva app");

    //                    using (SqlDataReader reader = cmd.ExecuteReader())
    //                    {
    //                        if (reader.Read())
    //                        {
    //                            int update4 = Convert.ToInt32(reader["AfectóUpdateReaMenorOIgualACero"]);
    //                            int update5 = Convert.ToInt32(reader["AfectóUpdateUltimoPago"]);

    //                            if (update4 > 0 && update5 > 0)
    //                            {
    //                                GrabarOffLine(abono, 0);
    //                            }
    //                        }
    //                    }
    //                }
    //            }

    //            DisplayAlert("Registro guardado", "Registro guardado con éxito", "OK");
    //        }
    //        else
    //        {
    //            DisplayAlert("Conexión", "Estás trabajando sin conexión", "OK");
    //            GrabarOffLine(abono, 1);
    //        }
    //    }
    //    catch (Exception ex)
    //    {
    //        DisplayAlert("Error", "Error: " + ex.Message, "OK");
    //        throw;
    //    }
    //}

    //private void GrabarAbono(Prestamos p, Mmovimiento abono)
    //{
    //    bool Estado = CONEXIONMAESTRA.VerificarCon();
    //    try
    //    {
    //        if (Estado)
    //        {
    //            CONEXIONMAESTRA.Abrir();
    //            SqlCommand cmd = new SqlCommand("RegistraAboMovCon1", CONEXIONMAESTRA.conectar);
    //            cmd.CommandType = CommandType.StoredProcedure;
    //            cmd.Parameters.AddWithValue("@strCodigoRut", Usuario.CodigoCobr);
    //            cmd.Parameters.AddWithValue("@strCodTipMov", abono.strCodTipMov);
    //            cmd.Parameters.AddWithValue("@strCodConMov", abono.strCodConMov);
    //            cmd.Parameters.AddWithValue("@dblValAboCre", txtAbono.Text.Trim());
    //            cmd.Parameters.AddWithValue("@strObservaRA", txtObservaciones.Text.Trim());
    //            cmd.Parameters.AddWithValue("@strNombreCte", p.nombreCliente);
    //            cmd.Parameters.AddWithValue("@lngNumCreAfe", p.NumPrestamo);
    //            cmd.Parameters.AddWithValue("@strLoginUsSe", Usuario.NombApel);
    //            cmd.Parameters.AddWithValue("@strComentAbo", "abono desde nueva app");
    //            var reader = cmd.ExecuteReader();
    //            if (reader.Read())
    //            {

    //                //int update1 = Convert.ToInt32(reader["AfectóUpdateSaldoMayorACero"]);
    //                //int update2 = Convert.ToInt32(reader["AfectóUpdateSaldoMenorOIgualACero"]);
    //                //int update3 = Convert.ToInt32(reader["AfectóUpdateReaMayorACero"]);
    //                int update4 = Convert.ToInt32(reader["AfectóUpdateReaMenorOIgualACero"]);
    //                int update5 = Convert.ToInt32(reader["AfectóUpdateUltimoPago"]);


    //                if (update4 > 0 && update5 > 0)
    //                {
    //                    GrabarOffLine(abono, 0);
    //                }                    
    //            }

    //            DisplayAlert("Registro guardado", "Registo guardado con exito", "OK");
    //        }
    //        else
    //        {
    //            DisplayAlert("Conexion", "Estas trabajando sin conexion", "OK");
    //            GrabarOffLine(abono, 1 );
    //        }
    //    }
    //    catch (Exception ex)
    //    {
    //        DisplayAlert("Error", "Error" + ex.Message, "OK");
    //        throw;
    //    }
    //    finally { CONEXIONMAESTRA.Cerrar(); }
    //}

    private void GrabarOffLine(Mmovimiento abono , int nuevoparam = 1)
    //private async Task GrabarOffLineAsync(Mmovimiento abono, int nuevoparam = 1)
    {
        abono = new Mmovimiento
        {
            strCodigoRut = Usuario.CodigoCobr,
            strCodTipMov = abono.strCodTipMov,
            strCodConMov = abono.strCodConMov,
            ValorMovto = Convert.ToInt32(txtAbono.Text),
            strObservaRA = txtObservaciones.Text,
            NombreCteCre = txtNombre.Text,
            NumeroCreAfe = txtIdCredito.Text,
            strLoginUsSe = Usuario.NombApel,
            strComentAbo = txtObservaciones.Text,
            Descripcion = (cmbTipoAbono.SelectedItem as TiposAbono)?.tipoAbono,
            nuevo = nuevoparam
        };
       App.SQLiteDB.SaveAbono(abono);

        int nuevoSaldo;
        if (abono.strCodTipMov == "02")
            nuevoSaldo = Convert.ToInt32(txtSaldo.Text);
        else
            nuevoSaldo = Convert.ToInt32(txtSaldo.Text) - Convert.ToInt32(txtAbono.Text);

        Prestamos prestamo = new Prestamos { };

        if (nuevoSaldo > 0)
        {
            prestamo = new Prestamos
            {
                NumPrestamo = Convert.ToInt32(txtIdCredito.Text),
                saldoActualCre = nuevoSaldo,
                marAboCreDia = 1,
                valUltPag = Convert.ToInt32(txtAbono.Text),
                fechaCancelacion = "01/01/0001"
            };
        }
        else
        {
            prestamo = new Prestamos
            {
                NumPrestamo = Convert.ToInt32(txtIdCredito.Text),
                saldoActualCre = 0,
                marAboCreDia = 1,
                valUltPag = Convert.ToInt32(txtAbono.Text),
                fechaCancelacion = DateTime.Today.ToString("dd/MM/yyyy")
            };
        }
        Thread.Sleep(100);
        var respuesta = App.SQLiteDB.UpdatePrestamos(prestamo);
       //await App.SQLiteDB.UpdatePrestamos(prestamo);

        if (!respuesta)
        {
            DisplayAlert("ERROR", "El registro NO se guardó", "OK");
            return;
        }

        if (!CONEXIONMAESTRA.VerificarCon())
        {            
                DisplayAlert("Registro", "El registro se guardo localmente de manera exitosa", "OK");
        }
    }

    public class TiposAbono
    {
        public int idTipoAbono { get; set; }
        public string? tipoAbono { get; set; }
    }
    public List<TiposAbono> tiposAbonos { get; set; }
    private List<TiposAbono> GetTiposAbono()
    {
        var tiposAbono = new List<TiposAbono>
            {
                new TiposAbono(){idTipoAbono=0, tipoAbono = "Recaudo"},
                new TiposAbono(){idTipoAbono=1, tipoAbono = "Microseguro"}
            };
        return tiposAbono;
    }

    public class FormasPago
    {
        public int id { get; set; }
        public string? formaPago { get; set; }
    }
    public List<FormasPago> formasPagos { get; set; }
    private List<FormasPago> GetFormasPago()
    {
        var formaLista = new List<FormasPago>
            {
                new FormasPago(){id = 0, formaPago = "Efectivo"}
            };
        return formaLista;
    }

    private void btnRetaque_Clicked(object sender, EventArgs e)
    {
        try
        {
            using (SqlConnection con = CONEXIONMAESTRA.GetConnection())
            {
                con.Open();

                using (SqlCommand cmd = new SqlCommand("MarcarCreditoParaRetaquePosterior", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@lngNumeroCredito", txtIdCredito.Text);

                    // Como no usas resultados, mejor ExecuteNonQuery en lugar de ExecuteReader
                    cmd.ExecuteNonQuery();
                }
            }

            Navigation.PushAsync(new ListaCreditos(Usuario));
        }
        catch (Exception ex)
        {
            // Mejor lanzar la excepción directamente para conservar la pila
            throw;
        }
    }


    //private void btnRetaque_Clicked(object sender, EventArgs e)
    //{
    //    try
    //    {
    //        CONEXIONMAESTRA.Abrir();
    //        SqlCommand cmd = new SqlCommand("MarcarCreditoParaRetaquePosterior", CONEXIONMAESTRA.conectar);
    //        cmd.CommandType = CommandType.StoredProcedure;
    //        cmd.Parameters.AddWithValue("@lngNumeroCredito", txtIdCredito.Text);
    //        cmd.ExecuteReader();
    //        CONEXIONMAESTRA.Cerrar();
    //        Navigation.PushAsync(new ListaCreditos(Usuario));
    //    }
    //    catch (Exception ex)
    //    {
    //        throw ex;
    //    }
    //    finally {  }
    //}
    private void btnPagos_Clicked(object sender, EventArgs e)
    {

        if (CONEXIONMAESTRA.VerificarCon())
            Navigation.PushAsync(new VerAbonos(txtIdCredito.Text, txtNombre.Text, txtSaldo.Text, Usuario));
        else
            DisplayAlert("Sin Internet", "Esta opción no esta disponible sin intenet (289)", "OK");
    }
    private void Button_WhatsApp(object sender, EventArgs e)
    {
        try
        {
            string phoneNumber = txtTelefono?.Text?.Trim() ?? "";
            WhatsApp(phoneNumber, "Hola");
        }
        catch (Exception ex)
        {
            DisplayAlert("Error", "Error" + ex.Message, "OK");
            throw;
        }
    }
    private async void WhatsApp(string phoneNumber, string? message)
    {
        try
        {
            if (!string.IsNullOrEmpty(phoneNumber))
            {
                string url = $"https://wa.me/{phoneNumber}?text={Uri.EscapeDataString(message ?? "")}";
                await Launcher.OpenAsync(new Uri(url));
            }
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    private void Button_Clicked_Llamar(object sender, EventArgs e)
    {
        try
        {
            Launcher.OpenAsync(new Uri("tel:" + txtTelefono.Text));
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    private void Navegar_Clicked(object sender, EventArgs e)
    {
        try
        {
            using (SqlConnection con = CONEXIONMAESTRA.GetConnection())
            {
                con.Open();

                using (SqlCommand cmd = new SqlCommand("consultarUbicacion", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@idCliente", txtId.Text.Trim());

                    using (SqlDataReader rdr = cmd.ExecuteReader())
                    {
                        if (rdr.Read())
                        {
                            var longitud = rdr["longitud"].ToString();
                            var latitud = rdr["latitud"].ToString();

                            if (!string.IsNullOrWhiteSpace(longitud) && !string.IsNullOrWhiteSpace(latitud))
                            {
                                var ubi = $"https://waze.com/ul?q=your address&ll={latitud},{longitud}&navigate=yes";
                                Launcher.OpenAsync(new Uri(ubi));
                            }
                            else
                            {
                                DisplayAlert("Sin Información", "No tiene la ubicación de este cliente guardada", "OK");
                            }
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error en Navegar_Clicked: {ex.Message}");
        }
    }

    //private void Navegar_Clicked(object sender, EventArgs e)
    //{
    //    try
    //    {
    //        CONEXIONMAESTRA.Abrir();
    //        SqlCommand cmd = new SqlCommand("consultarUbicacion", CONEXIONMAESTRA.conectar);
    //        cmd.CommandType = CommandType.StoredProcedure;
    //        cmd.Parameters.AddWithValue("@idCliente", txtId.Text.Trim());
    //        SqlDataReader rdr = cmd.ExecuteReader();
    //        if (rdr.Read())
    //        {
    //            var longitud = rdr["longitud"].ToString();
    //            var latitud = rdr["latitud"].ToString();
    //            if (!string.IsNullOrWhiteSpace(longitud) && !string.IsNullOrWhiteSpace(latitud))
    //            {
    //                var ubi = "https://waze.com/ul?q=your address&ll=" + latitud + "," + longitud + "&navigate=yes";
    //                Launcher.OpenAsync(new Uri(ubi));
    //            }
    //            else
    //            {
    //                DisplayAlert("Sin Información", "No tiene la ubicación de este cliente guardada", "OK");
    //            }
    //        }
    //    }
    //    catch (Exception ex)
    //    {
    //        Console.WriteLine(ex.Message);
    //    }
    //    finally { CONEXIONMAESTRA.Cerrar(); }
    //}

    double _lastScrollY = 0;
    double _maxScrollY = 0;
    private void OnScrollViewScrolled(object sender, ScrolledEventArgs e)
    {
        //if (e.ScrollY > _lastScrollY || e.ScrollY >= _maxScrollY)
        HideKeyboard();
        _lastScrollY = e.ScrollY;
    }
    private void HideKeyboard()
    {
    #if ANDROID
        var context = Android.App.Application.Context;
        var inputMethodManager = (Android.Views.InputMethods.InputMethodManager)context.GetSystemService(Android.Content.Context.InputMethodService);
        var activity = Microsoft.Maui.ApplicationModel.Platform.CurrentActivity;
        var token = activity?.CurrentFocus?.WindowToken;
        inputMethodManager?.HideSoftInputFromWindow(token, Android.Views.InputMethods.HideSoftInputFlags.None);
    #elif IOS
            UIKit.UIApplication.SharedApplication.SendAction(new ObjCRuntime.Selector("resignFirstResponder"), null, null, null);
    #endif
    }
    private void OnScrollViewSizeChanged(object sender, EventArgs e)
    {
        if (sender is ScrollView scrollView)
        {
            _maxScrollY = scrollView.ContentSize.Height - scrollView.Height;
        }
    }

    private void OnTapGestureRecognizerTapped(object sender, TappedEventArgs e)
    {
        HideKeyboard();
    }

    private async void btnVolver_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new ListaCreditos(Usuario));
    }
}