using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Npgsql;
using stolovaya.Models.Entities;
using stolovaya.Models.Requests;

namespace stolovaya.Controllers
{
    [ApiController]
    public class DishController : ControllerBase
    {
        private readonly string _connectionString;

        public DishController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        [HttpPost("api/dish/add")]
        public async Task<IActionResult> DishQuantityAdd([FromBody] AddQuantityDishInput model)
        {
            await using (var connection = new NpgsqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                var sqlAddQuantityDish = @"UPDATE dishes SET quantity = quantity + @Subtract WHERE id = @Id";

                await using (var AddQuantityDishCommand = new NpgsqlCommand(sqlAddQuantityDish, connection))
                {
                    AddQuantityDishCommand.Parameters.AddWithValue("Subtract", model.QuantityToAdd);
                    AddQuantityDishCommand.Parameters.AddWithValue("Id", model.Id);

                    await AddQuantityDishCommand.ExecuteNonQueryAsync();
                }
            }

            return Ok();
        }

        [HttpPost("api/dish/subtract")]
        public async Task<IActionResult> DishQuantitySubtract([FromBody] SubtractDishInput model)
        {
            await using (var connection = new NpgsqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var sqlSubtractQuantityDish = @"UPDATE dishes SET quantity = quantity - @Subtract WHERE id = @Id";
                await using (var SubtractQuantityDishCommand = new NpgsqlCommand(sqlSubtractQuantityDish, connection))
                {
                    SubtractQuantityDishCommand.Parameters.AddWithValue("Subtract", model.QuantityToSubtract);
                    SubtractQuantityDishCommand.Parameters.AddWithValue("Id", model.Id);
                    await SubtractQuantityDishCommand.ExecuteNonQueryAsync();
                }
            }
            return Ok();
        }

        [HttpGet("api/dishes")]
        public async Task<IActionResult> GetDish()
        {
            var dishes = new List<Dish>();
            await using (var connection = new NpgsqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                var sqlSelectCommand = @"SELECT id, dish_name, quantity FROM dishes";

                await using (var command = new NpgsqlCommand(sqlSelectCommand, connection))
                await using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        dishes.Add(new Dish
                        {
                            Id = reader.GetInt32(0),
                            DishName = reader.GetString(1),
                            Quantity = reader.GetInt32(2)
                        });
                    }
                }
            }

            return Ok(dishes);
        }

        [HttpGet("api/dishes/student")]
        public async Task<IActionResult> GetDishForStudent()
        {
            var dishes = new List<DishStudent>();
            await using (var connection = new NpgsqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                var sqlSelectCommand = @"SELECT id, dish_name FROM dishes";

                await using (var command = new NpgsqlCommand(sqlSelectCommand, connection))
                await using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        dishes.Add(new DishStudent
                        {
                            Id = reader.GetInt32(0),
                            DishName = reader.GetString(1)
                        });
                    }
                }
            }

            return Ok(dishes);
        }

        [HttpPost("api/dish/review")]
        public async Task<IActionResult> AddReview([FromBody] AddReviewInput model)
        {
            await using (var connection = new NpgsqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                var sqlInsertCommand = @"INSERT INTO reviews (dish, review) VALUES (@DishId, @ReviewText)";

                await using (var command = new NpgsqlCommand(sqlInsertCommand, connection))
                {
                    command.Parameters.AddWithValue("DishId", model.DishId);
                    command.Parameters.AddWithValue("ReviewText", model.ReviewText);
                    await command.ExecuteNonQueryAsync();
                }
            }
            return Ok();
        }
    }
}
