using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Data.SqlClient;
using System.IO;
using System.Net;
using System.Text;

namespace VNET.WebAPI.Public.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UtilitiesController : ControllerBase
    {
        //  https://public-facing-web.azurewebsites.net/api/utilities/InvokeFunctionApp

        [HttpGet]
        [Route("InvokeFunctionApp")]
        public string InvokeFunctionApp()
        {
            return $"InvokedFunctionApp: {MakeWebRequest("https://pm-function01.azurewebsites.net/api/WebEndpoint")}";
        }

        //  https://public-facing-web.azurewebsites.net/api/utilities/InvokeWebApp
        [HttpGet]
        [Route("InvokeWebApp")]
        public string InvokeWebApp()
        {
            return $"InvokedWebApp: {MakeWebRequest("https://private-facing-app.azurewebsites.net/api/utilities")}";
        }

        //  https://public-facing-web.azurewebsites.net/api/utilities/InvokeSQL
        //  This works since Azure SQL is integrated to the delegating VNET 
        [HttpGet]
        [Route("InvokeSQL")]
        public string InvokeSQL()
        {
            return $"InvokedSQL: {GetDatafromSQL()}";
        }

        private string MakeWebRequest(string url)
        {
            try
            {
                
                var request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "GET";
                string content = null;
                using (var response = (HttpWebResponse)request.GetResponse())
                {
                    using (var stream = response.GetResponseStream())
                    {
                        using (var sr = new StreamReader(stream))
                        {
                            content = sr.ReadToEnd();
                        }
                    }
                }
                return content;
            }
            catch (Exception ex)
            {
                //writer.Write("InvokeCustomerAPI Error: " + ex.Message);
                return (ex.Message);
            }
        }

        private string GetDatafromSQL()
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
                sb.Append(e.Message);
                if (e.InnerException != null)
                {
                    sb.Append($"Inner exception: {e.InnerException.Message}");
                }
            }

            return sb.ToString();
        }
    }
}