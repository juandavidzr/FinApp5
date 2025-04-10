using FinApp5.Conexiones;
using FinApp5.Modelo;
using Microsoft.Data.SqlClient;
using SQLite;
using System.Collections.ObjectModel;
using System.Data;

namespace FinApp5.Data
{
    public class SQLiteHelper
    {

        SQLiteAsyncConnection db;

        private string dbPath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "db.db3");

        //(var connection = new SQLiteConnection(System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "db.db3")))

        public SQLiteHelper(string dbPath)
        {
            db = new SQLiteAsyncConnection(dbPath);
            db.CreateTableAsync<Mcliente>().Wait();
            db.CreateTableAsync<Mbarrio>().Wait();
            db.CreateTableAsync<Mruta>().Wait();
            db.CreateTableAsync<Prestamos>().Wait();
            db.CreateTableAsync<Mmovimiento>().Wait();
            db.CreateTableAsync<Musuarios>().Wait();
            db.CreateTableAsync<MtipoGastos>().Wait();
            db.CreateTableAsync<Mgasto>().Wait();
        }
        public void FiltrarInformacionPersonalDeCliente(string cteNumIdenti, string CodigoCobr,
        out double dblSalAcuCte, out int intCanCreVigCte, out double dblMonto,
        out DateTime dteFecUltCre, out DateTime dteFechaAux)
        {
            dblSalAcuCte = 0;
            intCanCreVigCte = 0;
            dblMonto = 0;
            dteFecUltCre = DateTime.MinValue;
            dteFechaAux = DateTime.MinValue;

            try
            {
                using (var db = new SQLiteConnection(dbPath))
                {
                    //string query = @"
                    //SELECT pmoCantidadPre, pmoFechaUltCreOto, pmoFecUltPag, pmoVigente, pmoActivo, pmoSaldoActualCte 
                    //FROM Clientes 
                    //WHERE strCedulaCteOC = ? AND strCodigoRuta = ?";

                    string query = @"
                        SELECT	cantidadPrestada, saldoActualCre, fechaUltCreOto, fecUltPag, vigente, activo, desDiaPago
	                    FROM	Prestamos 
	                    WHERE   Prestamos.idCliente = ? 
			                    AND codigoRuta = ? and
			                    vigente = 1
			                    and activo = 1
	                            order by NumPrestamo";

                    var result = db.Query<Prestamos>(query, cteNumIdenti, CodigoCobr);

                    if (result.Count > 0)
                    {
                        foreach (var row in result)
                        {
                            dblMonto = Convert.ToDouble(row.cantidadPrestada);
                            dteFecUltCre = Convert.ToDateTime(row.fechaUltCreOto);
                            dteFechaAux = Convert.ToDateTime(row.fecUltPag);

                            if (row.vigente == 1 && row.activo == 1)
                            {
                                double dblSaldo = Convert.ToDouble(row.saldoActualCre);
                                dblSalAcuCte += dblSaldo;
                                intCanCreVigCte++;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al consultar la base de datos: " + ex.Message);
            }
        }



        public async void SincronizarCreditos(string usuario) //inserta los nuevos creditos en el servidor
        {
            Prestamos prestamo = new Prestamos();
            try
            {
                SqlCommand cmd = new SqlCommand("GrabaCredito", CONEXIONMAESTRA.conectar);
                cmd.CommandType = CommandType.StoredProcedure;
                var prestamosList = await App.SQLiteDB.GetNewPrestamos();
                if (prestamosList != null)
                {
                    foreach (var p in prestamosList)
                    {
                        prestamo = await App.SQLiteDB.GetPrestamosByIdAsync(p.NumPrestamo);
                        if (prestamo != null && p.codigoRuta != null && p.idCliente != null && p.codigoPlan != null && p.desDiaPago != null)
                        {
                            cmd.Parameters.AddWithValue("@strCodigRut", p.codigoRuta.Trim());//1
                            cmd.Parameters.AddWithValue("@strNumIdeCte", p.idCliente.Trim());//2
                            cmd.Parameters.AddWithValue("@dblNetoEnCre", p.cantidadPrestada);//3
                            cmd.Parameters.AddWithValue("@dblPorIntCre", p.interes);//4
                            cmd.Parameters.AddWithValue("@strCodPlaPac", p.codigoPlan.Trim());//5
                            cmd.Parameters.AddWithValue("@intNumCuoCre", p.numeroCuotas);//6
                            cmd.Parameters.AddWithValue("@intNumCreVig", p.cantidadCreVig);//7
                            cmd.Parameters.AddWithValue("@dblSaldoAcCr", p.saldoActualCre);//8
                            cmd.Parameters.AddWithValue("@intNumCuoPag", p.numCuoPag);//9
                            cmd.Parameters.AddWithValue("@intNumCuoPen", p.numCuoPen);//10
                            cmd.Parameters.AddWithValue("@strFecUltPag", p.fecUltPag);//11
                            cmd.Parameters.AddWithValue("@dblValUltPag", p.valUltPag);//12
                            cmd.Parameters.AddWithValue("@strFecVtoCre", p.fecVenCre);//13
                            cmd.Parameters.AddWithValue("@intPosCreEnr", p.posRutCre);//14
                            cmd.Parameters.AddWithValue("@intTieDiaCre", p.tiempoDias);//15
                            cmd.Parameters.AddWithValue("@strDesDiaPag", p.desDiaPago.Trim());//16
                            cmd.Parameters.AddWithValue("@dblValMicSeg", p.valorMicroSeg);//17
                            cmd.Parameters.AddWithValue("@sglSalAcuCte", p.salTotPenCte);//18
                            cmd.Parameters.AddWithValue("@strFecUltCre", p.fechaUltCreOto);//19
                            cmd.Parameters.AddWithValue("@dblValCuoPag", p.valCuotaPag);//20
                            cmd.Parameters.AddWithValue("@intNumDiaPPC", p.diaProPagCre);//21
                            cmd.Parameters.AddWithValue("@dblTotPagCre", p.totalPagCre);//22
                            cmd.Parameters.AddWithValue("@strNomCteCre", p.nombreCliente);//23
                            cmd.Parameters.AddWithValue("@strLoginUsSe", usuario); //24
                            cmd.Parameters.AddWithValue("@NotaCredit", p.observaciones); //25

                            CONEXIONMAESTRA.Abrir();
                            cmd.ExecuteReader();
                            cmd.Parameters.Clear();

                            prestamo.nuevo = 0;
                            var respuesta = App.SQLiteDB.UpdatePrestamoAsync(prestamo);
                        }
                        CONEXIONMAESTRA.Cerrar();
                    }
                }
            }
            catch (Exception ex)
            {
                //_ = DisplayAlert("error", ex.Message, "OK");
                Console.WriteLine(ex.Message);
            }
            finally { CONEXIONMAESTRA.Cerrar(); }
        }

        public async void GetClientes(string CodigoRuta) // Trae del servidor todos los clientes y los guarda en el cell localmente
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
                    Console.WriteLine("Habia registros pendientes por actualizar");
                //await DisplayAlert("Actualizar", "Habia registros pendientes por actualizar", "OK");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);

            }
            finally { CONEXIONMAESTRA.Cerrar(); }
        }

        public async void SincronizarGastos(string usuario) //inserta los nuevos gastos en el servidor
        {
            Mgasto gasto = new Mgasto();
            try
            {
                SqlCommand cmd = new SqlCommand("GrabarMovimientoDeGastoEnSesionDeTrabajo", CONEXIONMAESTRA.conectar);
                cmd.CommandType = CommandType.StoredProcedure;
                List<Mgasto>? gastosList = await App.SQLiteDB.GetGastosNew();
                if (gastosList.Count > 0)
                {
                    foreach (Mgasto g in gastosList)
                    {
                        gasto = await App.SQLiteDB.GetGastoByIdAsync(g.idGasto);
                        if (gasto != null)
                        {
                            cmd.Parameters.AddWithValue("@strCodigoRuta", g.strCodigoRuta);
                            cmd.Parameters.AddWithValue("@strCodConGas", g.strCodConGas);
                            cmd.Parameters.AddWithValue("@fltValorMov", g.fltValorMov);
                            cmd.Parameters.AddWithValue("@strDescripcion", g.strDescripcion);
                            cmd.Parameters.AddWithValue("@strLoginUsSeAc", usuario);
                            CONEXIONMAESTRA.Abrir();
                            cmd.ExecuteReader();
                            cmd.Parameters.Clear();
                            gasto.nuevo = 0;
                            await App.SQLiteDB.UpdateGastoAsync(gasto);
                        }
                        CONEXIONMAESTRA.Cerrar();
                    }
                }
            }
            catch (Exception ex)
            {
                //_ = DisplayAlert("error", ex.Message, "OK");
                Console.WriteLine(ex.Message);
            }
            finally { CONEXIONMAESTRA.Cerrar(); }
        }

        public Task<int> UpdateGastoAsync(Mgasto gasto)
        {
            return db.UpdateAsync(gasto);
        }

        public Task<Mgasto> GetGastoByIdAsync(int idGasto)
        {
            return db.Table<Mgasto>().Where(c => c.idGasto == idGasto).FirstOrDefaultAsync();
        }

        public Task<List<Mgasto>> GetGastosNew()
        {
            return db.Table<Mgasto>().Where(c => c.nuevo == 1).ToListAsync();
        }

        public async void SincronizarClientes(string CodigoRuta) //inserta los nuevos clientes en el servidor 
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
                        if (a.cteNumIdenti != null) // Verificar si cteNumIdenti no es nulo
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
                                cliente.nuevo = 0;
                                await App.SQLiteDB.UpdateClienteAsync(cliente);
                            }

                        }
                    }
                }
                CONEXIONMAESTRA.Cerrar();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                //_ = DisplayAlert("error", ex.Message, "OK");
                //cliente.nuevo = 0;
                //await App.SQLiteDB.UpdateClienteAsync(cliente);
            }
            finally { CONEXIONMAESTRA.Cerrar(); }
        }
        public Task<List<Prestamos>> GetCreditos()
        {
            return db.Table<Prestamos>().Where(c => c.nuevo != 1).ToListAsync();
        }
        /// <summary>
        /// Obtener todos los creditos
        /// </summary>
        /// <returns></returns>
        public async Task<ObservableCollection<Prestamos>> GetAllCredit(string? code)
        {
            try
            {
                return await Task.Run(async () =>
                {
                    var dt = await db.Table<Prestamos>().Where(c => c.codigoRuta == code && c.IndicaRetaque == 0 && c.marAboCreDia == 0).OrderBy(o => o.posRutCre).ToListAsync();

                    return new ObservableCollection<Prestamos>(dt);
                });
            }
            catch (Exception ex)
            {
                throw new Exception(code, ex);
            }
        }

        public async Task<List<Prestamos>> FiltrarCreditosDeRutaSegunCriterio(string codigoRuta, int selector)
        {
            string query = @" SELECT Prestamos.nombreCliente, 
            Prestamos.NumPrestamo, 
            Prestamos.posRutCre
            FROM Mcliente 
            INNER JOIN Prestamos 
            ON Mcliente.cteNumIdenti = Prestamos.idCliente
            WHERE Prestamos.codigoRuta = ? 
            AND Prestamos.Vigente = 1 
            AND Prestamos.Activo = 1 ";

            if (selector == 2)
            {
                query += "AND Prestamos.posRutCre <> -1 ";
                query += "ORDER BY Prestamos.posRutCre";
            }
            else
            {
                query += "ORDER BY Mcliente.cteNombApel";
            }

            return await db.QueryAsync<Prestamos>(query, codigoRuta);
        }
        

        /// <summary>
        /// Actualizar la posición de un crédito en la ruta destino
        /// </summary>
        /// <param name="nuevaPosicion"></param>
        /// <param name="numeroCredito"></param>
        /// <param name="codigoRuta"></param>
        /// <param name="numeroCreCambiaPos"></param>
        /// <returns></returns>
        public async Task ActualizarPosicionDeCreditoEnRutaDestinoAsync(int nuevaPosicion, string numeroCredito, string codigoRuta, string numeroCreCambiaPos)
        {
            await ActualizarCreditoAsync(nuevaPosicion, numeroCredito, codigoRuta);
            var creditos = await ObtenerCreditosPorRutaAsync(nuevaPosicion, numeroCredito, codigoRuta);
            if (creditos != null && creditos.Count > 0)
            {
                await ReasignarPosicionesAsync(creditos, nuevaPosicion);
            }

        }

        public async Task ActualizarCreditoAsync(int? nuevaPosicion, string numeroCredito, string codigoRuta)
        {

            string updateCommandText = @"UPDATE Prestamos SET posRutCre = ? , PosActualizada = 1 WHERE NumPrestamo = ? ";

            await db.ExecuteAsync(updateCommandText, nuevaPosicion, numeroCredito);

        }

        public async Task ActualizarPosActualizada()
        {

            string updateCommandText = @"UPDATE Prestamos SET PosActualizada = 0 ";

            await db.ExecuteAsync(updateCommandText);

        }

        private async Task<List<long>> ObtenerCreditosPorRutaAsync(int nuevaPosicion, string numeroCredito, string codigoRuta)
        {
            string selectCommandText = @"SELECT NumPrestamo FROM Prestamos 
                                                WHERE Vigente = 1 AND Activo = 1 
                                                AND posRutCre <> -1 and posRutCre >= ? and NumPrestamo <> ? 
                                                AND codigoRuta = ? ORDER BY posRutCre , NumPrestamo ";
            return await db.QueryScalarsAsync<long>(selectCommandText, nuevaPosicion, numeroCredito, codigoRuta);
        }

        public Task<List<Prestamos>> ConsultarCambioDeRutaOffline()
        {
            return db.Table<Prestamos>().Where(p => p.PosActualizada == 1).ToListAsync();
        }

        private async Task ReasignarPosicionesAsync(List<long> creditos, int nuevaPosicion)
        {
            int contador = nuevaPosicion + 1;
            foreach (var id in creditos)
            {
                string updatePositionCommandText = "UPDATE Prestamos SET posRutCre = ? WHERE NumPrestamo = ?";
                await db.ExecuteAsync(updatePositionCommandText, contador, id);
                contador++;
            }
        }

        public async Task ReasignarPosicionesServerAsync1(List<Prestamos> prestamos)
        {
            foreach (var prestamo in prestamos)
            {
                var pos = prestamo.posRutCre;
                var credito = prestamo.NumPrestamo;
                SqlCommand cmd = new("ActualizarPosicionDeCreditoEnRuta", CONEXIONMAESTRA.conectar)
                {
                    CommandType = CommandType.StoredProcedure
                };

                cmd.Parameters.AddWithValue("@strNumIdeCte", pos);
                cmd.Parameters.AddWithValue("@strNomComCte", credito);

                await cmd.ExecuteNonQueryAsync();

            }
        }
        /// <summary>
        /// Reasignar posiciones de crédito en ruta
        /// </summary>
        /// <param name="prestamos"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public async Task ReasignarPosicionesServerAsync(List<Prestamos> prestamos)
        {
            if (prestamos == null || prestamos.Count == 0)
            {
                throw new ArgumentException("La lista de préstamos no puede ser nula o vacía.", nameof(prestamos));
            }

            try
            {

                SqlCommand cmd = new SqlCommand("ActualizarPosicionDeCreditoEnRuta", CONEXIONMAESTRA.conectar);

                cmd.CommandType = CommandType.StoredProcedure;

                foreach (var prestamo in prestamos)
                {
                    cmd.Parameters.AddWithValue("@intNuePosCreRut", prestamo.posRutCre);
                    cmd.Parameters.AddWithValue("@lngNumeroCreAct", prestamo.NumPrestamo);
                    CONEXIONMAESTRA.Abrir();
                    cmd.ExecuteReader();
                    cmd.Parameters.Clear();
                    CONEXIONMAESTRA.Cerrar();
                }
                //CONEXIONMAESTRA.Cerrar();
            }
            catch (Exception ex)
            {
                // Manejo de errores, puedes loggear o lanzar una excepción personalizada
                Console.WriteLine($"Error al reasignar posiciones: {ex.Message}");
                throw;
            }
            finally
            {
                CONEXIONMAESTRA.Cerrar();
            }
        }

        public async Task<List<Prestamos>> ObtenerTodosCreditosPorRutaAsync(string codigoRuta)
        {
            string selectCommandText = @"SELECT NumPrestamo, posRutCre FROM Prestamos WHERE Vigente = 1 AND Activo = 1 AND posRutCre <> -1  
                                        AND codigoRuta = ? ORDER BY posRutCre , NumPrestamo";
            return await db.QueryAsync<Prestamos>(selectCommandText, codigoRuta);
        }

        public Task<int> SaveClienteAsync(Mcliente cli)
        {
            return db.InsertAsync(cli);
        }

        public Task<int> SaveGasto(Mgasto gasto)
        {
            return db.InsertAsync(gasto);
        }

        public Task<int> UpdateClienteAsync(Mcliente cli)
        {
            return db.UpdateAsync(cli);
        }

        public Task<int> DeleteAlumnoAsync(Mcliente cliente)
        {
            return db.DeleteAsync(cliente);
        }

        /// <summary>
        /// Recuperar todos los clientes
        /// </summary>
        /// <returns></returns>
        public Task<List<Mcliente>> GetClientesAsync()
        {
            return db.Table<Mcliente>().OrderBy(x => x.cteNumIdenti).ToListAsync();
        }

        /// <summary>
        /// Recupera cliente por id
        /// </summary>
        /// <param name="idCliente">id del Cliente q se requiere</param>
        /// <returns></returns>
        public Task<Mcliente> GetClienteByIdAsync(string idCliente)
        {
            return db.Table<Mcliente>().Where(a => a.cteNumIdenti == idCliente).FirstOrDefaultAsync();
        }

        //public Task<List<Barrio>> GetBarriosAsync(string ruta)
        public Task<List<Mbarrio>> GetBarriosAsync()
        {
            return db.Table<Mbarrio>().ToListAsync();
        }
        public Task<Mbarrio> GetBarrioByIdAsync(string id)
        {
            return db.Table<Mbarrio>().Where(a => a.IdBarrio == id).FirstOrDefaultAsync();
        }
        internal Task SaveBarrios(Mbarrio bar)
        {
            try
            {
                return db.InsertAsync(bar);
            }
            catch (Exception)
            {

                throw;
            }
        }
        public Task<int> DeleteBarrios()
        {
            return db.DeleteAllAsync<Mbarrio>();
        }

        public Task<List<Mmovimiento>> GetAbonosNew()
        {
            return db.Table<Mmovimiento>().Where(c => c.nuevo == 1).ToListAsync();
        }

        public Task<List<Mcliente>> GetClientesNew()
        {
            return db.Table<Mcliente>().Where(c => c.nuevo == 1).ToListAsync();
        }

        public Task<int> SavePrestamoAsync(Prestamos prestamo)
        {
            return db.InsertAsync(prestamo);
        }

        public async Task<List<Mruta>> getRutaAsync()
        {
            return db.Table<Mruta>().OrderBy(x => x.posicion).ToListAsync().Result;
            //return await db.Table<Mruta>().OrderBy(x => x.posicion).ToListAsync();
        }

        public Task<int> DeleteAbonos<T>()
        {
            return db.DeleteAllAsync<Mmovimiento>();
        }

        internal Task SaveRuta(Mruta ruta)
        {
            return db.InsertAsync(ruta);
        }

        public Task<int> DeleteRutaAsync<T>()
        {
            return db.DeleteAllAsync<Mruta>();
        }

        internal Task savePrestamos(Prestamos prestamos)
        {
            return db.InsertAsync(prestamos);
        }

        public Task<int> DeletePrestamosAsync<T>()
        {
            return db.DeleteAllAsync<Prestamos>();
        }

        public Task<int> DeleteMovContAsync<T>()
        {
            return db.DeleteAllAsync<Mmovimiento>();
        }

        internal Task SaveAbono(Mmovimiento abono)
        {
            return db.InsertAsync(abono);
        }

        public bool marcarAbonoSincronizado(int idMovimiento)
        {
            try
            {
                using (var connection = new SQLiteConnection(System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "db.db3")))
                {
                    SQLite.SQLiteCommand com = new SQLite.SQLiteCommand(connection);
                    com.CommandText = "UPDATE Mmovimiento SET nuevo = 0 WHERE idMovimiento = " + idMovimiento;
                    com.ExecuteNonQuery();
                    connection.Close();
                }
                return true;
            }
            catch (Exception)
            {
                return false;
                throw;
            }
            finally { CONEXIONMAESTRA.Cerrar(); }
        }

        public bool UpdatePrestamos(Prestamos prestamo)
        {
            try
            {
                using (var connection = new SQLiteConnection(System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "db.db3")))
                {
                    SQLite.SQLiteCommand com = new SQLite.SQLiteCommand(connection);

                    if (prestamo.fechaCancelacion == "01/01/0001")
                    {
                        com.CommandText = "UPDATE Prestamos SET saldoActualCre = " + prestamo.saldoActualCre +
                                          ", marAboCreDia = " + prestamo.marAboCreDia +
                                          ", fecUltPag = '" + DateTime.Today.ToString("M/dd/yyyy") + "'" +
                                          ", valUltPag = " + prestamo.valUltPag +
                                          ", fechaCancelacion = '" + prestamo.fechaCancelacion + "'" +
                                          " WHERE NumPrestamo = " + prestamo.NumPrestamo;
                    }
                    else
                    {
                        com.CommandText = "UPDATE Prestamos SET saldoActualCre = " + prestamo.saldoActualCre +
                                          ", marAboCreDia = " + prestamo.marAboCreDia +
                                          ", fecUltPag = '" + DateTime.Today.ToString("M/dd/yyyy") + "'" +
                                          ", valUltPag = " + prestamo.valUltPag +
                                          ", fechaCancelacion = '" + DateTime.Today.ToString("M/dd/yyyy") + "'" +
                                          " WHERE NumPrestamo = " + prestamo.NumPrestamo;
                    }
                    com.ExecuteNonQuery();
                    connection.Close();
                    return true;
                }
            }
            catch (Exception ex)
            {
                var err = ex.Message;
                return false;
            }
            finally { CONEXIONMAESTRA.Cerrar(); }
        }

        internal void marcarRetaque(string idCredito)
        {
            try
            {
                using (var connection = new SQLiteConnection(System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "db.db3")))
                {
                    SQLite.SQLiteCommand com = new SQLite.SQLiteCommand(connection);
                    com.CommandText = "UPDATE Prestamos SET IndicaRetaque = 1 WHERE idPrestamo = " + idCredito;
                    com.ExecuteNonQuery();
                    connection.Close();
                }

            }
            catch (Exception)
            {

                throw;
            }
            finally { CONEXIONMAESTRA.Cerrar(); }
        }

        public int LastRowID()
        {
            try
            {
                using (var connection = new SQLiteConnection(System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "db.db3")))
                {
                    SQLite.SQLiteCommand com = new SQLite.SQLiteCommand(connection);
                    com.CommandText = "Select Max(NumPrestamo) maximo from Prestamos";
                    var MaxId = com.ExecuteScalar<int>();
                    connection.Close();

                    return MaxId;

                }

            }
            catch (Exception ex)
            {
                var err = ex.Message;
                return 0;
            }
        }

        public Task<int> DeleteClientes<T>()
        {
            return db.DeleteAllAsync<Mcliente>();
        }

        public Task<List<Prestamos>> GetNewPrestamos()
        {
            return db.Table<Prestamos>().Where(p => p.nuevo == 1).ToListAsync();
        }

        public async Task<int> CountNewPrestamos()
        {
            //return db.Table<Prestamos>().Where(p => p.nuevo == 1).CountAsync();
            //var count = await db.Table<Prestamos>().CountAsync();
            var count = await db.Table<Prestamos>().Where(p => p.nuevo == 1).CountAsync();
            return count;
        }

        public Task<Prestamos> GetPrestamosByIdAsync(int id)
        {
            //return db.Table<Prestamos>().Where(p => p.IdInterno == id).FirstOrDefaultAsync();
            return db.Table<Prestamos>().Where(p => p.NumPrestamo == id).FirstOrDefaultAsync();
        }

        //public Task<int> UpdatePrestamoAsync(Prestamos prestamo)
        public bool UpdatePrestamoAsync(Prestamos prestamo)
        {
            try
            {
                using (var connection = new SQLiteConnection(System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "db.db3")))
                {
                    SQLite.SQLiteCommand com = new SQLite.SQLiteCommand(connection);
                    com.CommandText = "UPDATE Prestamos SET nuevo = 0 WHERE NumPrestamo = " + prestamo.NumPrestamo;
                    com.ExecuteNonQuery();
                    connection.Close();
                    return true;
                }
            }
            catch (Exception ex)
            {
                var err = ex.Message;
                return false;
            }
            finally { CONEXIONMAESTRA.Cerrar(); }
            //return db.UpdateAsync(prestamo);
        }

        public Task<int> CountNewClient()
        {
            var count = db.Table<Mcliente>().Where(p => p.nuevo == 1).CountAsync();
            return count;
        }

        internal async Task<int> CountNewAbonos()
        {
            var count = await db.Table<Mmovimiento>().Where(a => a.nuevo == 1).CountAsync();
            return count;
        }

        public void saveUsuario(Musuarios usuario)
        {
            db.InsertAsync(usuario);
        }

        public Task<Musuarios> GetUsuarioById(string? codigoCobr)
        {
            return db.Table<Musuarios>().Where(c => c.CodigoCobr == codigoCobr).FirstOrDefaultAsync();
        }

        public Task<Musuarios> GetUsuarioByIdandPw(string TxtUsuario, string TxtPw)
        {
            return db.Table<Musuarios>().Where(c => c.Usuario == TxtUsuario && c.pw == TxtPw).FirstOrDefaultAsync();
        }

        internal void SaveTipoGasto(MtipoGastos gasto)
        {
            db.InsertAsync(gasto);
        }

        public Task<List<MtipoGastos>> GetTiposGastos()
        {
            return db.Table<MtipoGastos>().ToListAsync();
        }

        public Task<int> DeleteUsuarios<T>()
        {
            return db.DeleteAllAsync<Musuarios>();
        }
    }
}
