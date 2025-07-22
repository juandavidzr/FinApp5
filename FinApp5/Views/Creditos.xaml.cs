using FinApp5.Conexiones;
using FinApp5.Modelo;
using Microsoft.Data.SqlClient;
using Microsoft.Maui.Controls.PlatformConfiguration;
using System.Data;
using System.Globalization;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Devices;

namespace FinApp5.Views;

public partial class Creditos : ContentPage
{
    Musuarios Usuario = new Musuarios();
    Mcliente Cliente = new Mcliente();
    private const string DePrimero = "De Primero";
    private const string PosiciónActual = "Posición Actual";
    private const string DeUltimo = "De Ultimo";
    double _lastScrollY = 0;
    double _maxScrollY = 0;
    public Creditos(Mcliente cliente, Musuarios usuario)
    {
        InitializeComponent();
        Usuario = usuario;
        Cliente = cliente;
        txtidCliente.Text = cliente.cteNumIdenti;
        txtNombreCli.Text = cliente.cteNombApel;
        ConsultarCliente(cliente.cteNumIdenti, usuario.CodigoCobr, Cliente);
        PlazoList = GetPlazos();
        cmbPlazo.ItemsSource = PlazoList;
        diasList = GetDias();
        cmbDias.ItemsSource = diasList;


        llenarRuta();

        cmbPosicion.SelectedIndex = 0;
        cmbPlazo.SelectedIndex = 0;
        cmbDias.SelectedIndex = 0;
    }

    private async Task ConsultarCliente(string? cteNumIdenti, string? CodigoCobr, Mcliente cliente)
    {
        try
        {
            double dblSalAcuCte = 0;
            int intCanCreVigCte = 0;
            double dblMonto = 0;
            double dblSaldo = 0;
            DateTime dteFechaAux = DateTime.Now;
            DateTime dteFecUltCre = DateTime.Now;
            string strFormatoNum = string.Empty;
            if (CONEXIONMAESTRA.VerificarCon())
            {
                CONEXIONMAESTRA.Abrir();
                SqlCommand cmd = new SqlCommand("FiltrarInformacionPersonalDeCliente", CONEXIONMAESTRA.conectar);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@strCedulaCteOC", cteNumIdenti);
                cmd.Parameters.AddWithValue("@strCodigoRuta", CodigoCobr);
                SqlDataReader rdr = cmd.ExecuteReader();
                if (rdr.HasRows)
                {
                    while (rdr.Read())
                    {
                        dblMonto = Convert.ToDouble(rdr["pmoCantidadPre"].ToString().Trim());
                        dteFecUltCre = Convert.ToDateTime(rdr["pmoFechaUltCreOto"].ToString().Trim());
                        dteFechaAux = Convert.ToDateTime(rdr["pmoFecUltPag"].ToString().Trim());
                        if ((Convert.ToInt16(rdr["pmoVigente"].ToString().Trim()) == 1) &&
                            ((Convert.ToInt16(rdr["pmoActivo"].ToString().Trim()) == 1)))
                        {
                            dblSaldo = Convert.ToDouble(rdr["pmoSaldoActualCte"].ToString().Trim());
                            dblSalAcuCte += dblSaldo;
                            intCanCreVigCte++;
                        }
                    }
                }
                else
                {
                    //DisplayAlert("No hay datos", "No hay datos", "OK");
                }
            }
            else
            {
                await DisplayAlert("Sin Internet", "Esta trabajando sin internet (83)", "OK");
                App.SQLiteDB.FiltrarInformacionPersonalDeCliente(cliente.cteNumIdenti, CodigoCobr, out dblSalAcuCte,
                    out intCanCreVigCte, out dblMonto, out dteFecUltCre, out dteFechaAux);
            }

            txtCreditos.Text = intCanCreVigCte.ToString().Trim();
            if (dblSalAcuCte >= 1000)
                strFormatoNum = "{0:0,0}";
            else
                strFormatoNum = "{0,0}";

            this.txtDeuda.Text = String.Format(CultureInfo.InvariantCulture, strFormatoNum.Trim(), Math.Truncate(dblSalAcuCte));
            this.txtFechaUltimo.Text = dteFecUltCre.ToString("yyyy-MM-dd");
            if (dblMonto >= 1000)
                strFormatoNum = "{0:0,0}";
            else
                strFormatoNum = "{0,0}";
            this.txtValorUltimo.Text = String.Format(CultureInfo.InvariantCulture, strFormatoNum.Trim(), Math.Truncate(dblMonto));
            this.txtUltimoPago.Text = dteFechaAux.ToString("yyyy-MM-dd");
        }
        catch (Exception ex)
        {
            DisplayAlert("error", ex.Message, "OK");
        }
        finally { CONEXIONMAESTRA.Cerrar(); }
    }

    private async void llenarRuta()
    {
        try
        {
            
            var codigoRuta = Usuario.CodigoCobr;
            if (CONEXIONMAESTRA.VerificarCon())
            {
                CONEXIONMAESTRA.Abrir();
                SqlCommand cmd = new SqlCommand("ObtenerRutaActualDeCobrador", CONEXIONMAESTRA.conectar);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@strCodigoRuta", codigoRuta);

                SqlDataReader rdr = cmd.ExecuteReader();
                int intIndice = 1;
                List<Mruta> rutaList = new List<Mruta>();
                rutaList = GetRuta();
                while (rdr.Read())
                {
                    rutaList.Add(new Mruta
                    {
                        nombreCliente = rdr["cteNombApel"].ToString().Trim() + " - " +
                                        rdr["pmoPosRutCre"].ToString().Trim(),
                        posicion = intIndice
                    });
                    intIndice++;
                }
                cmbPosicion.ItemsSource = rutaList;
                CONEXIONMAESTRA.Cerrar();
            }
            else
            {
                List<Mruta> rutaList = new List<Mruta>();
                rutaList = App.SQLiteDB.getRutaAsync().Result;

                if (rutaList != null)
                {
                    rutaList.AddRange(GetRuta());
                }
                 cmbPosicion.ItemsSource = rutaList;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
        finally { }
    }



    public class Plazo
    {
        public string idPlazo { get; set; }
        public string nombrePlazo { get; set; }
    }

    public class Dias
    {
        public int idDia { get; set; }
        public string nombreDia { get; set; }
    }

    public List<Mruta> GetRuta()
    {
        var ruta = new List<Mruta>() 
        {
             new Mruta { nombreCliente = DePrimero, posicion = -1 },
             new Mruta { nombreCliente = PosiciónActual, posicion = -2 },
             new Mruta { nombreCliente = DeUltimo, posicion = -3 }
        };
        return ruta;
    }

    public List<Plazo> PlazoList { get; set; }
    public List<Plazo> GetPlazos()
    {
        var plazos = new List<Plazo>()
            {
                new Plazo(){idPlazo = "01", nombrePlazo="Diario"},
                new Plazo(){idPlazo = "02", nombrePlazo="Semanal"},
                new Plazo(){idPlazo = "03", nombrePlazo="Quincenal"},
                new Plazo(){idPlazo = "04", nombrePlazo="Mensual"}
            };

        return plazos;
    }

    public List<Dias> diasList { get; set; }
    public List<Dias> GetDias()
    {
        var dias = new List<Dias>
            {
                new Dias(){idDia = 1, nombreDia = "Todos"},
                new Dias(){idDia = 2, nombreDia = "Lunes"},
                new Dias(){idDia = 3, nombreDia = "Martes"},
                new Dias(){idDia = 4, nombreDia = "Miercoles"},
                new Dias(){idDia = 5, nombreDia = "Jueves"},
                new Dias(){idDia = 6, nombreDia = "Viernes"},
                new Dias(){idDia = 7, nombreDia = "Sabado"},
                new Dias(){idDia = 8, nombreDia = "Domingo"}
            };
        return dias;
    }

    private void cmbPlazo_SelectedIndexChanged(object sender, EventArgs e)
    {

    }

    private void cmbDias_SelectedIndexChanged(object sender, EventArgs e)
    {

    }

    //private async Task btnGrabar_Clicked(object sender, EventArgs e)
    private void btnGrabar_Clicked(object sender, EventArgs e)
    {
        try
        {
            if (Usuario.CodigoCobr != null && CONEXIONMAESTRA.VerificarCon())
                App.SQLiteDB.SincronizarClientes(Usuario.CodigoCobr); //inserta los nuevos clientes en el servidor 

            var codigoRuta = Usuario.CodigoCobr;

            if (validarDatos())
            {
                btnGrabar.IsEnabled = false;
                var intNumeroCuotas = 0;
                var intSaldoActualCre = 0;
                var intNumCuoPag = 0;
                var dblValCuoPag = 0;
                var intNumCuoPen = 0;
                DateTime dteFechaVenCre = DateTime.MinValue;
                var dblValorCuoPen = 0;
                var intPosCredito = 0;
                DateTime dteFechaUltCreOto = DateTime.Today;
                string strCodPlaCre = string.Empty;
                int found = 0;
                var strPosicion = string.Empty;

                intSaldoActualCre = (Convert.ToInt32(txtDesembolso.Text) * Convert.ToInt32(txtInteres.Text) / 100) + Convert.ToInt32(txtDesembolso.Text);

                switch (cmbPlazo.SelectedIndex)
                {
                    case 0: //Diario
                        strCodPlaCre = "01";
                        intNumeroCuotas = Convert.ToInt32(txtTiempo.Text);
                        break;

                    case 1: //Semanal
                        strCodPlaCre = "02";
                        intNumeroCuotas = Convert.ToInt32(txtTiempo.Text) / 7;
                        break;

                    case 2: //Quincenal
                        strCodPlaCre = "03";
                        intNumeroCuotas = Convert.ToInt32(txtTiempo.Text) / 15;
                        break;

                    case 3: //Mensual
                        strCodPlaCre = "04";
                        intNumeroCuotas = Convert.ToInt32(txtTiempo.Text) / 30;
                        break;

                    default:
                        break;
                }

                if (intNumeroCuotas < 1)
                    intNumeroCuotas = 1;

                dblValCuoPag = intSaldoActualCre / intNumeroCuotas;
                intNumCuoPen = intSaldoActualCre / dblValCuoPag;

                dblValorCuoPen = intSaldoActualCre; // - Convert.ToInt32(txtAbono.Text);


                switch (cmbPosicion.SelectedIndex)
                {
                    case 0://primero
                        intPosCredito = 1;
                        break;
                    case 1://pos actual
                        intPosCredito = BuscarPosicionActualDelCreditoEnRuta();
                        break;
                    case 2: //ultimo
                        intPosCredito = cmbPosicion.Items.Count + 1;
                        break;

                    default:
                        int intIndice = cmbPosicion.SelectedIndex;
                        var cadena = cmbPosicion.Items[intIndice];
                        string[] info = { cadena };
                        foreach (string s in info)
                        {
                            found = s.IndexOf("-");
                            strPosicion = s.Substring(found + 2);
                        }
                        intPosCredito = Convert.ToInt32(strPosicion);
                        break;
                }
                dteFechaVenCre = DateTime.Today.AddDays(Convert.ToInt16(txtTiempo.Text.Trim()));

                NetworkAccess accessType = Connectivity.Current.NetworkAccess;

                if (accessType == NetworkAccess.Internet)
                {
                    var dblNetoEnCre = Convert.ToInt64(txtDesembolso.Text.Trim());
                    var dblPorIntCre = Convert.ToInt64(txtInteres.Text.Trim());

                    Double dblTotPagCre = (((dblNetoEnCre * dblPorIntCre) / 100) + dblNetoEnCre);
                    CONEXIONMAESTRA.Abrir();
                    SqlCommand cmd = new SqlCommand("GrabaCredito", CONEXIONMAESTRA.conectar);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@strCodigRut", Usuario.CodigoCobr); // "00001"); // 
                    cmd.Parameters.AddWithValue("@strNumIdeCte", txtidCliente.Text.Trim()); //   "01"); // txtidCliente.Text.Trim());//2
                    cmd.Parameters.AddWithValue("@dblNetoEnCre", dblNetoEnCre); //  dblNetoEnCre);//3
                    cmd.Parameters.AddWithValue("@dblPorIntCre", dblPorIntCre); //  dblPorIntCre);//4
                    cmd.Parameters.AddWithValue("@strCodPlaPac", strCodPlaCre); //   "01"); // cmbPlazo.SelectedIndex.ToString()); //5
                    cmd.Parameters.AddWithValue("@intNumCuoCre", intNumeroCuotas); //  intNumeroCuotas);//6
                    cmd.Parameters.AddWithValue("@intNumCreVig", "0"); //   0);//7
                    cmd.Parameters.AddWithValue("@dblSaldoAcCr", intSaldoActualCre); //  dblNetoEnCre);//8
                    cmd.Parameters.AddWithValue("@intNumCuoPag", intNumCuoPag); //  intNumCuoPag);//9
                    cmd.Parameters.AddWithValue("@intNumCuoPen", intNumCuoPen); //  intNumCuoPen);//10
                    cmd.Parameters.AddWithValue("@strFecUltPag", DateTime.Today.ToString("yyyy-MM-dd")); //  DateTime.Today.ToString("yyyy-MM-dd"));//11
                    cmd.Parameters.AddWithValue("@dblValUltPag", "0"); //   0);//12
                    cmd.Parameters.AddWithValue("@strFecVtoCre", dteFechaVenCre.ToString("yyyy-MM-dd")); //  dteFechaVenCre);//13
                    cmd.Parameters.AddWithValue("@intPosCreEnr", intPosCredito); //  intPosCredito);//14
                    cmd.Parameters.AddWithValue("@intTieDiaCre", "0"); //   0);//15
                    cmd.Parameters.AddWithValue("@strDesDiaPag", cmbDias.Items[cmbDias.SelectedIndex] + " - " + txtGuiaPago.Text.Trim()); //  txtGuiaPago);//16
                    cmd.Parameters.AddWithValue("@dblValMicSeg", "0"); //   0);//17
                    cmd.Parameters.AddWithValue("@sglSalAcuCte", dblTotPagCre); //  dblTotPagCre);//18
                    cmd.Parameters.AddWithValue("@strFecUltCre", DateTime.Today.ToString("yyyy-MM-dd")); //  DateTime.Today.ToString("yyyy-MM-dd"));//19
                    cmd.Parameters.AddWithValue("@dblValCuoPag", dblValCuoPag); //  dblValCuoPag);//20
                    cmd.Parameters.AddWithValue("@intNumDiaPPC", "0"); //   0);//21
                    cmd.Parameters.AddWithValue("@dblTotPagCre", dblTotPagCre); //  dblTotPagCre);//22
                    cmd.Parameters.AddWithValue("@strNomCteCre", txtNombreCli.Text.Trim()); //  txtNombreCli.Text.Trim());//23
                    cmd.Parameters.AddWithValue("@strLoginUsSe", Usuario.NombApel);  //24
                    cmd.Parameters.AddWithValue("@NotaCredit", txtNotas.Text.Trim()); //  txtNotas.Text.Trim()); //24

                    cmd.ExecuteReader();
                    CONEXIONMAESTRA.Cerrar();
                    DisplayAlert("Credito creado", "Credito creado", "OK");
                }
                else
                {
                    //DisplayAlert("Sin internet", "Esta trabajando sin internet (338)", "OK");

                    var rowid = LastRowID();

                    Prestamos prestamo = new Prestamos
                    {
                        rowid = rowid,
                        NumPrestamo = rowid,
                        idCliente = txtidCliente.Text,
                        nombreCliente = txtNombreCli.Text,
                        fechaPrestamo = DateTime.Today.ToString("yyyy-MM-dd"),
                        codigoRuta = codigoRuta,
                        cantidadPrestada = Convert.ToDouble(txtDesembolso.Text),
                        interes = Convert.ToDouble(txtInteres.Text),
                        codigoPlan = strCodPlaCre,
                        numeroCuotas = intNumeroCuotas,
                        observaciones = txtNotas.Text,
                        vigente = 1,
                        activo = 1,
                        saldoActualCre = intSaldoActualCre,
                        numCuoPag = intNumCuoPag,
                        numCuoPen = intNumCuoPen,
                        fecUltPag = DateTime.Today.ToString("yyyy-MM-dd"),
                        valUltPag = 0, //Convert.ToDouble(txtAbono.Text),
                        fecVenCre = dteFechaVenCre.ToString("yyyy-MM-dd"),
                        numCuoAtra = 0,
                        valorAtrazo = 0,
                        valorCuoPen = dblValorCuoPen,
                        posRutCre = intPosCredito,
                        tiempoDias = Convert.ToInt32(txtTiempo.Text),
                        desDiaPago = cmbDias.Items[cmbDias.SelectedIndex],
                        valorMicroSeg = 0, // Convert.ToDouble(txtMicroSeguro.Text),
                        salTotPenCte = intSaldoActualCre,
                        fechaUltCreOto = dteFechaUltCreOto.ToString("yyyy-MM-dd"),
                        valCuotaPag = dblValCuoPag,
                        diaProPagCre = DateTime.Today.Day,
                        marAboCreDia = 0,
                        totalPagCre = Convert.ToDouble(txtDesembolso.Text) * (1 + Convert.ToDouble(txtInteres.Text) / 100),
                        verificado = 0,
                        IndicaRetaque = 0,
                        DiaSemana = cmbDias.Items[cmbDias.SelectedIndex],
                        nuevo = 1
                    };
                    App.SQLiteDB.SavePrestamoAsync(prestamo);
                    DisplayAlert("Credito creado", "Credito creado localmente", "OK");
                    //btnGrabar.IsEnabled = true;
                }
                Navigation.PushAsync(new ListarClientes(Usuario));
                
            }
            else
            {
                DisplayAlert("Validar datos", "Por favor verifique que toda la información ingresada este completa y sea correcta", "OK");
                btnGrabar.IsEnabled = true;
            }

        }
        catch (Exception ex)
        {
            DisplayAlert("error", ex.Message, "OK");

        }
        finally { CONEXIONMAESTRA.Cerrar(); }
    }
    private int LastRowID()
    {
        var id = App.SQLiteDB.LastRowID() + 1;
        return id;
    }
    protected int BuscarPosicionActualDelCreditoEnRuta()
    {
        int intResult = 1;
        int found;
        var strPosicion = string.Empty;
        try
        {
            for (int intIndice = 1; intIndice < cmbPosicion.Items.Count; intIndice++)
            {
                cmbPosicion.SelectedIndex = intIndice;
                if (cmbPosicion.Items[intIndice].ToString().ToLower().Contains(txtNombreCli.Text.Trim().ToLower()))
                {
                    var cadena = cmbPosicion.Items[intIndice];
                    string[] info = { cadena };
                    var nombre = string.Empty;
                    foreach (string s in info)
                    {
                        found = s.IndexOf("-");
                        nombre = s.Substring(0, found).Trim();
                        if (nombre.Trim().ToLower() == txtNombreCli.Text.Trim().ToLower())
                        {
                            strPosicion = s.Substring(found + 2);
                            intResult = Convert.ToInt32(strPosicion);
                            break;
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            DisplayAlert("error", ex.Message, "OK");
            intResult = 1;
        }
        return intResult;
    }

    private bool validarDatos()
    {
        bool respuesta;
        if (
                string.IsNullOrEmpty(txtidCliente.Text) ||
                string.IsNullOrEmpty(txtDesembolso.Text) ||
                Convert.ToInt32(txtDesembolso.Text) <= 0 ||
                string.IsNullOrEmpty(txtInteres.Text) ||
                Convert.ToDouble(txtInteres.Text) < 0 ||
                Convert.ToDouble(txtInteres.Text) > 100 ||
                string.IsNullOrEmpty(txtTiempo.Text) ||
                cmbDias.SelectedItem.Equals(-1) ||
                cmbPlazo.SelectedItem.Equals(-1)

            )
            respuesta = false;
        else
            respuesta = true;

        return respuesta;
    }

    private void btnRegresar_Clicked(object sender, EventArgs e)
    {
    }
    private void cmbPosicion_SelectedIndexChanged(object sender, EventArgs e)
    {
    }

    private void OnScrollViewScrolled(object sender, ScrolledEventArgs e)
    {
        //if (e.ScrollY > _lastScrollY || e.ScrollY >= _maxScrollY)
        HideKeyboard();
        _lastScrollY = e.ScrollY;
    }
    private void HideKeyboard()
    {
    #if ANDROID
    var activity = Microsoft.Maui.ApplicationModel.Platform.CurrentActivity;
    if (activity?.CurrentFocus != null)
    {
        var inputMethodManager = (Android.Views.InputMethods.InputMethodManager)activity.GetSystemService(Android.Content.Context.InputMethodService);
        inputMethodManager?.HideSoftInputFromWindow(activity.CurrentFocus.WindowToken, Android.Views.InputMethods.HideSoftInputFlags.None);
        activity.CurrentFocus.ClearFocus(); // Asegurar que la vista pierde el foco
    }
    #elif IOS
        //UIKit.UIApplication.SharedApplication.SendAction(new ObjCRuntime.Selector("resignFirstResponder"), null, null, null);
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
}