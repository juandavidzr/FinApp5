using FinApp5.Modelo;
using FinApp5.ViewModels;

namespace FinApp5.Views;

public partial class SubMenuClientes : ContentPage
{
	public SubMenuClientes(Musuarios usuario)
	{
		InitializeComponent();
        BindingContext = new VMSubMenuClientes(Navigation, usuario);
    }
}