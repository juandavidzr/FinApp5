using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace FinApp5.ViewModels
{
    public class IndicatorViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChange(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public ICommand ActivarCommand { get; set; }

        private bool activador;

        public bool Activador
        {
            get { return activador; }
            set
            {
                if (value != activador)
                {
                    activador = value;
                    OnPropertyChange(nameof(Activador));
                }
            }
        }

        private string mensaje;

        public string Mensaje
        {
            get { return mensaje; }
            set
            {
                if (value != mensaje)
                {
                    mensaje = value;
                    OnPropertyChange(nameof(Mensaje));
                }
            }
        }

        public IndicatorViewModel()
        {
            ActivarCommand = new Command(execute: async () =>
            {
                //Mensaje = null;
                Activador = true;
                await Task.Delay(3000);
                Activador = false;

                //Mensaje = new string("!!!!!!!!!Gracias por SUSCRIBIRTE al canal\n Activa la campana de Notificaciones!!!");
            });
        }
    }
}
