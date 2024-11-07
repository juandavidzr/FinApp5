using FinApp5.Modelo;
using FinApp5.ViewModels;
namespace FinApp5.Views;

public partial class MenuPpal : ContentPage
{
    
    public MenuPpal(Musuarios usuario)
	{
        InitializeComponent();
        BindingContext = new VMmenuPrincipal(Navigation, usuario);
    }
}