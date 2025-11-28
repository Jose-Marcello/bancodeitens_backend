// BancoItens.Infrastructure/Data/Repository/Repository.cs
using BancoDeItens.Domain.Models;
//using BancoDeItens.Infrastructure.Data;
using BancoDeItens.Infrastructure.Data.Repositories;
using Microsoft.Extensions.Logging;

namespace BancoDeItens.Infrastructure.Data.Repository
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