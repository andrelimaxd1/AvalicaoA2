using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using BibliotecaApi.Data;
using BibliotecaApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseSqlite("Data Source=Andre_Ana.db"));

builder.Services.Configure<Microsoft.AspNetCore.Http.Json.JsonOptions>(opt =>
    opt.SerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);

var app = builder.Build();


//  Cadastro com Validação de Duplicado

app.MapPost("/api/livro/cadastrar", (Livro livro, AppDbContext db) =>
{
    
    if (db.Livros.Any(l => l.Nome.ToLower() == livro.Nome.ToLower()))
        return Results.BadRequest(new { mensagem = "Já existe um livro cadastrado com este nome." });

    livro.Disponivel = true; 
    db.Livros.Add(livro);
    db.SaveChanges();
    return Results.Created($"/api/livro/buscar/{livro.Nome}", livro);
});


// Listar (com retorno 404 se vazio)

app.MapGet("/api/livro/listar", (AppDbContext db) =>
{
    var livros = db.Livros.ToList();
    return livros.Count > 0 ? Results.Ok(livros) : Results.NotFound();
});


//  Buscar por Nome

app.MapGet("/api/livro/buscar/{nome}", (string nome, AppDbContext db) =>
{
    var livro = db.Livros.FirstOrDefault(l => l.Nome.ToLower() == nome.ToLower());
    return livro is null ? Results.NotFound() : Results.Ok(livro);
});


// Emprestar

app.MapPut("/api/livro/emprestar/{id}", (int id, AppDbContext db) =>
{
    var livro = db.Livros.Find(id);
    if (livro is null) return Results.NotFound();
    if (!livro.Disponivel) return Results.BadRequest(new { mensagem = "Livro indisponível." });

    livro.Disponivel = false;
    db.SaveChanges();
    return Results.Ok(new { mensagem = "Livro emprestado com sucesso." });
});


//  Devolver

app.MapPut("/api/livro/devolver/{id}", (int id, AppDbContext db) =>
{
    var livro = db.Livros.Find(id);
    if (livro is null) return Results.NotFound();
    if (livro.Disponivel) return Results.BadRequest(new { mensagem = "Livro não está emprestado." });

    livro.Disponivel = true;
    db.SaveChanges();
    return Results.Ok(new { mensagem = "Livro devolvido com sucesso." });
});


//  Listar

app.MapGet("/api/livro/disponiveis", (AppDbContext db) =>
{
    var disponiveis = db.Livros.Where(l => l.Disponivel).ToList();
    return disponiveis.Count > 0 ? Results.Ok(disponiveis) : Results.NotFound();
});

app.MapGet("/api/livro/emprestados", (AppDbContext db) =>
{
    var emprestados = db.Livros.Where(l => !l.Disponivel).ToList();
    return emprestados.Count > 0 ? Results.Ok(emprestados) : Results.NotFound();
});

app.Run();