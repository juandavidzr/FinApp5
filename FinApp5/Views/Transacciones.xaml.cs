using FinApp5.ViewModels;

namespace FinApp5.Views;
using FinApp5.Modelo;
using System;

public partial class Transacciones : ContentPage
{
    public Transacciones(Musuarios usuario)
    {
        InitializeComponent();
        BindingContext = new VMTransacciones(Navigation, usuario);
    }
    private void btn1_Tapped(object sender, EventArgs e)
    {
    }
    private void ImageButton_Clicked(object sender, EventArgs e)
    {
    }
}