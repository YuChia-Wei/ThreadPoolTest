using Microsoft.AspNetCore.Mvc;
using ThreadsPoolTest.SetMinThreadsPool.Models;
using ThreadsPoolTest.SetMinThreadsPool.Services;

ThreadPool.GetMinThreads(out var workerThreads, out var completionPortThreads);
ThreadPool.SetMinThreads(128, 128);

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
