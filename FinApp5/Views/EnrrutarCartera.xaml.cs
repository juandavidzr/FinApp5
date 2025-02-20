using FinApp5.Conexiones;
using FinApp5.Modelo;
using Microsoft.Data.SqlClient;
using System.Collections.ObjectModel;
using System.Data;

namespace FinApp5.Views;

public partial class EnrrutarCartera : ContentPage
{
    public string ruta { get; set; }
    public string lngNumeroCre { get; set; }
    public int intNuevaPosCre { get; set; }
    Musuarios Usuario = new Musuarios();
    Prestamos prestamo = new Prestamos();
    
    public EnrrutarCartera(Musuarios usuario)
	{
        Usuario = usuario;
        intNuevaPosCre = 1;
        InitializeComponent();
        CargarCreditosAsync();
    }
    public ObservableCollection<Prestamos> creditosCollection = new ObservableCollection<Prestamos>();
    public async Task CargarCreditosAsync()
    {
        try
        {
            lstCreditos.ItemsSource = null;
            lstCreditos1.ItemsSource = null;
            creditosCollection.Clear();

            ruta = Usuario.CodigoCobr;
            SqlCommand cmd = new SqlCommand("FiltrarCreditosDeRutaSegunCriterio", CONEXIONMAESTRA.conectar);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@strCodigoRuta", ruta);
            cmd.Parameters.AddWithValue("@intSelector", 1);
            CONEXIONMAESTRA.Abrir();
            SqlDataReader rdr = cmd.ExecuteReader();

            while (rdr.Read())
            {
                creditosCollection.Add(new Prestamos()
                {
                    nombreCliente = rdr["cteNombApel"].ToString().Trim(),
                    NumPrestamo = Convert.ToInt32(rdr["pmoNumeroPre"].ToString()),
                    posRutCre = Convert.ToInt32(rdr["pmoPosRutCre"].ToString().Trim())
                });
            }

            if (creditosCollection != null)
            {
                lstCreditos.ItemsSource = creditosCollection//.Where(p => p.activo == 1)
                                                            .OrderBy(p => p.nombreCliente).ToList();

                lstCreditos1.ItemsSource = creditosCollection//.Where(p => p.activo == 1)
                                                           .OrderBy(p => p.posRutCre).ToList();
            }
            intNuevaPosCre = 1;
        }
        catch (Exception ex)
        {
            await DisplayAlert("error", ex.Message, "OK");
            throw;
        }
        finally { CONEXIONMAESTRA.Cerrar(); }
    }

    private void lstCreditos_ItemSelected(object sender, SelectedItemChangedEventArgs e)
    {
        if (lstCreditos != null && e.SelectedItem != null)
        {
            prestamo = e.SelectedItem as Prestamos;
            lngNumeroCre = prestamo.NumPrestamo.ToString();
        }
        else 
        {
            DisplayAlert("error", "Por favor seleccione un credito", "OK");
        }

        

    }
    private void lstCreditos1_ItemSelected(object sender, SelectedItemChangedEventArgs e)
    {
        if (lstCreditos1 != null && e.SelectedItem != null)
        {
            prestamo = e.SelectedItem as Prestamos;
            intNuevaPosCre = prestamo.posRutCre ;
        }
        else
        {
            DisplayAlert("error", "Por favor seleccione un credito", "OK");
        }
    }

    //private void lstCreditos2_ItemSelected(object sender, SelectedItemChangedEventArgs e)
    //{
       
    //}

    private void Button_Clicked(object sender, EventArgs e)
    {
        try
        {
            if (lngNumeroCre != null)
            {
                SqlCommand cmd = new SqlCommand("ActualizarPosicionDeCreditoEnRutaDestino", CONEXIONMAESTRA.conectar);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@intNuePosCreRut", intNuevaPosCre);
                cmd.Parameters.AddWithValue("@lngNumeroCreAct", lngNumeroCre);
                cmd.Parameters.AddWithValue("@strCodigoRuta", ruta.Trim());
                CONEXIONMAESTRA.Abrir();
                cmd.ExecuteReader();
                CONEXIONMAESTRA.Cerrar();
                CargarCreditosAsync();
                
            }
            else
                DisplayAlert("error", "Por favor seleccione un credito", "OK");
        }
        catch (Exception ex)
        {
            DisplayAlert("error", ex.Message, "OK");
            throw;
        }
        finally { CONEXIONMAESTRA.Cerrar(); }
    }
}