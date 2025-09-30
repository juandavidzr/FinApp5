using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;


namespace FinAppApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CreditosController : ControllerBase
    {
        private readonly IConfiguration _config;

        public CreditosController(IConfiguration config)
        {
            _config = config;
        }

        [HttpGet("test")]
        public IActionResult TestConexion()
        {
            string connString = _config.GetConnectionString("SqlServer");

            using (SqlConnection conn = new SqlConnection(connString))
            {
                try
                {
                    conn.Open();
                    return Ok("✅ Conexión exitosa a la base de datos.");
                }
                catch (Exception ex)
                {
                    return BadRequest("❌ Error: " + ex.Message);
                }
            }
        }
    }
}
