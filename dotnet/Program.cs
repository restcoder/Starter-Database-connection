using Microsoft.Owin.Hosting;
using Owin;
using System;
using System.IO;
using System.Net.Http;
using System.Reflection;
using System.Threading;
using System.Web.Http;
using System.Collections.Generic;
using Npgsql;

namespace App
{
    class Program
    {
        static void Main(string[] args)
        {
            WebApp.Start<App>(new StartOptions
            {
                Port = int.Parse(Environment.GetEnvironmentVariable("PORT")),
                ServerFactory = "Nowin"
            });
            Console.WriteLine("READY");

            Thread.Sleep(Timeout.Infinite);
        }
    }

    class App
    {
        public void Configuration(IAppBuilder app)
        {
            var config = new HttpConfiguration();
            config.MapHttpAttributeRoutes();
            app.UseWebApi(config);
        }
    }

    public class ProductsController : ApiController
    {
        string GetConnectionString() {
          var uri = new Uri(Environment.GetEnvironmentVariable("POSTGRES_URL"));
          var split = uri.UserInfo.Split(':');
          string password = "";
          if (split.Length > 1) {
            password = split[1];
          }
          var database = uri.PathAndQuery.Replace("/", "");
          var cs = string.Format("Server={0};User Id={1};Password={2};Database={3}", uri.Host, split[0], password, database);
          Console.WriteLine(cs);
          return cs;
        }
    
        [Route("products")]
        [HttpGet]
        public object GetProducts()
        {
            var conn = new NpgsqlConnection(GetConnectionString());
            conn.Open();
            var command = new NpgsqlCommand("SELECT * from product", conn);
            NpgsqlDataReader dr = command.ExecuteReader();
            var list = new List<object>();
            while (dr.Read())
            {
                list.Add(new {id = dr[0], name = dr[1], quantity = dr[2]});
            }
            conn.Close();
            return list;
        }
    }
}
