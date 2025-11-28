using BancoDeItens.Domain.Interfaces;
using BancoDeItens.Domain.Models;
using BancoDeItens.Infrastructure.Data;
using Microsoft.Extensions.Logging;

namespace BancoDeItens.Infrastructure.Data.Repositories
{
    public class QuestaoRepository : RepositoryBase<Questao>, IQuestaoRepository
    {
        // Chama o construtor da base, passando o DbContext e o Logger
        public QuestaoRepository(BancoDeItensContext db, ILogger<Questao> logger)
            : base(db, logger)
        {
        }
    }
}