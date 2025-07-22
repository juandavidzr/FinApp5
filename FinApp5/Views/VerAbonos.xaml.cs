namespace FinApp5.Views;
using FinApp5.Modelo;
using System;
using FinApp5.ViewModels;
using System.Data;
using System.Globalization;
using FinApp5.Conexiones;
using Microsoft.Data.SqlClient;
using System.Collections.ObjectModel;

public partial class VerAbonos : ContentPage
{
    public ObservableCollection<Mmovimiento> MovimientosCollection = new ObservableCollection<Mmovimiento>();

    public VerAbonos(string lngNumeroUniCre, string strNombreCliente, string saldo)
	{
		InitializeComponent();
        llenarAbonos(lngNumeroUniCre, strNombreCliente, saldo);

    }

    private void llenarAbonos(string lngNumeroUniCre, string strNombreCliente, string saldo)
    {
        lstMovimientos.ItemsSource = null;
        MovimientosCollection.Clear();
               
        try
        {
            if (CONEXIONMAESTRA.VerificarCon())
            {
                CONEXIONMAESTRA.Abrir();
                SqlCommand cmd = new SqlCommand("FiltrarRegistrosDePagosDelCredito", CONEXIONMAESTRA.conectar);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@lngConsecutCre", lngNumeroUniCre);
                CONEXIONMAESTRA.Abrir();
                SqlDataReader rdr = cmd.ExecuteReader();

                while (rdr.Read())
                {
                    MovimientosCollection.Add(new Mmovimiento()
                    {
                        FechaHoraReg = Convert.ToDateTime(rdr["mcrFechaHoraReg"].ToString()),
                        ValorMovto = Convert.ToDouble(rdr["mrcValorMovto"].ToString()),
                        strCodTipMov = rdr["TipMov"].ToString()
                    });
                }
                if (MovimientosCollection != null)
                {
                    lstMovimientos.ItemsSource = MovimientosCollection;
                }
                var abonos = MovimientosCollection
                    .Where(m => m.strCodTipMov == "AB") // Reemplaza con el código real si es distinto
                    .ToList();

                int cantidadAbonos = abonos.Count;
                double totalAbonos = abonos.Sum(a => a.ValorMovto);

                lblCantidadAbonos.Text = $"Cantidad Abonos: {cantidadAbonos}";
                lblTotalAbonos.Text = $"Total Abonos: $ {totalAbonos:N0}";

                var microseguros = MovimientosCollection
                    .Where(m => m.strCodTipMov == "MS") 
                    .ToList();

                int cantidadMicroseguros = microseguros.Count;
                double totalMicroseguros = microseguros.Sum(a => a.ValorMovto);

                lblCantidadMicroseguros.Text = $"Cantidad Microseguros: {cantidadMicroseguros}";
                lblTotalMicroseguros.Text = $"Total Microseguros: $ {totalMicroseguros:N0}";

                if (double.TryParse(saldo, out double saldoActual))
                    lblSaldoActual.Text = $"Saldo Actual: $ {saldoActual:N0}";
                else
                    lblSaldoActual.Text = "Saldo Actual: No válido";
            }
            else
                DisplayAlert("Sin Internet", "Esta trabajando sin Internet (53)", "OK");
        }
        catch (Exception ex)
        {
            DisplayAlert("error", ex.Message, "OK");
            throw;
        }
        finally { CONEXIONMAESTRA.Cerrar(); }
    }

    private async void Button_Cerrar_Clicked(object sender, EventArgs e)
    {
    }
}