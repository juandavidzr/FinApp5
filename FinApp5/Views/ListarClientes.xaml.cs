using FinApp5.Conexiones;
using FinApp5.Modelo;
using FinApp5.ViewModels;
using Microsoft.Data.SqlClient;
using System.Collections.ObjectModel;
using System.Data;

namespace FinApp5.Views;

public partial class ListarClientes : ContentPage
{
    public ObservableCollection<Mcliente> clientsCollection = new ObservableCollection<Mcliente>();
    private List<Mcliente> ListaFinCtes = new List<Mcliente>();
    Musuarios Usuario = new Musuarios();
    public ListarClientes(Musuarios usuario)
    {
        InitializeComponent();
        llenarDatos(usuario);
        Usuario = usuario;
        BindingContext = new VMTransacciones(Navigation, usuario);

    }

    private async void llenarDatos(Musuarios usuario)
    {

        SqlCommand cmd = new SqlCommand();
        try
        {
            //var ruta = Helpers.Settings.CodigoRuta;
            var ruta = usuario.CodigoCobr;
            var intOpcionFil = 1;
            var criterio = "";

            clientsCollection.Clear();

            if (CONEXIONMAESTRA.VerificarCon())
            {

                CONEXIONMAESTRA.Abrir();
                cmd = new SqlCommand("FiltrarListadoDeClientesParaCreditoDeRuta", CONEXIONMAESTRA.conectar);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@strCodRutaTra", ruta);
                cmd.Parameters.AddWithValue("@intOpcionFil", intOpcionFil);
                cmd.Parameters.AddWithValue("@strCriterio", criterio);
                if (cmd.Connection.State == ConnectionState.Closed)
                    cmd.Connection.Open();
                SqlDataReader rdr = cmd.ExecuteReader();

                while (rdr.Read())
                {
                    clientsCollection.Add(new Mcliente()
                    {
                        cteNumIdenti = rdr["cteNumIdenti"].ToString(),
                        cteNombApel = rdr["cteNombApel"].ToString(),
                        cteTeleCelu = rdr["cteTeleCelu"].ToString(),
                        cteTeleFijo = rdr["cteTeleFijo"].ToString(),
                        cteDirCobCte = rdr["cteDirCobCte"].ToString(),

                    });

                }

                lstClientes.ItemsSource = clientsCollection;

                if (cmd.Connection.State == ConnectionState.Open)
                    cmd.Connection.Close();

            }
            else
            {
                //await DisplayAlert("Sin Internet", "Esta trabajando sin Internet (Linea 72)", "OK");
                var clienteList = await App.SQLiteDB.GetClientesAsync();
                if (clienteList != null)
                {
                    lstClientes.ItemsSource = clienteList;
                    clientsCollection.Clear();
                    foreach (var cliente in clienteList)
                    {
                        clientsCollection.Add(cliente);
                    }
                    if (clientsCollection != null)
                    {
                        lstClientes.ItemsSource = clientsCollection;
                    }
                }
            }

        }
        catch (Exception)
        {
            if (cmd.Connection.State == ConnectionState.Open)
                cmd.Connection.Close();
            throw;
        }
        finally
        {
           
        }

    }

    private void SearchBar_TextChanged(object sender, TextChangedEventArgs e)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(e.NewTextValue))
            {
                lstClientes.ItemsSource = clientsCollection.ToList();
            }
            else
            {
                lstClientes.ItemsSource = clientsCollection.Where(i => i.cteNombApel.ToLower().Contains(e.NewTextValue.ToLower()));
            }
        }
        catch (Exception)
        {

            throw;
        }

    }

    private void lstClientes_ItemSelected(object sender, SelectedItemChangedEventArgs e)
    {
        try
        {
            Mcliente cliente = new Mcliente();
            var cli = (Mcliente)e.SelectedItem;
            cliente.cteNumIdenti = cli.cteNumIdenti;
            cliente.cteNombApel = cli.cteNombApel;

            Navigation.PushAsync(new Creditos(cliente, Usuario));
        }
        catch (Exception ex)
        {
            DisplayAlert("Error", "Error" + ex.Message, "OK");
            throw;
        }

    }
}