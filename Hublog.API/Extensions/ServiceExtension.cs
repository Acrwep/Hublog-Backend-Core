using Hublog.Repository.Common;
using Hublog.Repository.Entities.Login;
using Hublog.Repository.Interface;
using Hublog.Repository.Repositories;
using Hublog.Service.Interface;
using Hublog.Service.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Security.Claims;
using System.Text;

namespace Hublog.API.Extensions
{
    public static class ServiceExtension
    {
        public static void ConfigureScope(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<Dapperr>();

            services.AddScoped<ILoginRepository, LoginRepository>();
            services.AddScoped<ILoginService, LoginService>();

            services.AddScoped<IScreenshotRepository, ScreenshotRepository>();
            services.AddScoped<IScreenshotService, ScreenshotService>();

            services.AddScoped<IAdminRepository, AdminRepository>();
            services.AddScoped<IAdminService, AdminService>();

            services.AddScoped<ITeamRepository, TeamRepository>();
            services.AddScoped<ITeamService, TeamService>();

            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IUserService, UserService>();

            services.AddScoped<IDesignationRepository, DesignationRepository>();
            services.AddScoped<IDesignationService, DesignationService>();

            services.AddScoped<IRoleRepository, RoleRepository>();
            services.AddScoped<IRoleService, RoleService>();
        }

        public static void ConfigureServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAutoMapper(typeof(MappingProfiles).Assembly);

            services.Configure<JWTSetting>(configuration.GetSection("Jwt"));

            services.AddAuthentication(options =>
             {
                 options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                 options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
             })
             .AddJwtBearer(options =>
                     {
                         var authKey = configuration["Jwt:Secret"];
                         var issuer = configuration["Jwt:Issuer"];
                         var audience = configuration["Jwt:Audience"];

                         options.RequireHttpsMetadata = true;
                         options.SaveToken = true;
                         options.TokenValidationParameters = new TokenValidationParameters
                         {
                             ValidateIssuerSigningKey = true,
                             IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(authKey)),
                             ValidateIssuer = true,
                             ValidIssuer = issuer,
                             ValidateAudience = true,
                             ValidAudience = audience,
                             ValidateLifetime = true,
                         };
                     });

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Hublog API", Version = "v1" });

                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Enter your token",
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    BearerFormat = "JWT",
                    Scheme = "bearer"
                });

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
                                        new string[] {}
                                    }
                                    });
            });

            services.AddAuthorization(options =>
            {
                options.AddPolicy(CommonConstant.Policies.AdminPolicy, policy =>
                    policy.RequireClaim(ClaimTypes.Role, CommonConstant.Role.Admin));
            });
        }
    }
}
