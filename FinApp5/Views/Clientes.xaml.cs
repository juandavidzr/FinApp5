namespace FinApp5.Views;

using FinApp5.Conexiones;
using FinApp5.Datos;
using FinApp5.Modelo;
using FinApp5.ViewModels;
using FinAppMaui.Services;
using Microsoft.Data.SqlClient;
using Microsoft.Maui.Graphics.Platform;
using Microsoft.Maui.Media;

using SkiaSharp;
using System.Data;
using static System.Net.WebRequestMethods;

public partial class Clientes : ContentPage
{
    private readonly HttpClient _http;
    private readonly ClienteService _clienteService;

    // Inyección del cliente configurado en MauiProgram


    bool ModoEdit = false;
    Musuarios Usuario = new Musuarios();
    private VMClientes viewModel;

    Mcliente cliente = new Mcliente();

    private FileResult _fotoCliente;
    private FileResult _fotoIDCliente;
    public Clientes(Musuarios usuario, ClienteService clienteService)
    {
        InitializeComponent();
        viewModel = new VMClientes(Navigation, usuario);
        BindingContext = viewModel;
        Usuario = usuario;

        _clienteService = clienteService;

        Loaded += (s, e) => SetFocus();       
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        //await viewModel.InicializarAsync(); // ahora inicializa todo de forma asincrónica

        try
        {
            await viewModel.InicializarAsync();
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error crítico", ex.Message, "OK");
        }
    }

    private void SetFocus()
    {
        TxtId.Focus();
    }


    private async void TxtId_Unfocused(object sender, FocusEventArgs e)
    {
        try
        {
            if (!String.IsNullOrEmpty(TxtId.Text))
            {
                Mcliente? cliente = new Mcliente();
                var funcion = new Dclientes();
                if (CONEXIONMAESTRA.VerificarCon())
                {
                    cliente = funcion.ConsultarCliente(TxtId.Text);
                    if (cliente != null)
                    {
                        TxtNombre.Text = cliente.cteNombApel;
                        TxtNombre.IsEnabled = false;
                        TxtDirDomicilio.Text = cliente.cteDireccion;
                        TxtDirDomicilio.IsEnabled = false;
                        var barDom = barrio.Where(p => p.IdBarrio == cliente.cteCodBarDom).FirstOrDefault();
                        if (barDom != null)
                            TxtBarrioDom.Text = barDom.NombreBarrio;
                        var barCobro = barrio.Where(p => p.IdBarrio == cliente.cteCodBarCob).FirstOrDefault();
                        if (barCobro != null)
                            TxtBarrioCobro.Text = barCobro.NombreBarrio;
                        cmbBarrioDom.IsVisible = false;
                        TxtDirCobro.Text = (cliente.cteDirCobCte);
                        TxtDirCobro.IsEnabled = false;
                        cmbBarrioCobro.IsVisible = false;
                        TxtLatitud.Text = cliente.latitud;
                        TxtLongitud.Text = cliente.longitud;
                        TxtTelefono1.Text = cliente.cteTeleCelu;
                        TxtTelefono1.IsEnabled = false;
                        TxtTelefono2.Text = cliente.cteTeleFijo;
                        TxtTelefono2.IsEnabled = false;
                        TxtNotas.Text = cliente.cteNotasGenerales;
                        if (cliente.Foto != null && cliente.Foto.Length > 0)
                        {
                            MemoryStream ms = new MemoryStream(cliente.Foto);
                            ImgCliente.Source = ImageSource.FromStream(() => ms);
                        }
                        else
                        {
                            ImgCliente.Source = null; 
                        }

                        if (cliente.IDFoto != null && cliente.IDFoto.Length > 0)
                        {
                            MemoryStream ms = new MemoryStream(cliente.IDFoto);
                            ImgId.Source = ImageSource.FromStream(() => ms);
                        }
                        else
                        {
                            ImgId.Source = null; 
                        }
                        ModoEdit = true;
                    }
                    else
                    {
                        await DisplayAlert("Buscar OffLine", "El cliente no fue encontrado en la base de datos OnLine, sera buscado en la base de datos Local", "OK");
                        Mcliente cliente1 = await App.SQLiteDB.GetClienteByIdAsync(TxtId.Text);
                        
                        if (cliente1 != null)
                        {
                            TxtNombre.Text = cliente1.cteNombApel;
                            TxtNombre.IsEnabled = false;
                            TxtDirDomicilio.Text = cliente1.cteDireccion;
                            TxtDirDomicilio.IsEnabled = false;
                            var barDom = barrio.Where(p => p.IdBarrio == cliente1.cteCodBarDom).FirstOrDefault();
                            if (barDom != null)
                                TxtBarrioDom.Text = barDom.NombreBarrio;
                            var barCobro = barrio.Where(p => p.IdBarrio == cliente1.cteCodBarCob).FirstOrDefault();
                            if (barCobro != null)
                                TxtBarrioCobro.Text = barCobro.NombreBarrio;
                            cmbBarrioDom.IsVisible = false;
                            TxtDirCobro.Text = (cliente1.cteDirCobCte);
                            TxtDirCobro.IsEnabled = false;
                            cmbBarrioCobro.IsVisible = false;
                            TxtLatitud.Text = cliente1.latitud;
                            TxtLongitud.Text = cliente1.longitud;
                            TxtTelefono1.Text = cliente1.cteTeleCelu;
                            TxtTelefono1.IsEnabled = false;
                            TxtTelefono2.Text = cliente1.cteTeleFijo;
                            TxtTelefono2.IsEnabled = false;
                            TxtNotas.Text = cliente1.cteNotasGenerales;
                            var nuevo = cliente1.nuevo;
                            ModoEdit = false;
                        }
                        else
                        {
                            await DisplayAlert("No Encontrado", "Cliente no encontrado ni localmente ni en la base de datos, debe crearlo", "OK");
                            limpiar();
                        }
                    }
                }
                else
                {
                    await DisplayAlert("Sin Internet", "Esta trabajando sin Internet (Linea 121)", "OK");
                    Mcliente cliente1 = await App.SQLiteDB.GetClienteByIdAsync(TxtId.Text);

                    if (cliente1 != null)
                    {
                        TxtNombre.Text = cliente1.cteNombApel;
                        TxtNombre.IsEnabled = false;
                        TxtDirDomicilio.Text = cliente1.cteDireccion;
                        TxtDirDomicilio.IsEnabled = false;
                        var barDom = barrio.Where(p => p.IdBarrio == cliente1.cteCodBarDom).FirstOrDefault();
                        if (barDom != null)
                            TxtBarrioDom.Text = barDom.NombreBarrio;
                        var barCobro = barrio.Where(p => p.IdBarrio == cliente1.cteCodBarCob).FirstOrDefault();
                        if (barCobro != null)
                            TxtBarrioCobro.Text = barCobro.NombreBarrio;
                        cmbBarrioDom.IsVisible = false;
                        TxtDirCobro.Text = (cliente1.cteDirCobCte);
                        TxtDirCobro.IsEnabled = false;
                        cmbBarrioCobro.IsVisible = false;
                        TxtLatitud.Text = cliente1.latitud;
                        TxtLongitud.Text = cliente1.longitud;
                        TxtTelefono1.Text = cliente1.cteTeleCelu;
                        TxtTelefono1.IsEnabled = false;
                        TxtTelefono2.Text = cliente1.cteTeleFijo;
                        TxtTelefono2.IsEnabled = false;
                        TxtNotas.Text = cliente1.cteNotasGenerales;
                        ModoEdit = true;
                    }
                    else
                    {
                        limpiar();
                        ModoEdit = false;
                    }
                }
            }
        }
        catch (Exception ex)
        {
            DisplayAlert("Error", "Error" + ex.Message, "OK");
        }
    }
    List<Mbarrio> barrio = new List<Mbarrio>();
    private async void llenarBarriosOffLine()
    {

        var barriosList = await App.SQLiteDB.GetBarriosAsync();
        if (barriosList != null)
        {
            cmbBarrioCobro.ItemsSource = barriosList;
            cmbBarrioDom.ItemsSource = barriosList;
        }
    }
    //private async void llenarBarrios()
    //{
    //    try
    //    {
    //        SqlCommand cmd = new SqlCommand();
    //        cmd = new SqlCommand("CargarItemsDeBarriosEnGral", CONEXIONMAESTRA.conectar);
    //        CONEXIONMAESTRA.Abrir();
    //        cmd.CommandType = CommandType.StoredProcedure;
    //        if (cmd.Connection.State == ConnectionState.Closed)
    //            cmd.Connection.Open();
    //        SqlDataReader rdr = cmd.ExecuteReader();

    //        if (rdr.HasRows)
    //        {
    //            //await App.SQLiteDB.DeleteBarrios();
    //            while (rdr.Read())
    //            {
    //                barrio.Add(new Mbarrio
    //                {
    //                    IdBarrio = rdr["rbcCodigo"].ToString(),
    //                    NombreBarrio = rdr["rbcNombre"].ToString()
    //                });
                    
    //            }
    //        }
    //        if (cmd.Connection.State == ConnectionState.Open)
    //            cmd.Connection.Close();
    //        if (barrio != null)
    //        {
    //            cmbBarrioCobro.ItemsSource = barrio;
    //            cmbBarrioDom.ItemsSource = barrio;
    //        }
    //    }
    //    catch (Exception ex)
    //    {
    //        await DisplayAlert("Error", "Error" + ex.Message, "OK");
    //    }
    //    finally { CONEXIONMAESTRA.Cerrar(); }
    //}
    string? uidBarrioDom;
    string? uidBarrioCobro;
    private void cmbBarrioDom_SelectedIndexChanged(object sender, EventArgs e)
    {
        try
        {
            if (cmbBarrioDom.SelectedIndex != -1)
            {
                Picker? item = sender as Picker;
                if (item != null)
                {
                    Mbarrio? selectedItem = item.SelectedItem as Mbarrio;
                    if (selectedItem != null)
                    {
                        uidBarrioDom = selectedItem.IdBarrio;
                        cliente.cteCodBarDom = uidBarrioDom;
                    }
                }
            }
        }
        catch (Exception ex)
        {
            DisplayAlert("Error", "Error" + ex.Message, "OK");
            throw;
        }
    }

    private bool validarDatos()
    {
        bool respuesta;
        if (string.IsNullOrEmpty(TxtId.Text) ||
            string.IsNullOrEmpty(uidBarrioDom) ||
            string.IsNullOrEmpty(uidBarrioCobro) ||
            string.IsNullOrEmpty(TxtNombre.Text) ||
            string.IsNullOrEmpty(TxtDirCobro.Text) ||
            string.IsNullOrEmpty(TxtDirDomicilio.Text) ||
            //string.IsNullOrEmpty(TxtLatitud.Text) ||
            //string.IsNullOrEmpty(TxtLongitud.Text) ||
            string.IsNullOrEmpty(TxtTelefono1.Text)
            )
            respuesta = false;
        else
            respuesta = true;

        return respuesta;
    }

    private void cmbBarrioCobro_SelectedIndexChanged(object sender, EventArgs e)
    {
        try
        {
            if (cmbBarrioCobro.SelectedIndex != -1)
            {
                Picker? item = sender as Picker;
                if (item != null)
                {
                    Mbarrio? selectedItem = item.SelectedItem as Mbarrio;
                    if (selectedItem != null)
                    {
                        uidBarrioCobro = selectedItem.IdBarrio;
                        cliente.cteCodBarCob = uidBarrioCobro;
                    }
                }
            }

        }
        catch (Exception ex)
        {
            DisplayAlert("Error", "Error" + ex.Message, "OK");
            throw;
        }
    }

    private void Nombre_Focused(object sender, FocusEventArgs e)
    {
        cmbBarrioDom.SelectedIndex = Convert.ToInt32(cliente.cteCodBarDom);
        cmbBarrioCobro.SelectedIndex = Convert.ToInt32(cliente.cteCodBarCob);
    }

    private void BtnLimpiar_Clicked(object sender, EventArgs e)
    {
        TxtId.Text = string.Empty;
        limpiar();

    }

    private void Button_Clicked_1(object sender, EventArgs e)
    {

    }

    private void Button_Clicked_2(object sender, EventArgs e)
    {

    }
    public async void limpiar()
    {
        try
        {
            TxtId.IsEnabled = true;
            //TxtId.Focus();
            
            //TxtId.Text = string.Empty;
            TxtNombre.Text = string.Empty;
            TxtNombre.IsEnabled = true;
            TxtDirDomicilio.Text = string.Empty;
            TxtDirDomicilio.IsEnabled = true;
            cmbBarrioDom.SelectedItem = -1;
            cmbBarrioDom.IsVisible = true;
            TxtDirCobro.Text = string.Empty;
            TxtDirCobro.IsEnabled = true;
            cmbBarrioCobro.SelectedItem = string.Empty;
            cmbBarrioCobro.IsVisible = true;
            TxtBarrioDom.Text = string.Empty;
            TxtBarrioDom.IsEnabled = true;
            TxtBarrioCobro.Text = string.Empty;
            TxtBarrioCobro.IsEnabled = true;
            TxtLatitud.Text = string.Empty;
            TxtLongitud.Text = string.Empty;
            TxtTelefono1.Text = string.Empty;
            TxtTelefono1.IsEnabled = true;
            TxtTelefono2.Text = string.Empty;
            TxtTelefono2.IsEnabled = true;
            TxtNotas.Text = string.Empty;
            ImgCliente.Source = null;
            ImgId.Source = null;
            ModoEdit = false;
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", "Error" + ex.Message, "OK");
        }
    }
    private async void Button_Clicked_Guardar(object sender, EventArgs e)
    {
        try
        {
            Mcliente cliente = new Mcliente();
            var funcion = new Dclientes();

            bool exito = false;

            if (ModoEdit)
            {
                cliente.cteNumIdenti = TxtId.Text;
                TxtDirCobro.IsEnabled = false;
                TxtDirDomicilio.IsEnabled = false;
                TxtTelefono1.IsEnabled = false;
                TxtTelefono2.IsEnabled = false;
                cmbBarrioDom.IsEnabled = false;
                cmbBarrioCobro.IsEnabled = false; ;
                //cliente.cteFechaRegCte = DateTime.Now.ToString("dd/MM/yyyy");
                cliente.cteCodRutReg = Usuario.CodigoCobr;
                cliente.cteNotasGenerales = TxtNotas.Text;
                cliente.latitud = TxtLatitud.Text;
                cliente.longitud = TxtLongitud.Text;
                if (_fotoCliente != null)
                    cliente.Foto = await ConvertirFotoABytes(_fotoCliente);

                if (_fotoIDCliente != null)
                    cliente.IDFoto = await ConvertirFotoABytes(_fotoIDCliente);



                if (CONEXIONMAESTRA.VerificarCon())
                {
                    //exito = funcion.ActualizarCliente(cliente);
                    var response = await _clienteService.ActualizarClienteAsync(cliente);
                    
                    if (response.IsSuccessStatusCode)
                    {
                         await DisplayAlert("Actualizar", "Datos actualizados", "OK");
                        TxtId.Text = string.Empty;
                        limpiar();
                    }
                    else
                        await DisplayAlert("Error", "Error", "OK");
                }
                else
                {
                    await DisplayAlert("Conexion", "Esta trabajando sin conexion", "OK");
                    cliente.nuevo = 2; //2 si fue actualizado offline, 1 si fue agregado offline, 0 si fue cargado de la web
                    await App.SQLiteDB.UpdateClienteAsync(cliente);
                    await DisplayAlert("Registro", "Actualizacion exitosa localmente", "OK");
                }
                ModoEdit = false;
            }
            else
            {
                if (validarDatos())
                {
                    cliente.cteCodTipIde = "CC";
                    cliente.cteNumIdenti = TxtId.Text;
                    cliente.cteNombApel = TxtNombre.Text;
                    cliente.cteDireccion = TxtDirDomicilio.Text;
                    cliente.cteDirCobCte = TxtDirCobro.Text;
                    cliente.cteTeleCelu = TxtTelefono1.Text;
                    cliente.cteTeleFijo = TxtTelefono2.Text;
                    cliente.longitud = TxtLongitud.Text;
                    cliente.cteCodBarDom = uidBarrioDom;
                    cliente.cteCodBarCob = uidBarrioCobro;
                    cliente.cteCodRutReg = Usuario.CodigoCobr;
                    cliente.cteNotasGenerales = TxtNotas.Text;
                    cliente.latitud = TxtLatitud.Text;
                    cliente.longitud = TxtLongitud.Text;

                    
                    cliente.Foto = await ConvertirFotoABytes(_fotoCliente);
                    cliente.IDFoto = await ConvertirFotoABytes(_fotoIDCliente);

                    if (CONEXIONMAESTRA.VerificarCon())
                    {
                        cliente.nuevo = 0;
                        await App.SQLiteDB.UpdateClienteAsync(cliente);

                        try
                        {

                            var response = await _clienteService.SubirClienteAsync(cliente);

                            if (response.IsSuccessStatusCode)
                            {
                                await DisplayAlert("Insertado", "Datos insertados correctamente en el servidor", "OK");
                                limpiar();
                            }
                            else
                            {
                                await DisplayAlert("Error", $"Error al insertar: {response.StatusCode}", "OK");
                            }

                        }
                        catch (Exception ex)
                        {
                            await DisplayAlert("Error", ex.Message, "OK");
                        }

                    }
                    else
                    {
                        cliente.nuevo = 1;
                        await DisplayAlert("Sin Internet", "Esta trabajando sin Internet (422)", "OK");
                        await App.SQLiteDB.SaveClienteAsync(cliente);
                        exito = true;
                        if (exito)
                        {
                            await DisplayAlert("Insertado", "Datos insertados localmente", "OK");
                            limpiar();
                        }
                        else
                            await DisplayAlert("Error", "Error", "OK");
                    }
                }
                else
                    await DisplayAlert("Verifique", "Datos incompletos por favor verifique que esten completos", "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", "Error" + ex.Message, "OK");
        }
    }

    private void btnInicio_Clicked(object sender, EventArgs e)
    {
        Navigation.PushAsync(new MenuPpal(Usuario));
    }

    public byte[] ResizeImage(byte[] imageData, int anchoMaximo)
    {
        //using var inputStream = new MemoryStream(imageData);
        //using var original = SKBitmap.Decode(inputStream);

        //var resized = original.Resize(new SKImageInfo(width, height), SKFilterQuality.High);

        //using var image = SKImage.FromBitmap(resized);
        //using var output = new MemoryStream();
        //image.Encode(SKEncodedImageFormat.Jpeg, 90).SaveTo(output);

        //return output.ToArray();

        using var input = new SKBitmap();
        using var ms = new MemoryStream(imageData);
        using var codec = SKCodec.Create(ms);
        var info = codec.Info;

        using var bitmap = SKBitmap.Decode(codec);

        int nuevoAncho = anchoMaximo;
        int nuevoAlto = (int)(bitmap.Height * (anchoMaximo / (float)bitmap.Width));

        using var resized = bitmap.Resize(new SKImageInfo(nuevoAncho, nuevoAlto), SKFilterQuality.Medium);
        using var image = SKImage.FromBitmap(resized);
        using var data = image.Encode(SKEncodedImageFormat.Jpeg, 80); // calidad 80%
        
        return data.ToArray();
    }

    private async void BtnTomarFoto_Clicked(object sender, EventArgs e)
    {
        if (!await SolicitarPermisosCamaraAsync())
        {
            await DisplayAlert("Permiso denegado", "No se concedió acceso a la cámara", "OK");
            return;
        }

        try
        {
            if (MediaPicker.Default.IsCaptureSupported)
            {
                var photo = await MediaPicker.Default.CapturePhotoAsync();
                if (photo != null)
                {
                    _fotoCliente = photo;
                    var stream = await photo.OpenReadAsync();
                    ImgCliente.Source = ImageSource.FromStream(() => stream);
                }
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"No se pudo tomar la foto: {ex.Message}", "OK");
        }
    }

    private async void BtnSeleccionarFoto_Clicked(object sender, EventArgs e)
    {
        try
        {
            var photo = await MediaPicker.Default.PickPhotoAsync();
            if (photo != null)
            {
                _fotoCliente = photo;
                var stream = await photo.OpenReadAsync();
                ImgCliente.Source = ImageSource.FromStream(() => stream);
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"No se pudo seleccionar la foto: {ex.Message}", "OK");
        }
    }

    private async Task<byte[]> ConvertirFotoABytes(FileResult photo)
    {
        if (photo == null) return null;
                
        using var stream = await photo.OpenReadAsync();
        using var memoryStream = new MemoryStream();
        await stream.CopyToAsync(memoryStream);
        var originalBytes = memoryStream.ToArray();
                
        byte[] resizedBytes = ResizeImage(originalBytes, 600);

        return resizedBytes;
    }


    private async Task<bool> SolicitarPermisosCamaraAsync()
    {
        var status = await Permissions.CheckStatusAsync<Permissions.Camera>();
        if (status != PermissionStatus.Granted)
        {
            status = await Permissions.RequestAsync<Permissions.Camera>();
        }
        return status == PermissionStatus.Granted;
    }

    private async void BtnTomarFotoID_Clicked(object sender, EventArgs e)
    {
        if (!await SolicitarPermisosCamaraAsync())
        {
            await DisplayAlert("Permiso denegado", "No se concedió acceso a la cámara", "OK");
            return;
        }

        try
        {
            if (MediaPicker.Default.IsCaptureSupported)
            {
                var photo = await MediaPicker.Default.CapturePhotoAsync();
                if (photo != null)
                {
                    _fotoIDCliente = photo;
                    var stream = await photo.OpenReadAsync();
                    ImgId.Source = ImageSource.FromStream(() => stream);
                }
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"No se pudo tomar la foto: {ex.Message}", "OK");
        }
    }

    private async void BtnSeleccionarFotoID_Clicked(object sender, EventArgs e)
    {
        try
        {
            var photo = await MediaPicker.Default.PickPhotoAsync();
            if (photo != null)
            {
                _fotoIDCliente = photo;
                var stream = await photo.OpenReadAsync();
                ImgId.Source = ImageSource.FromStream(() => stream);
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"No se pudo seleccionar la foto: {ex.Message}", "OK");
        }
    }
}