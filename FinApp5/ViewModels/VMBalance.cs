using FinApp5.Datos;
using FinApp5.Modelo;
using System.Windows.Input;

namespace FinApp5.ViewModels
{
    public class VMBalance : BaseViewModel
    {
        #region VARIABLES
        string? _TxtFecha;
        int? _TxtDebidoRuta;
        int? _TxtDebidoDia;
        int? _TxtRecaudo;
        int? _TxtMicroseguro;
        int? _TxtDesembolsos;
        int? _TxtGastos;
        int? _TxtEntradas;
        int? _TxtSalidas;
        int? _TxtSueldos;
        int? _TxtResultado;
        int? _TxtCreditos;
        int? _TxtVisitados;
        int? _TxtSinVisitar;
        int? _TxtPrimeraVez;
        int? _TxtCancelados;
        int? _TxtCajaAnterior;
        int? _TxtTotal;
        Musuarios Usuario = new Musuarios();

        #endregion
        #region CONSTRUCTOR
        public VMBalance(INavigation navigation, Musuarios usuario)
        {
            Navigation = navigation;
            Usuario = usuario;
            ConsultarFecha();
            DebidoRuta();
            DebidoDia();
            Recaudo();
            Microseguro();
            Desembolsos();
            Gastos();
            Entradas();
            Salidas();
            Sueldos();
            Resultado();
            Creditos();
            Visitados();
            SinVisitar();
            PrimeraVez();
            Cancelados();
            CajaAnterior();
            CajaActual();
        }
        #endregion
        #region OBJETOS
        public string? TxtFecha { get { return _TxtFecha; } set { SetValue(ref _TxtFecha, value); } }
        public int? TxtDebidoRuta { get { return _TxtDebidoRuta; } set { SetValue(ref _TxtDebidoRuta, value); } }
        public int? TxtDebidoDia { get { return _TxtDebidoDia; } set { SetValue(ref _TxtDebidoDia, value); } }
        public int? TxtRecaudo { get { return _TxtRecaudo; } set { SetValue(ref _TxtRecaudo, value); } }
        public int? TxtMicroseguro { get { return _TxtMicroseguro; } set { SetValue(ref _TxtMicroseguro, value); } }
        public int? TxtDesembolsos { get { return _TxtDesembolsos; } set { SetValue(ref _TxtDesembolsos, value); } }
        public int? TxtGastos { get { return _TxtGastos; } set { SetValue(ref _TxtGastos, value); } }
        public int? TxtEntradas { get { return _TxtEntradas; } set { SetValue(ref _TxtEntradas, value); } }
        public int? TxtSalidas { get { return _TxtSalidas; } set { SetValue(ref _TxtSalidas, value); } }
        public int? TxtSueldos { get { return _TxtSueldos; } set { SetValue(ref _TxtSueldos, value); } }
        public int? TxtResultado { get { return _TxtResultado; } set { SetValue(ref _TxtResultado, value); } }
        public int? TxtCreditos { get { return _TxtCreditos; } set { SetValue(ref _TxtCreditos, value); } }
        public int? TxtVisitados { get { return _TxtVisitados; } set { SetValue(ref _TxtVisitados, value); } }
        public int? TxtSinVisitar { get { return _TxtSinVisitar; } set { SetValue(ref _TxtSinVisitar, value); } }
        public int? TxtPrimeraVez { get { return _TxtPrimeraVez; } set { SetValue(ref _TxtPrimeraVez, value); } }
        public int? TxtCancelados { get { return _TxtCancelados; } set { SetValue(ref _TxtCancelados, value); } }
        public int? TxtCajaAnterior { get { return _TxtCajaAnterior; } set { SetValue(ref _TxtCajaAnterior, value); } }
        public int? TxtTotal { get { return _TxtTotal; } set { SetValue(ref _TxtTotal, value); } }
        #endregion
        #region PROCESOS
        public void ConsultarFecha()
        {
            Mbalance? mbalance = new Mbalance();
            var funcion = new Dbalance();
            mbalance = funcion.Fecha();
            if (mbalance != null)
                TxtFecha = mbalance.Fecha;
        }
        public void DebidoRuta()
        {
            Mbalance? mbalance = new Mbalance();
            var funcion = new Dbalance();
            mbalance = funcion.DebidoRuta(Usuario);
            if (mbalance != null)
                TxtDebidoRuta = mbalance.DebidoRuta;
        }
        public void DebidoDia()
        {
            Mbalance? mbalance = new Mbalance();
            var funcion = new Dbalance();
            mbalance = funcion.DebidoDia(Usuario);
            if (mbalance != null)
                TxtDebidoDia = mbalance.DebidoDia;
        }
        public void Recaudo()
        {
            Mbalance? mbalance = new Mbalance();
            var funcion = new Dbalance();
            mbalance = funcion.Recaudo(Usuario);
            if (mbalance != null)
                TxtRecaudo = mbalance.Recaudo;
        }
        public void Microseguro()
        {
            Mbalance? mbalance = new Mbalance();
            var funcion = new Dbalance();
            mbalance = funcion.Microseguro(Usuario);
            if (mbalance != null)
                TxtMicroseguro = mbalance.Microseguro;
        }

        public void Desembolsos()
        {
            Mbalance? mbalance = new Mbalance();
            var funcion = new Dbalance();
            mbalance = funcion.Desembolsos(Usuario);
            if (mbalance != null)
                TxtDesembolsos = mbalance.Desembolsos;
        }
        public void Gastos()
        {
            Mbalance? mbalance = new Mbalance();
            var funcion = new Dbalance();
            mbalance = funcion.Gastos(Usuario);
            if (mbalance != null)
                TxtGastos = mbalance.Gastos;
        }
        public void Entradas()
        {
            Mbalance? mbalance = new Mbalance();
            var funcion = new Dbalance();
            mbalance = funcion.Entradas(Usuario);
            if (mbalance != null)
                TxtEntradas = mbalance.Entradas;
        }
        public void Salidas()
        {
            Mbalance? mbalance = new Mbalance();
            var funcion = new Dbalance();
            mbalance = funcion.Salidas(Usuario);
            if (mbalance != null)
                TxtSalidas = mbalance.Salidas;
        }
        public void Sueldos()
        {
            Mbalance? mbalance = new Mbalance();
            var funcion = new Dbalance();
            mbalance = funcion.Sueldos(Usuario);
            if (mbalance != null)
                TxtSueldos = mbalance.Sueldos;
        }
        public async void Resultado()
        {
            TxtResultado = TxtRecaudo + TxtMicroseguro - TxtDesembolsos - TxtGastos + TxtEntradas - TxtSalidas - TxtSueldos ;
        }
        public void Creditos()
        {
            Mbalance? mbalance = new Mbalance();
            var funcion = new Dbalance();
            mbalance = funcion.Creditos(Usuario);
            if (mbalance != null)
                TxtCreditos = mbalance.Creditos;
        }
        public void Visitados()
        {
            Mbalance? mbalance = new Mbalance();
            var funcion = new Dbalance();
            mbalance = funcion.Visitados(Usuario);
            if (mbalance != null)
                TxtVisitados = mbalance.Visitados;
        }
        public async void SinVisitar()
        {
            TxtSinVisitar = TxtCreditos - TxtVisitados;
        }
        public void PrimeraVez()
        {
            Mbalance? mbalance = new Mbalance();
            var funcion = new Dbalance();
            mbalance = funcion.PrimeraVez(Usuario);
            if (mbalance != null)
                TxtPrimeraVez = mbalance.PrimeraVez;
        }
        public void Cancelados()
        {
            Mbalance? mbalance = new Mbalance();
            var funcion = new Dbalance();
            mbalance = funcion.Cancelados(Usuario);
            if (mbalance != null)
                TxtCancelados = mbalance.Cancelados;
        }

        public void CajaAnterior()
        {
            Mbalance? mbalance = new Mbalance();
            var funcion = new Dbalance();
            mbalance = funcion.CajaAnterior(Usuario);
            if (mbalance != null)
                TxtCajaAnterior = mbalance.CajaAnterior;
        }
        public void CajaActual()
        {
            TxtTotal = TxtResultado + TxtCajaAnterior;
        }

        #endregion
        #region COMANDOS
        //public ICommand ConsultarFechaCommand => new Command(ConsultarFecha);

        #endregion
    }
}
