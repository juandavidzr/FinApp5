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
        BindingContext = new VMBalance(Navigation, Usuario);
    }
}