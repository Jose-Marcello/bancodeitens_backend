using BancoDeItensWebApi.Data;
using BancoDeItensWebApi.Interfaces;
using BancoDeItensWebApi.Models;
using Microsoft.EntityFrameworkCore;

namespace BancoDeItensWebApi.Repositories
{
    // Implementação para acesso a dados da entidade Disciplina.
    public class DisciplinaRepository : IDisciplinaRepository
    {
        private readonly BancoDeItensContext _context;

        public DisciplinaRepository(BancoDeItensContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<QuestaoCadastroDto>> GetAllAsync()
        {
            // Retorna todas as Disciplinas
            return await _context.Disciplinas.ToListAsync();
        }
    }
}