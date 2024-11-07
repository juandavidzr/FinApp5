using FinApp5.Conexiones;
using FinApp5.Datos;
using FinApp5.Modelo;
using FinApp5.Views;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace FinApp5.ViewModels
{
    public class VMmenuPrincipal : BaseViewModel
    {
        #region VARIABLES
        string _Texto;
        Musuarios Usuario = new Musuarios();
        #endregion
        #region CONSTRUCTOR
        public VMmenuPrincipal(INavigation navigation, Musuarios usuario)
        {
            Navigation = navigation;
            Usuario = usuario;
            if (usuario.CodigoCobr != null)
            {
                if (CONEXIONMAESTRA.VerificarCon())
                {

                    SincronizarClientes(usuario.CodigoCobr);

                    GetBarrios(usuario.CodigoCobr); //Trae todos los barrio del servidor
                    SyncRuta(usuario.CodigoCobr); // llena la tabla ruta para poder enrrutar el cobro al momento de crearlo localmente
                    SyncCobros(usuario.CodigoCobr, "Ruta"); //descarga la cartera completa desde el servidor
                    GetClientes(usuario.CodigoCobr); // Trae del servidor todos los clientes y los guarda en el cell localmente

                    
                }
                else
                {
                    DisplayAlert("Conexion", "Esta trabajando sin conexion", "OK");
                }
            }
        }

        #endregion
        #region OBJETOS
        public string Texto
        {
            get { return _Texto; }
            set { SetValue(ref _Texto, value); }
        }
        #endregion
        #region PROCESOS

        private async void SincronizarClientes(string CodigoRuta) //inserta los nuevos clientes en el servidor
        {
            Mcliente cliente = new Mcliente();
            try
            {
                SqlCommand cmd = new SqlCommand("GrabaDatosPerCte", CONEXIONMAESTRA.conectar);
                cmd.CommandType = CommandType.StoredProcedure;
                List<Mcliente>? clienteList = await App.SQLiteDB.GetClientesNew();
                if (clienteList.Count > 0)
                {
                    foreach (var a in clienteList)
                    {
                        cliente = await App.SQLiteDB.GetClienteByIdAsync(a.cteNumIdenti);
                        if (cliente != null)
                        {
                            cmd.Parameters.AddWithValue("@strNumIdeCte", a.cteNumIdenti);
                            cmd.Parameters.AddWithValue("@strNomComCte", a.cteNombApel);
                            cmd.Parameters.AddWithValue("@strDirResCte", a.cteDireccion);
                            cmd.Parameters.AddWithValue("@strDirCobCte", a.cteDirCobCte);
                            cmd.Parameters.AddWithValue("@strNumTelFij", a.cteTeleFijo);
                            cmd.Parameters.AddWithValue("@strNumTelCel", a.cteTeleCelu);
                            cmd.Parameters.AddWithValue("@strCodBarDom", a.cteCodBarDom);
                            cmd.Parameters.AddWithValue("@strCodBarCob", a.cteCodBarCob);
                            cmd.Parameters.AddWithValue("@strCodigoRut", CodigoRuta);
                            cmd.Parameters.AddWithValue("@longitud", "0");
                            cmd.Parameters.AddWithValue("@latitud", "0");
                            cmd.Parameters.AddWithValue("@strNotasCte", a.cteNotasGenerales);

                            CONEXIONMAESTRA.Abrir();

                            cmd.ExecuteReader();
                            cmd.Parameters.Clear();
                        }
                        cliente.nuevo = 0;
                        await App.SQLiteDB.UpdateClienteAsync(cliente);
                    }
                }
                CONEXIONMAESTRA.Cerrar();
            }
            catch (Exception ex)
            {
                _ = DisplayAlert("error", ex.Message, "OK");
                //cliente.nuevo = 0;
                //await App.SQLiteDB.UpdateClienteAsync(cliente);
            }
            finally { CONEXIONMAESTRA.Cerrar(); }
        }


        private void SyncCobros(string ruta, string filtro)
        {
            try
            {
                CONEXIONMAESTRA.Abrir();
                SqlCommand cmd = new SqlCommand("DescargarCarteraCompleta", CONEXIONMAESTRA.conectar);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@strCodigoRuta", ruta);
                SqlDataReader rdr = cmd.ExecuteReader();

                App.SQLiteDB.DeletePrestamosAsync<Task>();

                while (rdr.Read())
                {
                    Prestamos prestamos = new Prestamos
                    {
                        rowid = Convert.ToInt32(rdr["pmoNumeroPre"]),
                        NumPrestamo = Convert.ToInt32(rdr["pmoNumeroPre"]),
                        idCliente = rdr["pmoIdentifiCli"].ToString(),
                        fechaPrestamo = (rdr["pmoFechaPre"].ToString()),
                        codigoRuta = ruta,
                        cantidadPrestada = Convert.ToDouble(rdr["pmoTotalPagCre"].ToString()),
                        nombreCliente = rdr["cteNombApel"].ToString(),
                        interes = Convert.ToDouble(rdr["pmoInteresPre"]),
                        codigoPlan = rdr["pmoCodigoPla"].ToString(),
                        numeroCuotas = Convert.ToInt32(rdr["pmoNumeroCuo"]),
                        observaciones = rdr["pmoObservaciones"].ToString(),
                        vigente = Convert.ToInt32(rdr["pmoVigente"]),
                        activo = Convert.ToInt32(rdr["pmoActivo"]),
                        fechaCancelacion = rdr["pmpFechaCan"].ToString(),
                        refinanciado = Convert.ToInt32(rdr["pmoRefinanciado"]),
                        trasladado = Convert.ToInt32(rdr["pmoTrasladado"]),
                        fechaTraCue = (rdr["pmoFechaTraCue"].ToString()),
                        cantidadCreVig = Convert.ToDouble(rdr["pmoCantidadCreVig"]),
                        saldoActualCre = Convert.ToDouble(rdr["pmoSaldoActualCte"]),
                        numCuoPag = Convert.ToInt32(rdr["pmoNumCuoPag"]),
                        numCuoPen = Convert.ToInt32(rdr["pmoNumCuoPen"]),
                        fecUltPag = rdr["pmoFecUltPag"].ToString(),
                        valUltPag = Convert.ToInt32(rdr["pmoValUltPag"]),
                        fecVenCre = (rdr["pmoFecVenCre"].ToString()),
                        numCuoAtra = Convert.ToInt32(rdr["pmoNumCuoAtra"]),
                        valorAtrazo = Convert.ToDouble(rdr["pmoValorAtrazo"]),
                        valorCuoPen = Convert.ToDouble(rdr["pmoValorCuoPen"]),
                        posRutCre = Convert.ToInt32(rdr["pmoPosRutCre"]),
                        tiempoDias = Convert.ToInt32(rdr["pmoTiempoDias"]),
                        desDiaPago = rdr["pmoDesDiaPago"].ToString(),
                        valorMicroSeg = Convert.ToDouble(rdr["pmoValorMicroSeg"]),
                        salTotPenCte = Convert.ToDouble(rdr["pmoSalTotPenCte"]),
                        fechaUltCreOto = rdr["pmoFechaUltCreOto"].ToString(),
                        valCuotaPag = Convert.ToDouble(rdr["pmoValCuotaPag"]),
                        diaProPagCre = Convert.ToInt32(rdr["pmoDiaProPagCre"]),
                        marAboCreDia = Convert.ToInt32(rdr["pmoMarAboCreDia"]),
                        totalPagCre = Convert.ToDouble(rdr["pmoTotalPagCre"]),
                        verificado = Convert.ToInt32(rdr["pmoVerificado"]),
                        IndicaRetaque = Convert.ToInt32(rdr["pmoIndicaRetaque"]),
                        DiaSemana = Convert.ToString(rdr["pmoDiaSemana"]),
                        nuevo = 0
                    };
                    App.SQLiteDB.savePrestamos(prestamos);
                }
                CONEXIONMAESTRA.Cerrar();

                rdr.Close();
            }
            catch (Exception ex)
            {
                DisplayAlert("error", ex.Message, "OK");

            }
            finally { CONEXIONMAESTRA.Cerrar(); }
        }
        private void SyncRuta(string codigoRuta) // llena la tabla ruta para poder enrrutar el cobro al momento de crearlo localmente
        {
           
            try
            {
                CONEXIONMAESTRA.Abrir();
                SqlCommand cmd = new SqlCommand("ObtenerRutaActualDeCobrador", CONEXIONMAESTRA.conectar);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@strCodigoRuta", codigoRuta);
                SqlDataReader rdr = cmd.ExecuteReader();
               

                App.SQLiteDB.DeleteRutaAsync<Task>();

                Mruta ruta = new Mruta();

                while (rdr.Read())
                {
                    ruta = new Mruta
                    {
                        nombreCliente = rdr["cteNombApel"].ToString() + " - " + rdr["pmoPosRutCre"].ToString(),
                        posicion = Convert.ToInt32(rdr["pmoPosRutCre"].ToString()),
                    };
                    App.SQLiteDB.SaveRuta(ruta);
                }

                ruta = new Mruta { nombreCliente = "De primero", posicion = -2 };
                App.SQLiteDB.SaveRuta(ruta);

                ruta = new Mruta { nombreCliente = "De ultimo", posicion = -3 };
                App.SQLiteDB.SaveRuta(ruta);

                ruta = new Mruta { nombreCliente = "Posición Actual", posicion = -4 };
                App.SQLiteDB.SaveRuta(ruta);

                CONEXIONMAESTRA.Cerrar();
                rdr.Close();
            }
            catch (Exception ex)
            {
                DisplayAlert("error", ex.Message, "OK");

            }
            finally { CONEXIONMAESTRA.Cerrar(); }
        }
        private void GetBarrios(string codigoRuta)
        {
            try
            {
                CONEXIONMAESTRA.Abrir();
                SqlCommand cmd = new SqlCommand("FiltrarBarriosPorRuta", CONEXIONMAESTRA.conectar);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@strCodigoRut", codigoRuta);
                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    Mbarrio bar = new Mbarrio
                    {
                        IdBarrio = rdr["rbcCodigo"].ToString(),
                        NombreBarrio = rdr["rbcNombre"].ToString()
                    };
                    //var barrio = App.SQLiteDB.GetBarrioByIdAsync(bar.IdBarrio);
                    //if (barrio == null)
                    App.SQLiteDB.SaveBarrios(bar);
                }
                CONEXIONMAESTRA.Cerrar();
                rdr.Close();
            }
            catch (Exception ex)
            {
                DisplayAlert("error", ex.Message, "OK");
                
            }
            finally { CONEXIONMAESTRA.Cerrar(); }
        }
        private async void GetClientes(string CodigoRuta) // Trae del servidor todos los clientes y los guarda en el cell localmente
        {
            try
            {
                //CONEXIONMAESTRA.Abrir();
                SqlCommand cmd = new SqlCommand("ConsultarTodosClientes", CONEXIONMAESTRA.conectar);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@strCodRutaTra", CodigoRuta);
                cmd.Parameters.AddWithValue("@intOpcionFil", 1);
                cmd.Parameters.AddWithValue("@strCriterio", 1);

                var clientesNew = await App.SQLiteDB.CountNewClient();
                if (clientesNew == 0)
                {
                    App.SQLiteDB.DeleteClientes<Task>();
                    CONEXIONMAESTRA.Abrir();
                    SqlDataReader rdr = cmd.ExecuteReader();
                    while (rdr.Read())
                    {
                        Mcliente cli = new Mcliente
                        {
                            cteNumIdenti = rdr["cteNumIdenti"].ToString(),
                            cteNombApel = rdr["cteNombApel"].ToString(),
                            cteDirCobCte = rdr["cteDirCobCte"].ToString(),
                            cteCodBarDom = rdr["cteCodBarDom"].ToString(),
                            cteDireccion = rdr["cteDirCobCte"].ToString(),
                            cteCodBarCob = rdr["cteCodBarCob"].ToString(),
                            cteTeleCelu = rdr["cteTeleCelu"].ToString(),
                            cteTeleFijo = rdr["cteTeleFijo"].ToString(),
                            cteNotasGenerales = rdr["cteNotasGenerales"].ToString(),
                            latitud = rdr["latitud"].ToString(),
                            longitud = rdr["longitud"].ToString(),
                            nuevo = 0
                        };
                        //await App.SQLiteDB.SaveClienteAsync(cli);
                        App.SQLiteDB.SaveClienteAsync(cli);

                    }

                    rdr.Close();
                    CONEXIONMAESTRA.Cerrar();
                }
                else
                    await DisplayAlert("Actualizar", "Habia registros pendientes por actualizar", "OK");
            }
            catch (Exception ex)
            {
                DisplayAlert("error", ex.Message, "OK");

            }
            finally { CONEXIONMAESTRA.Cerrar(); }

        }

        public async Task ProcesoAsyncrono()
        {

        }
        public async void SubMenuClientes()
        {
           Navigation.PushAsync(new SubMenuClientes(Usuario));
        }
        private void IrATransacciones()
        {
            Navigation.PushAsync(new Transacciones(Usuario));
        }
        private void IrAReportes()
        {
            Navigation.PushAsync(new Reportes(Usuario));
        }
        #endregion
        #region COMANDOS
        public ICommand ProcesoAsyncommand => new Command(async () => await ProcesoAsyncrono());
        public ICommand subMenuClientesCommand => new Command(SubMenuClientes);
        public ICommand IrATransaccionesCommand => new Command(IrATransacciones);
        public ICommand IrAReportesCommand => new Command(IrAReportes);



        #endregion
    }
}
