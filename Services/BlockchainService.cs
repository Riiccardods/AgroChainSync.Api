using System;
using System.IO;
using System.Linq;

namespace AgroChainSync.Api.Services
{
    public class BlockchainService
    {
        private readonly string _caminhoLog = "blockchain_log.txt";

        // Classe interna usada para representar o registro de blockchain (hash + data)
        public class RegistroBlockchain
        {
            public string Hash { get; set; } = string.Empty;
            public DateTime DataRegistro { get; set; }
        }

        // ✅ Simula a leitura de um hash da "blockchain" (arquivo de log)
        public RegistroBlockchain? BuscarLogPorContrato(int contratoId)
        {
            if (!File.Exists(_caminhoLog))
                return null;

            var linhas = File.ReadAllLines(_caminhoLog);

            foreach (var linha in linhas.Reverse())
            {
                if (linha.Contains($"ContratoId: {contratoId}"))
                {
                    var partes = linha.Split("|");
                    var data = DateTime.Parse(partes[0].Trim());
                    var hash = partes[2].Replace("Hash:", "").Trim();

                    return new RegistroBlockchain
                    {
                        Hash = hash,
                        DataRegistro = data
                    };
                }
            }

            return null;
        }

        // ✅ Simula o registro de um contrato na "blockchain" gerando um hash único
        public void RegistrarHashNaBlockchain(int contratoId)
        {
            string hashSimulado = Guid.NewGuid().ToString("N"); // Gera um hash aleatório tipo blockchain
            string dataAtual = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            string linha = $"{dataAtual} | ContratoId: {contratoId} | Hash: {hashSimulado}";

            File.AppendAllText(_caminhoLog, linha + Environment.NewLine);
        }
    }
}
