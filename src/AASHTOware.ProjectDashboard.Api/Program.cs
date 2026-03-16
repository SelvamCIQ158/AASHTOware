using AASHTOware.ProjectDashboard.Api.Data;
using AASHTOware.ProjectDashboard.Api.Infrastructure;
using AASHTOware.ProjectDashboard.Api.OData;
using Microsoft.AspNetCore.OData;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// EF Core with SQL Server
builder.Services.AddDbContext<ProjectDashboardDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("ProjectDashboard")));

// OData + Controllers
builder.Services.AddControllers()
    .AddOData(options => options
        .Select()
        .Filter()
        .OrderBy()
        .Expand()
        .Count()
        .SetMaxTop(1000)
        .AddRouteComponents("odata", EdmModelBuilder.GetEdmModel()));

// Swagger/OpenAPI
builder.Services.AddSwaggerGen(options =>
{
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        options.IncludeXmlComments(xmlPath);
    }
});

var app = builder.Build();

// Swagger UI
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "AASHTOware Project Dashboard OData API v1");
        options.EnableTryItOutByDefault();
    });
}

app.UseODataQueryValidation();
app.UseRouting();

app.MapControllers();

app.Run();

// Make Program accessible for WebApplicationFactory in tests
public partial class Program { }
