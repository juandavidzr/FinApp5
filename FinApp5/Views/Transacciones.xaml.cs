using FinApp5.ViewModels;
using FinApp5.Modelo;
using System;
using Microsoft.Maui.Controls;

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
    }

    private void btn1_Tapped(object sender, EventArgs e)
    {
    }

    private void ImageButton_Clicked(object sender, EventArgs e)
    {
    }

    private void btnInicio_Clicked(object sender, EventArgs e)
    {
        Navigation.PushAsync(new MenuPpal(Usuario));
    }
}
