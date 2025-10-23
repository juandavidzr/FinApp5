//using FinApp5.Modelo;
using FinApp5.Shared;
using FinAppApi.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Data.SqlClient;


namespace FinAppApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClientesController : ControllerBase
    {
        private readonly IConfiguration _config;

        public ClientesController(IConfiguration config)
        {
            _config = config;
        }

        [HttpPost("subir")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> SubirCliente([FromForm] SubirClienteRequest request)
        {
            try
            {
                byte[]? fotoBytes = null;

                if (request.Foto != null)
                {
                    using var ms = new MemoryStream();
                    await request.Foto.CopyToAsync(ms);
                    fotoBytes = ms.ToArray();
                }

                byte[]? fotoIDBytes = null;

                if (request.IDFoto != null)
                {
                    using var ms = new MemoryStream();
                    await request.IDFoto.CopyToAsync(ms);
                    fotoIDBytes = ms.ToArray();
                }

                using var con = new SqlConnection(_config.GetConnectionString("SqlServer"));
                await con.OpenAsync();

                using var cmd = new SqlCommand("GrabaDatosPerCte", con)
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandTimeout = 120
                };

                cmd.Parameters.AddWithValue("@strNumIdeCte", request.cteNumIdenti ?? string.Empty);
                cmd.Parameters.AddWithValue("@strNomComCte", request.cteNombApel ?? string.Empty);
                cmd.Parameters.AddWithValue("@strDirResCte", request.cteDireccion ?? string.Empty);
                cmd.Parameters.AddWithValue("@strDirCobCte", request.cteDirCobCte ?? string.Empty);
                cmd.Parameters.AddWithValue("@strNumTelFij", request.cteTeleFijo ?? string.Empty);
                cmd.Parameters.AddWithValue("@strNumTelCel", request.cteTeleCelu ?? string.Empty);
                cmd.Parameters.AddWithValue("@strCodBarDom", request.cteCodBarDom ?? string.Empty);
                cmd.Parameters.AddWithValue("@strCodBarCob", request.cteCodBarCob ?? string.Empty);
                cmd.Parameters.AddWithValue("@strCodigoRut", request.cteCodRutReg ?? string.Empty);
                cmd.Parameters.AddWithValue("@strNotasCte", request.cteNotasGenerales ?? string.Empty);
                cmd.Parameters.AddWithValue("@latitud", request.latitud ?? "0");
                cmd.Parameters.AddWithValue("@longitud", request.longitud ?? "0");

                if (fotoBytes != null)
                {
                    var fotoParam = new SqlParameter("@Foto", SqlDbType.VarBinary, -1);
                    fotoParam.Value = fotoBytes;
                    cmd.Parameters.Add(fotoParam);
                }
                else
                {
                    cmd.Parameters.Add("@Foto", SqlDbType.VarBinary, -1).Value = DBNull.Value;
                }

                if (fotoIDBytes != null)
                {
                    var fotoIDParam = new SqlParameter("@IDFoto", SqlDbType.VarBinary, -1);
                    fotoIDParam.Value = fotoIDBytes;
                    cmd.Parameters.Add(fotoIDParam);
                }
                else
                {
                    cmd.Parameters.Add("@IDFoto", SqlDbType.VarBinary, -1).Value = DBNull.Value;
                }

                await cmd.ExecuteNonQueryAsync();

                return Ok(new { mensaje = "Cliente guardado correctamente" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPut("actualizar")]
        public async Task<IActionResult> ActualizarCliente([FromForm] ClienteUpdateRequest request)
        {
            try
            {
                Console.WriteLine("🟢 Iniciando actualización de cliente...");

                // Guardar las fotos si existen
                byte[]? fotoBytes = null;
                byte[]? idFotoBytes = null;

                if (request.Foto != null && request.Foto.Length > 0)
                {
                    using var ms = new MemoryStream();
                    await request.Foto.CopyToAsync(ms);
                    fotoBytes = ms.ToArray();
                    Console.WriteLine("📸 Foto recibida correctamente");
                }

                if (request.IDFoto != null && request.IDFoto.Length > 0)
                {
                    using var ms = new MemoryStream();
                    await request.IDFoto.CopyToAsync(ms);
                    idFotoBytes = ms.ToArray();
                    Console.WriteLine("🪪 IDFoto recibida correctamente");
                }

                // Llamar al procedimiento almacenado
                //using (SqlConnection con = new SqlConnection(_connectionString))
                using var con = new SqlConnection(_config.GetConnectionString("SqlServer"));
                {
                    await con.OpenAsync();

                    using (SqlCommand cmd = new SqlCommand("ActualizarClienteIphone", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@strNumIdeOri", request.cteNumIdenti ?? "");
                        cmd.Parameters.AddWithValue("@strCodigoRut", request.cteCodRutReg ?? "");
                        cmd.Parameters.AddWithValue("@latitud", request.latitud ?? "0");
                        cmd.Parameters.AddWithValue("@longitud", request.longitud ?? "0");
                        cmd.Parameters.AddWithValue("@notas", request.cteNotasGenerales ?? "");

                        // Parámetros para las fotos
                        cmd.Parameters.Add("@Foto", SqlDbType.VarBinary).Value = (object?)fotoBytes ?? DBNull.Value;
                        cmd.Parameters.Add("@IDFoto", SqlDbType.VarBinary).Value = (object?)idFotoBytes ?? DBNull.Value;

                        await cmd.ExecuteNonQueryAsync();
                    }
                }

                return Ok(new { mensaje = "Cliente actualizado correctamente" });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error en ActualizarCliente: {ex.Message}");
                return StatusCode(500, new { error = ex.Message });
            }
        }


        //public async Task<IActionResult> SubirCliente([FromForm] SubirClienteRequest request)
        //{
        //    try
        //    {
        //        byte[]? fotoBytes = null;

        //        if (foto != null)
        //        {
        //            using var ms = new MemoryStream();
        //            await foto.CopyToAsync(ms);
        //            fotoBytes = ms.ToArray();
        //        }

        //        using var con = new SqlConnection(_config.GetConnectionString("SqlServer"));
        //        await con.OpenAsync();

        //        using var cmd = new SqlCommand("GrabaDatosPerCte", con)
        //        {
        //            CommandType = CommandType.StoredProcedure,
        //            CommandTimeout = 120
        //        };

        //        cmd.Parameters.AddWithValue("@strNumIdeCte", cliente.cteNumIdenti ?? string.Empty);
        //        cmd.Parameters.AddWithValue("@strNomComCte", cliente.cteNombApel ?? string.Empty);
        //        cmd.Parameters.AddWithValue("@strDirResCte", cliente.cteDireccion ?? string.Empty);
        //        cmd.Parameters.AddWithValue("@strDirCobCte", cliente.cteDirCobCte ?? string.Empty);
        //        cmd.Parameters.AddWithValue("@strNumTelFij", cliente.cteTeleFijo ?? string.Empty);
        //        cmd.Parameters.AddWithValue("@strNumTelCel", cliente.cteTeleCelu ?? string.Empty);
        //        cmd.Parameters.AddWithValue("@strCodBarDom", cliente.cteCodBarDom ?? string.Empty);
        //        cmd.Parameters.AddWithValue("@strCodBarCob", cliente.cteCodBarCob ?? string.Empty);
        //        cmd.Parameters.AddWithValue("@strCodigoRut", cliente.cteCodRutReg ?? string.Empty);
        //        cmd.Parameters.AddWithValue("@strNotasCte", cliente.cteNotasGenerales ?? string.Empty);
        //        cmd.Parameters.AddWithValue("@latitud", cliente.latitud ?? "0");
        //        cmd.Parameters.AddWithValue("@longitud", cliente.longitud ?? "0");

        //        if (cliente.Foto != null)
        //        {
        //            //cmd.Parameters.Add("@Foto", SqlDbType.VarBinary, -1).Value = cliente.Foto; // -1 = VARBINARY(MAX)
        //            var fotoParam = new SqlParameter("@Foto", SqlDbType.VarBinary, -1);
        //            fotoParam.Value = (object)cliente.Foto ?? DBNull.Value;

        //            cmd.Parameters.Add(fotoParam);
        //        }
        //        else
        //            cmd.Parameters.Add("@Foto", SqlDbType.VarBinary, -1).Value = DBNull.Value;

        //        await cmd.ExecuteNonQueryAsync();

        //        return Ok(new { mensaje = "Cliente guardado correctamente" });
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(new { error = ex.Message });
        //    }
        //}
    }
}
