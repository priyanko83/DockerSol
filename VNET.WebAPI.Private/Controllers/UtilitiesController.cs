using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace VNET.WebAPI.Private.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UtilitiesController : ControllerBase
    {
        //  This Web API is Accessible only from VNET & selected Virtual Machine IP address Only: https://private-facing-app.azurewebsites.net/api/utilities
        //  This Makes use of VNET integration feature of S1 app service plan. 
        //  Important: Above configuration means the WebAPI is still reachable from public internet, but will resolve in 403 error
        //  In Premium V2, V3 app service plans, we can use private link 
        //  With Private Link, the endpoint will not be reachable from public internet in first place, resolving to 404 error
        [HttpGet]
        public string InvokeFunctionApp()
        {
            // This Web API is joined to a delegating subnet. And function app allows calls only from that delegating subnet
            // Above setting enables web api to successfully invoke the function app (code below)
            return $"WebAPI-Private-InvokedFunctionApp: {MakeWebRequest("https://pm-function01.azurewebsites.net/api/WebEndpoint")}";
        }

        //  https://private-facing-app.azurewebsites.net/api/utilities/InvokeSQL
        //  This works since Azure SQL is integrated to the delegating VNET 
        [HttpGet]
        [Route("InvokeSQL")]
        public string InvokeSQL()
        {
            return $"WebAPI-Private-InvokedSQL: {GetDatafromSQL()}";
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