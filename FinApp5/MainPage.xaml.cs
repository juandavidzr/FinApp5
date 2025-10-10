using FinApp5.Modelo;
using FinApp5.ViewModels;
using FinApp5.Views;
using FinAppMaui.Services;
using System.Windows.Input;

namespace FinApp5
{
    public partial class MainPage : ContentPage
    {
        Musuarios Usuario = new Musuarios();

        public MainPage(VMingresar vm)
        {
            InitializeComponent();
            
            Loaded += (object? s, EventArgs e) => SetFocus();

            //TxtUsuario.Text = string.Empty;
            //TxtPW.Text = string.Empty;

            BindingContext = vm; // Usa el que MAUI inyecta
        }

        private void SetFocus()
        {
            //TxtUsuario.Focus();
        }
    }
}
