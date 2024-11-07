using SQLite;

namespace FinApp5.Modelo
{
    public class Mcliente
    {
        
        public string? cteCodTipIde { get; set; }
        [PrimaryKey]
        [Indexed]
        public string? cteNumIdenti { get; set; }
        public string? cteNombApel { get; set; }
        public string? cteDireccion { get; set; }
        public string? cteDirCobCte { get; set; }
        public string? cteTeleFijo { get; set; }
        public string? cteTeleCelu { get; set; }
        public string? cteOficio { get; set; }
        public string? cteSitioTrab { get; set; }
        public string? cteCodBarDom { get; set; }
        public string? cteCodBarCob { get; set; }
        public string? cteFechaRegCte { get; set; }
        public string? cteCodRutReg { get; set; }
        public string? cteNotasGenerales { get; set; }
        public string? latitud { get; set; }
        public string? longitud { get; set; }
        public int nuevo { get; set; } //2 si fue actualizado offline, 1 si fue agregado offline, 0 si fue cargado de la web
    }
}
