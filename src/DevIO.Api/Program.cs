using DevIO.Api.Configuration;
using Microsoft.AspNetCore.Mvc.ApiExplorer;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddIdentityConfiguration(builder.Configuration);

builder.Services.ResolveDependencies(builder.Configuration);

builder.Services.AddWebApiConfig();

builder.Services.AddSwaggerConfig();

builder.Services.AddLoggingConfig(builder.Configuration);

var app = builder.Build();

app.UseApplicationStartupConfig(app.Environment);

var apiVersionDescriptionProvider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();

app.UseSwaggerConfig(app.Environment, apiVersionDescriptionProvider);

app.UseLoggingConfiguration();

app.Run();

