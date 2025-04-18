using System.Text;
using AgroChainSync.Api.Services;
using Microsoft.Extensions.DependencyInjection; // 🧠 Para criar escopo manual
using System.Threading.Tasks;

namespace AgroChainSync.Api.Services
{
    public class ChatbotService
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public ChatbotService(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        // ✅ Entrada principal: processa o texto recebido e decide a ação
        public async Task<string> ProcessarComando(string texto, string numero)
        {
            texto = texto.ToLower().Trim();

            if (texto.StartsWith("comprovante "))
            {
                string cpf = texto.Replace("comprovante", "").Trim();
                return await GerarComprovante(cpf);
            }

            if (texto.StartsWith("listar "))
            {
                string cpf = texto.Replace("listar", "").Trim();
                return await ListarContratos(cpf);
            }

            if (texto == "ajuda")
            {
                return "📋 *Comandos disponíveis (versão 2):*\n\n" +
                       "🔎 *comprovante CPF* → Gera comprovante do contrato\n" +
                       "📋 *listar CPF* → Lista todos os contratos do cliente\n" +
                       "ℹ️ *ajuda* → Lista os comandos disponíveis\n";
            }

            return "🤖 Desculpe, não entendi o comando. Envie *ajuda* para ver os comandos disponíveis.";
        }

        // ✅ Gera comprovante com hash blockchain
        private async Task<string> GerarComprovante(string cpf)
        {
            using var scope = _scopeFactory.CreateScope();
            var contratoService = scope.ServiceProvider.GetRequiredService<ContratoService>();
            var blockchainService = scope.ServiceProvider.GetRequiredService<BlockchainService>();

            var contrato = await contratoService.BuscarPorCpf(cpf);

            if (contrato == null)
                return $"❌ Nenhum contrato encontrado para o CPF {cpf}.";

            var comprovante = blockchainService.BuscarLogPorContrato(contrato.Id);

            if (comprovante == null)
                return $"⚠️ Contrato encontrado, mas sem registro na blockchain.";

            return $"📄 *Comprovante do Contrato:*\n\n" +
                   $"🧾 *Nome:* {contrato.NomeCliente}\n" +
                   $"🆔 *CPF:* {contrato.Cpf}\n" +
                   $"🔐 *Hash blockchain:* {comprovante.Hash}\n" +
                   $"📅 *Registrado em:* {comprovante.DataRegistro:dd/MM/yyyy HH:mm}";
        }

        // ✅ Lista todos os contratos do CPF informado
        private async Task<string> ListarContratos(string cpf)
        {
            using var scope = _scopeFactory.CreateScope();
            var contratoService = scope.ServiceProvider.GetRequiredService<ContratoService>();

            var contratos = await contratoService.BuscarTodosPorCpf(cpf);

            if (contratos == null || contratos.Count == 0)
                return $"❌ Nenhum contrato encontrado para o CPF {cpf}.";

            var resposta = new StringBuilder();
            resposta.AppendLine($"📋 *Lista de Contratos para CPF {cpf}:*\n");

            foreach (var c in contratos)
            {
                resposta.AppendLine($"🧾 *Máquina:* {c.DescricaoMaquina}");
                resposta.AppendLine($"📅 *Início:* {c.DataInicio:dd/MM/yyyy}");
                resposta.AppendLine($"📅 *Fim:* {c.DataFim:dd/MM/yyyy}");
                resposta.AppendLine($"🔄 *Renova automaticamente:* {(c.RenovacaoAutomatica ? "Sim" : "Não")}");
                resposta.AppendLine();
            }

            return resposta.ToString();
        }
    }
}