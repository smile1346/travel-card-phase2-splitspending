using Microsoft.EntityFrameworkCore;
using SplitManagement.Data;
using SplitManagement.Services;
using SplitManagement.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Database Context
builder.Services.AddDbContext<SplitDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Repository Pattern
builder.Services.AddScoped<ISplitRepository, SplitRepository>();
builder.Services.AddScoped<ITagRepository, TagRepository>();

// Business Services
builder.Services.AddScoped<ISplitService, SplitService>();
builder.Services.AddScoped<ISettlementService, SettlementService>();

// HTTP Clients for other microservices
builder.Services.AddHttpClient("TripService", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["Services:TripService:BaseUrl"]);
});

builder.Services.AddHttpClient("TransactionService", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["Services:TransactionService:BaseUrl"]);
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin",
        policy =>
        {
            policy.AllowAnyOrigin() // 🔒 Allow only this domain
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});

var app = builder.Build();
app.UseCors("AllowSpecificOrigin");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();