using Webdev_project.Data;
using Webdev_project.Interfaces;

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



// Add services to the container.
builder.Services.AddControllersWithViews();

// Add users repository
builder.Services.AddSingleton<AuthenticationRepository>();

// Register the Services 
builder.Services.AddScoped<IUserRepository, AuthenticationRepository>();
builder.Services.AddScoped<ISessionRepository, SessionRepository>();

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

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");



app.Run();
