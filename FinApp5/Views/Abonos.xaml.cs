using Controls.UserDialogs.Maui;
using FinApp5.Conexiones;
using FinApp5.Modelo;
using Microsoft.Data.SqlClient;
using System.Data;

namespace FinApp5.Views;

public partial class Abonos : ContentPage
{
    Musuarios Usuario = new Musuarios();
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
        txtAbono.Text = Prestamo.valCuotaPag.ToString();
        cmbFormaPago.SelectedIndex = 0;
        cmbTipoAbono.SelectedIndex = 0;
        txtObservaciones.Text = " ";
        btnGrabar.IsEnabled = true;
        p = Prestamo;
    }

    private bool validarDatos()
    {
        bool respuesta;
        if (string.IsNullOrEmpty(txtAbono.Text))
            respuesta = false;
        else
            respuesta = true;

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
                GrabarAbono(p);
                await DisplayAlert("Registro guardado", "Registo guardado con exito", "OK");
                
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
    private void GrabarAbono(Prestamos p)
    {
        bool Estado = CONEXIONMAESTRA.VerificarCon();
        try
        {
            if (Estado)
            {
                //DisplayAlert("Conexion camino 1", "Estas trabajando sin conexion", "OK");
                CONEXIONMAESTRA.Abrir();
                SqlCommand cmd = new SqlCommand("RegistraAboMovCon", CONEXIONMAESTRA.conectar);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@strCodigoRut", Usuario.CodigoCobr);
                cmd.Parameters.AddWithValue("@strCodTipMov", "98");
                cmd.Parameters.AddWithValue("@strCodConMov", "88888");
                cmd.Parameters.AddWithValue("@dblValAboCre", txtAbono.Text.Trim());
                cmd.Parameters.AddWithValue("@strObservaRA", txtObservaciones.Text.Trim());
                cmd.Parameters.AddWithValue("@strNombreCte", p.nombreCliente);
                cmd.Parameters.AddWithValue("@lngNumCreAfe", p.NumPrestamo);
                cmd.Parameters.AddWithValue("@strLoginUsSe", Usuario.NombApel);
                cmd.Parameters.AddWithValue("@strComentAbo", "abono desde iphone");
                cmd.ExecuteReader();
            }
            else
            {
                DisplayAlert("Conexion", "Estas trabajando sin conexion", "OK");
                GrabarOffLine();
            }
        }
        catch (Exception ex)
        {
            DisplayAlert("Error", "Error" + ex.Message, "OK");
            throw;
        }
        finally { CONEXIONMAESTRA.Cerrar(); }
    }

    private void GrabarOffLine()
    {
        Mmovimiento abono = new Mmovimiento
        {
            strCodigoRut = Usuario.CodigoCobr,
            strCodTipMov = "98",
            strCodConMov = "8888",
            ValorMovto = Convert.ToInt32(txtAbono.Text),
            strObservaRA = txtObservaciones.Text,
            NombreCteCre = txtNombre.Text,
            NumeroCreAfe = txtIdCredito.Text,
            strLoginUsSe = Usuario.NombApel,
            strComentAbo = "Abono Off-Line",
            nuevo = 1
        };
        App.SQLiteDB.SaveAbono(abono);

        int nuevoSaldo = Convert.ToInt32(txtSaldo.Text) - Convert.ToInt32(txtAbono.Text);
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
        var respuesta = App.SQLiteDB.UpdatePrestamos(prestamo);

        if (respuesta)
            DisplayAlert("Registro", "El registro se guardo de manera exitosa", "OK");
        else
            DisplayAlert("ERROR", "El registro NO se guardo ", "OK");
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
                //new TiposAbono(){idTipoAbono=1, tipoAbono = "Microseguro"}
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
            CONEXIONMAESTRA.Abrir();
            SqlCommand cmd = new SqlCommand("MarcarCreditoParaRetaquePosterior", CONEXIONMAESTRA.conectar);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@lngNumeroCredito", txtIdCredito.Text);
            cmd.ExecuteReader();
        }
        catch (Exception ex)
        {
            throw ex;
        }
        finally { CONEXIONMAESTRA.Cerrar(); }
    }
}