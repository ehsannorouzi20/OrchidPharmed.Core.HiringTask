
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using OrchidPharmed.Core.HiringTask.API.Filters;
using OrchidPharmed.Core.HiringTask.API.Middleware;
using OrchidPharmed.Core.HiringTask.API.Models;
using OrchidPharmed.Core.HiringTask.API.Repositories;
using Serilog;
using StackExchange.Redis;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
    options.EnableDetailedErrors();
    options.EnableSensitiveDataLogging();
});

Log.Logger = new Serilog.LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.Console()
    .WriteTo.MSSqlServer(connectionString: builder.Configuration.GetConnectionString("DefaultConnection"),
    sinkOptions: new Serilog.Sinks.MSSqlServer.MSSqlServerSinkOptions { TableName = "Logs", AutoCreateSqlTable = true })
    .CreateLogger();

builder.Services.AddScoped(typeof(IUnitOfWork), typeof(UnitOfWork));
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(System.Reflection.Assembly.GetExecutingAssembly()));

builder.Services.AddControllers(options =>
{
    options.Filters.Add<UniformResultFilter>();
});

// Use Redis as Distributed Caching
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration["RedisServer:Address"];
    options.InstanceName = "MainInstance";
});
builder.Services.AddSingleton<OrchidPharmed.Core.HiringTask.API.Structure.ICacheManager, OrchidPharmed.Core.HiringTask.API.Structure.CacheManager>();

// Use Redis as NoSql Database
builder.Services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(builder.Configuration["RedisServer:Address"]));

builder.Services.AddApiVersioning(config =>
{
    config.DefaultApiVersion = new Microsoft.AspNetCore.Mvc.ApiVersion(1, 0);
    config.AssumeDefaultVersionWhenUnspecified = true;
    config.ReportApiVersions = true;
    config.ApiVersionReader = new UrlSegmentApiVersionReader();
});
builder.Services.AddVersionedApiExplorer(config =>
{
    config.GroupNameFormat = "'v'VVV";
    config.SubstituteApiVersionInUrl = true;
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.EnableAnnotations();
    var provider = builder.Services.BuildServiceProvider().GetRequiredService<IApiVersionDescriptionProvider>();
    foreach (var description in provider.ApiVersionDescriptions)
    {
        c.SwaggerDoc(description.GroupName,
            new Microsoft.OpenApi.Models.OpenApiInfo { Title = "OrchidPharmed Hiring Task API", Version = description.ApiVersion.ToString() });
    }
    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Please Enter JWT Token",
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
        BearerFormat = "JWT",
        Scheme = "Bearer"
    });
    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement {
    {
        new Microsoft.OpenApi.Models.OpenApiSecurityScheme
        {
            Reference = new Microsoft.OpenApi.Models.OpenApiReference
            {
                Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                Id = "Bearer"
            }
        },
        new string[]{ }
    }});
});
builder.Services.AddAuthentication(x =>
    {
        x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        x.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        var Key = Encoding.UTF8.GetBytes(builder.Configuration["JWT:Key"]);
        options.SaveToken = true;
        options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters()
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["JWT:Issuer"],
            ValidAudience = builder.Configuration["JWT:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Key),
        };
    });
builder.Services.AddAuthorization();
builder.Services.AddTransient<FluentValidation.IValidator<OrchidPharmed.Core.HiringTask.API.Structure.DTO.CreateProjectDTO>, OrchidPharmed.Core.HiringTask.API.Validators.CreateProjectDTOValidator>();
builder.Services.AddTransient<FluentValidation.IValidator<OrchidPharmed.Core.HiringTask.API.Structure.DTO.CreateTaskDTO>, OrchidPharmed.Core.HiringTask.API.Validators.CreateTaskDTOValidator>();
builder.Services.AddScoped<ExceptionFilter>();
builder.Services.AddCors(o => o.AddPolicy("MyPolicy", builder =>
{
    builder.AllowAnyOrigin()
           .AllowAnyMethod()
           .AllowAnyHeader();
}));
builder.Services.AddMvc()
    .AddJsonOptions(jsonOptions =>
    {
        jsonOptions.JsonSerializerOptions.PropertyNamingPolicy = null;
    });
builder.Services.AddAutoMapper(typeof(Program));
var app = builder.Build();
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    db.Database.Migrate();
}
app.UseStaticFiles();
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    using (var scope = app.Services.CreateScope())
    {
        var provider = scope.ServiceProvider.GetRequiredService<IApiVersionDescriptionProvider>();
        foreach (var desc in provider.ApiVersionDescriptions)
        {
            c.SwaggerEndpoint($"/swagger/{desc.GroupName}/swagger.json", desc.GroupName.ToUpperInvariant());
        }
    }
    c.InjectStylesheet("/swagger-ui/custom.css");
});
app.UseReDoc(c =>
{
    c.RoutePrefix = "Redoc";
    using (var scope = app.Services.CreateScope())
    {
        var provider = scope.ServiceProvider.GetRequiredService<IApiVersionDescriptionProvider>();
        foreach (var desc in provider.ApiVersionDescriptions)
        {
            c.SpecUrl($"/swagger/{desc.GroupName}/swagger.json");
        }
    }
});
app.UseRouting();
app.UseCors("MyPolicy");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
