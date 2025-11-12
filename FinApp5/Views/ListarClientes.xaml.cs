using FinApp5.Conexiones;
using FinApp5.Modelo;
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
        Usuario = usuario;
        _ = ListarClientesAsync(Usuario);
    }
    public async Task ListarClientesAsync(Musuarios usuario)
    {
        InitializeComponent();
        try
        {

            if (Usuario.CodigoCobr != null && CONEXIONMAESTRA.VerificarCon())
            {
                //_ = App.SQLiteDB.SincronizarClientes(Usuario.CodigoCobr); //inserta los nuevos clientes en el servidor 
                await App.SQLiteDB.SincronizarClientes(Usuario.CodigoCobr);
                if (Usuario?.Usuario != null)
                {
                    //_ = App.SQLiteDB.SincronizarCreditos(Usuario); //inserta los nuevos creditos en el servidor
                    //_ = App.SQLiteDB.SincronizarAbonos(usuario);
                    await App.SQLiteDB.SincronizarCreditos(usuario);
                    await App.SQLiteDB.SincronizarAbonos(usuario);
                }
                else
                    Console.WriteLine("Error: Usuario.Usuario es null.");
            }
            await LlenarDatosAsync(usuario);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Excepción: {ex.Message}");
            throw;
        }


        //BindingContext = new VMTransacciones(Navigation, usuario);
    }

    //private async void llenarDatos(Musuarios usuario)
    //{
    //    SqlCommand cmd = new SqlCommand();
    //    try
    //    {
    //        var ruta = usuario.CodigoCobr;
    //        var intOpcionFil = 1;
    //        var criterio = "";

    //        clientsCollection.Clear();

    //        if (CONEXIONMAESTRA.VerificarCon())
    //        {
    //            CONEXIONMAESTRA.Abrir();
    //            cmd = new SqlCommand("FiltrarListadoDeClientesParaCreditoDeRuta", CONEXIONMAESTRA.conectar);
    //            cmd.CommandType = CommandType.StoredProcedure;
    //            cmd.Parameters.AddWithValue("@strCodRutaTra", ruta);
    //            cmd.Parameters.AddWithValue("@intOpcionFil", intOpcionFil);
    //            cmd.Parameters.AddWithValue("@strCriterio", criterio);
    //            if (cmd.Connection.State == ConnectionState.Closed)
    //                cmd.Connection.Open();
    //            SqlDataReader rdr = cmd.ExecuteReader();

    //            while (rdr.Read())
    //            {
    //                clientsCollection.Add(new Mcliente()
    //                {
    //                    cteNumIdenti = rdr["cteNumIdenti"].ToString(),
    //                    cteNombApel = rdr["cteNombApel"].ToString(),
    //                    cteTeleCelu = rdr["cteTeleCelu"].ToString(),
    //                    cteTeleFijo = rdr["cteTeleFijo"].ToString(),
    //                    cteDirCobCte = rdr["cteDirCobCte"].ToString(),
    //                });
    //            }
    //            cvClientes.ItemsSource = clientsCollection;

    //            if (cmd.Connection.State == ConnectionState.Open)
    //                cmd.Connection.Close();
    //        }
    //        else
    //        {
    //            await DisplayAlert("Sin Internet", "Esta trabajando sin Internet (75)", "OK");
    //            var clienteList = await App.SQLiteDB.GetClientesAsync(Usuario.CodigoCobr);
    //            if (clienteList != null)
    //            {
    //                cvClientes.ItemsSource = clienteList;
    //                clientsCollection.Clear();
    //                foreach (var cliente in clienteList)
    //                {
    //                    clientsCollection.Add(cliente);
    //                }
    //                if (clientsCollection != null)
    //                {
    //                    cvClientes.ItemsSource = clientsCollection;
    //                }
    //            }
    //        }
    //        cvClientes.SelectedItem = null;
    //    }
    //    catch (Exception ex)
    //    {
    //        if (cmd.Connection.State == ConnectionState.Open)
    //            cmd.Connection.Close();
    //        throw;
    //    }
    //    //finally
    //    //{
    //    //    if (cmd.Connection.State == ConnectionState.Open)
    //    //        cmd.Connection.Close();
    //    //}
    //}

    private async Task LlenarDatosAsync(Musuarios usuario)
    {
        try
        {
            var ruta = usuario.CodigoCobr;
            var intOpcionFil = 1;
            var criterio = "";

            clientsCollection.Clear();

            if (CONEXIONMAESTRA.VerificarCon())
            {
                using (SqlConnection conn = CONEXIONMAESTRA.GetConnection())
                {
                    await conn.OpenAsync();

                    using (SqlCommand cmd = new SqlCommand("FiltrarListadoDeClientesParaCreditoDeRuta", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@strCodRutaTra", ruta);
                        cmd.Parameters.AddWithValue("@intOpcionFil", intOpcionFil);
                        cmd.Parameters.AddWithValue("@strCriterio", criterio);

                        using (SqlDataReader rdr = await cmd.ExecuteReaderAsync())
                        {
                            while (await rdr.ReadAsync())
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
                        }
                    }
                }

                cvClientes.ItemsSource = clientsCollection;
            }
            else
            {
                await DisplayAlert("Sin Internet", "Está trabajando sin Internet (75)", "OK");

                var clienteList = await App.SQLiteDB.GetClientesAsync(Usuario.CodigoCobr);
                if (clienteList != null)
                {
                    clientsCollection.Clear();
                    foreach (var cliente in clienteList)
                    {
                        clientsCollection.Add(cliente);
                    }

                    cvClientes.ItemsSource = clientsCollection;
                }
            }

            cvClientes.SelectedItem = null;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error en LlenarDatosAsync: {ex.Message}");
            await DisplayAlert("Error", ex.Message, "OK");
        }
    }


    private void SearchBar_TextChanged(object sender, TextChangedEventArgs e)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(e.NewTextValue))
            {
                cvClientes.ItemsSource = clientsCollection.ToList();
            }
            else
            {
                //lstClientes.ItemsSource = clientsCollection.Where(i => i.cteNombApel.ToLower().Contains(e.NewTextValue.ToLower()));
                cvClientes.ItemsSource = clientsCollection
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

    private void cvClientes_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        try
        {
            // Asegurarse de que haya al menos un elemento seleccionado
            if (e.CurrentSelection != null && e.CurrentSelection.Count > 0)
            {
                var cli = (Mcliente)e.CurrentSelection.FirstOrDefault();
                if (cli == null)
                    return;

                // Crear el objeto cliente que se pasará a la siguiente página
                Mcliente cliente = new Mcliente
                {
                    cteNumIdenti = cli.cteNumIdenti,
                    cteNombApel = cli.cteNombApel
                };

                // Navegar a la página de créditos
                Navigation.PushAsync(new Creditos(cliente, Usuario));

                // Opcional: limpiar la selección para que no quede resaltado
                ((CollectionView)sender).SelectedItem = null;
            }
        }
        catch (Exception ex)
        {
            DisplayAlert("Error", "Error " + ex.Message, "OK");
            throw;
        }
    }
}