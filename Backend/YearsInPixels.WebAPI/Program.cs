using YearsInPixels.Infrastructure;
using YearsInPixels.WebAPI.Middleware;

namespace YearsInPixels.WebAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

            var builder = WebApplication.CreateBuilder(args);


            builder.Services.AddCors(options =>
            {
                options.AddPolicy(name: MyAllowSpecificOrigins,
                                  policy =>
                                  {
                                      policy.WithOrigins("http://localhost:3000")
                                        .AllowAnyMethod()
                                        .AllowAnyHeader();
                                  });
            });

            builder.Services.AddControllers();

            builder.Services.AddInfrastructure(builder.Configuration);

            var app = builder.Build();

            app.UseCors(MyAllowSpecificOrigins);

            app.UseMiddleware<ExceptionMiddleware>();

            app.UseHttpsRedirection();

            app.UseAuthentication();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
