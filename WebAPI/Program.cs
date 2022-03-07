using Business.DependencyResolvers;
using Core.DependencyResolvers;
using Core.Extensions;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using System.Text.Json.Serialization;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Localization;
using System.Globalization;
using Core.Utilities.IoC;
using Core.Utilities.Security.Jwt;
using MediatR;

    var builder = WebApplication.CreateBuilder(args);
    var services = builder.Services;
    var configuration = builder.Configuration;
    var host = builder.Host;
    
    services.AddSingleton<IConfiguration>(x => configuration);
    services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
    services.AddMediatR(AppDomain.CurrentDomain.GetAssemblies());
    services.AddEndpointsApiExplorer();
    services.AddSwaggerGen();
    services.AddHttpClient();
    var tokenOptions = configuration.GetSection("TokenOptions").Get<TokenOptions>();
    services.ConfigureAuthentication(tokenOptions);

    services.AddControllers().AddNewtonsoftJson(options =>
    options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore);

    services.AddDependencyResolvers(new IDIModule[]
    {
        new CoreModule(),
        new BusinessModule()
    });

    host.UseServiceProviderFactory(new AutofacServiceProviderFactory());
    host.ConfigureContainer<ContainerBuilder>(b => b.RegisterModule(new AutofacBusinessModule()));
    
    services.AddControllers().AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });
    
    var corsPolicies = configuration.GetSection("CorsPolicies").Get<string[]>();
    services.AddCors(options =>
    {
        options.AddPolicy("CorsPolicy",
            //builder.WithOrigins(corsPolicies)
            b => b.AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader()
                // .AllowCredentials()
                );
    });


    var app = builder.Build();
    ServiceTool.ServiceProvider = ((IApplicationBuilder) app).ApplicationServices;

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseHsts();

    app.UseSecurityHeaders();

    app.ConfigureCustomExceptionMiddleware();

    app.UseCors("CorsPolicy");

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

    app.UseHttpsRedirection();
    
    app.UseRouting();
    
    app.UseAuthentication(); 
    
    app.UseAuthorization(); 
    
    app.UseEndpoints(endpoints =>
    {
        endpoints.MapControllers();
    });

    app.Run();
