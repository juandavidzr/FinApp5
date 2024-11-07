using FinApp5.Conexiones;
using FinApp5.Modelo;
using FinApp5.ViewModels;
using Microsoft.Data.SqlClient;
using System.Data;

namespace FinApp5.Datos
{
    public class Dbalance : BaseViewModel
    {
        SqlCommand cmd = new SqlCommand();

        public Mbalance? Fecha()
        {
            try
            {
                Mbalance mbalance = new Mbalance();
                mbalance.Fecha = DateTime.Now.ToString("dd/MM/yyyy");

                return mbalance;
            }
            catch (Exception)
            {

                return null;
            }
        }

        public Mbalance? DebidoRuta(Musuarios usuario)
        {
            try
            {
                Mbalance mbalance = new Mbalance();
                CONEXIONMAESTRA.Abrir();
                SqlCommand cmd = new SqlCommand("DebidoCobrarNew", CONEXIONMAESTRA.conectar);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@strCodigoRuta", usuario.CodigoCobr);

                SqlDataReader rdr = cmd.ExecuteReader();
                if (rdr.Read())
                {
                    mbalance.DebidoRuta = Convert.ToInt32(rdr["DebidoCobrar"]);

                    return mbalance;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                DisplayAlert("Error", "Error" + ex.Message, "OK");
                return null;
            }
            finally { CONEXIONMAESTRA.Cerrar(); }
        }

        public Mbalance? DebidoDia(Musuarios usuario)
        {
            try
            {
                Mbalance mbalance = new Mbalance();
                CONEXIONMAESTRA.Abrir();
                SqlCommand cmd = new SqlCommand("DebidoCobrarDiaNew", CONEXIONMAESTRA.conectar);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@strCodigoRuta", usuario.CodigoCobr);

                SqlDataReader rdr = cmd.ExecuteReader();
                if (rdr.Read())
                {
                    mbalance.DebidoDia = Convert.ToInt32(rdr["DebidoCobrarDia"]);

                    return mbalance;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception )
            {
                //DisplayAlert("Error", "Error" + ex.Message, "OK");
                return null;
            }
            finally { CONEXIONMAESTRA.Cerrar(); }
        }

        public Mbalance? Recaudo(Musuarios usuario)
        {
            try
            {
                Mbalance mbalance = new Mbalance();
                CONEXIONMAESTRA.Abrir();
                SqlCommand cmd = new SqlCommand("Recaudo", CONEXIONMAESTRA.conectar);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@strCodigoRuta", usuario.CodigoCobr);

                SqlDataReader rdr = cmd.ExecuteReader();
                if (rdr.Read())
                {
                    mbalance.Recaudo = Convert.ToInt32(rdr["Recaudo"]);

                    return mbalance;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception)
            {
                //DisplayAlert("Error", "Error" + ex.Message, "OK");
                return null;
            }
            finally { CONEXIONMAESTRA.Cerrar(); }
        }

        public Mbalance? Microseguro(Musuarios usuario)
        {
            try
            {
                Mbalance mbalance = new Mbalance();
                CONEXIONMAESTRA.Abrir();
                SqlCommand cmd = new SqlCommand("Microseguro", CONEXIONMAESTRA.conectar);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@strCodigoRuta", usuario.CodigoCobr);

                SqlDataReader rdr = cmd.ExecuteReader();
                if (rdr.Read())
                {
                    mbalance.Microseguro = Convert.ToInt32(rdr["Microseguro"]);

                    return mbalance;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception)
            {
                //DisplayAlert("Error", "Error" + ex.Message, "OK");
                return null;
            }
            finally { CONEXIONMAESTRA.Cerrar(); }
        }

        public Mbalance? Desembolsos(Musuarios usuario)
        {
            try
            {
                Mbalance mbalance = new Mbalance();
                CONEXIONMAESTRA.Abrir();
                SqlCommand cmd = new SqlCommand("Desembolsos", CONEXIONMAESTRA.conectar);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@strCodigoRuta", usuario.CodigoCobr);

                SqlDataReader rdr = cmd.ExecuteReader();
                if (rdr.Read())
                {
                    mbalance.Desembolsos = Convert.ToInt32(rdr["Desembolsos"]);

                    return mbalance;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception)
            {
                //DisplayAlert("Error", "Error" + ex.Message, "OK");
                return null;
            }
            finally { CONEXIONMAESTRA.Cerrar(); }
        }
        public Mbalance? Gastos(Musuarios usuario)
        {
            try
            {
                Mbalance mbalance = new Mbalance();
                CONEXIONMAESTRA.Abrir();
                SqlCommand cmd = new SqlCommand("Gastos", CONEXIONMAESTRA.conectar);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@strCodigoRuta", usuario.CodigoCobr);

                SqlDataReader rdr = cmd.ExecuteReader();
                if (rdr.Read())
                {
                    mbalance.Gastos = Convert.ToInt32(rdr["Gastos"]);
                    return mbalance;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception)
            {
                //DisplayAlert("Error", "Error" + ex.Message, "OK");
                return null;
            }
            finally { CONEXIONMAESTRA.Cerrar(); }
        }
        public Mbalance? Entradas(Musuarios usuario)
        {
            try
            {
                Mbalance mbalance = new Mbalance();
                CONEXIONMAESTRA.Abrir();
                SqlCommand cmd = new SqlCommand("Entradas", CONEXIONMAESTRA.conectar);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@strCodigoRuta", usuario.CodigoCobr);

                SqlDataReader rdr = cmd.ExecuteReader();
                if (rdr.Read())
                {
                    mbalance.Entradas = Convert.ToInt32(rdr["Entradas"]);

                    return mbalance;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception)
            {
                //DisplayAlert("Error", "Error" + ex.Message, "OK");
                return null;
            }
            finally { CONEXIONMAESTRA.Cerrar(); }
        }

        public Mbalance? Salidas(Musuarios usuario)
        {
            try
            {
                Mbalance mbalance = new Mbalance();
                CONEXIONMAESTRA.Abrir();
                SqlCommand cmd = new SqlCommand("Salidas", CONEXIONMAESTRA.conectar);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@strCodigoRuta", usuario.CodigoCobr);

                SqlDataReader rdr = cmd.ExecuteReader();
                if (rdr.Read())
                {
                    mbalance.Salidas = Convert.ToInt32( rdr["Salidas"]);

                    return mbalance;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception)
            {
                //DisplayAlert("Error", "Error" + ex.Message, "OK");
                return null;
            }
            finally { CONEXIONMAESTRA.Cerrar(); }
        }
        public Mbalance? Sueldos(Musuarios usuario)
        {
            try
            {
                Mbalance mbalance = new Mbalance();
                CONEXIONMAESTRA.Abrir();
                SqlCommand cmd = new SqlCommand("Sueldos", CONEXIONMAESTRA.conectar);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@strCodigoRuta", usuario.CodigoCobr);

                SqlDataReader rdr = cmd.ExecuteReader();
                if (rdr.Read())
                {
                    mbalance.Sueldos = Convert.ToInt32( rdr["Sueldos"]);

                    return mbalance;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception)
            {
                //DisplayAlert("Error", "Error" + ex.Message, "OK");
                return null;
            }
            finally { CONEXIONMAESTRA.Cerrar(); }

        }
        public Mbalance? Creditos(Musuarios usuario)
        {
            try
            {
                Mbalance mbalance = new Mbalance();
                CONEXIONMAESTRA.Abrir();
                SqlCommand cmd = new SqlCommand("CalcularCantidadCreditosEnRuta", CONEXIONMAESTRA.conectar);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@strCodigoRuta", usuario.CodigoCobr);

                SqlDataReader rdr = cmd.ExecuteReader();
                if (rdr.Read())
                {
                    mbalance.Creditos = Convert.ToInt32(rdr["CanCre"]);

                    return mbalance;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception)
            {
                //DisplayAlert("Error", "Error" + ex.Message, "OK");
                return null;
            }
            finally { CONEXIONMAESTRA.Cerrar(); }

        }
        public Mbalance? Visitados(Musuarios usuario)
        {
            try
            {
                Mbalance mbalance = new Mbalance();
                CONEXIONMAESTRA.Abrir();
                SqlCommand cmd = new SqlCommand("CalcularCantidadDeClientesVisitadosHoy", CONEXIONMAESTRA.conectar);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@strCodigoRuta", usuario.CodigoCobr);

                SqlDataReader rdr = cmd.ExecuteReader();
                if (rdr.Read())
                {
                    mbalance.Visitados = Convert.ToInt32(rdr["CAN"]);

                    return mbalance;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception)
            {
                //DisplayAlert("Error", "Error" + ex.Message, "OK");
                return null;
            }
            finally { CONEXIONMAESTRA.Cerrar(); }
        }
        public Mbalance? PrimeraVez(Musuarios usuario)
        {
            try
            {
                Mbalance mbalance = new Mbalance();
                CONEXIONMAESTRA.Abrir();
                SqlCommand cmd = new SqlCommand("creditosNuevos", CONEXIONMAESTRA.conectar);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@strCodigoRuta", usuario.CodigoCobr);

                SqlDataReader rdr = cmd.ExecuteReader();
                if (rdr.Read())
                {
                    mbalance.PrimeraVez = Convert.ToInt32(rdr["CAN"]);

                    return mbalance;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception)
            {
                //DisplayAlert("Error", "Error" + ex.Message, "OK");
                return null;
            }
            finally { CONEXIONMAESTRA.Cerrar(); }
        }
        public Mbalance? Cancelados(Musuarios usuario)
        {
            try
            {
                Mbalance mbalance = new Mbalance();
                CONEXIONMAESTRA.Abrir();
                SqlCommand cmd = new SqlCommand("Cancelados", CONEXIONMAESTRA.conectar);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@strCodigoRuta", usuario.CodigoCobr);

                SqlDataReader rdr = cmd.ExecuteReader();
                if (rdr.Read())
                {
                    mbalance.Cancelados = Convert.ToInt32(rdr["Cancelados"]);

                    return mbalance;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception)
            {
                //DisplayAlert("Error", "Error" + ex.Message, "OK");
                return null;
            }
            finally { CONEXIONMAESTRA.Cerrar(); }
        }
        public Mbalance? CajaAnterior(Musuarios usuario)
        {
            try
            {
                Mbalance mbalance = new Mbalance();
                CONEXIONMAESTRA.Abrir();
                SqlCommand cmd = new SqlCommand("CajaAnterior", CONEXIONMAESTRA.conectar);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@strCodigoRuta", usuario.CodigoCobr);

                SqlDataReader rdr = cmd.ExecuteReader();
                if (rdr.Read())
                {
                    mbalance.CajaAnterior = Convert.ToInt32(rdr["CajaAnterior"]);

                    return mbalance;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception)
            {
                //DisplayAlert("Error", "Error" + ex.Message, "OK");
                return null;
            }
            finally { CONEXIONMAESTRA.Cerrar(); }
        }
    }

}
