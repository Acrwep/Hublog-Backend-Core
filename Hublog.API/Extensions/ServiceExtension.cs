﻿using Hublog.Repository.Common;
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

            services.AddScoped<IReportRepository, ReportRepository>();
            services.AddScoped<IReportService, ReportService>();

            services.AddScoped<IAppsUrlsRepository, AppsUrlsRepository>();
            services.AddScoped<IAppsUrlsService, AppsUrlsService>();

            services.AddScoped<IAttendanceDashboardRepository, AttendanceDashboardRepository>();
            services.AddScoped<IAttendanceDashboardService, AttendanceDashboardService>();

            services.AddScoped<INoteBookRepository, NoteBookRepository>();
            services.AddScoped<INoteBookService, NoteBookService>();

            services.AddScoped<ISystemInfoRepository, SystemInfoRepository>();
            services.AddScoped<ISystemInfoService, SystemInfoService>();

            services.AddScoped<IProductivityRepository, ProductivityRepository>();
            services.AddScoped<IProductivityService, ProductivityService>();

            services.AddScoped<IAlertService, AlertService>();
            services.AddScoped<IAlertRepository, AlertRepository>();

            services.AddScoped<IActivityService, ActivityService>();
            services.AddScoped<IActivityRepository, ActivityRepository>();

            services.AddScoped<IManual_TimeService,Manual_TimeService>();
            services.AddScoped<IManual_TimeRepository, Manual_TimeRepository>();

            services.AddScoped<IWellnessService, WellnessService>();
            services.AddScoped<IWellnessRepository, WellnessRepository>();

            services.AddScoped<IProjectRepository, ProjectRepository>();
            services.AddScoped<IProjectService, ProjectService>();

            services.AddScoped<IGoalRepository, GoalRepository>();
            services.AddScoped<IGoalService, GoalService>();

            services.AddTransient<IEmailRepository, EmailRepository>();
            services.AddTransient<IEmailService, EmailService>();

            services.AddScoped<IForgotPasswordRepository, ForgotPasswordRepository>();
            services.AddScoped<IForgotPasswordService, ForgotPasswordService>();

            services.AddScoped<IOrganizationRepository, OrganizationRepository>();
            services.AddScoped<IOrganizationService, OrganizationService>();

            services.AddScoped<IOtpRepository, OtpRepository>();
            services.AddScoped<IOtpService, OtpService>();
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
                options.AddPolicy(CommonConstant.Policies.EmployeePolicy, policy =>
                    policy.RequireClaim(ClaimTypes.Role, CommonConstant.Role.Employee));

                options.AddPolicy(CommonConstant.Policies.AdminPolicy, policy =>
                    policy.Requirements.Add(new AdminOrManagerRequirement()));

                options.AddPolicy(CommonConstant.Policies.UserOrAdminPolicy, policy =>
                    policy.RequireClaim(ClaimTypes.Role,
                        CommonConstant.Role.Employee,
                        CommonConstant.Role.Admin));
            });
        }
    }
}
