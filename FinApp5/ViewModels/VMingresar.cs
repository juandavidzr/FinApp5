using FinApp5.Conexiones;
using FinApp5.Modelo;
using FinApp5.Services;
using FinApp5.Views;
using FinAppMaui.Services;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.DependencyInjection;
using System.Data;
using System.Runtime.Intrinsics.Arm;
using System.Windows.Input;

namespace FinApp5.ViewModels
{
    public class VMingresar : BaseViewModel
    {
        private readonly INavigationService _navigationService;
        private readonly IServiceProvider _serviceProvider;
        private readonly AuthService _authService; // Inyección de AuthService
        private readonly Musuarios _musuario;
        #region VARIABLES
        string _txtUsuario = string.Empty;
        string _txtPw = string.Empty;
        Musuarios usuario = new Musuarios();
        #endregion

        #region CONSTRUCTOR
        //public VMingresar(INavigationService navigationService, Musuarios Usuario, AuthService authService)
        public VMingresar(INavigationService navigationService, AuthService authService)
        {
            //Navigation = navigation;
            _musuario = new Musuarios();
            _navigationService = navigationService;
            //usuario = Usuario;
            _authService = authService ?? throw new ArgumentNullException(nameof(authService));
        }

        public async Task NavegarAAsync(Page pagina)
        {
            await _navigationService.NavigateToAsync(pagina);
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

        /*
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
        */

        public async void ingresar()
        {
            if (String.IsNullOrEmpty(TxtUsuario) || String.IsNullOrEmpty(TxtPw))
            {
                await DisplayAlert("Credenciales incorrectas", "Credenciales incorrectas", "OK");
            }
            else
            {
                try
                {
                    //if (!await CONEXIONMAESTRA.VerificarConexionAsync())
                    if (!CONEXIONMAESTRA.VerificarConexion())
                    {
                        await DisplayAlert("Sin Internet", "Esta trabajando sin conexion (66)", "OK");
                        usuario = await App.SQLiteDB.GetUsuarioByIdandPw(TxtUsuario.Trim(), TxtPw.Trim());
                        if (usuario != null)
                        {
                            await _navigationService.NavigateToAsync(new MenuPpal(usuario));
                            //await Navigation.PushAsync(new MenuPpal(usuario));
                            //await Shell.Current.GoToAsync(nameof(MenuPpal), true, new Dictionary<string, object>
                            //    {
                            //        { "Usuario", usuario }
                            //    });

                        }
                        else
                        {
                            await DisplayAlert("Credenciales incorrectas", "Credenciales incorrectas, debe tener conexion al menos la primer vez que inicie la app", "OK");
                        }
                        return; // Salimos del m��todo si no hay conexi��n
                    }
                    else
                    {
                        var usuario = await _authService.LoginAsync(TxtUsuario.Trim(), TxtPw.Trim());
                        if (usuario != null)
                        {
                            await App.SQLiteDB.DeleteUsuarios<Task>();
                            App.SQLiteDB.saveUsuario(usuario);

                            var usuarios = await App.SQLiteDB.GetUsuarios();
                            //foreach (var u in usuarios)
                            //{
                            //    Console.WriteLine($"Usuario: {u.Usuario}, CodigoCobr: {u.CodigoCobr}, Pw: {u.pw}");
                            //}

                            bool puedeContinuar = await App.SQLiteDB.PuedeContinuarAutenticacionAsync(usuario);
                            if (!puedeContinuar)
                            {
                                return; // Si no puede continuar, mostramos un mensaje y salimos del proceso.
                            }
                            //await Navigation.PushAsync(new MenuPpal(usuario));
                            //await Shell.Current.GoToAsync("//MenuPpal");
                            await _navigationService.NavigateToAsync(new MenuPpal(usuario));
                        }
                        else
                        {
                            await DisplayAlert("Credenciales incorrectas", "Credenciales incorrectas", "OK");
                        }
                    }
                    //bool aut = await _authService.LoginAsync(TxtUsuario.Trim(), TxtPw.Trim());
                }
                catch (System.Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    
                    // 🔹 si falla la API, validamos en SQLite (modo offline)
                    //usuario = await App.SQLiteDB.GetUsuarioByIdandPw(TxtUsuario.Trim(), TxtPw.Trim());
                    //if (usuario != null)
                    //    await Navigation.PushAsync(new MenuPpal(usuario));
                    //else
                    //    await DisplayAlert("Sin Internet", "Debe iniciar sesión online al menos la primera vez", "OK");
                }
            }
        }


        //private bool Autenticar(string login, string pass)
        //{
        //    try
        //    { 
        //        CONEXIONMAESTRA.Abrir();
        //        SqlCommand cmd =
        //            new SqlCommand
        //            ("SELECT cbrCodigoCobr, cbrNombApel, cbrNumIdenti, cbrIndicadorPeReAb, cbrFlagPerGraGas " +
        //            " FROM tbl_Cobradores WHERE cbrLogAppRut = '" + login + "' and cbrPasUniRut = '" + pass + "'", CONEXIONMAESTRA.conectar);

        //        SqlDataReader rdr = cmd.ExecuteReader();
        //        if (rdr.HasRows)
        //        {                    
        //            rdr.Read();
        //            usuario ??= new Musuarios();
        //            usuario.CodigoCobr = rdr["cbrCodigoCobr"].ToString();
        //            usuario.NombApel = rdr["cbrNombApel"].ToString();
        //            usuario.NumIdenti = rdr["cbrNumIdenti"].ToString();
        //            usuario.PermisoAbonar = rdr["cbrIndicadorPeReAb"].ToString();
        //            usuario.PermisoGastos = rdr["cbrFlagPerGraGas"].ToString();
        //            usuario.pw = pass.Trim();
        //            usuario.Usuario = login.Trim();
        //            App.SQLiteDB.DeleteUsuarios<Task>();
        //            App.SQLiteDB.saveUsuario(usuario);
        //            rdr.Close();
        //            return true;
        //        }
        //        else
        //        {
        //            return false;
        //        }
        //    }
        //    catch (System.Exception ex)
        //    {
        //        Console.WriteLine(ex.Message);
        //        return false;
        //    }
        //    finally { CONEXIONMAESTRA.Cerrar(); }
        //}
        #endregion

        #region COMANDOS
        public ICommand ingresarCommand => new Command(ingresar);
        #endregion
    }
}
