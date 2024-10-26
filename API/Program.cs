using Microsoft.EntityFrameworkCore;

namespace API;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();

        builder.Services.AddDbContext<AppDbContext>(options =>
        {
            options.UseInMemoryDatabase("inMemory");
        });

        var app = builder.Build();

        app.UseAuthorization();
        app.MapControllers();

        app.Run();
    }
}