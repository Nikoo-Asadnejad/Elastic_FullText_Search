using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Nest;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();



var settings = new ConnectionSettings(new Uri("http://localhost:9200"))
    .DefaultIndex("products");

var client = new ElasticClient(settings);

var indexExists = client.Indices.Exists("products").Exists;

if (!indexExists)
{
    var createIndexResponse = client.Indices.Create("products", c => c
        .Settings(s => s
            .Analysis(a => a
                .Analyzers(analyzers => analyzers
                    .Custom("my_custom_analyzer", ca => ca
                        .Tokenizer("standard")
                        .Filters("lowercase", "stop", "snowball")
                    )
                )
            )
        )
        .Map<Product>(m => m
            .Properties(p => p
                .Text(t => t
                        .Name(n => n.Name)
                        .Analyzer("my_custom_analyzer")  
                )
                .Text(t => t
                        .Name(n => n.Description)
                        .Analyzer("my_custom_analyzer")  
                )
                .Number(n => n
                    .Name(n => n.Price)
                    .Type(NumberType.Float)
                )
            )
        )
    );
    
    if (!createIndexResponse.IsValid)
    {
        throw new Exception("Index creation failed: " + createIndexResponse.ServerError.Error.Reason);
    }
}

builder.Services.AddSingleton<IElasticClient>(sp => client);

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapPost("/index",async  ([FromServices] IElasticClient elasticClient , [FromBody] Product product) =>
{
    var response = await elasticClient.IndexDocumentAsync(product);
    if (response.IsValid)
    {
        return Results.Ok(new { message = "Product indexed successfully." });
    }
    return Results.Conflict(response.OriginalException.ToString());
});

app.MapGet("/search", async ([FromServices] IElasticClient elasticClient ,[FromQuery]  string query) =>
{
    var searchResponse = await elasticClient.SearchAsync<Product>(s => s
        .Query(q => q
            .MultiMatch(m => m
                .Query(query)
                .Fields(f => f
                    .Field(p => p.Name)
                    .Field(p => p.Description)
                )
            )
        )
    );

    return Results.Ok(searchResponse.Documents.ToList());
});

app.Run();

public record Product
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public decimal Price { get; set; }
}

