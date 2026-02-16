using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Npgsql;
using stolovaya.Models.Entities;

namespace stolovaya.Controllers
{
    [ApiController]
    public class MenuController : ControllerBase
    {
        private string _connectionString;

        public MenuController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        [HttpGet("/api/menu/breakfast")]
        public async Task<IActionResult> GetBreakfastMenu()
        {
            var resultMenu = new List<Menu>();

            var days = new[] { "monday", "tuesday", "wednesday", "thursday", "friday" };

            await using (var conn = new NpgsqlConnection(_connectionString))
            {
                await conn.OpenAsync();

                var sqlSchedule = "SELECT monday, tuesday, wednesday, thursday, friday FROM menu_breakfast WHERE id = 2";

                var dishesIdsPerDay = new Dictionary<string, List<int>>();

                await using (var cmd = new NpgsqlCommand(sqlSchedule, conn))
                await using (var reader = await cmd.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        for (int i = 0; i < days.Length; i++)
                        {
                            string idsString = reader.IsDBNull(i) ? "" : reader.GetString(i);

                            var ids = idsString.Split(' ', StringSplitOptions.RemoveEmptyEntries)
                                               .Select(int.Parse)
                                               .ToList();

                            dishesIdsPerDay[days[i]] = ids;
                        }
                    }
                }

                foreach (var day in days)
                {
                    var dayResponse = new Menu { Day = day };
                    var dishIds = dishesIdsPerDay[day];

                    if (dishIds.Count == 0)
                    {
                        resultMenu.Add(dayResponse);
                        continue;
                    }

                    var idsForSql = string.Join(",", dishIds);

                    var sqlDishes = $@"SELECT id, dish_name, kilocalories, compound 
                               FROM dishes 
                               WHERE id IN ({idsForSql})";

                    await using (var cmdDish = new NpgsqlCommand(sqlDishes, conn))
                    await using (var readerDish = await cmdDish.ExecuteReaderAsync())
                    {
                        while (await readerDish.ReadAsync())
                        {
                            var dish = new MenuDish
                            {
                                Id = readerDish.GetInt32(0),
                                Name = readerDish.GetString(1),
                                Calories = readerDish.GetInt32(2)
                            };

                            string compoundStr = readerDish.IsDBNull(3) ? "" : readerDish.GetString(3);

                            dish.ProductIds = compoundStr.Split(' ', StringSplitOptions.RemoveEmptyEntries)
                                                         .Select(int.Parse)
                                                         .ToList();

                            dayResponse.Dishes.Add(dish);
                        }
                    }

                    foreach (var dish in dayResponse.Dishes)
                    {
                        if (dish.ProductIds.Count == 0) continue;

                        var prodIdsSql = string.Join(",", dish.ProductIds);

                        var sqlProducts = $@"SELECT product_name 
                                     FROM products 
                                     WHERE id IN ({prodIdsSql})";

                        await using (var cmdProd = new NpgsqlCommand(sqlProducts, conn))
                        await using (var readerProd = await cmdProd.ExecuteReaderAsync())
                        {
                            while (await readerProd.ReadAsync())
                            {
                                dish.ProductNames.Add(readerProd.GetString(0));
                            }
                        }
                    }

                    resultMenu.Add(dayResponse);
                }
            }

            return Ok(resultMenu);
        }

        [HttpGet("/api/menu/lunch")]
        public async Task<IActionResult> GetLunchMenu()
        {
            var resultMenu = new List<Menu>();

            var days = new[] { "monday", "tuesday", "wednesday", "thursday", "friday" };

            await using (var conn = new NpgsqlConnection(_connectionString))
            {
                await conn.OpenAsync();

                var sqlSchedule = "SELECT monday, tuesday, wednesday, thursday, friday FROM menu_lunch WHERE id = 1";

                var dishesIdsPerDay = new Dictionary<string, List<int>>();

                await using (var cmd = new NpgsqlCommand(sqlSchedule, conn))
                await using (var reader = await cmd.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        for (int i = 0; i < days.Length; i++)
                        {
                            string idsString = reader.IsDBNull(i) ? "" : reader.GetString(i);

                            var ids = idsString.Split(' ', StringSplitOptions.RemoveEmptyEntries)
                                               .Select(int.Parse)
                                               .ToList();

                            dishesIdsPerDay[days[i]] = ids;
                        }
                    }
                }

                foreach (var day in days)
                {
                    var dayResponse = new Menu { Day = day };
                    var dishIds = dishesIdsPerDay[day];

                    if (dishIds.Count == 0)
                    {
                        resultMenu.Add(dayResponse);
                        continue;
                    }

                    var idsForSql = string.Join(",", dishIds);

                    var sqlDishes = $@"SELECT id, dish_name, kilocalories, compound 
                               FROM dishes 
                               WHERE id IN ({idsForSql})";

                    await using (var cmdDish = new NpgsqlCommand(sqlDishes, conn))
                    await using (var readerDish = await cmdDish.ExecuteReaderAsync())
                    {
                        while (await readerDish.ReadAsync())
                        {
                            var dish = new MenuDish
                            {
                                Id = readerDish.GetInt32(0),
                                Name = readerDish.GetString(1),
                                Calories = readerDish.GetInt32(2)
                            };

                            string compoundStr = readerDish.IsDBNull(3) ? "" : readerDish.GetString(3);

                            dish.ProductIds = compoundStr.Split(' ', StringSplitOptions.RemoveEmptyEntries)
                                                         .Select(int.Parse)
                                                         .ToList();

                            dayResponse.Dishes.Add(dish);
                        }
                    }

                    foreach (var dish in dayResponse.Dishes)
                    {
                        if (dish.ProductIds.Count == 0) continue;

                        var prodIdsSql = string.Join(",", dish.ProductIds);

                        var sqlProducts = $@"SELECT product_name 
                                     FROM products 
                                     WHERE id IN ({prodIdsSql})";

                        await using (var cmdProd = new NpgsqlCommand(sqlProducts, conn))
                        await using (var readerProd = await cmdProd.ExecuteReaderAsync())
                        {
                            while (await readerProd.ReadAsync())
                            {
                                dish.ProductNames.Add(readerProd.GetString(0));
                            }
                        }
                    }

                    resultMenu.Add(dayResponse);
                }
            }

            return Ok(resultMenu);
        }
    }
}
