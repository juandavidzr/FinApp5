using FinApp5.ViewModels;
using FinApp5.Modelo;
using System;
using Microsoft.Maui.Controls;
using FinApp5.Conexiones;
namespace FinApp5.Views;
[QueryProperty(nameof(Usuario), "usuario")]
public partial class Transacciones : ContentPage
{
    // ============ PROPIEDADES Y CAMPOS ============
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
    // ============ CONSTRUCTORES ============
    public Transacciones()
    {
        InitializeComponent();
    }
    public Transacciones(Musuarios usuario) : this()
    {
        Usuario = usuario;
        if (Usuario.CodigoCobr != null && CONEXIONMAESTRA.VerificarCon())
            App.SQLiteDB.SyncCobros(usuario.CodigoCobr, "Ruta");
    }
    // ============ CICLO DE VIDA ============
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await AnimarSlideCascade();
    }
    // ============ MÉTODOS DE ANIMACIÓN - SLIDE CASCADE ============
    private async Task AnimarSlideCascade()
    {
        // Inicializar estado inicial de las tarjetas
        cardCredito.Opacity = 0;
        cardCredito.TranslationY = 50;
        cardRecaudo.Opacity = 0;
        cardRecaudo.TranslationY = 50;
        cardCartera.Opacity = 0;
        cardCartera.TranslationY = 50;
        cardGastos.Opacity = 0;
        cardGastos.TranslationY = 50;
        // Delay inicial antes de comenzar las animaciones
        await Task.Delay(100);
        // Animar primera tarjeta (Registrar Crédito)
        var tarea1 = cardCredito.FadeTo(1, 400, Easing.CubicOut);
        var tarea2 = cardCredito.TranslateTo(0, 0, 400, Easing.CubicOut);
        await Task.WhenAll(tarea1, tarea2);
        // Delay entre tarjetas para efecto cascada
        await Task.Delay(150);
        // Animar segunda tarjeta (Realizar Recaudo)
        var tarea3 = cardRecaudo.FadeTo(1, 400, Easing.CubicOut);
        var tarea4 = cardRecaudo.TranslateTo(0, 0, 400, Easing.CubicOut);
        await Task.WhenAll(tarea3, tarea4);
        // Delay entre tarjetas
        await Task.Delay(150);
        // Animar tercera tarjeta (Enrutar Cartera)
        var tarea5 = cardCartera.FadeTo(1, 400, Easing.CubicOut);
        var tarea6 = cardCartera.TranslateTo(0, 0, 400, Easing.CubicOut);
        await Task.WhenAll(tarea5, tarea6);
        // Delay entre tarjetas
        await Task.Delay(150);
        // Animar cuarta tarjeta (Registrar Gastos)
        var tarea7 = cardGastos.FadeTo(1, 400, Easing.CubicOut);
        var tarea8 = cardGastos.TranslateTo(0, 0, 400, Easing.CubicOut);
        await Task.WhenAll(tarea7, tarea8);
    }

    // ============ MÉTODOS DE ANIMACIÓN - SCALE DOWN ============
    private async void OnCardTapped(object sender, EventArgs e)
    {
        await cardCredito.ScaleTo(0.9, 100);
        await cardCredito.ScaleTo(1, 100);
    }

    private async void OnCardTapped_Recaudo(object sender, EventArgs e)
    {
        await cardRecaudo.ScaleTo(0.9, 100);
        await cardRecaudo.ScaleTo(1, 100);
        ImageButton_Clicked(sender, e);
    }

    private async void OnCardTapped_Cartera(object sender, EventArgs e)
    {
        await cardCartera.ScaleTo(0.9, 100);
        await cardCartera.ScaleTo(1, 100);
    }

    private async void OnCardTapped_Gastos(object sender, EventArgs e)
    {
        await cardGastos.ScaleTo(0.9, 100);
        await cardGastos.ScaleTo(1, 100);
    }

    // ============ EVENTOS ============
    private async void btnInicio_Clicked(object sender, EventArgs e)
    {
        await btnBack.ScaleTo(0.95, 100);
        await btnBack.ScaleTo(1, 100);
        await Navigation.PushAsync(new MenuPpal(Usuario));
    }
    private void ImageButton_Clicked(object sender, EventArgs e)
    {
    }
}