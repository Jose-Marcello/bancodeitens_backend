using BancoDeItens.Domain.Interfaces;
using BancoItens.Domain.Interfaces;
using BancoItens.Domain.Models;
using Microsoft.Extensions.Logging;

namespace BancoItens.Infrastructure.Data.Repositories
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