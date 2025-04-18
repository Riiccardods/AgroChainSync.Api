using System;

namespace AgroChainSync.Api.Models
{
    public class Contrato
    {
        public int Id { get; set; }
        public string NomeCliente { get; set; } = null!;
        public string Cpf { get; set; } = null!;
        public string DescricaoMaquina { get; set; } = null!;
        public DateTime DataInicio { get; set; }
        public DateTime DataFim { get; set; }
        public bool RenovacaoAutomatica { get; set; }
    }
}
