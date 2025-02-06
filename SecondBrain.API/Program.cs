using Neo4jClient;
using SecondBrain.Database.Neo4j;
using SecondBrain.Repositories.Neo4j;
using SecondBrain.Services;

var builder = WebApplication.CreateBuilder(args);

var configurations = builder.Configuration;

builder.Services.Configure<Neo4jSettingsModel>(configurations.GetSection("Neo4j"));
builder.Services.AddSingleton<Neo4jGraph>();
builder.Services.AddScoped<FileNodeService>();
builder.Services.AddScoped<FileNodeRepository>();

// Add services to the container.
builder.Services.AddControllers();
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

app.UseAuthorization();

app.MapControllers();

app.Run();
