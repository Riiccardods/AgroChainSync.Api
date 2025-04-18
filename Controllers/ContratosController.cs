using Microsoft.AspNetCore.Mvc;
using AgroChainSync.Api.Data;
using AgroChainSync.Api.DTOs;
using AgroChainSync.Api.Models;
using AgroChainSync.Api.Services;

namespace AgroChainSync.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ContratosController : ControllerBase
    {
        private readonly AgroContext _context;

        public ContratosController(AgroContext context)
        {
            _context = context;
        }

        // ✅ Endpoint para criar um novo contrato
        [HttpPost]
        public async Task<IActionResult> CriarContrato([FromBody] ContratoCreateDto dto)
        {
            var contrato = new Contrato
            {
                NomeCliente = dto.NomeCliente,
                Cpf = dto.Cpf,
                DescricaoMaquina = dto.DescricaoMaquina,
                DataInicio = dto.DataInicio,
                DataFim = dto.DataFim,
                RenovacaoAutomatica = dto.RenovacaoAutomatica
            };

            _context.Contratos.Add(contrato);
            await _context.SaveChangesAsync();

            return CreatedAtAction(null, new { id = contrato.Id }, contrato);
        }

        // ✅ Endpoint para simular o registro na blockchain (grava o hash no blockchain_log.txt)
        [HttpPost("{id}/registrar-hash")]
        public IActionResult RegistrarHash(int id, [FromServices] BlockchainService blockchainService)
        {
            blockchainService.RegistrarHashNaBlockchain(id);
            return Ok($"✅ Hash registrado com sucesso para o contrato {id}.");
        }
    }
}
