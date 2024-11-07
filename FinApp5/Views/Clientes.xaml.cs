namespace FinApp5.Views;

using FinApp5.Conexiones;
using FinApp5.Datos;
using FinApp5.Modelo;
using FinApp5.ViewModels;
using Microsoft.Data.SqlClient;
using System.Data;

public partial class Clientes : ContentPage
{


    bool ModoEdit = false;
    Musuarios Usuario = new Musuarios();

    //public string? TxtId { get { return _txtId; } set { SetValue(ref _txtId, value); } }

    Mcliente cliente = new Mcliente();
    public Clientes(Musuarios usuario)
    {
        InitializeComponent();
        Loaded += (s, e) => SetFocus();
        bool estado = CONEXIONMAESTRA.VerificarCon();
        if (estado)
        {
            Usuario = usuario;

            llenarBarrios();
            //if (usuario.CodigoCobr != null)
            //    GetClientes(usuario.CodigoCobr);

            BindingContext = new VMClientes(Navigation, usuario);
        }
        else
        {
            DisplayAlert("Conexion", "Estas trabajando sin conexion (Linea 37)", "OK");
            llenarBarriosOffLine();
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
    private async void llenarBarrios()
    {
        try
        {
            SqlCommand cmd = new SqlCommand();
            cmd = new SqlCommand("CargarItemsDeBarriosEnGral", CONEXIONMAESTRA.conectar);
            CONEXIONMAESTRA.Abrir();
            cmd.CommandType = CommandType.StoredProcedure;
            if (cmd.Connection.State == ConnectionState.Closed)
                cmd.Connection.Open();
            SqlDataReader rdr = cmd.ExecuteReader();

            if (rdr.HasRows)
            {
                //await App.SQLiteDB.DeleteBarrios();
                while (rdr.Read())
                {
                    barrio.Add(new Mbarrio
                    {
                        IdBarrio = rdr["rbcCodigo"].ToString(),
                        NombreBarrio = rdr["rbcNombre"].ToString()
                    });
                    
                }
            }
            if (cmd.Connection.State == ConnectionState.Open)
                cmd.Connection.Close();
            if (barrio != null)
            {
                cmbBarrioCobro.ItemsSource = barrio;
                cmbBarrioDom.ItemsSource = barrio;
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", "Error" + ex.Message, "OK");
        }
        finally { CONEXIONMAESTRA.Cerrar(); }
    }
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


                if (CONEXIONMAESTRA.VerificarCon())
                {
                    exito = funcion.ActualizarCliente(cliente);
                    ModoEdit = false;
                    if (exito)
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
                    await DisplayAlert("Registro", "Actualizacion exitosa", "OK");
                }
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

                    if (CONEXIONMAESTRA.VerificarCon())
                    {
                        cliente.nuevo = 0;
                        await App.SQLiteDB.UpdateClienteAsync(cliente);
                        exito = funcion.InsertarCliente(cliente);
                        if (exito)
                        {
                            await DisplayAlert("Insertado", "Datos insertados", "OK");
                            limpiar();
                        }
                        else
                            await DisplayAlert("Error", "Error", "OK");
                    }
                    else
                    {
                        cliente.nuevo = 1;
                        await DisplayAlert("Sin Internet", "Esta trabajando sin Internet", "OK");
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
}