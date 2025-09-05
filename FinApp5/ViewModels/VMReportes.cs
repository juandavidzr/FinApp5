using FinApp5.Conexiones;
using FinApp5.Modelo;
using FinApp5.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace FinApp5.ViewModels
{
    class VMReportes : BaseViewModel
    {
        #region VARIABLES
        string _Texto;
        Musuarios Usuario;
        #endregion
        #region CONSTRUCTOR
        public VMReportes(INavigation navigation, Musuarios usuario)
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
        public async Task ProcesoAsyncrono()
        {

        }
        public void ProcesoSimple()
        {

        }
        public void IrAInformeDia()
        {
            Navigation?.PushAsync(new InformeDia(Usuario));
        }
        public void IrABalance()
        {
            if (CONEXIONMAESTRA.VerificarCon())
            {
                Navigation?.PushAsync(new Balance(Usuario));
            }
            else
            {
                DisplayAlert("Sin Internet", "Informe no disponible sin internet (21)", "OK");
            }
        }
        #endregion
        #region COMANDOS
        public ICommand ProcesoAsyncommand => new Command(async () => await ProcesoAsyncrono());
        public ICommand ProcesoSimpcommand => new Command(ProcesoSimple);
        public ICommand IrAInformeDiaCommand => new Command(IrAInformeDia);
        public ICommand IrABalanceActualCommand => new Command(IrABalance);
        #endregion
    }
    
    
}
