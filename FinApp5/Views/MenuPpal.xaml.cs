using FinApp5.Modelo;
using FinApp5.ViewModels;
namespace FinApp5.Views;
[QueryProperty(nameof(Usuario), "Usuario")]
public partial class MenuPpal : ContentPage
{
    double _lastScrollY = 0;
    double _maxScrollY = 0;
    // private Musuarios _usuario;
    private VMmenuPrincipal viewModel;
    public Musuarios? Usuario { get; set; }
    public MenuPpal(Musuarios usuario)
    {
        InitializeComponent();
        viewModel = new VMmenuPrincipal(Navigation, usuario);
        BindingContext = viewModel;
        AnimateCardsOnLoad();
    }
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await viewModel.InicializarAsync(); // ahora inicializa todo de forma asincrónica
        StartContinuousAnimations();
    }
    //private void btnInicio_Clicked(object sender, EventArgs e)
    //{
    //    Navigation.PushAsync(new MainPage());
    //}
    private async void btnInicio_Clicked(object sender, EventArgs e)
    {
        var mainPage = App.Services.GetRequiredService<MainPage>();
        await Navigation.PushAsync(mainPage);
    }
    private void OnScrollViewScrolled(object sender, ScrolledEventArgs e)
    {
        //if (e.ScrollY > _lastScrollY || e.ScrollY >= _maxScrollY)
        HideKeyboard();
        _lastScrollY = e.ScrollY;
    }
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
    private void OnScrollViewSizeChanged(object sender, EventArgs e)
    {
        if (sender is ScrollView scrollView)
        {
            _maxScrollY = scrollView.ContentSize.Height - scrollView.Height;
        }
    }
    private void OnTapGestureRecognizerTapped(object sender, TappedEventArgs e)
    {
        HideKeyboard();
    }

    // ============ MÉTODOS DE ANIMACIÓN ============

    // Animación de entrada de las tarjetas
    private async void AnimateCardsOnLoad()
    {
        // Ocultar las tarjetas inicialmente
        frameMaestros.Opacity = 0;
        frameTransacciones.Opacity = 0;
        frameReportes.Opacity = 0;
        frameCerrarSesion.Opacity = 0;

        frameMaestros.TranslationX = -50;
        frameTransacciones.TranslationX = -50;
        frameReportes.TranslationX = -50;
        frameCerrarSesion.TranslationX = -50;

        // Animar entrada con delay
        await Task.Delay(200);
        await Task.WhenAll(
            frameMaestros.FadeTo(1, 400),
            frameMaestros.TranslateTo(0, 0, 400, Easing.SpringOut)
        );

        await Task.Delay(100);
        await Task.WhenAll(
            frameTransacciones.FadeTo(1, 400),
            frameTransacciones.TranslateTo(0, 0, 400, Easing.SpringOut)
        );

        await Task.Delay(100);
        await Task.WhenAll(
            frameReportes.FadeTo(1, 400),
            frameReportes.TranslateTo(0, 0, 400, Easing.SpringOut)
        );

        await Task.Delay(100);
        await Task.WhenAll(
            frameCerrarSesion.FadeTo(1, 400),
            frameCerrarSesion.TranslateTo(0, 0, 400, Easing.SpringOut)
        );
    }

    // Animación al tocar una tarjeta
    private async void OnCardTapped(object sender, EventArgs e)
    {
        var frame = (Frame)sender;

        // Animación de escala y rotación
        await Task.WhenAll(
            frame.ScaleTo(0.95, 100, Easing.CubicOut),
            frame.RotateTo(2, 100, Easing.CubicOut)
        );

        await Task.WhenAll(
            frame.ScaleTo(1, 100, Easing.CubicIn),
            frame.RotateTo(0, 100, Easing.CubicIn)
        );

        // Animación del ícono
        var grid = (Grid)frame.Content;
        var icon = grid.Children[0] as Frame;

        await icon.ScaleTo(1.2, 150, Easing.SpringOut);
        await icon.ScaleTo(1, 150, Easing.SpringIn);
    }

    // Animación especial para cerrar sesión
    private async void btnInicio_Clicked_Animated(object sender, EventArgs e)
    {
        await Task.WhenAll(
            frameCerrarSesion.ScaleTo(0.9, 100),
            frameCerrarSesion.FadeTo(0.7, 100)
        );

        await Task.WhenAll(
            frameCerrarSesion.ScaleTo(1, 100),
            frameCerrarSesion.FadeTo(1, 100)
        );

        // Animación de salida
        await Task.WhenAll(
            frameCerrarSesion.ScaleTo(0.8, 200),
            frameCerrarSesion.FadeTo(0, 200),
            iconCerrarSesion.RotateTo(360, 300)
        );

        // Llama al método original (SIN await porque btnInicio_Clicked ya es async)
        btnInicio_Clicked(sender, e);
    }
    // Animaciones continuas sutiles
    private async void StartContinuousAnimations()
    {
        while (true)
        {
            // Animación sutil del ícono de cada tarjeta
            await Task.WhenAll(
                iconMaestros.ScaleTo(1.05, 2000, Easing.SinInOut),
                iconTransacciones.ScaleTo(1.05, 2000, Easing.SinInOut),
                iconReportes.ScaleTo(1.05, 2000, Easing.SinInOut)
            );

            await Task.WhenAll(
                iconMaestros.ScaleTo(1, 2000, Easing.SinInOut),
                iconTransacciones.ScaleTo(1, 2000, Easing.SinInOut),
                iconReportes.ScaleTo(1, 2000, Easing.SinInOut)
            );

            // Pausar un poco antes de repetir
            await Task.Delay(1000);
        }
    }
}