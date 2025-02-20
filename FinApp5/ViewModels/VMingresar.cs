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
        public async void ingresar()
        {

            if (String.IsNullOrEmpty(TxtUsuario) || String.IsNullOrEmpty(TxtPw))
            {
                await DisplayAlert("Credenciales incorrectas", "Credenciales incorrectas", "OK");
            }
            else
            {
                //bool Estado = CONEXIONMAESTRA.VerificarCon();
                //var Estado = CONEXIONMAESTRA.VerificarConexionAsync();
                bool Estado = await CONEXIONMAESTRA.VerificarConexionAsync();
                bool aut = false;
                if (Estado)
                {
                    aut = Autenticar(TxtUsuario.Trim(), TxtPw.Trim());
                    if (aut)
                        await Navigation.PushAsync(new MenuPpal(usuario));
                    else
                        await DisplayAlert("Credenciales incorrectas", "Credenciales incorrectas", "OK");
                }
                else
                {
                    await DisplayAlert("Sin Internet", "Esta trabajando sin conexion (62)", "OK");
                    usuario = await App.SQLiteDB.GetUsuarioByIdandPw(TxtUsuario.Trim(), TxtPw.Trim());
                    if (usuario != null)
                    {
                        aut = true;
                        await Navigation.PushAsync(new MenuPpal(usuario));
                    }
                    else
                        await DisplayAlert("Credenciales incorrectas", "Credenciales incorrectas", "OK");
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
                    ("SELECT cbrCodigoCobr, cbrNombApel, cbrNumIdenti, cbrIndicadorPeReAb " +
                    " FROM tbl_Cobradores WHERE cbrLogAppRut = '" + login + "' and cbrPasUniRut = '" + pass + "'", CONEXIONMAESTRA.conectar);

                SqlDataReader rdr = cmd.ExecuteReader();
                if (rdr.HasRows)
                {
                    rdr.Read();
                    usuario.CodigoCobr = rdr["cbrCodigoCobr"].ToString();
                    usuario.NombApel = rdr["cbrNombApel"].ToString();
                    usuario.NumIdenti = rdr["cbrNumIdenti"].ToString();
                    usuario.PermisoAbonar = rdr["cbrIndicadorPeReAb"].ToString();
                    usuario.pw = pass.Trim();
                    usuario.Usuario = login.Trim();
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
