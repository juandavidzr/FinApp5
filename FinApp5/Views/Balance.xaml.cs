using FinApp5.Conexiones;
using FinApp5.Modelo;
using FinApp5.ViewModels;

namespace FinApp5.Views;

public partial class Balance : ContentPage
{
    public string ruta { get; set; }
    Musuarios Usuario = new Musuarios();
    public Balance(Musuarios usuario)
	{
		InitializeComponent();
        Usuario = usuario;
        if (CONEXIONMAESTRA.VerificarCon())
        {
            BindingContext = new VMBalance(Navigation, Usuario);
        }
        else
        {
            DisplayAlert("Sin Internet", "Informe no disponible sin internet (21)", "OK");
            
        }
    }
    
}