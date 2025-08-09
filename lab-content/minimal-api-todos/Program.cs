using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// TODO: Add your in-memory todos repository here
var todos = new List<Todo>
{
    new(1, "Learn C#", "open"),
    new(2, "Build an API", "open"), 
    new(3, "Deploy to production", "done")
};

// TODO: Implement the GET /api/todos endpoint here
// Requirements:
// 1. Accept optional 'status' query parameter 
// 2. Filter todos by status if provided
// 3. Valid status values: 'all', 'open', 'done'
// 4. Return 400 for invalid status values
// 5. Return all todos when status is 'all' or not provided

app.Run();

public record Todo(int Id, string Title, string Status);