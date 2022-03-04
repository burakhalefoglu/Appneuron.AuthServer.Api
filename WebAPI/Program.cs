using Business.DependencyResolvers;
using Core.DependencyResolvers;
using Core.Extensions;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using System.Text.Json.Serialization;
using Core.Utilities.Security.Encyption;
using Core.Utilities.Security.Jwt;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Localization;
using System.Globalization;
using System.Reflection;
using Business.Extensions;
using Core.Utilities.IoC;
using MediatR;

var builder = WebApplication.CreateBuilder(args);
    var services = builder.Services;
    var configuration = builder.Configuration;
    
    // Add services to the container.
    services.AddSingleton<IConfiguration>(x => configuration);
    // services.AddMediatRApi();
    // services.AddMediatR(Assembly.GetExecutingAssembly());

    services.AddControllers().AddNewtonsoftJson(options =>
    options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore);

    services.AddDependencyResolvers(new IDIModule[]
    {
        new CoreModule(),
        new BusinessModule()
    });

    builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());
    builder.Host.ConfigureContainer<ContainerBuilder>(builder => builder.RegisterModule(new AutofacBusinessModule()));
    
    services.AddControllers().AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });
    
    var corsPolicies = configuration.GetSection("CorsPolicies").Get<string[]>();
    services.AddCors(options =>
    {
        options.AddPolicy("CorsPolicy",
            //builder.WithOrigins(corsPolicies)
            builder => builder.AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader()
                // .AllowCredentials()
                );
    });

    var tokenOptions = configuration.GetSection("TokenOptions").Get<TokenOptions>();
    services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidIssuer = tokenOptions.Issuer,
                ValidAudiences = tokenOptions.Audience,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = SecurityKeyHelper.CreateSecurityKey(tokenOptions.SecurityKey),
                ClockSkew = TimeSpan.Zero
            };
        });


    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    services.AddEndpointsApiExplorer();
    services.AddSwaggerGen();
    var app = builder.Build();
    ServiceTool.ServiceProvider = ((IApplicationBuilder) app).ApplicationServices;

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    //güvenlik için gerekli...
    app.UseHsts();
    //güvenlik için gerekli...
    app.UseSecurityHeaders();

    app.ConfigureCustomExceptionMiddleware();

    app.UseHttpsRedirection();

    app.UseCors("CorsPolicy");

    app.UseRouting();

    app.UseAuthentication();

    app.UseAuthorization();

    // Make Turkish your default language. It shouldn't change according to the server.
    app.UseRequestLocalization(new RequestLocalizationOptions
    {
        DefaultRequestCulture = new RequestCulture("tr-TR")
    });

    var cultureInfo = new CultureInfo("tr-TR")
    {
        DateTimeFormat =
        {
            ShortTimePattern = "HH:mm"
        }
    };

    CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
    CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;

    //app.UseHttpsRedirection();

    app.MapControllers();

    app.Run();
