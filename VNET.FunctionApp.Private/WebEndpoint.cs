using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Text;
using System.Data.SqlClient;

namespace VNET.FunctionApp.Private
{
    public static class WebEndpoint
    {
        // This function app is configured to accept incoming traffic only from one subnet (and selected IP addresses from dev VMs)
        // If accessed from outside the vnet, (e.g. public internet) then it gives 403 error
        //  This does not work since function app is in consumption plan and outbound ip addresses aren't fixed. 
        [FunctionName("WebEndpoint")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string name = req.Query["name"];

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            name = name ?? data?.name;

            string responseMessage = GetDatafromSQL();

            return new OkObjectResult(responseMessage);
        }

        private static string GetDatafromSQL()
        {
            StringBuilder sb = new StringBuilder();

            try
            {
                SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder("Server=tcp:pm-sql-server01.database.windows.net,1433;Initial Catalog=pm-db;Persist Security Info=False;User ID=priyanko;Password=Abcdef@123456;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;");

                using (SqlConnection connection = new SqlConnection(builder.ConnectionString))
                {
                    connection.Open();

                    String sql = "SELECT name, collation_name FROM sys.databases";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                sb.Append(string.Format("{0} {1}         ", reader.GetString(0), reader.GetString(1)));
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                sb.Append(("{0} ", e.Message));
                if (e.InnerException != null)
                {
                    sb.Append(("Inner exception: {0} ", e.InnerException.Message));
                }
            }

            return sb.ToString();
        }
    }
}
