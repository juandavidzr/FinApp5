using FinApp5.Conexiones;
using FinApp5.Modelo;
using FinApp5.Views;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Runtime.Intrinsics.Arm;
using System.Windows.Input;

namespace FinApp5.ViewModels
{
    public class VMingresar : BaseViewModel
    {
        #region VARIABLES
        string _txtUsuario = string.Empty;
        string _txtPw = string.Empty;
        Musuarios usuario = new Musuarios();
        #endregion

        #region CONSTRUCTOR
        public VMingresar(INavigation? navigation)
        {
            
            Navigation = navigation;
        }
        #endregion

        #region OBJETOS
        public string TxtUsuario
        {
            get { return _txtUsuario; }
            set { SetValue(ref _txtUsuario, value); }
        }
        public string TxtPw
        {
            get { return _txtPw; }
            set { SetValue(ref _txtPw, value); }
        }
        #endregion

        #region PROCESOS

        private async void SincronizarClientes() //inserta los nuevos clientes en el servidor
        {
            Mcliente cliente = new Mcliente();
            try
            {
                SqlCommand cmd = new SqlCommand("GrabaDatosPerCte", CONEXIONMAESTRA.conectar);
                cmd.CommandType = CommandType.StoredProcedure;
                var clienteList = await App.SQLiteDB.GetClientesNew();
                if (clienteList.Count > 0)
                {
                    foreach (var a in clienteList)
                    {
                        cliente = await App.SQLiteDB.GetClienteByIdAsync(a.cteCodTipIde);
                        if (cliente != null)
                        {
                            cmd.Parameters.AddWithValue("@strNumIdeCte", a.cteCodTipIde);
                            cmd.Parameters.AddWithValue("@strNomComCte", a.cteNombApel);
                            cmd.Parameters.AddWithValue("@strDirResCte", a.cteDireccion);
                            cmd.Parameters.AddWithValue("@strDirCobCte", a.cteDirCobCte);
                            cmd.Parameters.AddWithValue("@strNumTelFij", a.cteTeleFijo);
                            cmd.Parameters.AddWithValue("@strNumTelCel", a.cteTeleCelu);
                            cmd.Parameters.AddWithValue("@strCodBarDom", a.cteCodBarDom);
                            cmd.Parameters.AddWithValue("@strCodBarCob", a.cteCodBarCob);
                            cmd.Parameters.AddWithValue("@strCodigoRut", a.cteCodRutReg);
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

        public async void ingresar()
        {
            if (String.IsNullOrEmpty(TxtUsuario) || String.IsNullOrEmpty(TxtPw))
            {
                await DisplayAlert("Credenciales incorrectas", "Credenciales incorrectas", "OK");
            }
            else
            {
                bool Estado = CONEXIONMAESTRA.VerificarCon();
                if (Estado)
                {
                    bool aut = Autenticar(TxtUsuario.Trim(), TxtPw.Trim());
                    if (aut)
                    {
                        await Navigation.PushAsync(new MenuPpal(usuario));
                    }
                    else
                    {
                        await DisplayAlert("Credenciales incorrectas", "Credenciales incorrectas", "OK");
                    }
                }
                else
                {
                    await DisplayAlert("Sin Internet", "Esta trabajando sin conexion", "OK");
                }
            }
            //Application.Current.MainPage = new NavigationPage(new MenuPpal());
        }
        private bool Autenticar(string login, string pass)
        {
            try
            {
                CONEXIONMAESTRA.Abrir();
                SqlCommand cmd =
                    new SqlCommand
                    ("SELECT cbrCodigoCobr, cbrNombApel, cbrNumIdenti " +
                    " FROM tbl_Cobradores WHERE cbrLogAppRut = '" + login + "' and cbrPasUniRut = '" + pass + "'", CONEXIONMAESTRA.conectar);

                SqlDataReader rdr = cmd.ExecuteReader();
                if (rdr.HasRows)
                {
                    rdr.Read();
                    usuario.CodigoCobr = rdr["cbrCodigoCobr"].ToString();
                    usuario.NombApel = rdr["cbrNombApel"].ToString();
                    usuario.NumIdenti = rdr["cbrNumIdenti"].ToString();
                    usuario.Usuario = login.Trim();
                    rdr.Close();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (System.Exception ex)
            {
                return false;
            }
            finally { CONEXIONMAESTRA.Cerrar(); }
        }
        #endregion

        #region COMANDOS
        public ICommand ingresarCommand => new Command(ingresar);
        #endregion
    }
}
