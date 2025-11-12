using Microsoft.AspNetCore.Mvc;

using System.Threading.Tasks;
using Microsoft.Data.SqlClient;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IConfiguration _config;

    public AuthController(IConfiguration config)
    {
        _config = config;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        string connString = _config.GetConnectionString("SqlServer");
        
        using (var conn = new SqlConnection(connString))
            
        {
            await conn.OpenAsync();

            string sql = "SELECT TOP 1 cbrNumIdenti, cbrPasUniRut, cbrCodigoCobr, cbrNombApel, cbrIndicadorPeReAb, cbrFlagPerGraGas FROM tbl_Cobradores WHERE cbrLogAppRut = @user AND cbrPasUniRut = @pass and cbrIndicadorAct=1";
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@user", request.Usuario);
                cmd.Parameters.AddWithValue("@pass", request.Password);

                //Console.WriteLine($"Usuario: {request.Usuario}, Password: {request.Password}");

                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        var user = new 
                        {
                            CodigoCobr = reader["cbrCodigoCobr"].ToString(),
                            NombApel = reader["cbrNombApel"].ToString(),
                            PermisoAbonar = reader["cbrIndicadorPeReAb"].ToString(),
                            PermisoGastos = reader["cbrFlagPerGraGas"].ToString(),
                            cbrPasUniRut = reader["cbrPasUniRut"].ToString(),
                            cbrNumIdenti = reader["cbrNumIdenti"].ToString(),
                        };
                        return Ok(new
                        {
                            Success = true,
                            Usuario = request.Usuario,
                            codigoCobr = user.CodigoCobr,
                            nombApel = user.NombApel,
                            permisoAbonar = user.PermisoAbonar,
                            permisoGastos = user.PermisoGastos,
                            pw = user.cbrPasUniRut,
                            NumIdenti = user.cbrNumIdenti
                        });
                    }
                }
            }
        }

        return Unauthorized(new { Success = false, Message = "Credenciales incorrectas" });
    }
}

public class LoginRequest
{
    public string Usuario { get; set; }
    public string Password { get; set; }
}
