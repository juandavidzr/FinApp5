using FinApp5.Conexiones;
using FinApp5.Modelo;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;

namespace FinApp5.Views;

public partial class EnrrutarCartera : ContentPage
{
    public string? ruta { get; set; }
    public string lngNumeroCre { get; set; }
    public string NumeroCreCambiaPos { get; set; }    
    /// <summary>
    /// Posición anterior
    /// </summary>
    public int? OldPosCre { get; set; }
    public int IntNuevaPosCre { get; set; } = 0;
    readonly Musuarios? Usuario = new();
    Prestamos prestamo = new();
    public List<Prestamos> prestamosOffLine = new List<Prestamos>();

    public EnrrutarCartera(Musuarios usuario)
    {
        Usuario = usuario;
        IntNuevaPosCre = 1;
        InitializeComponent();
        if(CONEXIONMAESTRA.VerificarCon())
            prestamosOffLine = ConsultarCambioDeRutaOffline();  

        _ = CargarCreditosAsync();
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

            if (CONEXIONMAESTRA.VerificarCon())
            {

                if (prestamosOffLine.Any())
                {
                    List<Prestamos> prestamos = await App.SQLiteDB.ObtenerTodosCreditosPorRutaAsync(ruta);

                    if (prestamos.Any())
                    {
                       await App.SQLiteDB.ReasignarPosicionesServerAsync(prestamos);
                    }
                }

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
            }
            else
            {
                var resultados = await App.SQLiteDB.FiltrarCreditosDeRutaSegunCriterio(ruta, 1);
                foreach (var item in resultados)
                {
                    creditosCollection.Add(new Prestamos()
                    {
                        nombreCliente = item.nombreCliente,
                        NumPrestamo = Convert.ToInt32(item.NumPrestamo),
                        posRutCre = item.posRutCre
                    });
                }

                //await App.SQLiteDB.ActualizarPosicionDeCreditoEnRutaDestinoAsync(IntNuevaPosCre, lngNumeroCre, ruta);

            }

            if (creditosCollection != null)
            {
                lstCreditos.ItemsSource = creditosCollection//.Where(p => p.activo == 1)
                                                            .OrderBy(p => p.nombreCliente).ToList();

                lstCreditos1.ItemsSource = creditosCollection//.Where(p => p.activo == 1)
                                                           .OrderBy(p => p.posRutCre).ToList();
            }
            IntNuevaPosCre = 1;
        }
        catch (Exception ex)
        {
            await DisplayAlert("error", ex.Message, "OK");
            throw;
        }
        finally { CONEXIONMAESTRA.Cerrar(); }

    }

    public List<Prestamos> ConsultarCambioDeRutaOffline()
    {
        return App.SQLiteDB.ConsultarCambioDeRutaOffline().Result;
    }
    private void lstCreditos_ItemSelected(object sender, SelectedItemChangedEventArgs e)
    {
        if (lstCreditos != null && e.SelectedItem != null)
        {
            prestamo = e.SelectedItem as Prestamos;
            lngNumeroCre = prestamo.NumPrestamo.ToString();
            OldPosCre = prestamo.posRutCre;
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
            IntNuevaPosCre = prestamo.posRutCre;
            NumeroCreCambiaPos = prestamo.NumPrestamo.ToString();
        }
        else
        {
            DisplayAlert("error", "Por favor seleccione un credito", "OK");
        }
    }

    //private void lstCreditos2_ItemSelected(object sender, SelectedItemChangedEventArgs e)
    //{

    //}

    private async void Button_Clicked(object sender, EventArgs e)
    {
        try
        {            
            if (lngNumeroCre != null)
            {

                if (CONEXIONMAESTRA.VerificarCon())
                {
                    SqlCommand cmd = new SqlCommand("ActualizarPosicionDeCreditoEnRutaDestino", CONEXIONMAESTRA.conectar);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@intNuePosCreRut", IntNuevaPosCre);
                    cmd.Parameters.AddWithValue("@lngNumeroCreAct", lngNumeroCre);
                    cmd.Parameters.AddWithValue("@strCodigoRuta", ruta);
                    CONEXIONMAESTRA.Abrir();
                    cmd.ExecuteReader();
                    CONEXIONMAESTRA.Cerrar();
                   
                }
                else
                {
                   await App.SQLiteDB.ActualizarPosicionDeCreditoEnRutaDestinoAsync(IntNuevaPosCre, lngNumeroCre, ruta, NumeroCreCambiaPos);
                }
                 _ = CargarCreditosAsync();
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