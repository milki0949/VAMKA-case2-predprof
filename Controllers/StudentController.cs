using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Npgsql;
using stolovaya.Models.Entities;
using stolovaya.Models.Requests;
using System.Data;
using System.Reflection;
using System.Text;

namespace stolovaya.Controllers
{
    [ApiController]
    public class StudentController : ControllerBase
    {
        private readonly string _connectionString;

        public StudentController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        [HttpGet("api/student/breakfast")]
        public async Task<IActionResult> GetStudentsBreakfast()
        {
            var students = new List<StudentBreakfast>();
            await using (var connection = new NpgsqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                var sqlSelectCommand = $@"SELECT id, full_name, got_breakfast FROM students WHERE can_eat_today = TRUE AND last_date_got_breakfast != CURRENT_DATE";

                await using (var command = new NpgsqlCommand(sqlSelectCommand, connection))
                await using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        students.Add(new StudentBreakfast
                        {
                            Id = reader.GetInt32(0),
                            FullName = reader.GetString(1),
                            GotBreakfast = reader.GetBoolean(2)
                        });
                    }
                }
            }

            return Ok(students);
        }

        [HttpPost("api/student/update/breakfast/{id}")]
        public async Task<IActionResult> UpdateGotBreakfast(int id)
        {
            await using (var connection = new NpgsqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                var sqlUpdateCommand = @"UPDATE students SET last_date_got_breakfast = CURRENT_DATE WHERE id = @Id";

                await using (var Updatecommand = new NpgsqlCommand(sqlUpdateCommand, connection))
                {
                    Updatecommand.Parameters.AddWithValue("Id", id);

                    await Updatecommand.ExecuteNonQueryAsync();
                }
            }

            return Ok();
        }

        [HttpGet("api/student/lunch")]
        public async Task<IActionResult> GetStudentsLunch()
        {
            var students = new List<StudentLunch>();
            await using (var connection = new NpgsqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                var sqlSelectCommand = $@"SELECT id, full_name, got_lunch FROM students WHERE can_eat_today = TRUE AND last_date_got_lunch != CURRENT_DATE";

                await using (var command = new NpgsqlCommand(sqlSelectCommand, connection))
                await using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        students.Add(new StudentLunch
                        {
                            Id = reader.GetInt32(0),
                            FullName = reader.GetString(1),
                            GotLunch = reader.GetBoolean(2)
                        });
                    }
                }
            }

            return Ok(students);
        }

        [HttpPost("api/student/update/lunch/{id}")]
        public async Task<IActionResult> UpdateGotLunch(int id)
        {
            await using (var connection = new NpgsqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                var sqlUpdateCommand = @"UPDATE students SET last_date_got_lunch = CURRENT_DATE WHERE id = @Id";

                await using (var Updatecommand = new NpgsqlCommand(sqlUpdateCommand, connection))
                {
                    Updatecommand.Parameters.AddWithValue("Id", id);

                    await Updatecommand.ExecuteNonQueryAsync();
                }

            }

            return Ok();
        }

        [HttpPost("/api/student/allergy/{id}")]
        public async Task<IActionResult> StudentAllergy([FromBody] List<StudentAllergyInput> products, int id)
        {
            StringBuilder allergyStringBuilder = new StringBuilder();
            string allergyString;
            Student student = null;

            await using (var connection = new NpgsqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                foreach(var product in products)
                    allergyStringBuilder.Append(product.ProductId).Append(" ");

                allergyString = allergyStringBuilder.ToString();

                var sqlNewAllergys = @"UPDATE students SET allergy = @Allergy WHERE id = @Id";

                await using (var newAllergysCommand = new NpgsqlCommand(sqlNewAllergys, connection))
                {
                    newAllergysCommand.Parameters.AddWithValue("Allergy", allergyString);
                    newAllergysCommand.Parameters.AddWithValue("Id", id);

                    newAllergysCommand.ExecuteNonQuery();
                }

                var sqlGetStudent = "SELECT id, full_name, login, allergy, can_eat_today, got_breakfast, got_lunch, last_date_got_breakfast, last_date_got_lunch, dislike_products, dislike_Dishes FROM students WHERE id = @Id";

                await using (var GetStudentCommand = new NpgsqlCommand(sqlGetStudent, connection))
                {
                    GetStudentCommand.Parameters.AddWithValue("Id", id);

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

        [HttpPost("/api/student/dislike/product/{id}")]
        public async Task<IActionResult> StudentDislikeProduct([FromBody] List<StudentAllergyInput> products, int id)
        {
            StringBuilder dislikeProductStringBuilder = new StringBuilder();
            Student student = null;
            string dislikeProduct;

            await using (var connection = new NpgsqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                foreach (var product in products)
                    dislikeProductStringBuilder.Append(product.ProductId).Append(" ");

                dislikeProduct = dislikeProductStringBuilder.ToString();

                var sqlNewDislikeProduct = @"UPDATE students SET dislike_products = @DislikeProduct WHERE id = @Id";

                await using (var newDislikeProductCommand = new NpgsqlCommand(sqlNewDislikeProduct, connection))
                {
                    newDislikeProductCommand.Parameters.AddWithValue("DislikeProduct", dislikeProduct);
                    newDislikeProductCommand.Parameters.AddWithValue("Id", id);

                    newDislikeProductCommand.ExecuteNonQuery();
                }

                var sqlGetStudent = "SELECT id, full_name, login, allergy, can_eat_today, got_breakfast, got_lunch, last_date_got_breakfast, last_date_got_lunch, dislike_products, dislike_Dishes FROM students WHERE id = @Id";

                await using (var GetStudentCommand = new NpgsqlCommand(sqlGetStudent, connection))
                {
                    GetStudentCommand.Parameters.AddWithValue("Id", id);

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

        [HttpPost("/api/student/dislike/dish/{id}")]
        public async Task<IActionResult> StudentDislikeDish([FromBody] List<StudentAllergyInput> products, int id)
        {
            StringBuilder dislikeDishStringBuilder = new StringBuilder();
            string dislikeDish;
            Student student = null;

            await using (var connection = new NpgsqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                foreach (var product in products)
                    dislikeDishStringBuilder.Append(product.ProductId).Append(" ");

                dislikeDish = dislikeDishStringBuilder.ToString();

                var sqlNewDislikeDish = @"UPDATE students SET dislike_Dishes = @DislikeDish WHERE id = @Id";

                await using (var newDislikeDishCommand = new NpgsqlCommand(sqlNewDislikeDish, connection))
                {
                    newDislikeDishCommand.Parameters.AddWithValue("DislikeDish", dislikeDish);
                    newDislikeDishCommand.Parameters.AddWithValue("Id", id);

                    newDislikeDishCommand.ExecuteNonQuery();
                }

                var sqlGetStudent = "SELECT id, full_name, login, allergy, can_eat_today, got_breakfast, got_lunch, last_date_got_breakfast, last_date_got_lunch, dislike_products, dislike_Dishes FROM students WHERE id = @Id";

                await using (var GetStudentCommand = new NpgsqlCommand(sqlGetStudent, connection))
                {
                    GetStudentCommand.Parameters.AddWithValue("Id", id);

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

        [HttpPost("api/student/get/breakfast/{id}")]
        public async Task<IActionResult> GetBreakfast(int id)
        {
            bool gotBreakfast = false;

            await using (var connection = new NpgsqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                var sqlGetBreakfast = @"UPDATE students SET got_breakfast = TRUE WHERE got_breakfast != TRUE AND last_date_got_breakfast = CURRENT_DATE AND id = @Id;";

                await using (var GetBreakfastCommand = new NpgsqlCommand(sqlGetBreakfast, connection))
                {
                    GetBreakfastCommand.Parameters.AddWithValue("Id", id);

                    await GetBreakfastCommand.ExecuteNonQueryAsync();
                }

                var sqlReturnBreakfastStatus = @"SELECT got_breakfast FROM students WHERE id = @Id";

                await using (var ReturnBreakfastStatusCommand = new NpgsqlCommand(sqlReturnBreakfastStatus, connection))
                {
                    ReturnBreakfastStatusCommand.Parameters.AddWithValue("Id", id);

                    await using (var reader = await ReturnBreakfastStatusCommand.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                           gotBreakfast = reader.GetBoolean(0);
                        }
                    }
                }

                var sqlTotalGotBreakfast = @"UPDATE nutrition_report 
                                            SET total_got_breakfast = total_got_breakfast + 1,
                                            total_got_food = total_got_food + 1
                                            WHERE EXISTS (
                                                SELECT 1 FROM students 
                                                WHERE id = @Id AND got_breakfast = TRUE
                                            );";

                await using (var sqlTotalGotBreakfastCommand = new NpgsqlCommand(sqlTotalGotBreakfast, connection))
                {
                    sqlTotalGotBreakfastCommand.Parameters.AddWithValue("Id", id);

                    sqlTotalGotBreakfastCommand.ExecuteNonQuery();
                }
            }

            return Ok(gotBreakfast);
        }

        [HttpPost("api/student/get/lunch/{id}")]
        public async Task<IActionResult> GetLunch(int id)
        {
            bool gotLunch = false;

            await using (var connection = new NpgsqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                var sqlGetLunch = @"UPDATE students SET got_lunch = TRUE WHERE got_lunch != TRUE AND last_date_got_lunch = CURRENT_DATE AND id = @Id;";

                await using (var GetLunchCommand = new NpgsqlCommand(sqlGetLunch, connection))
                {
                    GetLunchCommand.Parameters.AddWithValue("Id", id);

                    await GetLunchCommand.ExecuteNonQueryAsync();
                }

                var sqlReturnLucnhStatus = @"SELECT got_lunch FROM students WHERE id = @Id";

                await using (var ReturnLunchStatusCommand = new NpgsqlCommand(sqlReturnLucnhStatus, connection))
                {
                    ReturnLunchStatusCommand.Parameters.AddWithValue("Id", id);

                    await using (var reader = await ReturnLunchStatusCommand.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            gotLunch = reader.GetBoolean(0);
                        }
                    }
                }

                var sqlTotalGotLunch = @"UPDATE nutrition_report 
                                            SET total_got_lunch = total_got_lunch + 1,
                                            total_got_food = total_got_food + 1
                                            WHERE EXISTS (
                                                SELECT 1 FROM students 
                                                WHERE id = @Id AND got_lunch = TRUE
                                            );";

                await using (var sqlTotalGotLunchCommand = new NpgsqlCommand(sqlTotalGotLunch, connection))
                {
                    sqlTotalGotLunchCommand.Parameters.AddWithValue("Id", id);

                    sqlTotalGotLunchCommand.ExecuteNonQuery();
                }
            }

            return Ok(gotLunch);
        }
    }
}
