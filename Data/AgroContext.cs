using Microsoft.EntityFrameworkCore;
using AgroChainSync.Api.Models;

namespace AgroChainSync.Api.Data
{
    public class AgroContext : DbContext
    {
        public AgroContext(DbContextOptions<AgroContext> options) : base(options) { }

        public DbSet<Contrato> Contratos { get; set; }
    }
}
