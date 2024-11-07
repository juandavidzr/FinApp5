using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinApp5.Modelo
{
    public class Mbarrio
    {
        [PrimaryKey]
        public string? IdBarrio { get; set; }
        public string? NombreBarrio { get; set; }
    }
}
