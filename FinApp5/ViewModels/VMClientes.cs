using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Azure.Identity;
using FinApp5.Conexiones;
using FinApp5.Datos;
using FinApp5.Modelo;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Maui.Devices.Sensors;
using static System.Net.Mime.MediaTypeNames;



namespace FinApp5.ViewModels
{

    public class VMClientes : BaseViewModel
    {
        #region VARIABLES

        string? _txtId;
        string? _txtNombre;
        string? _txtDirDomicilio;
        string? _cmbBarrioDomicilio;
        string? _txtDirCobro;
        string? _txtBarrioCobro;
        string? _txtLatitud;
        string? _txtLongitud;
        string? _txtTelefono1;
        string? _txtTelefono2;
        string? _txtNotas;
        bool ModoEdit = false;
        Musuarios Usuario = new Musuarios();

        #endregion
        #region CONSTRUCTOR
        public VMClientes(INavigation navigation, Musuarios usuario)
        {
            Navigation = navigation;
            Usuario = usuario;
            //BtnUbicacion();
            //llenarBarrios();

           
        }
        #endregion
        #region OBJETOS
        public string? TxtId { get { return _txtId; } set { SetValue(ref _txtId, value); } }
        public string? TxtNombre { get { return _txtNombre; } set { SetValue(ref _txtNombre, value); } }
        public string? TxtDirDomicilio { get { return _txtDirDomicilio; } set { SetValue(ref _txtDirDomicilio, value); } }
        public string? cmbBarrioDomicilio { get { return _cmbBarrioDomicilio; } set { SetValue(ref _cmbBarrioDomicilio, value); } }
        public string? TxtDirCobro { get { return _txtDirCobro; } set { SetValue(ref _txtDirCobro, value); } }
        public string? TxtBarrioCobro { get { return _txtBarrioCobro; } set { SetValue(ref _txtBarrioCobro, value); } }
        public string? TxtLatitud { get { return _txtLatitud; } set { SetValue(ref _txtLatitud, value); } }
        public string? TxtLongitud { get { return _txtLongitud; } set { SetValue(ref _txtLongitud, value); } }
        public string? TxtTelefono1 { get { return _txtTelefono1; } set { SetValue(ref _txtTelefono1, value); } }
        public string? TxtTelefono2 { get { return _txtTelefono2; } set { SetValue(ref _txtTelefono2, value); } }
        public string? TxtNotas { get { return _txtNotas; } set { SetValue(ref _txtNotas, value); } }

        #endregion
        #region PROCESOS

        public async void BtnUbicacion()
        {
            try
            {
                Location? location = await Geolocation.GetLastKnownLocationAsync();
                if (location == null)
                {
                    location = await Geolocation.GetLocationAsync(new GeolocationRequest
                    {
                        DesiredAccuracy = GeolocationAccuracy.High,
                        Timeout = TimeSpan.FromSeconds(30)
                    });
                    if (location != null)
                    {
                        TxtLatitud = location.Latitude.ToString();
                        TxtLongitud = location.Longitude.ToString();
                    }
                }
                else
                {
                    TxtLatitud = location.Latitude.ToString();
                    TxtLongitud = location.Longitude.ToString();
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", "Error" + ex.Message, "OK");
            }
        }
        private async void BtnNavegar(object obj)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(TxtLongitud) && !string.IsNullOrWhiteSpace(TxtLatitud))
                {
                    var location = new Location(Convert.ToDouble(TxtLatitud), Convert.ToDouble(TxtLongitud));
                    var options = new MapLaunchOptions { Name = "Destino" };
                    await Map.Default.OpenAsync(location, options);
                }
                else
                {
                    await DisplayAlert("Falta Información", "Error", "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", "Error" + ex.Message, "OK");
            }
        }

        private async void BtnWhatsApp(object obj)
        {
            try
            {
                var url = $"https://wa.me/{TxtTelefono1}?text=Hola";
                await Launcher.OpenAsync(url);
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", "Error" + ex.Message, "OK");
            }
        }

        private void BtnLimpiar(object obj)
        {
            limpiar();
        }

        public async void limpiar()
        {
            try
            {
                //TxtId = string.Empty;
                TxtNombre = string.Empty;
                TxtDirDomicilio = string.Empty;
                cmbBarrioDomicilio = string.Empty;
                TxtDirCobro = string.Empty;
                TxtBarrioCobro = string.Empty;
                //TxtLatitud = string.Empty;
                //TxtLongitud = string.Empty;
                TxtTelefono1 = string.Empty;
                TxtTelefono2 = string.Empty;
                TxtNotas = string.Empty;
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", "Error" + ex.Message, "OK");
            }
        }
        private async void BtnGrabar(object obj)
        {
            try
            {
                Mcliente cliente = new Mcliente();
                var funcion = new Dclientes();

                bool exito = false;
                if (ModoEdit)
                {
                    cliente.cteNumIdenti = TxtId;
                    //cliente.cteDirCobCte = TxtDirCobro;
                    //cliente.cteDireccion = TxtDirDomicilio;
                    //cliente.cteTeleCelu = TxtTelefono1;
                    //cliente.cteTeleFijo = TxtTelefono2;
                    //cliente.cteCodBarDom = cmbBarrioDomicilio;
                    //cliente.cteCodBarCob = cmbbarrio;
                    //cliente.cteFechaRegCte = DateTime.Now.ToString("dd/MM/yyyy");
                    cliente.cteCodRutReg = Usuario.CodigoCobr;
                    cliente.cteNotasGenerales = TxtNotas;
                    cliente.latitud = TxtLatitud;
                    cliente.longitud = TxtLongitud;

                    exito = funcion.ActualizarCliente(cliente);
                    ModoEdit = false;
                    if (exito)
                    {
                        await DisplayAlert("Actualizar", "Datos actualizados", "OK");
                        limpiar();
                    }
                    else
                        await DisplayAlert("Error", "Error", "OK");
                }
                else
                {
                    cliente.cteCodTipIde = "CC";
                    cliente.cteNumIdenti = TxtId;
                    cliente.cteNombApel = TxtNombre;
                    cliente.cteDireccion = TxtDirDomicilio;
                    cliente.cteDirCobCte = TxtDirCobro;
                    cliente.cteTeleCelu = TxtTelefono1;
                    cliente.cteTeleFijo = TxtTelefono2;
                    cliente.longitud = TxtLongitud;
                    cliente.cteCodBarDom = cmbBarrioDomicilio;
                    cliente.cteCodBarCob = "1";
                    cliente.cteCodRutReg = Usuario.CodigoCobr;
                    cliente.cteNotasGenerales = TxtNotas;
                    cliente.latitud = TxtLatitud;
                    cliente.longitud = TxtLongitud;

                    exito = funcion.InsertarCliente(cliente);
                    if (exito)
                    {
                        await DisplayAlert("Insertado", "Datos insertados", "OK");
                        limpiar();
                    }
                    else
                        await DisplayAlert("Error", "Error", "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", "Error" + ex.Message, "OK");
            }
        }
        private async void ConsultarCliente(object obj)
        {
            try
            {
                if (!String.IsNullOrEmpty(TxtId))
                {


                    Mcliente cliente = new Mcliente();
                    var funcion = new Dclientes();

                    cliente = funcion.ConsultarCliente(TxtId);
                    if (cliente != null)
                    {
                        TxtNombre = cliente.cteNombApel;
                        TxtDirDomicilio = cliente.cteDireccion;
                        cmbBarrioDomicilio = cliente.cteCodBarDom;
                        TxtDirCobro = cliente.cteDirCobCte;
                        // = cliente.cteCodBarCob;
                        TxtLatitud = cliente.latitud;
                        TxtLongitud = cliente.longitud;
                        TxtTelefono1 = cliente.cteTeleCelu;
                        TxtTelefono2 = cliente.cteTeleFijo;
                        TxtNotas = cliente.cteNotasGenerales;
                        ModoEdit = true;
                    }
                    else
                        limpiar();

                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", "Error" + ex.Message, "OK");
            }
        }


        #endregion
        #region COMANDOS
        //public ICommand CargarBarriosCommand => new Command(llenarBarrios); 
        public ICommand BtnUbicacionCommand => new Command(BtnUbicacion);
        public ICommand BtnNavegarCommand => new Command(BtnNavegar);
        public ICommand BtnWhatsAppCommand => new Command(BtnWhatsApp);
        public ICommand BtnLimpiarCommand => new Command(BtnLimpiar);

        //public ICommand ConsultarClienteCommand => new Command(ConsultarCliente);
        public ICommand BtnGrabarCommand => new Command(BtnGrabar);
        #endregion
    }
}
