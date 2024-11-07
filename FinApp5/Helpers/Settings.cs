using Plugin.Settings;
using Plugin.Settings.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinApp5.Helpers
{
    public class Settings
    {
        private static ISettings AppSettings => CrossSettings.Current;

        public static string GeneralSettings
        {
            get => AppSettings.GetValueOrDefault(nameof(GeneralSettings), string.Empty);
            set => AppSettings.AddOrUpdateValue(nameof(GeneralSettings), value);
        }

        public static string user { get => AppSettings.GetValueOrDefault(nameof(user), string.Empty); set => AppSettings.AddOrUpdateValue(nameof(user), value); }
        public static string CodigoRuta { get => AppSettings.GetValueOrDefault(nameof(CodigoRuta), string.Empty); set => AppSettings.AddOrUpdateValue(nameof(CodigoRuta), value); }
        public static string NombreRuta { get => AppSettings.GetValueOrDefault(nameof(NombreRuta), string.Empty); set => AppSettings.AddOrUpdateValue(nameof(NombreRuta), value); }
        public static string idRuta { get => AppSettings.GetValueOrDefault(nameof(idRuta), string.Empty); set => AppSettings.AddOrUpdateValue(nameof(idRuta), value); }
        public static string idBarrioDom { get => AppSettings.GetValueOrDefault(nameof(idBarrioDom), string.Empty); set => AppSettings.AddOrUpdateValue(nameof(idBarrioDom), value); }
        public static string idBarrioCob { get => AppSettings.GetValueOrDefault(nameof(idBarrioCob), string.Empty); set => AppSettings.AddOrUpdateValue(nameof(idBarrioCob), value); }
        public static string idCliente { get => AppSettings.GetValueOrDefault(nameof(idCliente), string.Empty); set => AppSettings.AddOrUpdateValue(nameof(idCliente), value); }
        public static string nombreCliente { get => AppSettings.GetValueOrDefault(nameof(nombreCliente), string.Empty); set => AppSettings.AddOrUpdateValue(nameof(nombreCliente), value); }
        public static string dirCliente { get => AppSettings.GetValueOrDefault(nameof(dirCliente), string.Empty); set => AppSettings.AddOrUpdateValue(nameof(dirCliente), value); }
        public static string telCliente { get => AppSettings.GetValueOrDefault(nameof(telCliente), string.Empty); set => AppSettings.AddOrUpdateValue(nameof(telCliente), value); }
        public static int NumPrestamo { get => AppSettings.GetValueOrDefault(nameof(NumPrestamo), 0); set => AppSettings.AddOrUpdateValue(nameof(NumPrestamo), value); }
        public static int cuotasAtrazadas { get => AppSettings.GetValueOrDefault(nameof(cuotasAtrazadas), 0); set => AppSettings.AddOrUpdateValue(nameof(cuotasAtrazadas), value); }
        public static string fechaCredito
        {
            get => AppSettings.GetValueOrDefault(nameof(fechaCredito), string.Empty);
            set => AppSettings.AddOrUpdateValue(nameof(fechaCredito), value);
        }
        public static string fechaUltPago
        {
            get => AppSettings.GetValueOrDefault(nameof(fechaUltPago), string.Empty);
            set => AppSettings.AddOrUpdateValue(nameof(fechaUltPago), value);
        }
        public static int valorCuota { get => AppSettings.GetValueOrDefault(nameof(valorCuota), 0); set => AppSettings.AddOrUpdateValue(nameof(valorCuota), value); }
        public static string diaPago { get => AppSettings.GetValueOrDefault(nameof(diaPago), string.Empty); set => AppSettings.AddOrUpdateValue(nameof(diaPago), value); }
        public static int cuotasPendientes { get => AppSettings.GetValueOrDefault(nameof(cuotasPendientes), 0); set => AppSettings.AddOrUpdateValue(nameof(cuotasPendientes), value); }
        public static int saldo { get => AppSettings.GetValueOrDefault(nameof(saldo), 0); set => AppSettings.AddOrUpdateValue(nameof(saldo), value); }
    }
}
