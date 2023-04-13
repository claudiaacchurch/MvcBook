using Microsoft.EntityFrameworkCore;
using MvcBook.Models;
using MvcBook.Data;
using Microsoft.AspNetCore.Identity;
using SendGrid;
using SendGrid.Helpers.Mail;

var apiKey = Environment.GetEnvironmentVariable("SENDGRID_API_KEY");
var client = new SendGridClient(apiKey);
var msg = new SendGridMessage()
{
    From = new EmailAddress("claudiachurch00@gmail.com", "ClaudiaBookStore"),
    Subject = "Twilio SendGrid",
    PlainTextContent = "practice"
};
msg.AddTo(new EmailAddress("claudia.church@exasoftgroup.com", "practice"));
var response = await client.SendEmailAsync(msg);

// A success status code means SendGrid received the email request and will process it.
// Errors can still occur when SendGrid tries to send the email. 
// If email is not received, use this URL to debug: https://app.sendgrid.com/email_activity 
Console.WriteLine(response.IsSuccessStatusCode ? "Email queued successfully!" : "Something went wrong!");

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<MvcBookContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("MvcBookContext") ?? throw new InvalidOperationException("Connection string 'MvcBookContext' not found.")));

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddRoles<IdentityRole>() // Add this line to include roles
    .AddEntityFrameworkStores<MvcBookContext>();
// Add services to the container.
builder.Services.AddControllersWithViews();

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

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

app.Run();
