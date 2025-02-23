using Microsoft.EntityFrameworkCore;
using Negotiations.Models;
using Negotiations.NegotiationManager;
using Products;
using Products.Models;
using Products.ProductManager;
using Scalar.AspNetCore;
using WebApi.SessionManager;

namespace WebApi;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddDbContext<ProductContext>(opt => { opt.UseInMemoryDatabase("ProductList"); });
        builder.Services.AddDbContext<NegotiationContext>(opt => { opt.UseInMemoryDatabase("NegotiationList"); });

        builder.Services.AddScoped<IProductsManager, ProductsManager>();
        builder.Services.AddScoped<INegotiationManager, NegotiationManager>();
        builder.Services.AddSingleton<ISessionManager, SessionManager.SessionManager>();
        builder.Services.AddSingleton<IUserSessionManager, SessionManager.SessionManager>();

        builder.Services.AddControllers();

        builder.Services.AddOpenApi();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
            app.MapScalarApiReference();
        }

        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}