using HosteraBackend.API.Shared.Infrastructure.Interfaces.Asp.Configuration;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddRouting(options => options.LowercaseUrls = true);

// Configure Kebab-Case route naming convention for controllers
builder.Services.AddControllers(options => 
    options.Conventions.Add(new KebabCaseRouteNamingConvention()));

builder.Services.Configure<RouteOptions>(options =>
{
    options.ConstraintMap["kebab"] = typeof(KebabCaseRouteNamingConvention);
});

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();