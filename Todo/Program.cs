using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Todo.Data;
using Todo.Endpoints;
using Todo.Mapping;
using Todo.Services;

namespace Todo;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder();

        //Db Context EF Core
        builder.Services.AddDbContext<ApplicationDataContext>(options => 
            options.UseNpgsql(builder.Configuration.GetConnectionString("Default")));
        
        // Serilog logging
        builder.Host.UseSerilog((ctx, lc) => 
            lc.ReadFrom.Configuration(ctx.Configuration));
        
        // Swagger
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        
        //Automapper
        builder.Services.AddAutoMapper(typeof(TodoMappingProfile));
        
        //Validation
        builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly);
        
        //DI
        builder.Services.AddScoped<IDateTimeService, DateTimeService>();
        builder.Services.AddScoped<ITodoService, TodoService>();

        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();    
        }
        
        app.MapEndpoints();
        app.Run();
    }
}
