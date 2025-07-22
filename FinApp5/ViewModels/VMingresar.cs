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
        public VMingresar(INavigation? navigation , Musuarios Usuario)
        {
            Navigation = navigation;
            usuario = Usuario;
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
        public async void ingresar()
        {  
            if (String.IsNullOrEmpty(TxtUsuario) || String.IsNullOrEmpty(TxtPw))
            {
                await DisplayAlert("Credenciales incorrectas", "Credenciales incorrectas", "OK");
            }
            else
            {                
                bool Estado = await CONEXIONMAESTRA.VerificarConexionAsync();
                //Estado = true;
                bool aut = false;
                if (Estado)
                {
                    aut = Autenticar(TxtUsuario.Trim(), TxtPw.Trim());                 
                    if (aut)
                    {

                        bool puedeContinuar = await App.SQLiteDB.PuedeContinuarAutenticacionAsync(usuario);
                        if (!puedeContinuar)
                        {                           
                            return; // Si no puede continuar, mostramos un mensaje y salimos del proceso.
                        }
                        await Navigation.PushAsync(new MenuPpal(usuario));
                        
                    }                        
                    else
                        await DisplayAlert("Credenciales incorrectas", "Credenciales incorrectas", "OK");
                }
                else
                {
                    await DisplayAlert("Sin Internet", "Esta trabajando sin conexion (66)", "OK");
                    usuario = await App.SQLiteDB.GetUsuarioByIdandPw(TxtUsuario.Trim(), TxtPw.Trim());
                    if (usuario != null)
                        await Navigation.PushAsync(new MenuPpal(usuario));
                    else
                        await DisplayAlert("Credenciales incorrectas", "Credenciales incorrectas, debe tener conexion al menos la primer vez que inicie la app", "OK");
                }
            }
        }

        private bool Autenticar(string login, string pass)
        {
            try
            { 
                CONEXIONMAESTRA.Abrir();
                SqlCommand cmd =
                    new SqlCommand
                    ("SELECT cbrCodigoCobr, cbrNombApel, cbrNumIdenti, cbrIndicadorPeReAb, cbrFlagPerGraGas " +
                    " FROM tbl_Cobradores WHERE cbrLogAppRut = '" + login + "' and cbrPasUniRut = '" + pass + "'", CONEXIONMAESTRA.conectar);

                SqlDataReader rdr = cmd.ExecuteReader();
                if (rdr.HasRows)
                {                    
                    rdr.Read();
                    usuario ??= new Musuarios();
                    usuario.CodigoCobr = rdr["cbrCodigoCobr"].ToString();
                    usuario.NombApel = rdr["cbrNombApel"].ToString();
                    usuario.NumIdenti = rdr["cbrNumIdenti"].ToString();
                    usuario.PermisoAbonar = rdr["cbrIndicadorPeReAb"].ToString();
                    usuario.PermisoGastos = rdr["cbrFlagPerGraGas"].ToString();
                    usuario.pw = pass.Trim();
                    usuario.Usuario = login.Trim();
                    App.SQLiteDB.DeleteUsuarios<Task>();
                    App.SQLiteDB.saveUsuario(usuario);
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
                Console.WriteLine(ex.Message);
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
