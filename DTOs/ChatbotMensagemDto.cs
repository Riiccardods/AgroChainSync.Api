namespace AgroChainSync.Api.DTOs
{
    public class ChatbotMensagemDto
    {
        public Mensagem message { get; set; } = null!;
    }

    public class Mensagem
    {
        public string text { get; set; } = null!;
        public string from { get; set; } = null!;
    }
}
