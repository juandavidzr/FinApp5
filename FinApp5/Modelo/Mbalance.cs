using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinApp5.Modelo
{
    public class Mbalance
    {
        public string? Fecha { get; set; }
        public int? DebidoRuta {get; set;}
        public int? DebidoDia {get; set;}
        public int? Recaudo {get; set;}
        public int? Microseguro {get; set;}
        public int? Desembolsos {get; set;}
        public int? Gastos {get; set;}
        public int? Entradas {get; set;}
        public int? Salidas {get; set;}
        public int? Sueldos {get; set;}
        public int? Resultado {get; set;}
        public int? Creditos {get; set;}
        public int? Visitados {get; set;}
        public int? SinVisitar {get; set;}
        public int? PrimeraVez {get; set;}
        public int? Cancelados {get; set;}
        public int? CajaAnterior {get; set;}
        public int? Total { get; set; }
    }
}
