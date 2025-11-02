using Microsoft.EntityFrameworkCore;
using SimpleRestApi.Data;

var builder = WebApplication.CreateBuilder(args);

// Database config
var connectionString = builder.Configuration["ConnectionStrings:SimpleRestApiConnection"];
builder.Services.AddDbContext<SimpleRestApiContext>
(
    options => options.UseSqlServer(connectionString)
);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.Run();