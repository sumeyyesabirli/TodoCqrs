using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Project.CQRS.Handlers;
using Project.CQRS.Queries;
using Project.Dal;
using Project.Models;

var builder = WebApplication.CreateBuilder(args);
var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .Build();

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddDbContext<CalendarContext>(x =>
{
    x.UseNpgsql(configuration.GetConnectionString("CalendarContext"));
});

builder.Services.TryAddScoped<DbContext, CalendarContext>();
builder.Services.AddScoped<GetEventQueryHandler>();
builder.Services.AddScoped<CreateEventCommandHandler>();
builder.Services.AddScoped<RemoveEventCommandHandler>();
var app = builder.Build();


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();
app.MapControllers();

app.Run();
