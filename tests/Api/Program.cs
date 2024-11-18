using Microsoft.AspNetCore.Authorization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApiPolicies();

var app = builder.Build();
app.Run();
