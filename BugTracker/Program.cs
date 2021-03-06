using BugTracker.Data;
using BugTracker.Models;
using BugTracker.Services;
using BugTracker.Services.Factories;
using BugTracker.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

//var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
var connectionString = DataUtility.GetConnectionString(builder.Configuration);
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString, op => op.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery))); // Global Split Queries
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<BTUser>(options => options.SignIn.RequireConfirmedAccount = true)
    //.AddDefaultUI()
    //.AddDefaultTokenProviders()
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddClaimsPrincipalFactory<BTUserClaimsPrincipalFactory>();

builder.Services.AddControllersWithViews();

// Add response compression
builder.Services.AddResponseCompression(configureOptions =>
{
    configureOptions.Providers.Add<BrotliCompressionProvider>();
    configureOptions.Providers.Add<GzipCompressionProvider>();
    configureOptions.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(new[] {"image/svg+xml"});
});

// Reguster our services
builder.Services.AddScoped<IBTRolesService, BTRolesService>();
builder.Services.AddScoped<IBTCompanyInfoService, BTCompanyInfoService>();
builder.Services.AddScoped<IBTProjectService, BTProjectService>();
builder.Services.AddScoped<IBTTicketService, BTTicketService>();
builder.Services.AddScoped<IBTTicketHistoryService, BTTicketHistoryService>();
builder.Services.AddScoped<IBTNotificationService, BTNotificationService>();
builder.Services.AddScoped<IBTInviteService, BTInviteService>();
builder.Services.AddScoped<IBTFileService, BTFileService>();
builder.Services.AddScoped<IBTLookupService, BTLookupService>();

builder.Services.AddScoped<IEmailSender, BTEmailService>();
builder.Services.Configure<MailSettings>(builder.Configuration.GetSection("MailSettings"));


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
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

await DataUtility.ManageDataAsync(app);

app.Run();
