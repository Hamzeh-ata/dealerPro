using DealerPro.Models;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Hangfire;
using Hangfire.MemoryStorage;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using test.carrefour;
using test.GTS;
using test.HMG;
using test.Hubs;
using test.Interfaces;
using test.Models;
using test.OS;
using test.smartBuy;
using test.smartBuyMobiles;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
var builder = WebApplication.CreateBuilder(args);
// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSignalR();
builder.Services.AddHangfire(config =>
config.SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
.UseSimpleAssemblyNameTypeSerializer()
.UseDefaultTypeSerializer()
.UseMemoryStorage());

builder.Services.AddHangfireServer();
builder.Services.AddSingleton<ILaptopNamesHub, cityCenterLaptopsHub>();
builder.Services.AddSingleton<IcityCenterCpu, cityCenterCpu>();
builder.Services.AddSingleton<IcityCenterMotherBoards, cityCenterMotherBoards>();
builder.Services.AddSingleton<IcityCenterGraphicCard, cityCenterGraphicCard>();
builder.Services.AddSingleton<IcityCenterSSD, cityCenterSSD>();
builder.Services.AddSingleton<IcityCenterPCs, cityCenterPCs>();
builder.Services.AddSingleton<IcityCenterRams, cityCenterRams>();
builder.Services.AddSingleton<IcityCenterPSU, cityCenterPSU>();
builder.Services.AddSingleton<IcityCenterHDD, cityCenterHDD>();
builder.Services.AddSingleton<IcityCenterChairs, cityCenterChairs>();
builder.Services.AddSingleton<IcityCenterMouses, cityCenterMouses>();
builder.Services.AddSingleton<IcityCenterMonitors, cityCenterMonitors>();
builder.Services.AddSingleton<IcityCenterKeyBoards, cityCenterKeyBoards>();
builder.Services.AddSingleton<IGTSLaptops, GTSLaptops>();
builder.Services.AddSingleton<IGTSComputers, GTSComputers>();
builder.Services.AddSingleton<IGTSCpu, GTSCpu>();
builder.Services.AddSingleton<IGTSGpu, GTSGpu>();
builder.Services.AddSingleton<IGTSPsu, GTSPsu>();
builder.Services.AddSingleton<IGTSRams, GTSRams>();
builder.Services.AddSingleton<IGTSHDD, GTSHDD>();
builder.Services.AddSingleton<IGTSSSD, GTSSSD>();
builder.Services.AddSingleton<IGTSKeyboards, GTSKeyboards>();
builder.Services.AddSingleton<IGTSHeadset, GTSHeadset>();
builder.Services.AddSingleton<IGTSMonitor, GTSMonitor>();
builder.Services.AddSingleton<IGTSMouse, GTSMouse>();
builder.Services.AddSingleton<IOSComputers, OSComputers>();
builder.Services.AddSingleton<IOSLaptops, OSLaptops>();
builder.Services.AddSingleton<IOSCpus, OSCPUS>();
builder.Services.AddSingleton<IOSRams, OSRams>();
builder.Services.AddSingleton<IOSGpu, OSGpu>();
builder.Services.AddSingleton<iOSMB, OSMB>();
builder.Services.AddSingleton<IOSHdd, OSHdd>();
builder.Services.AddSingleton<IOSSdd, OSSdd>();
builder.Services.AddSingleton<IOSChairs, OSChairs>();
builder.Services.AddSingleton<IOSHeadset, OSHeadset>();
builder.Services.AddSingleton<IOSKeyboards, OSKeyboards>();
builder.Services.AddSingleton<IOSMouse, OSMouse>();
builder.Services.AddSingleton<IOSMonitors, OSMonitors>();
builder.Services.AddSingleton<IOSPsu, OSPsu>();
builder.Services.AddSingleton<IsmartBuyMobiles, smartBuyMobiles>();
builder.Services.AddSingleton<IsmartBuyTv_s, smartBuyTv_s>();
builder.Services.AddSingleton<IsmartBuyWashingMachines, smartBuyWashingMachines>();
builder.Services.AddSingleton<IsmartBuyDishWashers, smartBuyDishWashers>();
builder.Services.AddSingleton<IsmartBuyRefrigerators, smartBuyRefrigerators>();
builder.Services.AddSingleton<IHMGRefrigerators, HMGRefrigerators>();
builder.Services.AddSingleton<IHMGTvs, HMGTvs>();
builder.Services.AddSingleton<IHMGWashingMachines, HMGWashingMachines>();
builder.Services.AddSingleton<IHMGDishwashers, HMGDishwashers>();
builder.Services.AddSingleton<IHMGPhones, HMGPhones>();
builder.Services.AddSingleton<IcarrefourTv_s, carrefourTv_s>();
builder.Services.AddSingleton<IcarrefourDishWashers, carrefourDishWashers>();
builder.Services.AddSingleton<IcarrefourMobiles, carrefourMobiles>();

builder.Services.AddMvc();
builder.Services.AddRazorPages();

builder.Host.ConfigureAppConfiguration((context, config) =>
{
    config.AddJsonFile("appsettings.json", optional: true);
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = "https://securetoken.google.com/811c3",
            ValidAudience = "811c3",
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("-----BEGIN PRIVATE KEY-----\nMIIEvAIBADANBgkqhkiG9w0BAQEFAASCBKYwggSiAgEAAoIBAQCjcaLZCmniz0m0\nk8Cbhn14lLUcq2B85RqCJV+jh+Hwk3e4y3MeoYkTtWVkvvghwbA+gRSfkkd6cDzQ\naiMJQFYcyAOper8N7EwOUXJUXxCkM5zEAKlaSTTaFXKQHVSG58yJqGGv3HjaS4k9\nt6ye2mTtyfoXk4T0bD+zD9vC+MU0ZVO4OYdbjLHhQYsIFXaQ5Hj8qnS13rh1C+Tr\nOxHp8J/qpQapjoEcl71VU0s0DzqtP92R10O03cNNEwLFup+u3rdS2p++oWkNMBKd\nVHuhNGzyAPhXbAgJJ0rDeQjXzB+j26xTczhL7HlobYssEWVLEnbAnQ4D2SdZ+cCc\n0lrdOD1NAgMBAAECggEAQp1zKRDUfUvNF5tM2lajVv0NICU9S0o5VqgS5dzhGqOQ\nHTJWtjpSW0UIkVpAWcnOonsLw8nj9iQ+tRc9Nm0Ra+Odknrj2bHggclyVq70Ymhi\n0YQPgivqLaSmL9S0edOKCuUlA39Rhm9Y/V2aLpiGoGA83VUT0Gc6QyoIgNvRzHuF\nI1ITTMDeWpnEW73N9axttBcqAH+33DjESR58Id+0ExUIEItfUvDES0LnZ3kkeO+Z\n+ZfkiFcDpikZjsC9FGH3DFAl+IL9ad5yW9bJ7LLFrAggOUg9QYQiFkCRhnt31XYn\nY440jP9KWz+zJbyrXzQx54Sq2b0lJAfGfYuZrNkCBQKBgQDTJ9GkxYqSg2xrERFE\n9RM+k4moxkFHlcBNvPzEZvQfw7TP+PR5F5NKEzKw0ir3FOVNsaK49e8Xf5ipKAEY\nW24e0Zyia6aZf/7zNHTRRZoBNrYgGiv8iUGBVZqBVNABPwxlOX+86rrt4/kJaO1+\nvQ/GiaoR0EMYZMdp3Pqh5ggivwKBgQDGJ9AWMqOaepzAdG6R+MxWMJy1SFF27hFh\ns+BPNtU5v+YmIfU0sBx8HRyeictiY1MMiKzb8Jd5sgqJE+wTm1RY6+a+OdTePsk1\nxrDSIIU1LlT0E7pBerpiaAa6H+eicZhOEl++F6gRWBl5P1eHZ8O/rS6r01y31kKU\nMJREiys+8wKBgBhtvOVRLUzyA3MHkHXn3IgazYxHE3pSquLTgHLAbnHWVG7TIIV5\n4sJpIi4uwmW/dx/b8zVuznBrhJ/dTvMgcMcD/RkVVzrfAgHlCbbEVKLbT4q7PGeF\nAZ0S6EMaKs5aGvcDBfI3PdUT0NXz27YT7WVMu+4/p+OElUV8GUrtr/dFAoGAJc6C\nzTlsLZUnQzDzx7YIholP3OanZGGXv2Rqb7KujA4pAy2Hcz4GsUd8pmL0AxmgRsvs\n7ynAPN/TUsQSTstcFHst0y2Fh8HC8heutOivga4NV2RMLSIVXaErZ2ee8kdqH7sx\nuTsaPfTpJ1EGijcqCfAo72+sSQcAPPXn1AwZfDECgYBQxBPcwCyykNN6Q8+MpFqG\nJEeaFVVu9W8RTQo3Vzdj4pWmCasPv6Is4LN+G8E0/nJ2GRqzmlAckNtlpYDuJZld\nXDOrBCColFeshWdh8aXoI8xBWr/Vo3Exq7v9cVudbiOmWEToBa8MdEXcQIYBSUcZ\nPzM/QvId8wkWM/91aQXRsQ==\n-----END PRIVATE KEY-----\n"))
        };
    })
    .AddGoogle(options =>
    {
        options.ClientId = "122554343592-9cv9o3mh0r4tmcta7td96akk4dg2mtbd.apps.googleusercontent.com"; // Replace with your own Google client ID
        options.ClientSecret = "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"; // Replace with your own Google client secret
    });
// Configure application services
builder.Services.Configure<firebaseOptions>(builder.Configuration.GetSection("Firebase"));
// Add Identity services
// Add other necessary services and configurations

// Add controllers and views
builder.Services.AddControllersWithViews();


builder.Services.Configure<IdentityOptions>(options =>
{
    // Password settings
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 8;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireLowercase = true;

    // Lockout settings
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(30);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = true;

    // User settings
    options.User.RequireUniqueEmail = true;
});

var app = builder.Build();
var backgroundJobClient = app.Services.GetService<IBackgroundJobClient>();
var recurringJopManager = app.Services.GetService<IRecurringJobManager>();
var serviceProvider = app.Services.GetService<IServiceProvider>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseFileServer(new FileServerOptions
{
    FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "home")),
    RequestPath = "/home",
    EnableDefaultFiles = true
});
app.UseFileServer(new FileServerOptions
{
    FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "scrapping")),
    RequestPath = "/scrappingPage",
    EnableDefaultFiles = true
});
app.UseFileServer(new FileServerOptions
{
    FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "categories")),
    RequestPath = "/categories",
    EnableDefaultFiles = true
});
app.UseFileServer(new FileServerOptions
{
    FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "products")),
    RequestPath = "/products",
    EnableDefaultFiles = true
});
app.UseFileServer(new FileServerOptions
{
    FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "products")),
    RequestPath = "/home/products",
    EnableDefaultFiles = true
});
app.UseFileServer(new FileServerOptions
{
    FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "compare")),
    RequestPath = "/compare",
    EnableDefaultFiles = true
});
app.UseFileServer(new FileServerOptions
{
    FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "gamingCategories")),
    RequestPath = "/home/gamingCategories",
    EnableDefaultFiles = true
});
app.UseFileServer(new FileServerOptions
{
    FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "homeElectricCategories")),
    RequestPath = "/home/homeElectricCategories",
    EnableDefaultFiles = true
});
app.UseFileServer(new FileServerOptions
{
    FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "products")),
    RequestPath = "/home/gamingCategories/products",
    EnableDefaultFiles = true
});
app.UseFileServer(new FileServerOptions
{
    FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "products")),
    RequestPath = "/home/homeElectricCategories/products",
    EnableDefaultFiles = true
});
app.UseFileServer(new FileServerOptions
{
    FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "mobilesAndTablets")),
    RequestPath = "/home/mobilesAndTablets",
    EnableDefaultFiles = true
});
app.UseFileServer(new FileServerOptions
{
    FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "products")),
    RequestPath = "/home/mobilesAndTablets/products",
    EnableDefaultFiles = true
});
app.UseFileServer(new FileServerOptions
{
    FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "Login")),
    RequestPath = "/login",
    EnableDefaultFiles = true
});
app.UseFileServer(new FileServerOptions
{
    FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "signUp")),
    RequestPath = "/signUp",
    EnableDefaultFiles = true
});
app.UseFileServer(new FileServerOptions
{
    FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "storeDashBoard")),
    RequestPath = "/dashboard",
    EnableDefaultFiles = true
});
app.UseHttpsRedirection();


app.MapControllers();
app.UseRouting();
app.UseAuthorization();
app.UseEndpoints(endpoints =>
{
    endpoints.MapRazorPages();
});
app.UseEndpoints(endpoints =>
{
    endpoints.MapHub<cityCenterLaptopsHub>("/cityCenterLaptopsHub");
    endpoints.MapHangfireDashboard();
});
app.UseEndpoints(endpoints =>
{
    endpoints.MapHub<cityCenterCpu>("/cityCenterCpu");
});
app.UseEndpoints(endpoints =>
{
    endpoints.MapHub<cityCenterMotherBoards>("/cityCenterMotherBoards");
});
app.UseEndpoints(endpoints =>
{
    endpoints.MapHub<cityCenterGraphicCard>("/cityCenterGraphicCard");
});
app.UseEndpoints(endpoints =>
{
    endpoints.MapHub<cityCenterSSD>("/cityCenterSSD");
});
app.UseEndpoints(endpoints =>
{
    endpoints.MapHub<cityCenterPCs>("/cityCenterPCs");
});
app.UseEndpoints(endpoints =>
{
    endpoints.MapHub<cityCenterRams>("/cityCenterRams");
});
app.UseEndpoints(endpoints =>
{
    endpoints.MapHub<cityCenterPSU>("/cityCenterPSU");
});
app.UseEndpoints(endpoints =>
{
    endpoints.MapHub<cityCenterHDD>("/cityCenterHDD");
});
app.UseEndpoints(endpoints =>
{
    endpoints.MapHub<cityCenterChairs>("/cityCenterChairs");
});
app.UseEndpoints(endpoints =>
{
    endpoints.MapHub<cityCenterMouses>("/cityCenterMouses");
});
app.UseEndpoints(endpoints =>
{
    endpoints.MapHub<cityCenterMonitors>("/cityCenterMonitors");
});
app.UseEndpoints(endpoints =>
{
    endpoints.MapHub<cityCenterKeyBoards>("/cityCenterKeyBoards");
});
app.UseEndpoints(endpoints =>
{
    endpoints.MapHub<GTSLaptops>("/GTSLaptops");
});
app.UseEndpoints(endpoints =>
{
    endpoints.MapHub<GTSComputers>("/GTSComputers");
});
app.UseEndpoints(endpoints =>
{
    endpoints.MapHub<GTSCpu>("/GTSCpu");
});
app.UseEndpoints(endpoints =>
{
    endpoints.MapHub<GTSGpu>("/GTSGpu");
});
app.UseEndpoints(endpoints =>
{
    endpoints.MapHub<GTSPsu>("/GTSPsu");
});
app.UseEndpoints(endpoints =>
{
    endpoints.MapHub<GTSRams>("/GTSRams");
});
app.UseEndpoints(endpoints =>
{
    endpoints.MapHub<GTSHDD>("/GTSHDD");
});
app.UseEndpoints(endpoints =>
{
    endpoints.MapHub<GTSSSD>("/GTSSSD");
});
app.UseEndpoints(endpoints =>
{
    endpoints.MapHub<GTSKeyboards>("/GTSKeyboards");
});
app.UseEndpoints(endpoints =>
{
    endpoints.MapHub<GTSHeadset>("/GTSHeadset");
});
app.UseEndpoints(endpoints =>
{
    endpoints.MapHub<GTSMonitor>("/GTSMonitor");
});
app.UseEndpoints(endpoints =>
{
    endpoints.MapHub<GTSMouse>("/GTSMouse");
});
app.UseEndpoints(endpoints =>
{
    endpoints.MapHub<OSComputers>("/OSComputers");
    endpoints.MapHub<OSLaptops>("/OSLaptops");
    endpoints.MapHub<OSCPUS>("/OSCPUS");
    endpoints.MapHub<OSRams>("/OSRams");
    endpoints.MapHub<OSMB>("/OSMB");
    endpoints.MapHub<OSHdd>("/OSHdd");
    endpoints.MapHub<OSChairs>("/OSChairs");
    endpoints.MapHub<OSSdd>("/OSSdd");
    endpoints.MapHub<OSHeadset>("/OSHeadset");
    endpoints.MapHub<OSKeyboards>("/OSKeyboards");
    endpoints.MapHub<OSMouse>("/OSMouse");
    endpoints.MapHub<OSMonitors>("/OSMonitors");
    endpoints.MapHub<OSPsu>("/OSPsu");
    endpoints.MapHub<smartBuyMobiles>("/smartBuyMobiles");
    endpoints.MapHub<smartBuyTv_s>("/smartBuyTv_s");
    endpoints.MapHub<smartBuyWashingMachines>("/smartBuyWashingMachines");
    endpoints.MapHub<smartBuyDishWashers>("/smartBuyDishWashers");
    endpoints.MapHub<smartBuyRefrigerators>("/smartBuyRefrigerators");
    endpoints.MapHub<HMGRefrigerators>("/HMGRefrigerators");
    endpoints.MapHub<HMGTvs>("/HMGTvs");
    endpoints.MapHub<HMGWashingMachines>("/HMGWashingMachines");
    endpoints.MapHub<HMGDishwashers>("/HMGDishwashers");
    endpoints.MapHub<HMGPhones>("/HMGPhones");
    endpoints.MapHub<carrefourTv_s>("/carrefourTv_s");
    endpoints.MapHub<carrefourDishWashers>("/carrefourDishWashers");
    endpoints.MapHub<carrefourMobiles>("/carrefourMobiles");


});
app.UseEndpoints(endpoints =>
{
    endpoints.MapHub<OSGpu>("/OSGpu");
});
app.UseEndpoints(endpoints =>
{
    app.UseEndpoints(endpoints =>
    {
        endpoints.MapControllerRoute(
            name: "home",
            pattern: "home",
            defaults: new { controller = "Home", action = "Index" });

        endpoints.MapControllerRoute(
           name: "products",
           pattern: "/home/products",
           defaults: new { controller = "Products", action = "Index" });


    });

});

backgroundJobClient.Enqueue(() => Console.WriteLine("Hello Hangfire job!"));
recurringJopManager.AddOrUpdate("Run every first day of month", () => serviceProvider.GetService<ILaptopNamesHub>().GetLaptops(), "0 0 1 * *");
app.Run();
