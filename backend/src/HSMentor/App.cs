namespace HSMentor;

using HSMentor.Lib.Extensions.DI;
using HSMentor.Persistence;
using Microsoft.EntityFrameworkCore;

public class App
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddSwaggerGen();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddControllers();
        builder.Services.AddServices();

        var connectionString =
            builder.Configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException(
                "Connection string 'DefaultConnection' not found."
            );

        if (builder.Environment.IsEnvironment("Test"))
        {
            builder.Services.AddDbContext<HSMentorDbContext>(options =>
                options.UseInMemoryDatabase("HSMentor").UseLazyLoadingProxies()
            );
        }
        else
        {
            builder.Services.AddDbContext<HSMentorDbContext>(options =>
                options
                    .UseNpgsql(connectionString)
                    .UseSnakeCaseNamingConvention()
                    .UseLazyLoadingProxies()
            );
        }

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.MapControllers();
        app.UseHttpsRedirection();

        app.Run();
    }
}
