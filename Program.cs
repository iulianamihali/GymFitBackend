using GymFit.Data;
using GymFit.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.OData;

var builder = WebApplication.CreateBuilder(args);

//CORS
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("http://localhost:3000") 
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});


builder.Services.AddControllers()
    .AddOData(options =>
    {
        options
            .Select()
            .Filter()    
            .OrderBy()
            .Expand()
            .Count()
            .SetMaxTop(100);
    });


// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<GymFitContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("GymFitDb")));
//AddScoped bcs AuthService is using DbContext 
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<ClientService>();
builder.Services.AddScoped<TrainerService>();

// how to validate tokens
builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", options =>
    {
        options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(
                System.Text.Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:SecretKey"]))
        };
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors();


app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
