using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Azure;
using MudBlazor.Services;
using PoPhotoAlbum.Components;
using PoPhotoAlbum.Components.Account;
using PoPhotoAlbum.Data;
using Microsoft.AspNetCore.Components.Server.Circuits;

var builder = WebApplication.CreateBuilder(args);


var connectionString = builder.Configuration.GetConnectionString("SqliteConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
     options.UseSqlite(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();


builder.Services.AddIdentityCore<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddSignInManager()
    .AddDefaultTokenProviders();

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<IdentityUserAccessor>();
builder.Services.AddScoped<IdentityRedirectManager>();
builder.Services.AddScoped<AuthenticationStateProvider, IdentityRevalidatingAuthenticationStateProvider>();

builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = IdentityConstants.ApplicationScheme;
    options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
})
    .AddIdentityCookies();




builder.Services.AddAuthentication().AddGoogle(googleOptions =>
{
    googleOptions.ClientId = "413108831110-237pt74ek6jk7mau4av5div4qj3gqknj.apps.googleusercontent.com";
    googleOptions.ClientSecret = "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";
});

var testString = builder.Configuration["AzureBlobStorage:ConnectionString"];
Console.WriteLine(testString);

builder.Services.AddAzureClients(clientBuilder =>
{
    clientBuilder.AddBlobServiceClient(testString);
});

builder.Services.AddMudServices();
builder.Services.AddScoped<IPhotoService, PhotoService>();
builder.Services.AddSingleton<IEmailSender<ApplicationUser>, IdentityNoOpEmailSender>();

builder.Services.AddServerSideBlazor(options => options.DetailedErrors = true);
builder.Services.AddApplicationInsightsTelemetry(builder.Configuration["APPLICATIONINSIGHTS_CONNECTION_STRING"]);

// builder.Services.AddSignalR(e =>
//   {
//       e.MaximumReceiveMessageSize = 1024000; // Set the limit here (in bytes)
//   });


// builder.Services.AddServerSideBlazor(options =>
// {
//     options.MaximumReceiveMessageSize = 4 * 1024 * 1024; // 4MB
// });



var app = builder.Build();












// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.MapAdditionalIdentityEndpoints();

app.Run();





