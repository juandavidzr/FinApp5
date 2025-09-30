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
        //public async void ListarClientes()
        //{
        //    if (CONEXIONMAESTRA.VerificarCon())
        //    {
        //        if (PermisoCreditos())
        //        {
        //            using (UserDialogs.Instance.Loading())
        //            {
        //                await Task.Delay(3000);

        //                if (Usuario != null)
        //                {
        //                    await Navigation.PushAsync(new ListarClientes(Usuario));
        //                }
        //            }

        //        }
        //        else
        //        {
        //            await DisplayAlert("ADVERTENCIA", "No tiene permisos para realizar esta transacción", "OK");
        //        }
        //    }
        //    else
        //    {
        //        if (Usuario != null) // Ensure Usuario is not null  
        //        {
        //            Musuarios usuario = await App.SQLiteDB.GetUsuarioById(Usuario.CodigoCobr);
        //            if (usuario.PermisoAbonar != null)
        //            {
        //                var permiso = usuario.PermisoAbonar;
        //                if (permiso == "1")
        //                    if (Navigation != null)
        //                        await Navigation.PushAsync(new ListarClientes(Usuario));
        //                else
        //                    await DisplayAlert("ADVERTENCIA", "No tiene permisos para realizar esta transacción", "OK");
        //            }
        //        }
        //    }
        //}

        public async void ListarClientes()
        {
            if (CONEXIONMAESTRA.VerificarCon()) // Conexión online
            {
                if (await PermisoCreditosAsync())
                {
                    using (UserDialogs.Instance.Loading())
                    {
                        await Task.Delay(3000);

                        if (Usuario != null)
                        {
                            await Navigation.PushAsync(new ListarClientes(Usuario));
                        }
                        else
                        {
                            await DisplayAlert("ADVERTENCIA", "No se pudo obtener el usuario en línea", "OK");
                        }
                    }
                }
                else
                {
                    await DisplayAlert("ADVERTENCIA", "No tiene permisos para realizar esta transacción", "OK");
                }
            }
            else // Modo offline
            {
                if (Usuario != null) // Verificamos que Usuario tenga algo
                {
                    Musuarios usuario = await App.SQLiteDB.GetUsuarioById(Usuario.CodigoCobr);

                    if (usuario != null) // validamos si encontró el registro en SQLite
                    {
                        if (!string.IsNullOrEmpty(usuario.PermisoAbonar))
                        {
                            var permiso = usuario.PermisoAbonar;

                            if (permiso == "1")
                            {
                                if (Navigation != null)
                                    await Navigation.PushAsync(new ListarClientes(Usuario));
                            }
                            else
                            {
                                await DisplayAlert("ADVERTENCIA", "No tiene permisos para realizar esta transacción", "OK");
                            }
                        }
                        else
                        {
                            await DisplayAlert("ADVERTENCIA", "El campo PermisoAbonar está vacío en la base local", "OK");
                        }
                    }
                    else
                    {
                        await DisplayAlert("ADVERTENCIA", "Usuario no encontrado en la base local", "OK");
                    }
                }
                else
                {
                    await DisplayAlert("ADVERTENCIA", "No hay usuario cargado en memoria", "OK");
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
            if (CONEXIONMAESTRA.VerificarCon())
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
            else
            {
                Musuarios usuario = await App.SQLiteDB.GetUsuarioById(Usuario.CodigoCobr);
                if (usuario.PermisoGastos != null)
                {
                    var permiso = usuario.PermisoGastos;
                    if (permiso == "1")
                        await Navigation.PushAsync(new RegistarGastos(Usuario));
                    else
                        await DisplayAlert("ADVERTENCIA", "No tiene permisos para realizar esta transacción", "OK");
                }
                else
                {
                    await DisplayAlert("ADVERTENCIA", "ERROR", "OK");
                }
            }
        }

        private async void Enrrutar(object obj)
        {
            UserDialogs.Instance.ShowLoading();
            await Task.Delay(1000);
            await Navigation.PushAsync(new EnrrutarCartera(Usuario));
            UserDialogs.Instance.HideHud();
        }

        //private bool PermisoCreditos()
        //{
        //    SqlDataReader rdr;
        //    try
        //    {
        //        if (CONEXIONMAESTRA.VerificarCon())
        //        {

        //            SqlCommand cmd = new SqlCommand();
        //            cmd = new SqlCommand("permisoCreditos", CONEXIONMAESTRA.conectar);
        //            cmd.CommandType = CommandType.StoredProcedure;
        //            cmd.Parameters.AddWithValue("@strCodigoRutaAc", Usuario.CodigoCobr);
        //            CONEXIONMAESTRA.Abrir();

        //            rdr = cmd.ExecuteReader();

        //            if (rdr.Read())
        //            {
        //                var permiso = Convert.ToInt16(rdr["cbrIndAutConCre"].ToString());
        //                if (permiso == 1)
        //                    return true;
        //                else
        //                    return false;
        //            }
        //            else { return false; }
        //        }
        //        else
        //        {
        //            return false;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return false;
        //    }
        //    finally { CONEXIONMAESTRA.Cerrar(); }
        //}

        private async Task<bool> PermisoCreditosAsync()
        {
            try
            {
                using (SqlConnection con = CONEXIONMAESTRA.GetConnection())
                using (SqlCommand cmd = new SqlCommand("permisoCreditos", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@strCodigoRutaAc", Usuario.CodigoCobr);

                    await con.OpenAsync();

                    using (SqlDataReader rdr = await cmd.ExecuteReaderAsync())
                    {
                        if (await rdr.ReadAsync())
                        {
                            var permiso = Convert.ToInt16(rdr["cbrIndAutConCre"]);
                            return permiso == 1;
                        }
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }


        //private bool PermisoCreditos()
        //{
        //    try
        //    {
        //        if (!CONEXIONMAESTRA.VerificarCon())
        //            return false;

        //        using (SqlConnection conn = CONEXIONMAESTRA.GetConnection())
        //        {
        //            conn.Open();

        //            using (SqlCommand cmd = new SqlCommand("permisoCreditos", conn))
        //            {
        //                cmd.CommandType = CommandType.StoredProcedure;
        //                cmd.Parameters.AddWithValue("@strCodigoRutaAc", Usuario.CodigoCobr);

        //                using (SqlDataReader rdr = cmd.ExecuteReader())
        //                {
        //                    if (rdr.Read())
        //                    {
        //                        var permiso = Convert.ToInt16(rdr["cbrIndAutConCre"]);
        //                        return permiso == 1;
        //                    }
        //                }
        //            }
        //        }

        //        return false;
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine($"Error en PermisoCreditos: {ex.Message}");
        //        return false;
        //    }
        //}


        //private bool PermisoCreditos()
        //{
        //    try
        //    {
        //        if (CONEXIONMAESTRA.VerificarCon())
        //        {
        //            using (SqlCommand cmd = new SqlCommand("permisoCreditos", CONEXIONMAESTRA.conectar))
        //            {
        //                cmd.CommandType = CommandType.StoredProcedure;
        //                cmd.Parameters.AddWithValue("@strCodigoRutaAc", Usuario.CodigoCobr);

        //                CONEXIONMAESTRA.Abrir();

        //                using (SqlDataReader rdr = cmd.ExecuteReader())
        //                {
        //                    if (rdr.Read())
        //                    {
        //                        var permiso = Convert.ToInt16(rdr["cbrIndAutConCre"].ToString());
        //                        return permiso == 1;
        //                    }
        //                    else
        //                    {
        //                        return false;
        //                    }
        //                }
        //            }
        //        }
        //        else
        //        {
        //            return false;
        //        }
        //    }
        //    catch (Exception)
        //    {
        //        return false;
        //    }
        //    finally
        //    {
        //        CONEXIONMAESTRA.Cerrar();
        //    }
        //}


        //private bool ValidarPermisos()
        //{
        //    try
        //    {
        //        SqlCommand cmd = new("permisoGastos", CONEXIONMAESTRA.conectar)
        //        {
        //            CommandType = CommandType.StoredProcedure
        //        };
        //        cmd.Parameters.AddWithValue("@strCodigoRutaAc", Usuario.CodigoCobr);
        //        CONEXIONMAESTRA.Abrir();
        //        SqlDataReader rdr = cmd.ExecuteReader();
        //        if (rdr.Read())
        //        {
        //            var permiso = Convert.ToInt16(rdr["cbrFlagPerGraGas"].ToString());
        //            if (permiso == 1)
        //                return true;
        //            else
        //                return false;
        //        }
        //        else { return false; }
        //    }
        //    catch (Exception)
        //    {
        //        return false;
        //    }
        //    finally { CONEXIONMAESTRA.Cerrar(); }
        //}

        private bool ValidarPermisos()
        {
            try
            {
                using (var connection = CONEXIONMAESTRA.GetConnection())
                using (var cmd = new SqlCommand("permisoGastos", connection))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@strCodigoRutaAc", Usuario.CodigoCobr);

                    connection.Open();

                    using (var rdr = cmd.ExecuteReader())
                    {
                        if (rdr.Read())
                        {
                            var permiso = Convert.ToInt16(rdr["cbrFlagPerGraGas"].ToString());
                            return permiso == 1;
                        }
                        else
                        {
                            return false;
                        }
                    }
                }
            }
            catch (Exception)
            {
                return false;
            }
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