using BancoDeItens.Domain.Interfaces;
using BancoItens.Domain.Interfaces;
using BancoItens.Domain.Models;
using BancoItens.Infrastructure.Data;
using Microsoft.Extensions.Logging;

namespace BancoItens.Infrastructure.Data.Repositories
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