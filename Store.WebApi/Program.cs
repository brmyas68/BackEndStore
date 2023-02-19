using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Store.DataLayer.Context;
using Store.InterfaceService.InterfacesBase;
using Store.Service.ServiceBase;
using System.Reflection;
using System.Text;

var builder = WebApplication.CreateBuilder(args);



var SQlSTORE = builder.Configuration.GetConnectionString("AppDbSTORE");
builder.Services.AddDbContext<ContextStore>(x => x.UseSqlServer(SQlSTORE,
    sqlServerOptionsAction: sqlOptions => { sqlOptions.EnableRetryOnFailure(); }).UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking));

builder.Services.AddAutoMapper(Assembly.GetExecutingAssembly());

builder.Services.AddScoped<IUnitOfWorkStoreService, UnitOfWorkStoreService>();


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowOrigin",
        builder => builder
           //.SetIsOriginAllowed( _ => true)
           //.AllowAnyOrigin()
           .WithOrigins("http://localhost:3000", "http://localhost:3001", "https://localhost:3001")
          .AllowAnyMethod()
          .AllowAnyHeader()
          .AllowCredentials()
    .Build());
});


builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Version = "v1", Title = "MicroService Web API" });
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization ",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement()
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type=ReferenceType.SecurityScheme,
                                Id="Bearer"

                            },
                        },
                        new String[]{}
                    }
                });
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
    };
});



var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "MicroService Web API");

    });

}

app.UseDeveloperExceptionPage();
app.UseCors("AllowOrigin");

app.UseHttpsRedirection();
app.MapControllers();
app.UseAuthentication();
app.UseAuthorization();

// app.MapGet("/", () => "Hello World!");

app.Run();
