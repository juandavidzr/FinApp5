using FinApp5.ViewModels;
using FinApp5.Modelo;

namespace FinApp5.Views;

public partial class Reportes : ContentPage
{
	public Reportes(Musuarios usuario)
	{
		InitializeComponent();
        BindingContext = new VMReportes(Navigation, usuario);
    }
}