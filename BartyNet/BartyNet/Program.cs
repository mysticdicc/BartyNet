using BartyNet.Client.Pages;
using BartyNet.Components;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Components;
using MudBlazor.Services;
using Microsoft.Identity.Web;
using Microsoft.EntityFrameworkCore;
using BartyNet.Data;
using BartyLib.Classes.Images;
using BartyLib.Classes.Posts;
using MudBlazor;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents()
    .AddAuthenticationStateSerialization();

builder.Services.AddMudServices();
builder.Services.AddControllers();

builder.Services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApp(builder.Configuration.GetSection("EntraStuff"));
builder.Services.AddAuthorization();

builder.Services.AddScoped(sp =>
{
    NavigationManager navigation = sp.GetRequiredService<NavigationManager>();
    return new HttpClient { BaseAddress = new Uri(builder.Configuration.GetValue<string>("ApiBaseAddress")!) };
});
builder.Services.AddHttpClient();

builder.Services.AddDbContextFactory<BartyDb>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("SQL"))
);

builder.Services.AddTransient<ImageAPI>();
builder.Services.AddTransient<WebsitePostAPI>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();


app.UseAntiforgery();
app.MapControllers();
app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(BartyNet.Client._Imports).Assembly);
app.MapGroup("/authentication").MapLoginAndLogout();

app.Run();
