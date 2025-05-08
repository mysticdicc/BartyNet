using BartyLib.Classes.Images;
using BartyLib.Classes.Posts;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor;
using MudBlazor.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.Services.AddMudServices();

builder.Services.AddAuthorizationCore();
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddAuthenticationStateDeserialization();

builder.Services.AddScoped(sp =>
{
    NavigationManager navigation = sp.GetRequiredService<NavigationManager>();
    return new HttpClient { BaseAddress = new Uri(builder.Configuration.GetValue<string>("ApiBaseAddress")!) };
});

builder.Services.AddTransient<ImageAPI>();
builder.Services.AddTransient<WebsitePostAPI>();

await builder.Build().RunAsync();
