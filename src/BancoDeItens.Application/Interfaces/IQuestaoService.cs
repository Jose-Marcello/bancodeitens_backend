using BancoDeItens.Domain.Models;

namespace BancoDeItens.Application.Interfaces
{
    // Interface que define o contrato da camada de Serviço/Regra de Negócio para Questões.
    public interface IQuestaoService
    {
        // Obtém todas as questões.
        Task<IEnumerable<Questao>> GetAllQuestoesAsync();

        // Adiciona uma nova questão, incluindo validações ou lógica de negócio, se necessário.
        Task AddQuestaoAsync(Questao questao);
    }
}