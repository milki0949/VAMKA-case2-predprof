using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using stolovaya.Models.Entities;
using Npgsql;

namespace stolovaya.Controllers
{
    [ApiController]
    public class ReportController : ControllerBase
    {
        private readonly string _connectionString;

        public ReportController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        [HttpGet("/api/report/nutrition")]
        public async Task<IActionResult> GetNutritionReport()
        {
            NutritionReport nutritionReport = null;

            await using (var connection = new NpgsqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                var sqlGetNutritionReport = @"SELECT total_subscription, total_got_breakfast, total_got_lunch, total_got_food FROM nutrition_report WHERE id = 1";

                await using (var GetNutritionReportCommand = new NpgsqlCommand(sqlGetNutritionReport, connection))
                {
                    await using (var reader = await GetNutritionReportCommand.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            nutritionReport = new NutritionReport
                            {
                                TotalSubscription = reader.GetInt32(0),
                                TotalGotBreakfast = reader.GetInt32(1),
                                TotalGotLunch = reader.GetInt32(2),
                                TotalGotFood = reader.GetInt32(3)
                            };
                        }
                    }
                }
            }

            return Ok(nutritionReport);
        }

        [HttpGet("/api/report/cost")]
        public async Task<IActionResult> GetCostReport()
        {
            CostReport costReport = null;
            await using (var connection = new NpgsqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var sqlGetCostReport = @"SELECT total_purchased_product, total_spent FROM cost_report WHERE id = 1";
                await using (var GetCostReportCommand = new NpgsqlCommand(sqlGetCostReport, connection))
                {
                    await using (var reader = await GetCostReportCommand.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            costReport = new CostReport
                            {
                                TotalProduct = reader.GetInt32(0),
                                TotalSpent = reader.GetInt32(1)
                            };
                        }
                    }
                }
            }
            return Ok(costReport);
        }
    }
}
