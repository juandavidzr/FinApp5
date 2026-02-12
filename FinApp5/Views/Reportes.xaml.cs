using FinApp5.ViewModels;
using FinApp5.Modelo;
namespace FinApp5.Views;
public partial class Reportes : ContentPage
{
    public Reportes(Musuarios usuario)
    {
        InitializeComponent();
        BindingContext = new VMReportes(Navigation, usuario);
        // Inicializar las tarjetas como invisibles y desplazadas
        InicializarEstadoInicial();
    }
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        // Ejecutar la animación Slide Cascade
        await AnimarSlideCascade();
    }
    private void InicializarEstadoInicial()
    {
        // Ocultar y desplazar las tarjetas hacia abajo
        cardInformeDia.Opacity = 0;
        cardInformeDia.TranslationY = 50;
        cardBalanceActual.Opacity = 0;
        cardBalanceActual.TranslationY = 50;
    }
    private async Task AnimarSlideCascade()
    {
        // Delay inicial antes de comenzar las animaciones
        await Task.Delay(100);
        // Animar primera tarjeta (Informe Día)
        var tarea1 = cardInformeDia.FadeTo(1, 400, Easing.CubicOut);
        var tarea2 = cardInformeDia.TranslateTo(0, 0, 400, Easing.CubicOut);
        await Task.WhenAll(tarea1, tarea2);
        // Delay entre tarjetas para efecto cascada
        await Task.Delay(150);
        // Animar segunda tarjeta (Balance Actual)
        var tarea3 = cardBalanceActual.FadeTo(1, 400, Easing.CubicOut);
        var tarea4 = cardBalanceActual.TranslateTo(0, 0, 400, Easing.CubicOut);
        await Task.WhenAll(tarea3, tarea4);
    }

    // ============ MÉTODOS DE ANIMACIÓN - SCALE DOWN ============
    private async void OnCardTapped_InformeDia(object sender, EventArgs e)
    {
        await cardInformeDia.ScaleTo(0.9, 100);
        await cardInformeDia.ScaleTo(1, 100);
    }

    private async void OnCardTapped_BalanceActual(object sender, EventArgs e)
    {
        await cardBalanceActual.ScaleTo(0.9, 100);
        await cardBalanceActual.ScaleTo(1, 100);
    }

    // ============ EVENTOS ============
    private async void btnInicio_Clicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }
}