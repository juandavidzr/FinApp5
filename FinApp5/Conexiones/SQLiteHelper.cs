using FinApp5.Conexiones;
using FinApp5.Modelo;
using Microsoft.Data.SqlClient;
using SQLite;
using System.Data;

namespace FinApp5.Data
{
    public class SQLiteHelper
    {

        SQLiteAsyncConnection db;
        public SQLiteHelper(string dbPath)
        {
            db = new SQLiteAsyncConnection(dbPath);
            db.CreateTableAsync<Mcliente>().Wait();
            db.CreateTableAsync<Mbarrio>().Wait();
            db.CreateTableAsync<Mruta>().Wait();
            db.CreateTableAsync<Prestamos>().Wait();
            db.CreateTableAsync<Mmovimiento>().Wait();
            db.CreateTableAsync<Musuarios>().Wait();
            //db.CreateTableAsync<Abono>().Wait();
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
        public Task<List<Prestamos>> GetAllCredit()
        {
            return db.Table<Prestamos>().ToListAsync();
        }

        public Task<int> SaveClienteAsync(Mcliente cli)
        {
            return db.InsertAsync(cli);
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
            return db.Table<Mcliente>().ToListAsync();
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
                    com.CommandText = "UPDATE MovimientosCont SET nuevo = 0 WHERE idMovimiento = " + idMovimiento;
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
    }
}
