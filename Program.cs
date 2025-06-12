using JwtRoleAuthentication.Service;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using System.Text.Json.Serialization;
using testbills.Data;
using testbills.Models;
using testbills.Services;

namespace testbills

{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            // 1️⃣ Enregistrement de la politique CORS (vers la fin du builder.Services)
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAngularApp",
                    policy =>
                    {
                        policy.WithOrigins("http://localhost:4200")
                              .AllowAnyHeader()
                              .AllowAnyMethod();
                    });
            });


            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(option =>
            {
                // Spécification de la version et du titre de l'API
                option.SwaggerDoc("v1", new OpenApiInfo { Title = "Test API", Version = "v1" });

                // Configuration de l'authentification dans Swagger
                option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Veuillez saisir un jeton valide",
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    BearerFormat = "JWT",
                    Scheme = "Bearer"
                });

                option.AddSecurityRequirement(new OpenApiSecurityRequirement
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
                option.DescribeAllParametersInCamelCase();

                // Configure Swagger to understand file uploads
             
            });

            // Configuration des problèmes de détails
            builder.Services.AddProblemDetails();

            // Configuration du routage en minuscules
            builder.Services.AddRouting(options => options.LowercaseUrls = true);
            builder.Services.AddRouting(options => options.LowercaseUrls = true);

            // Configuration des bases de données
            builder.Services.AddDbContext<ApplicationDbContext>(opt =>
                  opt.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));
                  //opt.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            // Enregistrement du service TokenService
            builder.Services.AddScoped<TokenService, TokenService>();
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<DocumentIntelligenceService>();

            // Prise en charge des conversions de chaînes en énumérations
            builder.Services.AddControllers().AddJsonOptions(opt =>
            {
                opt.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            });
            builder.Services.AddSignalR();
            // Configuration des exigences d'identité
            builder.Services
                .AddIdentity<ApplicationUser, IdentityRole>(options =>
                {
                    options.SignIn.RequireConfirmedAccount = false;
                    options.User.RequireUniqueEmail = true;
                    options.Password.RequireDigit = false;
                    options.Password.RequiredLength = 6;
                    options.Password.RequireNonAlphanumeric = false;
                    options.Password.RequireUppercase = false;
                })
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>();

            // Configuration de l'authentification
            var jwtSettings = builder.Configuration.GetSection("JwtTokenSettings").Get<JwtTokenSettings>();

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.IncludeErrorDetails = true;
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ClockSkew = TimeSpan.Zero,
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings.ValidIssuer,
                    ValidAudience = jwtSettings.ValidAudience,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(jwtSettings.SymmetricSecurityKey)
                    ),
                };
            });


            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<TokenService, TokenService>();
            //builder.Services.AddSingleton<DocumentIntelligenceService>();
            builder.Services.AddDbContext<ApplicationDbContext>(opt =>
      opt.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));
     // opt.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            // 2️⃣ Dans le pipeline, avant UseAuthorization()
            app.UseCors("AllowAngularApp");

         


            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }

}
