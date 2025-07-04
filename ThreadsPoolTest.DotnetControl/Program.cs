using Microsoft.AspNetCore.Mvc;
using ThreadsPoolTest.DotnetControl.Models;
using ThreadsPoolTest.SetMinThreadsPool.Services;
using ThreadsPoolTest.UseCases.Files;
using FileBll = ThreadsPoolTest.DotnetControl.Services.FileBll;
using FileService = ThreadsPoolTest.UseCases.Files.Services.FileService;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IFileService, FileService>();
builder.Services.AddScoped<FileBll>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapPost("/upload/fromform", async ([FromForm] UploadFileRequest request, [FromServices] FileBll fileBll) =>
   {
       await fileBll.UploadSingleFile(request);
   })
   .WithName("upload from form")
   .DisableAntiforgery()
   .WithOpenApi();

app.Run();