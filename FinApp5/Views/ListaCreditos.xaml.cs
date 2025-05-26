using Controls.UserDialogs.Maui;
using FinApp5.Conexiones;
using FinApp5.Modelo;
using Microsoft.Data.SqlClient;
using Microsoft.Maui.Controls.PlatformConfiguration;
using System.Collections.ObjectModel;
using System.Data;
using FinApp5.ViewModels;


namespace FinApp5.Views;

public partial class ListaCreditos : ContentPage
{
    Musuarios Usuario = new Musuarios();
    public ListaCreditos(Musuarios usuario)
    {
        InitializeComponent();
        NavigationPage.SetHasBackButton(this, false);
        NavigationPage.SetHasNavigationBar(this, false);
        Usuario = usuario;
        VMAbono abono = new VMAbono(null, Usuario);

        //ToDo sincronizar Abonos
        _ = abono.SincronizarAbonosAsync();


        OrdenList = GetOrden();
        cmbOrden.ItemsSource = OrdenList;
        if (cmbOrden.SelectedIndex == -1)
            cmbOrden.SelectedIndex = 0;
    }


    private async void OnBackButtonPressed(object sender, ShellNavigatingEventArgs e)
    {
        if (e.Source == ShellNavigationSource.Pop) // Si el usuario presiona "Atrás"
        {
            e.Cancel(); // Bloquea la navegación atrás
            await Shell.Current.GoToAsync("///Transacciones", true, new Dictionary<string, object>
            {
                { "Usuario", Usuario }
            });
        }
    }

    public class Orden
    {
        public int id { get; set; }
        public string orden { get; set; }
    }
    public List<Orden> OrdenList { get; set; }
    private List<Orden> GetOrden()
    {
        var ordenLista = new List<Orden>
            {
                new Orden(){id = 0, orden = "Ruta"},
                new Orden(){id = 1, orden = "Mora"},
                new Orden(){id = 2, orden = "Abona"},
                new Orden(){id = 3, orden = "Reta"},
            };
        return ordenLista;
    }

    public ObservableCollection<Prestamos> creditosCollection = new ObservableCollection<Prestamos>();
    public async Task CargarCreditosAsync()
    {
        try
        {

            creditosCollection.Clear();
            var prestamos = new List<Prestamos>();
            //prestamos = await App.SQLiteDB.GetCreditos();

            var ruta = Usuario.CodigoCobr;
            var filtro = string.Empty;
            if (cmbOrden.SelectedIndex == -1)
                filtro = "Ruta";
            if (cmbOrden.SelectedIndex == 0)
                filtro = "Ruta";
            if (cmbOrden.SelectedIndex == 1)
                filtro = "Mora";
            if (cmbOrden.SelectedIndex == 2)
                filtro = "Abona";
            if (cmbOrden.SelectedIndex == 3)
                filtro = "Reta";

            SqlCommand cmd = new SqlCommand("DecargarCarteraSegunModo", CONEXIONMAESTRA.conectar);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@strCodigoRuta", ruta);
            cmd.Parameters.AddWithValue("@strModoFil", filtro);
            CONEXIONMAESTRA.Abrir();
            //SqlDataReader rdr = cmd.ExecuteReader();
            using (SqlDataReader rdr = cmd.ExecuteReader()) // Usa 'await'
            {

                while (rdr.Read())
                {
                    creditosCollection.Add(new Prestamos()
                    {

                        idCliente = rdr["pmoIdentifiCli"].ToString(),
                        nombreCliente = rdr["cteNombApel"].ToString(),
                        NumPrestamo = Convert.ToInt32(rdr["pmoNumeroPre"].ToString()),
                        fechaPrestamo = rdr["pmoFechaPre"].ToString(),
                        cantidadPrestada = Convert.ToInt32(rdr["pmoSaldoActualCte"]),
                        valUltPag = Convert.ToInt32(rdr["pmoValUltPag"]),

                        DireccionCobro = rdr["cteDirCobCte"].ToString(),
                        TelefonoCell = rdr["cteTeleCelu"].ToString(),
                        numCuoAtra = Convert.ToInt32(rdr["pmoNumCuoAtra"]),
                        valCuotaPag = Convert.ToInt32(rdr["pmoValCuotaPag"]),
                        desDiaPago = rdr["pmoDesDiaPago"].ToString(),
                        numCuoPen = Convert.ToInt32(rdr["pmoNumCuoPen"]),
                        saldoActualCre = Convert.ToInt32(rdr["pmoSaldoActualCte"]),
                        IndicaRetaque = Convert.ToInt32(rdr["pmoIndicaRetaque"]),
                        fecVenCre = rdr["pmoFecVenCre"].ToString(),
                        totalPagCre = Convert.ToDouble(rdr["TotalCre"].ToString()),
                        marAboCreDia = Convert.ToInt16(rdr["pmoMarAboCreDia"].ToString()),

                        
                    });
                }
            }


            if (creditosCollection != null)
            {
                //lstCreditos.ItemsSource = creditosCollection.Where(p => p.IndicaRetaque == 0 &&
                //                                                                p.marAboCreDia == 0)
                //                                            .OrderBy(p => p.posRutCre).ToList();
                lstCreditos.ItemsSource = creditosCollection;
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

    /// <summary>
    /// Cargar Creditos offLine
    /// </summary>
    /// <returns></returns>
    public async Task CargarCreditos()
    {
        creditosCollection.Clear();
        creditosCollection = await GetAllCredit(Usuario.CodigoCobr);
        lstCreditos.ItemsSource = creditosCollection;
    }

    public async Task<ObservableCollection<Prestamos>> GetAllCredit(string? code)
    {
        try
        {
            creditosCollection = await App.SQLiteDB.GetAllCredit(code);
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", "Error" + ex.Message, "OK");
        }
        return creditosCollection;
    }


    private void SearchBar_TextChanged(object sender, TextChangedEventArgs e)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(e.NewTextValue))
                lstCreditos.ItemsSource = creditosCollection.ToList();
            else
                lstCreditos.ItemsSource = creditosCollection.Where(i => i.nombreCliente.ToLower().Contains(e.NewTextValue.ToLower()));
        }
        catch (Exception)
        {
            throw;
        }
    }

    private void lstCreditos_ItemSelected(object sender, SelectedItemChangedEventArgs e)
    {
        try
        {
            var prestamo = (Prestamos)e.SelectedItem;
            if (e.SelectedItem.Equals(-1))
                DisplayAlert("error", "No hay elementos", "OK");
            else
                Navigation.PushAsync(new Abonos(prestamo, Usuario));
        }
        catch (Exception ex)
        {
            DisplayAlert("error", ex.Message, "OK");
        }
    }

    private void btnInicio_Clicked(object sender, EventArgs e)
    {
        Navigation.PushAsync(new MenuPpal(Usuario));
    }

    private async void cmbOrden_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (CONEXIONMAESTRA.VerificarCon())
        {
            VMAbono abono = new VMAbono(null, Usuario);            
            await abono.SincronizarAbonosAsync();

            await CargarCreditosAsync();
        }
        else 
        {
            //ToDo para commit 
            await CargarCreditos();
        }
        if (cmbOrden.SelectedIndex == 0) //ordenar por ruta
            lstCreditos.ItemsSource = creditosCollection.Where(p => p.IndicaRetaque == 0 &&
                                                                   p.marAboCreDia == 0).OrderBy(p => p.posRutCre).ToList();
        if (cmbOrden.SelectedIndex == 1) //creditos en mora
        {
            var fecha = DateTime.Today.AddDays(-60);
            lstCreditos.ItemsSource = creditosCollection.Where(p => Convert.ToDateTime(p.fecUltPag) < fecha).ToList();
        }
        if (cmbOrden.SelectedIndex == 2) //creditos con abonos le dia de hoy
            lstCreditos.ItemsSource = creditosCollection.Where(p => p.marAboCreDia == 1).ToList();
        if (cmbOrden.SelectedIndex == 3) //clasificados como para retacar
            lstCreditos.ItemsSource = creditosCollection.Where(p => p.marAboCreDia == 0 && p.IndicaRetaque == 1).ToList();
        
        UserDialogs.Instance.HideHud();
    }

    private void btnTransacciones_Clicked(object sender, EventArgs e)
    {
        Navigation.PushAsync(new Transacciones(Usuario));
    }
}