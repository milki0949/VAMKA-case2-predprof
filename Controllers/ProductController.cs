using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Npgsql;
using stolovaya.Models.Entities;
using stolovaya.Models.Requests;
using System.Text;

namespace stolovaya.Controllers
{
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly string _connectionString;

        public ProductController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        [HttpGet("/api/products/render")]
        public async Task<IActionResult> GetProductsApplication()
        {
            var products = new List<ProductsApplication>();
            await using (var connection = new NpgsqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                var sqlSelectCommand = @"SELECT id, product_name FROM products";

                await using (var command = new NpgsqlCommand(sqlSelectCommand, connection)) 
                await using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        products.Add(new ProductsApplication
                        {
                            Id = reader.GetInt32(0),
                            ProductName = reader.GetString(1)
                        });
                    }
                }
            }

            return Ok(products);
        }

        [HttpGet("/api/products")]
        public async Task<IActionResult> GetProducts()
        {
            var products = new List<Produt>();
            await using (var connection = new NpgsqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                var sqlSelectCommand = @"SELECT id, product_name, quantity FROM products";

                await using (var command = new NpgsqlCommand(sqlSelectCommand, connection)) 
                await using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        products.Add(new Produt
                        {
                            Id = reader.GetInt32(0),
                            ProductName = reader.GetString(1),
                            Quantity = reader.GetInt32(2)
                        });
                    }
                }
            }

            return Ok(products);
        }

        [HttpPost("/api/product/subtract")]
        public async Task<IActionResult> ProductQuantitySubtract([FromBody] SubtractProductInput model)
        {
            await using (var connection = new NpgsqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                var sqlSubtractQuantityProduct = @"UPDATE products SET quantity = GREATEST(0, quantity - @Subtract) WHERE id = @Id";

                await using (var SubtractQuantityProductCommand = new NpgsqlCommand(sqlSubtractQuantityProduct, connection))
                {
                    SubtractQuantityProductCommand.Parameters.AddWithValue("Subtract", model.QuantityToSubtract);
                    SubtractQuantityProductCommand.Parameters.AddWithValue("Id", model.Id);

                    await SubtractQuantityProductCommand.ExecuteNonQueryAsync();
                }
            }

            return Ok(new { message = "столбец обновлен" });
        }

        [HttpPost("/api/product/create/application")]
        public async Task<IActionResult> ApplicationProduct([FromBody] List<ApplicationProductInput> products)
        {
            StringBuilder productsStringBuild = new StringBuilder();
            StringBuilder pricesStringBuild = new StringBuilder();
            StringBuilder quantityStringBuid = new StringBuilder();
            string productsString;
            string pricesString;
            string quantityString;

            await using (var connection = new NpgsqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                foreach (var product in products)
                {
                    productsStringBuild.Append(product.Id).Append(" ");
                    quantityStringBuid.Append(product.Quantity).Append(" ");

                    var sqlGetPrice = @"SELECT price FROM products WHERE id = @Id";

                    await using (var getPriceCommand = new NpgsqlCommand(sqlGetPrice, connection))
                    {
                        getPriceCommand.Parameters.AddWithValue("Id", product.Id);

                        var result = await getPriceCommand.ExecuteScalarAsync();

                        int price = Convert.ToInt32(result); 
                        int total = price * product.Quantity;  

                        pricesStringBuild.Append(total).Append(" ");
                    }
                }

                productsString = productsStringBuild.ToString().TrimEnd();
                pricesString = pricesStringBuild.ToString().TrimEnd();
                quantityString = quantityStringBuid.ToString().TrimEnd();

                var sqlPostApplication = @"INSERT INTO applications (products, status, price, quantity) VALUES (@Products, false, @Price, @Quantity)";
                await using (var postApplicationCommand = new NpgsqlCommand(sqlPostApplication, connection))
                {
                    postApplicationCommand.Parameters.AddWithValue("Products", productsString);
                    postApplicationCommand.Parameters.AddWithValue("Price", pricesString);
                    postApplicationCommand.Parameters.AddWithValue("Quantity", quantityString);

                    await postApplicationCommand.ExecuteScalarAsync();
                }
            }

            return Ok();
        }
        [HttpGet("/api/product/application")]
        public async Task<IActionResult> GetApplications()
        {
            var applications = new List<Application>();
            var tempApps = new List<(int Id, int[] ProductIds, int[] Prices, int[] Quantities, bool Status)>();
            var allProductIds = new HashSet<int>();

            await using (var connection = new NpgsqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                var sqlGetApplications = @"SELECT id, products, status, price, quantity FROM applications";
                await using (var getApplicationsCommand = new NpgsqlCommand(sqlGetApplications, connection))
                await using (var readerApplications = await getApplicationsCommand.ExecuteReaderAsync())
                {
                    while (await readerApplications.ReadAsync())
                    {
                        int applicationId = readerApplications.GetInt32(0);
                        string productsString = readerApplications.GetString(1);
                        bool status = readerApplications.GetBoolean(2);
                        string pricesString = readerApplications.GetString(3);
                        string quantitiesString = readerApplications.GetString(4);

                        var productIds = productsString.Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToArray();
                        var prices = pricesString.Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToArray();
                        var quantities = quantitiesString.Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToArray();

                        foreach (var id in productIds) allProductIds.Add(id);
                        tempApps.Add((applicationId, productIds, prices, quantities, status));
                    }
                }

                var productNames = new Dictionary<int, string>();
                if (allProductIds.Count > 0)
                {
                    var parameters = allProductIds.Select((id, idx) => new Npgsql.NpgsqlParameter($"@p{idx}", id)).ToArray();
                    var inClause = string.Join(',', parameters.Select(p => p.ParameterName));
                    var sqlGetNames = $"SELECT id, product_name FROM products WHERE id IN ({inClause})";

                    await using (var cmdNames = new NpgsqlCommand(sqlGetNames, connection))
                    {
                        cmdNames.Parameters.AddRange(parameters);
                        await using (var readerNames = await cmdNames.ExecuteReaderAsync())
                        {
                            while (await readerNames.ReadAsync())
                            {
                                productNames[readerNames.GetInt32(0)] = readerNames.GetString(1);
                            }
                        }
                    }
                }

                foreach (var t in tempApps)
                {
                    var productDetails = new List<ApplicationProductDetails>();
                    int totalAmount = 0;
                    for (int i = 0; i < t.ProductIds.Length; i++)
                    {
                        int id = t.ProductIds[i];
                        int qty = t.Quantities.Length > i ? t.Quantities[i] : 0;
                        int price = t.Prices.Length > i ? t.Prices[i] : 0;
                        productDetails.Add(new ApplicationProductDetails
                        {
                            Id = id,
                            ProductName = productNames.TryGetValue(id, out var name) ? name : "Неизвестный продукт",
                            Quantity = qty,
                            Price = price
                        });
                        totalAmount += price;
                    }

                    applications.Add(new Application
                    {
                        Id = t.Id,
                        Products = productDetails,
                        TotalPrice = totalAmount,
                        Status = t.Status
                    });
                }
            }

            return Ok(applications);
        }

        [HttpPost("/api/products/application/approve")]
        public async Task<IActionResult> ApproveApplication([FromBody] ApproveApplicationInput model)
        {
            int totalQuantity = 0;

            await using (var conn = new NpgsqlConnection(_connectionString))
            {
                await conn.OpenAsync();

                try
                {
                    foreach (var productInApp in model.Products)
                    {
                        var sqlUpdateProduct = @"
                        UPDATE products
                        SET quantity = COALESCE(quantity, 0) + @Quantity
                        WHERE id = @Id";

                        await using (var updateProductCommand = new NpgsqlCommand(sqlUpdateProduct, conn))
                        {
                            updateProductCommand.Parameters.AddWithValue("Quantity", productInApp.Quantity);
                            updateProductCommand.Parameters.AddWithValue("Id", productInApp.Id);
                            totalQuantity += productInApp.Quantity;
                            await updateProductCommand.ExecuteNonQueryAsync();
                        }
                    }

                    var sqlUpdateStatus = @"UPDATE applications SET status = TRUE WHERE id = @Id";
                    await using (var cmd = new NpgsqlCommand(sqlUpdateStatus, conn))
                    {
                        cmd.Parameters.AddWithValue("Id", model.ApplicationId);
                        await cmd.ExecuteNonQueryAsync();
                    }

                    var sqlCostReport = @"UPDATE cost_report SET total_spent = @TotalSpent, total_purchased_product = @TotalProduct WHERE id = 1";
                    await using (var costReportCommand = new NpgsqlCommand(sqlCostReport, conn))
                    {
                        costReportCommand.Parameters.AddWithValue("TotalSpent", model.TotalSpent);
                        costReportCommand.Parameters.AddWithValue("TotalProduct", totalQuantity);
                        await costReportCommand.ExecuteNonQueryAsync();
                    }

                    return Ok();
                }
                catch (Exception ex)
                {
                    return StatusCode(500, new { message = "Ошибка при одобрении заявки: " + ex.Message });
                }
            }
        }

        [HttpDelete("/api/products/application/reject/{id}")]
        public async Task<IActionResult> RejectApplication(int id)
        {
            await using (var connection = new NpgsqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                var sqlDelete = @"DELETE FROM applications WHERE id = @Id";
                await using (var deleteCommand = new NpgsqlCommand(sqlDelete, connection))
                {
                    deleteCommand.Parameters.AddWithValue("Id", id);
                    int rowsAffected = await deleteCommand.ExecuteNonQueryAsync();

                    if (rowsAffected == 0) return NotFound();
                }
            }
            return Ok();
        }
    }
}
