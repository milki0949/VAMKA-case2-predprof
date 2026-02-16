using Microsoft.AspNetCore.Mvc;
using Npgsql;
using stolovaya.Models.Entities;
using stolovaya.Models.Requests;
using System.Reflection;
using System.Threading.Tasks;

namespace stolovaya.Controllers
{
    [ApiController]
    public class LoginController : ControllerBase
    {

        private string _connectionString;

        public LoginController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        [HttpPost("/api/student")]
        public async Task<IActionResult> StudentLogin([FromBody] LoginInput model)
        {
            Student student = null;

            await using (var connection = new NpgsqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                var sqlGetStudent = "SELECT id, full_name, login, allergy, can_eat_today, got_breakfast, got_lunch, last_date_got_breakfast, last_date_got_lunch, dislike_products, dislike_Dishes FROM students WHERE login = @login AND user_password = @password";

                await using (var GetStudentCommand = new NpgsqlCommand(sqlGetStudent, connection))
                {
                    GetStudentCommand.Parameters.AddWithValue("login", model.Login);
                    GetStudentCommand.Parameters.AddWithValue("password", model.UserPassword);

                    await using (var reader = await GetStudentCommand.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            student = new Student
                            {
                                Id = reader.GetInt32(0),
                                FullName = reader.GetString(1),
                                Login = reader.GetString(2),
                                Allergy = reader.GetString(3),
                                CanEatToday = reader.GetBoolean(4),
                                GotBreakfast = reader.GetBoolean(5),
                                GotLunch = reader.GetBoolean(6),
                                LastDateGotBreakfast = reader.GetDateTime(7),
                                LastDateGotLunch = reader.GetDateTime(8),
                                DislikeProducts = reader.GetString(9),
                                DislikeDishes = reader.GetString(10)
                            };
                        }
                    }
                }
            }

            if (student == null)
            {
                return Unauthorized(new { message = "Неверный логин или пароль" });
            }

            return Ok(student);
        }

        [HttpPost("/api/administrator")]
        public async Task<IActionResult> AdministratorLogin([FromBody] LoginInput model)
        {
            Administrator administrator = null;

            await using (var connection = new NpgsqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                var sqlGetAdministrator = "SELECT id, full_name, login FROM administrators WHERE login = @login AND user_password = @password";

                await using (var GetAdministratorCommand = new NpgsqlCommand(sqlGetAdministrator, connection))
                {
                    GetAdministratorCommand.Parameters.AddWithValue("login", model.Login);
                    GetAdministratorCommand.Parameters.AddWithValue("password", model.UserPassword);

                    await using (var reader = await GetAdministratorCommand.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            administrator = new Administrator
                            {
                                Id = reader.GetInt32(0),
                                FullName = reader.GetString(1),
                                Login = reader.GetString(2),
                            };
                        }
                    }
                }
            }

            if (administrator == null)
            {
                return Unauthorized(new { message = "Неверный логин или пароль" });
            }

            return Ok(administrator);
        }

        [HttpPost("/api/check")]
        public async Task<IActionResult> CheckDateGotFood([FromBody] LoginInput model)
        {
            Student student = null;

            await using (var connection = new NpgsqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                var sqlCheck = @"
                    UPDATE students
                    SET got_breakfast = FALSE, got_lunch = FALSE
                    WHERE last_date_got_breakfast::date = (CURRENT_DATE - 1) 
                    AND last_date_got_lunch::date = (CURRENT_DATE - 1)";

                await using (var CheckCommand = new NpgsqlCommand(sqlCheck, connection))
                {
                    CheckCommand.ExecuteNonQuery();
                }

                var sqlCheckSubscribtion = @"
                UPDATE students
                SET can_eat_today = TRUE
                WHERE id IN (
                    SELECT student
                    FROM cafeteria_subscription
                    WHERE end_subscription >= CURRENT_DATE);";

                await using (var CheckSubscribtion = new NpgsqlCommand(sqlCheckSubscribtion, connection)) 
                {
                    CheckSubscribtion.ExecuteNonQuery();
                }

                var sqlGetStudent = "SELECT id, full_name, login, allergy, can_eat_today, got_breakfast, got_lunch, last_date_got_breakfast, last_date_got_lunch, dislike_products, dislike_Dishes FROM students WHERE login = @login AND user_password = @password";

                await using (var GetStudentCommand = new NpgsqlCommand(sqlGetStudent, connection))
                {
                    GetStudentCommand.Parameters.AddWithValue("login", model.Login);
                    GetStudentCommand.Parameters.AddWithValue("password", model.UserPassword);

                    await using (var reader = await GetStudentCommand.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            student = new Student
                            {
                                Id = reader.GetInt32(0),
                                FullName = reader.GetString(1),
                                Login = reader.GetString(2),
                                Allergy = reader.GetString(3),
                                CanEatToday = reader.GetBoolean(4),
                                GotBreakfast = reader.GetBoolean(5),
                                GotLunch = reader.GetBoolean(6),
                                LastDateGotBreakfast = reader.GetDateTime(7),
                                LastDateGotLunch = reader.GetDateTime(8),
                                DislikeProducts = reader.GetString(9),
                                DislikeDishes = reader.GetString(10)
                            };
                        }
                    }
                }
            }

            return Ok(student);
        }

        [HttpPost("/api/cook")]
        public async Task<IActionResult> CookLogin([FromBody] LoginInput model)
        {
            Cook cook = null;

            await using (var connection = new NpgsqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                var sqlGetCook = "SELECT id, full_name, login FROM cooks WHERE login = @login AND user_password = @password";

                await using (var GetCookCommand = new NpgsqlCommand(sqlGetCook, connection))
                {
                    GetCookCommand.Parameters.AddWithValue("login", model.Login);
                    GetCookCommand.Parameters.AddWithValue("password", model.UserPassword);

                    await using (var reader = await GetCookCommand.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            cook = new Cook
                            {
                                Id = reader.GetInt32(0),
                                FullName = reader.GetString(1),
                                Login = reader.GetString(2),
                            };
                        }
                    }
                }
            }

            if (cook == null)
            {
                return Unauthorized(new { message = "Неверный логин или пароль" });
            }

            return Ok(cook);
        }
    }
}
