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
    private readonly Musuarios Usuario = new();
    public ListarClientes(Musuarios usuario)
    {
        InitializeComponent();
        try
        {
            Usuario = usuario;
            if (Usuario.CodigoCobr != null && CONEXIONMAESTRA.VerificarCon())
            {

                _ = App.SQLiteDB.SincronizarClientes(Usuario.CodigoCobr); //inserta los nuevos clientes en el servidor 
                if (Usuario?.Usuario != null)
                {
                    _ = App.SQLiteDB.SincronizarCreditos(Usuario); //inserta los nuevos creditos en el servidor

                   _ = App.SQLiteDB.SincronizarAbonos(usuario);
                }

                else
                    Console.WriteLine("⚠️ Error: Usuario.Usuario es null.");

            }

            llenarDatos(usuario);
        }
        catch (Exception ex)
        {

            throw;
        }
       

        //BindingContext = new VMTransacciones(Navigation, usuario);
    }
    
    private async void llenarDatos(Musuarios usuario)
    {
        SqlCommand cmd = new SqlCommand();
        try
        {
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
                await DisplayAlert("Sin Internet", "Esta trabajando sin Internet (75)", "OK");
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
            lstClientes.SelectedItem = null;
        }
        catch (Exception)
        {
            if (cmd.Connection.State == ConnectionState.Open)
                cmd.Connection.Close();
            throw;
        }
        //finally
        //{
        //    if (cmd.Connection.State == ConnectionState.Open)
        //        cmd.Connection.Close();
        //}
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
                //lstClientes.ItemsSource = clientsCollection.Where(i => i.cteNombApel.ToLower().Contains(e.NewTextValue.ToLower()));
                lstClientes.ItemsSource = clientsCollection
                                            .Where(i => (i.cteNombApel?.ToLower() ?? "").Contains(e.NewTextValue.ToLower()))
                                            .ToList();

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

    private void btnTransacciones_Clicked(object sender, EventArgs e)
    {
        Navigation.PushAsync(new Transacciones(Usuario));
    }

    private void btnInicio_Clicked(object sender, EventArgs e)
    {
        Navigation.PushAsync(new MenuPpal(Usuario));
    }
}