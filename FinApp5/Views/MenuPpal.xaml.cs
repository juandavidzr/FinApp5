using FinApp5.Modelo;
using FinApp5.ViewModels;

namespace FinApp5.Views;

[QueryProperty(nameof(Usuario), "Usuario")]
public partial class MenuPpal : ContentPage
{
    // ============ PROPIEDADES Y CAMPOS ============
    double _lastScrollY = 0;
    double _maxScrollY = 0;
    private VMmenuPrincipal viewModel;
    public Musuarios? Usuario { get; set; }

    // ============ CONSTRUCTOR ============
    public MenuPpal(Musuarios usuario)
    {
        InitializeComponent();
        viewModel = new VMmenuPrincipal(Navigation, usuario);
        BindingContext = viewModel;
    }

    // ============ CICLO DE VIDA ============
    protected override async void OnAppearing()
    {
        base.OnAppearing();

        var inicializarTask = viewModel.InicializarAsync();
        var animarTask = AnimateCardsOnLoad();

        await Task.WhenAll(inicializarTask, animarTask);
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
    }

    // ============ MÉTODOS DE ANIMACIÓN - SCALE DOWN ============
    private async void OnCardTapped_Maestros(object sender, EventArgs e)
    {
        await frameMaestros.ScaleTo(0.9, 100);
        await frameMaestros.ScaleTo(1, 100);
    }

    private async void OnCardTapped_Transacciones(object sender, EventArgs e)
    {
        await frameTransacciones.ScaleTo(0.9, 100);
        await frameTransacciones.ScaleTo(1, 100);
    }

    private async void OnCardTapped_Reportes(object sender, EventArgs e)
    {
        await frameReportes.ScaleTo(0.9, 100);
        await frameReportes.ScaleTo(1, 100);
    }

    private async void OnCardTapped_CerrarSesion(object sender, EventArgs e)
    {
        await frameCerrarSesion.ScaleTo(0.9, 100);
        await frameCerrarSesion.ScaleTo(1, 100);
        btnInicio_Clicked(sender, e);
    }

    // ============ EVENTOS DE NAVEGACIÓN ============
    private async void btnInicio_Clicked(object sender, EventArgs e)
    {
        var mainPage = App.Services.GetRequiredService<MainPage>();
        await Navigation.PushAsync(mainPage);
    }

    // ============ EVENTOS DE SCROLL ============
    private void OnScrollViewScrolled(object sender, ScrolledEventArgs e)
    {
        HideKeyboard();
        _lastScrollY = e.ScrollY;
    }

    private void OnScrollViewSizeChanged(object sender, EventArgs e)
    {
        if (sender is ScrollView scrollView)
        {
            _maxScrollY = scrollView.ContentSize.Height - scrollView.Height;
        }
    }

    // ============ EVENTOS DE GESTOS ============
    private void OnTapGestureRecognizerTapped(object sender, TappedEventArgs e)
    {
        HideKeyboard();
    }

    // ============ MÉTODOS AUXILIARES ============
    private void HideKeyboard()
    {
#if ANDROID
        var context = Android.App.Application.Context;
        var inputMethodManager = (Android.Views.InputMethods.InputMethodManager)context.GetSystemService(Android.Content.Context.InputMethodService);
        var activity = Microsoft.Maui.ApplicationModel.Platform.CurrentActivity;
        var token = activity?.CurrentFocus?.WindowToken;
        inputMethodManager?.HideSoftInputFromWindow(token, Android.Views.InputMethods.HideSoftInputFlags.None);
#elif IOS
        //UIKit.UIApplication.SharedApplication.SendAction(new ObjCRuntime.Selector("resignFirstResponder"), null, null, null);
#endif
    }

    // ============ MÉTODOS DE ANIMACIÓN ============
    // Animación de entrada: Zoom & Slide
    private async Task AnimateCardsOnLoad()
    {
        // Ocultar y preparar elementos
        frameMaestros.Opacity = 0;
        frameMaestros.Scale = 0.3;
        frameMaestros.TranslationX = -150;

        frameTransacciones.Opacity = 0;
        frameTransacciones.Scale = 0.3;
        frameTransacciones.TranslationX = -150;

        frameReportes.Opacity = 0;
        frameReportes.Scale = 0.3;
        frameReportes.TranslationX = -150;

        frameCerrarSesion.Opacity = 0;
        frameCerrarSesion.Scale = 0.3;
        frameCerrarSesion.TranslationX = -150;

        await Task.Delay(100);

        // Animar con efecto zoom + slide
        await Task.WhenAll(
            frameMaestros.FadeTo(1, 350),
            frameMaestros.ScaleTo(1, 400, Easing.CubicOut),
            frameMaestros.TranslateTo(0, 0, 400, Easing.CubicOut)
        );

        await Task.Delay(80);
        await Task.WhenAll(
            frameTransacciones.FadeTo(1, 350),
            frameTransacciones.ScaleTo(1, 400, Easing.CubicOut),
            frameTransacciones.TranslateTo(0, 0, 400, Easing.CubicOut)
        );

        await Task.Delay(80);
        await Task.WhenAll(
            frameReportes.FadeTo(1, 350),
            frameReportes.ScaleTo(1, 400, Easing.CubicOut),
            frameReportes.TranslateTo(0, 0, 400, Easing.CubicOut)
        );

        await Task.Delay(80);
        await Task.WhenAll(
            frameCerrarSesion.FadeTo(1, 350),
            frameCerrarSesion.ScaleTo(1, 400, Easing.CubicOut),
            frameCerrarSesion.TranslateTo(0, 0, 400, Easing.CubicOut)
        );
    }
}