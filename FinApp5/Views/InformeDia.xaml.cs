using FinApp5.Conexiones;
using FinApp5.Modelo;
using Microsoft.Data.SqlClient;
using System.Collections.ObjectModel;
using System.Data;

namespace FinApp5.Views;

public partial class InformeDia : ContentPage
{
    public string ruta { get; set; }
    
    Musuarios Usuario = new Musuarios();
    public ObservableCollection<Mmovimiento> MovimientosCollection = new ObservableCollection<Mmovimiento>();
    public InformeDia(Musuarios usuario)
	{
        InitializeComponent();
        Usuario = usuario;
        CargarMovimientosAsync();
    }
    public async Task CargarMovimientosAsync()
    {
        try
        {
            lstMovimientos.ItemsSource = null;

            MovimientosCollection.Clear();

            if (Usuario.CodigoCobr != null)
                ruta = Usuario.CodigoCobr;

            if (CONEXIONMAESTRA.VerificarCon())
            {
                SqlCommand cmd = new SqlCommand("DescargaDeMovtosDiariosGeneradosPorRuta", CONEXIONMAESTRA.conectar);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@strCodigoRuta", ruta);

                CONEXIONMAESTRA.Abrir();
                SqlDataReader rdr = cmd.ExecuteReader();

                while (rdr.Read())
                {
                    MovimientosCollection.Add(new Mmovimiento()
                    {
                        NombreCteCre = rdr["mrcNombreCteCre"].ToString(),
                        NumeroCreAfe = rdr["mrcNumeroCreAfe"].ToString(),
                        ValorMovto = Convert.ToDouble(rdr["mrcValorMovto"].ToString()),
                        FechaHoraReg = Convert.ToDateTime(rdr["mcrFechaHoraReg"].ToString()),
                        Descripcion = rdr["tmcDescripcion"].ToString(),
                        strComentAbo = rdr["mrcComentarioAbo"].ToString()
                    });
                }
            }
            else
            {
                await DisplayAlert("Sin Internet", "Esta trabajando sin Internet (56)", "OK");
                var abonosList = await App.SQLiteDB.GetAbonosNew();
                if (abonosList != null)
                {
                    lstMovimientos.ItemsSource = abonosList;
                    MovimientosCollection.Clear();
                    foreach (var abono in abonosList)
                    {
                        MovimientosCollection.Add(abono);
                    }
                }
            }
            if (MovimientosCollection != null)
            {
                //lstMovimientos.ItemsSource = MovimientosCollection.OrderBy(p => p.FechaHoraReg).ToList();
                lstMovimientos.ItemsSource = MovimientosCollection;
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("error", ex.Message, "OK");
            throw;
        }
        finally { CONEXIONMAESTRA.Cerrar(); }
    }
    //private void lstCreditos_ItemSelected(object sender, SelectedItemChangedEventArgs e)
    //{

    //}

    private void lstMovimientos_ItemSelected(object sender, SelectedItemChangedEventArgs e)
    {

    }
}