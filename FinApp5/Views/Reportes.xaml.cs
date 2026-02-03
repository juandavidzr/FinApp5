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

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await AnimateOnAppearing();
    }

    private async Task AnimateOnAppearing()
    {
        // Configurar estado inicial de los cards
        informeDiaBorder.Opacity = 0;
        informeDiaBorder.TranslationY = 30;
        balanceActualBorder.Opacity = 0;
        balanceActualBorder.TranslationY = 30;

        // Pequeña espera antes de iniciar
        await Task.Delay(150);

        // Animar cards de forma escalonada
        var tasks = new[]
        {
            AnimateCardIn(informeDiaBorder, 0),
            AnimateCardIn(balanceActualBorder, 100)
        };

        await Task.WhenAll(tasks);

        // Iniciar animación flotante
        _ = AnimateFloating(informeDiaBorder, 0);
        _ = AnimateFloating(balanceActualBorder, 600);
    }

    private async Task AnimateCardIn(Border border, int delay)
    {
        if (delay > 0)
            await Task.Delay(delay);

        await Task.WhenAll(
            border.FadeTo(1, 350, Easing.CubicOut),
            border.TranslateTo(0, 0, 400, Easing.CubicOut)
        );
    }

    private async Task AnimateFloating(Border border, int delay)
    {
        await Task.Delay(delay);

        try
        {
            while (true)
            {
                await border.TranslateTo(0, -4, 1500, Easing.SinInOut);
                await border.TranslateTo(0, 0, 1500, Easing.SinInOut);
            }
        }
        catch (Exception)
        {
            // Animación cancelada
        }
    }

    private async void OnInformeDiaTapped(object sender, EventArgs e)
    {
        await AnimateButtonPress(informeDiaBorder);
    }

    private async void OnBalanceActualTapped(object sender, EventArgs e)
    {
        await AnimateButtonPress(balanceActualBorder);
    }

    private async Task AnimateButtonPress(Border border)
    {
        await Task.WhenAll(
            border.ScaleTo(0.93, 80, Easing.CubicOut),
            border.RotateTo(1.5, 80, Easing.CubicOut)
        );

        await Task.WhenAll(
            border.ScaleTo(1.04, 120, Easing.CubicOut),
            border.RotateTo(-0.5, 120, Easing.CubicOut)
        );

        await Task.WhenAll(
            border.ScaleTo(1, 120, Easing.CubicOut),
            border.RotateTo(0, 120, Easing.CubicOut)
        );
    }

    private async void btnInicio_Clicked(object sender, EventArgs e)
    {
        // Animar salida de cards
        await Task.WhenAll(
            informeDiaBorder.FadeTo(0, 250),
            balanceActualBorder.FadeTo(0, 250)
        );

        await Shell.Current.GoToAsync("..");
    }
}