using BancoDeItensWebApi.Data;
using BancoDeItensWebApi.Interfaces;
using BancoDeItensWebApi.Models;
using Microsoft.EntityFrameworkCore;

namespace BancoDeItensWebApi.Repositories
{
    // Implementação concreta do repositório, lidando com o acesso ao PostgreSql.
    public class QuestaoRepository : IQuestaoRepository
    {
        private readonly BancoDeItensContext _context;

        // Injeção de Dependência do DbContext
        public QuestaoRepository(BancoDeItensContext context)
        {
            _context = context;
        }

        // Implementação para obter todas as questões de forma assíncrona.
        public async Task<IEnumerable<Questao>> GetAllAsync()
        {
            return await _context.Questoes.ToListAsync();
        }

        // Implementação para adicionar uma nova questão de forma assíncrona.
        public async Task AddAsync(Questao questao)
        {
            if (questao == null)
            {
                throw new ArgumentNullException(nameof(questao));
            }
            // Adiciona a questão ao contexto
            _context.Questoes.Add(questao);
            // Salva as mudanças no banco de dados
            await _context.SaveChangesAsync();
        }
    }
}