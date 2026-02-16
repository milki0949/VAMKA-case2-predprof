using Microsoft.AspNetCore.Mvc;
using Npgsql;
using stolovaya.Models.Requests;

namespace stolovaya.Controllers
{
    [ApiController]
    public class UserController : ControllerBase
    {
        private string _connectionString;

        public UserController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        [HttpPost("/api/administrator/registration")]
        public async Task<IActionResult> CreateAdministrator([FromBody] AdministratorInput model)
        {
            await using (var connection = new NpgsqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                var sqlPostAdmin = "INSERT INTO administrators (full_name, login, user_password) VALUES (@FullName, @Login, @UserPassword)";

                await using (var postAdminCommand = new NpgsqlCommand(sqlPostAdmin, connection))
                {
                    postAdminCommand.Parameters.AddWithValue("FullName", model.FullName);
                    postAdminCommand.Parameters.AddWithValue("Login", model.Login);
                    postAdminCommand.Parameters.AddWithValue("UserPassword", model.UserPassword);

                    await postAdminCommand.ExecuteNonQueryAsync();
                }
            }

            return Ok();
        }

        [HttpPost("/api/student/registration")]
        public async Task<IActionResult> CreateStudent([FromBody] StudentInput model)
        {
            await using (var connection = new NpgsqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                var sqlPostStudent = "INSERT INTO students (full_name, login, user_password, allergy, can_eat_today, got_breakfast, got_lunch, last_date_got_breakfast, last_date_got_lunch, dislike_products, dislike_dishes) VALUES (@FullName, @Login, @UserPassword, '', FALSE, FALSE, FALSE, CURRENT_DATE - 1, CURRENT_DATE - 1, '', '')";

                await using (var postStudentCommand = new NpgsqlCommand(sqlPostStudent, connection))
                {
                    postStudentCommand.Parameters.AddWithValue("FullName", model.FullName);
                    postStudentCommand.Parameters.AddWithValue("Login", model.Login);
                    postStudentCommand.Parameters.AddWithValue("UserPassword", model.UserPassword);

                    await postStudentCommand.ExecuteNonQueryAsync();
                }
            }

            return Ok();
        }

        [HttpPost("/api/cook/registration")]
        public async Task<IActionResult> CreateCook([FromBody] CookInput model)
        {
            await using (var connection = new NpgsqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var sqlPostCook = "INSERT INTO cooks (full_name, login, user_password) VALUES (@FullName, @Login, @UserPassword)";
                await using (var postCookCommand = new NpgsqlCommand(sqlPostCook, connection))
                {
                    postCookCommand.Parameters.AddWithValue("FullName", model.FullName);
                    postCookCommand.Parameters.AddWithValue("Login", model.Login);
                    postCookCommand.Parameters.AddWithValue("UserPassword", model.UserPassword);

                    await postCookCommand.ExecuteNonQueryAsync();
                }
            }

            return Ok();
        }
    }
}
