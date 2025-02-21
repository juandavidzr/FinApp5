using CommunityToolkit.Maui.Converters;
using FinApp5.Conexiones;
using FinApp5.Modelo;
using Microsoft.Data.SqlClient;
using System.Data;

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

    private void llenarConceptosGastos()
    {
        try
        {
            CONEXIONMAESTRA.Abrir();
            SqlCommand cmd = new SqlCommand("DescargaDeConceptosDeReporteDeGastos", CONEXIONMAESTRA.conectar);
            cmd.CommandType = CommandType.StoredProcedure;
            SqlDataReader rdr = cmd.ExecuteReader();
            //List<MtipoGastos> conceptosList = new List<MtipoGastos>();
            while (rdr.Read())
            {
                conceptosList.Add
                    (
                    new MtipoGastos
                    {
                        claCodigo = rdr["claCodigo"].ToString().Trim(),
                        claDescripcion = rdr["claDescripcion"].ToString().Trim()
                    }
                    );
            }
            cmbConceptos.ItemsSource = conceptosList;
            cmbConceptos.SelectedIndex = 0;

        }
        catch (Exception ex)
        {
            DisplayAlert("ERROR", ex.Message, "OK");
        }
    }

    private void Button_Clicked(object sender, EventArgs e)
    {
        try
        {

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


            var codigoGasto = conceptosList[cmbConceptos.SelectedIndex].claCodigo;

            if (codigoGasto == null)
                mgasto.strCodConGas = "00001";
            else
                mgasto.strCodConGas = codigoGasto;

            mgasto.fltValorMov = Convert.ToDouble(txtValor.Text);
            mgasto.strDescripcion = txtJustificacion.Text;
            mgasto.strLoginUsSeAc = Usuario.Usuario;

            bool grabo = GrabarGasto(mgasto);

            if (grabo)
                DisplayAlert("OK", "Registro grabado", "OK");
            else
                DisplayAlert("ERROR", "Registro NO grabado", "OK");


        }
        catch (Exception ex)
        {
            DisplayAlert("error", ex.Message, "OK");
        }
    }



    private bool GrabarGasto(Mgasto mgasto)
    {
        try
        {

            SqlCommand cmd = new SqlCommand("GrabarMovimientoDeGastoEnSesionDeTrabajo", CONEXIONMAESTRA.conectar);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@strCodigoRuta", mgasto.strCodigoRuta);
            cmd.Parameters.AddWithValue("@strCodConGas", mgasto.strCodConGas.Trim());
            cmd.Parameters.AddWithValue("@fltValorMov", mgasto.fltValorMov);
            cmd.Parameters.AddWithValue("@strDescripcion", mgasto.strDescripcion.Trim());
            cmd.Parameters.AddWithValue("@strLoginUsSeAc", mgasto.strLoginUsSeAc.Trim());
            CONEXIONMAESTRA.Abrir();
            cmd.ExecuteReader();
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return false;
        }
        finally { CONEXIONMAESTRA.Cerrar(); }
    }




}