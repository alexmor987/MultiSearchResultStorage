using BusinessLogic.Communication;
using BusinessLogic.Search;
using BusinessLogic.Search.Interfaces;
using BusinessLogic.Services;
using BusinessLogic.Services.Interfaces;
using DataAccess.DB.Models;
using DataAccess.Repositories.RepositoryContainer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.IISIntegration;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Models.Basic;
using Models.Services.Settings;


var builder = WebApplication.CreateBuilder(args);

#region DB

builder.Services.AddDbContext<WebSearchArchiveContext>(options =>
options.UseNpgsql(builder.Configuration.GetConnectionString("DataBase")));

builder.Services.AddScoped<IRepositoryContainer, RepositoryContainer>();

#endregion

#region Cache

builder.Services.AddStackExchangeRedisCache(options =>
    options.Configuration = builder.Configuration.GetConnectionString("Cache"));

#endregion





// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("tranId", new OpenApiSecurityScheme
    {
        Description = "Transaction ID header",
        Name = "tranId",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "apiKey"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "tranId"
                },
                Name = "tranId",
                In = ParameterLocation.Header,
            },
            new List<string>()
        }
    });
});


#region Logger

builder.Logging.AddLog4Net();
builder.Logging.AddFilter("Microsoft.EntityFrameworkCore", LogLevel.Warning);

#endregion

#region Security
builder.Services.AddCors(options =>
{
    string AllowedOrigins = builder.Configuration.GetValue<string>("AllowedHosts");

    options.AddPolicy("_AllowSpecificOrigins",
      builder =>
      {
          builder.WithOrigins(AllowedOrigins).AllowAnyMethod().AllowAnyHeader();
      });

    builder.Services.AddAuthentication(IISDefaults.AuthenticationScheme);
});

#endregion

#region Model Validation Errors handling

builder.Services.AddMvc().ConfigureApiBehaviorOptions(options =>
{
    options.InvalidModelStateResponseFactory = context =>
    {

        LogAutomaticBadRequest(context);

        ServerResponse response = new ServerResponse(400);
        return new OkObjectResult(response);
    };
});

#endregion

#region BusinessLogic

var settings = builder.Configuration.GetSection("ServicesSettings").Get<ServicesSettingsModel[]>().ToList();

builder.Services.AddScoped<IGetSearchResultsBL, GetSearchResultsBL>();
builder.Services.AddScoped<ICommunicationBL, CommunicationBL>();

#endregion

#region ExternalServices

RegisterExternalServices();

#endregion

builder.Services.AddHttpClient();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();



static void LogAutomaticBadRequest(ActionContext context)
{
    try
    {
        // Setup logger from DI - as explained in https://github.com/dotnet/AspNetCore.Docs/issues/12157
        var loggerFactory = context.HttpContext.RequestServices.GetRequiredService<ILoggerFactory>();
        var logger = loggerFactory.CreateLogger(context.ActionDescriptor.DisplayName);

        // Get error messages
        var errorMessages = string.Join(" | ", context.ModelState.Values
            .SelectMany(x => x.Errors)
            .Select(x => x.ErrorMessage));

        var request = context.HttpContext.Request;

        // Use whatever logging information you want
        logger.LogError("Model Validation Errors." +
                        $"{System.Environment.NewLine}Error(s): {errorMessages}" +
                        $"{System.Environment.NewLine}|{request.Method}| Full URL: {request.Path}.");
    }
    catch
    {


    }
}

void RegisterExternalServices()
{

    if (settings.First(i => i.Name == "GoogleSearchAPI").UseMock)
    {
        builder.Services.AddScoped<ISearchService, SearchServiceMock>();
    }
    else
    {
        builder.Services.AddScoped<ISearchService, SearchService>();
    }
}
