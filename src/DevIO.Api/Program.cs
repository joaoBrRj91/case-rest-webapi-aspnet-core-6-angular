using DevIO.Api.Configuration;
using DevIO.Data.Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<MeuDbContext>(option =>
{
    option.UseSqlServer(builder.Configuration.GetConnectionString("DefaultSqlServerConnection"));
});

builder.Services.ResolveDependencies();

builder.Services.AddWebApiConfig();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseApplicationStartupConfig();

