using Microsoft.AspNetCore.Mvc;
using AgroChainSync.Api.Services;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text;
using AgroChainSync.Api.Configurations;

namespace AgroChainSync.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ChatbotWebhookController : ControllerBase
    {
        private readonly ChatbotService _chatbotService;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ZApiConfiguracao _zapi;
        private readonly ICacheMensagensProcessadas _cache;

        public ChatbotWebhookController(
            ChatbotService chatbotService,
            IHttpClientFactory httpClientFactory,
            IOptions<ZApiConfiguracao> zapiOptions,
            ICacheMensagensProcessadas cache)
        {
            _chatbotService = chatbotService;
            _httpClientFactory = httpClientFactory;
            _zapi = zapiOptions.Value;
            _cache = cache;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] JsonElement mensagem)
        {
            try
            {
                // Loga a mensagem recebida
                var entrada = mensagem.ToString();
                await System.IO.File.AppendAllTextAsync("logs_webhook.txt", $"[{DateTime.Now}] {entrada}\n");

                // Processa em segundo plano para responder rápido à Z-API
                _ = Task.Run(async () =>
                {
                    try
                    {
                        // Se a mensagem foi enviada pelo próprio número, ignoramos (evita loop)
                        var fromMe = mensagem.GetProperty("fromMe").GetBoolean();
                        if (fromMe)
                            return;

                        // Extrai dados da mensagem
                        var texto = mensagem.GetProperty("text").GetProperty("message").GetString();
                        var numero = mensagem.GetProperty("phone").GetString();
                        var messageId = mensagem.GetProperty("messageId").GetString();

                        // Se a mensagem já foi processada (mesmo ID), ignorar
                        if (_cache.JaProcessada(messageId))
                            return;

                        // Marca como processada
                        _cache.MarcarComoProcessada(messageId);

                        // Processa o comando
                        string resposta = await _chatbotService.ProcessarComando(texto, numero);

                        // Monta a requisição para enviar via Z-API
                        var httpClient = _httpClientFactory.CreateClient();
                        var url = $"https://api.z-api.io/instances/{_zapi.InstanceId}/token/{_zapi.Token}/send-text";
                        var payload = new { phone = numero, message = resposta };

                        var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
                        content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                        content.Headers.Add("Client-Token", _zapi.ClientToken);

                        await httpClient.PostAsync(url, content);
                    }
                    catch (Exception ex2)
                    {
                        await System.IO.File.AppendAllTextAsync("logs_webhook.txt", $"[ERRO INTERNO {DateTime.Now}] {ex2.Message}\n");
                    }
                });

                return Ok(); // Retorna OK rápido para a Z-API
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro no webhook: {ex.Message}");
            }
        }
    }
}
