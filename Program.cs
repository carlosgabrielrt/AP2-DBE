using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddDbContext<AppDbContext>(options 
=> options.UseSqlite("Data Source=Filme.db"));



var app = builder.Build();


using (var scope = app.Services.CreateAsyncScope())
{
    var db = scope.ServiceProvider.GetRequiredService< AppDbContext> ();
    db.Database.EnsureCreated();
    
}

//OS Get/////////

app.MapGet("/status", () => new
{
    status = "online",
    mensagem = "API Funcionando",
    dataHora = DateTime.Now
});



app.MapGet("/filme", async (AppDbContext db) => 
{
    var filmes = await db.Filme.ToListAsync();
     return Results.Ok(filmes);
});


app.MapGet("/filme/{id:int}", async (int id, AppDbContext db)  => 
{
    var filmes = await db.Filme.FindAsync(id);

    if(filmes == null){
        return Results.NotFound();
    }
    return Results.Ok(filmes);

});

app.MapGet("/filme/busca/{titulo}", async(string titulo, AppDbContext db)=>{

    var filme = await db.Filme.FirstOrDefaultAsync(f => f.Titulo == titulo);
    if(filme == null)
        return Results.NotFound();
    
    return Results.Ok(filme);
});



///o Post//////

app.MapPost("/filme", async (Filme filmeReq, AppDbContext db) =>
{
   db.Filme.Add(filmeReq);
   await db.SaveChangesAsync();
   return Results.Created($"/filme/{filmeReq.Id}",filmeReq);

});

//////O put////

app.MapPut("/filme/{id:int}", async (int id, Filme filmeAtualizado, AppDbContext db)=>{

    var filme = await db.Filme.FindAsync(id);

    if(filme == null)

        return Results.NotFound();
    
    filme.Titulo = filmeAtualizado.Titulo;
    filme.Genero = filmeAtualizado.Genero;
    filme.lancamento = filmeAtualizado.lancamento;
    filme.Disponivel = filmeAtualizado.Disponivel;

    await db.SaveChangesAsync();

    return Results.Ok(filme);
});

//// O Delete/////

    app.MapDelete("/filme/{id:int}", async (int id, AppDbContext db)=>{
        
        var filme = await db.Filme.FindAsync(id);

        if(filme == null)
        return Results.NotFound();

        db.Remove(filme);
        await db.SaveChangesAsync();
        return Results.NoContent();


});


app.Run();


