using Microsoft.EntityFrameworkCore;
using MvcBook.Models;
using MvcBook.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using PayPalCheckoutSdk.Core;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDistributedMemoryCache();
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<MvcBookContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("MvcBookContext") ?? throw new InvalidOperationException("Connection string 'MvcBookContext' not found.")));

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddRoles<IdentityRole>() // include roles
    .AddEntityFrameworkStores<MvcBookContext>();

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy =>
       policy.RequireAssertion(context =>
           context.User.Identity != null &&
           context.User.Claims.Any(claim => claim.Type == ClaimTypes.Email && claim.Value == "admin@gmail.com")));

    options.FallbackPolicy = new AuthorizationPolicyBuilder()
    .RequireAuthenticatedUser()
    .Build();
});

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
builder.Services.Configure<SmtpSettings>(builder.Configuration.GetSection("SmtpSettings"));

builder.Services
    .AddFluentEmail(builder.Configuration["SmtpSettings:Username"])
    .AddRazorRenderer()
    .AddSmtpSender(new SmtpClient
    {
        Host = builder.Configuration["SmtpSettings:Host"],
        Port = int.Parse(builder.Configuration["SmtpSettings:Port"]),
        EnableSsl = bool.Parse(builder.Configuration["SmtpSettings:EnableSsl"]),
        Credentials = new NetworkCredential(
            builder.Configuration["SmtpSettings:Username"],
            builder.Configuration["SmtpSettings:Password"]
        )
    });

builder.Services.Configure<PayPalSettings>(builder.Configuration.GetSection("PayPal"));
builder.Services.AddScoped<PayPalEnvironment>(sp =>
{
    var settings = sp.GetRequiredService<IOptions<PayPalSettings>>().Value;
    if (settings.Environment == "sandbox")
    {
        return new SandboxEnvironment(settings.ClientId, settings.ClientSecret);
    }
    else
    {
        return new LiveEnvironment(settings.ClientId, settings.ClientSecret);
    }
});
builder.Services.AddScoped(sp => new PayPalHttpClient(sp.GetRequiredService<PayPalEnvironment>()));
// Add services to the container.
var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    SeedData.Initialize(services);
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
else
{
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.UseSession();

// Default route
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Payment route
app.MapControllerRoute(
    name: "payment",
    pattern: "{controller=Payment}/{action=CreatePayment}/{id?}");


app.MapRazorPages();

app.Run();
