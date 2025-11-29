using BancoItens.Domain.Models;
using BancoItens.Infrastructure.Data.Repositories;
using Microsoft.Extensions.Logging;

namespace BancoItens.Infrastructure.Data.Repository
{
    // Esta classe herda de RepositoryBase e a torna CONCRETA (não abstrata)
    public class Repository<TEntity> : RepositoryBase<TEntity> where TEntity : Entity, new()
    {
        // Chama o construtor da base
        public Repository(BancoDeItensContext db, ILogger<TEntity> logger)
            : base(db, logger)
        {
        }
    }
}