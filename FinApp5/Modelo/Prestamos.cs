using SQLite;

namespace FinApp5.Modelo
{
    public class Prestamos
    {
        [PrimaryKey, AutoIncrement, Unique]
        public int rowid { get; set; }
        public int NumPrestamo { get; set; }
        public string? idCliente { get; set; }
        public string? nombreCliente { get; set; }
        public string? fechaPrestamo { get; set; }
        public string? codigoRuta { get; set; }
        public double cantidadPrestada { get; set; }
        public double interes { get; set; }
        public string? codigoPlan { get; set; }
        public int numeroCuotas { get; set; }
        public string? observaciones { get; set; }
        public int vigente { get; set; }
        public int activo { get; set; }
        public string? fechaCancelacion { get; set; }
        public int refinanciado { get; set; }
        public int trasladado { get; set; }
        public string? fechaTraCue { get; set; }
        public double cantidadCreVig { get; set; }
        public double saldoActualCre { get; set; }
        public int numCuoPag { get; set; }
        public int numCuoPen { get; set; }
        public string? fecUltPag { get; set; }
        public int valUltPag { get; set; }
        public string? fecVenCre { get; set; }
        public int numCuoAtra { get; set; }
        public double valorAtrazo { get; set; }
        public double valorCuoPen { get; set; }
        public int posRutCre { get; set; }
        public int tiempoDias { get; set; }
        public string? desDiaPago { get; set; }
        public double valorMicroSeg { get; set; }
        public double salTotPenCte { get; set; }
        public string? fechaUltCreOto { get; set; }
        public double valCuotaPag { get; set; }
        public int diaProPagCre { get; set; }
        public int marAboCreDia { get; set; }
        public double totalPagCre { get; set; }
        public int verificado { get; set; }
        public int IndicaRetaque { get; set; }
        public string? DiaSemana { get; set; }
        public int nuevo { get; set; }
        public string? DireccionCobro { get; set; }
        public string? TelefonoCell { get; set; }
        
    }
}
