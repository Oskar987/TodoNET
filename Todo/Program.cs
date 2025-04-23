using System.Net;
using System.Security.Claims;
using System.Text;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations.Internal;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using Todo.Data;
using Todo.Data.Seed;
using Todo.Endpoints;
using Todo.Mapping;
using Todo.Models.Identity;
using Todo.Services;

namespace Todo;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder();

        //Db Context EF Core
        builder.Services.AddDbContext<ApplicationDataContext>(options => 
            options.UseNpgsql(builder.Configuration.GetConnectionString("Default")));
        
        //Identity
        builder.Services.AddDbContext<IdentityDataContext>(options => 
            options.UseNpgsql(builder.Configuration.GetConnectionString("Identity")));

        builder.Services.AddIdentity<User, Role>(options => options.SignIn.RequireConfirmedAccount = false)
            .AddEntityFrameworkStores<IdentityDataContext>()
            .AddSignInManager<SignInManager<User>>()
            .AddRoleManager<RoleManager<Role>>()
            .AddDefaultTokenProviders();
        //TODO: Check if AddDefaultTokenProviders it is neccessary
        
        //Authentication
        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.Events = new JwtBearerEvents
                {
                    OnChallenge = context =>
                    {
                        context.HandleResponse();
                        context.Response.StatusCode = 401;
                        return Task.CompletedTask;
                    },
                    
                    OnForbidden = context =>
                    {
                        context.Response.StatusCode = 403;
                        return Task.CompletedTask;
                    },
                    
                    OnTokenValidated = context =>
                    {
                        var claimsPrincipal = context.Principal;

                        var roles = claimsPrincipal?.FindAll(ClaimTypes.Role).ToList();
                        if (roles is not null)
                        {
                            foreach (var role in roles)
                            {
                                Console.WriteLine($"User = {claimsPrincipal?.FindFirst(ClaimTypes.NameIdentifier)?.Value} Role : {role.Value}");
                            }
                        }
                        return Task.CompletedTask;
                    }
                };

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = builder.Configuration["Jwt:Issuer"],
                    ValidAudience = builder.Configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
                };
            });

        builder.Services.Configure<IdentityOptions>(options =>
        {
            options.ClaimsIdentity.RoleClaimType = ClaimTypes.Role;
        });
        
        //Authorization
        builder.Services.AddAuthorization(options =>
        {
            options.DefaultPolicy = new AuthorizationPolicyBuilder(JwtBearerDefaults.AuthenticationScheme)
                .RequireAuthenticatedUser()
                .Build();
        }); 
        
        // Logging
        builder.Services.AddLogging();
        builder.Host.UseSerilog((ctx, lc) => 
            lc.ReadFrom.Configuration(ctx.Configuration));
        
        // Swagger
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(setup =>
        {
            var jwtSecurityScheme = new OpenApiSecurityScheme
            {
                BearerFormat = "JWT",
                Name = "JWT Authentication",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.Http,
                Scheme = JwtBearerDefaults.AuthenticationScheme,
                Description = "Put **_ONLY_** your JWT Bearer token on textbox below!",

                Reference = new OpenApiReference
                {
                    Id = JwtBearerDefaults.AuthenticationScheme,
                    Type = ReferenceType.SecurityScheme
                }
            };

            setup.AddSecurityDefinition(jwtSecurityScheme.Reference.Id, jwtSecurityScheme);

            setup.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                { jwtSecurityScheme, Array.Empty<string>() }
            });
        });
        
        //Automapper
        builder.Services.AddAutoMapper(typeof(TodoMappingProfile));
        
        //Validation
        builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly);
        
        //DI
        builder.Services.AddScoped<IDateTimeService, DateTimeService>();
        builder.Services.AddScoped<ITodoService, TodoService>();
        builder.Services.AddScoped<IJwtGeneratorService, JwtGeneratorService>();
        builder.Services.AddScoped<IUserContext, UserContext>();

        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();    
        }

        app.UseAuthentication();
        app.UseAuthorization();
        
        app.MapEndpoints();
        
        await app.ApplyMigrations();
        await app.RunAsync();
    }
}
