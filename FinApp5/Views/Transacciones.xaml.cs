using FinApp5.ViewModels;
using FinApp5.Modelo;
using System;
using Microsoft.Maui.Controls;
using FinApp5.Conexiones;

namespace FinApp5.Views;

//El atributo debe estar en la clase
[QueryProperty(nameof(Usuario), "usuario")]
public partial class Transacciones : ContentPage
{
    private Musuarios _usuario;
    public Musuarios Usuario
    {
        get => _usuario;
        set
        {
            _usuario = value;
            BindingContext = new VMTransacciones(Navigation, _usuario);
        }
    }

    // 🔹 Constructor sin parámetros (requerido por Shell)
    public Transacciones()
    {
        InitializeComponent();
    }

    // 🔹 Constructor con parámetros (para instancias manuales)
    public Transacciones(Musuarios usuario) : this() // Llama al constructor sin parámetros
    {
        Usuario = usuario;
        if (Usuario.CodigoCobr != null && CONEXIONMAESTRA.VerificarCon())
            App.SQLiteDB.SyncCobros(usuario.CodigoCobr, "Ruta");
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        AnimateOnAppearing();
    }

    private async void AnimateOnAppearing()
    {
        // Animar cards con efecto de entrada escalonado (SIN animar el logo)
        creditoBorder.Opacity = 0;
        creditoBorder.TranslationY = 50;
        recaudoBorder.Opacity = 0;
        recaudoBorder.TranslationY = 50;
        enrutarBorder.Opacity = 0;
        enrutarBorder.TranslationY = 50;
        gastosBorder.Opacity = 0;
        gastosBorder.TranslationY = 50;

        await Task.Delay(100);

        var fadeInTasks = new[]
        {
            AnimateCardIn(creditoBorder, 0),
            AnimateCardIn(recaudoBorder, 100),
            AnimateCardIn(enrutarBorder, 200),
            AnimateCardIn(gastosBorder, 300)
        };

        await Task.WhenAll(fadeInTasks);

        // Iniciar animación de hover continua
        StartHoverAnimations();
    }

    private async Task AnimateCardIn(Border border, uint delay)
    {
        await Task.Delay((int)delay);
        await Task.WhenAll(
            border.FadeTo(1, 400, Easing.CubicOut),
            border.TranslateTo(0, 0, 500, Easing.CubicOut)
        );
    }

    private void StartHoverAnimations()
    {
        // Animación flotante sutil para cada card
        AnimateFloating(creditoBorder, 0);
        AnimateFloating(recaudoBorder, 500);
        AnimateFloating(enrutarBorder, 1000);
        AnimateFloating(gastosBorder, 1500);
    }

    private async void AnimateFloating(Border border, int delay)
    {
        await Task.Delay(delay);

        try
        {
            while (true)
            {
                await border.TranslateTo(0, -5, 1500, Easing.SinInOut);
                await border.TranslateTo(0, 0, 1500, Easing.SinInOut);
            }
        }
        catch (TaskCanceledException)
        {
            // La animación se cancela cuando se navega fuera de la página
        }
    }

    // Animaciones de tap para cada botón
    private async void OnCreditoTapped(object sender, EventArgs e)
    {
        await AnimateButtonPress(creditoBorder);
    }

    private async void OnRecaudoTapped(object sender, EventArgs e)
    {
        await AnimateButtonPress(recaudoBorder);
        ImageButton_Clicked(sender, e);
    }

    private async void OnEnrutarTapped(object sender, EventArgs e)
    {
        await AnimateButtonPress(enrutarBorder);
    }

    private async void OnGastosTapped(object sender, EventArgs e)
    {
        await AnimateButtonPress(gastosBorder);
    }

    private async Task AnimateButtonPress(Border border)
    {
        // Efecto de presión con escala y rotación
        await Task.WhenAll(
            border.ScaleTo(0.92, 100, Easing.CubicOut),
            border.RotateTo(2, 100, Easing.CubicOut)
        );

        await Task.WhenAll(
            border.ScaleTo(1.05, 150, Easing.CubicOut),
            border.RotateTo(-1, 150, Easing.CubicOut)
        );

        await Task.WhenAll(
            border.ScaleTo(1, 150, Easing.CubicOut),
            border.RotateTo(0, 150, Easing.CubicOut)
        );
    }

    // Tus métodos existentes
    private void btn1_Tapped(object sender, EventArgs e)
    {
    }

    private void ImageButton_Clicked(object sender, EventArgs e)
    {
    }

    private async void btnInicio_Clicked(object sender, EventArgs e)
    {
        // Animar salida de cards
        await Task.WhenAll(
            creditoBorder.FadeTo(0, 300),
            recaudoBorder.FadeTo(0, 300),
            enrutarBorder.FadeTo(0, 300),
            gastosBorder.FadeTo(0, 300)
        );

        // Tu código de navegación existente
        await Navigation.PushAsync(new MenuPpal(Usuario));
    }
}