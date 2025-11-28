using BancoDeItens.Application.Interfaces;
using BancoDeItens.Domain.Interfaces;
using BancoDeItens.Domain.Models;
using Microsoft.Extensions.Logging;

namespace BancoDeItens.Application.Services
{
    // Crie a interface IDisciplinaService primeiro no Domain/Interfaces
    public class DisciplinaService : IDisciplinaService
    {
        private readonly IUnitOfWork _uow;
        private readonly ILogger<DisciplinaService> _logger;
        private readonly IDisciplinaRepository _disciplinaRepository;


        public DisciplinaService(IUnitOfWork uow, 
               ILogger<DisciplinaService> logger,
               IDisciplinaRepository disciplinaRepository)
        {
            _uow = uow;
            _logger = logger;
            _disciplinaRepository = disciplinaRepository;
            
        }

        public async Task AddDisciplinaAsync(Disciplina disciplina)
        {
            // 1. Obtém o repositório através do método genérico do UoW
            var repo = _uow.GetRepository<Disciplina>();

            // 2. Lógica de negócio/validação aqui (se houver)

            await repo.Adicionar(disciplina);

            // 3. Salva a transação de forma atômica
            await _uow.CommitAsync();
        }

        public async Task<IEnumerable<Disciplina>> GetAllDisciplinasAsync()
        {
            // O ideal aqui seria incluir o Eager Loading da Disciplina, mas o Repositório se encarrega.
            return await _disciplinaRepository.ObterTodos();
        }


    }
}