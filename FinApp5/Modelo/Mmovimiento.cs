using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinApp5.Modelo
{
    public class Mmovimiento
    {
        [PrimaryKey]
        //[Unique]
        [AutoIncrement]
        public int idMovimiento { get; set; }
        public string? NombreCteCre { get; set; }
        public string? NumeroCreAfe { get; set; }
        public double ValorMovto { get; set; }
        public DateTime FechaHoraReg { get; set; }
        public string? Descripcion { get; set; }
        public string? strCodigoRut { get; set; }
        public string? strCodTipMov { get; set; }
        public string? strCodConMov { get; set; }
        public string? strObservaRA { get; set; }
        public string? strLoginUsSe { get; set; }
        public string? strComentAbo { get; set; }
        public int nuevo { get; set; }

    }
}
