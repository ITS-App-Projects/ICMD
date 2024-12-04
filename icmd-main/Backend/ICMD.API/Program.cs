using ICMD.API.Auth;
using ICMD.API.Helpers;
using ICMD.Core.Account;
using ICMD.Core.Authorization;
using ICMD.Core.Common;
using ICMD.Core.Constants;
using ICMD.Core.Shared.AutoMapperConfig;
using ICMD.EntityFrameworkCore.Database;
using ICMD.EntityFrameworkCore.Seed;
using ICMD.Infra.IoC;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

internal class Program
{
    private const string DefaultCorsPolicyName = "CorsApi";
    private static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // builder.WebHost.ConfigureKestrel(options =>
        // {
        //     options.Limits.MaxRequestHeaderCount = 52428800;
        //     options.Limits.MaxRequestHeadersTotalSize = 52428800;
        //     options.Limits.MaxRequestBodySize = null;
        // });

        builder.Services.AddRazorPages();
        builder.Services.AddMvc(option =>
        {
            option.MaxModelBindingCollectionSize = int.MaxValue;
        });

        builder.Services.AddControllersWithViews();

        // builder.Services.Configure<IISServerOptions>(options =>
        // {
        //     options.MaxRequestBodySize = int.MaxValue;
        // });

        var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

        int sessionTimeoutMinutes = configuration.GetValue<int>("Authentication:JwtBearer:ExpirationTimeInMinutes");
        builder.Services.AddSession(options =>
        {
            options.IdleTimeout = TimeSpan.FromMinutes(sessionTimeoutMinutes); // Set session timeout to 30 minutes
        });

        builder.Services.Configure<FormOptions>(options =>
        {
            options.ValueLengthLimit = int.MaxValue;
            options.MultipartBodyLengthLimit = int.MaxValue; // if don't set default value is: 128 MB
            options.MultipartHeadersLengthLimit = int.MaxValue;
        });

        // Default classes and context file
        builder.Services.AddDbContext<ICMDDbContext>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("DBConnectionString")));
        builder.Services.Configure<JWTBearerDTO>(builder.Configuration.GetSection("Authentication:JwtBearer"));
        // Default filter for error log

        builder.Services.AddControllers().ConfigureApiBehaviorOptions(options =>
        {
            options.InvalidModelStateResponseFactory = context =>
            {
                var errors = context.ModelState.Values
                   .SelectMany(v => v.Errors)
                   .Select(e => e.ErrorMessage)
               .ToList();

                var errorMessage = string.Join("<br/> ", errors);

                var customResponse = new BaseResponse
                {
                    IsSucceeded = false,
                    Message = errorMessage,
                    StatusCode = System.Net.HttpStatusCode.BadRequest,
                    IsModelValidation = true,
                };

                return new BadRequestObjectResult(customResponse);
            };
        });

        // Identity configuration
        builder.Services.AddIdentity<ICMDUser, ICMDRole>()
            .AddRoles<ICMDRole>().AddEntityFrameworkStores<ICMDDbContext>()
          .AddDefaultTokenProviders();

        builder.Services.AddDistributedMemoryCache();
        builder.Services.AddScoped<RoleManager<ICMDRole>>();
        AuthConfigurer.Configure(builder.Services, builder.Configuration);
        builder.Services.AddDataProtection();
        builder.Services.AddScoped<TokenProvider>();
        builder.Services.AddScoped<CommonMethods>();
        builder.Services.AddScoped<StoredProcedureHelper>();
        builder.Services.AddScoped<ChangeLogHelper>();
        builder.Services.AddScoped<CSVImport>();
        builder.Services.AddTransient(provider => new Lazy<CommonMethods>(() => provider.GetRequiredService<CommonMethods>()));
        builder.Services.AddTransient(provider => new Lazy<ChangeLogHelper>(() => provider.GetRequiredService<ChangeLogHelper>()));
        builder.Services.AddDependencyInjection();

        //Configure CORS for angular UI
        builder.Services.AddCors(options =>
        {
            options.AddPolicy(DefaultCorsPolicyName, builde =>
            {
                //App:CorsOrigins in appsettings.json can contain more than one address with splitted by comma.
                builde
                    //.AllowAnyOrigin()
                    .WithOrigins(
                       (builder.Configuration["App:CorsOrigins"] ?? string.Empty).Split(new[] { "," }, System.StringSplitOptions.RemoveEmptyEntries).ToArray()
                    )
                    .SetIsOriginAllowedToAllowWildcardSubdomains()
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials();
            });
        });

        //Authorization Configuration
        builder.Services.AddAuthorization(config =>
        {
            config.AddPolicy(RoleConstants.Administrator, policy => policy.RequireRole(RoleConstants.Administrator));
        });

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(setup =>
        {
            // Include 'SecurityScheme' to use JWT Authentication
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
                {
                    jwtSecurityScheme, Array.Empty<string>()
                }
            });
        });

        builder.Services.Configure<CookieTempDataProviderOptions>(options =>
        {
            options.Cookie.IsEssential = true;
        });

        builder.Services.AddAuthentication()
         .AddCookie(options =>
         {
             options.LoginPath = "/Account/login";
             options.LogoutPath = "/logout";
         });

        builder.Services.AddAutoMapper(typeof(AutoMapperConfig));
        var app = builder.Build();

        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

        // Configure the HTTP request pipeline.
        if (app.Environment.IsProduction() || app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "ICMD API");
                c.RoutePrefix = String.Empty;
            });
        }
        app.UseCookiePolicy();
        app.UseHttpsRedirection();
        app.UseCors(DefaultCorsPolicyName);
        app.UseAuthentication();
        app.UseAuthorization();
        app.UseStaticFiles();
        app.UseSession();

        //Scope for User & Role Manager
        using (var scope = app.Services.CreateScope())
        {
            var dbContext = (ICMDDbContext)scope.ServiceProvider.GetService(typeof(ICMDDbContext));
            dbContext?.Database.Migrate();

            var userManager = (UserManager<ICMDUser>?)scope.ServiceProvider.GetService(typeof(UserManager<ICMDUser>));
            var roleManager = (RoleManager<ICMDRole>?)scope.ServiceProvider.GetService(typeof(RoleManager<ICMDRole>));

            if (userManager != null && roleManager != null && dbContext != null)
            {
                InitialHostDbBuilder initialHostDbBuilder = new(userManager, roleManager, dbContext);
                await initialHostDbBuilder.Create();
            }
        }
        //Middle Ware
        app.MapControllers();
        app.Run();
    }
}