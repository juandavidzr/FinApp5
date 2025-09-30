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

    public RegistarGastos(Musuarios usuario)
    {
        InitializeComponent();
        llenarConceptosGastos();

        Usuario = usuario;
    }
    //private void llenarConceptosGastos()
    //{
    //    try
    //    {
    //        if (CONEXIONMAESTRA.VerificarCon())
    //        {
    //            CONEXIONMAESTRA.Abrir();
    //            SqlCommand cmd = new SqlCommand("DescargaDeConceptosDeReporteDeGastos", CONEXIONMAESTRA.conectar);
    //            cmd.CommandType = CommandType.StoredProcedure;
    //            SqlDataReader rdr = cmd.ExecuteReader();
                
    //            while (rdr.Read())
    //            {
    //                conceptosList.Add
    //                    (
    //                    new MtipoGastos
    //                    {
    //                        claCodigo = rdr["claCodigo"].ToString().Trim(),
    //                        claDescripcion = rdr["claDescripcion"].ToString().Trim()
    //                    }
    //                    );
    //            }
    //            //cmbConceptos.ItemsSource = conceptosList;
    //            //cmbConceptos.SelectedIndex = 0;
    //        }
    //        else
    //        {
    //            //List<MtipoGastos> conceptosList = new List<MtipoGastos>();
    //            llenarConceptosGastosOffLine();
    //        }
    //        cmbConceptos.ItemsSource = conceptosList;
    //        cmbConceptos.SelectedIndex = 0;
    //    }
    //    catch (Exception ex)
    //    {
    //        DisplayAlert("ERROR", ex.Message, "OK");
    //    }
    //}

    private void llenarConceptosGastos()
    {
        try
        {
            if (CONEXIONMAESTRA.VerificarCon())
            {
                using (var connection = CONEXIONMAESTRA.GetConnection())
                using (var cmd = new SqlCommand("DescargaDeConceptosDeReporteDeGastos", connection))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    connection.Open();

                    using (var rdr = cmd.ExecuteReader())
                    {
                        while (rdr.Read())
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
            else
            {
                llenarConceptosGastosOffLine();
            }

            cmbConceptos.ItemsSource = conceptosList;
            cmbConceptos.SelectedIndex = 0;
        }
        catch (Exception ex)
        {
            DisplayAlert("ERROR", ex.Message, "OK");
        }
    }


    private async Task llenarConceptosGastosOffLine()
    {
        var conceptosList = await App.SQLiteDB.GetTiposGastos();
        if (conceptosList != null && conceptosList.Any())
        {
            cmbConceptos.ItemsSource = conceptosList;
        }
    }

    private async Task Grabar(object sender, EventArgs e)
    {
        
    }

    private async Task<bool> GrabarGastoAsync(Mgasto mgasto)
    {
        try
        {
            return await Task.Run(() =>
            {
                using (SqlConnection con = CONEXIONMAESTRA.GetConnection())
                using (SqlCommand cmd = new SqlCommand("GrabarMovimientoDeGastoEnSesionDeTrabajo", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@strCodigoRuta", mgasto.strCodigoRuta);
                    cmd.Parameters.AddWithValue("@strCodConGas", mgasto.strCodConGas);
                    cmd.Parameters.AddWithValue("@fltValorMov", mgasto.fltValorMov);
                    cmd.Parameters.AddWithValue("@strDescripcion", mgasto.strDescripcion);
                    cmd.Parameters.AddWithValue("@strLoginUsSeAc", mgasto.strLoginUsSeAc);

                    con.Open();
                    cmd.ExecuteNonQuery();
                    return true;
                }
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error al grabar gasto: " + ex.Message);
            return false;
        }
    }


    //private async Task<bool> GrabarGastoAsync(Mgasto mgasto)
    //{
    //    try
    //    {
    //        return await Task.Run(() =>
    //        {
    //            CONEXIONMAESTRA.Abrir();
    //            using (SqlCommand cmd = new SqlCommand("GrabarMovimientoDeGastoEnSesionDeTrabajo", CONEXIONMAESTRA.conectar))
    //            {
    //                cmd.CommandType = CommandType.StoredProcedure;
    //                cmd.Parameters.AddWithValue("@strCodigoRuta", mgasto.strCodigoRuta);
    //                cmd.Parameters.AddWithValue("@strCodConGas", mgasto.strCodConGas);
    //                cmd.Parameters.AddWithValue("@fltValorMov", mgasto.fltValorMov);
    //                cmd.Parameters.AddWithValue("@strDescripcion", mgasto.strDescripcion);
    //                cmd.Parameters.AddWithValue("@strLoginUsSeAc", mgasto.strLoginUsSeAc);

    //                cmd.ExecuteNonQuery();
    //                return true;
    //            }
    //        });
    //    }
    //    catch (Exception ex)
    //    {
    //        Console.WriteLine(ex.Message);
    //        return false;
    //    }
    //    finally
    //    {
    //        CONEXIONMAESTRA.Cerrar();
    //    }
    //}


    //private bool GrabarGasto(Mgasto mgasto)
    //{
    //    try
    //    {
    //        using (SqlConnection con = CONEXIONMAESTRA.GetConnection())
    //        {
    //            con.Open();
    //            using (SqlCommand cmd = new SqlCommand("GrabarMovimientoDeGastoEnSesionDeTrabajo", con))
    //            {
    //                cmd.CommandType = CommandType.StoredProcedure;
    //                cmd.Parameters.AddWithValue("@strCodigoRuta", mgasto.strCodigoRuta);
    //                cmd.Parameters.AddWithValue("@strCodConGas", mgasto.strCodConGas);
    //                cmd.Parameters.AddWithValue("@fltValorMov", mgasto.fltValorMov);
    //                cmd.Parameters.AddWithValue("@strDescripcion", mgasto.strDescripcion);
    //                cmd.Parameters.AddWithValue("@strLoginUsSeAc", mgasto.strLoginUsSeAc);

    //                cmd.ExecuteNonQuery();
    //            }
    //        }
    //        return true;
    //    }
    //    catch (Exception ex)
    //    {
    //        Console.WriteLine(ex.Message);
    //        return false;
    //    }
    //}


    //private bool GrabarGasto(Mgasto mgasto)
    //{
    //    try
    //    {
    //        SqlCommand cmd = new SqlCommand("GrabarMovimientoDeGastoEnSesionDeTrabajo", CONEXIONMAESTRA.conectar);
    //        cmd.CommandType = CommandType.StoredProcedure;
    //        cmd.Parameters.AddWithValue("@strCodigoRuta", mgasto.strCodigoRuta);
    //        cmd.Parameters.AddWithValue("@strCodConGas", mgasto.strCodConGas);
    //        cmd.Parameters.AddWithValue("@fltValorMov", mgasto.fltValorMov);
    //        cmd.Parameters.AddWithValue("@strDescripcion", mgasto.strDescripcion);
    //        cmd.Parameters.AddWithValue("@strLoginUsSeAc", mgasto.strLoginUsSeAc);
    //        CONEXIONMAESTRA.Abrir();
    //        cmd.ExecuteReader();
    //        return true;
    //    }
    //    catch (Exception ex)
    //    {
    //        Console.WriteLine(ex.Message);
    //        return false;
    //    }
    //    finally { CONEXIONMAESTRA.Cerrar(); }
    //}
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
            cmbConceptos.SelectedIndex = 0;
        }
        catch (Exception)
        {
            throw;
        }
    }

    

    private async void Button_Clicked(object sender, EventArgs e)
    {
        try
        {
            bool grabo = false;
            if (Convert.ToInt32(txtValor.Text) <= 0 || String.IsNullOrWhiteSpace(txtValor.Text) || String.IsNullOrEmpty(txtValor.Text))
            {
                DisplayAlert("ERROR", "Por favor digite un valor", "OK");
                return;
            }
            if (String.IsNullOrEmpty(txtJustificacion.Text))
            {
                DisplayAlert("ERROR", "Por favor digite una Justificación", "OK");
                return;
            }

            Mgasto mgasto = new Mgasto();
            mgasto.strCodigoRuta = Usuario.CodigoCobr;

            var codigoGasto = (MtipoGastos)cmbConceptos.SelectedItem;

            if (codigoGasto == null)
            {
                DisplayAlert("FALTAN DATOS", "Por favor selecciona un concepto para registrar el gasto", "OK");
                return;
            }
            else
                mgasto.strCodConGas = codigoGasto.claCodigo;

            mgasto.fltValorMov = Convert.ToDouble(txtValor.Text);
            mgasto.strDescripcion = txtJustificacion.Text;
            mgasto.strLoginUsSeAc = Usuario.Usuario;
            if (CONEXIONMAESTRA.VerificarCon())
            {
                grabo = await GrabarGastoAsync(mgasto);
                if (grabo)
                    DisplayAlert("GUARDADO", "Registro grabado", "OK");
                else
                    DisplayAlert("ERROR", "Registro NO grabado", "OK");
            }
            else
            {
                mgasto.nuevo = 1;
                App.SQLiteDB.SaveGasto(mgasto);
                DisplayAlert("GUARDADO LOCAL", "Registro grabado localmente, Esta trabajando sin conexión", "OK");
            }
            Limpiar();
        }
        catch (Exception ex)
        {
            await DisplayAlert("error", ex.Message, "OK");
        }
    }
}