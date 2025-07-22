using FinApp5.Modelo;
using FinApp5.ViewModels;
using FinApp5.Views;
using System.Windows.Input;

namespace FinApp5
{
    public partial class MainPage : ContentPage
    {
        Musuarios Usuario = new Musuarios();
        public MainPage()
        {
            InitializeComponent();
            Loaded += (s, e) => SetFocus();
            TxtUsuario.Text = string.Empty;
            TxtPW.Text = string.Empty;
            BindingContext = new VMingresar(Navigation, Usuario);
        }
        private void SetFocus()
        {
            TxtUsuario.Focus();
        }
    }
}
