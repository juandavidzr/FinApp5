using FinApp5.Modelo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using FinApp5.Conexiones;
using System.Data;
using FinApp5.ViewModels;


namespace FinApp5.Datos
{
    public class Dclientes : BaseViewModel
    {

        SqlCommand cmd = new SqlCommand();

        public bool InsertarCliente(Mcliente cliente)
        {
            try
            {
                using (SqlConnection con = CONEXIONMAESTRA.GetConnection())
                {
                    con.Open();

                    using (SqlCommand cmd = new SqlCommand("GrabaDatosPerCte", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@strNumIdeCte", cliente.cteNumIdenti ?? string.Empty);
                        cmd.Parameters.AddWithValue("@strNomComCte", cliente.cteNombApel ?? string.Empty);
                        cmd.Parameters.AddWithValue("@strDirResCte", cliente.cteDireccion ?? string.Empty);
                        cmd.Parameters.AddWithValue("@strDirCobCte", cliente.cteDirCobCte ?? string.Empty);
                        cmd.Parameters.AddWithValue("@strNumTelFij", cliente.cteTeleFijo ?? string.Empty);
                        cmd.Parameters.AddWithValue("@strNumTelCel", cliente.cteTeleCelu ?? string.Empty);
                        cmd.Parameters.AddWithValue("@strCodBarDom", cliente.cteCodBarDom ?? string.Empty);
                        cmd.Parameters.AddWithValue("@strCodBarCob", cliente.cteCodBarCob ?? string.Empty);
                        cmd.Parameters.AddWithValue("@strCodigoRut", cliente.cteCodRutReg ?? string.Empty);
                        cmd.Parameters.AddWithValue("@strNotasCte", cliente.cteNotasGenerales ?? string.Empty);
                        cmd.Parameters.AddWithValue("@latitud", cliente.latitud ?? "0");
                        cmd.Parameters.AddWithValue("@longitud", cliente.longitud ?? "0");

                        cmd.ExecuteNonQuery();
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                _ = DisplayAlert("Error", "Error: " + ex.Message, "OK");
                return false;
            }
        }

        //public bool InsertarCliente (Mcliente cliente)
        //{
        //    try
        //    {
        //        CONEXIONMAESTRA.Abrir();
        //        cmd = new SqlCommand ("GrabaDatosPerCte", CONEXIONMAESTRA.conectar);
        //        cmd.CommandType = CommandType.StoredProcedure;
        //        cmd.Parameters.AddWithValue("@strNumIdeCte", cliente.cteNumIdenti);
        //        cmd.Parameters.AddWithValue("@strNomComCte", cliente.cteNombApel);
        //        cmd.Parameters.AddWithValue("@strDirResCte", cliente.cteDireccion);
        //        cmd.Parameters.AddWithValue("@strDirCobCte", cliente.cteDirCobCte);
        //        cmd.Parameters.AddWithValue("@strNumTelFij", cliente.cteTeleFijo);
        //        cmd.Parameters.AddWithValue("@strNumTelCel", cliente.cteTeleCelu);
        //        cmd.Parameters.AddWithValue("@strCodBarDom", cliente.cteCodBarDom);
        //        cmd.Parameters.AddWithValue("@strCodBarCob", cliente.cteCodBarCob);
        //        cmd.Parameters.AddWithValue("@strCodigoRut", cliente.cteCodRutReg);
        //        cmd.Parameters.AddWithValue("@strNotasCte", cliente.cteNotasGenerales);
        //        cmd.Parameters.AddWithValue("@latitud", cliente.latitud);
        //        cmd.Parameters.AddWithValue("@longitud", cliente.longitud);

        //        cmd.ExecuteReader();
        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        _ = DisplayAlert("Error", "Error" + ex.Message, "OK");
        //        return false;
        //    }
        //    finally { CONEXIONMAESTRA.Cerrar(); }
        //}

        //public Mcliente? ConsultarCliente(string txtId)
        //{
        //    try
        //    {
        //        CONEXIONMAESTRA.Abrir();
        //        SqlCommand cmd = new SqlCommand("ObtenerDatosPersonalesDeCliente", CONEXIONMAESTRA.conectar);
        //        cmd.CommandType = CommandType.StoredProcedure;
        //        cmd.Parameters.AddWithValue("@strNumIdeCte", txtId);

        //        SqlDataReader rdr = cmd.ExecuteReader();
        //        if (rdr.Read())
        //        {
        //            Mcliente cliente = new Mcliente();
        //            cliente.cteNombApel = rdr["cteNombApel"].ToString();
        //            cliente.cteDireccion = rdr["cteDireccion"].ToString();
        //            cliente.cteCodBarDom = rdr["cteCodBarDom"].ToString();
        //            cliente.cteCodBarCob = rdr["cteCodBarCob"].ToString();
        //            cliente.cteDirCobCte = rdr["cteDirCobCte"].ToString();
        //            cliente.latitud = rdr["latitud"].ToString();
        //            cliente.longitud = rdr["longitud"].ToString();
        //            cliente.cteTeleCelu = rdr["cteTeleCelu"].ToString();
        //            cliente.cteTeleFijo = rdr["cteTeleFijo"].ToString();
        //            cliente.cteNotasGenerales = rdr["cteNotasGenerales"].ToString();

        //            return cliente;
        //        }
        //        else
        //        {
        //            return null;
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        return null;
        //    }
        //    finally { CONEXIONMAESTRA.Cerrar(); }
        //}

        public Mcliente? ConsultarCliente(string txtId)
        {
            try
            {
                using (SqlConnection conn = CONEXIONMAESTRA.GetConnection())
                {
                    conn.Open();

                    using (SqlCommand cmd = new SqlCommand("ObtenerDatosPersonalesDeCliente", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@strNumIdeCte", txtId);

                        using (SqlDataReader rdr = cmd.ExecuteReader())
                        {
                            if (rdr.Read())
                            {
                                return new Mcliente
                                {
                                    cteNombApel = rdr["cteNombApel"].ToString(),
                                    cteDireccion = rdr["cteDireccion"].ToString(),
                                    cteCodBarDom = rdr["cteCodBarDom"].ToString(),
                                    cteCodBarCob = rdr["cteCodBarCob"].ToString(),
                                    cteDirCobCte = rdr["cteDirCobCte"].ToString(),
                                    latitud = rdr["latitud"].ToString(),
                                    longitud = rdr["longitud"].ToString(),
                                    cteTeleCelu = rdr["cteTeleCelu"].ToString(),
                                    cteTeleFijo = rdr["cteTeleFijo"].ToString(),
                                    cteNotasGenerales = rdr["cteNotasGenerales"].ToString()
                                };
                            }
                        }
                    }
                }

                return null; // si no encontró el cliente
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en ConsultarCliente: {ex.Message}");
                return null;
            }
        }


        //public Mcliente? ConsultarClienteOffLine(string txtId)
        //{
        //    try
        //    {
        //        CONEXIONMAESTRA.Abrir();
        //        SqlCommand cmd = new SqlCommand("ObtenerDatosPersonalesDeCliente", CONEXIONMAESTRA.conectar);
        //        cmd.CommandType = CommandType.StoredProcedure;
        //        cmd.Parameters.AddWithValue("@strNumIdeCte", txtId);

        //        SqlDataReader rdr = cmd.ExecuteReader();
        //        if (rdr.Read())
        //        {
        //            Mcliente cliente = new Mcliente();
        //            cliente.cteNombApel = rdr["cteNombApel"].ToString();
        //            cliente.cteDireccion = rdr["cteDireccion"].ToString();
        //            cliente.cteCodBarDom = rdr["cteCodBarDom"].ToString();
        //            cliente.cteCodBarCob = rdr["cteCodBarCob"].ToString();
        //            cliente.cteDirCobCte = rdr["cteDirCobCte"].ToString();
        //            cliente.latitud = rdr["latitud"].ToString();
        //            cliente.longitud = rdr["longitud"].ToString();
        //            cliente.cteTeleCelu = rdr["cteTeleCelu"].ToString();
        //            cliente.cteTeleFijo = rdr["cteTeleFijo"].ToString();
        //            cliente.cteNotasGenerales = rdr["cteNotasGenerales"].ToString();

        //            return cliente;
        //        }
        //        else
        //        {
        //            return null;
        //        }

        //    }
        //    catch (Exception)
        //    {
        //        return null;
        //    }
        //    finally { CONEXIONMAESTRA.Cerrar(); }
        //}

        public bool ActualizarCliente(Mcliente cliente)
        {
            try
            {
                using (SqlConnection con = CONEXIONMAESTRA.GetConnection())
                {
                    con.Open();

                    using (SqlCommand cmd = new SqlCommand("ActualizarClienteIphone", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@strNumIdeOri", cliente.cteNumIdenti ?? string.Empty);
                        cmd.Parameters.AddWithValue("@strCodigoRut", cliente.cteCodRutReg ?? string.Empty);
                        cmd.Parameters.AddWithValue("@latitud", cliente.latitud ?? "0");
                        cmd.Parameters.AddWithValue("@longitud", cliente.longitud ?? "0");
                        cmd.Parameters.AddWithValue("@notas", cliente.cteNotasGenerales ?? string.Empty);

                        cmd.ExecuteNonQuery();
                    }
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }


        //public bool ActualizarCliente(Mcliente cliente)
        //{
        //    try
        //    {
        //        CONEXIONMAESTRA.Abrir();
        //        SqlCommand cmd = new SqlCommand("ActualizarClienteIphone", CONEXIONMAESTRA.conectar);
        //        cmd.CommandType = CommandType.StoredProcedure;

        //        cmd.Parameters.AddWithValue("@strNumIdeOri", cliente.cteNumIdenti);
        //        //cmd.Parameters.AddWithValue("@strDirResCte", cliente.cteDireccion);
        //        //cmd.Parameters.AddWithValue("@strDirCobCte", cliente.cteDirCobCte);
        //        //cmd.Parameters.AddWithValue("@strNumTelFij", cliente.cteTeleFijo);
        //        //cmd.Parameters.AddWithValue("@strNumTelCel", cliente.cteTeleCelu);
        //        //cmd.Parameters.AddWithValue("@strCodBarDom", cliente.cteCodBarDom);
        //        //cmd.Parameters.AddWithValue("@strCodBarCob", cliente.cteCodBarCob);
        //        cmd.Parameters.AddWithValue("@strCodigoRut", cliente.cteCodRutReg);
        //        cmd.Parameters.AddWithValue("@latitud", cliente.latitud);
        //        cmd.Parameters.AddWithValue("@longitud", cliente.longitud);
        //        cmd.Parameters.AddWithValue("@notas", cliente.cteNotasGenerales);

        //        cmd.ExecuteReader();

        //        return true;
        //    }
        //    catch (Exception )
        //    {
        //        return false;
        //    }
        //    finally { CONEXIONMAESTRA.Cerrar(); }
        //}
    }
}
