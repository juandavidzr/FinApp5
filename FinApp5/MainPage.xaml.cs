using FinApp5.Modelo;
using FinApp5.ViewModels;

namespace FinApp5
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();

            // Conectamos la vista con el ViewModel
            BindingContext = new VMingresar(Navigation, new Musuarios());
        }
    }
}
