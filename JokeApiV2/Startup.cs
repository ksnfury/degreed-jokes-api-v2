using JokeApiV2.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using JokeApiV2.Services.Cache;
using JokeApiV2.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using JokeApiV2.Services.Decorator;
using Microsoft.Extensions.Logging;

namespace JokeApiV2
{
    public class Startup
    {
        private readonly IConfiguration _configuration;


        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {

            // Add MySQL database
            services.AddDbContext<JokeDbContext>(options =>
            {
                options.UseMySql(_configuration.GetConnectionString("DefaultConnection"), new MySqlServerVersion(new Version(8, 0, 33)));
            });

            // Initialize the database and seed data
            //using (var scope = services.BuildServiceProvider().CreateScope())
            //{
            //    var serviceProvider = scope.ServiceProvider;

            //    if (serviceProvider.GetService<JokeDbContext>() != null)
            //    {
            //        var dbContext = serviceProvider.GetRequiredService<JokeDbContext>();

            //        // Create the database if it doesn't exist and seed it with initial data
            //        DbInitializer.Initialize(dbContext);
            //    }
            //}

            // Add logger services
            services.AddLogging();

            try
            {
                using (var scope = services.BuildServiceProvider().CreateScope())
                {
                    var serviceProvider = scope.ServiceProvider;
                    var dbContext = serviceProvider.GetRequiredService<JokeDbContext>();

                    // Check if the database exists and apply migrations
                    dbContext.Database.EnsureCreated();

                    // Check if the database is empty and seed it with initial data if necessary
                    if (!dbContext.Jokes.Any())
                    {
                        // Seed the database with initial data
                        DbInitializer.Initialize(dbContext);
                    }
                }
            }
            catch (Exception ex)
            {
                var _logger = services.BuildServiceProvider().GetRequiredService<ILogger<Startup>>();
                _logger.LogError(ex, "An exception occurred during database initialization.");
                services.AddSingleton<JokeDbContext>(provider => null); 
            }

            // Add required services
            services.AddControllers();

            // Configure authentication
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                // Configure JWT bearer authentication options
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = "JokeAPI",
                    ValidAudience = "JokeAPIUsers",
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("wZGZG2D+gG+7X+Y+kuRKMfbXSNYMaq0ZAwX3cJvT02c="))
                };
            });

            // Register your custom services

            services.AddScoped<IJokeService, JokeService>();
            services.AddScoped<IHighlightingDecorator, EmphasisHighlightingDecorator>();

            services.AddSingleton<ILogger<LRUJokeCache>, Logger<LRUJokeCache>>();
            services.AddSingleton<IJokeCache>(provider =>
            {
                ILogger<LRUJokeCache> logger = provider.GetRequiredService<ILogger<LRUJokeCache>>();
                return new LRUJokeCache(10, logger);
            });

            // adding logger to JokeService
            //services.AddSingleton<ILogger<JokeService>, Logger<JokeService>>();

            services.AddScoped<IJokeService>(provider =>
            {
                var dbContext = provider.GetService<JokeDbContext>();
                var highlightingDecorator = provider.GetService<IHighlightingDecorator>();
                var jokeCache = provider.GetService<IJokeCache>();
                var logger = provider.GetService<ILogger<JokeService>>();

                if (dbContext != null)
                {
                    return new JokeService(provider, highlightingDecorator, jokeCache, logger);
                }
                else
                {
                    return new JokeService(null, highlightingDecorator, jokeCache, logger); // Pass null when dbContext is not available
                }
            });


            services.AddCors(options =>
            {
                options.AddDefaultPolicy(builder =>
                {
                    builder.AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader();
                });
            });

            // Configure authorization
            services.AddAuthorization();

            // Add controllers
            services.AddControllers();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Joke API", Version = "v1" });

                // Define the security scheme as "Bearer"
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.Http,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    Description = "Enter 'Bearer' followed by a space and the JWT token",
                });

                // Add the security requirement to all operations
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[] { }
                    }
                });
            });

        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseCors(builder =>
            {
                builder.AllowAnyOrigin()
                       .AllowAnyMethod()
                       .AllowAnyHeader();
            });

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Joke API V1");
            });
        }

    }
}
