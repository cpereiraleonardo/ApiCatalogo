using ApiCatalogo.Context;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using Microsoft.OpenApi.Models;
using System.Reflection;
using ApiCatalogo.Filter;
using ApiCatalogo.Extensions;
using ApiCatalogo.Repository;
using AutoMapper;
using ApiCatalogo.DTOs.Mappings;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
var mappingConfig = new MapperConfiguration(mc =>
{
    mc.AddProfile(new MappingProfile());
});
IMapper mapper = mappingConfig.CreateMapper();

// Add services to the container.
builder.Services.AddScoped<AppiLogginFilter>();

//Adicionando Middleware Mapper para execu��o do AutoMapper
builder.Services.AddSingleton(mapper);

builder.Services.AddControllers().AddJsonOptions(options => 
options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    //Habilitando documenta��o no Swagger
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "Api Catalogo",
        Description = "An ASP.NET Core Web API catalago de produtos"
    });

    //Gerar documenta��o no Swagger a partir dos coment�rios da App
    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));

    //Defini��es de seguran��o para autentica��o usando o Bearer para exibir no Swagger
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT Authorization header using the Bearer scheme.\r\n\r\n Enter " +
                      "'Bearer'[space] and then your token in the text input below." +
                      "\r\n\r\nExample: \"Bearer 12345abcdef\"",
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    Array.Empty<string>()
                }
            });
});
 
//Pegando a string de conex�o do banco de dados no arquivo de configura��o.
string? mySqlConnection = builder.Configuration.GetConnectionString("DefaultConnection");

//Fazendo a conex�o com o banco de dados e inserido no Context para executar acessos ao banco de dados.
builder.Services.AddDbContext<AppDbContext>(options =>
options.UseMySql(mySqlConnection,
ServerVersion.AutoDetect(mySqlConnection)));

builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

//Adicionando o Middleware para criar a UnitOfWork para encapsular todos os m�todos CRUD da App
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

//Adicionando o Middleware de autentica��o na App usando Jwt
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options => 
        options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
           {
               ValidateIssuer = true,
               ValidateAudience = true,
               ValidateLifetime = true,
               ValidAudience = builder.Configuration["TokenConfiguration:Audience"],
               ValidIssuer = builder.Configuration["TokenConfiguration:Issuer"],
               ValidateIssuerSigningKey = true,
               IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
           }
        );

//Adicionando o Middleware para controle de vers�o da API
//builder.Services.AddApiVersioning(o =>
//{
//    o.AssumeDefaultVersionWhenUnspecified = true;
//    o.DefaultApiVersion = new ApiVersion(1, 0);
//    o.ReportApiVersions = true;
//}
//);

//builder.Services.AddVersionedApiExplorer(o =>
//{
//    o.GroupNameFormat = "'v'VVVV";
//    o.SubstituteApiVersionInUrl = true;
// });

//Adicionando o Middlewate para criar o Log em arquivo txt
//builder.Logging.AddProvider(
//    new CustomLoggerProvider(
//        new CustomLoggerProviderConfiguration 
//        {
//        LogLevel = LogLevel.Information
//        }
//    )
// );;

//Adicionando o Middleware CORS para possibilitar acesso externo � API
builder.Services.AddCors(options =>
{
    options.AddPolicy("EnableCORS", b =>
    {
        b.AllowAnyOrigin()
        .AllowAnyMethod()
        .AllowAnyHeader()
        .Build();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "ApiCatalogo");
    });
}
app.ConfigureExceptionHandler();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseCors("EnableCORS");
app.UseAuthorization();
app.MapControllers();
app.Run();
