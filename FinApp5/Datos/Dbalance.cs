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

        public int? EjecutarScalarSP(string storedProcedure, string? parametroRuta, string nombreColumna)
        {
            try
            {
                using (var connection = CONEXIONMAESTRA.GetConnection())
                using (var cmd = new SqlCommand(storedProcedure, connection))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@strCodigoRuta", parametroRuta);

                    connection.Open();

                    using (var rdr = cmd.ExecuteReader())
                    {
                        if (rdr.Read() && rdr[nombreColumna] != DBNull.Value)
                        {
                            return Convert.ToInt32(rdr[nombreColumna]);
                        }
                    }
                }

                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error ejecutando {storedProcedure}: {ex.Message}");
                return null;
            }
        }


        //public Mbalance? DebidoRuta(Musuarios usuario)
        //{
        //    try
        //    {
        //        Mbalance mbalance = new Mbalance();
        //        CONEXIONMAESTRA.Abrir();
        //        SqlCommand cmd = new SqlCommand("DebidoCobrarNew", CONEXIONMAESTRA.conectar);
        //        cmd.CommandType = CommandType.StoredProcedure;
        //        cmd.Parameters.AddWithValue("@strCodigoRuta", usuario.CodigoCobr);

        //        SqlDataReader rdr = cmd.ExecuteReader();
        //        if (rdr.Read())
        //        {
        //            mbalance.DebidoRuta = Convert.ToInt32(rdr["DebidoCobrar"]);

        //            return mbalance;
        //        }
        //        else
        //        {
        //            return null;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        DisplayAlert("Error", "Error" + ex.Message, "OK");
        //        return null;
        //    }
        //    finally { CONEXIONMAESTRA.Cerrar(); }
        //}

        //public Mbalance? DebidoRuta(Musuarios usuario)
        //{
        //    try
        //    {
        //        using (var connection = CONEXIONMAESTRA.GetConnection())
        //        using (var cmd = new SqlCommand("DebidoCobrarNew", connection))
        //        {
        //            cmd.CommandType = CommandType.StoredProcedure;
        //            cmd.Parameters.AddWithValue("@strCodigoRuta", usuario.CodigoCobr);

        //            connection.Open();

        //            using (var rdr = cmd.ExecuteReader())
        //            {
        //                if (rdr.Read())
        //                {
        //                    return new Mbalance
        //                    {
        //                        DebidoRuta = Convert.ToInt32(rdr["DebidoCobrar"])
        //                    };
        //                }
        //            }
        //        }

        //        return null;
        //    }
        //    catch (Exception ex)
        //    {
        //        DisplayAlert("Error", "Error: " + ex.Message, "OK");
        //        return null;
        //    }
        //}


        //public Mbalance? DebidoDia(Musuarios usuario)
        //{
        //    try
        //    {
        //        Mbalance mbalance = new Mbalance();
        //        CONEXIONMAESTRA.Abrir();
        //        SqlCommand cmd = new SqlCommand("DebidoCobrarDiaNew", CONEXIONMAESTRA.conectar);
        //        cmd.CommandType = CommandType.StoredProcedure;
        //        cmd.Parameters.AddWithValue("@strCodigoRuta", usuario.CodigoCobr);

        //        SqlDataReader rdr = cmd.ExecuteReader();
        //        if (rdr.Read())
        //        {
        //            mbalance.DebidoDia = Convert.ToInt32(rdr["DebidoCobrarDia"]);

        //            return mbalance;
        //        }
        //        else
        //        {
        //            return null;
        //        }
        //    }
        //    catch (Exception )
        //    {
        //        //DisplayAlert("Error", "Error" + ex.Message, "OK");
        //        return null;
        //    }
        //    finally { CONEXIONMAESTRA.Cerrar(); }
        //}

        public Mbalance? DebidoDia(Musuarios usuario)
        {
            var valor = EjecutarScalarSP("DebidoCobrarDiaNew", usuario.CodigoCobr, "DebidoCobrarDia");
            return valor.HasValue ? new Mbalance { DebidoDia = valor.Value } : null;
        }

        public Mbalance? DebidoRuta(Musuarios usuario)
        {
            var valor = EjecutarScalarSP("DebidoCobrarNew", usuario.CodigoCobr, "DebidoCobrar");
            return valor.HasValue ? new Mbalance { DebidoRuta = valor.Value } : null;
        }

        //public Mbalance? DebidoDia(Musuarios usuario)
        //{
        //    try
        //    {
        //        using (var connection = CONEXIONMAESTRA.GetConnection())
        //        using (var cmd = new SqlCommand("DebidoCobrarDiaNew", connection))
        //        {
        //            cmd.CommandType = CommandType.StoredProcedure;
        //            cmd.Parameters.AddWithValue("@strCodigoRuta", usuario.CodigoCobr);

        //            connection.Open();

        //            using (var rdr = cmd.ExecuteReader())
        //            {
        //                if (rdr.Read())
        //                {
        //                    return new Mbalance
        //                    {
        //                        DebidoDia = Convert.ToInt32(rdr["DebidoCobrarDia"])
        //                    };
        //                }
        //            }
        //        }

        //        return null;
        //    }
        //    catch (Exception)
        //    {
        //        // Puedes loguear aquí el error si lo necesitas
        //        return null;
        //    }
        //}

        public Mbalance? Recaudo(Musuarios usuario)
        {
            var valor = EjecutarScalarSP("Recaudo", usuario.CodigoCobr, "Recaudo");
            return valor.HasValue ? new Mbalance { Recaudo = valor.Value } : null;
        }

        //public Mbalance? Recaudo(Musuarios usuario)
        //{
        //    try
        //    {
        //        Mbalance mbalance = new Mbalance();
        //        CONEXIONMAESTRA.Abrir();
        //        SqlCommand cmd = new SqlCommand("Recaudo", CONEXIONMAESTRA.conectar);
        //        cmd.CommandType = CommandType.StoredProcedure;
        //        cmd.Parameters.AddWithValue("@strCodigoRuta", usuario.CodigoCobr);

        //        SqlDataReader rdr = cmd.ExecuteReader();
        //        if (rdr.Read())
        //        {
        //            mbalance.Recaudo = Convert.ToInt32(rdr["Recaudo"]);

        //            return mbalance;
        //        }
        //        else
        //        {
        //            return null;
        //        }
        //    }
        //    catch (Exception)
        //    {
        //        //DisplayAlert("Error", "Error" + ex.Message, "OK");
        //        return null;
        //    }
        //    finally { CONEXIONMAESTRA.Cerrar(); }
        //}


        public Mbalance? Microseguro(Musuarios usuario)
        {
            var valor = EjecutarScalarSP("Microseguro", usuario.CodigoCobr, "Microseguro");
            return valor.HasValue ? new Mbalance { Microseguro = valor.Value } : null;
        }

        //public Mbalance? Microseguro(Musuarios usuario)
        //{
        //    try
        //    {
        //        Mbalance mbalance = new Mbalance();
        //        CONEXIONMAESTRA.Abrir();
        //        SqlCommand cmd = new SqlCommand("Microseguro", CONEXIONMAESTRA.conectar);
        //        cmd.CommandType = CommandType.StoredProcedure;
        //        cmd.Parameters.AddWithValue("@strCodigoRuta", usuario.CodigoCobr);

        //        SqlDataReader rdr = cmd.ExecuteReader();
        //        if (rdr.Read())
        //        {
        //            mbalance.Microseguro = Convert.ToInt32(rdr["Microseguro"]);

        //            return mbalance;
        //        }
        //        else
        //        {
        //            return null;
        //        }
        //    }
        //    catch (Exception)
        //    {
        //        //DisplayAlert("Error", "Error" + ex.Message, "OK");
        //        return null;
        //    }
        //    finally { CONEXIONMAESTRA.Cerrar(); }
        //}

        public Mbalance? Desembolsos(Musuarios usuario)
        {
            var valor = EjecutarScalarSP("Desembolsos", usuario.CodigoCobr, "Desembolsos");
            return valor.HasValue ? new Mbalance { Desembolsos = valor.Value } : null;
        }

        //public Mbalance? Desembolsos(Musuarios usuario)
        //{
        //    try
        //    {
        //        Mbalance mbalance = new Mbalance();
        //        CONEXIONMAESTRA.Abrir();
        //        SqlCommand cmd = new SqlCommand("Desembolsos", CONEXIONMAESTRA.conectar);
        //        cmd.CommandType = CommandType.StoredProcedure;
        //        cmd.Parameters.AddWithValue("@strCodigoRuta", usuario.CodigoCobr);

        //        SqlDataReader rdr = cmd.ExecuteReader();
        //        if (rdr.Read())
        //        {
        //            mbalance.Desembolsos = Convert.ToInt32(rdr["Desembolsos"]);

        //            return mbalance;
        //        }
        //        else
        //        {
        //            return null;
        //        }
        //    }
        //    catch (Exception)
        //    {
        //        //DisplayAlert("Error", "Error" + ex.Message, "OK");
        //        return null;
        //    }
        //    finally { CONEXIONMAESTRA.Cerrar(); }
        //}

        public Mbalance? Gastos(Musuarios usuario)
        {
            var valor = EjecutarScalarSP("Gastos", usuario.CodigoCobr, "Gastos");
            return valor.HasValue ? new Mbalance { Gastos = valor.Value } : null;
        }

        //public Mbalance? Gastos(Musuarios usuario)
        //{
        //    try
        //    {
        //        Mbalance mbalance = new Mbalance();
        //        CONEXIONMAESTRA.Abrir();
        //        SqlCommand cmd = new SqlCommand("Gastos", CONEXIONMAESTRA.conectar);
        //        cmd.CommandType = CommandType.StoredProcedure;
        //        cmd.Parameters.AddWithValue("@strCodigoRuta", usuario.CodigoCobr);

        //        SqlDataReader rdr = cmd.ExecuteReader();
        //        if (rdr.Read())
        //        {
        //            mbalance.Gastos = Convert.ToInt32(rdr["Gastos"]);
        //            return mbalance;
        //        }
        //        else
        //        {
        //            return null;
        //        }
        //    }
        //    catch (Exception)
        //    {
        //        //DisplayAlert("Error", "Error" + ex.Message, "OK");
        //        return null;
        //    }
        //    finally { CONEXIONMAESTRA.Cerrar(); }
        //}

        public Mbalance? Entradas(Musuarios usuario)
        {
            var valor = EjecutarScalarSP("Entradas", usuario.CodigoCobr, "Entradas");
            return valor.HasValue ? new Mbalance { Entradas = valor.Value } : null;
        }

        //public Mbalance? Entradas(Musuarios usuario)
        //{
        //    try
        //    {
        //        Mbalance mbalance = new Mbalance();
        //        CONEXIONMAESTRA.Abrir();
        //        SqlCommand cmd = new SqlCommand("Entradas", CONEXIONMAESTRA.conectar);
        //        cmd.CommandType = CommandType.StoredProcedure;
        //        cmd.Parameters.AddWithValue("@strCodigoRuta", usuario.CodigoCobr);

        //        SqlDataReader rdr = cmd.ExecuteReader();
        //        if (rdr.Read())
        //        {
        //            mbalance.Entradas = Convert.ToInt32(rdr["Entradas"]);

        //            return mbalance;
        //        }
        //        else
        //        {
        //            return null;
        //        }
        //    }
        //    catch (Exception)
        //    {
        //        //DisplayAlert("Error", "Error" + ex.Message, "OK");
        //        return null;
        //    }
        //    finally { CONEXIONMAESTRA.Cerrar(); }
        //}

        public Mbalance? Salidas(Musuarios usuario)
        {
            var valor = EjecutarScalarSP("Salidas", usuario.CodigoCobr, "Salidas");
            return valor.HasValue ? new Mbalance { Salidas = valor.Value } : null;
        }

        //public Mbalance? Salidas(Musuarios usuario)
        //{
        //    try
        //    {
        //        Mbalance mbalance = new Mbalance();
        //        CONEXIONMAESTRA.Abrir();
        //        SqlCommand cmd = new SqlCommand("Salidas", CONEXIONMAESTRA.conectar);
        //        cmd.CommandType = CommandType.StoredProcedure;
        //        cmd.Parameters.AddWithValue("@strCodigoRuta", usuario.CodigoCobr);

        //        SqlDataReader rdr = cmd.ExecuteReader();
        //        if (rdr.Read())
        //        {
        //            mbalance.Salidas = Convert.ToInt32( rdr["Salidas"]);

        //            return mbalance;
        //        }
        //        else
        //        {
        //            return null;
        //        }
        //    }
        //    catch (Exception)
        //    {
        //        //DisplayAlert("Error", "Error" + ex.Message, "OK");
        //        return null;
        //    }
        //    finally { CONEXIONMAESTRA.Cerrar(); }
        //}

        public Mbalance? Sueldos(Musuarios usuario)
        {
            var valor = EjecutarScalarSP("Sueldos", usuario.CodigoCobr, "Sueldos");
            return valor.HasValue ? new Mbalance { Sueldos = valor.Value } : null;
        }

        //public Mbalance? Sueldos(Musuarios usuario)
        //{
        //    try
        //    {
        //        Mbalance mbalance = new Mbalance();
        //        CONEXIONMAESTRA.Abrir();
        //        SqlCommand cmd = new SqlCommand("Sueldos", CONEXIONMAESTRA.conectar);
        //        cmd.CommandType = CommandType.StoredProcedure;
        //        cmd.Parameters.AddWithValue("@strCodigoRuta", usuario.CodigoCobr);

        //        SqlDataReader rdr = cmd.ExecuteReader();
        //        if (rdr.Read())
        //        {
        //            mbalance.Sueldos = Convert.ToInt32( rdr["Sueldos"]);

        //            return mbalance;
        //        }
        //        else
        //        {
        //            return null;
        //        }
        //    }
        //    catch (Exception)
        //    {
        //        //DisplayAlert("Error", "Error" + ex.Message, "OK");
        //        return null;
        //    }
        //    finally { CONEXIONMAESTRA.Cerrar(); }

        //}

        public Mbalance? Creditos(Musuarios usuario)
        {
            var valor = EjecutarScalarSP("CalcularCantidadCreditosEnRuta", usuario.CodigoCobr, "CanCre");
            return valor.HasValue ? new Mbalance { Creditos = valor.Value } : null;
        }

        //public Mbalance? Creditos(Musuarios usuario)
        //{
        //    try
        //    {
        //        Mbalance mbalance = new Mbalance();
        //        CONEXIONMAESTRA.Abrir();
        //        SqlCommand cmd = new SqlCommand("CalcularCantidadCreditosEnRuta", CONEXIONMAESTRA.conectar);
        //        cmd.CommandType = CommandType.StoredProcedure;
        //        cmd.Parameters.AddWithValue("@strCodigoRuta", usuario.CodigoCobr);

        //        SqlDataReader rdr = cmd.ExecuteReader();
        //        if (rdr.Read())
        //        {
        //            mbalance.Creditos = Convert.ToInt32(rdr["CanCre"]);

        //            return mbalance;
        //        }
        //        else
        //        {
        //            return null;
        //        }
        //    }
        //    catch (Exception)
        //    {
        //        //DisplayAlert("Error", "Error" + ex.Message, "OK");
        //        return null;
        //    }
        //    finally { CONEXIONMAESTRA.Cerrar(); }

        //}

        public Mbalance? Visitados(Musuarios usuario)
        {
            var valor = EjecutarScalarSP("CalcularCantidadDeClientesVisitadosHoy", usuario.CodigoCobr, "CAN");
            return valor.HasValue ? new Mbalance { Visitados = valor.Value } : null;
        }

        //public Mbalance? Visitados(Musuarios usuario)
        //{
        //    try
        //    {
        //        Mbalance mbalance = new Mbalance();
        //        CONEXIONMAESTRA.Abrir();
        //        SqlCommand cmd = new SqlCommand("CalcularCantidadDeClientesVisitadosHoy", CONEXIONMAESTRA.conectar);
        //        cmd.CommandType = CommandType.StoredProcedure;
        //        cmd.Parameters.AddWithValue("@strCodigoRuta", usuario.CodigoCobr);

        //        SqlDataReader rdr = cmd.ExecuteReader();
        //        if (rdr.Read())
        //        {
        //            mbalance.Visitados = Convert.ToInt32(rdr["CAN"]);

        //            return mbalance;
        //        }
        //        else
        //        {
        //            return null;
        //        }
        //    }
        //    catch (Exception)
        //    {
        //        //DisplayAlert("Error", "Error" + ex.Message, "OK");
        //        return null;
        //    }
        //    finally { CONEXIONMAESTRA.Cerrar(); }
        //}

        public Mbalance? PrimeraVez(Musuarios usuario)
        {
            var valor = EjecutarScalarSP("creditosNuevos", usuario.CodigoCobr, "CAN");
            return valor.HasValue ? new Mbalance { PrimeraVez = valor.Value } : null;
        }

        //public Mbalance? PrimeraVez(Musuarios usuario)
        //{
        //    try
        //    {
        //        Mbalance mbalance = new Mbalance();
        //        CONEXIONMAESTRA.Abrir();
        //        SqlCommand cmd = new SqlCommand("creditosNuevos", CONEXIONMAESTRA.conectar);
        //        cmd.CommandType = CommandType.StoredProcedure;
        //        cmd.Parameters.AddWithValue("@strCodigoRuta", usuario.CodigoCobr);

        //        SqlDataReader rdr = cmd.ExecuteReader();
        //        if (rdr.Read())
        //        {
        //            mbalance.PrimeraVez = Convert.ToInt32(rdr["CAN"]);

        //            return mbalance;
        //        }
        //        else
        //        {
        //            return null;
        //        }
        //    }
        //    catch (Exception)
        //    {
        //        //DisplayAlert("Error", "Error" + ex.Message, "OK");
        //        return null;
        //    }
        //    finally { CONEXIONMAESTRA.Cerrar(); }
        //}

        public Mbalance? Cancelados(Musuarios usuario)
        {
            var valor = EjecutarScalarSP("Cancelados", usuario.CodigoCobr, "Cancelados");
            return valor.HasValue ? new Mbalance { Cancelados = valor.Value } : null;
        }

        //public Mbalance? Cancelados(Musuarios usuario)
        //{
        //    try
        //    {
        //        Mbalance mbalance = new Mbalance();
        //        CONEXIONMAESTRA.Abrir();
        //        SqlCommand cmd = new SqlCommand("Cancelados", CONEXIONMAESTRA.conectar);
        //        cmd.CommandType = CommandType.StoredProcedure;
        //        cmd.Parameters.AddWithValue("@strCodigoRuta", usuario.CodigoCobr);

        //        SqlDataReader rdr = cmd.ExecuteReader();
        //        if (rdr.Read())
        //        {
        //            mbalance.Cancelados = Convert.ToInt32(rdr["Cancelados"]);

        //            return mbalance;
        //        }
        //        else
        //        {
        //            return null;
        //        }
        //    }
        //    catch (Exception)
        //    {
        //        //DisplayAlert("Error", "Error" + ex.Message, "OK");
        //        return null;
        //    }
        //    finally { CONEXIONMAESTRA.Cerrar(); }
        //}

        public Mbalance? CajaAnterior(Musuarios usuario)
        {
            var valor = EjecutarScalarSP("CajaAnterior", usuario.CodigoCobr, "CajaAnterior");
            return valor.HasValue ? new Mbalance { CajaAnterior = valor.Value } : null;
        }

        //public Mbalance? CajaAnterior(Musuarios usuario)
        //{
        //    try
        //    {
        //        Mbalance mbalance = new Mbalance();
        //        CONEXIONMAESTRA.Abrir();
        //        SqlCommand cmd = new SqlCommand("CajaAnterior", CONEXIONMAESTRA.conectar);
        //        cmd.CommandType = CommandType.StoredProcedure;
        //        cmd.Parameters.AddWithValue("@strCodigoRuta", usuario.CodigoCobr);

        //        SqlDataReader rdr = cmd.ExecuteReader();
        //        if (rdr.Read())
        //        {
        //            mbalance.CajaAnterior = Convert.ToInt32(rdr["CajaAnterior"]);

        //            return mbalance;
        //        }
        //        else
        //        {
        //            return null;
        //        }
        //    }
        //    catch (Exception)
        //    {
        //        //DisplayAlert("Error", "Error" + ex.Message, "OK");
        //        return null;
        //    }
        //    finally { CONEXIONMAESTRA.Cerrar(); }
        //}
    }

}
