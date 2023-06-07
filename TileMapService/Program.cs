using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.FileProviders;
using Serilog;
using TileMapService.Repositorys;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        var conf = builder.Configuration;

        builder.Host.UseSerilog((context, services, configuration) => configuration
            .ReadFrom.Configuration(context.Configuration)
            .ReadFrom.Services(services)
            .Enrich.FromLogContext());


        builder.Services.AddCors(options => { options.AddPolicy("AllowAllHeaders", builder => { builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod(); }); });
        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        builder.Services.AddSingleton(new MBTilesTileSource(conf));

        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseDefaultFiles();
        app.UseStaticFiles();

        var provider = new FileExtensionContentTypeProvider();
        provider.Mappings[".pbf"] = "application/x-protobuf";

        app.UseStaticFiles(new StaticFileOptions
        {
            FileProvider = new PhysicalFileProvider(Path.Combine(builder.Environment.ContentRootPath, "wwwroot\\fonts\\Bold")),
            RequestPath = "/fonts/Bold",
            ContentTypeProvider = provider
        });
        app.UseStaticFiles(new StaticFileOptions
        {
            FileProvider = new PhysicalFileProvider(Path.Combine(builder.Environment.ContentRootPath, "wwwroot\\fonts\\Italic")),
            RequestPath = "/fonts/Italic",
            ContentTypeProvider = provider
        });
        app.UseStaticFiles(new StaticFileOptions
        {
            FileProvider = new PhysicalFileProvider(Path.Combine(builder.Environment.ContentRootPath, "wwwroot\\fonts\\Regular")),
            RequestPath = "/fonts/Regular",
            ContentTypeProvider = provider
        });



        app.UseCors("AllowAllHeaders");
        app.UseAuthorization();

        app.MapControllers();

        app.UseSerilogRequestLogging();

        app.Run();
    }
}