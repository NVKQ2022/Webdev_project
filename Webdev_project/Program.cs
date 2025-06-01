using Webdev_project.Data;
using Webdev_project.Interfaces;
using Webdev_project.Models;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureKestrel(serverOptions =>
{
    // serverOptions.ListenAnyIP(7264); // Listen on all IPs, port 5000
    serverOptions.ListenAnyIP(7264, listenOptions =>
    {
        listenOptions.UseHttps();
    });
});

builder.Configuration.AddEnvironmentVariables();

// Add a configure class 
builder.Services.Configure<MongoDbSettings>(
    builder.Configuration.GetSection("MongoDbSettings"));


// Add services to the container.
builder.Services.AddControllersWithViews();

// Add users repository
builder.Services.AddSingleton<AuthenticationRepository>();

// Register the Services 
builder.Services.AddScoped<IUserRepository, AuthenticationRepository>();
builder.Services.AddScoped<ISessionRepository, SessionRepository>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<ProductRepository>();


builder.Services.AddSession();

var app = builder.Build();



// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();


app.UseRouting();

app.UseSession();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");



app.Run();
