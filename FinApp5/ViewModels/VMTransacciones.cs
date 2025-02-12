using Controls.UserDialogs.Maui;
using FinApp5.Conexiones;
using FinApp5.Modelo;
using FinApp5.Views;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace FinApp5.ViewModels
{
    public class VMTransacciones : BaseViewModel
    {
        #region VARIABLES
        string _Texto;
        Musuarios Usuario;
        
        #endregion
        #region CONSTRUCTOR

        public VMTransacciones(INavigation navigation, Musuarios usuario)
        {
            Navigation = navigation;
            Usuario = usuario;
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
        public async Task ProcesoAsyncrono()
        {

        }
        public async void ListarClientes()
        {
            if (CONEXIONMAESTRA.VerificarCon())
            {
                if (PermisoCreditos())
                {
                    UserDialogs.Instance.Loading();
                    await Task.Delay(3000);
                    await Navigation.PushAsync(new ListarClientes(Usuario));
                    UserDialogs.Instance.HideHud();
                }
                else
                {
                    await DisplayAlert("ADVERTENCIA", "No tiene permisos para realizar esta transacción", "OK");
                }
            }
            else
            {
                DisplayAlert("Sin Internet", "Esta trabajando sin Internet (Linea 115)", "OK");
                Musuarios usuario = await App.SQLiteDB.GetUsuarioById(Usuario.CodigoCobr);
                if (usuario.PermisoAbonar != null)
                {
                    var permiso = usuario.PermisoAbonar;
                    if (permiso == "1")
                        await Navigation.PushAsync(new ListarClientes(Usuario));
                    else
                        await DisplayAlert("ADVERTENCIA", "No tiene permisos para realizar esta transacción", "OK");
                }
            }
        }
        public async void ListarCreditos()
        {
            UserDialogs.Instance.Loading();
            Task.Delay(3000);
            

            await Navigation.PushAsync(new ListaCreditos(Usuario));

            UserDialogs.Instance.HideHud();

        }

        public async void RegistarGastos(object obj)
        {
            if (ValidarPermisos())
            {
                await Navigation.PushAsync(new RegistarGastos(Usuario));
            }
            else
            {
               await DisplayAlert("ADVERTENCIA", "No tiene permisos para realizar esta transacción", "OK");
            }
        }

        private async void Enrrutar(object obj)
        {
            UserDialogs.Instance.ShowLoading();
            await Task.Delay(1000);
            await Navigation.PushAsync(new EnrrutarCartera(Usuario));
            UserDialogs.Instance.HideHud();
        }

        private bool PermisoCreditos()
        {
            try
            {
                if (CONEXIONMAESTRA.VerificarCon())
                {
                    SqlCommand cmd = new SqlCommand("permisoCreditos", CONEXIONMAESTRA.conectar);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@strCodigoRutaAc", Usuario.CodigoCobr);
                    CONEXIONMAESTRA.Abrir();
                    SqlDataReader rdr = cmd.ExecuteReader();
                    if (rdr.Read())
                    {
                        var permiso = Convert.ToInt16(rdr["cbrIndAutConCre"].ToString());
                        if (permiso == 1)
                            return true;
                        else
                            return false;
                    }
                    else { return false; }
                }
                else
                {
                    return false;
                }
            }
            catch (Exception)
            {
                return false;
            }
            finally { CONEXIONMAESTRA.Cerrar(); }
        }

        private bool ValidarPermisos()
        {
            try
            {
                SqlCommand cmd = new SqlCommand("permisoGastos", CONEXIONMAESTRA.conectar);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@strCodigoRutaAc", Usuario.CodigoCobr);
                CONEXIONMAESTRA.Abrir();
                SqlDataReader rdr = cmd.ExecuteReader();
                if (rdr.Read())
                {
                    var permiso = Convert.ToInt16(rdr["cbrFlagPerGraGas"].ToString());
                    if (permiso == 1)
                        return true;
                    else
                        return false;
                }
                else {  return false; }
            }
            catch (Exception)
            {
                return false;
            }
            finally { CONEXIONMAESTRA.Cerrar(); }
        }

        #endregion
        #region COMANDOS
        public ICommand ProcesoAsyncommand => new Command(async () => await ProcesoAsyncrono());
        public ICommand ListarClientesCommand => new Command(ListarClientes);
        public ICommand ListarCreditosCommand => new Command(ListarCreditos);
        public ICommand EnrrutarCommand => new Command(Enrrutar);
        public ICommand RegistarGastosCommand => new Command(RegistarGastos);




        #endregion
    }
}