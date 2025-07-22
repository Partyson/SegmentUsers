using System.Text.Json.Serialization;
using Amazon.S3;
using DotNetEnv;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using SegmentUsers.Application.Interfaces;
using SegmentUsers.Application.Services;
using SegmentUsers.Domain.Entities;
using SegmentUsers.Infrastructure.Data;
using Swashbuckle.AspNetCore.Filters;

Env.Load();

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
    });

builder.Services.AddOpenApi();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString,
        b => b.MigrationsAssembly("SegmentUsers.Infrastructure")));

builder.Services.AddScoped<ISegmentService, SegmentService>();
builder.Services.AddScoped<IUserService,UserService>();

var bucketAccessKey = Environment.GetEnvironmentVariable("YC_STORAGE_ACCESS_KEY");
var bucketSecretKey = Environment.GetEnvironmentVariable("YC_STORAGE_SECRET_KEY");
            
var s3Config = new AmazonS3Config
{
    ServiceURL = "https://s3.yandexcloud.net",
    ForcePathStyle = true
};
        
builder.Services.AddSingleton<IAmazonS3>(new AmazonS3Client(bucketAccessKey, bucketSecretKey, s3Config));

builder.Services.AddAuthorization();

builder.Services.AddIdentityApiEndpoints<User>()
    .AddEntityFrameworkStores<AppDbContext>();

builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
    });
    options.OperationFilter<SecurityRequirementsOperationFilter>();
});
    

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(x => x.WithOrigins("http://ui:5002").AllowAnyMethod().AllowAnyHeader().AllowCredentials());

app.MapIdentityApi<User>();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();