using Microsoft.AspNetCore.Mvc;
using searchEngineWebApp;
using searchEngineWebApp.Model;
using searchEngineWebApp.Service;
using UglyToad.PdfPig.Content;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddSingleton<ArticlaData>();
builder.Services.AddTransient<IIndexing, Indexing>();



var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();


app.MapPost("/Index", ([FromBody]IndexRequest request, IIndexing indexing) =>
{
    indexing.IndexData(request.Path);
});
app.MapGet("/Index", ([FromQuery] string word, IIndexing indexing) =>
{
    return  Results.Ok(indexing.Search(word));
});

app.Run();
