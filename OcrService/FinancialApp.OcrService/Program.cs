using MassTransit;

var builder = WebApplication.CreateBuilder(args);

// 1. ADICIONE ESTA LINHA: Ensina a aplicação a usar Controllers
builder.Services.AddControllers();

// Adiciona o Swagger para podermos testar a API facilmente
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddMassTransit(x =>
{
    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host("localhost", "/", h => {
            h.Username("guest");
            h.Password("guest");
        });
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// 2. ADICIONE ESTA LINHA: Mapeia as rotas para o ReceiptsController que criámos
app.MapControllers();

app.Run();