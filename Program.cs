using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using vCashBlazorDemo;
using Blazored.LocalStorage;
using vCash.Data.Repositories;
using vCashBlazorDemo.Services;
using vCashBlazorDemo.Services.Local;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

// Persistence
builder.Services.AddBlazoredLocalStorage();

// Repositories (Local Mock)
builder.Services.AddScoped<IGuestRepository, LocalGuestRepository>();
builder.Services.AddScoped<ITransactionRepository, LocalTransactionRepository>();
builder.Services.AddScoped<IDispenserRepository, LocalDispenserRepository>();

// Services
builder.Services.AddScoped<RiskService>();
builder.Services.AddScoped<NotificationService>(); // Stub
builder.Services.AddScoped<ImageGenerationService>(); // Stub
builder.Services.AddScoped<HardwareService>();
builder.Services.AddScoped<GuestService>();
builder.Services.AddScoped<TransactionService>();

await builder.Build().RunAsync();
