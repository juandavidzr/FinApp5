using FinApp5.Modelo;
using FinApp5.ViewModels;
using FinApp5.Views;
using FinAppMaui.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;



namespace FinApp5.ViewModels
{
    public class VMSubMenuClientes : BaseViewModel
    {
        #region VARIABLES
        string _Texto;
        Musuarios Usuario;
        #endregion
        #region CONSTRUCTOR
        public VMSubMenuClientes(INavigation navigation, Musuarios usuario)
        {
            Navigation = navigation;
            Usuario = usuario;
            

        }
        #endregion

        #region OBJETOS
        public string Texto
        {
            get { return _Texto; }
            set { SetValue(ref _Texto, value); }
        }
        #endregion
        #region PROCESOS
        public async void IrAClientes()
        {
            //await Navigation.PushAsync(new Clientes(Usuario));
            //var page = MauiProgram.Services.GetRequiredService<Clientes>();

            //var page = new Clientes(Usuario);
            //await Application.Current.MainPage.Navigation.PushAsync(page);

            var clienteService = MauiProgram.Services.GetRequiredService<ClienteService>();
            var page = new Clientes(Usuario, clienteService);
            await Navigation.PushAsync(page);
            
        }
        public void p1()
        {
            
        }
        #endregion
        #region COMANDOS
        //public ICommand subMenuClientesCommand => new Command(async () => await IrAClientes());
        public ICommand irAClientesCommand => new Command(IrAClientes);

        //public ICommand subMenuClientesCommand => new Command(IrAClientes);
        #endregion
    }
}
