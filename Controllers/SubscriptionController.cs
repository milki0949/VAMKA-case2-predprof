using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using stolovaya.Models.Requests;
using Npgsql;

namespace stolovaya.Controllers
{
    [ApiController]
    public class SubscriptionController : ControllerBase
    {
        private readonly string _connectionString;

        public SubscriptionController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        [HttpPost("/api/subscription/weekly/{StudentID}")]
        public async Task<IActionResult> CreateNewWeeklySubscription(int StudentID)
        {
            await using (var connection = new NpgsqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                var sqlCreateNewSubscription = @"INSERT INTO cafeteria_subscription (student, end_subscription) values (@Student, CURRENT_DATE + 7)";

                await using (var CreateNewSubscriptionCommand = new NpgsqlCommand(sqlCreateNewSubscription, connection))
                {
                    CreateNewSubscriptionCommand.Parameters.AddWithValue("Student", StudentID);

                    CreateNewSubscriptionCommand.ExecuteNonQuery();
                }

                var sqlTotalSubscription = @"UPDATE nutrition_report SET total_subscription = total_subscription + 1 WHERE id = 1";

                await using (var sqlTotalSubscriptionCommand = new NpgsqlCommand(sqlTotalSubscription, connection))
                {
                    sqlTotalSubscriptionCommand.ExecuteNonQuery();
                }
            }

            return Ok();
        }

        [HttpPost("/api/subscription/day/{StudentID}")]
        public async Task<IActionResult> CreateNewDaySubscription(int StudentID)
        {
            await using (var connection = new NpgsqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                var sqlCreateNewSubscription = @"INSERT INTO cafeteria_subscription (student, end_subscription) values (@Student, CURRENT_DATE)";

                await using (var CreateNewSubscriptionCommand = new NpgsqlCommand(sqlCreateNewSubscription, connection))
                {
                    CreateNewSubscriptionCommand.Parameters.AddWithValue("Student", StudentID);

                    CreateNewSubscriptionCommand.ExecuteNonQuery();
                }

                var sqlTotalSubscription = @"UPDATE nutrition_report SET total_subscription = total_subscription + 1 WHERE id = 1";

                await using (var sqlTotalSubscriptionCommand = new NpgsqlCommand(sqlTotalSubscription, connection))
                {
                    sqlTotalSubscriptionCommand.ExecuteNonQuery();
                }
            }

            return Ok();
        }
    }
}
