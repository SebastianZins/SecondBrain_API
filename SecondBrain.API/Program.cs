using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SecondBrain.Database.MongoDB;
using SecondBrain.Database.Neo4j;
using SecondBrain.Models.InternalModels;
using SecondBrain.Repositories.MongoDB;
using SecondBrain.Repositories.Neo4j;
using SecondBrain.Services.Auth;
using SecondBrain.Services.FileStructure;
using SecondBrain.Services.Section;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

var configurations = builder.Configuration;

var settings = new JwtSettings();

builder.Services.Configure<Neo4jSettingsModel>(configurations.GetSection("Neo4j"));
builder.Services.Configure<MongoDbSettingsModel>(configurations.GetSection("MongoDB"));

builder.Services.AddSingleton<Neo4jGraph>();
builder.Services.AddSingleton<FileSectionContext>();
builder.Services.AddSingleton<AttachmentsContext>();

builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<FileStructureService>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<FileService>();
builder.Services.AddScoped<FileSectionDataService>();
builder.Services.AddScoped<FileSectionMetaDataService>();
builder.Services.AddScoped<ListSectionDataService>();

builder.Services.AddScoped<FileStructureRepository>();
builder.Services.AddScoped<UserRepository>();
builder.Services.AddScoped<AttachmentRepository>();
builder.Services.AddScoped<TagRepository>();
builder.Services.AddScoped<FileRepository>();
builder.Services.AddScoped<FileSectionRepository>();
builder.Services.AddScoped<ListSectionRepository>();

builder.Services.AddOptions();
builder.Services.Configure<JwtSettings>(configurations.GetSection("JwtSettings"));
builder.Services.AddMvc();

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add CORS policy#
builder.Services.AddCors(options =>
{
    options.AddPolicy("SecondBrain",
    builder =>
    {
        builder.WithOrigins("http://localhost:4200")
               .AllowAnyHeader()
               .AllowAnyMethod()
               .AllowCredentials();
    });
});


builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
})
    .AddCookie(options =>
    {
        options.Cookie.Name = "SecondBrain-Login";
        options.Cookie.SameSite = SameSiteMode.None;
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        options.Cookie.HttpOnly = true;
        options.Events = new CookieAuthenticationEvents
        {
            OnRedirectToLogin = context =>
            {
                context.Response.StatusCode = 401;
                return Task.CompletedTask;
            },
            OnRedirectToAccessDenied = context =>
            {
                context.Response.StatusCode = 403;
                return Task.CompletedTask;
            }
        };
    })
    .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
{
    options.RequireHttpsMetadata = false;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidIssuer = settings.Issuer,
        ValidAudience = settings.Audience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(settings.Key)),
        ValidateIssuerSigningKey = true,
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero // the default for this setting is 5 minutes
    };
    options.Events = new JwtBearerEvents
    {
        OnAuthenticationFailed = context =>
        {
            if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
            {
                context.Response.Headers.Add("X-Token-Expired", true.ToString().ToLower());
            }
            return Task.CompletedTask;
        }
    };

});

var app = builder.Build();

// Use CORS policy
app.UseCors("SecondBrain");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.Use(async (context, next) =>
{
    if (context.Request.Method == "OPTIONS")
    {
        context.Response.StatusCode = 204;
        return;
    }

    await next();
});

app.MapControllers();

app.Run();
