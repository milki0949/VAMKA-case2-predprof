using Npgsql;
using stolovaya.Models.Entities;
using Microsoft.AspNetCore.Mvc;

namespace stolovaya.Controllers
{
    [ApiController]
    public class SchoolCodeController : ControllerBase
    {
        private string _connectionString;

        public SchoolCodeController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        [HttpGet("api/administrator/code")] 
        public async Task<IActionResult> GetSchoolAdministratorCode()
        {
            return await GetSchoolCode("administrator_code", "schools");
        }

        [HttpGet("api/cook/code")]
        public async Task<IActionResult> GetSchoolCookCode()
        {
            return await GetSchoolCode("cook_code", "schools");
        }

        [HttpGet("api/student/code")]
        public async Task<IActionResult> GetSchoolStudentCode()
        {
            return await GetSchoolCode("student_code", "schools");
        }

        private async Task<IActionResult> GetSchoolCode(string roleCode, string tableName)
        {
            string schoolCode = null;
            await using (var connection = new NpgsqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var sqlGetSchoolCode = $"SELECT {roleCode} FROM school_code WHERE id = 1";
                await using (var getSchoolCodeCommand = new NpgsqlCommand(sqlGetSchoolCode, connection))
                {
                    var result = await getSchoolCodeCommand.ExecuteScalarAsync();
                    schoolCode = result.ToString();
                }
            }
            return Ok(new { Code = schoolCode });
        }
    }
}
