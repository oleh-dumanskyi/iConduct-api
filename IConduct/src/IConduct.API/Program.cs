using IConduct.API.Configuration;
using System.Reflection;

namespace IConduct.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddOpenApi();
            builder.Services.AddSwaggerGen(c =>
            {
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

                c.IncludeXmlComments(xmlPath);
            });
            builder.Services.AddEndpointsApiExplorer();

            builder.Services.AddInfrastructure();
            builder.Services.AddServices();
            
            builder.Services.AddControllers();

            var app = builder.Build();

            app.MapOpenApi();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/openapi/v1.json", "IConduct Test Api - V1");
                c.RoutePrefix = string.Empty;
            });
            app.MapControllers();

            app.Run();
        }
    }
}
