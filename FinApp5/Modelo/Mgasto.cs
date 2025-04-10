using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinApp5.Modelo
{
    public class Mgasto
    {
        [PrimaryKey]
        [AutoIncrement]
        public int idGasto { get; set; }
        public string? strCodigoRuta { get; set; }
        public string? strCodConGas { get;  set; }
        public double? fltValorMov { get; set; }
        public string? strDescripcion { get; set; }
        public string? strLoginUsSeAc { get; set; }
        public int nuevo { get; set; } //1 si fue grabado localmente, 0 si ya fue sincronizado con la web

    }
}
