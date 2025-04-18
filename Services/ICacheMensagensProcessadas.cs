namespace AgroChainSync.Api.Services
{
    public interface ICacheMensagensProcessadas
    {
        // Verifica se uma mensagem com esse ID já foi processada
        bool JaProcessada(string messageId);

        // Marca a mensagem como processada
        void MarcarComoProcessada(string messageId);
    }
}
