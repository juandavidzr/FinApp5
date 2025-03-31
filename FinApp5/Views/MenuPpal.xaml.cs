using FinApp5.Modelo;
using FinApp5.ViewModels;
namespace FinApp5.Views;

public partial class MenuPpal : ContentPage
{
    double _lastScrollY = 0;
    double _maxScrollY = 0;
    public MenuPpal(Musuarios usuario)
	{
        InitializeComponent();
        BindingContext = new VMmenuPrincipal(Navigation, usuario);
    }

    private void btnInicio_Clicked(object sender, EventArgs e)
    {
        Navigation.PushAsync(new MainPage());
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
            UIKit.UIApplication.SharedApplication.SendAction(new ObjCRuntime.Selector("resignFirstResponder"), null, null, null);
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
}