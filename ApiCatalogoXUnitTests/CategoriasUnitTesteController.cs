
using ApiCatalogo.Context;
using ApiCatalogo.Controllers;
using ApiCatalogo.DTOs;
using ApiCatalogo.DTOs.Mappings;
using ApiCatalogo.Pagination;
using ApiCatalogo.Repository;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Build.Framework;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace ApiCatalogoXUnitTests;

public class CategoriasUnitTesteController
{
    private IMapper mapper;
    private IUnitOfWork repository;
    public static string connectionString = "Server=localhost;Database=CatalogoBD;Uid=root;Pwd=root;";

    public static DbContextOptions<AppDbContext> dbContextOptions { get; }

    static CategoriasUnitTesteController()
    {
        dbContextOptions = new DbContextOptionsBuilder<AppDbContext>()
            .UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)).Options;
    }

    public CategoriasUnitTesteController()
    {
        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile(new MappingProfile());
        });
        mapper = config.CreateMapper();

        var context = new AppDbContext(dbContextOptions);

        //DBUnitTestsMockInitializer db = new DBUnitTestsMockInitializer();
        //db.Seed(context);

        repository = new UnitOfWork(context);
    }
    //teste unitários

    [Fact]
    public async void GetCategorias_Return_OkResult()
    {
        //Arrange
        var controller = new CategoriasController(repository, mapper);

        //Act
        var param = new CategoriasParemeters();
        var data = await controller.Get(param);

        Assert.IsType<List<CategoriaDTO>>(data.Value);
    }
}
