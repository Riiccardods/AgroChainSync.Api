using AgroChainSync.Api.Data;
using AgroChainSync.Api.Services;
using AgroChainSync.Api.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

// 🔧 Ativa controllers e Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// 🔌 Conexão com o MySQL
builder.Services.AddDbContext<AgroContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("ConexaoSQL"),
        new MySqlServerVersion(new Version(8, 0, 21))
    )
);

// 💉 Injeção de dependência dos serviços
builder.Services.AddScoped<ContratoService>();
builder.Services.AddScoped<BlockchainService>();

// ⚠️ Agora como Singleton, pois ele cria escopo manual
builder.Services.AddSingleton<ChatbotService>();

// 🧠 Cache de mensagens processadas (evita duplicação e loop infinito)
builder.Services.AddSingleton<ICacheMensagensProcessadas, CacheMensagensProcessadas>();

// 🌐 Suporte a chamadas HTTP externas (usado pela Z-API)
builder.Services.AddHttpClient();

// ⚙️ Configuração da Z-API
builder.Services.Configure<ZApiConfiguracao>(
    builder.Configuration.GetSection("ZApi")
);

// 🌍 Libera CORS pra testes e integração externa
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader());
});

// 🔐 Autorização (caso queira adicionar autenticação no futuro)
builder.Services.AddAuthorization();

var app = builder.Build();

// 📊 Ativa Swagger sempre
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "AgroChainSync.API v1");
    c.RoutePrefix = "swagger";
});

// 🚀 Pipeline padrão
app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthorization();
app.MapControllers();

app.Run();
