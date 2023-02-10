using Mango.Services.Identity.Initializer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Mirchi.Services.Identity;
using Mirchi.Services.Identity.DBContexts;
using Mirchi.Services.Identity.Initialiser;
using Mirchi.Services.Identity.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<ApplicationDBContext>(dbContextOptions =>
dbContextOptions.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddIdentity<ApplicationUser, IdentityRole>().AddEntityFrameworkStores<ApplicationDBContext>().AddDefaultTokenProviders();
var b = builder.Services.AddIdentityServer(options =>
{
    options.Events.RaiseErrorEvents = true;
    options.Events.RaiseInformationEvents = true;
    options.Events.RaiseSuccessEvents = true;
    options.Events.RaiseFailureEvents = true;
    options.EmitStaticAudienceClaim = true;
}).AddInMemoryIdentityResources(SD.IdentityResources).AddInMemoryApiScopes(SD.ApiScopes).AddInMemoryClients(SD.Clients).AddAspNetIdentity<ApplicationUser>();
builder.Services.AddScoped<IDbInitialiser, DbInitialiser>();

b.AddDeveloperSigningCredential();
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
app.UseIdentityServer();
app.UseRouting();

app.UseAuthorization();
using var scope = app.Services.CreateScope();
var db = scope.ServiceProvider.GetRequiredService<IDbInitialiser>();
db.Initialize();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
