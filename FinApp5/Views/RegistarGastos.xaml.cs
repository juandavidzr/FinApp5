using CommunityToolkit.Maui.Converters;
using FinApp5.Conexiones;
using FinApp5.Modelo;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Threading.Tasks;

namespace FinApp5.Views;

public partial class RegistarGastos : ContentPage
{
    Musuarios Usuario = new Musuarios();
    Mgasto mgasto = new Mgasto();
    List<MtipoGastos> conceptosList = new List<MtipoGastos>();

    // NUEVO: Flag para evitar guardados múltiples
    private bool _isSaving = false;

    public RegistarGastos(Musuarios usuario)
    {
        InitializeComponent();
        Usuario = usuario;

        // CRÍTICO: Llamar de forma asíncrona sin bloquear el constructor
        _ = InicializarAsync();
    }

    // NUEVO: Método para inicializar de forma asíncrona
    private async Task InicializarAsync()
    {
        try
        {
            await llenarConceptosGastosAsync();
        }
        catch (Exception ex)
        {
            await DisplayAlert("ERROR", $"Error al inicializar: {ex.Message}", "OK");
        }
    }

    // CORREGIDO: Ahora es completamente asíncrono
    private async Task llenarConceptosGastosAsync()
    {
        try
        {
            if (CONEXIONMAESTRA.VerificarCon())
            {
                // Cargar desde servidor
                using (var connection = CONEXIONMAESTRA.GetConnection())
                {
                    await connection.OpenAsync();

                    using (var cmd = new SqlCommand("DescargaDeConceptosDeReporteDeGastos", connection))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        using (var rdr = await cmd.ExecuteReaderAsync())
                        {
                            while (await rdr.ReadAsync())
                            {
                                conceptosList.Add(new MtipoGastos
                                {
                                    claCodigo = rdr["claCodigo"].ToString().Trim(),
                                    claDescripcion = rdr["claDescripcion"].ToString().Trim()
                                });
                            }
                        }
                    }
                }
            }
            else
            {
                // Cargar desde SQLite
                await llenarConceptosGastosOffLineAsync();
            }

            // Actualizar UI en el hilo principal
            MainThread.BeginInvokeOnMainThread(() =>
            {
                if (conceptosList != null && conceptosList.Any())
                {
                    cmbConceptos.ItemsSource = conceptosList;
                    cmbConceptos.SelectedIndex = 0;
                }
            });
        }
        catch (Exception ex)
        {
            await DisplayAlert("ERROR", ex.Message, "OK");
        }
    }

    // CORREGIDO: Nombre consistente y asíncrono
    private async Task llenarConceptosGastosOffLineAsync()
    {
        var conceptosOffline = await App.SQLiteDB.GetTiposGastos();

        if (conceptosOffline != null && conceptosOffline.Any())
        {
            conceptosList.AddRange(conceptosOffline);
        }
    }

    // CORREGIDO: Método de grabado asíncrono mejorado
    private async Task<bool> GrabarGastoAsync(Mgasto mgasto)
    {
        try
        {
            using (SqlConnection con = CONEXIONMAESTRA.GetConnection())
            {
                await con.OpenAsync();

                using (SqlCommand cmd = new SqlCommand("GrabarMovimientoDeGastoEnSesionDeTrabajo", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@strCodigoRuta", mgasto.strCodigoRuta);
                    cmd.Parameters.AddWithValue("@strCodConGas", mgasto.strCodConGas);
                    cmd.Parameters.AddWithValue("@fltValorMov", mgasto.fltValorMov);
                    cmd.Parameters.AddWithValue("@strDescripcion", mgasto.strDescripcion);
                    cmd.Parameters.AddWithValue("@strLoginUsSeAc", mgasto.strLoginUsSeAc);

                    await cmd.ExecuteNonQueryAsync();
                    return true;
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error al grabar gasto: " + ex.Message);
            return false;
        }
    }

    private void BtnLimpiar_Clicked(object sender, EventArgs e)
    {
        Limpiar();
    }

    private void Limpiar()
    {
        try
        {
            txtValor.Text = string.Empty;
            txtJustificacion.Text = string.Empty;

            if (cmbConceptos.ItemsSource != null)
            {
                cmbConceptos.SelectedIndex = 0;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al limpiar: {ex.Message}");
        }
    }

    // CORREGIDO: Botón grabar completamente asíncrono con protecciones
    private async void Button_Clicked(object sender, EventArgs e)
    {
        // Evitar clics múltiples
        if (_isSaving)
            return;

        try
        {
            _isSaving = true;

            // Cambiar texto del botón para feedback visual
            if (sender is Button btn)
            {
                btn.IsEnabled = false;
                btn.Text = "Guardando...";
            }

            // Validaciones
            if (string.IsNullOrWhiteSpace(txtValor.Text) ||
                !double.TryParse(txtValor.Text, out double valor) ||
                valor <= 0)
            {
                await DisplayAlert("ERROR", "Por favor digite un valor válido", "OK");
                return;
            }

            if (string.IsNullOrEmpty(txtJustificacion.Text))
            {
                await DisplayAlert("ERROR", "Por favor digite una justificación", "OK");
                return;
            }

            var codigoGasto = (MtipoGastos)cmbConceptos.SelectedItem;

            if (codigoGasto == null)
            {
                await DisplayAlert("FALTAN DATOS", "Por favor selecciona un concepto para registrar el gasto", "OK");
                return;
            }

            // Crear objeto de gasto
            Mgasto mgasto = new Mgasto
            {
                strCodigoRuta = Usuario.CodigoCobr,
                strCodConGas = codigoGasto.claCodigo,
                fltValorMov = valor,
                strDescripcion = txtJustificacion.Text,
                strLoginUsSeAc = Usuario.Usuario
            };

            bool grabo = false;

            if (CONEXIONMAESTRA.VerificarCon())
            {
                // MODO ONLINE
                grabo = await GrabarGastoAsync(mgasto);

                if (grabo)
                {
                    await DisplayAlert("ÉXITO", "Registro grabado correctamente", "OK");
                    Limpiar();
                }
                else
                {
                    await DisplayAlert("ERROR", "No se pudo grabar el registro", "OK");
                }
            }
            else
            {
                // MODO OFFLINE
                mgasto.nuevo = 1;

                // ✅ CRÍTICO: Usar await si SaveGasto es asíncrono
                // Si SaveGasto NO es async, déjalo como está
                // Si SaveGasto ES async (SaveGastoAsync), usa await:
                App.SQLiteDB.SaveGasto(mgasto);

                await DisplayAlert("GUARDADO LOCAL", "Registro guardado localmente. Se sincronizará cuando haya conexión.", "OK");
                Limpiar();
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("ERROR", $"Error al guardar: {ex.Message}", "OK");
        }
        finally
        {
            _isSaving = false;

            // Restaurar botón
            if (sender is Button btn)
            {
                btn.IsEnabled = true;
                btn.Text = "Grabar";
            }
        }
    }

    // NUEVO: Limpiar recursos al salir
    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        // Si tienes operaciones pendientes, cancélalas aquí
    }
}