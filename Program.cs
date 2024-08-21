using System.Net.Http;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MinimalApi.Utils.Extensions;

namespace MinimalApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddAuthorization();
            builder.Services.AddHttpClient();
            builder.Services.AddHttpClient("custom")
                .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback = (_, _, _, _) => true
                });
            var conString = builder.Configuration.GetConnectionString("DbDocQueue");
            builder.Services.AddHealthChecks().AddSqlServerQueryHealthCheck(conString);
            builder.Services.AddDapperRepository(options => options.UserSqlServer(conString));

            var app = builder.Build();
            app.UseAuthorization();
            app.MapHealthChecks("/Health");
            app.MapGet("/Pulse", (HttpContext _) => "The service is running.");
            app.MapGet("/Pulse/AddDoc", (string docId) => docId);
            app.MapGet("/Pulse/DeleteDoc", (string docId) => docId);

            app.Run();
        }
    }
}
