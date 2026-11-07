using HosteraBackend.API.AccessControl.Application.CommandServices;
using HosteraBackend.API.AccessControl.Application.Internal.CommandServices;
using HosteraBackend.API.AccessControl.Application.Internal.QueryServices;
using HosteraBackend.API.AccessControl.Application.QueryServices;
using HosteraBackend.API.AccessControl.Domain.Repositories;
using HosteraBackend.API.AccessControl.Infrastructure.Persistence.EntityFrameworkCore.Repositories;
using HosteraBackend.API.Inventory.Application.CommandServices;
using HosteraBackend.API.Inventory.Application.Internal.CommandServices;
using HosteraBackend.API.Inventory.Application.Internal.QueryServices;
using HosteraBackend.API.Inventory.Application.QueryServices;
using HosteraBackend.API.Inventory.Domain.Repositories;
using HosteraBackend.API.Inventory.Infrastructure.Persistence.EntityFrameworkCore.Repositories;
using HosteraBackend.API.Rooms.Application.CommandServices;
using HosteraBackend.API.Rooms.Application.Internal.CommandServices;
using HosteraBackend.API.Rooms.Application.Internal.QueryServices;
using HosteraBackend.API.Rooms.Application.QueryServices;
using HosteraBackend.API.Rooms.Domain.Repositories;
using HosteraBackend.API.Rooms.Infrastructure.Persistence.EntityFrameworkCore.Repositories;
using HosteraBackend.API.Shared.Domain.Repositories;
using HosteraBackend.API.Shared.Infrastructure.Interfaces.Asp.Configuration;
using HosteraBackend.API.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;
using HosteraBackend.API.Shared.Infrastructure.Persistence.EntityFrameworkCore.Repositories;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

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

// Add CORS Policy
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllPolicy",
        policy => policy.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader());
});

// Add Database Connection
builder.Services.AddDbContext<AppDbContext>(options =>
{
    var connectionStringTemplate = builder.Configuration.GetConnectionString("DefaultConnection");
    if (string.IsNullOrWhiteSpace(connectionStringTemplate))
        throw new InvalidOperationException("Database connection string is not set in the configuration.");

    // Environment variables such as %HOSTERA_DB_PASSWORD% keep the credentials out of the repository
    var connectionString = Environment.ExpandEnvironmentVariables(connectionStringTemplate);
    options.UseMySQL(connectionString);

    if (builder.Environment.IsDevelopment())
        options.EnableSensitiveDataLogging().EnableDetailedErrors();
});

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Dependency Injection

// Shared Bounded Context
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// Inventory Bounded Context
builder.Services.AddScoped<IStorageLocationRepository, StorageLocationRepository>();
builder.Services.AddScoped<IInventoryItemRepository, InventoryItemRepository>();
builder.Services.AddScoped<IStorageLocationCommandService, StorageLocationCommandService>();
builder.Services.AddScoped<IStorageLocationQueryService, StorageLocationQueryService>();
builder.Services.AddScoped<IInventoryItemCommandService, InventoryItemCommandService>();
builder.Services.AddScoped<IInventoryItemQueryService, InventoryItemQueryService>();

// Access Control Bounded Context
builder.Services.AddScoped<ICredentialRepository, CredentialRepository>();
builder.Services.AddScoped<IStaffMemberRepository, StaffMemberRepository>();
builder.Services.AddScoped<ICredentialCommandService, CredentialCommandService>();
builder.Services.AddScoped<ICredentialQueryService, CredentialQueryService>();
builder.Services.AddScoped<IStaffMemberQueryService, StaffMemberQueryService>();

// Rooms Bounded Context
builder.Services.AddScoped<IRatePlanRepository, RatePlanRepository>();
builder.Services.AddScoped<IDailyRateRepository, DailyRateRepository>();
builder.Services.AddScoped<IRatePlanCommandService, RatePlanCommandService>();
builder.Services.AddScoped<IRatePlanQueryService, RatePlanQueryService>();
builder.Services.AddScoped<IPropertyRepository, PropertyRepository>();
builder.Services.AddScoped<IPropertyQueryService, PropertyQueryService>();

var app = builder.Build();

// Verify the database exists and create it with its tables when it does not
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    context.Database.EnsureCreated();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

// Apply CORS Policy
app.UseCors("AllowAllPolicy");

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
