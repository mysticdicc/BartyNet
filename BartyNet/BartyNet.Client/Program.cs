using BartyLib.Classes.Images;
using BartyLib.Classes.Posts;
using BartyNet.Client.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor;
using MudBlazor.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.Services.AddMudServices();

builder.Services.AddAuthorizationCore();
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddAuthenticationStateDeserialization();

var baseAddress = new Uri(builder.Configuration.GetValue<string>("ApiBaseAddress")!);

builder.Services.AddScoped(sp =>
{
    NavigationManager navigation = sp.GetRequiredService<NavigationManager>();
    return new HttpClient { BaseAddress = baseAddress };
});
builder.Services.AddHttpClient();


SharedServices.Register(builder.Services, baseAddress);

await builder.Build().RunAsync();
