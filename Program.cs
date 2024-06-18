using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using ResuMeAPI.Authentication;
using ResuMeAPI.Data;
using ResuMeAPI.Helpers;
using ResuMeAPI.Interfaces;
using System.Text.Json.Serialization;

namespace ResuMeAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
            });

            // Register the DB context
            builder.Services.AddDbContext<ResuMeApiDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("ResuMeTestDB"))
            );

            // Register the additional services
            builder.Services.AddScoped<IDbTransactionService, DbTransactionService>();

            // Add CORS services
            builder.Services.AddCors(options =>
            {
                options.AddDefaultPolicy(builder =>
                {
                    builder.AllowAnyOrigin()
                           .AllowAnyMethod()
                           .AllowAnyHeader();
                });

                // You can also define a named policy if you want to be more specific
                options.AddPolicy("AllowSpecificOrigin",
                    builder =>
                    {
                        builder.WithOrigins("http://localhost:3000") // Specify the allowed origins
                               .AllowAnyMethod()
                               .AllowAnyHeader();
                    });
            });

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(x =>
            {
                x.AddSecurityDefinition("ApiKey", new OpenApiSecurityScheme
                {
                    Name = "x-api-key",
                    Description = "The API key to secure the application",
                    Type = SecuritySchemeType.ApiKey,
                    In = ParameterLocation.Header,
                    Scheme = "ApiKeyScheme"
                });

                var scheme = new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Reference = new OpenApiReference
                    {
                        Id = "ApiKey",
                        Type = ReferenceType.SecurityScheme
                    }
                };

                var requirement = new OpenApiSecurityRequirement
                {
                    { scheme, new List<string>() }
                };

                x.AddSecurityRequirement(requirement);
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseCors(); // Enable CORS globally

            app.UseHttpsRedirection();

            // Apply the API key middleware to all the controllers & endpoints
            app.UseMiddleware<ApiKeyAuthMiddleware>();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
