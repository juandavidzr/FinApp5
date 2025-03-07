using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Maui.Networking;

namespace FinApp5.Services
{
    public class ConexionService
    {
        private bool _tieneInternet;
        public bool TieneInternet => Connectivity.Current.NetworkAccess == NetworkAccess.Internet; // Propiedad de solo lectura
                                                                                                   // Evento para notificar cuando cambia el estado de la conexión
        public event Action<bool> ConectividadCambiada;

        public ConexionService()
        {
            _tieneInternet = Connectivity.Current.NetworkAccess == NetworkAccess.Internet;
            // Suscribirse al evento de conectividad
            Connectivity.ConnectivityChanged += (sender, e) =>
            {
                _tieneInternet = e.NetworkAccess == NetworkAccess.Internet;

                // Disparar evento para notificar a las páginas
                ConectividadCambiada?.Invoke(_tieneInternet);
            };
        }

    }
}
