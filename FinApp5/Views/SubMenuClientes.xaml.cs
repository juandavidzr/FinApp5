using FinApp5.Modelo;
using FinApp5.ViewModels;

namespace FinApp5.Views;

public partial class SubMenuClientes : ContentPage
{
    private readonly Musuarios Usuario = new Musuarios();
    public SubMenuClientes(Musuarios usuario)
	{
		InitializeComponent();
        Usuario = usuario;
        BindingContext = new VMSubMenuClientes(Navigation, usuario);
    }

    private void btnInicio_Clicked(object sender, EventArgs e)
    {
        Navigation.PushAsync(new MenuPpal(Usuario));
    }
}