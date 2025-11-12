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

        //public bool InsertarCliente(Mcliente cliente)
        //{
        //    try
        //    {
        //        using (SqlConnection con = CONEXIONMAESTRA.GetConnection())
        //        {
        //            con.Open();

        //            using (SqlCommand cmd = new SqlCommand("GrabaDatosPerCte", con))
        //            {
        //                cmd.CommandType = CommandType.StoredProcedure;
        //                cmd.CommandTimeout = 120;
        //                cmd.Parameters.AddWithValue("@strNumIdeCte", cliente.cteNumIdenti ?? string.Empty);
        //                cmd.Parameters.AddWithValue("@strNomComCte", cliente.cteNombApel ?? string.Empty);
        //                cmd.Parameters.AddWithValue("@strDirResCte", cliente.cteDireccion ?? string.Empty);
        //                cmd.Parameters.AddWithValue("@strDirCobCte", cliente.cteDirCobCte ?? string.Empty);
        //                cmd.Parameters.AddWithValue("@strNumTelFij", cliente.cteTeleFijo ?? string.Empty);
        //                cmd.Parameters.AddWithValue("@strNumTelCel", cliente.cteTeleCelu ?? string.Empty);
        //                cmd.Parameters.AddWithValue("@strCodBarDom", cliente.cteCodBarDom ?? string.Empty);
        //                cmd.Parameters.AddWithValue("@strCodBarCob", cliente.cteCodBarCob ?? string.Empty);
        //                cmd.Parameters.AddWithValue("@strCodigoRut", cliente.cteCodRutReg ?? string.Empty);
        //                cmd.Parameters.AddWithValue("@strNotasCte", cliente.cteNotasGenerales ?? string.Empty);
        //                cmd.Parameters.AddWithValue("@latitud", cliente.latitud ?? "0");
        //                cmd.Parameters.AddWithValue("@longitud", cliente.longitud ?? "0");

        //                if (cliente.Foto != null)
        //                    cmd.Parameters.Add("@Foto", SqlDbType.VarBinary).Value = cliente.Foto;
        //                else
        //                    cmd.Parameters.Add("@Foto", SqlDbType.VarBinary).Value = DBNull.Value;


        //                cmd.ExecuteNonQuery();
        //            }
        //        }

        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        //_ = DisplayAlert("Error", "Error: " + ex.Message, "OK");
        //        Console.WriteLine("Error en InsertarCliente: " + ex.ToString());
        //        throw new Exception("Error en InsertarCliente: " + ex.Message, ex);
        //        //return false;
        //    }
        //}

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

        public async Task<bool> InsertarClienteAsync(Mcliente cliente)
        {
            try
            {
                using (SqlConnection con = CONEXIONMAESTRA.GetConnection())
                {
                    await con.OpenAsync();

                    using (SqlCommand cmd = new SqlCommand("GrabaDatosPerCte", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 120;

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

                        if (cliente.Foto != null)
                        { 
                            //cmd.Parameters.Add("@Foto", SqlDbType.VarBinary, -1).Value = cliente.Foto; // -1 = VARBINARY(MAX)
                            var fotoParam = new SqlParameter("@Foto", SqlDbType.VarBinary, -1);
                            fotoParam.Value = (object)cliente.Foto ?? DBNull.Value;
                            
                            cmd.Parameters.Add(fotoParam);
                        }
                        else
                            cmd.Parameters.Add("@Foto", SqlDbType.VarBinary, -1).Value = DBNull.Value;

                        await cmd.ExecuteNonQueryAsync();
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                // No uses DisplayAlert aquí, maneja en la UI
                Console.WriteLine($"Error InsertarClienteAsync: {ex}");
                
                throw;
            }
        }

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
                                byte[]? fotoBytes = null;
                                try
                                {
                                    int fotoIndex = rdr.GetOrdinal("Foto");
                                    if (!rdr.IsDBNull(fotoIndex))
                                        fotoBytes = (byte[])rdr["Foto"];
                                }
                                catch (IndexOutOfRangeException)
                                {
                                    fotoBytes = null;
                                }

                                byte[]? fotoIDBytes = null;
                                try
                                {
                                    int fotoIDIndex = rdr.GetOrdinal("IDFoto");
                                    if (!rdr.IsDBNull(fotoIDIndex))
                                        fotoIDBytes = (byte[])rdr["IDFoto"];
                                }
                                catch (IndexOutOfRangeException)
                                {
                                    fotoIDBytes = null;
                                }

                                byte[]? fotoLugarBytes = null;
                                try
                                {
                                    int fotoLugarIndex = rdr.GetOrdinal("lugarFoto");
                                    if (!rdr.IsDBNull(fotoLugarIndex))
                                        fotoLugarBytes = (byte[])rdr["lugarFoto"];
                                }
                                catch (IndexOutOfRangeException)
                                {
                                    fotoLugarBytes = null;
                                }

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
                                    cteNotasGenerales = rdr["cteNotasGenerales"].ToString(),
                                    Foto = fotoBytes, 
                                    IDFoto = fotoIDBytes,
                                    FotoLugar = fotoLugarBytes
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

                        if (cliente.Foto != null && cliente.Foto.Length > 0)
                        {
                            var fotoParam = new SqlParameter("@Foto", SqlDbType.VarBinary, -1)
                            {
                                Value = cliente.Foto
                            };
                            cmd.Parameters.Add(fotoParam);
                        }
                        else
                        {
                            cmd.Parameters.Add("@Foto", SqlDbType.VarBinary, -1).Value = DBNull.Value;
                        }
                        
                        if (cliente.IDFoto != null && cliente.IDFoto.Length > 0)
                        {
                            var idFotoParam = new SqlParameter("@IDFoto", SqlDbType.VarBinary, -1)
                            {
                                Value = cliente.IDFoto
                            };
                            cmd.Parameters.Add(idFotoParam);
                        }
                        else
                        {
                            cmd.Parameters.Add("@IDFoto", SqlDbType.VarBinary, -1).Value = DBNull.Value;
                        }

                        if (cliente.FotoLugar != null && cliente.FotoLugar.Length > 0)
                        {
                            var lugarFotoParam = new SqlParameter("@lugarFoto", SqlDbType.VarBinary, -1)
                            {
                                Value = cliente.FotoLugar
                            };
                            cmd.Parameters.Add(lugarFotoParam);
                        }
                        else
                        {
                            cmd.Parameters.Add("@lugarFoto", SqlDbType.VarBinary, -1).Value = DBNull.Value;
                        }

                        cmd.ExecuteNonQuery();
                    }
                }

                return true;
            }
            catch (Exception ex)
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
