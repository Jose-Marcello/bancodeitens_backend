using BancoDeItens.Domain.Interfaces;
using BancoDeItens.Domain.Models;
//ing BancoDeItens.Infrastructure.Data.Context;
using Microsoft.Extensions.Logging;

namespace BancoDeItens.Infrastructure.Data.Repositories
{
    
    public class DisciplinaRepository : RepositoryBase<Disciplina>, IDisciplinaRepository
    {
        // Simplesmente chama o construtor da base
        public DisciplinaRepository(BancoDeItensContext db, ILogger<Disciplina> logger)
            : base(db, logger)
        {
        }
    }
}