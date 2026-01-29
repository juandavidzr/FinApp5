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

    // NUEVO: CancellationToken para cancelar búsquedas
    private CancellationTokenSource _searchCts;
    private bool _isNavigating = false;

    public ListarClientes(Musuarios usuario)
    {
        // CRÍTICO: InitializeComponent PRIMERO
        InitializeComponent();

        Usuario = usuario;

        // Cargar datos después de inicializar la UI
        _ = ListarClientesAsync(Usuario);
    }

    public async Task ListarClientesAsync(Musuarios usuario)
    {
        // REMOVIDO: InitializeComponent() ya se llamó en el constructor
        try
        {
            if (Usuario.CodigoCobr != null && CONEXIONMAESTRA.VerificarCon())
            {
                await App.SQLiteDB.SincronizarClientes(Usuario.CodigoCobr);

                if (Usuario?.Usuario != null)
                {
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
            await DisplayAlert("Error", $"Error al cargar clientes: {ex.Message}", "OK");
        }
    }

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

                // PROTECCIÓN: Verificar que el control aún exista
                if (cvClientes != null && cvClientes.Handler != null)
                {
                    cvClientes.ItemsSource = clientsCollection;
                }
            }
            else
            {
                await DisplayAlert("Sin Internet", "Está trabajando sin Internet", "OK");

                var clienteList = await App.SQLiteDB.GetClientesAsync(Usuario.CodigoCobr);
                if (clienteList != null)
                {
                    clientsCollection.Clear();
                    foreach (var cliente in clienteList)
                    {
                        clientsCollection.Add(cliente);
                    }

                    // PROTECCIÓN: Verificar que el control aún exista
                    if (cvClientes != null && cvClientes.Handler != null)
                    {
                        cvClientes.ItemsSource = clientsCollection;
                    }
                }
            }

            if (cvClientes != null)
            {
                cvClientes.SelectedItem = null;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error en LlenarDatosAsync: {ex.Message}");
            await DisplayAlert("Error", ex.Message, "OK");
        }
    }

    // CORREGIDO: SearchBar con debounce y sin crear listas nuevas
    private async void SearchBar_TextChanged(object sender, TextChangedEventArgs e)
    {
        // Cancelar búsqueda anterior
        _searchCts?.Cancel();
        _searchCts = new CancellationTokenSource();
        var token = _searchCts.Token;

        try
        {
            // Debounce: esperar 300ms antes de buscar
            await Task.Delay(300, token);

            if (token.IsCancellationRequested)
                return;

            // PROTECCIÓN: Verificar que los controles existan
            if (cvClientes == null || cvClientes.Handler == null)
                return;

            if (string.IsNullOrWhiteSpace(e.NewTextValue))
            {
                // Mostrar todos los clientes
                cvClientes.ItemsSource = clientsCollection;
            }
            else
            {
                // Filtrar - NO crear nueva lista, usar la colección existente
                var filtrados = clientsCollection
                    .Where(i => (i.cteNombApel?.ToLower() ?? "").Contains(e.NewTextValue.ToLower()))
                    .ToList();

                if (!token.IsCancellationRequested && cvClientes.Handler != null)
                {
                    cvClientes.ItemsSource = filtrados;
                }
            }
        }
        catch (OperationCanceledException)
        {
            // Búsqueda cancelada, ignorar
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error en búsqueda: {ex.Message}");
        }
    }

    // NOTA: Este método parece estar duplicado con cvClientes_SelectionChanged
    // Considera eliminarlo si usas ListView con SelectionChanged
    private async void lstClientes_ItemSelected(object sender, SelectedItemChangedEventArgs e)
    {
        if (_isNavigating || e.SelectedItem == null)
            return;

        try
        {
            _isNavigating = true;

            var cli = (Mcliente)e.SelectedItem;

            Mcliente cliente = new Mcliente
            {
                cteNumIdenti = cli.cteNumIdenti,
                cteNombApel = cli.cteNombApel
            };

            await Navigation.PushAsync(new Creditos(cliente, Usuario));

            // Limpiar selección
            if (cvClientes != null)
            {
                cvClientes.SelectedItem = null;
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", "Error: " + ex.Message, "OK");
        }
        finally
        {
            _isNavigating = false;
        }
    }

    private async void btnTransacciones_Clicked(object sender, EventArgs e)
    {
        if (_isNavigating)
            return;

        try
        {
            _isNavigating = true;
            await Navigation.PushAsync(new Transacciones(Usuario));
        }
        finally
        {
            _isNavigating = false;
        }
    }

    private async void btnInicio_Clicked(object sender, EventArgs e)
    {
        if (_isNavigating)
            return;

        try
        {
            _isNavigating = true;
            await Navigation.PushAsync(new MenuPpal(Usuario));
        }
        finally
        {
            _isNavigating = false;
        }
    }

    // Este parece ser el método correcto si usas ListView
    private async void cvClientes_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (_isNavigating)
            return;

        try
        {
            // Asegurarse de que haya al menos un elemento seleccionado
            if (e.CurrentSelection != null && e.CurrentSelection.Count > 0)
            {
                _isNavigating = true;

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
                await Navigation.PushAsync(new Creditos(cliente, Usuario));

                // Limpiar la selección
                if (cvClientes != null && cvClientes.Handler != null)
                {
                    cvClientes.SelectedItem = null;
                }
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", "Error: " + ex.Message, "OK");
        }
        finally
        {
            _isNavigating = false;
        }
    }

    // NUEVO: Limpiar recursos al salir de la página
    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        _searchCts?.Cancel();
        _searchCts?.Dispose();
    }
}