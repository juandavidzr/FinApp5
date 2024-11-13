using FinApp5.ViewModels;
using FinApp5.Views;
using System.Windows.Input;

namespace FinApp5
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
            Loaded += (s, e) => SetFocus();

            BindingContext = new VMingresar(Navigation);
        }
        private void SetFocus()
        {
            TxtUsuario.Focus();
        }
    }
}
