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

        }
        catch (Exception ex)
        {
            DisplayAlert("error", ex.Message, "OK");
            throw;
        }
        finally { CONEXIONMAESTRA.Cerrar(); }
    }
}