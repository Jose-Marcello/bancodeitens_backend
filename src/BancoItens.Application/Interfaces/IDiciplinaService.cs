using BancoItens.Domain.Models;

namespace BancoDeItens.Application.Interfaces
{
    // Interface que define o contrato da camada de Serviço/Regra de Negócio para Questões.
    public interface IDisciplinaService
    {
        // Obtém todas as questões.
        Task<IEnumerable<Questao>> GetAllDisciplinasAsync();

        // Adiciona uma nova questão, incluindo validações ou lógica de negócio, se necessário.
        Task AddDisciplinaAsync(Disciplina disciplina);
    }
}