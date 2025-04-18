using System.Collections.Concurrent;

namespace AgroChainSync.Api.Services
{
    public class CacheMensagensProcessadas : ICacheMensagensProcessadas
    {
        // Armazena os messageId e a hora que foram processados
        private readonly ConcurrentDictionary<string, DateTime> _mensagens = new();

        // Verifica se a mensagem já foi processada
        public bool JaProcessada(string messageId)
        {
            LimparMensagensAntigas();
            return _mensagens.ContainsKey(messageId);
        }

        // Marca uma mensagem como processada agora
        public void MarcarComoProcessada(string messageId)
        {
            _mensagens[messageId] = DateTime.Now;
        }

        // Remove mensagens antigas (mais de 10 minutos) para liberar memória
        private void LimparMensagensAntigas()
        {
            var agora = DateTime.Now;
            var expiradas = _mensagens
                .Where(par => (agora - par.Value).TotalMinutes > 10)
                .Select(par => par.Key)
                .ToList();

            foreach (var key in expiradas)
            {
                _mensagens.TryRemove(key, out _);
            }
        }
    }
}
