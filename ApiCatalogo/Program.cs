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
using Microsoft.AspNetCore.OData;
using ApiCatalogo.GraphQL;

var builder = WebApplication.CreateBuilder(args);
var mappingConfig = new MapperConfiguration(mc =>
{
    mc.AddProfile(new MappingProfile());
});
IMapper mapper = mappingConfig.CreateMapper();

// Add services to the container.
builder.Services.AddScoped<AppiLogginFilter>();

//Adicionando Middleware Mapper para execução do AutoMapper
builder.Services.AddSingleton(mapper);

builder.Services.AddControllers().AddJsonOptions(options => 
options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    //Habilitando documentação no Swagger
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "Api Catalogo",
        Description = "An ASP.NET Core Web API catalago de produtos"
    });

    //Gerar documentação no Swagger a partir dos comentários da App
    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));

    //Definições de seguranção para autenticação usando o Bearer para exibir no Swagger
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
 
//Pegando a string de conexão do banco de dados no arquivo de configuração.
string? mySqlConnection = builder.Configuration.GetConnectionString("DefaultConnection");

//Fazendo a conexão com o banco de dados e inserido no Context para executar acessos ao banco de dados.
builder.Services.AddDbContext<AppDbContext>(options =>
options.UseMySql(mySqlConnection,
ServerVersion.AutoDetect(mySqlConnection)));

builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

//Adicionando o Middleware para criar a UnitOfWork para encapsular todos os métodos CRUD da App
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();


//Adicionando o Middleware de autenticação na App usando Jwt
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

//Adicionando o Middleware para controle de versão da API
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

//Adicionando o Middleware CORS para possibilitar acesso externo à API
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



builder.Services.AddControllers().AddOData(options => options.Select().Filter().OrderBy().Count().Expand());


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

//infelizmente esta dando um conflito com a UnitOfWork e não estou conseguindo resolver o problema.
//app.UseMiddleware<TesteGraphQLMiddleware>();
app.ConfigureExceptionHandler();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseCors("EnableCORS");
app.UseAuthorization();
app.MapControllers();

app.Run();
