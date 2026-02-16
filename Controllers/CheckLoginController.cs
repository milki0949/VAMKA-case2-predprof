using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Npgsql;
using stolovaya.Controllers;

namespace stolovaya.Controllers
{
    [ApiController]
    public class CheckLoginController : ControllerBase
    {
        private string _connectionString;

        public CheckLoginController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        [HttpGet("/api/administrator/login/{login}")]
        public async Task<IActionResult> CheckLoginAdministrator(string login)
        {
            return await CheckLoginInTable("administrators", login);
        }

        [HttpGet("/api/cook/login/{login}")]
        public async Task<IActionResult> CheckLoginCook(string login)
        {
            return await CheckLoginInTable("cooks", login);
        }

        [HttpGet("/api/student/login/{login}")]
        public async Task<IActionResult> CheckLoginStudent(string login)
        {
            return await CheckLoginInTable("students", login);
        }

        private async Task<IActionResult> CheckLoginInTable(string tableName, string login)
        {
            bool isTaken = false;

            await using (var connection = new NpgsqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                var sqlCheckLogin = $@"SELECT COUNT(*) FROM {tableName} WHERE login = @login";

                await using (var checkLoginCommand = new NpgsqlCommand(sqlCheckLogin, connection))
                {
                    checkLoginCommand.Parameters.AddWithValue("login", login);

                    var count = (long)await checkLoginCommand.ExecuteScalarAsync();

                    if (count > 0) 
                        isTaken = true;
                    else
                        isTaken = false;
                }
            }

            return Ok(new { IsTaken = isTaken });
        }
    }
}


