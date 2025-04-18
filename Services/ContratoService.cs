using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using AgroChainSync.Api.Data;
using AgroChainSync.Api.Models;

namespace AgroChainSync.Api.Services
{
    public class ContratoService
    {
        private readonly AgroContext _context;

        public ContratoService(AgroContext context)
        {
            _context = context;
        }

        // ✅ Retorna o contrato mais recente por CPF (usado para o comando "comprovante")
        public async Task<Contrato?> BuscarPorCpf(string cpf)
        {
            return await _context.Contratos
                .Where(c => c.Cpf == cpf)
                .OrderByDescending(c => c.Id) // ordena para pegar o mais novo
                .FirstOrDefaultAsync();
        }

        // ✅ Retorna todos os contratos do CPF (usado para o comando "listar")
        public async Task<List<Contrato>> BuscarTodosPorCpf(string cpf)
        {
            return await _context.Contratos
                .Where(c => c.Cpf == cpf)
                .OrderByDescending(c => c.Id) // ordena do mais novo pro mais antigo
                .ToListAsync();
        }
    }
}
